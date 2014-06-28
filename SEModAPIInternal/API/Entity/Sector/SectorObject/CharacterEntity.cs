using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;
using Sandbox.Game.Weapons;

using SEModAPI.API;

using SEModAPIInternal.API.Server;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
	public class CharacterEntity : BaseEntity
	{
		#region "Constructors and Initializers"

		public CharacterEntity(MyObjectBuilder_Character definition)
			: base(definition)
		{
		}

		#endregion

		#region "Properties"

		[Category("Character")]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public VRageMath.Vector3 LinearVelocity
		{
			get { return GetSubTypeEntity().LinearVelocity; }
			set
			{
				if (GetSubTypeEntity().LinearVelocity == value) return;
				GetSubTypeEntity().LinearVelocity = value;
				Changed = true;

				if (BackingObject != null)
					BaseEntityManagerWrapper.GetInstance().UpdateEntityVelocity(BackingObject, value);
			}
		}

		[Category("Character")]
		[Browsable(false)]
		public MyObjectBuilder_Battery Battery
		{
			get { return GetSubTypeEntity().Battery; }
			set
			{
				if (GetSubTypeEntity().Battery == value) return;
				GetSubTypeEntity().Battery = value;
				Changed = true;
			}
		}

		[Category("Character")]
		public float BatteryLevel
		{
			get
			{
				float originalValue = GetSubTypeEntity().Battery.CurrentCapacity;
				float percentageValue = (float)Math.Round(originalValue * 10000000, 2);
				return percentageValue;
			}
			set
			{
				float originalValue = GetSubTypeEntity().Battery.CurrentCapacity;
				float percentageValue = (float)Math.Round(originalValue * 10000000, 2);
				if (percentageValue == value) return;
				GetSubTypeEntity().Battery.CurrentCapacity = value / 10000000;
				Changed = true;

				if (BackingObject != null)
					CharacterInternalWrapper.GetInstance().UpdateCharacterBatteryLevel(this, GetSubTypeEntity().Battery.CurrentCapacity);
			}
		}

		[Category("Character")]
		public float Health
		{
			get
			{
				float health = GetSubTypeEntity().Health.GetValueOrDefault(-1);
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

				GetSubTypeEntity().Health = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_Character GetSubTypeEntity()
		{
			return (MyObjectBuilder_Character)BaseEntity;
		}

		#endregion
	}

	public class CharacterInternalWrapper : BaseInternalWrapper
	{
		#region "Attributes"

		protected new static CharacterInternalWrapper m_instance;

		public static string CharacterGetHealthMethod = "7047AFF5D44FC8A44572E92DBAD13011";
		public static string CharacterDamageCharacterMethod = "CF6EEF37B5AE4047E65CA4A0BB43F774";
		public static string CharacterSetHealthMethod = "92A0500FD8772AB1AC3A6F79FD2A1C72";
		public static string CharacterGetBatteryCapacityMethod = "CF72A89940254CB8F535F177150FC743";
		public static string CharacterSetBatteryCapacityMethod = "C3BF60F3540A8A48CB8FEE0CDD3A95C6";

		#endregion

		#region "Constructors and Initializers"

		protected CharacterInternalWrapper(string basePath)
			: base(basePath)
		{
			m_instance = this;

			Console.WriteLine("Finished loading CharacterInternalWrapper");
		}

		new public static CharacterInternalWrapper GetInstance(string basePath = "")
		{
			if (m_instance == null)
			{
				m_instance = new CharacterInternalWrapper(basePath);
			}
			return (CharacterInternalWrapper)m_instance;
		}

		#endregion

		#region "Properties"

		new public static bool IsDebugging
		{
			get
			{
				CharacterInternalWrapper.GetInstance();
				return m_isDebugging;
			}
			set
			{
				CharacterInternalWrapper.GetInstance();
				m_isDebugging = value;
			}
		}

		#endregion

		#region "Methods"

		public float GetCharacterHealth(CharacterEntity character)
		{
			try
			{
				float health = (float)InvokeEntityMethod(character.BackingObject, CharacterGetHealthMethod, new object[] { });

				return health;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex.ToString());
				return -1;
			}
		}

		public bool DamageCharacter(CharacterEntity character, float damage)
		{
			try
			{
				InvokeEntityMethod(character.BackingObject, CharacterDamageCharacterMethod, new object[] { damage, MyDamageType.Unknown, true });

				return true;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex.ToString());
				return false;
			}
		}

		public bool UpdateCharacterHealth(CharacterEntity character, float health)
		{
			try
			{
				InvokeEntityMethod(character.BackingObject, CharacterSetHealthMethod, new object[] { health });

				return true;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex.ToString());
				return false;
			}
		}

		public Object GetCharacterBattery(CharacterEntity character)
		{
			try
			{
				Object battery = InvokeEntityMethod(character.BackingObject, CharacterGetBatteryCapacityMethod, new object[] { });

				return battery;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex.ToString());
				return -1;
			}
		}

		public bool UpdateCharacterBatteryLevel(CharacterEntity character, float capacity)
		{
			try
			{
				Object battery = GetCharacterBattery(character);
				InvokeEntityMethod(battery, CharacterSetBatteryCapacityMethod, new object[] { capacity });

				return true;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex.ToString());
				return false;
			}
		}

		#endregion
	}
}
