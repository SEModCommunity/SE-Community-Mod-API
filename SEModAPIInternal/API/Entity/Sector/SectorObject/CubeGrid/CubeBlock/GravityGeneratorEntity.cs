using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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

				if (BackingObject != null)
				{
					Action action = InternalUpdateGravityGenerator;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
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

				if (BackingObject != null)
				{
					Action action = InternalUpdateGravityGenerator;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
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

		protected void InternalUpdateGravityGenerator()
		{
			try
			{
				Object baseObject = BackingObject;
				Object actualObject = GetActualObject();

				MethodInfo updateAcceleration = actualObject.GetType().GetMethod(GravityGeneratorSetAccelerationMethod);
				updateAcceleration.Invoke(actualObject, new object[] { GravityAcceleration });

				MethodInfo updateFieldSize = actualObject.GetType().GetMethod(GravityGeneratorSetFieldSizeMethod);
				updateFieldSize.Invoke(actualObject, new object[] { (Vector3)FieldSize });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
