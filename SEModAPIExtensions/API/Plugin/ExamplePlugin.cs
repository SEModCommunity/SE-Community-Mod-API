using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEModAPIExtensions.API.Plugin.Events;

using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Entity.Sector.SectorObject;

using VRageMath;

namespace SEModAPIExtensions.API.Plugin
{
	public class ExamplePlugin : PluginBase, IPlayerEventHandler, ICubeGridHandler, IBaseEntityHandler
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
			//Console.WriteLine("ExamplePlugin - Plugin '" + Id.ToString() + "' updated!");
		}

		public void OnPlayerJoined(ulong remoteUserId, CharacterEntity character)
		{
			Console.WriteLine("ExamplePlugin - Player '" + remoteUserId.ToString() + "' joined with character '" + character.EntityId.ToString() + "'");
		}

		public void OnPlayerLeft(ulong remoteUserId, CharacterEntity character)
		{
			Console.WriteLine("ExamplePlugin - Player '" + remoteUserId.ToString() + "' left with character '" + character.EntityId.ToString() + "'");
		}

		public void OnBaseEntityMoved(BaseEntity entity)
		{
			//Console.WriteLine("ExamplePlugin - BaseEntity '" + entity.Name + "' moved to '" + ((Vector3)entity.Position).ToString() + "'");
		}

		public void OnCubeGridMoved(CubeGridEntity cubeGrid)
		{
			//Console.WriteLine("ExamplePlugin - CubeGrid '" + cubeGrid.Name + "' moved to '" + ((Vector3)cubeGrid.Position).ToString() + "'");
		}

		public void OnCubeGridCreated(CubeGridEntity cubeGrid)
		{
			Console.WriteLine("ExamplePlugin - CubeGrid '" + cubeGrid.Name + "' created");
		}

		public void OnCubeGridDeleted(CubeGridEntity cubeGrid)
		{
			Console.WriteLine("ExamplePlugin - CubeGrid '" + cubeGrid.Name + "' deleted");
		}

		#endregion
	}
}
