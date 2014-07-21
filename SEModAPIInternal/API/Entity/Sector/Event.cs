using System;
using System.Collections.Generic;
using System.ComponentModel;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;

using SEModAPI.API.Definitions;

namespace SEModAPIInternal.API.Entity.Sector
{
	public class Event : BaseObject
	{
		#region "Constructors and Initializers"

		public Event(MyObjectBuilder_GlobalEventBase definition)
			: base(definition)
		{}

		#endregion

		#region "Properties"

		[Category("Event")]
		[Browsable(true)]
		[ReadOnly(true)]
		public override string Name
		{
			get
			{
				string name = ObjectBuilder.EventType.ToString();
				return name;
			}
		}

		[Category("Event")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_GlobalEventBase ObjectBuilder
		{
			get
			{
				return (MyObjectBuilder_GlobalEventBase)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[Category("Event")]
		[Browsable(true)]
		[ReadOnly(true)]
		public long ActivationTimeMs
		{
			get { return ObjectBuilder.ActivationTimeMs; }
			set
			{
				if (ObjectBuilder.ActivationTimeMs == value) return;
				ObjectBuilder.ActivationTimeMs = value;
				Changed = true;
			}
		}

		[Category("Event")]
		[Browsable(true)]
		[ReadOnly(true)]
		public SerializableDefinitionId DefinitionId
		{
			get { return ObjectBuilder.DefinitionId; }
			set
			{
				if (ObjectBuilder.DefinitionId.Equals(value)) return;
				ObjectBuilder.DefinitionId = value;
				Changed = true;
			}
		}

		[Category("Event")]
		[Browsable(true)]
		[ReadOnly(true)]
		public bool Enabled
		{
			get { return ObjectBuilder.Enabled; }
			set
			{
				if (ObjectBuilder.Enabled == value) return;
				ObjectBuilder.Enabled = value;
				Changed = true;
			}
		}

		[Category("Event")]
		[Browsable(true)]
		[ReadOnly(true)]
		public MyGlobalEventTypeEnum EventType
		{
			get { return ObjectBuilder.EventType; }
			set
			{
				if (ObjectBuilder.EventType == value) return;
				ObjectBuilder.EventType = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		#endregion
	}
}
