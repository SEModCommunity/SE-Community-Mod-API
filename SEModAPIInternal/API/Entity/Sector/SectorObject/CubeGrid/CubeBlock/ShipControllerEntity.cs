using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

using VRageMath;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract(Name = "ShipControllerEntityProxy")]
	public class ShipControllerEntity : TerminalBlockEntity
	{
		#region "Attributes"

		private ShipControllerNetworkManager m_networkManager;
		private CharacterEntity m_pilot;
		private bool m_weaponStatus;

		public static string ShipControllerEntityNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string ShipControllerEntityClass = "12BACAB3471C8707CE7420AE0465548C";

		public static string ShipControllerEntityGetNetworkManager = "4D19E6CD06284069B97E08353C984ABB";
		public static string ShipControllerEntityGetPilotEntityMethod = "7214F843D41AA5091768E08C1801E5FC";
		public static string ShipControllerEntitySetPilotEntityMethod = "AC280CA879823319A66F3C71D6478297";

		#endregion

		#region "Constructors and Initializers"

		public ShipControllerEntity(CubeGridEntity parent, MyObjectBuilder_ShipController definition)
			: base(parent, definition)
		{
		}

		public ShipControllerEntity(CubeGridEntity parent, MyObjectBuilder_ShipController definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
			m_networkManager = new ShipControllerNetworkManager(GetShipControllerNetworkManager(), this);
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Ship Controller")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_ShipController ObjectBuilder
		{
			get
			{
				return (MyObjectBuilder_ShipController)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category("Ship Controller")]
		[ReadOnly(true)]
		public bool ControlThrusters
		{
			get { return ObjectBuilder.ControlThrusters; }
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category("Ship Controller")]
		[ReadOnly(true)]
		public bool ControlWheels
		{
			get { return ObjectBuilder.ControlWheels; }
			private set
			{
				//Do nothing!
			}
		}
		/*
		[IgnoreDataMember]
		[Category("Ship Controller")]
		[ReadOnly(true)]
		public MyObjectBuilder_AutopilotBase Autopilot
		{
			get { return ObjectBuilder.Autopilot; }
			private set
			{
				//Do nothing!
			}
		}

		[IgnoreDataMember]
		[Category("Ship Controller")]
		[Browsable(false)]
		[ReadOnly(true)]
		[Obsolete]
		public MyObjectBuilder_Character Pilot
		{
			get { return ObjectBuilder.Pilot; }
			private set
			{
				//Do nothing!
			}
		}*/

		[IgnoreDataMember]
		[Category("Ship Controller")]
		[Browsable(false)]
		public CharacterEntity PilotEntity
		{
			get
			{
				if (BackingObject == null || ActualObject == null)
					return null;

				Object backingPilot = GetPilotEntity();
				if (backingPilot == null)
					return null;

				if (m_pilot == null)
				{
					try
					{
						MyObjectBuilder_Character objectBuilder = (MyObjectBuilder_Character)BaseEntity.GetObjectBuilder(backingPilot);
						m_pilot = new CharacterEntity(objectBuilder, backingPilot);
					}
					catch (Exception ex)
					{
						LogManager.ErrorLog.WriteLine(ex);
					}
				}

				if (m_pilot != null)
				{
					try
					{
						if (m_pilot.BackingObject != backingPilot)
						{
							MyObjectBuilder_Character objectBuilder = (MyObjectBuilder_Character)BaseEntity.GetObjectBuilder(backingPilot);
							m_pilot.BackingObject = backingPilot;
							m_pilot.ObjectBuilder = objectBuilder;
						}
					}
					catch (Exception ex)
					{
						LogManager.ErrorLog.WriteLine(ex);
					}
				}

				return m_pilot;
			}
			set
			{
				m_pilot = value;
				Changed = true;

				if (BackingObject != null && ActualObject != null)
				{
					Action action = InternalUpdatePilotEntity;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[IgnoreDataMember]
		[Category("Ship Controller")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal ShipControllerNetworkManager NetworkManager
		{
			get { return m_networkManager; }
		}

		#endregion

		#region "Methods"

		new public static bool ReflectionUnitTest()
		{
			try
			{
				bool result = true;

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(ShipControllerEntityNamespace, ShipControllerEntityClass);
				if (type == null)
					throw new Exception("Could not find type for ShipControllerEntity");

				result &= BaseObject.HasMethod(type, ShipControllerEntityGetNetworkManager);
				result &= BaseObject.HasMethod(type, ShipControllerEntityGetPilotEntityMethod);
				result &= BaseObject.HasMethod(type, ShipControllerEntitySetPilotEntityMethod);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine(ex);
				return false;
			}
		}

		protected Object GetShipControllerNetworkManager()
		{
			Object result = InvokeEntityMethod(ActualObject, ShipControllerEntityGetNetworkManager);
			return result;
		}

		protected Object GetPilotEntity()
		{
			Object result = InvokeEntityMethod(ActualObject, ShipControllerEntityGetPilotEntityMethod);
			return result;
		}

		protected void InternalUpdatePilotEntity()
		{
			if (m_pilot == null || m_pilot.BackingObject == null)
				return;

			BaseObject.InvokeEntityMethod(ActualObject, ShipControllerEntitySetPilotEntityMethod, new object[] { m_pilot.BackingObject, Type.Missing, Type.Missing });
		}

		#endregion
	}

	public class ShipControllerNetworkManager
	{
		#region "Attributes"

		private Object m_networkManager;
		private ShipControllerEntity m_parent;

		private static bool m_isRegistered;

		//Packets
		//2480 - Pilot Relative World PositionOrientation
		//2481 - Dampeners On/Off
		//2487 - Autopilot Data
		//2488 - Thruster Power On/Off
		//2489 - Motor Handbrake On/Off

		public static string ShipControllerNetworkManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string ShipControllerNetworkManagerClass = "FC3A3372AD1F9E2E193FE3F7683D7DEF";

		public static string ShipControllerNetworkManagerBroadcastDampenersStatus = "7D17A6F76089A3756ED081F5CCB0E739";

		#endregion

		#region "Constructors and Initializers"

		public ShipControllerNetworkManager(Object networkManager, ShipControllerEntity parent)
		{
			m_networkManager = networkManager;
			m_parent = parent;

			Action action = RegisterPacketHandlers;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		#endregion

		#region "Properties"

		public Object BackingObject
		{
			get { return m_networkManager; }
		}

		public static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(ShipControllerNetworkManagerNamespace, ShipControllerNetworkManagerClass);
				return type;
			}
		}

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				bool result = true;
				Type type = InternalType;
				if (type == null)
					throw new Exception("Could not find type for ShipControllerNetworkManager");

				result &= BaseObject.HasMethod(type, ShipControllerNetworkManagerBroadcastDampenersStatus);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine(ex);
				return false;
			}
		}

		internal void BroadcastDampenersStatus(bool status)
		{
			BaseObject.InvokeEntityMethod(BackingObject, ShipControllerNetworkManagerBroadcastDampenersStatus, new object[] { status });
		}

		protected static void RegisterPacketHandlers()
		{
			try
			{
				if (m_isRegistered)
					return;
				/*
				Type packetType = InternalType.GetNestedType("8368ACD3E728CDA04FE741CDC05B1D16", BindingFlags.Public | BindingFlags.NonPublic);
				MethodInfo method = typeof(ShipControllerNetworkManager).GetMethod("ReceivePositionOrientationPacket", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
				bool result = NetworkManager.RegisterCustomPacketHandler(PacketRegistrationType.Instance, packetType, method, InternalType);
				if (!result)
					return;
				*/
				m_isRegistered = true;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected static void ReceivePositionOrientationPacket<T>(Object instanceNetManager, ref T packet, Object masterNetManager) where T : struct
		{
			try
			{
				//For now we ignore any inbound packets that set the positionorientation
				//This prevents the clients from having any control over the actual ship position
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
