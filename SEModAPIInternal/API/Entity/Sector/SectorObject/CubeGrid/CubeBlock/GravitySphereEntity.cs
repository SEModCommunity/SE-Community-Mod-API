using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.API;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

using VRageMath;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract(Name = "GravityGeneratorEntityProxy")]
	public class GravitySphereEntity : GravityBaseEntity
	{
		#region "Attributes"

		public static string GravitySphereNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string GravitySphereClass = "7A213631DF7D41E1073CC5A0B88C2411";

		public static string GravitySphereSetFieldRadiusMethod = "72427BC45A87B1130BF96C0306A88317";

		#endregion

		#region "Constructors and Initializers"

		public GravitySphereEntity(CubeGridEntity parent, MyObjectBuilder_GravityGeneratorSphere definition)
			: base(parent, definition)
		{
		}

		public GravitySphereEntity(CubeGridEntity parent, MyObjectBuilder_GravityGeneratorSphere definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Gravity Generator Sphere")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_GravityGeneratorSphere ObjectBuilder
		{
			get
			{
				MyObjectBuilder_GravityGeneratorSphere gravity = (MyObjectBuilder_GravityGeneratorSphere)base.ObjectBuilder;

				return gravity;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category("Gravity Generator Sphere")]
		public float FieldRadius
		{
			get { return ObjectBuilder.Radius; }
			set
			{
				if (ObjectBuilder.Radius.Equals(value)) return;
				ObjectBuilder.Radius = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateFieldRadius;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				bool result = true;

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(GravitySphereNamespace, GravitySphereClass);
				if (type == null)
					throw new Exception("Could not find internal type for GravitySphereEntity");
				result &= HasMethod(type, GravitySphereSetFieldRadiusMethod);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		protected void InternalUpdateFieldRadius()
		{
			try
			{
				InvokeEntityMethod(ActualObject, GravitySphereSetFieldRadiusMethod, new object[] { FieldRadius });
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
