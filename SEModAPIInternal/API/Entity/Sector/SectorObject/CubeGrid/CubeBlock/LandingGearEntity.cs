using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract(Name = "LandingGearEntityProxy")]
	public class LandingGearEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private LandingGearNetworkManager m_landingNetManager;

		private bool m_isLocked;
		private bool m_autoLockEnabled;
		private float m_brakeForce;

		public static string LandingGearNamespace = "6DDCED906C852CFDABA0B56B84D0BD74";
		public static string LandingGearClass = "5C73AAF1736F3AA9956574C6D9A2EEBE";

		public static string LandingGearGetAutoLockMethod = "2029452E34497F90579C6E212EF3B5C9";
		public static string LandingGearSetAutoLockMethod = "F542ACDC0D61EB46F733A5527CFFBE14";
		public static string LandingGearGetBrakeForceMethod = "A84CC3FC7B1C4CA0A631E34D2F024163";
		public static string LandingGearSetBrakeForceMethod = "013F45FD594F8A80D5952A7AC22A931E";

		public static string LandingGearIsLockedField = "00F45118D3A7F21253C28F4B11D1F70E";
		public static string LandingGearNetManagerField = "4D9CE737B011256C0232620C5234AAD4";

		#endregion

		#region "Constructors and Intializers"

		public LandingGearEntity(CubeGridEntity parent, MyObjectBuilder_LandingGear definition)
			: base(parent, definition)
		{
			m_isLocked = definition.IsLocked;
			m_autoLockEnabled = definition.AutoLock;
			m_brakeForce = definition.BrakeForce;
		}

		public LandingGearEntity(CubeGridEntity parent, MyObjectBuilder_LandingGear definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
			m_isLocked = definition.IsLocked;
			m_autoLockEnabled = definition.AutoLock;
			m_brakeForce = definition.BrakeForce;

			m_landingNetManager = new LandingGearNetworkManager(this, GetLandingGearNetManager());
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Landing Gear")]
		[Browsable(false)]
		[ReadOnly(true)]
		new internal static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(LandingGearNamespace, LandingGearClass);
				return type;
			}
		}

		[IgnoreDataMember]
		[Category("Landing Gear")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_LandingGear ObjectBuilder
		{
			get { return (MyObjectBuilder_LandingGear)base.ObjectBuilder; }
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category("Landing Gear")]
		public bool IsLocked
		{
			get
			{
				if (BackingObject == null || ActualObject == null)
					return ObjectBuilder.IsLocked;

				return GetIsLocked();
			}
			set
			{
				if (ObjectBuilder.IsLocked == value) return;
				ObjectBuilder.IsLocked = value;
				Changed = true;

				m_isLocked = value;

				if (BackingObject != null && ActualObject != null)
				{
					Action action = InternalUpdateIsLocked;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[DataMember]
		[Category("Landing Gear")]
		public bool AutoLock
		{
			get
			{
				if (BackingObject == null || ActualObject == null)
					return ObjectBuilder.AutoLock;

				return GetAutoLockEnabled();
			}
			set
			{
				if (ObjectBuilder.AutoLock == value) return;
				ObjectBuilder.AutoLock = value;
				Changed = true;

				m_autoLockEnabled = value;

				if (BackingObject != null && ActualObject != null)
				{
					Action action = InternalUpdateAutoLockEnabled;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[DataMember]
		[Category("Landing Gear")]
		public float BrakeForce
		{
			get
			{
				if (BackingObject == null || ActualObject == null)
					return ObjectBuilder.BrakeForce;

				return GetBrakeForce();
			}
			set
			{
				if (ObjectBuilder.BrakeForce == value) return;
				ObjectBuilder.BrakeForce = value;
				Changed = true;

				m_brakeForce = value;

				if (BackingObject != null && ActualObject != null)
				{
					Action action = InternalUpdateBrakeForce;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[IgnoreDataMember]
		internal LandingGearNetworkManager LandingGearNetManager
		{
			get { return m_landingNetManager; }
		}

		#endregion

		#region "Methods"

		new public static bool ReflectionUnitTest()
		{
			try
			{
				bool result = true;

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(LandingGearNamespace, LandingGearClass);
				if (type == null)
					throw new Exception("Could not find internal type for LandingGearEntity");

				result &= HasMethod(type, LandingGearGetAutoLockMethod);
				result &= HasMethod(type, LandingGearSetAutoLockMethod);
				result &= HasMethod(type, LandingGearGetBrakeForceMethod);
				result &= HasMethod(type, LandingGearSetBrakeForceMethod);

				result &= HasField(type, LandingGearIsLockedField);
				result &= HasField(type, LandingGearNetManagerField);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		protected Object GetLandingGearNetManager()
		{
			Object result = GetEntityFieldValue(ActualObject, LandingGearNetManagerField);
			return result;
		}

		protected bool GetIsLocked()
		{
			try
			{
				bool result = (bool)GetEntityFieldValue(ActualObject, LandingGearIsLockedField);
				return result;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return m_isLocked;
			}
		}

		protected bool GetAutoLockEnabled()
		{
			try
			{
				bool result = (bool)InvokeEntityMethod(ActualObject, LandingGearGetAutoLockMethod);
				return result;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return m_autoLockEnabled;
			}
		}

		protected float GetBrakeForce()
		{
			try
			{
				float result = (float)InvokeEntityMethod(ActualObject, LandingGearGetBrakeForceMethod);
				return result;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return m_brakeForce;
			}
		}

		protected void InternalUpdateIsLocked()
		{
			SetEntityFieldValue(ActualObject, LandingGearIsLockedField, m_isLocked);

			LandingGearNetManager.BroadcastIsLocked();
		}

		protected void InternalUpdateAutoLockEnabled()
		{
			InvokeEntityMethod(ActualObject, LandingGearSetAutoLockMethod, new object[] { m_autoLockEnabled });

			LandingGearNetManager.BroadcastAutoLock();
		}

		protected void InternalUpdateBrakeForce()
		{
			InvokeEntityMethod(ActualObject, LandingGearSetBrakeForceMethod, new object[] { m_brakeForce });

			LandingGearNetManager.BroadcastBrakeForce();
		}

		#endregion
	}

	public class LandingGearNetworkManager
	{
		#region "Attributes"

		private LandingGearEntity m_parent;
		private Object m_backingObject;

		public static string LandingGearNetworkManagerNamespace = LandingGearEntity.LandingGearNamespace + "." + LandingGearEntity.LandingGearClass;
		public static string LandingGearNetworkManagerClass = "26556F6F0AE7CF1827348C8BE3041E52";

		public static string LandingGearNetworkManagerBroadcastIsLockedMethod = "486EB5B14ECC3CFCBB6A41DC47E8E457";
		public static string LandingGearNetworkManagerBroadcastAutoLockMethod = "EE7AB0648967FCDF7B20E1C359BC67E0";
		public static string LandingGearNetworkManagerBroadcastBrakeForceMethod = "78A3CD1FD04B6E57EB1053EE5E3F1CF7";

		#endregion

		#region "Constructors and Initializers"

		public LandingGearNetworkManager(LandingGearEntity parent, Object backingObject)
		{
			m_parent = parent;
			m_backingObject = backingObject;
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
				Type type = LandingGearEntity.InternalType.GetNestedType(LandingGearNetworkManagerClass, BindingFlags.Public | BindingFlags.NonPublic);
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
					throw new Exception("Could not find internal type for LandingGearNetworkManager");
				result &= BaseObject.HasMethod(type, LandingGearNetworkManagerBroadcastIsLockedMethod);
				result &= BaseObject.HasMethod(type, LandingGearNetworkManagerBroadcastAutoLockMethod);
				result &= BaseObject.HasMethod(type, LandingGearNetworkManagerBroadcastBrakeForceMethod);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		public void BroadcastIsLocked()
		{
			BaseObject.InvokeEntityMethod(BackingObject, LandingGearNetworkManagerBroadcastIsLockedMethod, new object[] { m_parent.IsLocked });
		}

		public void BroadcastAutoLock()
		{
			BaseObject.InvokeEntityMethod(BackingObject, LandingGearNetworkManagerBroadcastAutoLockMethod, new object[] { m_parent.AutoLock });
		}

		public void BroadcastBrakeForce()
		{
			BaseObject.InvokeEntityMethod(BackingObject, LandingGearNetworkManagerBroadcastBrakeForceMethod, new object[] { m_parent.BrakeForce });
		}

		#endregion
	}
}
