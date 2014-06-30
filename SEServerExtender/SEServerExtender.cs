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

		void UpdateNodeInventoryItemBranch<T>(TreeNode node, List<T> source, string name)
			where T : InventoryItemEntity
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

		void UpdateNodeCubeBlockBranch<T>(TreeNode node, List<T> source, string name)
			where T : CubeBlockEntity
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

			string[] chatMessages = SandboxGameAssemblyWrapper.GetInstance().ChatMessages.ToArray();
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

			List<CubeGridEntity> cubeGrids = BaseEntityManagerWrapper.GetInstance().GetCubeGrids();
			List<CharacterEntity> characters = BaseEntityManagerWrapper.GetInstance().GetCharacters();
			List<VoxelMap> voxelMaps = BaseEntityManagerWrapper.GetInstance().GetVoxelMaps();
			List<FloatingObject> floatingObjects = BaseEntityManagerWrapper.GetInstance().GetFloatingObjects();
			List<Meteor> meteors = BaseEntityManagerWrapper.GetInstance().GetMeteors();

			UpdateNodeBranch<CubeGridEntity>(TRV_Entities.Nodes[0], cubeGrids, "Cube Grids");
			UpdateNodeBranch<CharacterEntity>(TRV_Entities.Nodes[1], characters, "Characters");
			UpdateNodeBranch<VoxelMap>(TRV_Entities.Nodes[2], voxelMaps, "Voxel Maps");
			UpdateNodeBranch<FloatingObject>(TRV_Entities.Nodes[3], floatingObjects, "Floating Objects");
			UpdateNodeBranch<Meteor>(TRV_Entities.Nodes[4], meteors, "Meteors");

			TRV_Entities.EndUpdate();
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

		private void TRV_Entities_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node == null)
				return;
			var linkedObject = e.Node.Tag;

			PG_Entities_Details.SelectedObject = linkedObject;
			BTN_Entities_New.Enabled = false;
			BTN_Entities_Export.Enabled = false;

			if (linkedObject == null)
			{
				BTN_Entities_Delete.Enabled = false;
				return;
			}
			else
			{
				BTN_Entities_Delete.Enabled = true;
			}

			if (linkedObject is CubeGridEntity)
			{
				BTN_Entities_New.Enabled = true;
				BTN_Entities_Export.Enabled = true;

				if (e.Node.Nodes.Count < 2)
				{
					e.Node.Nodes.Add("Structural Blocks (0)");
					e.Node.Nodes.Add("Container Blocks (0)");
					e.Node.Nodes.Add("Reactor Blocks (0)");
				}

				CubeGridEntity cubeGrid = (CubeGridEntity)linkedObject;

				List<CubeBlockEntity> structuralBlocks = CubeBlockInternalWrapper.GetInstance().GetStructuralBlocks(cubeGrid);
				List<CargoContainerEntity> containerBlocks = CubeBlockInternalWrapper.GetInstance().GetCargoContainerBlocks(cubeGrid);
				List<ReactorEntity> reactorBlocks = CubeBlockInternalWrapper.GetInstance().GetReactorBlocks(cubeGrid);

				TRV_Entities.BeginUpdate();

				UpdateNodeCubeBlockBranch<CubeBlockEntity>(e.Node.Nodes[0], structuralBlocks, "Structural Blocks");
				UpdateNodeCubeBlockBranch<CargoContainerEntity>(e.Node.Nodes[1], containerBlocks, "Container Blocks");
				UpdateNodeCubeBlockBranch<ReactorEntity>(e.Node.Nodes[2], reactorBlocks, "Reactor Blocks");

				TRV_Entities.EndUpdate();
			}

			if (linkedObject is CharacterEntity)
			{
				CharacterEntity character = (CharacterEntity)linkedObject;

				InventoryEntity inventory = character.Inventory;

				TRV_Entities.BeginUpdate();

				UpdateNodeInventoryItemBranch<InventoryItemEntity>(e.Node, inventory.Items, e.Node.Text);

				TRV_Entities.EndUpdate();
			}

			if (linkedObject is CargoContainerEntity)
			{
				CargoContainerEntity container = (CargoContainerEntity)linkedObject;

				InventoryEntity inventory = container.Inventory;

				TRV_Entities.BeginUpdate();

				UpdateNodeInventoryItemBranch<InventoryItemEntity>(e.Node, inventory.Items, e.Node.Text);

				TRV_Entities.EndUpdate();
			}

			if (linkedObject is ReactorEntity)
			{
				ReactorEntity reactor = (ReactorEntity)linkedObject;

				InventoryEntity inventory = reactor.Inventory;

				TRV_Entities.BeginUpdate();

				UpdateNodeInventoryItemBranch<InventoryItemEntity>(e.Node, inventory.Items, e.Node.Text);

				TRV_Entities.EndUpdate();
			}
		}

		private void BTN_Entities_Delete_Click(object sender, EventArgs e)
		{
			Object linkedObject = TRV_Entities.SelectedNode.Tag;
			if (linkedObject is CubeGridEntity)
			{
				try
				{
					CubeGridEntity item = (CubeGridEntity)linkedObject;
					BaseEntityManagerWrapper.GetInstance().RemoveEntity(item.BackingObject);

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
			if (linkedObject is CharacterEntity)
			{
				try
				{
					CharacterEntity item = (CharacterEntity)linkedObject;
					BaseEntityManagerWrapper.GetInstance().RemoveEntity(item.BackingObject);

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
			if (linkedObject is VoxelMap)
			{
				try
				{
					VoxelMap item = (VoxelMap)linkedObject;
					BaseEntityManagerWrapper.GetInstance().RemoveEntity(item.BackingObject);

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
			if (linkedObject is FloatingObject)
			{
				try
				{
					FloatingObject item = (FloatingObject)linkedObject;
					BaseEntityManagerWrapper.GetInstance().RemoveEntity(item.BackingObject);

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
			if (linkedObject is Meteor)
			{
				try
				{
					Meteor item = (Meteor)linkedObject;
					BaseEntityManagerWrapper.GetInstance().RemoveEntity(item.BackingObject);

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
				SandboxGameAssemblyWrapper.GetInstance().SendPublicChatMessage(message);
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
					SandboxGameAssemblyWrapper.GetInstance().SendPublicChatMessage(message);
					TXT_Chat_Message.Text = "";
				}
			}
		}

		#endregion
	}
}
