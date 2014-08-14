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
	[DataContract(Name = "GravityBaseEntityProxy")]
	public class GravityBaseEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private float m_acceleration;

		public static string GravityBaseNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string GravityBaseClass = "8CCFFE7D12F92AA0036E030AEAB6236F";

		public static string GravityBaseSetAccelerationMethod = "F73ED856331E83CE2A7A37C0FA54AFB9";

		#endregion

		#region "Constructors and Initializers"

		public GravityBaseEntity(CubeGridEntity parent, MyObjectBuilder_FunctionalBlock definition)
			: base(parent, definition)
		{
			m_acceleration = 9.81f;
		}

		public GravityBaseEntity(CubeGridEntity parent, MyObjectBuilder_FunctionalBlock definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
			m_acceleration = 9.81f;
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Gravity Base")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_FunctionalBlock ObjectBuilder
		{
			get
			{
				MyObjectBuilder_FunctionalBlock gravity = (MyObjectBuilder_FunctionalBlock)base.ObjectBuilder;

				return gravity;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category("Gravity Base")]
		public float GravityAcceleration
		{
			get { return m_acceleration; }
			set
			{
				if (m_acceleration == value) return;
				m_acceleration = value;
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

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(GravityBaseNamespace, GravityBaseClass);
				if (type == null)
					throw new Exception("Could not find internal type for GravityBaseEntity");
				result &= HasMethod(type, GravityBaseSetAccelerationMethod);

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
				InvokeEntityMethod(ActualObject, GravityBaseSetAccelerationMethod, new object[] { GravityAcceleration });
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
