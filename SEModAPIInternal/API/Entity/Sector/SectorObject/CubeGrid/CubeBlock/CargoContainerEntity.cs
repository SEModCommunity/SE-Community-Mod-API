using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	public class CargoContainerEntity : TerminalBlockEntity
	{
		#region "Attributes"

		private InventoryEntity m_Inventory;

		public static string CargoContainerNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string CargoContainerClass = "0B52AF23069247D1A6D57F957ED070E3";

		public static string CargoContainerGetInventoryMethod = "GetInventory";

		#endregion

		#region "Constructors and Initializers"

		public CargoContainerEntity(CubeGridEntity parent, MyObjectBuilder_CargoContainer definition)
			: base(parent, definition)
		{
			m_Inventory = new InventoryEntity(definition.Inventory);
		}

		public CargoContainerEntity(CubeGridEntity parent, MyObjectBuilder_CargoContainer definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
			m_Inventory = new InventoryEntity(definition.Inventory, InternalGetContainerInventory());
		}

		#endregion

		#region "Properties"

		[Category("Cargo Container")]
		[Browsable(false)]
		public InventoryEntity Inventory
		{
			get
			{
				return m_Inventory;
			}
		}

		[Category("Cargo Container")]
		public float ItemCount
		{
			get
			{
				float count = 0;
				foreach (var item in Inventory.Items)
				{
					count += item.Amount;
				}
				return count;
			}
		}

		[Category("Cargo Container")]
		public float ItemMass
		{
			get
			{
				float mass = 0;
				foreach (var item in Inventory.Items)
				{
					mass += item.Mass;
				}
				return mass;
			}
		}

		[Category("Cargo Container")]
		public float ItemVolume
		{
			get
			{
				float volume = 0;
				foreach (var item in Inventory.Items)
				{
					volume += item.Volume;
				}
				return volume;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_CargoContainer GetSubTypeEntity()
		{
			MyObjectBuilder_CargoContainer container = (MyObjectBuilder_CargoContainer)BaseEntity;

			//Make sure the inventory is up-to-date
			Inventory.RefreshInventory();
			container.Inventory = Inventory.GetSubTypeEntity();

			return container;
		}

		#region "Internal"

		protected Object InternalGetContainerInventory()
		{
			try
			{
				Object baseObject = BackingObject;
				Object actualObject = GetActualObject();
				Object inventory = InvokeEntityMethod(actualObject, CargoContainerGetInventoryMethod, new object[] { 0 });

				return inventory;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		#endregion

		#endregion
	}
}
