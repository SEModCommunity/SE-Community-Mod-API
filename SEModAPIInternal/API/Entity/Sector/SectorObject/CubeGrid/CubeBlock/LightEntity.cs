using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

using VRageMath;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	public class LightEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		public static string LightNamespace = "AAD9061F948E6A3635200145188D64A9";
		public static string LightClass = "8A6EF7294F2DD99246C9BF7574C1E26C";

		public static string LightUpdateColorMethod = "4F2069E3D80C0FFC0E5594A4B0EEC9E8";
		public static string LightUpdateIntensityMethod = "8017479AD649F97C1D60B7A69627D433";
		public static string LightUpdateFalloffMethod = "C3366430336FC45474244C38663E85C3";
		public static string LightUpdateRadiusMethod = "671145C348E272C8E78649055AF2073D";

		#endregion

		#region "Constructors and Initializers"

		public LightEntity(CubeGridEntity parent, MyObjectBuilder_LightingBlock definition)
			: base(parent, definition)
		{
		}

		public LightEntity(CubeGridEntity parent, MyObjectBuilder_LightingBlock definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
		}

		#endregion

		#region "Properties"

		[Category("Light")]
		public float ColorAlpha
		{
			get { return GetSubTypeEntity().ColorAlpha; }
			set
			{
				GetSubTypeEntity().ColorAlpha = value;

				if (BackingObject != null)
				{
					Action action = InternalUpdateLight;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Light")]
		public float ColorRed
		{
			get { return GetSubTypeEntity().ColorRed; }
			set
			{
				GetSubTypeEntity().ColorRed = value;

				if (BackingObject != null)
				{
					Action action = InternalUpdateLight;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Light")]
		public float ColorGreen
		{
			get { return GetSubTypeEntity().ColorGreen; }
			set
			{
				GetSubTypeEntity().ColorGreen = value;

				if (BackingObject != null)
				{
					Action action = InternalUpdateLight;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Light")]
		public float ColorBlue
		{
			get { return GetSubTypeEntity().ColorBlue; }
			set
			{
				GetSubTypeEntity().ColorBlue = value;

				if (BackingObject != null)
				{
					Action action = InternalUpdateLight;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Light")]
		public float Intensity
		{
			get { return GetSubTypeEntity().Intensity; }
			set
			{
				GetSubTypeEntity().Intensity = value;

				if (BackingObject != null)
				{
					Action action = InternalUpdateLight;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Light")]
		public float Falloff
		{
			get { return GetSubTypeEntity().Falloff; }
			set
			{
				GetSubTypeEntity().Falloff = value;

				if (BackingObject != null)
				{
					Action action = InternalUpdateLight;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Light")]
		public float Radius
		{
			get { return GetSubTypeEntity().Radius; }
			set
			{
				GetSubTypeEntity().Radius = value;

				if (BackingObject != null)
				{
					Action action = InternalUpdateLight;
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
		new internal MyObjectBuilder_LightingBlock GetSubTypeEntity()
		{
			return (MyObjectBuilder_LightingBlock)BaseEntity;
		}

		protected void InternalUpdateLight()
		{
			try
			{
				Color color = new Color(ColorRed, ColorGreen, ColorBlue, ColorAlpha);
				InvokeEntityMethod(BackingObject, LightUpdateColorMethod, new object[] { color });
				InvokeEntityMethod(BackingObject, LightUpdateIntensityMethod, new object[] { Intensity });
				InvokeEntityMethod(BackingObject, LightUpdateFalloffMethod, new object[] { Falloff });
				InvokeEntityMethod(BackingObject, LightUpdateRadiusMethod, new object[] { Radius });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
