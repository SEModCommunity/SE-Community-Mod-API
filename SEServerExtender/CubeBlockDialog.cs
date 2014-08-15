using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;

using Sandbox.Definitions;

using SEModAPI.API.Definitions;
using SEModAPI.API.Definitions.CubeBlocks;
using SEModAPI.Support;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
using SEModAPIInternal.Support;

using VRageMath;

namespace SEServerExtender
{
	public partial class CubeBlockDialog : Form
	{
		#region "Attributes"

		private CubeGridEntity m_parent;

		#endregion

		#region "Constructors and Initializers"

		public CubeBlockDialog()
		{
			InitializeComponent();

			CMB_BlockType.BeginUpdate();
			foreach (var entry in BlockRegistry.Instance.Registry)
			{
				CMB_BlockType.Items.Add(entry.Value.Name);
			}
			CMB_BlockType.EndUpdate();

			TXT_Position_X.Text = "0";
			TXT_Position_Y.Text = "0";
			TXT_Position_Z.Text = "0";
		}

		#endregion

		#region "Properties"

		public CubeGridEntity ParentCubeGrid
		{
			get { return m_parent; }
			set { m_parent = value; }
		}

		public KeyValuePair<Type, Type> SelectedType
		{
			get
			{
				KeyValuePair<Type, Type> entry = BlockRegistry.Instance.Registry.ElementAt(CMB_BlockType.SelectedIndex);
				return entry;
			}
		}

		public string SelectedSubType
		{
			get { return (string) CMB_BlockSubType.SelectedItem; }
		}

		public Vector3I Position
		{
			get
			{
				try
				{
					int pos_x = int.Parse(TXT_Position_X.Text);
					int pos_y = int.Parse(TXT_Position_Y.Text);
					int pos_z = int.Parse(TXT_Position_Z.Text);

					return new Vector3I(pos_x, pos_y, pos_z);
				}
				catch (Exception ex)
				{
					LogManager.ErrorLog.WriteLine(ex);
					return Vector3I.Zero;
				}
			}
		}

		#endregion

		#region "Methods"

		private void BTN_CubeBlock_Add_Click(object sender, EventArgs e)
		{
			if (CMB_BlockType.SelectedIndex < 0)
				return;

			try
			{
				MyObjectBuilder_CubeBlock objectBuilder = (MyObjectBuilder_CubeBlock) Activator.CreateInstance(SelectedType.Key);
				objectBuilder.SubtypeName = SelectedSubType;
				objectBuilder.Min = Position;

				CubeBlockEntity cubeBlock = (CubeBlockEntity) Activator.CreateInstance(SelectedType.Value, new object[] { Parent, objectBuilder });
				ParentCubeGrid.AddCubeBlock(cubeBlock);

				this.Close();
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		private void CMB_BlockType_SelectedIndexChanged(object sender, EventArgs e)
		{
			CMB_BlockSubType.BeginUpdate();
			CMB_BlockSubType.Items.Clear();
			foreach (MyCubeBlockDefinition cubeBlockDefinition in Enumerable.OfType<MyCubeBlockDefinition>((IEnumerable)MyDefinitionManager.Static.GetAllDefinitions()))
			{
				if (cubeBlockDefinition.Id.TypeId == SelectedType.Key)
				{
					CMB_BlockSubType.Items.Add(cubeBlockDefinition.Id.SubtypeName);					
				}
			}
			CMB_BlockSubType.EndUpdate();
		}

		#endregion
	}
}
