using Microsoft.Xml.Serialization.GeneratedAssembly;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.Voxels;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.API.Definitions;

namespace SEModAPI.API.SaveData
{
	public class Sector : OverLayerDefinition<MyObjectBuilder_Sector>
	{
		#region "Attributes"

		//Sector Events
		private EventManager m_eventManager;

		//Sector Objects
		private CubeGridManager m_cubeGridManager;
		private VoxelMapManager m_voxelMapManager;
		private FloatingObjectManager m_floatingObjectManager;
		private MeteorManager m_meteorManager;
		//TODO - Build a manager for this so that we aren't using a list
		private List<SectorObject<MyObjectBuilder_EntityBase>> m_unknownObjects;

		#endregion

		#region "Constructors and Initializers"

		public Sector(MyObjectBuilder_Sector definition)
			: base(definition)
		{
			m_cubeGridManager = new CubeGridManager();
			m_voxelMapManager = new VoxelMapManager();
			m_floatingObjectManager = new FloatingObjectManager();
			m_meteorManager = new MeteorManager();

			m_unknownObjects = new List<SectorObject<MyObjectBuilder_EntityBase>>();

			List<MyObjectBuilder_GlobalEventBase> events = new List<MyObjectBuilder_GlobalEventBase>();
			foreach (var sectorEvent in definition.SectorEvents.Events)
			{
				events.Add(sectorEvent);
			}

			List<MyObjectBuilder_CubeGrid> cubeGrids = new List<MyObjectBuilder_CubeGrid>();
			List<MyObjectBuilder_VoxelMap> voxelMaps = new List<MyObjectBuilder_VoxelMap>();
			List<MyObjectBuilder_FloatingObject> floatingObjects = new List<MyObjectBuilder_FloatingObject>();
			List<MyObjectBuilder_Meteor> meteors = new List<MyObjectBuilder_Meteor>();
			foreach (var sectorObject in definition.SectorObjects)
			{
				if (sectorObject.TypeId == MyObjectBuilderTypeEnum.CubeGrid)
				{
					MyObjectBuilder_CubeGrid cubeGrid = (MyObjectBuilder_CubeGrid)sectorObject;
					cubeGrids.Add(cubeGrid);
				}
				else if (sectorObject.TypeId == MyObjectBuilderTypeEnum.VoxelMap)
				{
					MyObjectBuilder_VoxelMap voxelMap = (MyObjectBuilder_VoxelMap)sectorObject;
					voxelMaps.Add(voxelMap);
				}
				else if (sectorObject.TypeId == MyObjectBuilderTypeEnum.FloatingObject)
				{
					MyObjectBuilder_FloatingObject floatingObject = (MyObjectBuilder_FloatingObject)sectorObject;
					floatingObjects.Add(floatingObject);
				}
				else if (sectorObject.TypeId == MyObjectBuilderTypeEnum.Meteor)
				{
					MyObjectBuilder_Meteor meteor = (MyObjectBuilder_Meteor)sectorObject;
					meteors.Add(meteor);
				}
				else
				{
					m_unknownObjects.Add(new SectorObject<MyObjectBuilder_EntityBase>(sectorObject));
				}
			}

			//Build the managers from the lists
			m_eventManager = new EventManager(events);
			m_cubeGridManager.Load(cubeGrids.ToArray());
			m_voxelMapManager.Load(voxelMaps.ToArray());
			m_floatingObjectManager.Load(floatingObjects.ToArray());
			m_meteorManager.Load(meteors.ToArray());
		}

		#endregion

		#region "Properties"

		[Category("Sector")]
		[Browsable(false)]
		new public MyObjectBuilder_Sector BaseDefinition
		{
			get
			{
				//Update the events in the base definition
				m_baseDefinition.SectorEvents.Events = new List<MyObjectBuilder_GlobalEventBase>();
				foreach (var item in m_eventManager.Definitions)
				{
					m_baseDefinition.SectorEvents.Events.Add(item.BaseDefinition);
				}

				//Update the sector objects in the base definition
				m_baseDefinition.SectorObjects = new List<MyObjectBuilder_EntityBase>();
				foreach (var item in m_cubeGridManager.Definitions)
				{
					m_baseDefinition.SectorObjects.Add(item.BaseDefinition);
				}
				foreach (var item in m_voxelMapManager.Definitions)
				{
					m_baseDefinition.SectorObjects.Add(item.BaseDefinition);
				}
				foreach (var item in m_floatingObjectManager.Definitions)
				{
					m_baseDefinition.SectorObjects.Add(item.BaseDefinition);
				}
				foreach (var item in m_meteorManager.Definitions)
				{
					m_baseDefinition.SectorObjects.Add(item.BaseDefinition);
				}
				foreach (var item in m_unknownObjects)
				{
					m_baseDefinition.SectorObjects.Add(item.BaseDefinition);
				}

				return m_baseDefinition;
			}
		}

		[Category("Sector")]
		public VRageMath.Vector3I Position
		{
			get { return m_baseDefinition.Position; }
		}

		[Category("Sector")]
		public int AppVersion
		{
			get { return m_baseDefinition.AppVersion; }
		}

		[Category("Sector")]
		[Browsable(false)]
		public List<Event> Events
		{
			get
			{
				//TODO - Look into changing manager base class to return a List so we don't have to do the array conversion
				List<Event> newList = new List<Event>(m_eventManager.Definitions);
				return newList;
			}
		}

		[Category("Sector")]
		[Browsable(false)]
		public List<CubeGrid> CubeGrids
		{
			get
			{
				//TODO - Look into changing manager base class to return a List so we don't have to do the array conversion
				List<CubeGrid> newList = new List<CubeGrid>(m_cubeGridManager.Definitions);
				return newList;
			}
		}

		[Category("Sector")]
		[Browsable(false)]
		public List<VoxelMap> VoxelMaps
		{
			get
			{
				//TODO - Look into changing manager base class to return a List so we don't have to do the array conversion
				List<VoxelMap> newList = new List<VoxelMap>(m_voxelMapManager.Definitions);
				return newList;
			}
		}

		[Category("Sector")]
		[Browsable(false)]
		public List<FloatingObject> FloatingObjects
		{
			get
			{
				//TODO - Look into changing manager base class to return a List so we don't have to do the array conversion
				List<FloatingObject> newList = new List<FloatingObject>(m_floatingObjectManager.Definitions);
				return newList;
			}
		}

		[Category("Sector")]
		[Browsable(false)]
		public List<Meteor> Meteors
		{
			get
			{
				//TODO - Look into changing manager base class to return a List so we don't have to do the array conversion
				List<Meteor> newList = new List<Meteor>(m_meteorManager.Definitions);
				return newList;
			}
		}

		[Category("Sector")]
		[Browsable(false)]
		public List<SectorObject<MyObjectBuilder_EntityBase>> UnknownObjects
		{
			get { return m_unknownObjects; }
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_Sector definition)
		{
			return "SANDBOX_" + this.Position.X + "_" + this.Position.Y + "_" + this.Position.Z + "_";
		}

		public Object NewEntry(Type newType)
		{
			if(newType == typeof(CubeGrid))
				return m_cubeGridManager.NewEntry();
			if(newType == typeof(VoxelMap))
				return m_voxelMapManager.NewEntry();
			if (newType == typeof(FloatingObject))
				return m_floatingObjectManager.NewEntry();
			if (newType == typeof(Meteor))
				return m_meteorManager.NewEntry();

			return null;
		}

		public bool DeleteEntry(Object source)
		{
			Type deleteType = source.GetType();
			if (deleteType == typeof(CubeGrid))
				return m_cubeGridManager.DeleteEntry((CubeGrid)source);
			if (deleteType == typeof(VoxelMap))
				return m_voxelMapManager.DeleteEntry((VoxelMap)source);
			if (deleteType == typeof(FloatingObject))
				return m_floatingObjectManager.DeleteEntry((FloatingObject)source);
			if (deleteType == typeof(Meteor))
				return m_meteorManager.DeleteEntry((Meteor)source);

			return false;
		}

		#endregion
	}

	public class SectorManager : SerializableDefinitionsManager<MyObjectBuilder_Sector, Sector>
	{
		#region "Attributes"

		private Sector m_Sector;

		#endregion

		#region "Constructors and Initializers"

		public SectorManager()
		{
		}

		#endregion

		#region "Properties"

		public Sector Sector
		{
			get { return m_Sector; }
		}

		#endregion

		#region "Methods"

		new public void Load(FileInfo fileInfo)
		{
			//Save the file info to the property
			FileInfo = fileInfo;

			//Read in the sector data
			MyObjectBuilder_Sector data = ReadSpaceEngineersFile<MyObjectBuilder_Sector, MyObjectBuilder_SectorSerializer>(this.FileInfo.FullName);

			//And instantiate the sector with the data
			m_Sector = new Sector(data);
		}

		new public bool Save()
		{
			return WriteSpaceEngineersFile<MyObjectBuilder_Sector, MyObjectBuilder_SectorSerializer>(m_Sector.BaseDefinition, this.FileInfo.FullName);
		}

		#endregion
	}
}
