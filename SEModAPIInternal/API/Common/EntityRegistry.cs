using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Voxels;

using SEModAPIInternal.API.Entity.Sector.SectorObject;

namespace SEModAPIInternal.API.Common
{
	public class EntityRegistry : GameObjectRegistry
	{
		#region "Attributes"

		private static EntityRegistry m_instance;

		#endregion

		#region "Constructors and Initializers"

		protected EntityRegistry()
		{
			Register(typeof(MyObjectBuilder_Character), typeof(CharacterEntity));
			Register(typeof(MyObjectBuilder_CubeGrid), typeof(CubeGridEntity));
			Register(typeof(MyObjectBuilder_FloatingObject), typeof(FloatingObject));
			Register(typeof(MyObjectBuilder_Meteor), typeof(Meteor));
			Register(typeof(MyObjectBuilder_VoxelMap), typeof(VoxelMap));
		}

		#endregion

		#region "Properties"

		new public static EntityRegistry Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new EntityRegistry();

				return m_instance;
			}
		}

		#endregion
	}
}
