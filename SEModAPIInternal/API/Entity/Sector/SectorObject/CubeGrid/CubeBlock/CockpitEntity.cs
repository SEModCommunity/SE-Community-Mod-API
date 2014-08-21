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
	[DataContract(Name = "CockpitEntityProxy")]
	public class CockpitEntity : TerminalBlockEntity
	{
		#region "Attributes"

		private CockpitNetworkManager m_networkManager;

		public static string CockpitEntityNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string CockpitEntityClass = "0A875207E28B2C7707366CDD300684DF";

		public static string CockpitEntityGetNetworkManager = "FBCE4A2D52D8DBD116860A130829EED5";

		#endregion

		#region "Constructors and Initializers"

		public CockpitEntity(CubeGridEntity parent, MyObjectBuilder_Cockpit definition)
			: base(parent, definition)
		{
		}

		public CockpitEntity(CubeGridEntity parent, MyObjectBuilder_Cockpit definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
			m_networkManager = new CockpitNetworkManager(GetCockpitNetworkManager(), this);
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Cockpit")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_Cockpit ObjectBuilder
		{
			get
			{
				MyObjectBuilder_Cockpit objectBuilder = (MyObjectBuilder_Cockpit)base.ObjectBuilder;

				return objectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category("Cockpit")]
		[ReadOnly(true)]
		public bool ControlThrusters
		{
			get { return ObjectBuilder.ControlThrusters; }
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category("Cockpit")]
		[ReadOnly(true)]
		public bool ControlWheels
		{
			get { return ObjectBuilder.ControlWheels; }
			private set
			{
				//Do nothing!
			}
		}

		[IgnoreDataMember]
		[Category("Cockpit")]
		[ReadOnly(true)]
		public MyObjectBuilder_AutopilotBase Autopilot
		{
			get { return ObjectBuilder.Autopilot; }
			private set
			{
				//Do nothing!
			}
		}

		[IgnoreDataMember]
		[Category("Cockpit")]
		[ReadOnly(true)]
		public MyObjectBuilder_Character Pilot
		{
			get { return ObjectBuilder.Pilot; }
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category("Cockpit")]
		public bool IsPassengerSeat
		{
			get
			{
				if (ObjectBuilder.SubtypeName == "PassengerSeatLarge")
					return true;

				if (ObjectBuilder.SubtypeName == "PassengerSeatSmall")
					return true;

				return false;
			}
			private set
			{
				//Do nothing!
			}
		}

		[IgnoreDataMember]
		[Category("Cockpit")]
		[Browsable(false)]
		public CockpitNetworkManager NetworkManager
		{
			get { return m_networkManager; }
		}

		#endregion

		#region "Methods"

		new public static bool ReflectionUnitTest()
		{
			try
			{
				bool result = true;
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(CockpitEntityNamespace, CockpitEntityClass);
				if (type == null)
					throw new Exception("Could not find type for CockpitEntity");
				result &= BaseObject.HasMethod(type, CockpitEntityGetNetworkManager);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine(ex);
				return false;
			}
		}

		protected Object GetCockpitNetworkManager()
		{
			Object result = InvokeEntityMethod(ActualObject, CockpitEntityGetNetworkManager);
			return result;
		}

		#endregion
	}

	public class CockpitNetworkManager
	{
		#region "Attributes"

		private Object m_networkManager;
		private CockpitEntity m_parent;

		public static string CockpitNetworkManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string CockpitNetworkManagerClass = "F622141FE0D93611A749A5DB71DF471F";

		public static string CockpitNetworkManagerBroadcastDampenersStatus = "CF03B0F414BEA9134AFC06C2F31333E8";

		#endregion

		#region "Constructors and Initializers"

		public CockpitNetworkManager(Object networkManager, CockpitEntity parent)
		{
			m_networkManager = networkManager;
			m_parent = parent;
		}

		#endregion

		#region "Properties"

		public Object BackingObject
		{
			get { return m_networkManager; }
		}

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				bool result = true;
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(CockpitNetworkManagerNamespace, CockpitNetworkManagerClass);
				if (type == null)
					throw new Exception("Could not find type for CockpitNetworkManager");
				result &= BaseObject.HasMethod(type, CockpitNetworkManagerBroadcastDampenersStatus);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine(ex);
				return false;
			}
		}

		public void BroadcastDampenersStatus(bool status)
		{
			BaseObject.InvokeEntityMethod(BackingObject, CockpitNetworkManagerBroadcastDampenersStatus, new object[] { status });
		}

		#endregion
	}
}
