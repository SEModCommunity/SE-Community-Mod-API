using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEModAPIInternal.API.Entity.Sector.SectorObject;

namespace SEModAPIExtensions.API.Plugin.Events
{
	public interface IPlayerEventHandler
	{
		void OnPlayerJoined(ulong remoteUserId, CharacterEntity character);
		void OnPlayerLeft(ulong remoteUserId, CharacterEntity character);
	}
}
