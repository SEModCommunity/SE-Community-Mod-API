using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	public class BatteryBlockEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		public static string BatteryBlockNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string BatteryBlockClass = "711CB30D2043393F07630CD237B5EFBF";
		public static string BatteryBlockSetCurrentStoredPowerMethod = "365694972F163426A27531B867041ABB";
		public static string BatteryBlockSetMaxStoredPowerMethod = "51188413AE93A8E2B2375B7721F1A3FC";
		public static string BatteryBlockSetProducerEnabledMethod = "5538173B5047FC438226267C0088356E";

		#endregion

		#region "Constructors and Initializers"

		public BatteryBlockEntity(CubeGridEntity parent, MyObjectBuilder_BatteryBlock definition)
			: base(parent, definition)
		{
		}

		public BatteryBlockEntity(CubeGridEntity parent, MyObjectBuilder_BatteryBlock definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
		}

		#endregion

		#region "Properties"

		[Category("Battery Block")]
		public float CurrentStoredPower
		{
			get { return GetSubTypeEntity().CurrentStoredPower; }
			set
			{
				if (GetSubTypeEntity().CurrentStoredPower == value) return;
				GetSubTypeEntity().CurrentStoredPower = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateBatteryBlock;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Battery Block")]
		public float MaxStoredPower
		{
			get { return GetSubTypeEntity().MaxStoredPower; }
			set
			{
				if (GetSubTypeEntity().MaxStoredPower == value) return;
				GetSubTypeEntity().MaxStoredPower = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateBatteryBlock;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Battery Block")]
		public bool ProducerEnabled
		{
			get { return GetSubTypeEntity().ProducerEnabled; }
			set
			{
				if (GetSubTypeEntity().ProducerEnabled == value) return;
				GetSubTypeEntity().ProducerEnabled = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateBatteryBlock;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_BatteryBlock GetSubTypeEntity()
		{
			return (MyObjectBuilder_BatteryBlock)BaseEntity;
		}

		#region "Internal"

		protected void InternalUpdateBatteryBlock()
		{
			try
			{
				InvokeEntityMethod(BackingObject, BatteryBlockSetCurrentStoredPowerMethod, new object[] { CurrentStoredPower });
				InvokeEntityMethod(BackingObject, BatteryBlockSetMaxStoredPowerMethod, new object[] { MaxStoredPower });
				InvokeEntityMethod(BackingObject, BatteryBlockSetProducerEnabledMethod, new object[] { ProducerEnabled });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#endregion

		#endregion
	}
}
