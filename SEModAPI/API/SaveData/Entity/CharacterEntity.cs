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
	public class CharacterEntity : SectorObject<MyObjectBuilder_Character>
	{
		#region "Constructors and Initializers"

		public CharacterEntity(MyObjectBuilder_Character definition)
			: base(definition)
		{
			EntityId = definition.EntityId;
		}

		#endregion

		#region "Properties"

		[Category("Character")]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public VRageMath.Vector3 LinearVelocity
		{
			get { return m_baseDefinition.LinearVelocity; }
			set
			{
				if (m_baseDefinition.LinearVelocity == value) return;
				m_baseDefinition.LinearVelocity = value;
				Changed = true;
			}
		}

		[Category("Character")]
		[Browsable(false)]
		public MyObjectBuilder_Battery Battery
		{
			get { return m_baseDefinition.Battery; }
			set
			{
				if (m_baseDefinition.Battery == value) return;
				m_baseDefinition.Battery = value;
				Changed = true;
			}
		}

		[Category("Character")]
		public float BatteryLevel
		{
			get { return m_baseDefinition.Battery.CurrentCapacity; }
			set
			{
				if (m_baseDefinition.Battery.CurrentCapacity == value) return;
				m_baseDefinition.Battery.CurrentCapacity = value;
				Changed = true;
			}
		}

		[Category("Character")]
		public float Health
		{
			get { return m_baseDefinition.Health.GetValueOrDefault(); }
			set
			{
				if (m_baseDefinition.Health == value) return;
				m_baseDefinition.Health = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_Character definition)
		{
			return m_baseDefinition.Name;
		}

		#endregion
	}
}
