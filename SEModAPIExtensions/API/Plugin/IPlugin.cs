using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEModAPIExtensions.API.Plugin
{
	/// <summary>
	/// Standardized interface for plugins
	/// </summary>
	public interface IPlugin
	{
		#region "Methods"
		void Init();
		void Update();
		void Shutdown();
		#endregion

		#region "Properties"
		Guid Id
		{ get; }
		string Name
		{ get; }
		string Version
		{ get; }
		#endregion
	}
}
