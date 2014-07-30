using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using Sandbox.Common.ObjectBuilders;

using SEModAPI.API.Definitions;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;
using SEModAPIInternal.API.Utility;
using SEModAPIInternal.Support;

using VRageMath;
using VRage.Common.Utils;

namespace SEModAPIExtensions.API
{
	public class CargoShipManager
	{
		private static CargoShipManager m_instance;

		protected CargoShipManager()
		{
			m_instance = this;

			Console.WriteLine("Finished loading CargoShipManager");
		}

		public static CargoShipManager Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new CargoShipManager();

				return m_instance;
			}
		}

		public void SpawnCargoShipGroup(ulong remoteUserId = 0)
		{
			SpawnCargoShipGroup(true, remoteUserId);
		}

		public void SpawnCargoShipGroup(bool spawnAtAsteroids = true, ulong remoteUserId = 0)
		{
			float worldSize = SandboxGameAssemblyWrapper.Instance.GetServerConfig().SessionSettings.WorldSizeKm * 1000.0f;
			float spawnSize = 0.25f * worldSize;
			float destinationSize = 0.02f * spawnSize;

			if (spawnAtAsteroids)
			{
				float farthestAsteroidDistance = 0;
				float nearestAsteroidDistance = 999999;
				foreach (VoxelMap voxelMap in SectorObjectManager.Instance.GetTypedInternalData<VoxelMap>())
				{
					Vector3 asteroidPositon = voxelMap.Position;
					if (asteroidPositon.Length() > farthestAsteroidDistance)
						farthestAsteroidDistance = asteroidPositon.Length();
					if (asteroidPositon.Length() < nearestAsteroidDistance)
						nearestAsteroidDistance = asteroidPositon.Length();
				}

				spawnSize = farthestAsteroidDistance * 2;
				destinationSize = nearestAsteroidDistance * 2;
			}

			Vector3 groupPosition = UtilityFunctions.GenerateRandomBorderPosition(new Vector3(-spawnSize, -spawnSize, -spawnSize), new Vector3(spawnSize, spawnSize, spawnSize));
			Vector3 destinationPosition = UtilityFunctions.GenerateRandomBorderPosition(new Vector3(-destinationSize, -destinationSize, -destinationSize), new Vector3(destinationSize, destinationSize, destinationSize));

			SpawnCargoShipGroup(groupPosition, destinationPosition, remoteUserId);
		}

		public void SpawnCargoShipGroup(Vector3 startPosition, Vector3 stopPosition, ulong remoteUserId = 0)
		{
			try
			{
				//Load the spawn groups
				SpawnGroupsDefinitionsManager spawnGroupsDefinitionsManager = new SpawnGroupsDefinitionsManager();
				FileInfo contentDataFile = new FileInfo(Path.Combine(MyFileSystem.ContentPath, "Data", "SpawnGroups.sbc"));
				spawnGroupsDefinitionsManager.Load(contentDataFile);

				//Calculate lowest and highest frequencies
				float lowestFrequency = 999999;
				float highestFrequency = 0;
				foreach (SpawnGroupDefinition entry in spawnGroupsDefinitionsManager.Definitions)
				{
					if (entry.Frequency < lowestFrequency)
						lowestFrequency = entry.Frequency;
					if (entry.Frequency > highestFrequency)
						highestFrequency = entry.Frequency;
				}
				if (lowestFrequency <= 0)
					lowestFrequency = 1;

				//Get a list of which groups *could* spawn
				Random random = new Random((int)DateTime.Now.ToBinary());
				double randomChance = random.NextDouble();
				randomChance = randomChance * (highestFrequency / lowestFrequency);
				List<SpawnGroupDefinition> possibleGroups = new List<SpawnGroupDefinition>();
				foreach (SpawnGroupDefinition entry in spawnGroupsDefinitionsManager.Definitions)
				{
					if (entry.Frequency >= randomChance)
					{
						possibleGroups.Add(entry);
					}
				}

				//Determine which group *will* spawn
				randomChance = random.NextDouble();
				int randomShipIndex = Math.Max(0, Math.Min((int)Math.Round(randomChance * possibleGroups.Count, 0), possibleGroups.Count-1));
				SpawnGroupDefinition randomSpawnGroup = possibleGroups[randomShipIndex];

				ChatManager.Instance.SendPrivateChatMessage(remoteUserId, "Spawning cargo group '" + randomSpawnGroup.Name + "' ...");

				//Spawn the ships in the group
				Matrix orientation = Matrix.CreateLookAt(startPosition, stopPosition, new Vector3(0, 1, 0));
				foreach (SpawnGroupPrefab entry in randomSpawnGroup.Prefabs)
				{
					FileInfo prefabFile = new FileInfo(Path.Combine(MyFileSystem.ContentPath, "Data", "Prefabs", entry.File + ".sbc"));
					if (!prefabFile.Exists)
						continue;

					//Create the ship
					CubeGridEntity cubeGrid = new CubeGridEntity(prefabFile);

					//Set the ship position and orientation
					Vector3 shipPosition = Vector3.Transform(entry.Position, orientation) + startPosition;
					orientation.Translation = shipPosition;
					MyPositionAndOrientation newPositionOrientation = new MyPositionAndOrientation(orientation);
					cubeGrid.PositionAndOrientation = newPositionOrientation;

					//Set the ship velocity
					//Speed is clamped between 1.0f and the max cube grid speed
					Vector3 travelVector = stopPosition - startPosition;
					travelVector.Normalize();
					Vector3 shipVelocity = travelVector * (float)Math.Min(cubeGrid.MaxLinearVelocity, Math.Max(1.0f, entry.Speed));
					cubeGrid.LinearVelocity = shipVelocity;

					cubeGrid.IsDampenersEnabled = false;

					foreach (CubeBlockEntity cubeBlock in cubeGrid.CubeBlocks)
					{
						//Set the beacon names
						if (cubeBlock is BeaconEntity)
						{
							BeaconEntity beacon = (BeaconEntity)cubeBlock;
							beacon.CustomName = entry.Name;
						}

						//Set the owner of every block with an entity id
						//TODO - Find out if setting to an arbitrary non-zero works for this
						if (cubeBlock.EntityId != 0)
						{
							cubeBlock.Owner = 42;
						}
					}

					//And add the ship to the world
					SectorObjectManager.Instance.AddEntity(cubeGrid);

					//Force a refresh of the cube grid
					List<CubeBlockEntity> cubeBlocks = cubeGrid.CubeBlocks;
				}

				ChatManager.Instance.SendPrivateChatMessage(remoteUserId, "Cargo group '" + randomSpawnGroup.BaseDefinition.DisplayName + "' spawned with " + randomSpawnGroup.Prefabs.Length.ToString() + " ships at " + startPosition.ToString());
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}
	}
}
