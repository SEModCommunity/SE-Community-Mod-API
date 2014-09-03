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

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract(Name = "TerminalBlockEntityProxy")]
	[KnownType(typeof(InventoryEntity))]
	public class TerminalBlockEntity : CubeBlockEntity
	{
		#region "Attributes"

		private string m_customName;

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
			m_customName = definition.CustomName;
		}

		public TerminalBlockEntity(CubeGridEntity parent, MyObjectBuilder_TerminalBlock definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
			m_customName = definition.CustomName;
		}

		#endregion

		#region "Properties"

		[DataMember]
		[Category("Terminal Block")]
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

		[IgnoreDataMember]
		[Category("Terminal Block")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_TerminalBlock ObjectBuilder
		{
			get
			{
				try
				{
					if (m_objectBuilder == null)
						m_objectBuilder = new MyObjectBuilder_TerminalBlock();

					return (MyObjectBuilder_TerminalBlock)base.ObjectBuilder;
				}
				catch (Exception)
				{
					return new MyObjectBuilder_TerminalBlock();
				}
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category("Terminal Block")]
		public string CustomName
		{
			get
			{
				if(BackingObject == null || ActualObject == null)
					return ObjectBuilder.CustomName;

				return GetCustomName();
			}
			set
			{
				if (CustomName == value) return;
				ObjectBuilder.CustomName = value;
				m_customName = value;
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

		new public static bool ReflectionUnitTest()
		{
			try
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(TerminalBlockNamespace, TerminalBlockClass);
				if (type == null)
					throw new Exception("Could not find internal type for TerminalBlockEntity");
				bool result = true;
				result &= HasMethod(type, TerminalBlockGetCustomNameMethod);
				result &= HasMethod(type, TerminalBlockSetCustomNameMethod);
				result &= HasMethod(type, TerminalBlockBroadcastCustomNameMethod);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine(ex);
				return false;
			}
		}

		protected string GetCustomName()
		{
			Object rawObject = InvokeEntityMethod(ActualObject, TerminalBlockGetCustomNameMethod);
			if (rawObject == null)
				return "";
			StringBuilder result = (StringBuilder)rawObject;
			return result.ToString();
		}

		protected void InternalSetCustomName()
		{
			try
			{
				StringBuilder newCustomName = new StringBuilder(m_customName);

				InvokeEntityMethod(ActualObject, TerminalBlockSetCustomNameMethod, new object[] { newCustomName });
				InvokeStaticMethod(ActualObject.GetType(), TerminalBlockBroadcastCustomNameMethod, new object[] { ActualObject, newCustomName.ToString() });
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
