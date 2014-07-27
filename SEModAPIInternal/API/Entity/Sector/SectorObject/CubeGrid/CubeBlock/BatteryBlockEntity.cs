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

		private PowerReceiver m_powerReceiver;
		private float m_maxPowerOutput;
		private float m_maxStoredPower;

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
		public static string BatteryBlockGetSemiautoEnabledMethod = "19312D5BF11FBC0A682B613E21621BA6";
		public static string BatteryBlockSetSemiautoEnabledMethod = "A3BEE5A757F096951F158F9FFF5A878A";
		public static string BatteryBlockGetPowerReceiverMethod = "get_PowerReceiver";

		//Internal fields
		public static string BatteryBlockCurrentStoredPowerField = "736E72768436E8A7C1F33EF1F4396B9E";
		public static string BatteryBlockMaxStoredPowerField = "3E888DF7D4F5C207088050DF6CA348D5";
		public static string BatteryBlockProducerEnabledField = "5CE4521F11C6B1D64721848D226F15BF";
		public static string BatteryBlockSemiautoEnabledField = "61505AAA6C86342099EFC9D89532BBE7";
		public static string BatteryBlockBatteryDefinitionField = "F0C59D70E13560B7212CEF8CF082A67B";
		public static string BatteryBlockNetManagerField = "E93BD8EF419C322C547231F9BF991090";

		public static string BatteryBlockNetManagerNamespace = BatteryBlockNamespace + BatteryBlockClass;
		public static string BatteryBlockNetManagerClass = "6704740496C47C5FDE69887798D17883";
		public static string BatteryBlockNetManagerBroadcastProducerEnabledMethod = "280D7AE8C0F523FF089618970C13B55B";
		public static string BatteryBlockNetManagerBroadcastCurrentStoredPowerMethod = "F512BA7EF29F6A8B7DE3D56BAAC0207B";

		#endregion

		#region "Constructors and Initializers"

		public BatteryBlockEntity(CubeGridEntity parent, MyObjectBuilder_BatteryBlock definition)
			: base(parent, definition)
		{
		}

		public BatteryBlockEntity(CubeGridEntity parent, MyObjectBuilder_BatteryBlock definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
			m_powerReceiver = new PowerReceiver(ActualObject, Parent.PowerManager, InternalGetPowerReceiver(), new Func<float>(PowerRateCallback));
			m_maxPowerOutput = 0;
			m_maxStoredPower = definition.MaxStoredPower;
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
				MyObjectBuilder_BatteryBlock batteryBlock = (MyObjectBuilder_BatteryBlock)base.ObjectBuilder;

				batteryBlock.MaxStoredPower = m_maxStoredPower;

				return batteryBlock;
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
			get
			{
				float maxStoredPower = 0;

				if (ActualObject != null)
				{
					maxStoredPower = (float)InvokeEntityMethod(ActualObject, BatteryBlockGetMaxStoredPowerMethod);
				}
				else
				{
					maxStoredPower = m_maxStoredPower;
				}

				return maxStoredPower;
			}
			set
			{
				if (m_maxStoredPower == value) return;
				m_maxStoredPower = value;
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
		public bool SemiautoEnabled
		{
			get { return ObjectBuilder.SemiautoEnabled; }
			set
			{
				if (ObjectBuilder.SemiautoEnabled == value) return;
				ObjectBuilder.SemiautoEnabled = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateBatteryBlockSemiautoEnabled;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Battery Block")]
		public float RequiredPowerInput
		{
			get { return PowerReceiver.MaxRequiredInput; }
			set
			{
				if (PowerReceiver.MaxRequiredInput == value) return;
				PowerReceiver.MaxRequiredInput = value;
				Changed = true;
			}
		}

		[Category("Battery Block")]
		public float MaxPowerOutput
		{
			get { return m_maxPowerOutput; }
			set
			{
				if (m_maxPowerOutput == value) return;
				m_maxPowerOutput = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateBatteryBlockMaxPowerOutput;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Battery")]
		[Description("Internal power receiver object")]
		[Browsable(false)]
		internal PowerReceiver PowerReceiver
		{
			get { return m_powerReceiver; }
		}

		#endregion

		#region "Methods"

		#region "Internal"

		protected float PowerRateCallback()
		{
			if(ProducerEnabled || (CurrentStoredPower / MaxStoredPower) >= 0.98)
			{
				return 0.0f;
			}
			else
			{
				return PowerReceiver.MaxRequiredInput;
			}
		}

		protected Object InternalGetPowerReceiver()
		{
			Object result = InvokeEntityMethod(ActualObject, BatteryBlockGetPowerReceiverMethod);

			return result;
		}

		protected Object InternalGetNetManager()
		{
			try
			{
				FieldInfo field = GetEntityField(ActualObject, BatteryBlockNetManagerField);
				Object result = field.GetValue(ActualObject);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		protected void InternalUpdateBatteryBlockCurrentStoredPower()
		{
			try
			{
				InvokeEntityMethod(ActualObject, BatteryBlockSetCurrentStoredPowerMethod, new object[] { CurrentStoredPower });
				InvokeEntityMethod(InternalGetNetManager(), BatteryBlockNetManagerBroadcastCurrentStoredPowerMethod, new object[] { CurrentStoredPower });
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
				InvokeEntityMethod(ActualObject, BatteryBlockSetMaxStoredPowerMethod, new object[] { m_maxStoredPower });
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
				InvokeEntityMethod(ActualObject, BatteryBlockSetProducerEnabledMethod, new object[] { ProducerEnabled });
				InvokeEntityMethod(InternalGetNetManager(), BatteryBlockNetManagerBroadcastProducerEnabledMethod, new object[] { ProducerEnabled });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected void InternalUpdateBatteryBlockSemiautoEnabled()
		{
			try
			{
				InvokeEntityMethod(ActualObject, BatteryBlockSetSemiautoEnabledMethod, new object[] { SemiautoEnabled });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected void InternalUpdateBatteryBlockMaxPowerOutput()
		{
			//TODO - Do stuff
		}

		#endregion

		#endregion
	}
}
