using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEModAPIExtensions.API.Plugin.Events
{
	/// <summary>
	/// Events wrapper interface for Chat
	/// </summary>
	public interface IChatEventHandler
	{
		#region "Events"

		void OnChatReceived(ChatManager.ChatEvent chatEvent);

		void OnChatSent(ChatManager.ChatEvent chatEvent);

		#endregion
	}
}
