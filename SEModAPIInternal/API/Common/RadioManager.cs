using System;
using System.ComponentModel;

using SEModAPIInternal.API.Entity;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Common
{
	public class RadioManager
	{
		#region "Attributes"

		private Object m_backingObject;
		private RadioManagerNetworkManager m_networkManager;

		private float m_broadcastRadius;
		private Object m_linkedEntity;
		private bool m_isEnabled;
		private int m_aabbTreeId;

		public static string RadioManagerNamespace = "6DDCED906C852CFDABA0B56B84D0BD74";
		public static string RadioManagerClass = "994372BD682BE5E79F2F32E79BE318F5";

		public static string RadioManagerGetBroadcastRadiusMethod = "CC1F306EACC95C00A05C100712656EBC";
		public static string RadioManagerSetBroadcastRadiusMethod = "C42CAA2B50B8705C7F36262BCE8E60EA";
		public static string RadioManagerGetLinkedEntityMethod = "7DE57FDDF37DD6219A990596E0283F01";
		public static string RadioManagerSetLinkedEntityMethod = "1C653F74AF87659F7AA9B39E35D789CE";
		public static string RadioManagerGetEnabledMethod = "78F34EF54782BBB097110F15BB3F5CC7";
		public static string RadioManagerSetEnabledMethod = "5DCB378F714DC1A82AF40135BBE08BE1";
		public static string RadioManagerGetAABBTreeIdMethod = "20FCED684DEC4EA0CCEE92D67DB109F1";
		public static string RadioManagerSetAABBTreeIdMethod = "10D69A54D5F2CE65056EBB10BCF3D8B3";

		public static string RadioManagerNetworkManagerField = "74012B9403A8C0F8C32FE86DA34CA0F6";

		#endregion

		#region "Constructors and Initializers"

		public RadioManager(Object backingObject)
		{
			try
			{
				m_backingObject = backingObject;
				m_networkManager = new RadioManagerNetworkManager(this);

				m_broadcastRadius = (float)BaseObject.InvokeEntityMethod(BackingObject, RadioManagerGetBroadcastRadiusMethod);
				m_linkedEntity = BaseObject.InvokeEntityMethod(BackingObject, RadioManagerGetLinkedEntityMethod);
				m_isEnabled = (bool)BaseObject.InvokeEntityMethod(BackingObject, RadioManagerGetEnabledMethod);
				m_aabbTreeId = (int)BaseObject.InvokeEntityMethod(BackingObject, RadioManagerGetAABBTreeIdMethod);
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		#endregion

		#region "Properties"

		[Category("Radio Manager")]
		[Browsable(false)]
		public Object BackingObject
		{
			get { return m_backingObject; }
		}

		[Category("Radio Manager")]
		public float BroadcastRadius
		{
			get { return m_broadcastRadius; }
			set
			{
				if (m_broadcastRadius == value) return;
				m_broadcastRadius = value;

				if (BackingObject != null)
				{
					Action action = InternalUpdateBroadcastRadius;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Radio Manager")]
		[Browsable(false)]
		public Object LinkedEntity
		{
			get { return m_linkedEntity; }
			set
			{
				if (m_linkedEntity == value) return;
				m_linkedEntity = value;

				if (BackingObject != null)
				{
					Action action = InternalUpdateLinkedEntity;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Radio Manager")]
		public bool Enabled
		{
			get { return m_isEnabled; }
			set
			{
				if (m_isEnabled == value) return;
				m_isEnabled = value;

				if (BackingObject != null)
				{
					Action action = InternalUpdateEnabled;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Radio Manager")]
		public int TreeId
		{
			get { return m_aabbTreeId; }
			set
			{
				if (m_aabbTreeId == value) return;
				m_aabbTreeId = value;

				if (BackingObject != null)
				{
					Action action = InternalUpdateTreeId;
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
				Type type1 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(RadioManagerNamespace, RadioManagerClass);
				if (type1 == null)
					throw new Exception("Could not find internal type for RadioManager");
				bool result = true;
				result &= BaseObject.HasMethod(type1, RadioManagerGetBroadcastRadiusMethod);
				result &= BaseObject.HasMethod(type1, RadioManagerSetBroadcastRadiusMethod);
				result &= BaseObject.HasMethod(type1, RadioManagerGetLinkedEntityMethod);
				result &= BaseObject.HasMethod(type1, RadioManagerSetLinkedEntityMethod);
				result &= BaseObject.HasMethod(type1, RadioManagerGetEnabledMethod);
				result &= BaseObject.HasMethod(type1, RadioManagerSetEnabledMethod);
				result &= BaseObject.HasMethod(type1, RadioManagerGetAABBTreeIdMethod);
				result &= BaseObject.HasMethod(type1, RadioManagerSetAABBTreeIdMethod);
				result &= BaseObject.HasField(type1, RadioManagerNetworkManagerField);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		internal Object GetNetworkManager()
		{
			try
			{
				Object result = BaseObject.GetEntityFieldValue(BackingObject, RadioManagerNetworkManagerField);
				return result;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		protected void InternalUpdateBroadcastRadius()
		{
			m_networkManager.BroadcastRadius();
			BaseObject.InvokeEntityMethod(BackingObject, RadioManagerSetBroadcastRadiusMethod, new object[] { BroadcastRadius });
		}

		protected void InternalUpdateLinkedEntity()
		{
			BaseObject.InvokeEntityMethod(BackingObject, RadioManagerSetLinkedEntityMethod, new object[] { LinkedEntity });
		}

		protected void InternalUpdateEnabled()
		{
			m_networkManager.BroadcastEnabled();
			BaseObject.InvokeEntityMethod(BackingObject, RadioManagerSetEnabledMethod, new object[] { Enabled });
		}

		protected void InternalUpdateTreeId()
		{
			BaseObject.InvokeEntityMethod(BackingObject, RadioManagerSetAABBTreeIdMethod, new object[] { TreeId });
		}

		#endregion
	}

	public class RadioManagerNetworkManager
	{
		#region "Attributes"

		private RadioManager m_parent;

		public static string RadioManagerNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string RadioManagerNetManagerClass = "1CE6F03E36FA552A8223DEBF0554411C";

		public static string RadioManagerNetManagerBroadcastRadiusMethod = "D98F28A89752B53473A94214E2D0E0EA";
		public static string RadioManagerNetManagerBroadcastEnabledMethod = "CCB8371C1B91E687E88786806386E87C";

		#endregion

		#region "Constructors and Initializers"

		public RadioManagerNetworkManager(RadioManager parent)
		{
			m_parent = parent;
		}

		#endregion

		#region "Properties"

		public Object BackingObject
		{
			get
			{
				Object result = m_parent.GetNetworkManager();
				return result;
			}
		}

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				Type type1 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(RadioManagerNetManagerNamespace, RadioManagerNetManagerClass);
				if (type1 == null)
					throw new Exception("Could not find internal type for RadioManagerNetworkManager");
				bool result = true;
				result &= BaseObject.HasMethod(type1, RadioManagerNetManagerBroadcastRadiusMethod);
				result &= BaseObject.HasMethod(type1, RadioManagerNetManagerBroadcastEnabledMethod);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		public void BroadcastRadius()
		{
			Action action = InternalBroadcastRadius;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		public void BroadcastEnabled()
		{
			Action action = InternalBroadcastEnabled;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		#region "Internal"

		protected void InternalBroadcastRadius()
		{
			BaseObject.InvokeEntityMethod(BackingObject, RadioManagerNetManagerBroadcastRadiusMethod, new object[] { m_parent.BroadcastRadius });
		}

		protected void InternalBroadcastEnabled()
		{
			BaseObject.InvokeEntityMethod(BackingObject, RadioManagerNetManagerBroadcastEnabledMethod, new object[] { m_parent.Enabled });
		}

		#endregion

		#endregion
	}
}
