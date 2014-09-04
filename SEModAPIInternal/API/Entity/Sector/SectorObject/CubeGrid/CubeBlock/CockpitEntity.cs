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
	[DataContract(Name = "CockpitEntityProxy")]
	public class CockpitEntity : ShipControllerEntity
	{
		#region "Attributes"

		private bool m_weaponStatus;

		public static string CockpitEntityNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string CockpitEntityClass = "0A875207E28B2C7707366CDD300684DF";

		#endregion

		#region "Constructors and Initializers"

		public CockpitEntity(CubeGridEntity parent, MyObjectBuilder_Cockpit definition)
			: base(parent, definition)
		{
		}

		public CockpitEntity(CubeGridEntity parent, MyObjectBuilder_Cockpit definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
		}

		#endregion

		#region "Properties"

		[DataMember]
		[Category("Cockpit")]
		[ReadOnly(true)]
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

				return result;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine(ex);
				return false;
			}
		}

		public void FireWeapons()
		{
			if (m_weaponStatus)
				return;

			m_weaponStatus = true;

			Action action = InternalFireWeapons;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		public void StopWeapons()
		{
			if (!m_weaponStatus)
				return;

			m_weaponStatus = false;

			Action action = InternalStopWeapons;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		protected void InternalFireWeapons()
		{
			//TODO - Patch 1.046 broke all of this. Find another method to call
		}

		protected void InternalStopWeapons()
		{
			//TODO - Patch 1.046 broke all of this. Find another method to call
		}

		#endregion
	}
}
