using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	public class ProductionBlockEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private InventoryEntity m_inputInventory;
		private InventoryEntity m_outputInventory;

		public static string ProductionBlockNamespace = "6DDCED906C852CFDABA0B56B84D0BD74";
		public static string ProductionBlockClass = "F00C92FFA6F55CD283D86570FFC332AE";

		public static string ProductionBlockGetInputInventoryMethod = "GetInventory";
		public static string ProductionBlockGetOutputInventoryMethod = "GetInventory";

		#endregion

		#region "Constructors and Intializers"

		public ProductionBlockEntity(CubeGridEntity parent, MyObjectBuilder_ProductionBlock definition)
			: base(parent, definition)
		{
			m_inputInventory = new InventoryEntity(definition.InputInventory);
			m_outputInventory = new InventoryEntity(definition.OutputInventory);
		}

		public ProductionBlockEntity(CubeGridEntity parent, MyObjectBuilder_ProductionBlock definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
			m_inputInventory = new InventoryEntity(definition.InputInventory, InternalGetInputInventory());
			m_outputInventory = new InventoryEntity(definition.OutputInventory, InternalGetOutputInventory());
		}

		#endregion

		#region "Properties"

		[Category("Production Block")]
		[Browsable(false)]
		public InventoryEntity InputInventory
		{
			get
			{
				return m_inputInventory;
			}
		}

		[Category("Production Block")]
		[Browsable(false)]
		public InventoryEntity OutputInventory
		{
			get
			{
				return m_outputInventory;
			}
		}

		#endregion

		#region "Methods"

		#region "Internal"

		protected Object InternalGetInputInventory()
		{
			try
			{
				Object baseObject = BackingObject;
				Object actualObject = GetActualObject();
				Object inventory = InvokeEntityMethod(actualObject, ProductionBlockGetInputInventoryMethod, new object[] { 0 });

				return inventory;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		protected Object InternalGetOutputInventory()
		{
			try
			{
				Object baseObject = BackingObject;
				Object actualObject = GetActualObject();
				Object inventory = InvokeEntityMethod(actualObject, ProductionBlockGetOutputInventoryMethod, new object[] { 1 });

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
