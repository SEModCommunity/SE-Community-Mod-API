using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xml.Serialization.GeneratedAssembly;
using System.Xml;
using System.ComponentModel;

using Sandbox.Common.ObjectBuilders;

namespace SEModAPI.API.Definitions
{
	public class DedicatedConfigDefinition
	{
		#region "Attributes"

		private MyConfigDedicatedData m_definition;

		#endregion

		#region "Constructors and Initializers"

		public DedicatedConfigDefinition(MyConfigDedicatedData definition)
		{
			m_definition = definition;
			Changed = false;
		}

		#endregion

		#region "Properties"

		[Browsable(false)]
		[ReadOnly(true)]
		[Description("Determine if the configuration has changed since it has been loaded or saved")]
		/// <summary>
		/// Determine if the configuration has changed since it has been loaded or saved
		/// </summary>
		public bool Changed
		{
			get;
			private set;
		}


		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the server's name")]
		[Category("Server Settings")]
		/// <summary>
		/// Get or set the server's name
		/// </summary>
		public string ServerName
		{
			get { return m_definition.ServerName; }
			set
			{
				if (m_definition.ServerName == value) return;
				m_definition.ServerName = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the server's port")]
		[Category("Server Settings")]
		/// <summary>
		/// Get or set the server's port
		/// </summary>
		public int ServerPort
		{
			get { return m_definition.ServerPort; }
			set
			{
				if (m_definition.ServerPort == value) return;
				m_definition.ServerPort = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the game mode")]
		[Category("Global Settings")]
		/// <summary>
		/// Get or set the game mode
		/// </summary>
		public MyGameModeEnum GameMode
		{
			get { return m_definition.SessionSettings.GameMode; }
			set
			{
				if (m_definition.SessionSettings.GameMode == value) return;
				m_definition.SessionSettings.GameMode = value;
				return;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the inventory size multiplier")]
		[Category("Global Settings")]
		/// <summary>
		/// Get or set the inventory size multiplier
		/// </summary>
		public float InventorySizeMultiplier
		{
			get { return m_definition.SessionSettings.InventorySizeMultiplier; }
			set
			{
				if (m_definition.SessionSettings.InventorySizeMultiplier == value) return;
				m_definition.SessionSettings.InventorySizeMultiplier = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the assembler speed multiplier")]
		[Category("Global Settings")]
		/// <summary>
		/// Get or set the assembler speed multiplier
		/// </summary>
		public float AssemblerSpeedMultiplier
		{
			get { return m_definition.SessionSettings.AssemblerSpeedMultiplier; }
			set
			{
				if (m_definition.SessionSettings.AssemblerSpeedMultiplier == value) return;
				m_definition.SessionSettings.AssemblerSpeedMultiplier = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the assembler efficiency multiplier")]
		[Category("Global Settings")]
		/// <summary>
		/// Get or set the assembler efficiency multiplier
		/// </summary>
		public float AssemblerEfficiencyMultiplier
		{
			get { return m_definition.SessionSettings.AssemblerEfficiencyMultiplier; }
			set
			{
				if (m_definition.SessionSettings.AssemblerEfficiencyMultiplier == value) return;
				m_definition.SessionSettings.AssemblerEfficiencyMultiplier = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the refinery speed multiplier")]
		[Category("Global Settings")]
		/// <summary>
		/// Get or set the refinery speed multiplier
		/// </summary>
		public float RefinerySpeedMultiplier
		{
			get { return m_definition.SessionSettings.RefinerySpeedMultiplier; }
			set
			{
				if (m_definition.SessionSettings.RefinerySpeedMultiplier == value) return;
				m_definition.SessionSettings.RefinerySpeedMultiplier = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the online mode")]
		[Category("Global Settings")]
		/// <summary>
		/// Get or set the online mode
		/// </summary>
		public MyOnlineModeEnum OnlineMode
		{
			get { return m_definition.SessionSettings.OnlineMode; }
			set
			{
				if (m_definition.SessionSettings.OnlineMode == value) return;
				m_definition.SessionSettings.OnlineMode = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the maximum number of players")]
		[Category("Server Settings")]
		/// <summary>
		/// Get or set the maximum number of players
		/// </summary>
		public short MaxPlayers
		{
			get { return m_definition.SessionSettings.MaxPlayers; }
			set
			{
				if (m_definition.SessionSettings.MaxPlayers == value) return;
				m_definition.SessionSettings.MaxPlayers = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the maximum number of floating object")]
		[Category("Global Settings")]
		/// <summary>
		/// Get or set the maximum number of floating object
		/// </summary>
		public short MaxFloatingObject
		{
			get { return m_definition.SessionSettings.MaxFloatingObjects; }
			set
			{
				if (m_definition.SessionSettings.MaxFloatingObjects == value) return;
				m_definition.SessionSettings.MaxFloatingObjects = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the environment hostility")]
		[Category("World Settings")]
		/// <summary>
		/// Get or set the environment hostility
		/// </summary>
		public MyEnvironmentHostilityEnum EnvironmentHostility
		{
			get { return m_definition.SessionSettings.EnvironmentHostility; }
			set
			{
				if (m_definition.SessionSettings.EnvironmentHostility == value) return;
				m_definition.SessionSettings.EnvironmentHostility = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Determine whether the player's health auto heal")]
		[Category("Global Settings")]
		/// <summary>
		/// Determine whether the player's health auto heal
		/// </summary>
		public bool AutoHealing
		{
			get { return m_definition.SessionSettings.AutoHealing; }
			set
			{
				if (m_definition.SessionSettings.AutoHealing == value) return;
				m_definition.SessionSettings.AutoHealing = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Determine whether the player can copy/paste ships")]
		[Category("Global Settings")]
		/// <summary>
		/// Determine whether the player can copy/paste ships
		/// </summary>
		public bool EnableCopyPaste
		{
			get { return m_definition.SessionSettings.EnableCopyPaste; }
			set
			{
				if (m_definition.SessionSettings.EnableCopyPaste == value) return;
				m_definition.SessionSettings.EnableCopyPaste = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Determine whether the server will save regularly the sector")]
		[Category("Server Settings")]
		/// <summary>
		/// Determine whether the server will save regularly the sector
		/// </summary>
		public bool AutoSave
		{
			get { return m_definition.SessionSettings.AutoSave; }
			set
			{
				if (m_definition.SessionSettings.AutoSave == value) return;
				m_definition.SessionSettings.AutoSave = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Determine whether the weapons are functional")]
		[Category("Global Settings")]
		/// <summary>
		/// Determine whether the weapons are functional
		/// </summary>
		public bool WeaponsEnabled
		{
			get { return m_definition.SessionSettings.WeaponsEnabled; }
			set
			{
				if (m_definition.SessionSettings.WeaponsEnabled == value) return;
				m_definition.SessionSettings.WeaponsEnabled = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Determine whether the player names will show on the HUD")]
		[Category("Global Settings")]
		/// <summary>
		/// Determine whether the player names will show on the HUD
		/// </summary>
		public bool ShowPlayerNamesOnHud
		{
			get { return m_definition.SessionSettings.ShowPlayerNamesOnHud; }
			set
			{
				if (m_definition.SessionSettings.ShowPlayerNamesOnHud == value) return;
				m_definition.SessionSettings.ShowPlayerNamesOnHud = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Determine whether the thrusters damage blocks")]
		[Category("Global Settings")]
		/// <summary>
		/// Determine whether the thrusters damage blocks
		/// </summary>
		public bool ThrusterDamage
		{
			get { return m_definition.SessionSettings.ThrusterDamage; }
			set
			{
				if (m_definition.SessionSettings.ThrusterDamage == value) return;
				m_definition.SessionSettings.ThrusterDamage = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Determine whether random ships spawn on the server")]
		[Category("World Settings")]
		/// <summary>
		/// Determine whether random ships spawn on the server
		/// </summary>
		public bool CargoShipsEnabled
		{
			get { return m_definition.SessionSettings.CargoShipsEnabled; }
			set
			{
				if (m_definition.SessionSettings.CargoShipsEnabled == value) return;
				m_definition.SessionSettings.CargoShipsEnabled = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Determine whether spectator mode is enable")]
		[Category("Global Settings")]
		/// <summary>
		/// Determine whether spectator mode is enable
		/// </summary>
		public bool EnableSpectator
		{
			get { return m_definition.SessionSettings.EnableSpectator; }
			set
			{
				if (m_definition.SessionSettings.EnableSpectator == value) return;
				m_definition.SessionSettings.EnableSpectator = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Determine whether the server will automatically remove debris")]
		[Category("World Settings")]
		/// <summary>
		/// Determine whether the server will automatically remove debris
		/// </summary>
		public bool RemoveTrash
		{
			get { return m_definition.SessionSettings.RemoveTrash; }
			set
			{
				if (m_definition.SessionSettings.RemoveTrash == value) return;
				m_definition.SessionSettings.RemoveTrash = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the world borders. Ships and players cannot go further than this")]
		[Category("World Settings")]
		/// <summary>
		/// Get or set the world borders. Ships and players cannot go further than this
		/// </summary>
		public int WorldSizeKm
		{
			get { return m_definition.SessionSettings.WorldSizeKm; }
			set
			{
				if (m_definition.SessionSettings.WorldSizeKm == value) return;
				m_definition.SessionSettings.WorldSizeKm = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Determine whether starter ships are removed after a while")]
		[Category("World Settings")]
		/// <summary>
		/// Determine whether starter ships are removed after a while
		/// </summary>
		public bool RespawnShipDelete
		{
			get { return m_definition.SessionSettings.RespawnShipDelete; }
			set
			{
				if (m_definition.SessionSettings.RespawnShipDelete == value) return;
				m_definition.SessionSettings.RespawnShipDelete = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Determine whether the server should reset the ships ownership")]
		[Category("World Settings")]
		/// <summary>
		/// Determine whether the server should reset the ships ownership
		/// </summary>
		public bool ResetOwnership
		{
			get { return m_definition.SessionSettings.ResetOwnership; }
			set
			{
				if (m_definition.SessionSettings.ResetOwnership == value) return;
				m_definition.SessionSettings.ResetOwnership = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the Scenario's TypeId")]
		[Category("World Settings")]
		/// <summary>
		/// Get or set the Scenario's TypeId
		/// </summary>
		public MyObjectBuilderTypeEnum ScenarioTypeId
		{
			get { return m_definition.Scenario.TypeId; }
			set
			{
				if (m_definition.Scenario.TypeId == value) return;
				m_definition.Scenario.TypeId = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the scenario's subtype Id")]
		[Category("World Settings")]
		/// <summary>
		/// Get or set the scenario's subtype Id
		/// </summary>
		public string ScenarioSubtypeId
		{
			get { return m_definition.Scenario.SubtypeId; }
			set
			{
				if (m_definition.Scenario.SubtypeId == value) return;
				m_definition.Scenario.SubtypeId = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the path of the world to load")]
		[EditorAttribute(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
		[Category("World Settings")]
		/// <summary>
		/// Get or set the path of the world to load
		/// </summary>
		public string LoadWorld
		{
			get { return m_definition.LoadWorld; }
			set
			{
				if (m_definition.LoadWorld == value) return;
				m_definition.LoadWorld = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the Ip the server will listen on. 0.0.0.0 to listen to every Ip")]
		[Category("Server Settings")]
		/// <summary>
		/// Get or set the Ip the server will listen on. 0.0.0.0 to listen to every Ip
		/// </summary>
		public string Ip
		{
			get { return m_definition.IP; }
			set
			{
				if (m_definition.IP == value) return;
				m_definition.IP = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the steam port")]
		[Category("Server Settings")]
		/// <summary>
		/// Get or set the steam port
		/// </summary>
		public int SteamPort
		{
			get { return m_definition.SteamPort; }
			set
			{
				if (m_definition.SteamPort == value) return;
				m_definition.SteamPort = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the number of asteroid in the world")]
		[Category("World Settings")]
		/// <summary>
		/// Get or set the number of asteroid in the world
		/// </summary>
		public int AsteroidAmount
		{
			get { return m_definition.AsteroidAmount; }
			set
			{
				if (m_definition.AsteroidAmount == value) return;
				m_definition.AsteroidAmount = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the list of administrators of the server")]
		[Category("Server Settings")]
		/// <summary>
		/// Get or set the list of administrators of the server
		/// </summary>
		public List<ulong> Administrators
		{
			get { return m_definition.Administrators; }
			set
			{
				if (m_definition.Administrators == value) return;
				m_definition.Administrators = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the list of banned players")]
		[Category("Server Settings")]
		/// <summary>
		/// Get or set the list of banned players
		/// </summary>
		public List<ulong> Banned
		{
			get { return m_definition.Banned; }
			set
			{
				if (m_definition.Banned == value) return;
				m_definition.Banned = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the GroupId of the server.\n" +
					"Only member of this group will be able to join the server.\n" +
					"Set to 0 to open the server to everyone")]
		[Category("Server Settings")]
		/// <summary>
		/// Get or set the GroupId of the server. 
		/// Only member of this group will be able to join the server.
		/// Set to 0 to open the server to everyone
		/// </summary>
		public ulong GroupID
		{
			get { return m_definition.GroupID; }
			set
			{
				if (m_definition.GroupID == value) return;
				m_definition.GroupID = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Load the dedicated server configuration file
		/// </summary>
		/// <param name="fileInfo">Path to the configuration file</param>
		/// <returns></returns>
		public static MyConfigDedicatedData Load(FileInfo fileInfo)
		{
			object fileContent = null;

			string filePath = fileInfo.FullName;

			if (!File.Exists(filePath))
			{
				throw new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileMissing, filePath);
			}

			try
			{
				var settings = new XmlReaderSettings
				{
					IgnoreComments = true,
					IgnoreWhitespace = true,
				};

				using (var xmlReader = XmlReader.Create(filePath, settings))
				{
					var serializer = (MyConfigDedicatedDataSerializer)Activator.CreateInstance(typeof(MyConfigDedicatedDataSerializer));
					fileContent = serializer.Deserialize(xmlReader);
				}
			}
			catch
			{
				throw new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileCorrupted, filePath);
			}

			if (fileContent == null)
			{
				throw new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileEmpty, filePath);
			}

			return (MyConfigDedicatedData)fileContent;
		}

		public bool Save(FileInfo fileInfo)
		{
			if (!this.Changed) return false;
			if (fileInfo == null) return false;

			//Save the definitions container out to the file
			try
			{
				using (var xmlTextWriter = new XmlTextWriter(fileInfo.FullName, null))
				{
					xmlTextWriter.Formatting = Formatting.Indented;
					xmlTextWriter.Indentation = 2;
					xmlTextWriter.IndentChar = ' ';
					MyConfigDedicatedDataSerializer serializer = (MyConfigDedicatedDataSerializer)Activator.CreateInstance(typeof(MyConfigDedicatedDataSerializer));
					serializer.Serialize(xmlTextWriter, m_definition);
				}
			}
			catch
			{
				throw new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileCorrupted, fileInfo.FullName);
			}

			if (m_definition == null)
			{
				throw new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileEmpty, fileInfo.FullName);
			}

			Changed = false;

			return true;
		}


		#endregion
	}
}
