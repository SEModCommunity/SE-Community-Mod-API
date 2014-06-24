using System;
using System.Collections.Generic;
using System.ComponentModel;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;

using SEModAPI.API.Definitions;
using SEModAPI.API.Internal;
using SEModAPI.API.SaveData.Entity;

namespace SEModAPI.API.SaveData
{
	public class Meteor : SectorObject<MyObjectBuilder_Meteor>
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

		#endregion

		#region "Properties"

		[Category("Meteor")]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public VRageMath.Vector3 LinearVelocity
		{
			get { return m_baseDefinition.LinearVelocity; }
			set
			{
				if (m_baseDefinition.LinearVelocity == value) return;
				m_baseDefinition.LinearVelocity = value;
				Changed = true;
				if (BackingObject != null)
					GameObjectManagerWrapper.GetInstance().UpdateEntityVelocity(BackingObject, value);
			}
		}

		[Category("Meteor")]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public VRageMath.Vector3 AngularVelocity
		{
			get { return m_baseDefinition.AngularVelocity; }
			set
			{
				if (m_baseDefinition.AngularVelocity == value) return;
				m_baseDefinition.AngularVelocity = value;
				Changed = true;

				if (BackingObject != null)
					GameObjectManagerWrapper.GetInstance().UpdateEntityAngularVelocity(BackingObject, value);
			}
		}

		[Category("Meteor")]
		public float Integrity
		{
			get { return m_baseDefinition.Integrity; }
			set
			{
				if (m_baseDefinition.Integrity == value) return;
				m_baseDefinition.Integrity = value;
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

		protected override string GetNameFrom(MyObjectBuilder_Meteor definition)
		{
			return definition.Item.PhysicalContent.SubtypeName;
		}

		#endregion
	}

	public class MeteorManager : SerializableEntityManager<MyObjectBuilder_Meteor, Meteor>
	{
	}
}
