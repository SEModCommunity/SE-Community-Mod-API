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
	[DataContract(Name = "VirtualMassEntityProxy")]
	public class VirtualMassEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		public static string VirtualMassNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string VirtualMassClass = "ADD99AA5F8D01EA9FCF30E8AEE011CCD";

		#endregion

		#region "Constructors and Initializers"

		public VirtualMassEntity(CubeGridEntity parent, MyObjectBuilder_VirtualMass definition)
			: base(parent, definition)
		{
		}

		public VirtualMassEntity(CubeGridEntity parent, MyObjectBuilder_VirtualMass definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Virtual Mass")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_VirtualMass ObjectBuilder
		{
			get
			{
				return (MyObjectBuilder_VirtualMass)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[IgnoreDataMember]
		[Category("Virtual Mass")]
		[Browsable(false)]
		[ReadOnly(true)]
		new public static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(VirtualMassNamespace, VirtualMassClass);
				return type;
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
					throw new Exception("Could not find internal type for VirtualMassEntity");

				//result &= HasMethod(type, AntennaGetRadioManagerMethod);

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
