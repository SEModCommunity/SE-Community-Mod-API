using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEModAPIExtensions.API.Plugin.Events
{
	/// <summary>
	/// Events wrapper interface for Players
	/// </summary>
	public interface IPlayerEventHandler
	{
		#region "Events"

		void OnPlayerJoined(ulong remoteUserId);

		void OnPlayerLeft(ulong remoteUserId);

		#endregion
	}
}
