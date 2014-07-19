using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEModAPIExtensions.API.Plugin
{
	public interface IPlugin
	{
		void Init();
		void Update();
		void Shutdown();
		Guid Id
		{ get; }
		string Name
		{ get; }
		string Version
		{ get; }
        CommandBase CommandExecutor
        { get; }
	}
}
