using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	public class BeaconEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private RadioManager m_radioManager;

		public static string BeaconNamespace = "6DDCED906C852CFDABA0B56B84D0BD74";
		public static string BeaconClass = "BF2916D43CF7F023839AD13F50F31990";
		public static string BeaconGetRadioManagerMethod = "A80801D0C8F1D45AB0E81B934FB5EF90";

		#endregion

		#region "Constructors and Initializers"

		public BeaconEntity(CubeGridEntity parent, MyObjectBuilder_Beacon definition)
			: base(parent, definition)
		{
		}

		public BeaconEntity(CubeGridEntity parent, MyObjectBuilder_Beacon definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
			m_radioManager = new RadioManager(InternalGetRadioManager());
		}

		#endregion

		#region "Properties"

		[Category("Beacon")]
		public float BroadcastRadius
		{
			get
			{
				float result = GetSubTypeEntity().BroadcastRadius;

				if (m_radioManager != null)
					result = m_radioManager.BroadcastRadius;

				return result;
			}
			set
			{
				if (GetSubTypeEntity().BroadcastRadius == value) return;
				GetSubTypeEntity().BroadcastRadius = value;
				Changed = true;

				if(m_radioManager != null)
					m_radioManager.BroadcastRadius = BroadcastRadius;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_Beacon GetSubTypeEntity()
		{
			return (MyObjectBuilder_Beacon)BaseEntity;
		}

		#region "Internal"

		protected void InternalUpdateBeacon()
		{
			try
			{
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected Object InternalGetRadioManager()
		{
			try
			{
				Object result = InvokeEntityMethod(BackingObject, BeaconGetRadioManagerMethod);
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
