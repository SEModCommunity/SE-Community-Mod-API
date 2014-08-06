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
	[DataContract(Name = "AntennaEntityProxy")]
	public class AntennaEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private RadioManager m_radioManager;

		public static string AntennaNamespace = "6DDCED906C852CFDABA0B56B84D0BD74";
		public static string AntennaClass = "BEFE9BC9C0DE00D5CD95A054194EF6AB";

		public static string AntennaGetRadioManagerMethod = "C7762F68C17313AFB9CB8A5FA0528A50";

		#endregion

		#region "Constructors and Initializers"

		public AntennaEntity(CubeGridEntity parent, MyObjectBuilder_RadioAntenna definition)
			: base(parent, definition)
		{
		}

		public AntennaEntity(CubeGridEntity parent, MyObjectBuilder_RadioAntenna definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
			Object internalRadioManager = InternalGetRadioManager();
			m_radioManager = new RadioManager(internalRadioManager);
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Antenna")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_RadioAntenna ObjectBuilder
		{
			get
			{
				MyObjectBuilder_RadioAntenna antenna = (MyObjectBuilder_RadioAntenna)base.ObjectBuilder;

				return antenna;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category("Antenna")]
		public float BroadcastRadius
		{
			get
			{
				float result = ObjectBuilder.BroadcastRadius;

				if (m_radioManager != null)
					result = m_radioManager.BroadcastRadius;

				return result;
			}
			set
			{
				if (ObjectBuilder.BroadcastRadius == value) return;
				ObjectBuilder.BroadcastRadius = value;
				Changed = true;

				if(m_radioManager != null)
					m_radioManager.BroadcastRadius = value;
			}
		}

		[IgnoreDataMember]
		[Category("Antenna")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal RadioManager RadioManager
		{
			get { return m_radioManager; }
		}

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				bool result = true;

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(AntennaNamespace, AntennaClass);
				if (type == null)
					throw new Exception("Could not find internal type for AntennaEntity");
				result &= HasMethod(type, AntennaGetRadioManagerMethod);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		#region "Internal"

		protected Object InternalGetRadioManager()
		{
			try
			{
				Object result = InvokeEntityMethod(ActualObject, AntennaGetRadioManagerMethod);
				return result;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		#endregion

		#endregion
	}
}
