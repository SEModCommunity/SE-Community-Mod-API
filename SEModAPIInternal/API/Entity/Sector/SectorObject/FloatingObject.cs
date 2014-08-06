using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;

using SEModAPI.API.Definitions;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
	[DataContract(Name = "FloatingObjectProxy")]
	public class FloatingObject : BaseEntity
	{
		#region "Attributes"

		private static Type m_internalType;
		private InventoryItemEntity m_item;

		public static string FloatingObjectNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string FloatingObjectClass = "60663B6C2E735862064C925471BD4138";

		#endregion

		#region "Constructors and Initializers"

		public FloatingObject(MyObjectBuilder_FloatingObject definition)
			: base(definition)
		{
			m_item = new InventoryItemEntity(definition.Item);
		}

		public FloatingObject(MyObjectBuilder_FloatingObject definition, Object backingObject)
			: base(definition, backingObject)
		{
			m_item = new InventoryItemEntity(definition.Item);
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Floating Object")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal static Type InternalType
		{
			get
			{
				if (m_internalType == null)
					m_internalType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(FloatingObjectNamespace, FloatingObjectClass);
				return m_internalType;
			}
		}

		[DataMember]
		[Category("Floating Object")]
		[Browsable(true)]
		public override string Name
		{
			get { return ObjectBuilder.Item.PhysicalContent.SubtypeName; }
		}

		[DataMember]
		[Category("Floating Object")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_FloatingObject ObjectBuilder
		{
			get
			{
				return (MyObjectBuilder_FloatingObject)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category("Floating Object")]
		[Browsable(false)]
		public InventoryItemEntity Item
		{
			get { return m_item; }
			set
			{
				if (m_item == value) return;
				m_item = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateItem;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}
		
		#endregion

		#region "Methods"

		public override void Dispose()
		{
			//TODO - Get this to work after 1.040
		}

		protected void InternalUpdateItem()
		{
			try
			{
				//TODO - Add methods to set item of the floating object
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		#endregion
	}

	public class FloatingObjectManager
	{
		public static string FloatingObjectManagerNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string FloatingObjectManagerClass = "66E5A072764E86AD0AC8B63304F0DC31";
	}
}
