using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract(Name = "ConveyorTubeEntityProxy")]
	public class ConveyorTubeEntity : CubeBlockEntity
	{
		#region "Attributes"

		public static string ConveyorTubeNamespace = "";
		public static string ConveyorTubeClass = "";

		#endregion

		#region "Constructors and Intializers"

		public ConveyorTubeEntity(CubeGridEntity parent, MyObjectBuilder_ConveyorConnector definition)
			: base(parent, definition)
		{
		}

		public ConveyorTubeEntity(CubeGridEntity parent, MyObjectBuilder_ConveyorConnector definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
		}

		#endregion

		#region "Properties"
		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_ConveyorConnector GetSubTypeEntity()
		{
			return (MyObjectBuilder_ConveyorConnector)ObjectBuilder;
		}

		#endregion
	}
}
