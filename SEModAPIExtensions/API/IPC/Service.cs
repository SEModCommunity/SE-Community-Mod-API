using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.Support;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;

namespace SEModAPIExtensions.API.IPC
{
	[ServiceBehavior(
		ConcurrencyMode=ConcurrencyMode.Single,
		InstanceContextMode=InstanceContextMode.PerSession,
		IncludeExceptionDetailInFaults=true,
		IgnoreExtensionDataObject=true
	)]
	public class InternalService : IInternalServiceContract
	{
		public static string BaseURI = "http://localhost:8000/SEServerExtender/";

		public List<ulong> GetConnectedPlayers()
		{
			return PlayerManager.Instance.ConnectedPlayers;
		}

		public string GetPlayerName(ulong steamId)
		{
			return PlayerMap.Instance.GetPlayerNameFromSteamId(steamId);
		}

		public List<BaseEntity> GetSectorEntities()
		{
			//LogManager.APILog.WriteLineAndConsole("WCF Service requested all entities!");

			return SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>();
		}

		public List<CubeGridEntity> GetSectorCubeGridEntities()
		{
			//LogManager.APILog.WriteLineAndConsole("WCF Service requested cubegrid entities!");

			return SectorObjectManager.Instance.GetTypedInternalData<CubeGridEntity>();
		}

		public List<CubeBlockEntity> GetCubeBlocks(long cubeGridEntityId)
		{
			//LogManager.APILog.WriteLineAndConsole("WCF Service requested cubeblock entities!");

			List<CubeBlockEntity> cubeBlocks = new List<CubeBlockEntity>();
			foreach (CubeGridEntity cubeGrid in GetSectorCubeGridEntities())
			{
				if (cubeGrid.EntityId == cubeGridEntityId)
				{
					cubeBlocks = cubeGrid.CubeBlocks;
					break;
				}
			}

			return cubeBlocks;
		}

		public List<InventoryItemEntity> GetInventoryItems(long cubeGridEntityId, long containerBlockEntityId, ushort inventoryIndex = 0)
		{
			List<InventoryItemEntity> inventoryItems = new List<InventoryItemEntity>();
			foreach (CubeBlockEntity cubeBlock in GetCubeBlocks(cubeGridEntityId))
			{
				if (!(cubeBlock is TerminalBlockEntity))
					continue;

				if (cubeBlock.EntityId != containerBlockEntityId)
					continue;

				if (cubeBlock is CargoContainerEntity)
				{
					inventoryItems = ((CargoContainerEntity)cubeBlock).Inventory.Items;
					break;
				}

				if (cubeBlock is ReactorEntity)
				{
					inventoryItems = ((ReactorEntity)cubeBlock).Inventory.Items;
					break;
				}

				if (cubeBlock is ProductionBlockEntity)
				{
					if (inventoryIndex == 0)
						inventoryItems = ((ProductionBlockEntity)cubeBlock).InputInventory.Items;
					if (inventoryIndex == 1)
						inventoryItems = ((ProductionBlockEntity)cubeBlock).OutputInventory.Items;
					break;
				}

				if (cubeBlock is ShipDrillEntity)
				{
					inventoryItems = ((ShipDrillEntity)cubeBlock).Inventory.Items;
					break;
				}

				if (cubeBlock is ShipToolBaseEntity)
				{
					inventoryItems = ((ShipToolBaseEntity)cubeBlock).Inventory.Items;
					break;
				}
			}

			return inventoryItems;
		}

		public void UpdateEntity(BaseEntity entity)
		{
			LogManager.APILog.WriteLineAndConsole("WCF Service - Received base entity '" + entity.Name + "' with id '" + entity.EntityId.ToString() + "'");

			foreach (BaseEntity baseEntity in GetSectorEntities())
			{
				if (baseEntity.EntityId == entity.EntityId)
				{
					//Copy over the deserialized dummy entity to the actual entity
					Type type = baseEntity.GetType();
					PropertyInfo[] properties = type.GetProperties();
					foreach (PropertyInfo property in properties)
					{
						try
						{
							if (!property.CanWrite)
								continue;
							if (property.GetSetMethod() == null)
								continue;

							object[] dataMemberAttributes = property.GetCustomAttributes(typeof(DataMemberAttribute), true);
							if (dataMemberAttributes == null || dataMemberAttributes.Length == 0)
								continue;

							Object newValue = property.GetValue(entity, new object[] { });
							if (newValue == null)
								continue;

							Object oldValue = property.GetValue(baseEntity, new object[] { });
							if (newValue.Equals(oldValue))
								continue;

							property.SetValue(baseEntity, newValue, new object[] { });
						}
						catch (Exception ex)
						{
							//Do nothing
						}
					}
					break;
				}
			}
		}

		public void UpdateCubeBlock(CubeBlockEntity cubeBlock)
		{
		}
	}
}
