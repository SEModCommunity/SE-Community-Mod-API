using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

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

		[Category("Cube Block")]
		[Browsable(false)]
		new public T BaseDefinition
		{
			get { return m_baseDefinition; }
		}

		[Category("Cube Block")]
		[TypeConverter(typeof(SerializableVector3ITypeConverter))]
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

		[Category("Cube Block")]
		[Browsable(false)]
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

		[Category("Cube Block")]
		[TypeConverter(typeof(SerializableVector3TypeConverter))]
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
		#region "Attributes"

		private Dictionary<long, Object> m_rawDefinitions = new Dictionary<long, Object>();

		#endregion

		#region "Properties"

		new public Object[] Definitions
		{
			get
			{
				Object[] tempList = new Object[m_rawDefinitions.Values.Count];
				m_rawDefinitions.Values.CopyTo(tempList, 0);
				return tempList;
			}
		}

		#endregion

		#region "Methods"

		new public List<MyObjectBuilder_CubeBlock> ExtractBaseDefinitions()
		{
			List<MyObjectBuilder_CubeBlock> list = new List<MyObjectBuilder_CubeBlock>();
			foreach (var def in m_rawDefinitions.Values)
			{
				Type defType = def.GetType();
				PropertyInfo defProp = defType.GetProperty("BaseDefinition", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
				var baseDef = (MyObjectBuilder_CubeBlock) defProp.GetGetMethod().Invoke(def, new object[] { });
				list.Add(baseDef);
			}
			return list;
		}

		public void Load(List<MyObjectBuilder_CubeBlock> source)
		{
			//Copy the data into the manager
			m_rawDefinitions.Clear();
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
						NewEntry<MyObjectBuilder_CubeBlock, CubeBlock<MyObjectBuilder_CubeBlock>>((MyObjectBuilder_CubeBlock)definition);
						break;
				}
			}
		}

		public CubeBlock<T> NewEntry<T, V>(T source)
			where T : MyObjectBuilder_CubeBlock
			where V : CubeBlock<T>
		{
			if (!IsMutable) return default(CubeBlock<T>);

			var newEntryType = typeof(V);

			var newEntry = (V)Activator.CreateInstance(newEntryType, new object[] { source });

			long entityId = newEntry.EntityId;
			if (entityId == 0)
				entityId = newEntry.GenerateEntityId();
			m_rawDefinitions.Add(entityId, newEntry);

			return newEntry;
		}

		#endregion
	}
}
