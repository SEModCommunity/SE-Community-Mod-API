using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Voxels;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.API;
using SEModAPI.Support;

using SEModAPIInternal.API;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Entity.Sector;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;
using SEModAPIInternal.API.Server;
using SEModAPIInternal.Support;

using SEServerExtender.API;

namespace SEServerExtender
{
	public partial class SEServerExtender : Form
	{
		#region "Attributes"

		private ProcessWrapper m_processWrapper;
		private Timer m_entityTreeRefreshTimer;
		private Timer m_chatViewRefreshTimer;

		#endregion

		#region "Constructors and Initializers"

		public SEServerExtender()
		{
			//Determine wether or not we could find the game installation
			try
			{
				new GameInstallationInfo();
			}
			catch (AutoException)
			{
				string gamePath = GetGamePath();
				if (gamePath == null || gamePath == "")
				{
					//If the game path was not found, we skip all initialisation
					this.Visible = false;
					return;
				}
				new GameInstallationInfo(gamePath);
			}

			InitializeComponent();

			m_processWrapper = new ProcessWrapper();

			m_entityTreeRefreshTimer = new Timer();
			m_entityTreeRefreshTimer.Interval = 500;
			m_entityTreeRefreshTimer.Tick += new EventHandler(TreeViewRefresh);

			m_chatViewRefreshTimer = new Timer();
			m_chatViewRefreshTimer.Interval = 500;
			m_chatViewRefreshTimer.Tick += new EventHandler(ChatViewRefresh);

			TRV_Entities.Nodes.Add("Cube Grids (0)");
			TRV_Entities.Nodes.Add("Characters (0)");
			TRV_Entities.Nodes.Add("Voxel Maps (0)");
			TRV_Entities.Nodes.Add("Floating Objects (0)");
			TRV_Entities.Nodes.Add("Meteors (0)");
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Try to find manually the SpaceEngineers game path
		/// </summary>
		/// <returns>The game path, or null if not found</returns>
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

		void UpdateNodeInventoryItemBranch<T>(TreeNode node, List<T> source)
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

		void UpdateNodeBranch<T>(TreeNode node, List<T> source, string name)
			where T : BaseEntity
		{
			try
			{
				bool entriesChanged = (node.Nodes.Count != source.Count);
				if (entriesChanged)
				{
					node.Nodes.Clear();
					node.Text = name + " (" + source.Count.ToString() + ")";
				}

				int index = 0;
				foreach (var item in source)
				{
					SerializableVector3 rawPosition = item.Position;
					double distance = Math.Round(Math.Sqrt(rawPosition.X * rawPosition.X + rawPosition.Y * rawPosition.Y + rawPosition.Z * rawPosition.Z), 2);

					TreeNode itemNode = null;
					if (entriesChanged)
					{
						itemNode = node.Nodes.Add(item.Name + " | Dist: " + distance.ToString() + "m");
						itemNode.Tag = item;
					}
					else
					{
						itemNode = node.Nodes[index];
						itemNode.Text = item.Name + " | Dist: " + distance.ToString() + "m";
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

		void ChatViewRefresh(object sender, EventArgs e)
		{
			LST_Chat_Messages.BeginUpdate();

			string[] chatMessages = ChatManager.GetInstance().ChatMessages.ToArray();
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

			List<ulong> connectedPlayers = SandboxGameAssemblyWrapper.GetInstance().GetConnectedPlayers();
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

		void TreeViewRefresh(object sender, EventArgs e)
		{
			TRV_Entities.BeginUpdate();

			List<CubeGridEntity> cubeGrids = SandboxGameAssemblyWrapper.GetCubeGrids();
			List<CharacterEntity> characters = SandboxGameAssemblyWrapper.GetCharacters();
			List<VoxelMap> voxelMaps = SandboxGameAssemblyWrapper.GetVoxelMaps();
			List<FloatingObject> floatingObjects = SandboxGameAssemblyWrapper.GetFloatingObjects();
			List<Meteor> meteors = SandboxGameAssemblyWrapper.GetMeteors();

			UpdateNodeBranch<CubeGridEntity>(TRV_Entities.Nodes[0], cubeGrids, "Cube Grids");
			UpdateNodeBranch<CharacterEntity>(TRV_Entities.Nodes[1], characters, "Characters");
			UpdateNodeBranch<VoxelMap>(TRV_Entities.Nodes[2], voxelMaps, "Voxel Maps");
			UpdateNodeBranch<FloatingObject>(TRV_Entities.Nodes[3], floatingObjects, "Floating Objects");
			UpdateNodeBranch<Meteor>(TRV_Entities.Nodes[4], meteors, "Meteors");

			TRV_Entities.EndUpdate();
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
				miscBlocksNode = blocksNode.Nodes.Add("Misc");

				structuralBlocksNode.Name = structuralBlocksNode.Text;
				containerBlocksNode.Name = containerBlocksNode.Text;
				productionBlocksNode.Name = productionBlocksNode.Text;
				energyBlocksNode.Name = energyBlocksNode.Text;
				conveyorBlocksNode.Name = conveyorBlocksNode.Text;
				utilityBlocksNode.Name = utilityBlocksNode.Text;
				weaponBlocksNode.Name = weaponBlocksNode.Text;
				toolBlocksNode.Name = toolBlocksNode.Text;
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
				miscBlocksNode = blocksNode.Nodes[8];

				structuralBlocksNode.Nodes.Clear();
				containerBlocksNode.Nodes.Clear();
				productionBlocksNode.Nodes.Clear();
				energyBlocksNode.Nodes.Clear();
				conveyorBlocksNode.Nodes.Clear();
				utilityBlocksNode.Nodes.Clear();
				weaponBlocksNode.Nodes.Clear();
				toolBlocksNode.Nodes.Clear();
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
				else if (cubeBlockType == typeof(CargoContainerEntity))
				{
					containerBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlockType == typeof(ReactorEntity))
				{
					energyBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlockType == typeof(BeaconEntity))
				{
					utilityBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlockType == typeof(CockpitEntity))
				{
					utilityBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlockType == typeof(GravityGeneratorEntity))
				{
					utilityBlocksNode.Nodes.Add(newNode);
				}
				else if (cubeBlockType == typeof(MedicalRoomEntity))
				{
					utilityBlocksNode.Nodes.Add(newNode);
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
			miscBlocksNode.Text = miscBlocksNode.Name + " (" + miscBlocksNode.Nodes.Count.ToString() + ")";
		}

		private void BTN_ServerControl_Start_Click(object sender, EventArgs e)
		{
			m_processWrapper.StartGame("");

			m_entityTreeRefreshTimer.Start();
			m_chatViewRefreshTimer.Start();
		}

		private void BTN_ServerControl_Stop_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Not yet implemented");
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

			if (e.Node == null)
				return;
			if (e.Node.Tag == null)
				return;

			var linkedObject = e.Node.Tag;
			PG_Entities_Details.SelectedObject = linkedObject;

			if (linkedObject is FloatingObject)
			{
				BTN_Entities_Export.Enabled = true;
				BTN_Entities_Delete.Enabled = true;
			}

			if (linkedObject is VoxelMap)
			{
				BTN_Entities_Export.Enabled = true;
				BTN_Entities_Delete.Enabled = true;
			}

			if (linkedObject is Meteor)
			{
				BTN_Entities_Export.Enabled = true;
				BTN_Entities_Delete.Enabled = true;
			}

			if (linkedObject is CubeGridEntity)
			{
				BTN_Entities_Export.Enabled = true;
				BTN_Entities_New.Enabled = true;
				BTN_Entities_Delete.Enabled = true;

				TRV_Entities.BeginUpdate();

				RenderCubeGridChildNodes((CubeGridEntity)linkedObject, e.Node);

				TRV_Entities.EndUpdate();
			}

			if (linkedObject is CharacterEntity)
			{
				BTN_Entities_Export.Enabled = true;

				CharacterEntity character = (CharacterEntity)linkedObject;

				InventoryEntity inventory = character.Inventory;

				TRV_Entities.BeginUpdate();

				UpdateNodeInventoryItemBranch<InventoryItemEntity>(e.Node, inventory.Items);

				TRV_Entities.EndUpdate();
			}

			if (linkedObject is CargoContainerEntity)
			{
				CargoContainerEntity container = (CargoContainerEntity)linkedObject;

				InventoryEntity inventory = container.Inventory;

				TRV_Entities.BeginUpdate();

				UpdateNodeInventoryItemBranch<InventoryItemEntity>(e.Node, inventory.Items);

				TRV_Entities.EndUpdate();
			}

			if (linkedObject is ReactorEntity)
			{
				ReactorEntity reactor = (ReactorEntity)linkedObject;

				InventoryEntity inventory = reactor.Inventory;

				TRV_Entities.BeginUpdate();

				UpdateNodeInventoryItemBranch<InventoryItemEntity>(e.Node, inventory.Items);

				TRV_Entities.EndUpdate();
			}
		}

		private void BTN_Entities_Delete_Click(object sender, EventArgs e)
		{
			Object linkedObject = TRV_Entities.SelectedNode.Tag;
			if (linkedObject is BaseEntity)
			{
				try
				{
					BaseEntity item = (BaseEntity)linkedObject;
					item.Dispose();

					PG_Entities_Details.SelectedObject = null;
					TRV_Entities.SelectedNode.Tag = null;
					TRV_Entities.SelectedNode.Remove();
					BTN_Entities_Delete.Enabled = false;
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString());
				}
			}
		}

		private void BTN_Entities_New_Click(object sender, EventArgs e)
		{
			if (TRV_Entities.SelectedNode.Text.Contains("Cube Grids") || TRV_Entities.SelectedNode.Tag is CubeGridEntity)
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
							CubeGridEntity cubeGrid = new CubeGridEntity(fileInfo);
							bool result = cubeGrid.AddCubeGrid();
						}
						catch (Exception ex)
						{
							MessageBox.Show(this, ex.ToString());
						}
					}
				}
			}
		}

		private void CHK_Control_Debugging_CheckedChanged(object sender, EventArgs e)
		{
			SandboxGameAssemblyWrapper.IsDebugging = CHK_Control_Debugging.CheckState == CheckState.Checked;
		}

		private void CHK_Control_EnableFactions_CheckedChanged(object sender, EventArgs e)
		{
			SandboxGameAssemblyWrapper.EnableFactions(CHK_Control_EnableFactions.CheckState == CheckState.Checked);
		}

		private void BTN_Entities_Export_Click(object sender, EventArgs e)
		{
			Object linkedObject = TRV_Entities.SelectedNode.Tag;
			if (linkedObject is CubeGridEntity)
			{
				CubeGridEntity item = (CubeGridEntity)linkedObject;
				MyObjectBuilder_CubeGrid cubeGrid = item.GetSubTypeEntity();

				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.Filter = "sbc file (*.sbc)|*.sbc|All files (*.*)|*.*";
				saveFileDialog.InitialDirectory = GameInstallationInfo.GamePath;

				if (saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					FileInfo fileInfo = new FileInfo(saveFileDialog.FileName);
					try
					{
						item.Export(fileInfo);
					}
					catch (Exception ex)
					{
						MessageBox.Show(this, ex.Message);
					}
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

		private void BTN_Chat_Send_Click(object sender, EventArgs e)
		{
			string message = TXT_Chat_Message.Text;
			if (message != null && message != "")
			{
				ChatManager.GetInstance().SendPublicChatMessage(message);
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
					ChatManager.GetInstance().SendPublicChatMessage(message);
					TXT_Chat_Message.Text = "";
				}
			}
		}

		#endregion
	}
}
