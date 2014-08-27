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
	[DataContract(Name = "SolarPanelEntityProxy")]
	public class SolarPanelEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private PowerProducer m_powerProducer;
		private float m_maxPowerOutput;

		public static string SolarPanelNamespace = "AAD9061F948E6A3635200145188D64A9";
		public static string SolarPanelClass = "6238A2EF481D720035D5BC6E545E769C";

		public static string SolarPanelSetMaxOutputMethod = "802879712B29AAC2DB4EC9F7B128C979";

		#endregion

		#region "Constructors and Intializers"

		public SolarPanelEntity(CubeGridEntity parent, MyObjectBuilder_SolarPanel definition)
			: base(parent, definition)
		{
		}

		public SolarPanelEntity(CubeGridEntity parent, MyObjectBuilder_SolarPanel definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
			m_powerProducer = new PowerProducer(Parent.PowerManager, ActualObject);
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Solar Panel")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_SolarPanel ObjectBuilder
		{
			get { return (MyObjectBuilder_SolarPanel)base.ObjectBuilder; }
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category("Solar Panel")]
		public float MaxPower
		{
			get { return PowerProducer.MaxPowerOutput; }
			set
			{
				m_maxPowerOutput = value;

				Action action = InternalUpdateMaxPowerOutput;
				SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
			}
		}

		[DataMember]
		[Category("Solar Panel")]
		public float Power
		{
			get { return PowerProducer.PowerOutput; }
			set { PowerProducer.PowerOutput = value; }
		}

		[IgnoreDataMember]
		[Category("Solar Panel")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal PowerProducer PowerProducer
		{
			get { return m_powerProducer; }
		}

		#endregion

		#region "Methods"

		new public static bool ReflectionUnitTest()
		{
			try
			{
				bool result = true;

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(SolarPanelNamespace, SolarPanelClass);
				if (type == null)
					throw new Exception("Could not find internal type for SolarPanelEntity");
				result &= HasMethod(type, SolarPanelSetMaxOutputMethod);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		protected void InternalUpdateMaxPowerOutput()
		{
			InvokeEntityMethod(ActualObject, SolarPanelSetMaxOutputMethod, new object[] { m_maxPowerOutput });
		}

		#endregion
	}
}
