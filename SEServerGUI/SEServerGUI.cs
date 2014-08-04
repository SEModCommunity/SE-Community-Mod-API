using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SEServerGUI.ServiceReference;

namespace SEServerGUI
{
	public partial class SEServerGUI : Form
	{
		private InternalServiceContractClient client;

		public SEServerGUI()
		{
			InitializeComponent();

			client = new InternalServiceContractClient();
		}

		private void BTN_Connect_Click(object sender, EventArgs e)
		{
			LST_Entities.Items.Clear();
			foreach (var entry in client.GetSectorEntities())
			{
				LST_Entities.Items.Add(entry);
			}
		}

		private void LST_Entities_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (LST_Entities.SelectedItem == null)
				return;

			PG_Entity_Properties.SelectedObject = LST_Entities.SelectedItem;
		}
	}
}
