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
	public class TerminalBlockEntity : CubeBlockEntity
	{
		#region "Attributes"

		public static string TerminalBlockNamespace = "6DDCED906C852CFDABA0B56B84D0BD74";
		public static string TerminalBlockClass = "CCFD704C70C3F20F7E84E8EA42D7A730";
		public static string TerminalBlockGetCustomNameMethod = "DE9705A29F3FE6F1E501595879B2E54F";
		public static string TerminalBlockSetCustomNameMethod = "774FC8084C0899CEF5C8DAE867B847FE";
		public static string TerminalBlockBroadcastCustomNameMethod = "97B2C51E83D10649FBF8E598D77C8BF8";

		#endregion

		#region "Constructors and Initializers"

		public TerminalBlockEntity(CubeGridEntity parent, MyObjectBuilder_TerminalBlock definition)
			: base(parent, definition)
		{
		}

		public TerminalBlockEntity(CubeGridEntity parent, MyObjectBuilder_TerminalBlock definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
		}

		#endregion

		#region "Properties"

		public override string Name
		{
			get
			{
				String name = CustomName;
				if (name == null || name == "")
					name = base.Name;
				return name;
			}
		}

		[Category("Terminal Block")]
		public string CustomName
		{
			get { return GetSubTypeEntity().CustomName; }
			set
			{
				if (GetSubTypeEntity().CustomName == value) return;
				GetSubTypeEntity().CustomName = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalSetCustomName;
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
		new internal MyObjectBuilder_TerminalBlock GetSubTypeEntity()
		{
			return (MyObjectBuilder_TerminalBlock)BaseEntity;
		}

		public void InternalSetCustomName()
		{
			try
			{
				Object actualCubeObject = GetActualObject();

				if (SandboxGameAssemblyWrapper.IsDebugging)
				{
					Console.WriteLine("TerminalBlock '" + Name + "': Setting custom name to '" + CustomName + "'");
				}

				StringBuilder newCustomName = new StringBuilder(CustomName);

				Type actualType = actualCubeObject.GetType();
				while (actualType.Name != TerminalBlockClass && actualType.Name != "" && actualType.Name != "Object")
				{
					actualType = actualType.BaseType;
				}
				MethodInfo method2 = actualType.GetMethod(TerminalBlockSetCustomNameMethod, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
				method2.Invoke(actualCubeObject, new object[] { newCustomName });
				MethodInfo method3 = actualType.GetMethod(TerminalBlockBroadcastCustomNameMethod, BindingFlags.NonPublic | BindingFlags.Static);
				method3.Invoke(null, new object[] { actualCubeObject, newCustomName.ToString() });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
