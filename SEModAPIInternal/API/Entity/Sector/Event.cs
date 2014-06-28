using System;
using System.Collections.Generic;

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

		public override string Name
		{
			get
			{
				string name = GetSubTypeEntity().EventType.ToString();
				return name;
			}
		}

		public long ActivationTimeMs
		{
			get { return GetSubTypeEntity().ActivationTimeMs; }
			set
			{
				if (GetSubTypeEntity().ActivationTimeMs == value) return;
				GetSubTypeEntity().ActivationTimeMs = value;
				Changed = true;
			}
		}

		public SerializableDefinitionId DefinitionId
		{
			get { return GetSubTypeEntity().DefinitionId; }
			set
			{
				if (GetSubTypeEntity().DefinitionId.Equals(value)) return;
				GetSubTypeEntity().DefinitionId = value;
				Changed = true;
			}
		}

		public bool Enabled
		{
			get { return GetSubTypeEntity().Enabled; }
			set
			{
				if (GetSubTypeEntity().Enabled == value) return;
				GetSubTypeEntity().Enabled = value;
				Changed = true;
			}
		}

		public MyGlobalEventTypeEnum EventType
		{
			get { return GetSubTypeEntity().EventType; }
			set
			{
				if (GetSubTypeEntity().EventType == value) return;
				GetSubTypeEntity().EventType = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		internal MyObjectBuilder_GlobalEventBase GetSubTypeEntity()
		{
			return (MyObjectBuilder_GlobalEventBase)BaseEntity;
		}

		#endregion
	}
}
