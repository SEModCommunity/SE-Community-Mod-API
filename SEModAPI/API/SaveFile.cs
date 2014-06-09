using Microsoft.Xml.Serialization.GeneratedAssembly;
using Sandbox.Common.ObjectBuilders;
using System.Collections.Generic;
using System.IO;
using SEModAPI.API;

namespace SEModAPI
{
    /// <summary>
    /// This class was made only for testing purpose
    /// </summary>
    public class SaveFile
    {
        #region Attributes

        MyObjectBuilder_Sector _data;

        #endregion Attributes

        #region Properties



        #endregion Properties

        public List<MyObjectBuilder_EntityBase> Objects
        {
            get { return _data.SectorObjects; }
        }

        #region Constructors & Initializers

        public SaveFile(string pFileLocation, ConfigFileSerializer pConfigFile)
        {
            if (!File.Exists(pFileLocation))
            {
                throw new AutoException(new GameInstallationInfoException(GameInstallationInfoExceptionState.SteamPathNotFound));    
            }

            _data = pConfigFile.ReadSpaceEngineersFile<MyObjectBuilder_Sector, MyObjectBuilder_SectorSerializer>(pFileLocation);
        }

        #endregion Constructors & Initializers
    }
}
