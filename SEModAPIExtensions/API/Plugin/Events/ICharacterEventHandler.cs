using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEModAPIInternal.API.Entity.Sector.SectorObject;

namespace SEModAPIExtensions.API.Plugin.Events
{
	/// <summary>
	/// Events wrapper interface for Character
	/// </summary>
	public interface ICharacterEventHandler
	{
		#region "Events"

		/// <summary>
		/// On Character created:
		///		When a player connect,
		///		When a player get out of a cockpit,
		///		When a player respawn.
		/// </summary>
		/// <param name="entity">The created Character</param>
		void OnCharacterCreated(CharacterEntity entity);

		/// <summary>
		/// On Character deleted:
		///		When a player disconnect,
		///		When a player enters a cockpit,
		///		When a player body dispawn some time after being killed.
		/// </summary>
		/// <param name="entity">The deleted Character</param>
		void OnCharacterDeleted(CharacterEntity entity);

		#endregion
	}
}
