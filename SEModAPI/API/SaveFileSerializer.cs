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

        private Sector m_Sector;

        #endregion Attributes

		#region "Constructors and Initializers"

		public SaveFileSerializer(string pFileLocation, ConfigFileSerializer pConfigFile)
		{
			if (!File.Exists(pFileLocation))
			{
				throw new AutoException(new GameInstallationInfoException(GameInstallationInfoExceptionState.SteamPathNotFound));
			}

			m_Sector = new Sector(pConfigFile.ReadSpaceEngineersFile<MyObjectBuilder_Sector, MyObjectBuilder_SectorSerializer>(pFileLocation));
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
