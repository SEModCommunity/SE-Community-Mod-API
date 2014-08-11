using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEModAPIInternal.API.Common;
namespace SEModAPIExtensions.API
{
	public class PluginManagerThreadParams
	{
		public Object plugin;
		public Guid key;
		public List<EntityEventManager.EntityEvent> events;
		public List<ChatManager.ChatEvent> chatEvents;
	}
}
