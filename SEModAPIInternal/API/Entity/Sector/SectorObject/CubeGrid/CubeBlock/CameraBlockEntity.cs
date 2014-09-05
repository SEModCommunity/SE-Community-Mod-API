using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract(Name = "CameraBlockEntityProxy")]
	public class CameraBlockEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private bool m_isActive;

		public static string CameraBlockNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string CameraBlockClass = "36C2B65B8C04A8F75A1DF888DE7B41F0";

		public static string CameraBlockGetIsActiveMethod = "B9A8CAA8AE60E632F1A995F9492C5626";
		public static string CameraBlockSetIsActiveMethod = "F1C9E65ABDAB28453725C43D05F95D4E";

		#endregion

		#region "Constructors and Initializers"

		public CameraBlockEntity(CubeGridEntity parent, MyObjectBuilder_CameraBlock definition)
			: base(parent, definition)
		{
			m_isActive = definition.IsActive;
		}

		public CameraBlockEntity(CubeGridEntity parent, MyObjectBuilder_CameraBlock definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
			m_isActive = definition.IsActive;
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Camera")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_CameraBlock ObjectBuilder
		{
			get
			{
				return (MyObjectBuilder_CameraBlock)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[IgnoreDataMember]
		[Category("Camera")]
		[Browsable(false)]
		[ReadOnly(true)]
		new public static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(CameraBlockNamespace, CameraBlockClass);
				return type;
			}
		}

		[DataMember]
		[Category("Camera")]
		public bool IsActive
		{
			get
			{
				if(BackingObject == null || ActualObject == null)
					return ObjectBuilder.IsActive;

				return GetIsCameraActive();
			}
			set
			{
				if(IsActive == value) return;
				ObjectBuilder.IsActive = value;
				m_isActive = value;
				Changed = true;

				if(BackingObject != null && ActualObject != null)
				{
					Action action = InternalUpdateCameraIsActive;
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

				Type type = InternalType;
				if (type == null)
					throw new Exception("Could not find internal type for CameraBlockEntity");

				result &= HasMethod(type, CameraBlockGetIsActiveMethod);
				result &= HasMethod(type, CameraBlockSetIsActiveMethod);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		protected bool GetIsCameraActive()
		{
			Object rawResult = InvokeEntityMethod(ActualObject, CameraBlockGetIsActiveMethod);
			if (rawResult == null)
				return false;
			bool result = (bool)rawResult;
			return result;
		}

		protected void InternalUpdateCameraIsActive()
		{
			InvokeEntityMethod(ActualObject, CameraBlockSetIsActiveMethod, new object[] { m_isActive });
		}

		#endregion
	}
}
