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

				if (BackingObject != null)
					GameObjectManagerWrapper.GetInstance().UpdateEntityVelocity(BackingObject, value);
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
			get
			{
				float originalValue = m_baseDefinition.Battery.CurrentCapacity;
				float percentageValue = (float)Math.Round(originalValue * 10000000, 2);
				return percentageValue;
			}
			set
			{
				float originalValue = m_baseDefinition.Battery.CurrentCapacity;
				float percentageValue = (float)Math.Round(originalValue * 10000000, 2);
				if (percentageValue == value) return;
				m_baseDefinition.Battery.CurrentCapacity = value / 10000000;
				Changed = true;

				if (BackingObject != null)
					CharacterInternalWrapper.GetInstance().UpdateCharacterBatteryLevel(this, m_baseDefinition.Battery.CurrentCapacity);
			}
		}

		[Category("Character")]
		public float Health
		{
			get
			{
				float health = m_baseDefinition.Health.GetValueOrDefault(-1);
				if (BackingObject != null)
					if (health <= 0)
						health = CharacterInternalWrapper.GetInstance().GetCharacterHealth(this);
				return health;
			}
			set
			{
				if (Health == value) return;

				if (BackingObject != null)
					CharacterInternalWrapper.GetInstance().DamageCharacter(this, Health - value);

				m_baseDefinition.Health = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_Character definition)
		{
			//TODO - Find a way to get the player's steam name
			return EntityId.ToString();
		}

		#endregion
	}
}
