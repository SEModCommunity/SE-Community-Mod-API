using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.API.Definitions;
using SEModAPI.API.Internal;

using VRageMath;
using VRage;

namespace SEModAPI.API.SaveData
{
	public class SectorObject<T> : SerializableEntity<T> where T : MyObjectBuilder_EntityBase
	{
		#region "Attributes"

		private Object m_backingObject;
		private Thread m_backingThread;

		#endregion

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

				GameObjectManagerWrapper.GetInstance().UpdateEntityId(BackingObject, value);
			}
		}

		[Category("Sector Object")]
		[Browsable(false)]
		public Object BackingObject
		{
			get { return m_backingObject; }
			set
			{
				m_backingObject = value;
				Changed = true;
			}
		}

		[Category("Sector Object")]
		[Browsable(false)]
		public Thread BackingThread
		{
			get { return m_backingThread; }
			set
			{
				m_backingThread = value;
				Changed = true;
			}
		}

		[Category("Sector Object")]
		[ReadOnly(true)]
		public MyPersistentEntityFlags2 PersistentFlags
		{
			get { return m_baseDefinition.PersistentFlags; }
			set
			{
				if (m_baseDefinition.PersistentFlags == value) return;
				m_baseDefinition.PersistentFlags = value;
				Changed = true;

				//TODO - Find what the backing field is for this
			}
		}

		[Category("Sector Object")]
		[Browsable(false)]
		public MyPositionAndOrientation PositionAndOrientation
		{
			get { return m_baseDefinition.PositionAndOrientation.GetValueOrDefault(); }
			set
			{
				if (m_baseDefinition.PositionAndOrientation.Equals(value)) return;
				m_baseDefinition.PositionAndOrientation = value;
				Changed = true;
			}
		}

		[Category("Sector Object")]
		[TypeConverter(typeof(SerializableVector3TypeConverter))]
		public SerializableVector3 Position
		{
			get { return m_baseDefinition.PositionAndOrientation.GetValueOrDefault().Position; }
			set
			{
				if (Position.Equals(value)) return;
				MyPositionAndOrientation? positionOrientation = new MyPositionAndOrientation(value, Forward, Up);
				m_baseDefinition.PositionAndOrientation = positionOrientation;
				Changed = true;

				GameObjectManagerWrapper.GetInstance().UpdateEntityPosition(BackingObject, value);
			}
		}

		[Category("Sector Object")]
		[TypeConverter(typeof(SerializableVector3TypeConverter))]
		public SerializableVector3 Up
		{
			get { return m_baseDefinition.PositionAndOrientation.GetValueOrDefault().Up; }
			set
			{
				if (Up.Equals(value)) return;
				MyPositionAndOrientation? positionOrientation = new MyPositionAndOrientation(Position, Forward, value);
				m_baseDefinition.PositionAndOrientation = positionOrientation;
				Changed = true;
			}
		}

		[Category("Sector Object")]
		[TypeConverter(typeof(SerializableVector3TypeConverter))]
		public SerializableVector3 Forward
		{
			get { return m_baseDefinition.PositionAndOrientation.GetValueOrDefault().Forward; }
			set
			{
				if (Forward.Equals(value)) return;
				MyPositionAndOrientation? positionOrientation = new MyPositionAndOrientation(Position, value, Up);
				m_baseDefinition.PositionAndOrientation = positionOrientation;
				Changed = true;
			}
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
