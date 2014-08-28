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
	[DataContract(Name = "RotorEntityProxy")]
	public class RotorEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		public static string RotorNamespace = "";
		public static string RotorClass = "";

		#endregion

		#region "Constructors and Intializers"

		public RotorEntity(CubeGridEntity parent, MyObjectBuilder_MotorBase definition)
			: base(parent, definition)
		{
		}

		public RotorEntity(CubeGridEntity parent, MyObjectBuilder_MotorBase definition, Object backingObject)
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
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(RotorNamespace, RotorClass);
				return type;
			}
		}

		[IgnoreDataMember]
		[Category("Rotor")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_MotorBase ObjectBuilder
		{
			get { return (MyObjectBuilder_MotorBase)base.ObjectBuilder; }
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
					throw new Exception("Could not find internal type for RotorEntity");

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
