using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.API.Internal;

namespace SEModAPI.API.SaveData.Entity
{
	public class TerminalBlockEntity<T> : CubeBlockEntity<T> where T : MyObjectBuilder_TerminalBlock
	{
		#region "Constructors and Initializers"

		public TerminalBlockEntity(T definition)
			: base(definition)
		{
			EntityId = definition.EntityId;
		}

		#endregion

		#region "Properties"

		[Category("Terminal Block")]
		public string CustomName
		{
			get { return m_baseDefinition.CustomName; }
			set
			{
				if (m_baseDefinition.CustomName == value) return;
				m_baseDefinition.CustomName = value;
				Changed = true;

				if (BackingObject != null)
					CubeBlockInternalWrapper.GetInstance().UpdateTerminalBlockCustomName(BackingObject, value);
			}
		}

		#endregion
	}
}
