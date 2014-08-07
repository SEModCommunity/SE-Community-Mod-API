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
	[DataContract(Name = "GyroEntityProxy")]
	public class GyroEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private GyroNetworkManager m_networkManager;

		public static string GyroNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string GyroClass = "C33277D56ED00A2772C484D143A9CB42";

		public static string GyroSetOverrideMethod = "248F13F6DFA54470FE81204B119B7DE1";
		public static string GyroSetPowerMethod = "46001A635CDAA4591E15661BA2083B75";
		public static string GyroSetTargetAngularVelocityMethod = "5E85AE802570A4C0028157B436BDF0B5";

		public static string GyroNetworkManagerField = "172E1F246A16951E55BC16D121BE2667";

		#endregion

		#region "Constructors and Intializers"

		public GyroEntity(CubeGridEntity parent, MyObjectBuilder_Gyro definition)
			: base(parent, definition)
		{
		}

		public GyroEntity(CubeGridEntity parent, MyObjectBuilder_Gyro definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
			m_networkManager = new GyroNetworkManager(this, InternalGetGyroNetworkManager());
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Gyro")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_Gyro ObjectBuilder
		{
			get
			{
				MyObjectBuilder_Gyro gyro = (MyObjectBuilder_Gyro)base.ObjectBuilder;

				return gyro;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category("Gyro")]
		[Browsable(true)]
		[ReadOnly(false)]
		public bool GyroOverride
		{
			get { return ObjectBuilder.GyroOverride; }
			set
			{
				if (ObjectBuilder.GyroOverride == value) return;
				ObjectBuilder.GyroOverride = value;
				Changed = true;

				if (BackingObject != null && ActualObject != null)
				{
					Action action = InternalUpdateGyroOverride;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[DataMember]
		[Category("Gyro")]
		[Browsable(true)]
		[ReadOnly(false)]
		public float GyroPower
		{
			get { return ObjectBuilder.GyroPower; }
			set
			{
				if (ObjectBuilder.GyroPower == value) return;
				ObjectBuilder.GyroPower = value;
				Changed = true;

				if (BackingObject != null && ActualObject != null)
				{
					Action action = InternalUpdateGyroPower;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[DataMember]
		[Category("Gyro")]
		[Browsable(true)]
		[ReadOnly(false)]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public Vector3Wrapper TargetAngularVelocity
		{
			get { return (Vector3Wrapper)ObjectBuilder.TargetAngularVelocity; }
			set
			{
				if ((Vector3)ObjectBuilder.TargetAngularVelocity == (Vector3)value) return;
				ObjectBuilder.TargetAngularVelocity = value;
				Changed = true;

				if (BackingObject != null && ActualObject != null)
				{
					Action action = InternalUpdateTargetAngularVelocity;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				bool result = true;

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(GyroNamespace, GyroClass);
				if (type == null)
					throw new Exception("Could not find internal type for GyroEntity");
				result &= HasMethod(type, GyroSetOverrideMethod);
				result &= HasMethod(type, GyroSetPowerMethod);
				result &= HasMethod(type, GyroSetTargetAngularVelocityMethod);
				result &= HasField(type, GyroNetworkManagerField);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		#region "Internal"

		protected Object InternalGetGyroNetworkManager()
		{
			try
			{
				FieldInfo field = GetEntityField(ActualObject, GyroNetworkManagerField);
				Object result = field.GetValue(ActualObject);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		protected void InternalUpdateGyroOverride()
		{
			InvokeEntityMethod(ActualObject, GyroSetOverrideMethod, new object[] { GyroOverride });
			m_networkManager.BroadcastOverride();
		}

		protected void InternalUpdateGyroPower()
		{
			InvokeEntityMethod(ActualObject, GyroSetPowerMethod, new object[] { GyroPower });
			m_networkManager.BroadcastPower();
		}

		protected void InternalUpdateTargetAngularVelocity()
		{
			InvokeEntityMethod(ActualObject, GyroSetTargetAngularVelocityMethod, new object[] { (Vector3)TargetAngularVelocity });
			m_networkManager.BroadcastTargetAngularVelocity();
		}

		#endregion

		#endregion
	}

	public class GyroNetworkManager
	{
		#region "Attributes"

		private GyroEntity m_parent;
		private Object m_backingObject;

		public static string GyroNetworkManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string GyroNetworkManagerClass = "B4646791D7A57BE4E2EE21A1F22A4364";

		//Packet ID 7587
		public static string GyroNetworkManagerBroadcastOverrideMethod = "42AE4A855B87970C577E7D3B84925D3D";
		//Packet ID 7586
		public static string GyroNetworkManagerBroadcastPowerMethod = "4D6A36FFABF6AE6DA2E92DCA0ADBB5FD";
		//Packet ID 7588
		public static string GyroNetworkManagerBroadcastTargetAngularVelocityMethod = "8948F689D38487B17CF831F240CEEF08";

		#endregion

		#region "Constructors and Initializers"

		public GyroNetworkManager(GyroEntity parent, Object backingObject)
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

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(GyroNetworkManagerNamespace, GyroNetworkManagerClass);
				if (type == null)
					throw new Exception("Could not find internal type for GyroNetworkManager");
				result &= BaseObject.HasMethod(type, GyroNetworkManagerBroadcastOverrideMethod);
				result &= BaseObject.HasMethod(type, GyroNetworkManagerBroadcastPowerMethod);
				result &= BaseObject.HasMethod(type, GyroNetworkManagerBroadcastTargetAngularVelocityMethod);

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

		public void BroadcastPower()
		{
			Action action = InternalBroadcastPower;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		public void BroadcastTargetAngularVelocity()
		{
			Action action = InternalBroadcastTargetAngularVelocity;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		#region "Internal"

		protected void InternalBroadcastOverride()
		{
			BaseObject.InvokeEntityMethod(m_backingObject, GyroNetworkManagerBroadcastOverrideMethod, new object[] { m_parent.GyroOverride });
		}

		protected void InternalBroadcastPower()
		{
			BaseObject.InvokeEntityMethod(m_backingObject, GyroNetworkManagerBroadcastPowerMethod, new object[] { m_parent.GyroPower });
		}

		protected void InternalBroadcastTargetAngularVelocity()
		{
			Vector3 newTarget = (Vector3)m_parent.TargetAngularVelocity;
			BaseObject.InvokeEntityMethod(m_backingObject, GyroNetworkManagerBroadcastTargetAngularVelocityMethod, new object[] { newTarget });
		}

		#endregion

		#endregion
	}
}
