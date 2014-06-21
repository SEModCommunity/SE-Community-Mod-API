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
			m_processWrapper.SetEntityTree(TRV_Entities);

			m_entityTreeRefreshTimer = new Timer();
			m_entityTreeRefreshTimer.Interval = 500;
			m_entityTreeRefreshTimer.Tick += new EventHandler(timer_Tick);
		}

		void timer_Tick(object sender, EventArgs e)
		{
			int nodeCount = TRV_Entities.Nodes.Count;

			List<MyObjectBuilder_EntityBase> entities = m_processWrapper.GetEntityList();

			TRV_Entities.BeginUpdate();
			if (nodeCount != entities.Count)
			{
				TRV_Entities.Nodes.Clear();

				foreach (var entity in entities)
				{
					if (entity.GetType() == typeof(MyObjectBuilder_CubeGrid))
					{
						MyObjectBuilder_CubeGrid cubeGridBase = (MyObjectBuilder_CubeGrid)entity;
						CubeGrid cubeGrid = new CubeGrid(cubeGridBase);

						SerializableVector3 rawVelocity = cubeGrid.LinearVelocity;
						double velocity = Math.Round(Math.Sqrt(rawVelocity.X * rawVelocity.X + rawVelocity.Y * rawVelocity.Y + rawVelocity.Z * rawVelocity.Z), 2);

						TreeNode newNode = TRV_Entities.Nodes.Add(cubeGrid.Name + " | Velocity: " + velocity.ToString() + " m/s");
						newNode.Tag = cubeGrid;
					}
					else if (entity.GetType() == typeof(MyObjectBuilder_Character))
					{
						MyObjectBuilder_Character characterBase = (MyObjectBuilder_Character)entity;
						CharacterEntity character = new CharacterEntity(characterBase);

						SerializableVector3 rawVelocity = character.LinearVelocity;
						double velocity = Math.Round(Math.Sqrt(rawVelocity.X * rawVelocity.X + rawVelocity.Y * rawVelocity.Y + rawVelocity.Z * rawVelocity.Z), 2);

						TreeNode newNode = TRV_Entities.Nodes.Add(character.Name + " | Velocity: " + velocity.ToString() + " m/s");
						newNode.Tag = character;
					}
					else
					{
						TreeNode newNode = TRV_Entities.Nodes.Add(entity.TypeId.ToString() + "/" + entity.SubtypeName);
						newNode.Tag = entity;
					}
				}
			}
			else
			{
				int index = 0;
				foreach (var entity in entities)
				{
					TreeNode node = TRV_Entities.Nodes[index];
					if (entity.GetType() == typeof(MyObjectBuilder_CubeGrid))
					{
						MyObjectBuilder_CubeGrid cubeGridBase = (MyObjectBuilder_CubeGrid)entity;
						CubeGrid cubeGrid = new CubeGrid(cubeGridBase);

						SerializableVector3 rawVelocity = cubeGrid.LinearVelocity;
						double velocity = Math.Round(Math.Sqrt(rawVelocity.X * rawVelocity.X + rawVelocity.Y * rawVelocity.Y + rawVelocity.Z * rawVelocity.Z), 2);

						node.Text = cubeGrid.Name + " | Velocity: " + velocity.ToString() + " m/s";
						node.Tag = cubeGrid;
					}
					else if (entity.GetType() == typeof(MyObjectBuilder_Character))
					{
						MyObjectBuilder_Character characterBase = (MyObjectBuilder_Character)entity;
						CharacterEntity character = new CharacterEntity(characterBase);

						SerializableVector3 rawVelocity = character.LinearVelocity;
						double velocity = Math.Round(Math.Sqrt(rawVelocity.X * rawVelocity.X + rawVelocity.Y * rawVelocity.Y + rawVelocity.Z * rawVelocity.Z), 2);

						node.Text = character.Name + " | Velocity: " + velocity.ToString() + " m/s";
						node.Tag = character;
					}
					else
					{
						node.Tag = entity;
					}
					index++;
				}

			}
			TRV_Entities.EndUpdate();

			TreeNode selectedNode = TRV_Entities.SelectedNode;
			if (selectedNode == null)
				return;
			var linkedObject = selectedNode.Tag;
			if (linkedObject == null)
				return;
			PG_Entities_Details.SelectedObject = linkedObject;
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
			if (linkedObject == null)
				return;

			PG_Entities_Details.SelectedObject = linkedObject;
		}
	}
}
