using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using SEServerGUI.ServiceReference;
using SEServerGUI.ServerServiceReference;
using SEServerGUI.ChatServiceReference;
using SEServerGUI.PluginServiceReference;

using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;
using SEModAPIInternal.Support;

using VRageMath;

namespace SEServerGUI
{
	public partial class SEServerGUIForm : Form
	{
		#region "Attributes"

		private InternalServiceContractClient m_serviceClient;
		private ServerServiceContractClient m_serverClient;
		private ChatServiceContractClient m_chatClient;
		private PluginServiceContractClient m_pluginClient;

		private List<BaseEntityProxy> m_sectorEntities;
		private List<CubeGridEntityProxy> m_sectorCubeGridEntities;

		private System.Windows.Forms.Timer m_serverStatusCheckTimer;
		private System.Windows.Forms.Timer m_entityTreeRefreshTimer;
		private System.Windows.Forms.Timer m_chatViewRefreshTimer;
		private System.Windows.Forms.Timer m_pluginManagerRefreshTimer;

		private ServerProxy m_serverProxy;

		#endregion

		#region "Constructors and Initializers"

		public SEServerGUIForm()
		{
			InitializeComponent();

			m_serviceClient = new InternalServiceContractClient();
			m_serverClient = new ServerServiceContractClient();
			m_chatClient = new ChatServiceContractClient();
			m_pluginClient = new PluginServiceContractClient();

			m_sectorEntities = new List<BaseEntityProxy>();
			m_sectorCubeGridEntities = new List<CubeGridEntityProxy>();

			m_serverStatusCheckTimer = new System.Windows.Forms.Timer();
			m_serverStatusCheckTimer.Interval = 4000;
			m_serverStatusCheckTimer.Tick += new EventHandler(ServerStatusRefresh);

			m_entityTreeRefreshTimer = new System.Windows.Forms.Timer();
			m_entityTreeRefreshTimer.Interval = 1000;
			m_entityTreeRefreshTimer.Tick += new EventHandler(TreeViewRefresh);

			m_chatViewRefreshTimer = new System.Windows.Forms.Timer();
			m_chatViewRefreshTimer.Interval = 1000;
			m_chatViewRefreshTimer.Tick += new EventHandler(ChatViewRefresh);

			m_pluginManagerRefreshTimer = new System.Windows.Forms.Timer();
			m_pluginManagerRefreshTimer.Interval = 10000;
			m_pluginManagerRefreshTimer.Tick += new EventHandler(PluginManagerRefresh);

			CMB_Control_AutosaveInterval.BeginUpdate();
			CMB_Control_AutosaveInterval.Items.Add(1);
			CMB_Control_AutosaveInterval.Items.Add(2);
			CMB_Control_AutosaveInterval.Items.Add(5);
			CMB_Control_AutosaveInterval.Items.Add(10);
			CMB_Control_AutosaveInterval.Items.Add(30);
			CMB_Control_AutosaveInterval.SelectedIndex = 2;
			CMB_Control_AutosaveInterval.EndUpdate();
		}

		#endregion

		#region "Methods"

		private void Disconnect()
		{
			m_serverProxy = null;

			TRV_Entities.Nodes.Clear();
			LST_Chat_Messages.Items.Clear();
			LST_Chat_ConnectedPlayers.Items.Clear();
			LST_Plugins.Items.Clear();

			m_serverStatusCheckTimer.Stop();
			m_entityTreeRefreshTimer.Stop();
			m_chatViewRefreshTimer.Stop();
			m_pluginManagerRefreshTimer.Stop();

			BTN_StartServer.Enabled = false;
			BTN_StopServer.Enabled = false;
			BTN_Connect.Enabled = true;
			BTN_Chat_Send.Enabled = false;
			BTN_Plugins_Load.Enabled = false;
			BTN_Plugins_Unload.Enabled = false;

			TXT_Chat_Message.Enabled = false;

			CMB_Control_AutosaveInterval.Enabled = false;
			CMB_Control_AutosaveInterval.SelectedIndex = 2;

			CHK_Control_Debugging.Enabled = false;
			CHK_Control_Debugging.Checked = false;
		}

		#region "Control"

		private void ServerStatusRefresh(object sender, EventArgs e)
		{
			//Refresh the server
			try
			{
				m_serverProxy = m_serverClient.GetServer();
			}
			catch (Exception ex)
			{
				Disconnect();
				return;
			}

			CMB_Control_AutosaveInterval.Enabled = true;
			//CHK_Control_Debugging.Enabled = true;

			if (m_serverProxy.IsRunning)
			{
				BTN_StartServer.Enabled = false;
				BTN_StopServer.Enabled = true;
				BTN_Chat_Send.Enabled = true;

				TXT_Chat_Message.Enabled = true;
			}
			else
			{
				BTN_StartServer.Enabled = true;
				BTN_StopServer.Enabled = false;
				BTN_Chat_Send.Enabled = false;

				TXT_Chat_Message.Enabled = false;
			}

			int intervalMinutes = (int)Math.Round(m_serverProxy.AutosaveInterval / 60000.0);
			if(!CMB_Control_AutosaveInterval.ContainsFocus)
				CMB_Control_AutosaveInterval.SelectedItem = intervalMinutes;
		}

		private void BTN_Connect_Click(object sender, EventArgs e)
		{
			if (!m_serverStatusCheckTimer.Enabled)
				m_serverStatusCheckTimer.Start();
			if (!m_entityTreeRefreshTimer.Enabled)
				m_entityTreeRefreshTimer.Start();
			if (!m_chatViewRefreshTimer.Enabled)
				m_chatViewRefreshTimer.Start();
			if (!m_pluginManagerRefreshTimer.Enabled)
				m_pluginManagerRefreshTimer.Start();

			BTN_Connect.Enabled = false;
		}

		private void BTN_StartServer_Click(object sender, EventArgs e)
		{
			m_serverClient.StartServer();
		}

		private void BTN_StopServer_Click(object sender, EventArgs e)
		{
			Disconnect();

			m_serverClient.StopServer();
		}

		private void CMB_Control_AutosaveInterval_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!CMB_Control_AutosaveInterval.Enabled || CMB_Control_AutosaveInterval.SelectedIndex == -1) return;
			if (m_serverProxy == null)
				return;

			double interval = 2;
			try
			{
				interval = double.Parse(CMB_Control_AutosaveInterval.Text);
			}
			catch (Exception ex)
			{
				//Do something
			}

			m_serverClient.SetAutosaveInterval(interval * 60000);
		}

		private void CHK_Control_Debugging_CheckedChanged(object sender, EventArgs e)
		{

		}

		#endregion

		#region "Entities"

		private void UpdateNodeInventoryItemBranch<T>(TreeNode node, List<T> source)
			where T : InventoryItemEntityProxy
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
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		private void TreeViewRefresh(object sender, EventArgs e)
		{
			if (m_serverProxy == null)
				return;
			if (!m_serverProxy.IsRunning)
				return;

			//Refresh the entities
			try
			{
				m_sectorEntities = m_serviceClient.GetSectorEntities();
			}
			catch (Exception ex)
			{
				Disconnect();
				return;
			}

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
			sectorObjectsNode.Text = sectorObjectsNode.Name + " (" + m_sectorEntities.Count.ToString() + ")";

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

			//List<CubeGridEntityProxy> list = m_sectorCubeGridEntities;
			List<CubeGridEntityProxy> list = new List<CubeGridEntityProxy>();
			foreach (var entry in m_sectorEntities)
			{
				if (entry is CubeGridEntityProxy)
					list.Add((CubeGridEntityProxy)entry);
			}

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

					CubeGridEntityProxy item = (CubeGridEntityProxy)node.Tag;
					bool foundMatch = false;
					foreach (CubeGridEntityProxy listItem in list)
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
					LogManager.ErrorLog.WriteLine(ex);
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
					LogManager.ErrorLog.WriteLine(ex);
				}
			}

			//Update node text
			rootNode.Text = rootNode.Name + " (" + rootNode.Nodes.Count.ToString() + ")";
		}

		private void RenderCharacterNodes(TreeNode rootNode)
		{
			if (rootNode == null)
				return;

			List<CharacterEntityProxy> list = new List<CharacterEntityProxy>();
			foreach (var entry in m_sectorEntities)
			{
				if (entry is CharacterEntityProxy)
					list.Add((CharacterEntityProxy)entry);
			}

			//Cleanup and update the existing nodes
			foreach (TreeNode node in rootNode.Nodes)
			{
				try
				{
					if (node == null)
						continue;

					if (node.Tag != null && list.Contains(node.Tag))
					{
						CharacterEntityProxy item = (CharacterEntityProxy)node.Tag;

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
					LogManager.ErrorLog.WriteLine(ex);
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
					LogManager.ErrorLog.WriteLine(ex);
				}
			}

			//Update node text
			rootNode.Text = rootNode.Name + " (" + rootNode.Nodes.Count.ToString() + ")";
		}

		private void RenderVoxelMapNodes(TreeNode rootNode)
		{
			if (rootNode == null)
				return;

			List<VoxelMapProxy> list = new List<VoxelMapProxy>();
			foreach (var entry in m_sectorEntities)
			{
				if (entry is VoxelMapProxy)
					list.Add((VoxelMapProxy)entry);
			}

			//Cleanup and update the existing nodes
			foreach (TreeNode node in rootNode.Nodes)
			{
				try
				{
					if (node == null)
						continue;

					if (node.Tag != null && list.Contains(node.Tag))
					{
						VoxelMapProxy item = (VoxelMapProxy)node.Tag;

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
					LogManager.ErrorLog.WriteLine(ex);
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
					LogManager.ErrorLog.WriteLine(ex);
				}
			}

			//Update node text
			rootNode.Text = rootNode.Name + " (" + rootNode.Nodes.Count.ToString() + ")";
		}

		private void RenderFloatingObjectNodes(TreeNode rootNode)
		{
			if (rootNode == null)
				return;

			List<FloatingObjectProxy> list = new List<FloatingObjectProxy>();
			foreach (var entry in m_sectorEntities)
			{
				if (entry is FloatingObjectProxy)
					list.Add((FloatingObjectProxy)entry);
			}

			//Cleanup and update the existing nodes
			foreach (TreeNode node in rootNode.Nodes)
			{
				try
				{
					if (node == null)
						continue;

					if (node.Tag != null && list.Contains(node.Tag))
					{
						FloatingObjectProxy item = (FloatingObjectProxy)node.Tag;

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
					LogManager.ErrorLog.WriteLine(ex);
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
					LogManager.ErrorLog.WriteLine(ex);
				}
			}

			//Update node text
			rootNode.Text = rootNode.Name + " (" + rootNode.Nodes.Count.ToString() + ")";
		}

		private void RenderMeteorNodes(TreeNode rootNode)
		{
			if (rootNode == null)
				return;

			List<MeteorProxy> list = new List<MeteorProxy>();
			foreach (var entry in m_sectorEntities)
			{
				if (entry is MeteorProxy)
					list.Add((MeteorProxy)entry);
			}

			//Cleanup and update the existing nodes
			foreach (TreeNode node in rootNode.Nodes)
			{
				try
				{
					if (node == null)
						continue;

					if (node.Tag != null && list.Contains(node.Tag))
					{
						MeteorProxy item = (MeteorProxy)node.Tag;

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
					LogManager.ErrorLog.WriteLine(ex);
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
					LogManager.ErrorLog.WriteLine(ex);
				}
			}

			//Update node text
			rootNode.Text = rootNode.Name + " (" + rootNode.Nodes.Count.ToString() + ")";
		}

		private void RenderCubeGridChildNodes(CubeGridEntityProxy cubeGrid, TreeNode blocksNode)
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

			List<CubeBlockEntityProxy> cubeBlocks = m_serviceClient.GetCubeBlocks(cubeGrid.EntityId);

			foreach (var cubeBlock in cubeBlocks)
			{
				TreeNode newNode = new TreeNode(cubeBlock.Name);
				newNode.Name = newNode.Text;
				newNode.Tag = cubeBlock;

				Type cubeBlockType = cubeBlock.GetType();

				if (cubeBlockType == typeof(CubeBlockEntityProxy))
				{
					structuralBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is CargoContainerEntityProxy)
				{
					containerBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is ReactorEntityProxy)
				{
					energyBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is BatteryBlockEntityProxy)
				{
					energyBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is BeaconEntityProxy)
				{
					utilityBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is CockpitEntityProxy)
				{
					utilityBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is GravityGeneratorEntityProxy)
				{
					utilityBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is MedicalRoomEntityProxy)
				{
					utilityBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is DoorEntityProxy)
				{
					utilityBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is InteriorLightEntityProxy)
				{
					lightBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is ReflectorLightEntityProxy)
				{
					lightBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is RefineryEntityProxy)
				{
					productionBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is AssemblerEntityProxy)
				{
					productionBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is ConveyorBlockEntityProxy)
				{
					conveyorBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is ConveyorTubeEntityProxy)
				{
					conveyorBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is SolarPanelEntityProxy)
				{
					energyBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is GatlingTurretEntityProxy)
				{
					weaponBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is MissileTurretEntityProxy)
				{
					weaponBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is ShipGrinderEntityProxy)
				{
					toolBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is ShipWelderEntityProxy)
				{
					toolBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlock is ShipDrillEntityProxy)
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

			if (selectedNode.Tag == null)
				return;

			var linkedObject = selectedNode.Tag;
			PG_Entities_Details.SelectedObject = linkedObject;

			if (linkedObject is CubeGridEntityProxy)
			{
				TRV_Entities.BeginUpdate();

				RenderCubeGridChildNodes((CubeGridEntityProxy)linkedObject, e.Node);

				TRV_Entities.EndUpdate();
			}

			if (linkedObject is CharacterEntityProxy)
			{
				CharacterEntityProxy character = (CharacterEntityProxy)linkedObject;

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

			if (linkedObject is CargoContainerEntityProxy)
			{
				CargoContainerEntityProxy container = (CargoContainerEntityProxy)linkedObject;

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

			if (linkedObject is ReactorEntityProxy)
			{
				ReactorEntityProxy reactor = (ReactorEntityProxy)linkedObject;

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

			if (linkedObject is ShipToolBaseEntityProxy)
			{
				ShipToolBaseEntityProxy shipTool = (ShipToolBaseEntityProxy)linkedObject;

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

			if (linkedObject is ShipDrillEntityProxy)
			{
				ShipDrillEntityProxy shipDrill = (ShipDrillEntityProxy)linkedObject;

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

			if (linkedObject is ProductionBlockEntityProxy)
			{
				ProductionBlockEntityProxy productionBlock = (ProductionBlockEntityProxy)linkedObject;

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

			if (linkedObject is InventoryEntityProxy)
			{
				InventoryEntityProxy inventory = (InventoryEntityProxy)linkedObject;

				if(parentNode.Tag is CubeBlockEntityProxy && parentNode.Parent.Parent.Tag is CubeGridEntityProxy)
				{
					CubeGridEntityProxy cubeGrid = (CubeGridEntityProxy)parentNode.Parent.Parent.Tag;
					CubeBlockEntityProxy cubeBlock = (CubeBlockEntityProxy)parentNode.Tag;

					long cubeGridEntityId = cubeGrid.EntityId;
					long cubeBlockEntityId = cubeBlock.EntityId;
					ushort inventoryIndex = 0;
					List<InventoryItemEntityProxy> inventoryItems = m_serviceClient.GetInventoryItems(cubeGridEntityId, cubeBlockEntityId, inventoryIndex);

					UpdateNodeInventoryItemBranch<InventoryItemEntityProxy>(e.Node, inventoryItems);
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

		#endregion

		#region "Chat"

		private void ChatViewRefresh(object sender, EventArgs e)
		{
			if (m_serverProxy == null)
				return;
			if (!m_serverProxy.IsRunning)
				return;

			BTN_Chat_Send.Enabled = true;
			TXT_Chat_Message.Enabled = true;

			//Refresh the chat messages
			string[] chatMessages;
			try
			{
				chatMessages = m_chatClient.GetChatMessages().ToArray();
			}
			catch (Exception ex)
			{
				Disconnect();
				return;
			}

			LST_Chat_Messages.BeginUpdate();

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

			List<ulong> connectedPlayers = m_serviceClient.GetConnectedPlayers();
			if (connectedPlayers.Count != LST_Chat_ConnectedPlayers.Items.Count)
			{
				LST_Chat_ConnectedPlayers.Items.Clear();
				foreach (ulong remoteUserId in connectedPlayers)
				{
					string playerName = m_serviceClient.GetPlayerName(remoteUserId);

					LST_Chat_ConnectedPlayers.Items.Add(playerName);
				}
			}

			LST_Chat_ConnectedPlayers.EndUpdate();
		}

		private void BTN_Chat_Send_Click(object sender, EventArgs e)
		{
			string message = TXT_Chat_Message.Text;
			if (message != null && message != "")
			{
				m_chatClient.SendPublicChatMessage(message);
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
					m_chatClient.SendPublicChatMessage(message);
					TXT_Chat_Message.Text = "";
				}
			}
		}

		#endregion

		#region "Plugins"

		private void PluginManagerRefresh(object sender, EventArgs e)
		{
			if (m_serverProxy == null)
				return;
			if (!m_serverProxy.IsRunning)
				return;

			List<Guid> pluginGuids;
			try
			{
				pluginGuids = m_pluginClient.GetPluginGuids();
			}
			catch (Exception ex)
			{
				Disconnect();
				return;
			}

			if (pluginGuids.Count == LST_Plugins.Items.Count)
				return;

			LST_Plugins.BeginUpdate();
			int selectedIndex = LST_Plugins.SelectedIndex;
			LST_Plugins.Items.Clear();
			foreach (var key in pluginGuids)
			{
				LST_Plugins.Items.Add(key);
			}
			LST_Plugins.SelectedIndex = selectedIndex;
			LST_Plugins.EndUpdate();
		}

		private void LST_Plugins_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (LST_Plugins.SelectedItem == null)
				return;

			Guid selectedItem = (Guid)LST_Plugins.SelectedItem;

			PG_Plugins.SelectedObject = selectedItem;

			bool pluginState = m_pluginClient.GetPluginStatus(selectedItem);
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

		private void BTN_Plugins_Refresh_Click(object sender, EventArgs e)
		{

		}

		private void BTN_Plugins_Load_Click(object sender, EventArgs e)
		{
			if (LST_Plugins.SelectedItem == null)
				return;

			Guid selectedItem = (Guid)LST_Plugins.SelectedItem;

			m_pluginClient.LoadPlugin(selectedItem);
		}

		private void BTN_Plugins_Unload_Click(object sender, EventArgs e)
		{
			if (LST_Plugins.SelectedItem == null)
				return;

			Guid selectedItem = (Guid)LST_Plugins.SelectedItem;

			m_pluginClient.UnloadPlugin(selectedItem);
		}

		#endregion

		#endregion
	}
}
