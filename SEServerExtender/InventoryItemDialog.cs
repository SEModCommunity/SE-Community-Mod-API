using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;

using SEModAPI.Support;
using SEModAPI.API.Definitions;
using SEModAPI.API.Definitions.CubeBlocks;

using SEModAPIInternal.API.Entity;
using SEModAPIInternal.Support;

namespace SEServerExtender
{
	public partial class InventoryItemDialog : Form
	{
		#region "Attributes"

		private static PhysicalItemDefinitionsManager m_physicalItemsManager;
		private static ComponentDefinitionsManager m_componentsManager;
		private static AmmoMagazinesDefinitionsManager m_ammoManager;

		private static List<SerializableDefinitionId> m_idList;

		private InventoryEntity m_container;

		#endregion

		#region "Constructors and Initializers"

		public InventoryItemDialog()
		{
			//Load up the static item managers
			if (m_physicalItemsManager == null)
			{
				m_physicalItemsManager = new PhysicalItemDefinitionsManager();
				m_physicalItemsManager.Load(PhysicalItemDefinitionsManager.GetContentDataFile("PhysicalItems.sbc"));
			}
			if (m_componentsManager == null)
			{
				m_componentsManager = new ComponentDefinitionsManager();
				m_componentsManager.Load(ComponentDefinitionsManager.GetContentDataFile("Components.sbc"));
			}
			if (m_ammoManager == null)
			{
				m_ammoManager = new AmmoMagazinesDefinitionsManager();
				m_ammoManager.Load(AmmoMagazinesDefinitionsManager.GetContentDataFile("AmmoMagazines.sbc"));
			}

			//Populate the static list with the ids from the items
			if (m_idList == null)
			{
				m_idList = new List<SerializableDefinitionId>();
				foreach (var def in m_physicalItemsManager.Definitions)
				{
					m_idList.Add(def.Id);
				}
				foreach (var def in m_componentsManager.Definitions)
				{
					m_idList.Add(def.Id);
				}
				foreach (var def in m_ammoManager.Definitions)
				{
					m_idList.Add(def.Id);
				}
			}

			InitializeComponent();

			CMB_ItemType.BeginUpdate();
			foreach (var entry in m_idList)
			{
				CMB_ItemType.Items.Add(entry);
			}
			CMB_ItemType.EndUpdate();

			TXT_ItemAmount.Text = "0.0";
		}

		#endregion

		#region "Properties"

		public InventoryEntity Container
		{
			get { return m_container; }
			set { m_container = value; }
		}

		public SerializableDefinitionId SelectedType
		{
			get { return (SerializableDefinitionId)CMB_ItemType.SelectedItem; }
		}

		public float Amount
		{
			get
			{
				try
				{
					float amount = float.Parse(TXT_ItemAmount.Text);

					return amount;
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
					return 0;
				}
			}
		}

		#endregion

		#region "Methods"

		private void BTN_InventoryItem_Add_Click(object sender, EventArgs e)
		{
			if (Amount <= 0.0f)
				return;

			try
			{
				MyObjectBuilder_InventoryItem objectBuilder = MyObjectBuilder_Base.CreateNewObject<MyObjectBuilder_InventoryItem>();
				objectBuilder.Content = MyObjectBuilder_Base.CreateNewObject(SelectedType.TypeId, SelectedType.SubtypeId);
				objectBuilder.Amount = Amount;
				InventoryItemEntity newItem = new InventoryItemEntity(objectBuilder);

				Container.NewEntry(newItem);

				this.Close();
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
