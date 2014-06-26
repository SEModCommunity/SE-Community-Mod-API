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
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
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
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
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
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
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
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
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
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				return false;
			}
		}

		#endregion
	}
}
