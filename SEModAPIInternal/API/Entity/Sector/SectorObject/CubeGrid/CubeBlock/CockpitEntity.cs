using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;
using System.Reflection;
using VRageMath;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract(Name = "CockpitEntityProxy")]
	public class CockpitEntity : TerminalBlockEntity
	{
		#region "Attributes"

		private CockpitNetworkManager m_networkManager;
		private CharacterEntity m_pilot;
		private bool m_controlShip;
		private bool m_weaponStatus;

		public static string CockpitEntityNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string CockpitEntityClass = "0A875207E28B2C7707366CDD300684DF";

		public static string CockpitEntityGetNetworkManager = "FBCE4A2D52D8DBD116860A130829EED5";
		public static string CockpitEntityGetPilotEntityMethod = "799C3E3B295452F5723BE6E8288CC461";
		public static string CockpitEntitySetPilotEntityMethod = "1BB7956FA537A66315E07C562677018A";

		public static string CockpitEntityEnableShipControlField = "EB562350C4FA158AF77C44A73ABFA712";

		#endregion

		#region "Constructors and Initializers"

		public CockpitEntity(CubeGridEntity parent, MyObjectBuilder_Cockpit definition)
			: base(parent, definition)
		{
			m_controlShip = true;
		}

		public CockpitEntity(CubeGridEntity parent, MyObjectBuilder_Cockpit definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
			m_controlShip = true;
			m_networkManager = new CockpitNetworkManager(GetCockpitNetworkManager(), this);
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Cockpit")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_Cockpit ObjectBuilder
		{
			get
			{
				MyObjectBuilder_Cockpit objectBuilder = (MyObjectBuilder_Cockpit)base.ObjectBuilder;

				return objectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category("Cockpit")]
		public bool ControlShip
		{
			get
			{
				try
				{
					if (BackingObject == null || ActualObject == null)
						return m_controlShip;

					bool result = (bool)BaseObject.GetEntityFieldValue(ActualObject, CockpitEntityEnableShipControlField);
					return result;
				}
				catch (Exception ex)
				{
					LogManager.ErrorLog.WriteLine(ex);
					return true;
				}
			}
			set
			{
				if (m_controlShip == value) return;
				m_controlShip = value;
				Changed = true;

				if (BackingObject != null && ActualObject != null)
				{
					Action action = InternalUpdateShipControlEnabled;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[DataMember]
		[Category("Cockpit")]
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
		[Category("Cockpit")]
		[ReadOnly(true)]
		public bool ControlWheels
		{
			get { return ObjectBuilder.ControlWheels; }
			private set
			{
				//Do nothing!
			}
		}

		[IgnoreDataMember]
		[Category("Cockpit")]
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
		[Category("Cockpit")]
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
		}

		[IgnoreDataMember]
		[Category("Cockpit")]
		[Browsable(false)]
		public CharacterEntity PilotEntity
		{
			get
			{
				if (BackingObject == null || ActualObject == null)
					return null;

				Object backingPilot = GetCockpitPilotEntity();
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

		[DataMember]
		[Category("Cockpit")]
		[ReadOnly(true)]
		public bool IsPassengerSeat
		{
			get
			{
				if (ObjectBuilder.SubtypeName == "PassengerSeatLarge")
					return true;

				if (ObjectBuilder.SubtypeName == "PassengerSeatSmall")
					return true;

				return false;
			}
			private set
			{
				//Do nothing!
			}
		}

		[IgnoreDataMember]
		[Category("Cockpit")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal CockpitNetworkManager NetworkManager
		{
			get
			{
				if(m_networkManager == null)
					m_networkManager = new CockpitNetworkManager(GetCockpitNetworkManager(), this);

				return m_networkManager;
			}
		}

		#endregion

		#region "Methods"

		new public static bool ReflectionUnitTest()
		{
			try
			{
				bool result = true;
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(CockpitEntityNamespace, CockpitEntityClass);
				if (type == null)
					throw new Exception("Could not find type for CockpitEntity");
				result &= BaseObject.HasMethod(type, CockpitEntityGetNetworkManager);
				result &= BaseObject.HasMethod(type, CockpitEntityGetPilotEntityMethod);
				result &= BaseObject.HasMethod(type, CockpitEntitySetPilotEntityMethod);
				result &= BaseObject.HasField(type, CockpitEntityEnableShipControlField);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine(ex);
				return false;
			}
		}

		public void FireWeapons()
		{
			if (m_weaponStatus)
				return;

			m_weaponStatus = true;

			Action action = InternalFireWeapons;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		public void StopWeapons()
		{
			if (!m_weaponStatus)
				return;

			m_weaponStatus = false;

			Action action = InternalStopWeapons;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		protected Object GetCockpitNetworkManager()
		{
			Object result = InvokeEntityMethod(ActualObject, CockpitEntityGetNetworkManager);
			return result;
		}

		protected Object GetCockpitPilotEntity()
		{
			Object result = InvokeEntityMethod(ActualObject, CockpitEntityGetPilotEntityMethod);
			return result;
		}

		protected void InternalUpdateShipControlEnabled()
		{
			BaseObject.SetEntityFieldValue(ActualObject, CockpitEntityEnableShipControlField, m_controlShip);
		}

		protected void InternalUpdatePilotEntity()
		{
			if (m_pilot == null || m_pilot.BackingObject == null)
				return;

			BaseObject.InvokeEntityMethod(ActualObject, CockpitEntitySetPilotEntityMethod, new object[] { m_pilot.BackingObject });
		}

		protected void InternalFireWeapons()
		{
			NetworkManager.BroadcastWeaponActionOn();
		}

		protected void InternalStopWeapons()
		{
			NetworkManager.BroadcastWeaponActionOff();
		}

		#endregion
	}

	public class CockpitNetworkManager
	{
		#region "Attributes"

		private Object m_networkManager;
		private CockpitEntity m_parent;

		private static bool m_isRegistered;

		//Packets
		//2480 - Pilot Relative World PositionOrientation
		//2481 - Dampeners On/Off
		//2487 - Autopilot Data
		//2488 - Thruster Power On/Off
		//2489 - Motor Handbrake On/Off

		public static string CockpitNetworkManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string CockpitNetworkManagerClass = "F622141FE0D93611A749A5DB71DF471F";

		public static string CockpitNetworkManagerBroadcastDampenersStatus = "CF03B0F414BEA9134AFC06C2F31333E8";
		public static string CockpitNetworkManagerBroadcastWeaponActionOnMethod = "1D1345BB3B40C08D45FB81048E3D55FE";
		public static string CockpitNetworkManagerBroadcastWeaponActionOffMethod = "B3B9BD9A7BEB261390495E63154E5B85";

		#endregion

		#region "Constructors and Initializers"

		public CockpitNetworkManager(Object networkManager, CockpitEntity parent)
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
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(CockpitNetworkManagerNamespace, CockpitNetworkManagerClass);
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
					throw new Exception("Could not find type for CockpitNetworkManager");

				result &= BaseObject.HasMethod(type, CockpitNetworkManagerBroadcastDampenersStatus);
				result &= BaseObject.HasMethod(type, CockpitNetworkManagerBroadcastWeaponActionOnMethod);
				result &= BaseObject.HasMethod(type, CockpitNetworkManagerBroadcastWeaponActionOffMethod);

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
			BaseObject.InvokeEntityMethod(BackingObject, CockpitNetworkManagerBroadcastDampenersStatus, new object[] { status });
		}

		internal void BroadcastWeaponActionOn()
		{
			Vector3 weaponDirection = m_parent.Forward;
			BaseObject.InvokeEntityMethod(BackingObject, CockpitNetworkManagerBroadcastWeaponActionOnMethod, new object[] { weaponDirection, Type.Missing });
		}

		internal void BroadcastWeaponActionOff()
		{
			BaseObject.InvokeEntityMethod(BackingObject, CockpitNetworkManagerBroadcastWeaponActionOffMethod, new object[] { Type.Missing });
		}

		protected static void RegisterPacketHandlers()
		{
			try
			{
				if (m_isRegistered)
					return;
				/*
				Type packetType = InternalType.GetNestedType("8368ACD3E728CDA04FE741CDC05B1D16", BindingFlags.Public | BindingFlags.NonPublic);
				MethodInfo method = typeof(CockpitNetworkManager).GetMethod("ReceivePositionOrientationPacket", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
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
