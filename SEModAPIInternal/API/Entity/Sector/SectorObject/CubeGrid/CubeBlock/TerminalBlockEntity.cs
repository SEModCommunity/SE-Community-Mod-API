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
	public class TerminalBlockEntity : CubeBlockEntity
	{
		#region "Constructors and Initializers"

		public TerminalBlockEntity(MyObjectBuilder_TerminalBlock definition)
			: base(definition)
		{
			EntityId = definition.EntityId;
		}

		#endregion

		#region "Properties"

		public override string Name
		{
			get
			{
				String name = CustomName;
				if (name == null || name == "")
					name = base.Name;
				return name;
			}
		}

		[Category("Terminal Block")]
		public string CustomName
		{
			get { return GetSubTypeEntity().CustomName; }
			set
			{
				if (GetSubTypeEntity().CustomName == value) return;
				GetSubTypeEntity().CustomName = value;
				Changed = true;

				if (BackingObject != null)
					CubeBlockInternalWrapper.GetInstance().UpdateTerminalBlockCustomName(BackingObject, value);
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_TerminalBlock GetSubTypeEntity()
		{
			return (MyObjectBuilder_TerminalBlock)BaseEntity;
		}

		#endregion
	}
}
