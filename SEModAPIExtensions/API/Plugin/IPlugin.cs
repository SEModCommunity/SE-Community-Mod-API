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
		void AddCommandExecutor();
		Guid Id
		{ get; }
		string Name
		{ get; }
		string Version
		{ get; }
	}
}
