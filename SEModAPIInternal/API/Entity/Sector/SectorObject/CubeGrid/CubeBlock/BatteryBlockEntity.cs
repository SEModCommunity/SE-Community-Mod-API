using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

using Sandbox.Definitions;
using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Utility;
using SEModAPIInternal.Support;

using VRage.Serialization;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract(Name = "BatteryBlockEntityProxy")]
	public class BatteryBlockEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private BatteryBlockNetworkManager m_batteryBlockNetManager;

		private float m_maxPowerOutput;
		private float m_maxStoredPower;

		//Internal class
		public static string BatteryBlockNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string BatteryBlockClass = "711CB30D2043393F07630CD237B5EFBF";

		//Internal methods
		public static string BatteryBlockGetCurrentStoredPowerMethod = "82DBD55631B1D9694F1DCB5BFF88AB5B";
		public static string BatteryBlockSetCurrentStoredPowerMethod = "365694972F163426A27531B867041ABB";
		public static string BatteryBlockGetMaxStoredPowerMethod = "1E1C89D073DDC026426B44820B1A6286";
		public static string BatteryBlockSetMaxStoredPowerMethod = "51188413AE93A8E2B2375B7721F1A3FC";
		public static string BatteryBlockGetProducerEnabledMethod = "36B457125A54787901017D24BD0E3346";
		public static string BatteryBlockSetProducerEnabledMethod = "5538173B5047FC438226267C0088356E";
		public static string BatteryBlockGetSemiautoEnabledMethod = "19312D5BF11FBC0A682B613E21621BA6";
		public static string BatteryBlockSetSemiautoEnabledMethod = "A3BEE5A757F096951F158F9FFF5A878A";

		//Internal fields
		public static string BatteryBlockCurrentStoredPowerField = "736E72768436E8A7C1F33EF1F4396B9E";
		public static string BatteryBlockMaxStoredPowerField = "3E888DF7D4F5C207088050DF6CA348D5";
		public static string BatteryBlockProducerEnabledField = "5CE4521F11C6B1D64721848D226F15BF";
		public static string BatteryBlockSemiautoEnabledField = "61505AAA6C86342099EFC9D89532BBE7";
		public static string BatteryBlockBatteryDefinitionField = "F0C59D70E13560B7212CEF8CF082A67B";
		public static string BatteryBlockNetManagerField = "E93BD8EF419C322C547231F9BF991090";

		#endregion

		#region "Constructors and Initializers"

		public BatteryBlockEntity(CubeGridEntity parent, MyObjectBuilder_BatteryBlock definition)
			: base(parent, definition)
		{
		}

		public BatteryBlockEntity(CubeGridEntity parent, MyObjectBuilder_BatteryBlock definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
			m_maxPowerOutput = 0;
			m_maxStoredPower = definition.MaxStoredPower;

			m_batteryBlockNetManager = new BatteryBlockNetworkManager(this, InternalGetNetManager());
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Battery Block")]
		[Browsable(false)]
		[ReadOnly(true)]
		new internal static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(BatteryBlockNamespace, BatteryBlockClass);
				return type;
			}
		}

		[IgnoreDataMember]
		[Category("Battery Block")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_BatteryBlock ObjectBuilder
		{
			get
			{
				MyObjectBuilder_BatteryBlock batteryBlock = (MyObjectBuilder_BatteryBlock)base.ObjectBuilder;

				batteryBlock.MaxStoredPower = m_maxStoredPower;

				return batteryBlock;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category("Battery Block")]
		public float CurrentStoredPower
		{
			get { return ObjectBuilder.CurrentStoredPower; }
			set
			{
				if (ObjectBuilder.CurrentStoredPower == value) return;
				ObjectBuilder.CurrentStoredPower = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateBatteryBlockCurrentStoredPower;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[DataMember]
		[Category("Battery Block")]
		public float MaxStoredPower
		{
			get
			{
				float maxStoredPower = 0;

				if (ActualObject != null)
				{
					maxStoredPower = (float)InvokeEntityMethod(ActualObject, BatteryBlockGetMaxStoredPowerMethod);
				}
				else
				{
					maxStoredPower = m_maxStoredPower;
				}

				return maxStoredPower;
			}
			set
			{
				if (m_maxStoredPower == value) return;
				m_maxStoredPower = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateBatteryBlockMaxStoredPower;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[DataMember]
		[Category("Battery Block")]
		public bool ProducerEnabled
		{
			get { return ObjectBuilder.ProducerEnabled; }
			set
			{
				if (ObjectBuilder.ProducerEnabled == value) return;
				ObjectBuilder.ProducerEnabled = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateBatteryBlockProducerEnabled;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[DataMember]
		[Category("Battery Block")]
		public bool SemiautoEnabled
		{
			get { return ObjectBuilder.SemiautoEnabled; }
			set
			{
				if (ObjectBuilder.SemiautoEnabled == value) return;
				ObjectBuilder.SemiautoEnabled = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateBatteryBlockSemiautoEnabled;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[DataMember]
		[Category("Battery Block")]
		public float RequiredPowerInput
		{
			get { return PowerReceiver.MaxRequiredInput; }
			set
			{
				if (PowerReceiver.MaxRequiredInput == value) return;
				PowerReceiver.MaxRequiredInput = value;
				Changed = true;
			}
		}

		[DataMember]
		[Category("Battery Block")]
		public float MaxPowerOutput
		{
			get { return m_maxPowerOutput; }
			set
			{
				if (m_maxPowerOutput == value) return;
				m_maxPowerOutput = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateBatteryBlockMaxPowerOutput;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[IgnoreDataMember]
		[Browsable(false)]
		[ReadOnly(true)]
		internal BatteryBlockNetworkManager BatteryNetManager
		{
			get { return m_batteryBlockNetManager; }
		}

		#endregion

		#region "Methods"

		new public static bool ReflectionUnitTest()
		{
			try
			{
				bool result = true;

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(BatteryBlockNamespace, BatteryBlockClass);
				if (type == null)
					throw new Exception("Could not find internal type for BatteryBlockEntity");

				result &= HasMethod(type, BatteryBlockGetCurrentStoredPowerMethod);
				result &= HasMethod(type, BatteryBlockSetCurrentStoredPowerMethod);
				result &= HasMethod(type, BatteryBlockGetMaxStoredPowerMethod);
				result &= HasMethod(type, BatteryBlockSetMaxStoredPowerMethod);
				result &= HasMethod(type, BatteryBlockGetProducerEnabledMethod);
				result &= HasMethod(type, BatteryBlockSetProducerEnabledMethod);
				result &= HasMethod(type, BatteryBlockGetSemiautoEnabledMethod);
				result &= HasMethod(type, BatteryBlockSetSemiautoEnabledMethod);

				result &= HasField(type, BatteryBlockCurrentStoredPowerField);
				result &= HasField(type, BatteryBlockMaxStoredPowerField);
				result &= HasField(type, BatteryBlockProducerEnabledField);
				result &= HasField(type, BatteryBlockSemiautoEnabledField);
				result &= HasField(type, BatteryBlockBatteryDefinitionField);
				result &= HasField(type, BatteryBlockNetManagerField);
				
				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		#region "Internal"

		protected override float InternalPowerReceiverCallback()
		{
			if(ProducerEnabled || (CurrentStoredPower / MaxStoredPower) >= 0.98)
			{
				return 0.0f;
			}
			else
			{
				return PowerReceiver.MaxRequiredInput;
			}
		}

		protected Object InternalGetNetManager()
		{
			try
			{
				FieldInfo field = GetEntityField(ActualObject, BatteryBlockNetManagerField);
				Object result = field.GetValue(ActualObject);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		protected void InternalUpdateBatteryBlockCurrentStoredPower()
		{
			try
			{
				InvokeEntityMethod(ActualObject, BatteryBlockSetCurrentStoredPowerMethod, new object[] { CurrentStoredPower });
				BatteryNetManager.BroadcastCurrentStoredPower();
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected void InternalUpdateBatteryBlockMaxStoredPower()
		{
			try
			{
				InvokeEntityMethod(ActualObject, BatteryBlockSetMaxStoredPowerMethod, new object[] { m_maxStoredPower });
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected void InternalUpdateBatteryBlockProducerEnabled()
		{
			try
			{
				InvokeEntityMethod(ActualObject, BatteryBlockSetProducerEnabledMethod, new object[] { ProducerEnabled });
				BatteryNetManager.BroadcastProducerEnabled();
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected void InternalUpdateBatteryBlockSemiautoEnabled()
		{
			try
			{
				InvokeEntityMethod(ActualObject, BatteryBlockSetSemiautoEnabledMethod, new object[] { SemiautoEnabled });
				BatteryNetManager.BroadcastSemiautoEnabled();
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected void InternalUpdateBatteryBlockMaxPowerOutput()
		{
			//TODO - Do stuff
		}

		#endregion

		#endregion
	}

	public class BatteryBlockNetworkManager
	{
		#region "Attributes"

		private BatteryBlockEntity m_parent;
		private Object m_backingObject;

		private static bool m_isRegistered;

		public static string BatteryBlockNetworkManagerNamespace = BatteryBlockEntity.BatteryBlockNamespace + "." + BatteryBlockEntity.BatteryBlockClass;
		public static string BatteryBlockNetworkManagerClass = "6704740496C47C5FDE69887798D17883";

		public static string BatteryBlockNetManagerBroadcastProducerEnabledMethod = "280D7AE8C0F523FF089618970C13B55B";
		public static string BatteryBlockNetManagerBroadcastCurrentStoredPowerMethod = "F512BA7EF29F6A8B7DE3D56BAAC0207B";
		public static string BatteryBlockNetManagerBroadcastSemiautoEnabledMethod = "72CE36DE9C0BAB6FEADA5D10CF5B867A";
		public static string BatteryBlockNetManagerCurrentPowerPacketReceiver = "F512BA7EF29F6A8B7DE3D56BAAC0207B";

		///////////////////////////////////////////////////////////////////////

		//Packets
		//1587 - CurrentStoredPower
		//1588 - ??
		//15870 - ProducerEnabled On/Off
		//15871 - SemiautoEnabled On/Off

		public static string BatteryBlockNetManagerCurrentStoredPowerPacketClass = "59DE66D2ECADE0929A1C776D7FA907E2";

		public static string BatteryBlockNetManagerCurrentStoredPowerPacketGetIdMethod = "BB64D4385310CACE0C4AB2810898C4CE";

		public static string BatteryBlockNetManagerCurrentStoredPowerPacketValueField = "ADC3AB91A03B31875821D57B8B718AF5";

		#endregion

		#region "Constructors and Initializers"

		public BatteryBlockNetworkManager(BatteryBlockEntity parent, Object backingObject)
		{
			m_parent = parent;
			m_backingObject = backingObject;

			Action action = RegisterPacketHandlers;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		#endregion

		#region "Properties"

		internal Object BackingObject
		{
			get { return m_backingObject; }
		}

		public static Type InternalType
		{
			get
			{
				Type type = BatteryBlockEntity.InternalType.GetNestedType(BatteryBlockNetworkManagerClass, BindingFlags.Public | BindingFlags.NonPublic);
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
					throw new Exception("Could not find internal type for BatteryBlockNetworkManager");
				result &= BaseObject.HasMethod(type, BatteryBlockNetManagerBroadcastProducerEnabledMethod);
				result &= BaseObject.HasMethod(type, BatteryBlockNetManagerBroadcastCurrentStoredPowerMethod);
				result &= BaseObject.HasMethod(type, BatteryBlockNetManagerBroadcastSemiautoEnabledMethod);

				Type packetType = InternalType.GetNestedType(BatteryBlockNetManagerCurrentStoredPowerPacketClass, BindingFlags.Public | BindingFlags.NonPublic);
				result &= BaseObject.HasMethod(packetType, BatteryBlockNetManagerCurrentStoredPowerPacketGetIdMethod);
				result &= BaseObject.HasField(packetType, BatteryBlockNetManagerCurrentStoredPowerPacketValueField);

				Type refPacketType = packetType.MakeByRefType();

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		public void BroadcastProducerEnabled()
		{
			BaseObject.InvokeEntityMethod(BackingObject, BatteryBlockNetManagerBroadcastProducerEnabledMethod, new object[] { m_parent.ProducerEnabled });
		}

		public void BroadcastCurrentStoredPower()
		{
			BaseObject.InvokeEntityMethod(BackingObject, BatteryBlockNetManagerBroadcastCurrentStoredPowerMethod, new object[] { m_parent.CurrentStoredPower });
		}

		public void BroadcastSemiautoEnabled()
		{
			BaseObject.InvokeEntityMethod(BackingObject, BatteryBlockNetManagerBroadcastSemiautoEnabledMethod, new object[] { m_parent.SemiautoEnabled });
		}

		protected static void RegisterPacketHandlers()
		{
			try
			{
				if (m_isRegistered)
					return;

				Type packetType = InternalType.GetNestedType(BatteryBlockNetManagerCurrentStoredPowerPacketClass, BindingFlags.Public | BindingFlags.NonPublic);
				MethodInfo method = typeof(BatteryBlockNetworkManager).GetMethod("ReceiveCurrentPowerPacket", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
				bool result = NetworkManager.RegisterCustomPacketHandler(PacketRegistrationType.Static, packetType, method, InternalType);
				if (!result)
					return;

				m_isRegistered = true;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected static void ReceiveCurrentPowerPacket<T>(ref T packet, Object netManager) where T : struct
		{
			try
			{
				object result = BaseObject.InvokeEntityMethod(packet, BatteryBlockNetManagerCurrentStoredPowerPacketGetIdMethod);
				if (result == null)
					return;
				long entityId = (long)result;
				BaseObject matchedEntity = GameEntityManager.GetEntity(entityId);
				if (matchedEntity == null)
					return;
				if (!(matchedEntity is BatteryBlockEntity))
					return;
				BatteryBlockEntity battery = (BatteryBlockEntity)matchedEntity;

				result = BaseObject.GetEntityFieldValue(packet, BatteryBlockNetManagerCurrentStoredPowerPacketValueField);
				if (result == null)
					return;
				float packetPowerLevel = (float)result;
				if (packetPowerLevel == 1.0f)
					return;

				BaseObject.SetEntityFieldValue(packet, BatteryBlockNetManagerCurrentStoredPowerPacketValueField, battery.CurrentStoredPower);

				Type refPacketType = packet.GetType().MakeByRefType();
				MethodInfo basePacketHandlerMethod = BaseObject.GetStaticMethod(InternalType, BatteryBlockNetManagerCurrentPowerPacketReceiver, new Type[] { refPacketType, netManager.GetType() });
				basePacketHandlerMethod.Invoke(null, new object[] { packet, netManager });
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
