using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEModAPIInternal.API.Entity.Sector.SectorObject;

namespace SEModAPIExtensions.API.Plugin.Events
{
	public interface ICharacterEventHandler
	{
		void OnCharacterCreated(CharacterEntity entity);
		void OnCharacterDeleted(CharacterEntity entity);
		void OnCharacterMoved(CharacterEntity cubeGrid);
	}
}
