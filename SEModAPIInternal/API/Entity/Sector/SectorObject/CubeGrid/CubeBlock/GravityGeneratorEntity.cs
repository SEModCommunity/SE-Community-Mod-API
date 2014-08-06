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
	public class GravityGeneratorEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		public static string GravityGeneratorNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string GravityGeneratorClass = "8F510E70FE6A50C0B39D09689C2D6CF4";

		public static string GravityGeneratorSetAccelerationMethod = "79BD2994D8EC029801BD5978D7474622";
		public static string GravityGeneratorSetFieldSizeMethod = "79D354AC704AAF4576B6F44487097505";

		#endregion

		#region "Constructors and Initializers"

		public GravityGeneratorEntity(CubeGridEntity parent, MyObjectBuilder_GravityGenerator definition)
			: base(parent, definition)
		{
		}

		public GravityGeneratorEntity(CubeGridEntity parent, MyObjectBuilder_GravityGenerator definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Gravity Generator")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_GravityGenerator ObjectBuilder
		{
			get
			{
				MyObjectBuilder_GravityGenerator gravity = (MyObjectBuilder_GravityGenerator)base.ObjectBuilder;

				return gravity;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category("Gravity Generator")]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public SerializableVector3 FieldSize
		{
			get { return ObjectBuilder.FieldSize; }
			set
			{
				if (ObjectBuilder.FieldSize.Equals(value)) return;
				ObjectBuilder.FieldSize = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateFieldSize;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[DataMember]
		[Category("Gravity Generator")]
		public float GravityAcceleration
		{
			get { return ObjectBuilder.GravityAcceleration; }
			set
			{
				if (ObjectBuilder.GravityAcceleration == value) return;
				ObjectBuilder.GravityAcceleration = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateGravityAcceleration;
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

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(GravityGeneratorNamespace, GravityGeneratorClass);
				if (type == null)
					throw new Exception("Could not find internal type for GravityGeneratorEntity");
				result &= HasMethod(type, GravityGeneratorSetAccelerationMethod);
				result &= HasMethod(type, GravityGeneratorSetFieldSizeMethod);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		protected void InternalUpdateGravityAcceleration()
		{
			try
			{
				InvokeEntityMethod(ActualObject, GravityGeneratorSetAccelerationMethod, new object[] { GravityAcceleration });
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected void InternalUpdateFieldSize()
		{
			try
			{
				InvokeEntityMethod(ActualObject, GravityGeneratorSetFieldSizeMethod, new object[] { (Vector3)FieldSize });
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
