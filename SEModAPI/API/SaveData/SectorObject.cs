using System;
using System.Collections.Generic;
using System.ComponentModel;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.API.Definitions;

namespace SEModAPI.API.SaveData
{
	public class SectorObject<T> : SerializableEntity<T> where T : MyObjectBuilder_EntityBase
	{
		#region "Constructors and Initializers"

		public SectorObject(T definition)
			: base(definition)
		{ }

		#endregion

		#region "Properties"

		new public long EntityId
		{
			get { return m_baseDefinition.EntityId; }
			set
			{
				if (m_baseDefinition.EntityId == value) return;
				m_baseDefinition.EntityId = value;
				Changed = true;
			}
		}

		public MyPersistentEntityFlags2 PersistentFlags
		{
			get { return m_baseDefinition.PersistentFlags; }
			set
			{
				if (m_baseDefinition.PersistentFlags == value) return;
				m_baseDefinition.PersistentFlags = value;
				Changed = true;
			}
		}

		[Browsable(false)]
		public MyPositionAndOrientation PositionAndOrientation
		{
			get { return m_baseDefinition.PositionAndOrientation.GetValueOrDefault(); }
			set
			{
				if (m_baseDefinition.PositionAndOrientation.ToString() == value.ToString()) return;
				m_baseDefinition.PositionAndOrientation = value;
				Changed = true;
			}
		}

		[Category("Vector3")]
		[TypeConverter(typeof(SerializableVector3TypeConverter))]
		public SerializableVector3 Position
		{
			get { return m_baseDefinition.PositionAndOrientation.GetValueOrDefault().Position; }
		}

		[Category("Vector3")]
		[TypeConverter(typeof(SerializableVector3TypeConverter))]
		public SerializableVector3 Up
		{
			get { return m_baseDefinition.PositionAndOrientation.GetValueOrDefault().Up; }
		}

		[Category("Vector3")]
		[TypeConverter(typeof(SerializableVector3TypeConverter))]
		public SerializableVector3 Forward
		{
			get { return m_baseDefinition.PositionAndOrientation.GetValueOrDefault().Forward; }
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(T definition)
		{
			return definition.EntityId.ToString();
		}

		#endregion
	}
}
