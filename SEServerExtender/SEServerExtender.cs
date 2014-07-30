using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.Voxels;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.API;
using SEModAPI.API.Definitions;
using SEModAPI.Support;

using SEModAPIExtensions.API;

using SEModAPIInternal.API;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Entity.Sector;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;
using SEModAPIInternal.API.Server;
using SEModAPIInternal.Support;

using VRage.Common.Utils;
using VRageMath;

namespace SEServerExtender
{
	public partial class SEServerExtender : Form
	{
		#region "Attributes"

		//General
		private static SEServerExtender m_instance;
		private Server m_server;

		//Timers
		private System.Windows.Forms.Timer m_entityTreeRefreshTimer;
		private System.Windows.Forms.Timer m_chatViewRefreshTimer;
		private System.Windows.Forms.Timer m_factionRefreshTimer;
		private System.Windows.Forms.Timer m_pluginManagerRefreshTimer;
		private System.Windows.Forms.Timer m_statusCheckTimer;
		private System.Windows.Forms.Timer m_utilitiesCleanFloatingObjectsTimer;

		//Utilities Page
		private int m_floatingObjectsCount;

		#endregion

		#region "Constructors and Initializers"

		public SEServerExtender(Server server)
		{
			m_instance = this;
			m_server = server;

			//Run init functions
			InitializeComponent();
			if (!SetupTimers())
				Close();
			if (!SetupControls())
				Close();
			m_server.LoadServerConfig();
			UpdateControls();
			PG_Control_Server_Properties.SelectedObject = m_server.Config;

			//Update the title bar text with the assembly version
			this.Text = "SEServerExtender " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
		}

		private bool SetupTimers()
		{
			m_entityTreeRefreshTimer = new System.Windows.Forms.Timer();
			m_entityTreeRefreshTimer.Interval = 500;
			m_entityTreeRefreshTimer.Tick += new EventHandler(TreeViewRefresh);

			m_chatViewRefreshTimer = new System.Windows.Forms.Timer();
			m_chatViewRefreshTimer.Interval = 1000;
			m_chatViewRefreshTimer.Tick += new EventHandler(ChatViewRefresh);

			m_factionRefreshTimer = new System.Windows.Forms.Timer();
			m_factionRefreshTimer.Interval = 5000;
			m_factionRefreshTimer.Tick += new EventHandler(FactionRefresh);

			m_pluginManagerRefreshTimer = new System.Windows.Forms.Timer();
			m_pluginManagerRefreshTimer.Interval = 10000;
			m_pluginManagerRefreshTimer.Tick += new EventHandler(PluginManagerRefresh);

			m_statusCheckTimer = new System.Windows.Forms.Timer();
			m_statusCheckTimer.Interval = 500;
			m_statusCheckTimer.Tick += new EventHandler(StatusCheckRefresh);
			m_statusCheckTimer.Start();

			m_utilitiesCleanFloatingObjectsTimer = new System.Windows.Forms.Timer();
			m_utilitiesCleanFloatingObjectsTimer.Interval = 1000;
			m_utilitiesCleanFloatingObjectsTimer.Tick += new EventHandler(UtilitiesCleanFloatingObjects);

			return true;
		}

		private bool SetupControls()
		{
			try
			{
				List<String> instanceList = SandboxGameAssemblyWrapper.Instance.GetCommonInstanceList();
				CMB_Control_CommonInstanceList.BeginUpdate();
				CMB_Control_CommonInstanceList.Items.AddRange(instanceList.ToArray());
				if (CMB_Control_CommonInstanceList.Items.Count > 0)
					CMB_Control_CommonInstanceList.SelectedIndex = 0;
				CMB_Control_CommonInstanceList.EndUpdate();

				CMB_Control_AutosaveInterval.BeginUpdate();
				CMB_Control_AutosaveInterval.Items.Add(1);
				CMB_Control_AutosaveInterval.Items.Add(2);
				CMB_Control_AutosaveInterval.Items.Add(5);
				CMB_Control_AutosaveInterval.Items.Add(10);
				CMB_Control_AutosaveInterval.Items.Add(30);
				CMB_Control_AutosaveInterval.SelectedIndex = 2;
				CMB_Control_AutosaveInterval.EndUpdate();
			}
			catch (AutoException)
			{
				return false;
			}

			return true;
		}

		#endregion

		#region "Methods"

		#region "General"

		private void StatusCheckRefresh(object sender, EventArgs e)
		{
			UpdateControls();

			if (!m_server.IsRunning && m_server.CommandLineArgs.autoStart)
			{
				BTN_ServerControl_Start.PerformClick();
			}

			if (m_server.IsRunning)
			{
				if(!m_entityTreeRefreshTimer.Enabled)
					m_entityTreeRefreshTimer.Start();
				if (!m_chatViewRefreshTimer.Enabled)
					m_chatViewRefreshTimer.Start();
				if (!m_factionRefreshTimer.Enabled)
					m_factionRefreshTimer.Start();
				if (!m_pluginManagerRefreshTimer.Enabled)
					m_pluginManagerRefreshTimer.Start();
			}
		}

		private string GetGamePath()
		{
			bool continueLoad = true;

			OpenFileDialog OFD_GamePath = new OpenFileDialog();

			string steamPath = GameInstallationInfo.GetGameSteamPath();
			if (steamPath != null)
				OFD_GamePath.InitialDirectory = Path.Combine(steamPath, "SteamApps", "common");

			while (continueLoad)
			{
				DialogResult resultOpen = OFD_GamePath.ShowDialog();
				if (resultOpen == DialogResult.OK)
				{
					string selectedPath = Path.GetDirectoryName(OFD_GamePath.FileName);
					string gamePath = Path.Combine(selectedPath, "..");
					if (GameInstallationInfo.IsValidGamePath(gamePath))
						return gamePath;
					else
					{
						DialogResult resultRetry = MessageBox.Show("The selected location is an invalid SpaceEngineers installation.",
							"Invalid installation", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

						if (resultRetry != DialogResult.Retry)
							continueLoad = false;
					}
				}
				else
					continueLoad = false;
			}

			//If this point is reached, then the user must have cancelled it.
			MessageBox.Show("The game installation location could not be found. The application can not run without it.",
				"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

			return null;
		}

		#endregion

		#region "Control"

		private void BTN_ServerControl_Start_Click(object sender, EventArgs e)
		{
			m_entityTreeRefreshTimer.Start();
			m_chatViewRefreshTimer.Start();
			m_factionRefreshTimer.Start();
			m_pluginManagerRefreshTimer.Start();

			if (m_server.IsRunning)
				return;

			if (m_server.Config != null && m_server.Config.Changed)
				m_server.SaveServerConfig();

			m_server.StartServer();
		}

		private void BTN_ServerControl_Stop_Click(object sender, EventArgs e)
		{
			m_entityTreeRefreshTimer.Stop();
			m_chatViewRefreshTimer.Stop();
			m_factionRefreshTimer.Stop();
			m_pluginManagerRefreshTimer.Stop();
			m_utilitiesCleanFloatingObjectsTimer.Stop();

			m_server.StopServer();
		}

		private void CHK_Control_Debugging_CheckedChanged(object sender, EventArgs e)
		{
			SandboxGameAssemblyWrapper.IsDebugging = CHK_Control_Debugging.CheckState == CheckState.Checked;
		}

		private void CHK_Control_CommonDataPath_CheckedChanged(object sender, EventArgs e)
		{
			SandboxGameAssemblyWrapper.UseCommonProgramData = CHK_Control_CommonDataPath.CheckState == CheckState.Checked;
			CMB_Control_CommonInstanceList.Enabled = SandboxGameAssemblyWrapper.UseCommonProgramData;

			if (SandboxGameAssemblyWrapper.UseCommonProgramData)
				m_server.InstanceName = CMB_Control_CommonInstanceList.Text;
			else
				m_server.InstanceName = "";

			m_server.LoadServerConfig();

			PG_Control_Server_Properties.SelectedObject = m_server.Config;
		}

		private void BTN_Control_Server_Reset_Click(object sender, EventArgs e)
		{
			//Refresh the loaded config
			m_server.LoadServerConfig();
			UpdateControls();

			PG_Control_Server_Properties.SelectedObject = m_server.Config;
		}

		private void BTN_Control_Server_Save_Click(object sender, EventArgs e)
		{
			//Save the loaded config
			m_server.SaveServerConfig();
			UpdateControls();
		}

		private void PG_Control_Server_Properties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			UpdateControls();
		}

		private void CMB_Control_CommonInstanceList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!CMB_Control_CommonInstanceList.Enabled || CMB_Control_CommonInstanceList.SelectedIndex == -1) return;

			m_server.InstanceName = CMB_Control_CommonInstanceList.Text;
			SandboxGameAssemblyWrapper.Instance.InitMyFileSystem(CMB_Control_CommonInstanceList.Text);

			m_server.LoadServerConfig();

			PG_Control_Server_Properties.SelectedObject = m_server.Config;
		}

		private void CMB_Control_AutosaveInterval_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!CMB_Control_AutosaveInterval.Enabled || CMB_Control_AutosaveInterval.SelectedIndex == -1) return;

			double interval = 2;
			try
			{
				interval = double.Parse(CMB_Control_AutosaveInterval.Text);
			}
			catch (Exception ex)
			{
				//Do something
			}

			m_server.AutosaveInterval = interval * 60000;
		}

		private void UpdateControls()
		{
			if (m_server.Config == null)
				SandboxGameAssemblyWrapper.UseCommonProgramData = true;

			if (m_server.CommandLineArgs.instanceName.Length != 0)
			{
				CHK_Control_CommonDataPath.Checked = true;
				foreach (var item in CMB_Control_CommonInstanceList.Items)
				{
					if (item.ToString().Equals(m_server.CommandLineArgs.instanceName))
					{
						CMB_Control_CommonInstanceList.SelectedItem = item;
						break;
					}
				}
			}

			if (m_server.IsRunning)
				CHK_Control_Debugging.Checked = SandboxGameAssemblyWrapper.IsDebugging;
			if (m_server.IsRunning && m_server.CommandLineArgs.instanceName.Length > 0)
				CMB_Control_CommonInstanceList.SelectedText = m_server.CommandLineArgs.instanceName;

			BTN_ServerControl_Stop.Enabled = m_server.IsRunning;
			BTN_ServerControl_Start.Enabled = !m_server.IsRunning;
			BTN_Chat_Send.Enabled = m_server.IsRunning;
			if(!m_server.IsRunning)
				BTN_Entities_New.Enabled = false;
			BTN_Utilities_ClearFloatingObjectsNow.Enabled = m_server.IsRunning;

			if (CHK_Control_CommonDataPath.CheckState == CheckState.Checked)
				CMB_Control_CommonInstanceList.Enabled = !m_server.IsRunning;
			else
				CMB_Control_CommonInstanceList.Enabled = false;

			TXT_Chat_Message.Enabled = m_server.IsRunning;

			PG_Entities_Details.Enabled = m_server.IsRunning;
			PG_Factions.Enabled = m_server.IsRunning;
			PG_Plugins.Enabled = m_server.IsRunning;
			PG_Control_Server_Properties.Enabled = !m_server.IsRunning;

			if (m_server.Config != null)
			{
				CHK_Control_CommonDataPath.Enabled = !m_server.IsRunning;

				BTN_Control_Server_Save.Enabled = m_server.Config.Changed;
				BTN_Control_Server_Reset.Enabled = m_server.Config.Changed;
			}
			else
			{
				CHK_Control_CommonDataPath.Checked = true;

				BTN_Control_Server_Save.Enabled = false;
				BTN_Control_Server_Reset.Enabled = false;
			}
		}

		#endregion

		#region "Entities"

		private void UpdateNodeInventoryItemBranch<T>(TreeNode node, List<T> source)
			where T : InventoryItemEntity
		{
			try
			{
				bool entriesChanged = (node.Nodes.Count != source.Count);
				if (entriesChanged)
				{
					node.Nodes.Clear();
					node.Text = node.Name + " (" + source.Count.ToString() + ")";
				}

				int index = 0;
				foreach (var item in source)
				{
					TreeNode itemNode = null;
					if (entriesChanged)
					{
						itemNode = node.Nodes.Add(item.Name);
						itemNode.Tag = item;
					}
					else
					{
						itemNode = node.Nodes[index];
						itemNode.Text = item.Name;
						itemNode.Tag = item;
					}

					index++;
				}
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		private void TreeViewRefresh(object sender, EventArgs e)
		{
			if (!SandboxGameAssemblyWrapper.Instance.IsGameStarted)
				return;

			TRV_Entities.BeginUpdate();

			TreeNode sectorObjectsNode;
			TreeNode sectorEventsNode;

			if (TRV_Entities.Nodes.Count < 2)
			{
				sectorObjectsNode = TRV_Entities.Nodes.Add("Sector Objects");
				sectorEventsNode = TRV_Entities.Nodes.Add("Sector Events");

				sectorObjectsNode.Name = sectorObjectsNode.Text;
				sectorEventsNode.Name = sectorEventsNode.Text;
			}
			else
			{
				sectorObjectsNode = TRV_Entities.Nodes[0];
				sectorEventsNode = TRV_Entities.Nodes[1];
			}

			RenderSectorObjectChildNodes(sectorObjectsNode);
			sectorObjectsNode.Text = sectorObjectsNode.Name + " (" + SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>().Count.ToString() + ")";
			sectorObjectsNode.Tag = SectorObjectManager.Instance;

			TRV_Entities.EndUpdate();
		}

		private void RenderSectorObjectChildNodes(TreeNode objectsNode)
		{
			if (TRV_Entities.IsDisposed)
				return;

			TreeNode cubeGridsNode;
			TreeNode charactersNode;
			TreeNode voxelMapsNode;
			TreeNode floatingObjectsNode;
			TreeNode meteorsNode;

			if (objectsNode.Nodes.Count < 5)
			{
				objectsNode.Nodes.Clear();

				cubeGridsNode = objectsNode.Nodes.Add("Cube Grids");
				charactersNode = objectsNode.Nodes.Add("Characters");
				voxelMapsNode = objectsNode.Nodes.Add("Voxel Maps");
				floatingObjectsNode = objectsNode.Nodes.Add("Floating Objects");
				meteorsNode = objectsNode.Nodes.Add("Meteors");

				cubeGridsNode.Name = cubeGridsNode.Text;
				charactersNode.Name = charactersNode.Text;
				voxelMapsNode.Name = voxelMapsNode.Text;
				floatingObjectsNode.Name = floatingObjectsNode.Text;
				meteorsNode.Name = meteorsNode.Text;
			}
			else
			{
				cubeGridsNode = objectsNode.Nodes[0];
				charactersNode = objectsNode.Nodes[1];
				voxelMapsNode = objectsNode.Nodes[2];
				floatingObjectsNode = objectsNode.Nodes[3];
				meteorsNode = objectsNode.Nodes[4];
			}

			RenderCubeGridNodes(cubeGridsNode);
			RenderCharacterNodes(charactersNode);
			RenderVoxelMapNodes(voxelMapsNode);
			RenderFloatingObjectNodes(floatingObjectsNode);
			RenderMeteorNodes(meteorsNode);
		}

		private void RenderCubeGridNodes(TreeNode rootNode)
		{
			if (rootNode == null)
				return;

			//Get cube grids from sector object manager
			List<CubeGridEntity> list = SectorObjectManager.Instance.GetTypedInternalData<CubeGridEntity>();

			//Cleanup and update the existing nodes
			foreach (TreeNode node in rootNode.Nodes)
			{
				try
				{
					if (node == null)
						continue;
					if (node.Tag == null)
					{
						node.Remove();
						continue;
					}

					CubeGridEntity item = (CubeGridEntity)node.Tag;
					bool foundMatch = false;
					foreach (CubeGridEntity listItem in list)
					{
						if (listItem.EntityId == item.EntityId)
						{
							foundMatch = true;

							Vector3 rawPosition = item.Position;
							double distance = Math.Round(rawPosition.Length(), 0);
							string newNodeText = item.Name + " | Mass: " + Math.Floor(item.Mass).ToString() + "kg | Dist: " + distance.ToString() + "m";
							node.Text = newNodeText;

							list.Remove(listItem);

							break;
						}
					}

					if (!foundMatch)
					{
						node.Remove();
						continue;
					}
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
				}
			}

			//Add new nodes
			foreach (var item in list)
			{
				try
				{
					if (item == null)
						continue;

					Vector3 rawPosition = item.Position;
					double distance = rawPosition.Length();

					Type sectorObjectType = item.GetType();
					string nodeKey = item.EntityId.ToString();

					TreeNode newNode = rootNode.Nodes.Add(nodeKey, item.Name + " | Mass: " + Math.Floor(item.Mass).ToString() + "kg | Dist: " + distance.ToString() + "m");
					newNode.Name = item.Name;
					newNode.Tag = item;
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
				}
			}

			//Update node text
			rootNode.Text = rootNode.Name + " (" + rootNode.Nodes.Count.ToString() + ")";
		}

		private void RenderCharacterNodes(TreeNode rootNode)
		{
			if (rootNode == null)
				return;

			//Get entities from sector object manager
			List<CharacterEntity> list = SectorObjectManager.Instance.GetTypedInternalData<CharacterEntity>();

			//Cleanup and update the existing nodes
			foreach (TreeNode node in rootNode.Nodes)
			{
				try
				{
					if (node == null)
						continue;

					if (node.Tag != null && list.Contains(node.Tag))
					{
						CharacterEntity item = (CharacterEntity)node.Tag;

						if (!item.IsDisposed)
						{
							Vector3 rawPosition = item.Position;
							double distance = Math.Round(rawPosition.Length(), 0);
							string newNodeText = item.Name + " | Dist: " + distance.ToString() + "m";
							node.Text = newNodeText;
						}
						list.Remove(item);
					}
					else
					{
						node.Remove();
					}
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
				}
			}

			//Add new nodes
			foreach (var item in list)
			{
				try
				{
					if (item == null)
						continue;

					Vector3 rawPosition = item.Position;
					double distance = rawPosition.Length();

					Type sectorObjectType = item.GetType();
					string nodeKey = item.EntityId.ToString();

					TreeNode newNode = rootNode.Nodes.Add(nodeKey, item.Name + " | Dist: " + distance.ToString() + "m");
					newNode.Name = item.Name;
					newNode.Tag = item;
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
				}
			}

			//Update node text
			rootNode.Text = rootNode.Name + " (" + rootNode.Nodes.Count.ToString() + ")";
		}

		private void RenderVoxelMapNodes(TreeNode rootNode)
		{
			if (rootNode == null)
				return;

			//Get entities from sector object manager
			List<VoxelMap> list = SectorObjectManager.Instance.GetTypedInternalData<VoxelMap>();

			//Cleanup and update the existing nodes
			foreach (TreeNode node in rootNode.Nodes)
			{
				try
				{
					if (node == null)
						continue;

					if (node.Tag != null && list.Contains(node.Tag))
					{
						VoxelMap item = (VoxelMap)node.Tag;

						if (!item.IsDisposed)
						{
							Vector3 rawPosition = item.Position;
							double distance = Math.Round(rawPosition.Length(), 0);
							string newNodeText = item.Name + " | Dist: " + distance.ToString() + "m";
							node.Text = newNodeText;
						}
						list.Remove(item);
					}
					else
					{
						node.Remove();
					}
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
				}
			}

			//Add new nodes
			foreach (var item in list)
			{
				try
				{
					if (item == null)
						continue;

					Vector3 rawPosition = item.Position;
					double distance = rawPosition.Length();

					Type sectorObjectType = item.GetType();
					string nodeKey = item.EntityId.ToString();

					TreeNode newNode = rootNode.Nodes.Add(nodeKey, item.Name + " | Dist: " + distance.ToString() + "m");
					newNode.Name = item.Name;
					newNode.Tag = item;
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
				}
			}

			//Update node text
			rootNode.Text = rootNode.Name + " (" + rootNode.Nodes.Count.ToString() + ")";
		}

		private void RenderFloatingObjectNodes(TreeNode rootNode)
		{
			if (rootNode == null)
				return;

			//Get entities from sector object manager
			List<FloatingObject> list = SectorObjectManager.Instance.GetTypedInternalData<FloatingObject>();

			//Cleanup and update the existing nodes
			foreach (TreeNode node in rootNode.Nodes)
			{
				try
				{
					if (node == null)
						continue;

					if (node.Tag != null && list.Contains(node.Tag))
					{
						FloatingObject item = (FloatingObject)node.Tag;

						if (!item.IsDisposed && item.Item != null)
						{
							Vector3 rawPosition = item.Position;
							double distance = Math.Round(rawPosition.Length(), 0);
							string newNodeText = item.Name + " | Amount: " + item.Item.Amount.ToString() + " | Dist: " + distance.ToString() + "m";
							node.Text = newNodeText;
						}
						list.Remove(item);
					}
					else
					{
						node.Remove();
					}
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
				}
			}

			//Add new nodes
			foreach (var item in list)
			{
				try
				{
					if (item == null)
						continue;
					if (item.IsDisposed)
						continue;
					if (item.Item == null)
						continue;

					Vector3 rawPosition = item.Position;
					double distance = rawPosition.Length();

					Type sectorObjectType = item.GetType();
					string nodeKey = item.EntityId.ToString();

					TreeNode newNode = rootNode.Nodes.Add(nodeKey, item.Name + " | Amount: " + item.Item.Amount.ToString() + " | Dist: " + distance.ToString() + "m");
					newNode.Name = item.Name;
					newNode.Tag = item;
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
				}
			}

			//Update node text
			rootNode.Text = rootNode.Name + " (" + rootNode.Nodes.Count.ToString() + ")";

			// Update a var for the Utilities Floating object cleaner.
			m_floatingObjectsCount = rootNode.Nodes.Count;
		}

		private void RenderMeteorNodes(TreeNode rootNode)
		{
			if (rootNode == null)
				return;

			//Get entities from sector object manager
			List<Meteor> list = SectorObjectManager.Instance.GetTypedInternalData<Meteor>();

			//Cleanup and update the existing nodes
			foreach (TreeNode node in rootNode.Nodes)
			{
				try
				{
					if (node == null)
						continue;

					if (node.Tag != null && list.Contains(node.Tag))
					{
						Meteor item = (Meteor)node.Tag;

						if (!item.IsDisposed)
						{
							Vector3 rawPosition = item.Position;
							double distance = Math.Round(rawPosition.Length(), 0);
							string newNodeText = item.Name + " | Dist: " + distance.ToString() + "m";
							node.Text = newNodeText;
						}
						list.Remove(item);
					}
					else
					{
						node.Remove();
					}
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
				}
			}

			//Add new nodes
			foreach (var item in list)
			{
				try
				{
					if (item == null)
						continue;

					Vector3 rawPosition = item.Position;
					double distance = rawPosition.Length();

					string nodeKey = item.EntityId.ToString();
					TreeNode newNode = rootNode.Nodes.Add(nodeKey, item.Name + " | Dist: " + distance.ToString() + "m");
					newNode.Name = item.Name;
					newNode.Tag = item;
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
				}
			}

			//Update node text
			rootNode.Text = rootNode.Name + " (" + rootNode.Nodes.Count.ToString() + ")";
		}

		private void RenderCubeGridChildNodes(CubeGridEntity cubeGrid, TreeNode blocksNode)
		{
			TreeNode structuralBlocksNode;
			TreeNode containerBlocksNode;
			TreeNode productionBlocksNode;
			TreeNode energyBlocksNode;
			TreeNode conveyorBlocksNode;
			TreeNode utilityBlocksNode;
			TreeNode weaponBlocksNode;
			TreeNode toolBlocksNode;
			TreeNode lightBlocksNode;
			TreeNode miscBlocksNode;

			if (blocksNode.Nodes.Count < 9)
			{
				structuralBlocksNode = blocksNode.Nodes.Add("Structural");
				containerBlocksNode = blocksNode.Nodes.Add("Containers");
				productionBlocksNode = blocksNode.Nodes.Add("Refinement and Production");
				energyBlocksNode = blocksNode.Nodes.Add("Energy");
				conveyorBlocksNode = blocksNode.Nodes.Add("Conveyor");
				utilityBlocksNode = blocksNode.Nodes.Add("Utility");
				weaponBlocksNode = blocksNode.Nodes.Add("Weapons");
				toolBlocksNode = blocksNode.Nodes.Add("Tools");
				lightBlocksNode = blocksNode.Nodes.Add("Lights");
				miscBlocksNode = blocksNode.Nodes.Add("Misc");

				structuralBlocksNode.Name = structuralBlocksNode.Text;
				containerBlocksNode.Name = containerBlocksNode.Text;
				productionBlocksNode.Name = productionBlocksNode.Text;
				energyBlocksNode.Name = energyBlocksNode.Text;
				conveyorBlocksNode.Name = conveyorBlocksNode.Text;
				utilityBlocksNode.Name = utilityBlocksNode.Text;
				weaponBlocksNode.Name = weaponBlocksNode.Text;
				toolBlocksNode.Name = toolBlocksNode.Text;
				lightBlocksNode.Name = lightBlocksNode.Text;
				miscBlocksNode.Name = miscBlocksNode.Text;
			}
			else
			{
				structuralBlocksNode = blocksNode.Nodes[0];
				containerBlocksNode = blocksNode.Nodes[1];
				productionBlocksNode = blocksNode.Nodes[2];
				energyBlocksNode = blocksNode.Nodes[3];
				conveyorBlocksNode = blocksNode.Nodes[4];
				utilityBlocksNode = blocksNode.Nodes[5];
				weaponBlocksNode = blocksNode.Nodes[6];
				toolBlocksNode = blocksNode.Nodes[7];
				lightBlocksNode = blocksNode.Nodes[8];
				miscBlocksNode = blocksNode.Nodes[9];

				structuralBlocksNode.Nodes.Clear();
				containerBlocksNode.Nodes.Clear();
				productionBlocksNode.Nodes.Clear();
				energyBlocksNode.Nodes.Clear();
				conveyorBlocksNode.Nodes.Clear();
				utilityBlocksNode.Nodes.Clear();
				weaponBlocksNode.Nodes.Clear();
				toolBlocksNode.Nodes.Clear();
				lightBlocksNode.Nodes.Clear();
				miscBlocksNode.Nodes.Clear();
			}

			foreach (var cubeBlock in cubeGrid.CubeBlocks)
			{
				TreeNode newNode = new TreeNode(cubeBlock.Name);
				newNode.Name = newNode.Text;
				newNode.Tag = cubeBlock;

				Type cubeBlockType = cubeBlock.GetType();

				if (cubeBlockType == typeof(CubeBlockEntity))
				{
					structuralBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is CargoContainerEntity)
				{
					containerBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is ReactorEntity)
				{
					energyBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is BatteryBlockEntity)
				{
					energyBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is BeaconEntity)
				{
					utilityBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is CockpitEntity)
				{
					utilityBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is GravityGeneratorEntity)
				{
					utilityBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is MedicalRoomEntity)
				{
					utilityBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is DoorEntity)
				{
					utilityBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is InteriorLightEntity)
				{
					lightBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is ReflectorLightEntity)
				{
					lightBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is RefineryEntity)
				{
					productionBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is AssemblerEntity)
				{
					productionBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is ConveyorBlockEntity)
				{
					conveyorBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is ConveyorTubeEntity)
				{
					conveyorBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is SolarPanelEntity)
				{
					energyBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is GatlingTurretEntity)
				{
					weaponBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is MissileTurretEntity)
				{
					weaponBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is ShipGrinderEntity)
				{
					toolBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is ShipWelderEntity)
				{
					toolBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is ShipDrillEntity)
				{
					toolBlocksNode.Nodes.Add(newNode);
				}
				else
				{
					miscBlocksNode.Nodes.Add(newNode);
				}
			}

			structuralBlocksNode.Text = structuralBlocksNode.Name + " (" + structuralBlocksNode.Nodes.Count.ToString() + ")";
			containerBlocksNode.Text = containerBlocksNode.Name + " (" + containerBlocksNode.Nodes.Count.ToString() + ")";
			productionBlocksNode.Text = productionBlocksNode.Name + " (" + productionBlocksNode.Nodes.Count.ToString() + ")";
			energyBlocksNode.Text = energyBlocksNode.Name + " (" + energyBlocksNode.Nodes.Count.ToString() + ")";
			conveyorBlocksNode.Text = conveyorBlocksNode.Name + " (" + conveyorBlocksNode.Nodes.Count.ToString() + ")";
			utilityBlocksNode.Text = utilityBlocksNode.Name + " (" + utilityBlocksNode.Nodes.Count.ToString() + ")";
			weaponBlocksNode.Text = weaponBlocksNode.Name + " (" + weaponBlocksNode.Nodes.Count.ToString() + ")";
			toolBlocksNode.Text = toolBlocksNode.Name + " (" + toolBlocksNode.Nodes.Count.ToString() + ")";
			lightBlocksNode.Text = lightBlocksNode.Name + " (" + lightBlocksNode.Nodes.Count.ToString() + ")";
			miscBlocksNode.Text = miscBlocksNode.Name + " (" + miscBlocksNode.Nodes.Count.ToString() + ")";
		}

		private void TRV_Entities_NodeRefresh(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (e.Clicks < 2)
				return;
			if (e.Node == null)
				return;
			if (e.Node.Tag == null)
				return;

			//Clear the child nodes
			e.Node.Nodes.Clear();

			//Call the main node select event handler to populate the node
			TreeViewEventArgs newEvent = new TreeViewEventArgs(e.Node);
			TRV_Entities_AfterSelect(sender, newEvent);
		}

		private void TRV_Entities_AfterSelect(object sender, TreeViewEventArgs e)
		{
			BTN_Entities_Export.Enabled = false;
			BTN_Entities_New.Enabled = false;
			BTN_Entities_Delete.Enabled = false;

			TreeNode selectedNode = e.Node;

			if (selectedNode == null)
				return;

			TreeNode parentNode = e.Node.Parent;

			if (parentNode == null)
				return;

			if (parentNode.Tag != null && parentNode.Tag is SectorObjectManager)
			{
				if (selectedNode == parentNode.Nodes[0])
				{
					BTN_Entities_New.Enabled = true;
				}
			}

			if (selectedNode.Tag == null)
				return;

			var linkedObject = selectedNode.Tag;
			PG_Entities_Details.SelectedObject = linkedObject;

			//Enable export for all objects that inherit from BaseObject
			if (linkedObject is BaseObject)
			{
				BTN_Entities_Export.Enabled = true;
			}

			//Enable new and delete for all objects that inherit from BaseEntity
			if (linkedObject is BaseEntity)
			{
				BTN_Entities_Delete.Enabled = true;
			}

			if (linkedObject is CubeGridEntity)
			{
				BTN_Entities_New.Enabled = true;

				TRV_Entities.BeginUpdate();

				RenderCubeGridChildNodes((CubeGridEntity)linkedObject, e.Node);

				TRV_Entities.EndUpdate();
			}

			if (linkedObject is CharacterEntity)
			{
				CharacterEntity character = (CharacterEntity)linkedObject;

				if (e.Node.Nodes.Count < 1)
				{
					TRV_Entities.BeginUpdate();

					e.Node.Nodes.Clear();
					TreeNode itemsNode = e.Node.Nodes.Add("Items");
					itemsNode.Name = itemsNode.Text;
					itemsNode.Tag = character.Inventory;

					TRV_Entities.EndUpdate();
				}
			}

			if (linkedObject is CargoContainerEntity)
			{
				CargoContainerEntity container = (CargoContainerEntity)linkedObject;

				if (e.Node.Nodes.Count < 1)
				{
					TRV_Entities.BeginUpdate();

					e.Node.Nodes.Clear();
					TreeNode itemsNode = e.Node.Nodes.Add("Items");
					itemsNode.Name = itemsNode.Text;
					itemsNode.Tag = container.Inventory;

					TRV_Entities.EndUpdate();
				}
			}

			if (linkedObject is ReactorEntity)
			{
				ReactorEntity reactor = (ReactorEntity)linkedObject;

				if (e.Node.Nodes.Count < 1)
				{
					TRV_Entities.BeginUpdate();

					e.Node.Nodes.Clear();
					TreeNode itemsNode = e.Node.Nodes.Add("Items");
					itemsNode.Name = itemsNode.Text;
					itemsNode.Tag = reactor.Inventory;

					TRV_Entities.EndUpdate();
				}
			}

			if (linkedObject is ShipToolBaseEntity)
			{
				ShipToolBaseEntity shipTool = (ShipToolBaseEntity)linkedObject;

				if (e.Node.Nodes.Count < 1)
				{
					TRV_Entities.BeginUpdate();

					e.Node.Nodes.Clear();
					TreeNode itemsNode = e.Node.Nodes.Add("Items");
					itemsNode.Name = itemsNode.Text;
					itemsNode.Tag = shipTool.Inventory;

					TRV_Entities.EndUpdate();
				}
			}

			if (linkedObject is ShipDrillEntity)
			{
				ShipDrillEntity shipDrill = (ShipDrillEntity)linkedObject;

				if (e.Node.Nodes.Count < 1)
				{
					TRV_Entities.BeginUpdate();

					e.Node.Nodes.Clear();
					TreeNode itemsNode = e.Node.Nodes.Add("Items");
					itemsNode.Name = itemsNode.Text;
					itemsNode.Tag = shipDrill.Inventory;

					TRV_Entities.EndUpdate();
				}
			}

			if (linkedObject is ProductionBlockEntity)
			{
				ProductionBlockEntity productionBlock = (ProductionBlockEntity)linkedObject;

				if (e.Node.Nodes.Count < 2)
				{
					TRV_Entities.BeginUpdate();

					e.Node.Nodes.Clear();
					TreeNode inputNode = e.Node.Nodes.Add("Input");
					inputNode.Name = inputNode.Text;
					inputNode.Tag = productionBlock.InputInventory;
					TreeNode outputNode = e.Node.Nodes.Add("Output");
					outputNode.Name = outputNode.Text;
					outputNode.Tag = productionBlock.OutputInventory;

					TRV_Entities.EndUpdate();
				}
			}

			if (linkedObject is InventoryEntity)
			{
				BTN_Entities_New.Enabled = true;

				InventoryEntity inventory = (InventoryEntity)linkedObject;

				UpdateNodeInventoryItemBranch<InventoryItemEntity>(e.Node, inventory.Items);
			}

			if (linkedObject is InventoryItemEntity)
			{
				BTN_Entities_New.Enabled = true;
				BTN_Entities_Delete.Enabled = true;
			}
		}

		private void BTN_Entities_Delete_Click(object sender, EventArgs e)
		{
			try
			{
				Object linkedObject = TRV_Entities.SelectedNode.Tag;
				if (!(linkedObject is BaseObject))
					return;

				BaseObject baseObject = (BaseObject)linkedObject;
				baseObject.Dispose();

				TreeNode parentNode = TRV_Entities.SelectedNode.Parent;
				TRV_Entities.SelectedNode.Tag = null;
				TreeNode newSelectedNode = TRV_Entities.SelectedNode.NextVisibleNode;
				if (newSelectedNode == null)
					newSelectedNode = TRV_Entities.SelectedNode.PrevVisibleNode;
				if (newSelectedNode == null)
					newSelectedNode = parentNode.FirstNode;
				TRV_Entities.SelectedNode.Remove();
				if (parentNode.FirstNode != null)
					PG_Entities_Details.SelectedObject = parentNode.FirstNode.Tag;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		private void BTN_Entities_New_Click(object sender, EventArgs e)
		{
			TreeNode selectedNode = TRV_Entities.SelectedNode;

			if (selectedNode == null)
				return;

			TreeNode parentNode = selectedNode.Parent;

			if (parentNode == null)
				return;

			if (parentNode.Tag != null && parentNode.Tag is SectorObjectManager)
			{
				if (selectedNode == parentNode.Nodes[0])
				{
					CreateCubeGridImportDialog();
					return;
				}
			}

			if (selectedNode.Tag == null)
				return;

			if (!(selectedNode.Tag is BaseObject))
				return;

			BaseObject linkedObject = (BaseObject)selectedNode.Tag;

			if (linkedObject is InventoryEntity)
			{
				InventoryItemDialog newItemDialog = new InventoryItemDialog();
				newItemDialog.Container = (InventoryEntity)linkedObject;
				newItemDialog.ShowDialog(this);
				return;
			}

			if(linkedObject is InventoryItemEntity)
			{
				InventoryItemDialog newItemDialog = new InventoryItemDialog();
				newItemDialog.Container = ((InventoryItemEntity)linkedObject).Container;
				newItemDialog.ShowDialog(this);
				return;
			}

			if(linkedObject is CubeGridEntity)
			{
				CreateCubeGridImportDialog();
				return;
			}
		}

		private void BTN_Entities_Export_Click(object sender, EventArgs e)
		{
			if (TRV_Entities.SelectedNode == null)
				return;
			Object linkedObject = TRV_Entities.SelectedNode.Tag;
			if (linkedObject == null)
				return;
			if (!(linkedObject is BaseObject))
				return;

			BaseObject objectToExport = (BaseObject)linkedObject;

			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "sbc file (*.sbc)|*.sbc|All files (*.*)|*.*";
			saveFileDialog.InitialDirectory = GameInstallationInfo.GamePath;

			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				FileInfo fileInfo = new FileInfo(saveFileDialog.FileName);
				try
				{
					objectToExport.Export(fileInfo);
				}
				catch (Exception ex)
				{
					MessageBox.Show(this, ex.Message);
				}
			}
		}

		private void PG_Entities_Details_Click(object sender, EventArgs e)
		{
			TreeNode node = TRV_Entities.SelectedNode;
			if (node == null)
				return;
			var linkedObject = node.Tag;
			PG_Entities_Details.SelectedObject = linkedObject;
		}

		private void CreateCubeGridImportDialog()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				InitialDirectory = GameInstallationInfo.GamePath,
				DefaultExt = "sbc file (*.sbc)"
			};

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
				if (fileInfo.Exists)
				{
					try
					{
						CubeGridEntity newEntity = new CubeGridEntity(fileInfo);

						if (newEntity != null)
							SectorObjectManager.Instance.AddEntity(newEntity);
					}
					catch (Exception ex)
					{
						LogManager.GameLog.WriteLine(ex);
						MessageBox.Show(this, ex.ToString());
					}
				}
			}
		}

		#endregion

		#region "Chat"

		private void ChatViewRefresh(object sender, EventArgs e)
		{
			if (m_server.CommandLineArgs.noGUI)
				return;

			LST_Chat_Messages.BeginUpdate();

			string[] chatMessages = ChatManager.Instance.ChatMessages.ToArray();
			if (chatMessages.Length != LST_Chat_Messages.Items.Count)
			{
				LST_Chat_Messages.Items.Clear();
				LST_Chat_Messages.Items.AddRange(chatMessages);

				//Auto-scroll to the bottom of the list
				LST_Chat_Messages.SelectedIndex = LST_Chat_Messages.Items.Count - 1;
				LST_Chat_Messages.SelectedIndex = -1;
			}
			LST_Chat_Messages.EndUpdate();

			LST_Chat_ConnectedPlayers.BeginUpdate();

			List<ulong> connectedPlayers = ServerNetworkManager.Instance.GetConnectedPlayers();
			if (connectedPlayers.Count != LST_Chat_ConnectedPlayers.Items.Count)
			{
				LST_Chat_ConnectedPlayers.Items.Clear();
				foreach (ulong remoteUserId in connectedPlayers)
				{
					LST_Chat_ConnectedPlayers.Items.Add(remoteUserId.ToString());
				}
			}

			LST_Chat_ConnectedPlayers.EndUpdate();
		}

		private void BTN_Chat_Send_Click(object sender, EventArgs e)
		{
			string message = TXT_Chat_Message.Text;
			if (message != null && message != "")
			{
				ChatManager.Instance.SendPublicChatMessage(message);
				TXT_Chat_Message.Text = "";
			}
		}

		private void TXT_Chat_Message_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				string message = TXT_Chat_Message.Text;
				if (message != null && message != "")
				{
					ChatManager.Instance.SendPublicChatMessage(message);
					TXT_Chat_Message.Text = "";
				}
			}
		}

		#endregion

		#region "Factions"

		private void FactionRefresh(object sender, EventArgs e)
		{
			try
			{
				if (SandboxGameAssemblyWrapper.Instance.IsGameStarted)
				{
					TRV_Factions.BeginUpdate();

					List<Faction> list = FactionsManager.Instance.Factions;

					//Cleanup and update the existing nodes
					foreach (TreeNode node in TRV_Factions.Nodes)
					{
						try
						{
							if (node == null)
								continue;
							if (node.Tag == null)
							{
								node.Remove();
								continue;
							}

							Faction item = (Faction)node.Tag;
							bool foundMatch = false;
							foreach (Faction faction in list)
							{
								if (faction.Id == item.Id)
								{
									foundMatch = true;

									string newNodeText = item.Name + " (" + item.Members.Count.ToString() + ")";
									node.Text = newNodeText;

									TreeNode membersNode = node.Nodes[0];
									TreeNode joinRequestsNode = node.Nodes[1];

									if (membersNode.Nodes.Count != item.Members.Count)
									{
										membersNode.Nodes.Clear();
										foreach (FactionMember member in item.Members)
										{
											TreeNode memberNode = membersNode.Nodes.Add(member.PlayerId.ToString(), member.PlayerId.ToString());
											memberNode.Name = member.PlayerId.ToString();
											memberNode.Tag = member;
										}
									}
									if (joinRequestsNode.Nodes.Count != item.JoinRequests.Count)
									{
										joinRequestsNode.Nodes.Clear();
										foreach (FactionMember member in item.JoinRequests)
										{
											TreeNode joinRequestNode = joinRequestsNode.Nodes.Add(member.PlayerId.ToString(), member.PlayerId.ToString());
											joinRequestNode.Name = member.PlayerId.ToString();
											joinRequestNode.Tag = member;
										}
									}

									list.Remove(faction);

									break;
								}
							}

							if (!foundMatch)
							{
								node.Remove();
								continue;
							}
						}
						catch (Exception ex)
						{
							LogManager.GameLog.WriteLine(ex);
						}
					}

					//Add new nodes
					foreach (var item in list)
					{
						try
						{
							if (item == null)
								continue;

							Type sectorObjectType = item.GetType();
							string nodeKey = item.Id.ToString();

							TreeNode newNode = TRV_Factions.Nodes.Add(nodeKey, item.Name + " (" + item.Members.Count.ToString() + ")");
							newNode.Name = item.Name;
							newNode.Tag = item;

							TreeNode membersNode = newNode.Nodes.Add("Members");
							TreeNode joinRequestsNode = newNode.Nodes.Add("Join Requests");

							foreach (FactionMember member in item.Members)
							{
								TreeNode memberNode = membersNode.Nodes.Add(member.PlayerId.ToString(), member.PlayerId.ToString());
								memberNode.Name = member.PlayerId.ToString();
								memberNode.Tag = member;
							}
							foreach (FactionMember member in item.JoinRequests)
							{
								TreeNode memberNode = membersNode.Nodes.Add(member.PlayerId.ToString(), member.PlayerId.ToString());
								memberNode.Name = member.PlayerId.ToString();
								memberNode.Tag = member;
							}
						}
						catch (Exception ex)
						{
							LogManager.GameLog.WriteLine(ex);
						}
					}

					TRV_Factions.EndUpdate();
				}
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		private void TRV_Factions_AfterSelect(object sender, TreeViewEventArgs e)
		{
			BTN_Factions_Delete.Enabled = false;

			if (e.Node == null)
				return;
			if (e.Node.Tag == null)
				return;

			var linkedObject = e.Node.Tag;

			BTN_Factions_Delete.Enabled = true;

			PG_Factions.SelectedObject = linkedObject;

			//DEBUG
			if (e.Node.Text.Equals("Join Requests"))
				BTN_Factions_Delete.Enabled = false;
		}

		private void BTN_Factions_Delete_Click(object sender, EventArgs e)
		{
			TreeNode node = TRV_Factions.SelectedNode;
			if (node == null)
				return;
			if (node.Tag == null)
				return;

			var linkedObject = node.Tag;

			if (linkedObject is Faction)
			{
				Faction faction = (Faction)linkedObject;
				FactionsManager.Instance.RemoveFaction(faction.Id);
			}

			if (linkedObject is FactionMember)
			{
				FactionMember member = (FactionMember)linkedObject;
				member.Parent.RemoveMember(member.PlayerId);
			}
		}

		#endregion

		#region "Plugins"

		private void PluginManagerRefresh(object sender, EventArgs e)
		{
			if (PluginManager.Instance.Initialized)
			{
				if (PluginManager.Instance.Plugins.Count == LST_Plugins.Items.Count)
					return;

				LST_Plugins.BeginUpdate();
				int selectedIndex = LST_Plugins.SelectedIndex;
				LST_Plugins.Items.Clear();
				foreach (var key in PluginManager.Instance.Plugins.Keys)
				{
					LST_Plugins.Items.Add(key);
				}
				LST_Plugins.SelectedIndex = selectedIndex;
				LST_Plugins.EndUpdate();
			}
		}

		private void LST_Plugins_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (LST_Plugins.SelectedItem == null)
				return;

			Guid selectedItem = (Guid)LST_Plugins.SelectedItem;
			Object plugin = PluginManager.Instance.Plugins[selectedItem];

			PG_Plugins.SelectedObject = plugin;

			bool pluginState = PluginManager.Instance.GetPluginState(selectedItem);
			if (pluginState)
			{
				BTN_Plugins_Load.Enabled = false;
				BTN_Plugins_Unload.Enabled = true;
			}
			else
			{
				BTN_Plugins_Load.Enabled = true;
				BTN_Plugins_Unload.Enabled = false;
			}
		}

		private void BTN_Plugins_Unload_Click(object sender, EventArgs e)
		{
			if (LST_Plugins.SelectedItem == null)
				return;

			Guid selectedItem = (Guid)LST_Plugins.SelectedItem;
			PluginManager.Instance.UnloadPlugin(selectedItem);
		}

		private void BTN_Plugins_Load_Click(object sender, EventArgs e)
		{
			if (LST_Plugins.SelectedItem == null)
				return;

			Guid selectedItem = (Guid)LST_Plugins.SelectedItem;
			PluginManager.Instance.InitPlugin(selectedItem);
		}

		private void BTN_Plugins_Refresh_Click(object sender, EventArgs e)
		{
			PluginManager.Instance.LoadPlugins(m_server.CommandLineArgs.instanceName);
		}

		#endregion

		#region "Utilities"

		// Get all floating objects and dispose of them.
		private void BeginClearObjects()
		{
			try
			{
				Console.WriteLine("[{0}] Clearing floating objects...", DateTime.Now.ToString());
				BTN_Utilities_ClearFloatingObjectsNow.Enabled = false;

				List<FloatingObject> floatingObjectList = SectorObjectManager.Instance.GetTypedInternalData<FloatingObject>();
				foreach (FloatingObject sectorObject in floatingObjectList)
				{
					sectorObject.Dispose();
				}

				BTN_Utilities_ClearFloatingObjectsNow.Enabled = true;
				Console.WriteLine("[{0}] Floating objects cleared!", DateTime.Now.ToString());
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.ToString());
			}
		}

		// Start the Auto Clean timer if user checks the auto clean checkbox.
		private void CHK_Utilities_FloatingObjectAutoClean_CheckedChanged(object sender, EventArgs e)
		{
			if (CHK_Utilities_FloatingObjectAutoClean.Checked == true)
			{
				m_utilitiesCleanFloatingObjectsTimer.Start();
			}
			else
			{
				if (m_utilitiesCleanFloatingObjectsTimer.Enabled == true)
				{
					m_utilitiesCleanFloatingObjectsTimer.Enabled = false;
					m_utilitiesCleanFloatingObjectsTimer.Stop();
				}
			}
		}

		// Delete all floating objects when the count reaches the amount set.
		private void UtilitiesCleanFloatingObjects(object sender, EventArgs e)
		{
			if (m_floatingObjectsCount > TXT_Utilities_FloatingObjectAmount.IntValue)
			{
				BTN_Utilities_ClearFloatingObjectsNow_Click(null, null);
			}
		}

		// Delete all floating objects now
		private void BTN_Utilities_ClearFloatingObjectsNow_Click(object sender, EventArgs e)
		{
			// Start the floating object cleaner on a seperate thread to avoid hanging the UI.
			System.Threading.Tasks.Task.Factory.StartNew(() => BeginClearObjects());
		}

		#endregion

		#endregion
	}
}
