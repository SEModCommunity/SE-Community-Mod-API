using System;
using System.Collections.Generic;
using System.ComponentModel;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.API;
using SEModAPI.API.Definitions;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Server;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
	public class Meteor : BaseEntity
	{
		#region "Attributes"

		private InventoryItemEntity m_item;

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

		public override string Name
		{
			get { return GetSubTypeEntity().Item.PhysicalContent.SubtypeName; }
		}

		[Category("Meteor")]
		[Browsable(true)]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public override SerializableVector3 LinearVelocity
		{
			get { return GetSubTypeEntity().LinearVelocity; }
			set
			{
				if (GetSubTypeEntity().LinearVelocity == value) return;
				GetSubTypeEntity().LinearVelocity = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateEntityLinearVelocity;
					SandboxGameAssemblyWrapper.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Meteor")]
		[Browsable(true)]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public override SerializableVector3 AngularVelocity
		{
			get { return GetSubTypeEntity().AngularVelocity; }
			set
			{
				if (GetSubTypeEntity().AngularVelocity == value) return;
				GetSubTypeEntity().AngularVelocity = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateEntityAngularVelocity;
					SandboxGameAssemblyWrapper.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Meteor")]
		public float Integrity
		{
			get { return GetSubTypeEntity().Integrity; }
			set
			{
				if (GetSubTypeEntity().Integrity == value) return;
				GetSubTypeEntity().Integrity = value;
				Changed = true;

				//TODO - Add functionality for backing object for this property
			}
		}

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
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_Meteor GetSubTypeEntity()
		{
			return (MyObjectBuilder_Meteor)BaseEntity;
		}

		#endregion
	}
}
