using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	public class ReactorEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private InventoryEntity m_Inventory;

		#endregion

		#region "Constructors and Initializers"

		public ReactorEntity(MyObjectBuilder_Reactor definition)
			: base(definition)
		{
			m_Inventory = new InventoryEntity(definition.Inventory);
		}

		public ReactorEntity(MyObjectBuilder_Reactor definition, Object backingObject)
			: base(definition, backingObject)
		{
			m_Inventory = new InventoryEntity(definition.Inventory);	//TODO - Add a method to get the reactors backing inventory object
		}

		#endregion

		#region "Properties"

		[Category("Reactor")]
		[Browsable(false)]
		public InventoryEntity Inventory
		{
			get { return m_Inventory; }
		}

		[Category("Reactor")]
		public float Fuel
		{
			get
			{
				float fuelMass = 0;
				foreach (var item in m_Inventory.Items)
				{
					fuelMass += item.Mass;
				}
				return fuelMass;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_Reactor GetSubTypeEntity()
		{
			return (MyObjectBuilder_Reactor)BaseEntity;
		}

		#endregion
	}
}
