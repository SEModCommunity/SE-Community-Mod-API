using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEModAPIExtensions.API.Plugin.Events;

using SEModAPIInternal.API.Entity.Sector.SectorObject;

namespace SEModAPIExtensions.API.Plugin
{
	public class ExamplePlugin : PluginBase, IPlayerEventHandler
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
			//Console.WriteLine("Plugin '" + Id.ToString() + "' updated!");
		}

		public void OnPlayerJoined(ulong remoteUserId, CharacterEntity character)
		{
			Console.WriteLine("Player '" + remoteUserId.ToString() + "' joined with character '" + character.EntityId.ToString() + "'");
		}

		public void OnPlayerLeft(ulong remoteUserId, CharacterEntity character)
		{
			Console.WriteLine("Player '" + remoteUserId.ToString() + "' left with character '" + character.EntityId.ToString() + "'");
		}

		#endregion
	}
}
