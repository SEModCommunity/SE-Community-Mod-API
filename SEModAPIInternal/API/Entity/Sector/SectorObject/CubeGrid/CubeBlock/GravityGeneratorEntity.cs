using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.API;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	public class GravityGeneratorEntity : FunctionalBlockEntity
	{
		#region "Constructors and Initializers"

		public GravityGeneratorEntity(MyObjectBuilder_GravityGenerator definition)
			: base(definition)
		{
		}

		public GravityGeneratorEntity(MyObjectBuilder_GravityGenerator definition, Object backingObject)
			: base(definition, backingObject)
		{
		}

		#endregion

		#region "Properties"

		[Category("Gravity Generator")]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public SerializableVector3 FieldSize
		{
			get { return GetSubTypeEntity().FieldSize; }
			set
			{
				if (GetSubTypeEntity().FieldSize.Equals(value)) return;
				GetSubTypeEntity().FieldSize = value;
				Changed = true;
			}
		}

		[Category("Gravity Generator")]
		public float GravityAcceleration
		{
			get { return GetSubTypeEntity().GravityAcceleration; }
			set
			{
				if (GetSubTypeEntity().GravityAcceleration == value) return;
				GetSubTypeEntity().GravityAcceleration = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_GravityGenerator GetSubTypeEntity()
		{
			return (MyObjectBuilder_GravityGenerator)BaseEntity;
		}

		#endregion
	}
}
