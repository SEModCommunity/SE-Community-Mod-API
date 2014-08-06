using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

using Sandbox.Common.ObjectBuilders;

using SEModAPI.API;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

using VRageMath;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract(Name = "ThrustEntityProxy")]
	public class ThrustEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private float m_thrustOverride;
		private ThrustNetworkManager m_networkManager;

		public static string ThrustNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string ThrustClass = "A52459FBA230B557AC325120832EB494";

		public static string ThrustGetOverrideMethod = "1223DF2E5F66BA5F65ADACCA781A5C96";
		public static string ThrustSetOverrideMethod = "93422020A3B0D5FF9ABC998955038E04";
		public static string ThrustGetMaxThrustVectorMethod = "8243DD42600709719ECFE7B7BEA0AAE6";
		public static string ThrustGetMaxPowerConsumptionMethod = "7C636D47F30E12E7A784FEAF91430C12";
		public static string ThrustGetMinPowerConsumptionMethod = "A86E949F21293FFCCC0A736D01F85167";

		public static string ThrustNetManagerField = "8F27FD71A1830B00FC6F01DDF7E6D795";

		//Note: The following fields exist but are not broadcast and as such setting these on the server will do nothing client-side
		public static string ThrustFlameColorField = "3BB80065D0377A358D2F75331BF07A6D";
		public static string ThrustLightField = "079E76305C1B63982C61439EDDB9D211";
		public static string ThrustFlameScaleCoefficientField = "5912C868C1061CCE7788DC17F8FDE754";

		//Thrust flame scale coefficient values:
		//LargeShip-Large: 700
		//LargeShip-Small: 500
		//SmallShip-Large: 300
		//SmallShip-Small: 200

		#endregion

		#region "Constructors and Intializers"

		public ThrustEntity(CubeGridEntity parent, MyObjectBuilder_Thrust definition)
			: base(parent, definition)
		{
		}

		public ThrustEntity(CubeGridEntity parent, MyObjectBuilder_Thrust definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
			m_thrustOverride = 0;
			m_networkManager = new ThrustNetworkManager(this, InternalGetThrustNetManager());
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Thrust")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_Thrust ObjectBuilder
		{
			get
			{
				MyObjectBuilder_Thrust thrust = (MyObjectBuilder_Thrust)base.ObjectBuilder;


				return thrust;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category("Thrust")]
		[Browsable(true)]
		[ReadOnly(false)]
		public float Override
		{
			get
			{
				if (BackingObject == null || ActualObject == null)
					return m_thrustOverride;

				return InternalGetThrustOverride();
			}
			set
			{
				m_thrustOverride = value;
				Changed = true;

				if (BackingObject != null && ActualObject != null)
				{
					Action action = InternalUpdateOverride;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[DataMember]
		[Category("Thrust")]
		[Browsable(true)]
		[ReadOnly(true)]
		public Vector3Wrapper MaxThrustVector
		{
			get
			{
				if (BackingObject == null || ActualObject == null)
					return Vector3.Zero;

				return InternalGetMaxThrustVector();
			}
			private set
			{
				//Do nothing!
			}
		}

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				bool result = true;

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(ThrustNamespace, ThrustClass);
				if (type == null)
					throw new Exception("Could not find internal type for ThrustEntity");
				result &= HasMethod(type, ThrustGetOverrideMethod);
				result &= HasMethod(type, ThrustSetOverrideMethod);
				result &= HasMethod(type, ThrustGetMaxThrustVectorMethod);
				result &= HasMethod(type, ThrustGetMaxPowerConsumptionMethod);
				result &= HasMethod(type, ThrustGetMinPowerConsumptionMethod);
				result &= HasField(type, ThrustNetManagerField);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		#region "Internal"

		protected Object InternalGetThrustNetManager()
		{
			try
			{
				FieldInfo field = GetEntityField(ActualObject, ThrustNetManagerField);
				Object result = field.GetValue(ActualObject);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		protected float InternalGetThrustOverride()
		{
			float result = (float)InvokeEntityMethod(ActualObject, ThrustGetOverrideMethod);

			return result;
		}

		protected Vector3 InternalGetMaxThrustVector()
		{
			Vector3 result = (Vector3)InvokeEntityMethod(ActualObject, ThrustGetMaxThrustVectorMethod);

			return result;
		}

		protected void InternalUpdateOverride()
		{
			InvokeEntityMethod(ActualObject, ThrustSetOverrideMethod, new object[] { m_thrustOverride });
			m_networkManager.BroadcastOverride();
		}

		#endregion

		#endregion
	}

	public class ThrustNetworkManager
	{
		#region "Attributes"

		private ThrustEntity m_parent;
		private Object m_backingObject;

		public static string ThrustNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string ThrustNetManagerClass = "648783FEE567EB6633169761E312362D";

		public static string ThrustNetManagerBroadcastOverrideMethod = "836153C49F86AF1526ABA97002D09721";

		//Packet ID 7416
		public static string ThrustNetManagerOverridePacket = "330002FC250DECBB7D7A5F94ABC33BB0";

		#endregion

		#region "Constructors and Intializers"

		public ThrustNetworkManager(ThrustEntity parent, Object backingObject)
		{
			m_parent = parent;
			m_backingObject = backingObject;
		}

		#endregion

		#region "Properties"
		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				bool result = true;

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(ThrustNetManagerNamespace, ThrustNetManagerClass);
				if (type == null)
					throw new Exception("Could not find network manager type for ThrustEntity");
				result &= BaseObject.HasMethod(type, ThrustNetManagerBroadcastOverrideMethod);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		public void BroadcastOverride()
		{
			Action action = InternalBroadcastOverride;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		#region "Internal"

		protected void InternalBroadcastOverride()
		{
			float thrustOverride = m_parent.Override;
			BaseObject.InvokeEntityMethod(m_backingObject, ThrustNetManagerBroadcastOverrideMethod, new object[] { thrustOverride });
		}

		#endregion

		#endregion
	}
}
