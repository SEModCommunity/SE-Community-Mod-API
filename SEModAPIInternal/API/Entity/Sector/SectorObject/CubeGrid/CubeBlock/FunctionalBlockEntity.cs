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
	public class FunctionalBlockEntity : TerminalBlockEntity
	{
		#region "Constructors and Initializers"

		public FunctionalBlockEntity(MyObjectBuilder_FunctionalBlock definition)
			: base(definition)
		{
			EntityId = definition.EntityId;
		}

		#endregion

		#region "Properties"

		[Category("Functional Block")]
		public bool Enabled
		{
			get { return GetSubTypeEntity().Enabled; }
			set
			{
				if (GetSubTypeEntity().Enabled == value) return;
				GetSubTypeEntity().Enabled = value;
				Changed = true;

				if (BackingObject != null)
					CubeBlockInternalWrapper.GetInstance().UpdateFunctionalBlockEnabled(BackingObject, value);
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_FunctionalBlock GetSubTypeEntity()
		{
			return (MyObjectBuilder_FunctionalBlock)BaseEntity;
		}

		#endregion
	}
}
