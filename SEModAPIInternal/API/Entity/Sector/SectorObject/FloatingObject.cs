using System;
using System.Collections.Generic;
using System.ComponentModel;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;

using SEModAPI.API.Definitions;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
	public class FloatingObject : BaseEntity
	{
		#region "Constructors and Initializers"

		public FloatingObject(MyObjectBuilder_FloatingObject definition)
			: base(definition)
		{ }

		public FloatingObject(MyObjectBuilder_FloatingObject definition, Object backingObject)
			: base(definition, backingObject)
		{ }

		#endregion

		#region "Properties"

		public override string Name
		{
			get { return GetSubTypeEntity().Item.PhysicalContent.SubtypeName; }
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_FloatingObject GetSubTypeEntity()
		{
			return (MyObjectBuilder_FloatingObject)BaseEntity;
		}

		#endregion
	}
}
