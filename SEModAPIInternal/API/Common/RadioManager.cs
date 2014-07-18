﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Common
{
	public class RadioManager
	{
		#region "Attributes"

		private Object m_backingObject;
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

		#endregion

		#region "Constructors and Initializers"

		public RadioManager(Object backingObject)
		{
			try
			{
				m_backingObject = backingObject;

				m_broadcastRadius = (float)BaseObject.InvokeEntityMethod(BackingObject, RadioManagerGetBroadcastRadiusMethod);
				m_linkedEntity = BaseObject.InvokeEntityMethod(BackingObject, RadioManagerGetLinkedEntityMethod);
				m_isEnabled = (bool)BaseObject.InvokeEntityMethod(BackingObject, RadioManagerGetEnabledMethod);
				m_aabbTreeId = (int)BaseObject.InvokeEntityMethod(BackingObject, RadioManagerGetAABBTreeIdMethod);
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
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

		protected void InternalUpdateBroadcastRadius()
		{
			BaseObject.InvokeEntityMethod(BackingObject, RadioManagerSetBroadcastRadiusMethod, new object[] { BroadcastRadius });
		}

		protected void InternalUpdateLinkedEntity()
		{
			BaseObject.InvokeEntityMethod(BackingObject, RadioManagerSetLinkedEntityMethod, new object[] { LinkedEntity });
		}

		protected void InternalUpdateEnabled()
		{
			BaseObject.InvokeEntityMethod(BackingObject, RadioManagerSetEnabledMethod, new object[] { Enabled });
		}

		protected void InternalUpdateTreeId()
		{
			BaseObject.InvokeEntityMethod(BackingObject, RadioManagerSetAABBTreeIdMethod, new object[] { TreeId });
		}

		protected void InternalUpdateRadioManager()
		{
			BaseObject.InvokeEntityMethod(BackingObject, RadioManagerSetBroadcastRadiusMethod, new object[] { BroadcastRadius });
			BaseObject.InvokeEntityMethod(BackingObject, RadioManagerSetLinkedEntityMethod, new object[] { LinkedEntity });
			BaseObject.InvokeEntityMethod(BackingObject, RadioManagerSetEnabledMethod, new object[] { Enabled });
			BaseObject.InvokeEntityMethod(BackingObject, RadioManagerSetAABBTreeIdMethod, new object[] { TreeId });
		}

		#endregion
	}
}
