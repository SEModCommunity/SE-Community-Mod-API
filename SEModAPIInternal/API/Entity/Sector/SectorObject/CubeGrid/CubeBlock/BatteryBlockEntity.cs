using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

using Sandbox.Definitions;
using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	public class BatteryBlockEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private MyBatteryBlockDefinition m_batteryBlock;

		//Internal class
		public static string BatteryBlockNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string BatteryBlockClass = "711CB30D2043393F07630CD237B5EFBF";

		//Internal methods
		public static string BatteryBlockGetCurrentStoredPowerMethod = "82DBD55631B1D9694F1DCB5BFF88AB5B";
		public static string BatteryBlockSetCurrentStoredPowerMethod = "365694972F163426A27531B867041ABB";
		public static string BatteryBlockGetMaxStoredPowerMethod = "1E1C89D073DDC026426B44820B1A6286";
		public static string BatteryBlockSetMaxStoredPowerMethod = "51188413AE93A8E2B2375B7721F1A3FC";
		public static string BatteryBlockGetProducerEnabledMethod = "36B457125A54787901017D24BD0E3346";
		public static string BatteryBlockSetProducerEnabledMethod = "5538173B5047FC438226267C0088356E";

		//Internal fields
		public static string BatteryBlockCurrentStoredPowerField = "736E72768436E8A7C1F33EF1F4396B9E";
		public static string BatteryBlockMaxStoredPowerField = "3E888DF7D4F5C207088050DF6CA348D5";
		public static string BatteryBlockProducerEnabledField = "5CE4521F11C6B1D64721848D226F15BF";
		public static string BatteryBlockBatteryDefinitionField = "F0C59D70E13560B7212CEF8CF082A67B";

		#endregion

		#region "Constructors and Initializers"

		public BatteryBlockEntity(CubeGridEntity parent, MyObjectBuilder_BatteryBlock definition)
			: base(parent, definition)
		{
			m_batteryBlock = new MyBatteryBlockDefinition();
		}

		public BatteryBlockEntity(CubeGridEntity parent, MyObjectBuilder_BatteryBlock definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
			m_batteryBlock = new MyBatteryBlockDefinition();
		}

		#endregion

		#region "Properties"

		[Category("Battery Block")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_BatteryBlock ObjectBuilder
		{
			get
			{
				return (MyObjectBuilder_BatteryBlock)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[Category("Battery Block")]
		public float CurrentStoredPower
		{
			get { return ObjectBuilder.CurrentStoredPower; }
			set
			{
				if (ObjectBuilder.CurrentStoredPower == value) return;
				ObjectBuilder.CurrentStoredPower = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateBatteryBlockCurrentStoredPower;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Battery Block")]
		public float MaxStoredPower
		{
			get { return ObjectBuilder.MaxStoredPower; }
			set
			{
				if (ObjectBuilder.MaxStoredPower == value) return;
				ObjectBuilder.MaxStoredPower = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateBatteryBlockMaxStoredPower;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Battery Block")]
		public bool ProducerEnabled
		{
			get { return ObjectBuilder.ProducerEnabled; }
			set
			{
				if (ObjectBuilder.ProducerEnabled == value) return;
				ObjectBuilder.ProducerEnabled = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateBatteryBlockProducerEnabled;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Battery Block")]
		public float RequiredPowerInput
		{
			get
			{
				if (ActualObject != null)
					InternalGetBatteryBlockDefinition();

				return m_batteryBlock.RequiredPowerInput;
			}
			set
			{
				if (m_batteryBlock.RequiredPowerInput == value) return;
				m_batteryBlock.RequiredPowerInput = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalSetBatteryBlockDefinition;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Battery Block")]
		public float MaxPowerOutput
		{
			get
			{
				if (ActualObject != null)
					InternalGetBatteryBlockDefinition();

				return m_batteryBlock.MaxPowerOutput;
			}
			set
			{
				if (m_batteryBlock.MaxPowerOutput == value) return;
				m_batteryBlock.MaxPowerOutput = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalSetBatteryBlockDefinition;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		#endregion

		#region "Methods"

		#region "Internal"

		protected void InternalGetBatteryBlockDefinition()
		{
			try
			{
				FieldInfo field = GetEntityField(ActualObject, BatteryBlockBatteryDefinitionField);
				if (field == null)
					return;
				m_batteryBlock = (MyBatteryBlockDefinition)field.GetValue(ActualObject);
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected void InternalSetBatteryBlockDefinition()
		{
			try
			{
				FieldInfo field = GetEntityField(ActualObject, BatteryBlockBatteryDefinitionField);
				if (field == null)
					return;
				field.SetValue(ActualObject, m_batteryBlock);
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected void InternalUpdateBatteryBlockCurrentStoredPower()
		{
			try
			{
				InvokeEntityMethod(BackingObject, BatteryBlockSetCurrentStoredPowerMethod, new object[] { CurrentStoredPower });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected void InternalUpdateBatteryBlockMaxStoredPower()
		{
			try
			{
				InvokeEntityMethod(BackingObject, BatteryBlockSetMaxStoredPowerMethod, new object[] { MaxStoredPower });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected void InternalUpdateBatteryBlockProducerEnabled()
		{
			try
			{
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
