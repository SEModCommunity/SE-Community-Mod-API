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
		public static string LightNetworkManagerField = "EB495BC5B3C2335D2B8AD10C334D0928";

		public static string LightNetworkManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string LightNetworkManagerClass = "0F8EE1AD651CB822CB9635B463AE6CD5";
		public static string LightNetworkManagerSendUpdateMethod = "582447224E2B03FA4EAB3D6C2DDD48D9";	//Color, Radius, Falloff, Intensity

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
		[Browsable(false)]
		public Color Color
		{
			get
			{
				var baseEntity = GetSubTypeEntity();
				Color color = new Color(baseEntity.ColorRed, baseEntity.ColorGreen, baseEntity.ColorBlue, baseEntity.ColorAlpha);

				return color;
			}
			set
			{
				if (Color == value) return;
				var baseEntity = GetSubTypeEntity();
				baseEntity.ColorAlpha = value.A;
				baseEntity.ColorRed = value.R;
				baseEntity.ColorGreen = value.G;
				baseEntity.ColorBlue = value.B;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateLight;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Light")]
		public float ColorAlpha
		{
			get { return GetSubTypeEntity().ColorAlpha; }
			set
			{
				if (GetSubTypeEntity().ColorAlpha == value) return;
				GetSubTypeEntity().ColorAlpha = value;
				Changed = true;

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
				if (GetSubTypeEntity().ColorRed == value) return;
				GetSubTypeEntity().ColorRed = value;
				Changed = true;

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
				if (GetSubTypeEntity().ColorGreen == value) return;
				GetSubTypeEntity().ColorGreen = value;
				Changed = true;

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
				if (GetSubTypeEntity().ColorBlue == value) return;
				GetSubTypeEntity().ColorBlue = value;
				Changed = true;

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
				if (GetSubTypeEntity().Intensity == value) return;
				GetSubTypeEntity().Intensity = value;
				Changed = true;

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
				if (GetSubTypeEntity().Falloff == value) return;
				GetSubTypeEntity().Falloff = value;
				Changed = true;

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
				if (GetSubTypeEntity().Radius == value) return;
				GetSubTypeEntity().Radius = value;
				Changed = true;

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

		protected Object GetLightNetworkManager()
		{
			try
			{
				Object actualObject = GetActualObject();

				FieldInfo field = GetEntityField(actualObject, LightNetworkManagerField);
				Object networkManager = field.GetValue(actualObject);
				return networkManager;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		protected void InternalUpdateLight()
		{
			try
			{
				Object actualObject = GetActualObject();

				Color color = new Color(ColorRed / 255.0f, ColorGreen / 255.0f, ColorBlue / 255.0f, ColorAlpha / 255.0f);
				InvokeEntityMethod(actualObject, LightUpdateColorMethod, new object[] { color });
				InvokeEntityMethod(actualObject, LightUpdateIntensityMethod, new object[] { Intensity });
				InvokeEntityMethod(actualObject, LightUpdateFalloffMethod, new object[] { Falloff });
				InvokeEntityMethod(actualObject, LightUpdateRadiusMethod, new object[] { Radius });

				Object netManager = GetLightNetworkManager();
				InvokeEntityMethod(netManager, LightNetworkManagerSendUpdateMethod, new object[] { color, Radius, Falloff, Intensity });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
