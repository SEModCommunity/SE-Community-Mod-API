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

		public static string FunctionalBlockNamespace = "6DDCED906C852CFDABA0B56B84D0BD74";
		public static string FunctionalBlockClass = "7085736D64DCC58ED5DCA05FFEEA9664";
		public static string FunctionalBlockSetEnabledMethod = "97EC0047E8B562F4590B905BD8571F51";

		#endregion

		#region "Constructors and Initializers"

		public FunctionalBlockEntity(MyObjectBuilder_FunctionalBlock definition)
			: base(definition)
		{
			EntityId = definition.EntityId;
		}

		#endregion

		#region "Properties"

		[Category("Functional Block")]
		public bool Enabled
		{
			get { return GetSubTypeEntity().Enabled; }
			set
			{
				if (GetSubTypeEntity().Enabled == value) return;
				GetSubTypeEntity().Enabled = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalSetEnabled;
					SandboxGameAssemblyWrapper.EnqueueMainGameAction(action);
				}
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_FunctionalBlock GetSubTypeEntity()
		{
			return (MyObjectBuilder_FunctionalBlock)BaseEntity;
		}

		public void InternalSetEnabled()
		{
			try
			{
				Type backingType = BackingObject.GetType();
				MethodInfo method = backingType.GetMethod(CubeBlockGetActualBlock_Method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
				Object actualCubeObject = method.Invoke(m_self.BackingObject, new object[] { });

				if (SandboxGameAssemblyWrapper.IsDebugging)
				{
					Console.WriteLine("FunctionalBlock '" + Name + "': Setting enabled/disabled to '" + (Enabled ? "enabled" : "disabled") + "'");
				}

				Type actualType = actualCubeObject.GetType();
				while (actualType.Name != FunctionalBlockClass && actualType.Name != "" && actualType.Name != "Object")
				{
					actualType = actualType.BaseType;
				}
				MethodInfo method2 = actualType.GetMethod(FunctionalBlockSetEnabledMethod, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
				method2.Invoke(actualCubeObject, new object[] { Enabled });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex.ToString());
			}
		}

		#endregion
	}
}
