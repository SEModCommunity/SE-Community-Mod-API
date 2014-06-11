using Microsoft.Xml.Serialization.GeneratedAssembly;

using System.Collections.Generic;
using System.IO;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Voxels;

using SEModAPI.API;
using SEModAPI.API.Definitions;
using SEModAPI.API.SaveData;
using SEModAPI.Support;

namespace SEModAPI
{
    public class SaveFileSerializer
    {
        #region "Attributes"

        private MyObjectBuilder_Sector m_data;

		private List<Event> m_events;
		private List<CubeGrid> m_cubeGrids;
		private List<VoxelMap> m_voxelMaps;
		private List<FloatingObject> m_floatingObjects;
		private List<Meteor> m_meteors;
		private List<SectorObject<MyObjectBuilder_EntityBase>> m_unknownObjects;

        #endregion Attributes

		#region "Constructors and Initializers"

		public SaveFileSerializer(string pFileLocation, ConfigFileSerializer pConfigFile)
		{
			if (!File.Exists(pFileLocation))
			{
				throw new AutoException(new GameInstallationInfoException(GameInstallationInfoExceptionState.SteamPathNotFound));
			}

			m_data = pConfigFile.ReadSpaceEngineersFile<MyObjectBuilder_Sector, MyObjectBuilder_SectorSerializer>(pFileLocation);

			m_events = new List<Event>();
			m_cubeGrids = new List<CubeGrid>();
			m_voxelMaps = new List<VoxelMap>();
			m_floatingObjects = new List<FloatingObject>();
			m_meteors = new List<Meteor>();
			m_unknownObjects = new List<SectorObject<MyObjectBuilder_EntityBase>>();

			foreach (var sectorEvent in m_data.SectorEvents.Events)
			{
				m_events.Add(new Event(sectorEvent));
			}

			foreach (var sectorObject in m_data.SectorObjects)
			{
				if (sectorObject.TypeId == MyObjectBuilderTypeEnum.CubeGrid)
				{
					MyObjectBuilder_CubeGrid cubeGrid = (MyObjectBuilder_CubeGrid)sectorObject;
					m_cubeGrids.Add(new CubeGrid(cubeGrid));
				}
				else if (sectorObject.TypeId == MyObjectBuilderTypeEnum.VoxelMap)
				{
					MyObjectBuilder_VoxelMap voxelMap = (MyObjectBuilder_VoxelMap)sectorObject;
					m_voxelMaps.Add(new VoxelMap(voxelMap));
				}
				else if (sectorObject.TypeId == MyObjectBuilderTypeEnum.FloatingObject)
				{
					MyObjectBuilder_FloatingObject floatingObject = (MyObjectBuilder_FloatingObject)sectorObject;
					m_floatingObjects.Add(new FloatingObject(floatingObject));
				}
				else if (sectorObject.TypeId == MyObjectBuilderTypeEnum.Meteor)
				{
					MyObjectBuilder_Meteor meteor = (MyObjectBuilder_Meteor)sectorObject;
					m_meteors.Add(new Meteor(meteor));
				}
				else
				{
					m_unknownObjects.Add(new SectorObject<MyObjectBuilder_EntityBase>(sectorObject));
				}
			}
		}

		#endregion

		#region "Properties"

		public VRageMath.Vector3I Position
		{
			get { return m_data.Position; }
		}

		public int AppVersion
		{
			get { return m_data.AppVersion; }
		}

		public List<Event> Events
		{
			get { return m_events; }
		}

		public List<CubeGrid> CubeGrids
		{
			get { return m_cubeGrids; }
		}

		public List<VoxelMap> VoxelMaps
		{
			get { return m_voxelMaps; }
		}

		public List<FloatingObject> FloatingObjects
		{
			get { return m_floatingObjects; }
		}

		public List<Meteor> Meteors
		{
			get { return m_meteors; }
		}

		public List<SectorObject<MyObjectBuilder_EntityBase>> UnknownObjects
		{
			get { return m_unknownObjects; }
		}

		#endregion
    }
}
