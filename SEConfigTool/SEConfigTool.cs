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
                ConfigFileSerializer Serializer = new ConfigFileSerializer();
                //SaveFile save = new SaveFile(@"E:\PROGRAMATION\Formulaires c#\SANDBOX_0_0_0_.sbs", Serializer);

                //foreach (MyObjectBuilder_EntityBase currentObject in save.Objects)
                //{
                //    float x = currentObject.PositionAndOrientation.Value.Position.x;
                //    float y = currentObject.PositionAndOrientation.Value.Position.y;
                //    float z = currentObject.PositionAndOrientation.Value.Position.z;

                //    float dist = (float)Math.Sqrt(x * x + y * y + z * z);

                //    BlockList.Items.Add(currentObject.TypeId.ToString() + " | " + "Dist: " + dist.ToString("F2") + "m | " + x + ";" + z +";"+y);
                //}
            }
            catch (AutoException AEx)
            {
                string ExceptionString = AEx.GetDebugString();
            }
        }
    }
}
