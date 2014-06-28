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

using SEModAPIInternal.API.Entity.Sector;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity
{
	public class SectorEntity : BaseObject
	{
		#region "Attributes"

		//Sector Events
		private BaseObjectManager m_eventManager;

		//Sector Objects
		private CubeGridManager m_cubeGridManager;
		private BaseObjectManager m_voxelMapManager;
		private BaseObjectManager m_floatingObjectManager;
		private BaseObjectManager m_meteorManager;
		private BaseObjectManager m_unknownObjectManager;

		#endregion

		#region "Constructors and Initializers"

		public SectorEntity(MyObjectBuilder_Sector definition)
			: base(definition)
		{
			m_eventManager = new BaseObjectManager();
			m_cubeGridManager = new CubeGridManager();
			m_voxelMapManager = new BaseObjectManager();
			m_floatingObjectManager = new BaseObjectManager();
			m_meteorManager = new BaseObjectManager();
			m_unknownObjectManager = new BaseObjectManager();

			List<Event> events = new List<Event>();
			foreach (var sectorEvent in definition.SectorEvents.Events)
			{
				events.Add(new Event(sectorEvent));
			}

			List<CubeGridEntity> cubeGrids = new List<CubeGridEntity>();
			List<VoxelMap> voxelMaps = new List<VoxelMap>();
			List<FloatingObject> floatingObjects = new List<FloatingObject>();
			List<Meteor> meteors = new List<Meteor>();
			List<BaseEntity> unknowns = new List<BaseEntity>();
			foreach (var sectorObject in definition.SectorObjects)
			{
				if (sectorObject.TypeId == MyObjectBuilderTypeEnum.CubeGrid)
				{
					cubeGrids.Add(new CubeGridEntity((MyObjectBuilder_CubeGrid)sectorObject));
				}
				else if (sectorObject.TypeId == MyObjectBuilderTypeEnum.VoxelMap)
				{
					voxelMaps.Add(new VoxelMap((MyObjectBuilder_VoxelMap)sectorObject));
				}
				else if (sectorObject.TypeId == MyObjectBuilderTypeEnum.FloatingObject)
				{
					floatingObjects.Add(new FloatingObject((MyObjectBuilder_FloatingObject)sectorObject));
				}
				else if (sectorObject.TypeId == MyObjectBuilderTypeEnum.Meteor)
				{
					meteors.Add(new Meteor((MyObjectBuilder_Meteor)sectorObject));
				}
				else
				{
					unknowns.Add(new BaseEntity(sectorObject));
				}
			}

			//Build the managers from the lists
			m_eventManager.Load(events.ToArray());
			m_cubeGridManager.Load(cubeGrids.ToArray());
			m_voxelMapManager.Load(voxelMaps.ToArray());
			m_floatingObjectManager.Load(floatingObjects.ToArray());
			m_meteorManager.Load(meteors.ToArray());
			m_unknownObjectManager.Load(unknowns.ToArray());
		}

		#endregion

		#region "Properties"

		/// <summary>
		/// API formated name of the object
		/// </summary>
		[Category("Sector")]
		[Browsable(true)]
		[ReadOnly(true)]
		[Description("The formatted name of the object")]
		public override string Name
		{
			get { return "SANDBOX_" + this.Position.X + "_" + this.Position.Y + "_" + this.Position.Z + "_"; }
		}

		[Category("Sector")]
		public VRageMath.Vector3I Position
		{
			get { return GetSubTypeEntity().Position; }
		}

		[Category("Sector")]
		public int AppVersion
		{
			get { return GetSubTypeEntity().AppVersion; }
		}

		[Category("Sector")]
		[Browsable(false)]
		public List<Event> Events
		{
			get
			{
				var newList = m_eventManager.GetTypedInternalData<Event>();
				return newList;
			}
		}

		[Category("Sector")]
		[Browsable(false)]
		public List<CubeGridEntity> CubeGrids
		{
			get
			{
				var newList = m_cubeGridManager.GetTypedInternalData<CubeGridEntity>();
				return newList;
			}
		}

		[Category("Sector")]
		[Browsable(false)]
		public List<VoxelMap> VoxelMaps
		{
			get
			{
				var newList = m_voxelMapManager.GetTypedInternalData<VoxelMap>();
				return newList;
			}
		}

		[Category("Sector")]
		[Browsable(false)]
		public List<FloatingObject> FloatingObjects
		{
			get
			{
				var newList = m_floatingObjectManager.GetTypedInternalData<FloatingObject>();
				return newList;
			}
		}

		[Category("Sector")]
		[Browsable(false)]
		public List<Meteor> Meteors
		{
			get
			{
				var newList = m_meteorManager.GetTypedInternalData<Meteor>();
				return newList;
			}
		}

		[Category("Sector")]
		[Browsable(false)]
		public List<BaseEntity> UnknownObjects
		{
			get
			{
				var newList = m_unknownObjectManager.GetTypedInternalData<BaseEntity>();
				return newList;
			}
		}

		#endregion

		#region "Methods"

		public BaseObject NewEntry(Type newType)
		{
			if (newType == typeof(CubeGridEntity))
				return m_cubeGridManager.NewEntry<CubeGridEntity>();
			if(newType == typeof(VoxelMap))
				return m_voxelMapManager.NewEntry<VoxelMap>();
			if (newType == typeof(FloatingObject))
				return m_floatingObjectManager.NewEntry<FloatingObject>();
			if (newType == typeof(Meteor))
				return m_meteorManager.NewEntry<Meteor>();

			return null;
		}

		public bool DeleteEntry(Object source)
		{
			Type deleteType = source.GetType();
			if (deleteType == typeof(CubeGridEntity))
				return m_cubeGridManager.DeleteEntry((CubeGridEntity)source);
			if (deleteType == typeof(VoxelMap))
				return m_voxelMapManager.DeleteEntry((VoxelMap)source);
			if (deleteType == typeof(FloatingObject))
				return m_floatingObjectManager.DeleteEntry((FloatingObject)source);
			if (deleteType == typeof(Meteor))
				return m_meteorManager.DeleteEntry((Meteor)source);

			return false;
		}

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_Sector GetSubTypeEntity()
		{
			MyObjectBuilder_Sector baseSector = (MyObjectBuilder_Sector)BaseEntity;

			try
			{
				//Update the events in the base definition
				baseSector.SectorEvents.Events.Clear();
				foreach (var item in m_eventManager.GetTypedInternalData<Event>())
				{
					baseSector.SectorEvents.Events.Add(item.GetSubTypeEntity());
				}

				//Update the sector objects in the base definition
				baseSector.SectorObjects.Clear();
				foreach (var item in m_cubeGridManager.GetTypedInternalData<CubeGridEntity>())
				{
					baseSector.SectorObjects.Add(item.GetSubTypeEntity());
				}
				foreach (var item in m_voxelMapManager.GetTypedInternalData<VoxelMap>())
				{
					baseSector.SectorObjects.Add(item.GetSubTypeEntity());
				}
				foreach (var item in m_floatingObjectManager.GetTypedInternalData<FloatingObject>())
				{
					baseSector.SectorObjects.Add(item.GetSubTypeEntity());
				}
				foreach (var item in m_meteorManager.GetTypedInternalData<Meteor>())
				{
					baseSector.SectorObjects.Add(item.GetSubTypeEntity());
				}
				foreach (var item in m_unknownObjectManager.GetTypedInternalData<BaseEntity>())
				{
					baseSector.SectorObjects.Add(item.GetSubTypeEntity());
				}
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
			return baseSector;
		}

		#endregion
	}

	public class SectorManager : BaseObjectManager
	{
		#region "Attributes"

		private SectorEntity m_Sector;

		#endregion

		#region "Constructors and Initializers"

		public SectorManager()
		{
		}

		#endregion

		#region "Properties"

		public SectorEntity Sector
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
			m_Sector = new SectorEntity(data);
		}

		new public bool Save()
		{
			return WriteSpaceEngineersFile<MyObjectBuilder_Sector, MyObjectBuilder_SectorSerializer>(m_Sector.GetSubTypeEntity(), this.FileInfo.FullName);
		}

		#endregion
	}
}
