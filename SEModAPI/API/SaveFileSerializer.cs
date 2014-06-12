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

		private ConfigFileSerializer m_configFileSerializer;
        private Sector m_Sector;

        #endregion Attributes

		#region "Constructors and Initializers"

		public SaveFileSerializer(FileInfo fileInfo, ConfigFileSerializer pConfigFile)
		{
			m_configFileSerializer = pConfigFile;
			if (!File.Exists(fileInfo.FullName))
			{
				throw new AutoException(new GameInstallationInfoException(GameInstallationInfoExceptionState.SteamPathNotFound));
			}

			m_Sector = new Sector(m_configFileSerializer.ReadSpaceEngineersFile<MyObjectBuilder_Sector, MyObjectBuilder_SectorSerializer>(fileInfo.FullName), fileInfo);
		}

		#endregion

		#region "Properties"

		public Sector Sector
		{
			get { return m_Sector; }
		}

		#endregion
	}
}
