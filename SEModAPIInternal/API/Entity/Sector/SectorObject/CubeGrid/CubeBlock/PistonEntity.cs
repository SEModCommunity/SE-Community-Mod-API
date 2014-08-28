using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract(Name = "PistonEntityProxy")]
	public class PistonEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		public static string PistonNamespace = "";
		public static string PistonClass = "";

		#endregion

		#region "Constructors and Intializers"

		public PistonEntity(CubeGridEntity parent, MyObjectBuilder_PistonBase definition)
			: base(parent, definition)
		{
		}

		public PistonEntity(CubeGridEntity parent, MyObjectBuilder_PistonBase definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Merge Block")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(PistonNamespace, PistonClass);
				return type;
			}
		}

		[IgnoreDataMember]
		[Category("Piston")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_PistonBase ObjectBuilder
		{
			get { return (MyObjectBuilder_PistonBase)base.ObjectBuilder; }
			set
			{
				base.ObjectBuilder = value;
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
					throw new Exception("Could not find internal type for PistonEntity");

				//result &= HasMethod(type, BatteryBlockGetCurrentStoredPowerMethod);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		#endregion
	}
}
