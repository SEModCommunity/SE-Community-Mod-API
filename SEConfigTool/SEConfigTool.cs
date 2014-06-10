using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SEModAPI;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEConfigTool
{
    public partial class SEConfigTool : Form
    {
        public SEConfigTool()
        {
            InitializeComponent();
        }

        private void SEConfigTool_Load(object sender, EventArgs e)
        {
            try
            {
				ConfigFileSerializer config = new ConfigFileSerializer();

				// Initialize the block list
				this.BlockList.BeginUpdate();
				foreach (MyObjectBuilder_CubeBlockDefinition def in config.CubeBlockDefinitions)
				{
					this.BlockList.Items.Add(def.BlockPairName);
				}
				this.BlockList.EndUpdate();

				// Initialize the spawn group list
				this.SpawnGroupList.BeginUpdate();
				foreach (MyObjectBuilder_SpawnGroupDefinition def in config.SpawnGroupDefinitions)
				{
					this.SpawnGroupList.Items.Add("Group");
					foreach (Sandbox.Common.ObjectBuilders.Definitions.MyObjectBuilder_SpawnGroupDefinition.SpawnGroupPrefab prefab in def.Prefabs)
					{
						this.SpawnGroupList.Items.Add("\tFile: " + prefab.File + "\tName: " + prefab.BeaconText + "\tPosition: " + prefab.Position.ToString() + "\tSpeed: " + prefab.Speed.ToString());
					}
				}
				this.SpawnGroupList.EndUpdate();

				// Initialize the global events list
				this.GlobalEventList.BeginUpdate();
				foreach (MyObjectBuilder_GlobalEventDefinition def in config.GlobalEventDefinitions)
				{
					this.GlobalEventList.Items.Add(def.EventType.ToString());
				}
				this.GlobalEventList.EndUpdate();

			}
            catch (AutoException AEx)
            {
                string ExceptionString = AEx.GetDebugString();
            }
        }
    }
}
