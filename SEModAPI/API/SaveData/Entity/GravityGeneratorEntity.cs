using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

namespace SEModAPI.API.SaveData.Entity
{
	public class GravityGeneratorEntity : FunctionalBlockEntity<MyObjectBuilder_GravityGenerator>
	{
		#region "Constructors and Initializers"

		public GravityGeneratorEntity(MyObjectBuilder_GravityGenerator definition)
			: base(definition)
		{
			EntityId = definition.EntityId;
		}

		#endregion

		#region "Properties"

		[Category("Gravity Generator")]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public SerializableVector3 FieldSize
		{
			get { return m_baseDefinition.FieldSize; }
			set
			{
				if (m_baseDefinition.FieldSize.Equals(value)) return;
				m_baseDefinition.FieldSize = value;
				Changed = true;
			}
		}

		[Category("Gravity Generator")]
		public float GravityAcceleration
		{
			get { return m_baseDefinition.GravityAcceleration; }
			set
			{
				if (m_baseDefinition.GravityAcceleration == value) return;
				m_baseDefinition.GravityAcceleration = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_GravityGenerator definition)
		{
			return definition.TypeId.ToString();
		}

		#endregion
	}
}
