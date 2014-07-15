using System;
using System.Collections.Generic;
using System.ComponentModel;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;

using SEModAPI.API.Definitions;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
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

		[Category("Floating Object")]
		[Browsable(true)]
		public override string Name
		{
			get { return GetSubTypeEntity().Item.PhysicalContent.SubtypeName; }
		}

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

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_FloatingObject GetSubTypeEntity()
		{
			return (MyObjectBuilder_FloatingObject)BaseEntity;
		}

		protected void InternalUpdateItem()
		{
			try
			{
				//TODO - Add methods to set item of the floating object
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
