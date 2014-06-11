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
		private List<CubeGrid> m_cubeGrids;

        #endregion Attributes

		#region "Constructors and Initializers"

		public SaveFileSerializer(string pFileLocation, ConfigFileSerializer pConfigFile)
		{
			if (!File.Exists(pFileLocation))
			{
				throw new AutoException(new GameInstallationInfoException(GameInstallationInfoExceptionState.SteamPathNotFound));
			}

			m_data = pConfigFile.ReadSpaceEngineersFile<MyObjectBuilder_Sector, MyObjectBuilder_SectorSerializer>(pFileLocation);

			m_cubeGrids = new List<CubeGrid>();
			foreach (var sectorObject in m_data.SectorObjects)
			{
				if (sectorObject.TypeId == MyObjectBuilderTypeEnum.CubeGrid)
				{
					MyObjectBuilder_CubeGrid cubeGrid = (MyObjectBuilder_CubeGrid)sectorObject;
					m_cubeGrids.Add(new CubeGrid(cubeGrid));
				}

				if (sectorObject.TypeId == MyObjectBuilderTypeEnum.VoxelMap)
				{
					MyObjectBuilder_VoxelMap voxelMap = (MyObjectBuilder_VoxelMap)sectorObject;
					//TODO - Implement VoxelMap interface and save new instance from save file
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

		public MyObjectBuilder_GlobalEvents Events
		{
			get { return m_data.SectorEvents; }
		}

		public List<MyObjectBuilder_EntityBase> Objects
        {
            get { return m_data.SectorObjects; }
        }

		public List<CubeGrid> CubeGrids
		{
			get { return m_cubeGrids; }
		}

		#endregion
    }
}
