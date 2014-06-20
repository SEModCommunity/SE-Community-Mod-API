using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

namespace SEModAPI.API.SaveData.Entity
{
	public class FunctionalBlockEntity<T> : TerminalBlockEntity<T> where T : MyObjectBuilder_FunctionalBlock
	{
		#region "Constructors and Initializers"

		public FunctionalBlockEntity(T definition)
			: base(definition)
		{
			EntityId = definition.EntityId;
		}

		#endregion

		#region "Properties"

		[Category("Functional Block")]
		public bool Enabled
		{
			get { return m_baseDefinition.Enabled; }
			set
			{
				if (m_baseDefinition.Enabled == value) return;
				m_baseDefinition.Enabled = value;
				Changed = true;
			}
		}

		#endregion
	}
}
