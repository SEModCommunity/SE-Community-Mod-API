using System;
using System.Collections.Generic;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.API.Definitions;
using SEModAPI.API.SaveData.Entity;

namespace SEModAPI.API.SaveData
{
	public class CubeBlock<T> : SerializableEntity<T> where T : MyObjectBuilder_CubeBlock
	{
		#region "Constructors and Initializers"

		public CubeBlock(T definition)
			: base(definition)
		{
			EntityId = definition.EntityId;
		}

		#endregion

		#region "Properties"

		public SerializableVector3I Min
		{
			get { return m_baseDefinition.Min; }
			set
			{
				if (m_baseDefinition.Min.Equals(value)) return;
				m_baseDefinition.Min = value;
				Changed = true;
			}
		}

		public SerializableBlockOrientation BlockOrientation
		{
			get { return m_baseDefinition.BlockOrientation; }
			set
			{
				if (m_baseDefinition.BlockOrientation.Equals(value)) return;
				m_baseDefinition.BlockOrientation = value;
				Changed = true;
			}
		}

		public SerializableVector3 ColorMaskHSV
		{
			get { return m_baseDefinition.ColorMaskHSV; }
			set
			{
				if (m_baseDefinition.ColorMaskHSV.Equals(value)) return;
				m_baseDefinition.ColorMaskHSV = value;
				Changed = true;
			}
		}

		#endregion
	}

	public class CubeBlockManager : SerializableEntityManager<MyObjectBuilder_CubeBlock, CubeBlock<MyObjectBuilder_CubeBlock>>
	{
		protected Dictionary<long, Object> m_definitions = new Dictionary<long, Object>();

		new public Object[] Definitions
		{
			get
			{
				Object[] tempList = new Object[m_definitions.Values.Count];
				m_definitions.Values.CopyTo(tempList, 0);
				return tempList;
			}
		}

		#region "Methods"

		new public void Load(MyObjectBuilder_CubeBlock[] source)
		{
			//Copy the data into the manager
			m_definitions.Clear();
			foreach (var definition in source)
			{
				switch (definition.TypeId)
				{
					case MyObjectBuilderTypeEnum.CargoContainer:
						NewEntry<MyObjectBuilder_CargoContainer, CargoContainerEntity>((MyObjectBuilder_CargoContainer)definition);
						break;
					case MyObjectBuilderTypeEnum.Reactor:
						NewEntry<MyObjectBuilder_Reactor, ReactorEntity>((MyObjectBuilder_Reactor)definition);
						break;
					case MyObjectBuilderTypeEnum.MedicalRoom:
						NewEntry<MyObjectBuilder_MedicalRoom, MedicalRoomEntity>((MyObjectBuilder_MedicalRoom)definition);
						break;
					default:
						NewEntry(definition);
						break;
				}
			}
		}

		new public CubeBlock<MyObjectBuilder_CubeBlock> NewEntry(MyObjectBuilder_CubeBlock source)
		{
			if (!IsMutable) return default(CubeBlock<MyObjectBuilder_CubeBlock>);

			CubeBlock<MyObjectBuilder_CubeBlock> newEntry = (CubeBlock<MyObjectBuilder_CubeBlock>)Activator.CreateInstance(typeof(CubeBlock<MyObjectBuilder_CubeBlock>), new object[] { source });

			long entityId = newEntry.EntityId;
			if (entityId == 0)
				entityId = newEntry.GenerateEntityId();
			m_definitions.Add(entityId, newEntry);

			return newEntry;
		}

		public CubeBlock<T> NewEntry<T, V>(T source)
			where T : MyObjectBuilder_CubeBlock
			where V : CubeBlock<T>
		{
			if (!IsMutable) return default(CubeBlock<T>);

			var newEntry = (V)Activator.CreateInstance(typeof(V), new object[] { source });

			long entityId = newEntry.EntityId;
			if (entityId == 0)
				entityId = newEntry.GenerateEntityId();
			m_definitions.Add(entityId, newEntry);

			return newEntry;
		}

		#endregion
	}
}
