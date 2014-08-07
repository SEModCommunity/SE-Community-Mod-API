using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.API;
using SEModAPI.API.Definitions;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

using VRageMath;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
	[DataContract(Name = "MeteorProxy")]
	public class Meteor : BaseEntity
	{
		#region "Attributes"

		private InventoryItemEntity m_item;
		private static Type m_internalType;

		public static string MeteorNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string MeteorClass = "E5A0E3F04CC6DEFB7410825523C63704";

		#endregion

		#region "Constructors and Initializers"

		public Meteor(MyObjectBuilder_Meteor definition)
			: base(definition)
		{
			m_item = new InventoryItemEntity(definition.Item);
		}

		public Meteor(MyObjectBuilder_Meteor definition, Object backingObject)
			: base(definition, backingObject)
		{
			m_item = new InventoryItemEntity(definition.Item);
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Meteor")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal static Type InternalType
		{
			get
			{
				if (m_internalType == null)
					m_internalType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(MeteorNamespace, MeteorClass);
				return m_internalType;
			}
		}

		[DataMember]
		[Category("Meteor")]
		[Browsable(true)]
		public override string Name
		{
			get { return ObjectBuilder.Item.PhysicalContent.SubtypeName; }
		}

		[DataMember]
		[Category("Meteor")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_Meteor ObjectBuilder
		{
			get
			{
				return (MyObjectBuilder_Meteor)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category("Meteor")]
		public float Integrity
		{
			get { return ObjectBuilder.Integrity; }
			set
			{
				if (ObjectBuilder.Integrity == value) return;
				ObjectBuilder.Integrity = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateMeteorIntegrity;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[DataMember]
		[Category("Meteor")]
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
					Action action = InternalUpdateMeteorItem;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		#endregion

		#region "Methods"

		protected void InternalUpdateMeteorIntegrity()
		{
			try
			{
				//TODO - Add methods to set integrity and item of the meteor
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected void InternalUpdateMeteorItem()
		{
			try
			{
				//TODO - Add methods to set integrity and item of the meteor
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
