using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Definitions;

using SEModAPIInternal.API.Entity;
using SEModAPIInternal.Support;

using VRage;

namespace SEServerExtender
{
	public partial class InventoryItemDialog : Form
	{
		#region "Attributes"

		private static List<MyDefinitionId> m_idList;

		private InventoryEntity m_container;

		#endregion

		#region "Constructors and Initializers"

		public InventoryItemDialog()
		{
			//Populate the static list with the ids from the items
			if (m_idList == null)
			{
				m_idList = new List<MyDefinitionId>();

				foreach (MyPhysicalItemDefinition def in Enumerable.OfType<MyPhysicalItemDefinition>((IEnumerable)MyDefinitionManager.Static.GetAllDefinitions()))
				{
					m_idList.Add(def.Id);
				}
				foreach (MyComponentDefinition def in Enumerable.OfType<MyComponentDefinition>((IEnumerable)MyDefinitionManager.Static.GetAllDefinitions()))
				{
					m_idList.Add(def.Id);
				}
				foreach (MyAmmoMagazineDefinition def in Enumerable.OfType<MyAmmoMagazineDefinition>((IEnumerable)MyDefinitionManager.Static.GetAllDefinitions()))
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

		public InventoryEntity InventoryContainer
		{
			get { return m_container; }
			set { m_container = value; }
		}

		public MyDefinitionId SelectedType
		{
			get { return (MyDefinitionId)CMB_ItemType.SelectedItem; }
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
					LogManager.ErrorLog.WriteLine(ex);
					return 0;
				}
			}
		}

		#endregion

		#region "Methods"

		private void BTN_InventoryItem_Add_Click(object sender, EventArgs e)
		{
			if (CMB_ItemType.SelectedItem == null)
				return;
			if (Amount <= 0.0f)
				return;

			try
			{
				MyObjectBuilder_InventoryItem objectBuilder = MyObjectBuilder_Base.CreateNewObject<MyObjectBuilder_InventoryItem>();
				objectBuilder.Content = MyObjectBuilder_Base.CreateNewObject(SelectedType.TypeId, SelectedType.SubtypeId.ToString());
				objectBuilder.Amount = (MyFixedPoint)Amount;
				InventoryItemEntity newItem = new InventoryItemEntity(objectBuilder);

				InventoryContainer.NewEntry(newItem);

				this.Close();
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
