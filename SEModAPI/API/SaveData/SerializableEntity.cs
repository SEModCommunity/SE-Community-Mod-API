using Microsoft.Xml.Serialization.GeneratedAssembly;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Reflection;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;

using SEModAPI.Support;
using SEModAPI.API.Definitions;

namespace SEModAPI.API.SaveData
{
	public abstract class SerializableEntity<T> : OverLayerDefinition<T> where T : MyObjectBuilder_Base
	{
		#region "Attributes"

		private long m_entityId = 0;

		#endregion

		#region "Constructors and Initializers"

		protected SerializableEntity(T baseDefinition)
			: base(baseDefinition)
		{
		}

		#endregion

		#region "Properties"

		public MyObjectBuilderTypeEnum TypeId
		{
			get { return m_baseDefinition.TypeId; }
			set
			{
				if (m_baseDefinition.TypeId == value) return;
				m_baseDefinition.ChangeType(value, SubtypeName);
				Changed = true;
			}
		}

		public string SubtypeName
		{
			get { return m_baseDefinition.SubtypeName; }
			set
			{
				if (m_baseDefinition.SubtypeName == value) return;
				m_baseDefinition.SubtypeName = value;
				Changed = true;
			}
		}

		public long EntityId
		{
			get { return m_entityId; }
			set
			{
				if (m_entityId == value) return;
				m_entityId = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		public long GenerateEntityId()
		{
			// Not the offical SE way of generating IDs, but its fast and we don't have to worry about a random seed.
			var buffer = Guid.NewGuid().ToByteArray();
			return BitConverter.ToInt64(buffer, 0);
		}

		protected override string GetNameFrom(T definition)
		{
			return definition.SubtypeName;
		}

		#endregion
	}

	public class SerializableEntityManager<T, U> : SerializableDefinitionsManager<T, U>
		where T : MyObjectBuilder_Base
		where U : SerializableEntity<T>
	{
		#region "Methods"

		new public U NewEntry(T source)
		{
			if (!IsMutable) return default(U);

			var newEntry = CreateOverLayerSubTypeInstance(source);
			long entityId = newEntry.EntityId;
			if (entityId == 0)
				entityId = newEntry.GenerateEntityId();
			GetInternalData().Add(entityId, newEntry);

			return newEntry;
		}

		#endregion
	}
}
