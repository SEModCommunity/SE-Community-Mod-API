using System;
using System.Collections.Generic;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;

using SEModAPI.API.Definitions;

namespace SEModAPI.API.SaveData
{
	public class Event : OverLayerDefinition<MyObjectBuilder_GlobalEventBase>
	{
		#region "Constructors and Initializers"

		public Event(MyObjectBuilder_GlobalEventBase definition)
			: base(definition)
		{}

		#endregion

		#region "Properties"

		new public string Name
		{
			get { return this.GetNameFrom(m_baseDefinition); }
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_GlobalEventBase definition)
		{
			return m_baseDefinition.DefinitionId.SubtypeId;
		}

		#endregion
	}
}
