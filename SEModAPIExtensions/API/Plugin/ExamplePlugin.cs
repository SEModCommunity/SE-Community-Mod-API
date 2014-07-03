using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEModAPIExtensions.API.Plugin
{
	public class ExamplePlugin : PluginBase
	{
		#region "Constructors and Initializers"

		public ExamplePlugin()
		{
			Console.WriteLine("Plugin '" + Id.ToString() + "' constructed!");
		}

		public override void Init()
		{
			Console.WriteLine("Plugin '" + Id.ToString() + "' initialized!");
		}

		#endregion

		#region "Methods"

		public override void Update()
		{
			Console.WriteLine("Plugin '" + Id.ToString() + "' updated!");
		}

		#endregion
	}
}
