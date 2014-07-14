using System;
using System.Collections.Generic;
using System.ComponentModel;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;

using SEModAPI.API.Definitions;

using SEModAPIInternal.API.Common;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
	public class FloatingObject : BaseEntity
	{
		#region "Attributes"

		private static Type m_internalType;

		public static string FloatingObjectNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string FloatingObjectClass = "60663B6C2E735862064C925471BD4138";

		#endregion

		#region "Constructors and Initializers"

		public FloatingObject(MyObjectBuilder_FloatingObject definition)
			: base(definition)
		{ }

		public FloatingObject(MyObjectBuilder_FloatingObject definition, Object backingObject)
			: base(definition, backingObject)
		{ }

		#endregion

		#region "Properties"

		[Browsable(false)]
		[ReadOnly(true)]
		internal static Type InternalType
		{
			get
			{
				if (m_internalType == null)
					m_internalType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(FloatingObjectNamespace, FloatingObjectClass);
				return m_internalType;
			}
		}

		[Category("Floating Object")]
		[Browsable(true)]
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
