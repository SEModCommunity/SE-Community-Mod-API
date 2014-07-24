using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;
using Sandbox.Definitions;

using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	public class ReactorEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private InventoryEntity m_Inventory;
		private PowerProducer m_powerProducer;

		public static string ReactorNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string ReactorClass = "714451095FE0D12C399607EAC612A6FE";

		public static string ReactorGetInventoryMethod = "GetInventory";

		#endregion

		#region "Constructors and Initializers"

		public ReactorEntity(CubeGridEntity parent, MyObjectBuilder_Reactor definition)
			: base(parent, definition)
		{
			m_Inventory = new InventoryEntity(definition.Inventory);
			m_powerProducer = new PowerProducer(Parent.PowerManager, null);
		}

		public ReactorEntity(CubeGridEntity parent, MyObjectBuilder_Reactor definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
			m_Inventory = new InventoryEntity(definition.Inventory, InternalGetReactorInventory());
			m_powerProducer = new PowerProducer(Parent.PowerManager, ActualObject);
		}

		#endregion

		#region "Properties"

		[Category("Reactor")]
		[Browsable(false)]
		public InventoryEntity Inventory
		{
			get
			{
				return m_Inventory;
			}
		}

		[Category("Reactor")]
		public float Fuel
		{
			get
			{
				float fuelMass = 0;
				foreach (var item in Inventory.Items)
				{
					fuelMass += item.Amount;
				}
				return fuelMass;
			}
		}

		[Category("Reactor")]
		public float MaxPower
		{
			get { return PowerProducer.MaxPowerOutput; }
			set { PowerProducer.MaxPowerOutput = value; }
		}

		[Category("Reactor")]
		public float Power
		{
			get { return PowerProducer.PowerOutput; }
			set { PowerProducer.PowerOutput = value; }
		}

		[Category("Reactor")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal PowerProducer PowerProducer
		{
			get { return m_powerProducer; }
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_Reactor GetSubTypeEntity()
		{
			MyObjectBuilder_Reactor reactor = (MyObjectBuilder_Reactor)ObjectBuilder;

			//Make sure the inventory is up-to-date
			Inventory.RefreshInventory();
			reactor.Inventory = Inventory.ObjectBuilder;

			return reactor;
		}

		#region "Internal"

		protected Object InternalGetReactorInventory()
		{
			try
			{
				Object baseObject = BackingObject;
				Object actualObject = GetActualObject();
				Object inventory = InvokeEntityMethod(actualObject, ReactorGetInventoryMethod, new object[] { 0 });

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
