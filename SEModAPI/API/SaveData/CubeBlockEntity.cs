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
using SEModAPI.API.SaveData.Entity;

namespace SEModAPI.API.SaveData
{
	public class CubeBlockEntity<T> : SerializableEntity<T> where T : MyObjectBuilder_CubeBlock
	{
		#region "Attributes"

		private Object m_backingObject;
		private Thread m_backingThread;

		#endregion

		#region "Constructors and Initializers"

		public CubeBlockEntity(T definition)
			: base(definition)
		{
			EntityId = definition.EntityId;
		}

		#endregion

		#region "Properties"

		[Category("Cube Block")]
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

		[Category("Cube Block")]
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

		[Category("Cube Block")]
		public float BuildPercent
		{
			get { return m_baseDefinition.BuildPercent; }
			set
			{
				if (m_baseDefinition.BuildPercent == value) return;
				m_baseDefinition.BuildPercent = value;
				Changed = true;
			}
		}

		[Category("Cube Block")]
		public float IntegrityPercent
		{
			get { return m_baseDefinition.IntegrityPercent; }
			set
			{
				if (m_baseDefinition.IntegrityPercent == value) return;
				m_baseDefinition.IntegrityPercent = value;
				Changed = true;
			}
		}

		[Category("Cube Block")]
		[Description("Added as of 1.035.005")]
		public ulong Owner
		{
			get { return m_baseDefinition.Owner; }
			set
			{
				if (m_baseDefinition.Owner == value) return;
				m_baseDefinition.Owner = value;
				Changed = true;
			}
		}

		[Category("Cube Block")]
		[Description("Added as of 1.035.005")]
		public bool ShareWithFaction
		{
			get { return m_baseDefinition.ShareWithFaction; }
			set
			{
				if (m_baseDefinition.ShareWithFaction == value) return;
				m_baseDefinition.ShareWithFaction = value;
				Changed = true;
			}
		}

		#endregion
	}

	public class CubeBlockManager : SerializableEntityManager<MyObjectBuilder_CubeBlock, CubeBlockEntity<MyObjectBuilder_CubeBlock>>
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
				try
				{
					if (def == null)
						continue;

					Type defType = def.GetType();
					PropertyInfo defProp = defType.GetProperty("BaseDefinition", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
					if (defProp == null)
						continue;

					var baseDef = (MyObjectBuilder_CubeBlock)defProp.GetGetMethod().Invoke(def, new object[] { });
					if (baseDef == null)
						continue;

					list.Add(baseDef);
				}
				catch (Exception ex)
				{
					//TODO - Do something with an exception from here
				}
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
					case MyObjectBuilderTypeEnum.Cockpit:
						NewEntry<MyObjectBuilder_Cockpit, CockpitEntity>((MyObjectBuilder_Cockpit)definition);
						break;
					case MyObjectBuilderTypeEnum.Beacon:
						NewEntry<MyObjectBuilder_Beacon, BeaconEntity>((MyObjectBuilder_Beacon)definition);
						break;
					case MyObjectBuilderTypeEnum.GravityGenerator:
						NewEntry<MyObjectBuilder_GravityGenerator, GravityGeneratorEntity>((MyObjectBuilder_GravityGenerator)definition);
						break;
					default:
						NewEntry<MyObjectBuilder_CubeBlock, CubeBlockEntity<MyObjectBuilder_CubeBlock>>((MyObjectBuilder_CubeBlock)definition);
						break;
				}
			}
		}

		public CubeBlockEntity<T> NewEntry<T, V>(T source)
			where T : MyObjectBuilder_CubeBlock
			where V : CubeBlockEntity<T>
		{
			if (!IsMutable) return default(CubeBlockEntity<T>);

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
