using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEModAPIExtensions.API.Plugin
{
	public interface ICommand
	{
		bool execute(string cmd, string[] args);
		Object Owner
		{ get; }
		Object Executor
		{ get; }
		Permission
		{ get; }
	}
}
