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

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

using VRageMath;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract(Name = "LightEntityProxy")]
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

		////////////////////////////////////////////////////////////////////////

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

		[IgnoreDataMember]
		[Category("Light")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_LightingBlock ObjectBuilder
		{
			get
			{
				return (MyObjectBuilder_LightingBlock)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category("Light")]
		[Browsable(false)]
		public Color Color
		{
			get
			{
				var baseEntity = ObjectBuilder;
				Color color = new Color(baseEntity.ColorRed, baseEntity.ColorGreen, baseEntity.ColorBlue, baseEntity.ColorAlpha);

				return color;
			}
			set
			{
				if (Color == value) return;
				var baseEntity = ObjectBuilder;
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

		[IgnoreDataMember]
		[Category("Light")]
		public float ColorAlpha
		{
			get { return ObjectBuilder.ColorAlpha; }
			set
			{
				if (ObjectBuilder.ColorAlpha == value) return;
				ObjectBuilder.ColorAlpha = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateLight;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[IgnoreDataMember]
		[Category("Light")]
		public float ColorRed
		{
			get { return ObjectBuilder.ColorRed; }
			set
			{
				if (ObjectBuilder.ColorRed == value) return;
				ObjectBuilder.ColorRed = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateLight;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[IgnoreDataMember]
		[Category("Light")]
		public float ColorGreen
		{
			get { return ObjectBuilder.ColorGreen; }
			set
			{
				if (ObjectBuilder.ColorGreen == value) return;
				ObjectBuilder.ColorGreen = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateLight;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[IgnoreDataMember]
		[Category("Light")]
		public float ColorBlue
		{
			get { return ObjectBuilder.ColorBlue; }
			set
			{
				if (ObjectBuilder.ColorBlue == value) return;
				ObjectBuilder.ColorBlue = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateLight;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[DataMember]
		[Category("Light")]
		public float Intensity
		{
			get { return ObjectBuilder.Intensity; }
			set
			{
				if (ObjectBuilder.Intensity == value) return;
				ObjectBuilder.Intensity = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateLight;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[DataMember]
		[Category("Light")]
		public float Falloff
		{
			get { return ObjectBuilder.Falloff; }
			set
			{
				if (ObjectBuilder.Falloff == value) return;
				ObjectBuilder.Falloff = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateLight;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[DataMember]
		[Category("Light")]
		public float Radius
		{
			get { return ObjectBuilder.Radius; }
			set
			{
				if (ObjectBuilder.Radius == value) return;
				ObjectBuilder.Radius = value;
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

		new public static bool ReflectionUnitTest()
		{
			try
			{
				bool result = true;

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(LightNamespace, LightClass);
				if (type == null)
					throw new Exception("Could not find internal type for LightEntity");
				result &= HasMethod(type, LightUpdateColorMethod);
				result &= HasMethod(type, LightUpdateIntensityMethod);
				result &= HasMethod(type, LightUpdateFalloffMethod);
				result &= HasMethod(type, LightUpdateRadiusMethod);
				result &= HasField(type, LightNetworkManagerField);

				Type type2 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(LightNetworkManagerNamespace, LightNetworkManagerClass);
				if (type2 == null)
					throw new Exception("Could not find network manager type for LightEntity");
				result &= HasMethod(type2, LightNetworkManagerSendUpdateMethod);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
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
				LogManager.ErrorLog.WriteLine(ex);
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
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
