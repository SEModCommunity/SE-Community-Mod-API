using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEModAPIExtensions.API.Plugin.Events
{
	public interface IPlayerEventHandler
	{
		void OnPlayerJoined(ulong remoteUserId);
		void OnPlayerLeft(ulong remoteUserId);
	}
}
