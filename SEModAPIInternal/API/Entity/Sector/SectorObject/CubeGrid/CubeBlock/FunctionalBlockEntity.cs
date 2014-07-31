using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	public class FunctionalBlockEntity : TerminalBlockEntity
	{
		#region "Attributes"

		private PowerReceiver m_powerReceiver;

		public static string FunctionalBlockNamespace = "6DDCED906C852CFDABA0B56B84D0BD74";
		public static string FunctionalBlockClass = "7085736D64DCC58ED5DCA05FFEEA9664";

		public static string FunctionalBlockSetEnabledMethod = "97EC0047E8B562F4590B905BD8571F51";
		public static string FunctionalBlockBroadcastEnabledMethod = "D979DB9AA474782929587EC7DE5E53AA";
		public static string FunctionalBlockGetPowerReceiverMethod = "get_PowerReceiver";

		#endregion

		#region "Constructors and Initializers"

		public FunctionalBlockEntity(CubeGridEntity parent, MyObjectBuilder_FunctionalBlock definition)
			: base(parent, definition)
		{
		}

		public FunctionalBlockEntity(CubeGridEntity parent, MyObjectBuilder_FunctionalBlock definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
			m_powerReceiver = new PowerReceiver(ActualObject, Parent.PowerManager, InternalGetPowerReceiver(), new Func<float>(InternalPowerReceiverCallback));
		}

		#endregion

		#region "Properties"

		[Category("Functional Block")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_FunctionalBlock ObjectBuilder
		{
			get
			{
				return (MyObjectBuilder_FunctionalBlock)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[Category("Functional Block")]
		public bool Enabled
		{
			get { return ObjectBuilder.Enabled; }
			set
			{
				var baseEntity = ObjectBuilder;
				if (baseEntity.Enabled == value) return;
				baseEntity.Enabled = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateFunctionalBlock;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Functional Block")]
		public float CurrentInput
		{
			get
			{
				return PowerReceiver.CurrentInput;
			}
		}

		[Category("Functional Block")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal PowerReceiver PowerReceiver
		{
			get { return m_powerReceiver; }
		}

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(FunctionalBlockNamespace, FunctionalBlockClass);
				if (type == null)
					throw new Exception("Could not find internal type for FunctionalBlockEntity");
				bool result = true;
				result &= HasMethod(type, FunctionalBlockSetEnabledMethod);
				result &= HasMethod(type, FunctionalBlockBroadcastEnabledMethod);
				//result &= HasMethod(type, FunctionalBlockGetPowerReceiverMethod);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine(ex);
				return false;
			}
		}

		protected void InternalUpdateFunctionalBlock()
		{
			try
			{
				Object actualCubeObject = GetActualObject();

				if (SandboxGameAssemblyWrapper.IsDebugging)
				{
					Console.WriteLine("FunctionalBlock '" + Name + "': Setting enabled/disabled to '" + (Enabled ? "enabled" : "disabled") + "'");
				}

				Type actualType = actualCubeObject.GetType();
				int iterationCutoff = 5;
				while (actualType.Name != FunctionalBlockClass && actualType.Name != "" && actualType.Name != "Object" && iterationCutoff > 0)
				{
					actualType = actualType.BaseType;
					iterationCutoff--;
				}
				MethodInfo method2 = actualType.GetMethod(FunctionalBlockSetEnabledMethod);
				method2.Invoke(actualCubeObject, new object[] { Enabled });
				MethodInfo method3 = actualType.GetMethod(FunctionalBlockBroadcastEnabledMethod);
				method3.Invoke(actualCubeObject, new object[] { Enabled });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected virtual float InternalPowerReceiverCallback()
		{
			return 0;
		}

		protected virtual Object InternalGetPowerReceiver()
		{
			Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(FunctionalBlockNamespace, FunctionalBlockClass);
			if (!HasMethod(type, FunctionalBlockGetPowerReceiverMethod))
				return null;

			return InvokeEntityMethod(ActualObject, FunctionalBlockGetPowerReceiverMethod);
		}

		#endregion
	}
}
