using Havok;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Voxels;
using Sandbox.Game.Weapons;

using SEModAPI.API.SaveData;
using SEModAPI.API.SaveData.Entity;

using VRage;
using VRageMath;

namespace SEModAPI.API.Internal
{
	public class CharacterInternalWrapper : BaseInternalWrapper
	{
		#region "Attributes"

		protected new static CharacterInternalWrapper m_instance;

		#endregion

		#region "Constructors and Initializers"

		protected CharacterInternalWrapper(string basePath)
			: base(basePath)
		{
			m_instance = this;
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
			float health = (float)InvokeEntityMethod(character.BackingObject, "7047AFF5D44FC8A44572E92DBAD13011", new object[] { });

			return health;
		}

		public bool DamageCharacter(CharacterEntity character, float damage)
		{
			try
			{
				InvokeEntityMethod(character.BackingObject, "CF6EEF37B5AE4047E65CA4A0BB43F774", new object[] { damage, MyDamageType.Unknown, true });

				return true;
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				return false;
			}
		}

		public bool UpdateCharacterHealth(CharacterEntity character, float health)
		{
			try
			{
				InvokeEntityMethod(character.BackingObject, "92A0500FD8772AB1AC3A6F79FD2A1C72", new object[] { health });

				return true;
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				return false;
			}
		}

		public Object GetCharacterBattery(CharacterEntity character)
		{
			Object battery = InvokeEntityMethod(character.BackingObject, "CF72A89940254CB8F535F177150FC743", new object[] { });

			return battery;
		}

		public bool UpdateCharacterBatteryLevel(CharacterEntity character, float capacity)
		{
			try
			{
				Object battery = GetCharacterBattery(character);
				InvokeEntityMethod(battery, "C3BF60F3540A8A48CB8FEE0CDD3A95C6", new object[] { capacity });

				return true;
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				return false;
			}
		}

		#endregion
	}
}
