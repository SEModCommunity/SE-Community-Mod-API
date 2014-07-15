using System;
using System.Text;

using SEModAPIExtensions.API.Plugin;

using SEModAPIInternal.API.Common;

namespace SEModAPIExtensions.API
{
	public class PluginCommand
	{
		#region "Attributes"
		
		private static PluginCommand m_instance;
		
		private PluginBase m_owner;
		private string m_cmd;
		private List<string> m_args;
		
		#endregion
		
	}
}
