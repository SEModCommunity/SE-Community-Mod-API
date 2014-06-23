using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.API;
using SEModAPI.API.SaveData;
using SEModAPI.API.SaveData.Entity;

using SEServerExtender.API;

namespace SEServerExtender
{
	public partial class SEServerExtender : Form
	{
		private ProcessWrapper m_processWrapper;
		private Timer m_entityTreeRefreshTimer;

		public SEServerExtender()
		{
			InitializeComponent();

			new GameInstallationInfo();

			m_processWrapper = new ProcessWrapper();

			m_entityTreeRefreshTimer = new Timer();
			m_entityTreeRefreshTimer.Interval = 500;
			m_entityTreeRefreshTimer.Tick += new EventHandler(timer_Tick);

			TRV_Entities.Nodes.Add("Cube Grids (0)");
			TRV_Entities.Nodes.Add("Characters (0)");
		}

		void timer_Tick(object sender, EventArgs e)
		{
			TRV_Entities.BeginUpdate();

			List<CubeGrid> cubeGrids = m_processWrapper.GetGameObjectManager().GetCubeGrids();
			List<CharacterEntity> characters = m_processWrapper.GetGameObjectManager().GetCharacters();

			TreeNode cubeGridsNode = TRV_Entities.Nodes[0];
			TreeNode charactersNode = TRV_Entities.Nodes[1];

			bool gridEntriesChanged = (cubeGridsNode.Nodes.Count != cubeGrids.Count);
			if (gridEntriesChanged)
			{
				cubeGridsNode.Nodes.Clear();
				cubeGridsNode.Text = "Cube Grids (" + cubeGrids.Count.ToString() + ")";
			}
			bool charactersChanged = (charactersNode.Nodes.Count != characters.Count);
			if (charactersChanged)
			{
				charactersNode.Nodes.Clear();
				charactersNode.Text = "Characters (" + characters.Count.ToString() + ")";
			}

			int index = 0;
			foreach (CubeGrid cubeGrid in cubeGrids)
			{
				SerializableVector3 rawVelocity = cubeGrid.LinearVelocity;
				double velocity = Math.Round(Math.Sqrt(rawVelocity.X * rawVelocity.X + rawVelocity.Y * rawVelocity.Y + rawVelocity.Z * rawVelocity.Z), 2);

				TreeNode node = null;
				if (gridEntriesChanged)
				{
					node = cubeGridsNode.Nodes.Add(cubeGrid.Name + " | Velocity: " + velocity.ToString() + " m/s");
					node.Tag = cubeGrid;
				}
				else
				{
					node = cubeGridsNode.Nodes[index];
					node.Text = cubeGrid.Name + " | Velocity: " + velocity.ToString() + " m/s";
					node.Tag = cubeGrid;
				}

				index++;
			}
			index = 0;
			foreach (CharacterEntity character in characters)
			{
				SerializableVector3 rawVelocity = character.LinearVelocity;
				double velocity = Math.Round(Math.Sqrt(rawVelocity.X * rawVelocity.X + rawVelocity.Y * rawVelocity.Y + rawVelocity.Z * rawVelocity.Z), 2);

				TreeNode node = null;
				if (charactersChanged)
				{
					node = charactersNode.Nodes.Add(character.Name + " | Velocity: " + velocity.ToString() + " m/s");
					node.Tag = character;
				}
				else
				{
					node = charactersNode.Nodes[index];
					node.Text = character.Name + " | Velocity: " + velocity.ToString() + " m/s";
					node.Tag = character;
				}

				index++;
			}

			TRV_Entities.EndUpdate();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			m_processWrapper.StartGame(TXT_Control_WorldName.Text);

			m_entityTreeRefreshTimer.Start();
		}

		private void BTN_ServerControl_Stop_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Not yet implemented");
		}

		private void TRV_Entities_AfterSelect(object sender, TreeViewEventArgs e)
		{
			var linkedObject = e.Node.Tag;

			PG_Entities_Details.SelectedObject = linkedObject;
		}
	}
}
