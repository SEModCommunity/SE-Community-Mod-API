using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Voxels;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.API;
using SEModAPI.API.Internal;
using SEModAPI.API.SaveData;
using SEModAPI.API.SaveData.Entity;

using SEServerExtender.API;

namespace SEServerExtender
{
	public partial class SEServerExtender : Form
	{
		#region "Attributes"

		private ProcessWrapper m_processWrapper;
		private Timer m_entityTreeRefreshTimer;
		private Timer m_propertyGridRefreshTimer;

		#endregion

		#region "Constructors and Initializers"

		public SEServerExtender()
		{
			InitializeComponent();

			new GameInstallationInfo();

			m_processWrapper = new ProcessWrapper();

			m_entityTreeRefreshTimer = new Timer();
			m_entityTreeRefreshTimer.Interval = 500;
			m_entityTreeRefreshTimer.Tick += new EventHandler(TreeViewRefresh);

			m_propertyGridRefreshTimer = new Timer();
			m_propertyGridRefreshTimer.Interval = 5000;
			m_propertyGridRefreshTimer.Tick += new EventHandler(PropertyGridRefresh);

			TRV_Entities.Nodes.Add("Cube Grids (0)");
			TRV_Entities.Nodes.Add("Characters (0)");
			TRV_Entities.Nodes.Add("Voxel Maps (0)");
			TRV_Entities.Nodes.Add("Floating Objects (0)");
			TRV_Entities.Nodes.Add("Meteors (0)");
		}

		#endregion

		#region "Methods"

		void UpdateNodeCubeBlockBranch<T, TO>(TreeNode node, List<T> source, string name)
			where TO : MyObjectBuilder_CubeBlock
			where T : CubeBlockEntity<TO>
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
				//TODO - Do something about the exception
			}
		}

		void UpdateNodeBranch<T, TO>(TreeNode node, List<T> source, string name)
			where TO : MyObjectBuilder_EntityBase
			where T : SectorObject<TO>
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
				//TODO - Do something about the exception
			}
		}

		void PropertyGridRefresh(object sender, EventArgs e)
		{
			TreeNode selectedNode = TRV_Entities.SelectedNode;
			if (selectedNode == null)
			{
				PG_Entities_Details.SelectedObject = null;
				return;
			}

			PG_Entities_Details.SelectedObject = selectedNode.Tag;
		}

		void TreeViewRefresh(object sender, EventArgs e)
		{
			TRV_Entities.BeginUpdate();

			List<CubeGrid> cubeGrids = GameObjectManagerWrapper.GetInstance().GetCubeGrids();
			List<CharacterEntity> characters = GameObjectManagerWrapper.GetInstance().GetCharacters();
			List<VoxelMap> voxelMaps = GameObjectManagerWrapper.GetInstance().GetVoxelMaps();
			List<FloatingObject> floatingObjects = GameObjectManagerWrapper.GetInstance().GetFloatingObjects();
			List<Meteor> meteors = GameObjectManagerWrapper.GetInstance().GetMeteors();

			UpdateNodeBranch<CubeGrid, MyObjectBuilder_CubeGrid>(TRV_Entities.Nodes[0], cubeGrids, "Cube Grids");
			UpdateNodeBranch<CharacterEntity, MyObjectBuilder_Character>(TRV_Entities.Nodes[1], characters, "Characters");
			UpdateNodeBranch<VoxelMap, MyObjectBuilder_VoxelMap>(TRV_Entities.Nodes[2], voxelMaps, "Voxel Maps");
			UpdateNodeBranch<FloatingObject, MyObjectBuilder_FloatingObject>(TRV_Entities.Nodes[3], floatingObjects, "Floating Objects");
			UpdateNodeBranch<Meteor, MyObjectBuilder_Meteor>(TRV_Entities.Nodes[4], meteors, "Meteors");

			TRV_Entities.EndUpdate();
		}

		private void BTN_ServerControl_Start_Click(object sender, EventArgs e)
		{
			m_processWrapper.StartGame(TXT_Control_WorldName.Text);

			m_entityTreeRefreshTimer.Start();
			m_propertyGridRefreshTimer.Start();
		}

		private void BTN_ServerControl_Stop_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Not yet implemented");
		}

		private void TRV_Entities_AfterSelect(object sender, TreeViewEventArgs e)
		{
			var linkedObject = e.Node.Tag;

			PG_Entities_Details.SelectedObject = linkedObject;

			if (linkedObject is CubeGrid)
			{
				if (e.Node.Nodes.Count < 2)
				{
					e.Node.Nodes.Add("Structural Blocks (0)");
					e.Node.Nodes.Add("Container Blocks (0)");
				}

				CubeGrid cubeGrid = (CubeGrid)linkedObject;

				List<CubeBlockEntity<MyObjectBuilder_CubeBlock>> structuralBlocks = CubeBlockInternalWrapper.GetInstance().GetStructuralBlocks(cubeGrid);
				List<CargoContainerEntity> containerBlocks = CubeBlockInternalWrapper.GetInstance().GetCargoContainerBlocks(cubeGrid);

				TRV_Entities.BeginUpdate();

				UpdateNodeCubeBlockBranch<CubeBlockEntity<MyObjectBuilder_CubeBlock>, MyObjectBuilder_CubeBlock>(e.Node.Nodes[0], structuralBlocks, "Structural Blocks");
				UpdateNodeCubeBlockBranch<CargoContainerEntity, MyObjectBuilder_CargoContainer>(e.Node.Nodes[1], containerBlocks, "Container Blocks");

				TRV_Entities.EndUpdate();
			}
		}

		#endregion
	}
}
