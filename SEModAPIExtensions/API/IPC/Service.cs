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
using VRageMath;
using System.ServiceModel.Web;
using System.Security.Permissions;

namespace SEModAPIExtensions.API.IPC
{
	public class WebService : IWebServiceContract
	{
		public void GetOptions()
		{
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Origin", "*");
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Request-Method", "POST,GET,PUT,DELETE,OPTIONS");
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Headers", "X-Requested-With,Content-Type");
		}

		public List<BaseEntity> GetSectorEntities()
		{
			return SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>();
		}

		public List<CubeGridEntity> GetSectorCubeGridEntities()
		{
			return SectorObjectManager.Instance.GetTypedInternalData<CubeGridEntity>();
		}

		public List<CubeBlockEntity> GetCubeBlocks(string cubeGridEntityId)
		{
			long entityId = 0;
			try
			{
				entityId = long.Parse(cubeGridEntityId);
			}
			catch (Exception)
			{
			}
			List<CubeBlockEntity> cubeBlocks = new List<CubeBlockEntity>();
			List<CubeGridEntity> cubeGrids = SectorObjectManager.Instance.GetTypedInternalData<CubeGridEntity>();
			foreach (CubeGridEntity cubeGrid in cubeGrids)
			{
				if (cubeGrid.EntityId == entityId)
				{
					cubeBlocks = cubeGrid.CubeBlocks;
					break;
				}
			}

			return cubeBlocks;
		}
	}

	[ServiceBehavior(
		ConcurrencyMode=ConcurrencyMode.Single,
		InstanceContextMode=InstanceContextMode.PerSession,
		IncludeExceptionDetailInFaults=true,
		IgnoreExtensionDataObject=true
	)]
	public class InternalService : IInternalServiceContract
	{
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
			return SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>();
		}

		public List<CubeGridEntity> GetSectorCubeGridEntities()
		{
			return SectorObjectManager.Instance.GetTypedInternalData<CubeGridEntity>();
		}

		public List<CubeBlockEntity> GetCubeBlocks(long cubeGridEntityId)
		{
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
						catch (Exception)
						{
							//Do nothing
						}
					}
					break;
				}
			}
		}

		public void UpdateCubeBlock(CubeGridEntity parent, CubeBlockEntity cubeBlock)
		{
			LogManager.APILog.WriteLineAndConsole("WCF Service - Received cube block entity '" + cubeBlock.Name + "' with id '" + cubeBlock.EntityId.ToString() + "'");

			foreach (CubeGridEntity cubeGrid in GetSectorCubeGridEntities())
			{
				if (cubeGrid.EntityId == parent.EntityId)
				{
					//Find the matching block by either the entity id or the position of the block in the grid
					foreach (CubeBlockEntity baseBlock in cubeGrid.CubeBlocks)
					{
						//If entity id is defined but there is a mismatch, skip this block
						if (baseBlock.EntityId != 0 && baseBlock.EntityId != cubeBlock.EntityId)
							continue;

						//If entity is is NOT defined but there is a position mismatch, skip this block
						if (baseBlock.EntityId == 0 && (Vector3I)baseBlock.Min != (Vector3I)cubeBlock.Min)
							continue;

						//Copy over the deserialized dummy cube block to the actual cube block
						Type type = baseBlock.GetType();
						PropertyInfo[] properties = type.GetProperties();
						foreach (PropertyInfo property in properties)
						{
							try
							{
								//Check if the property can be publicly set
								if (!property.CanWrite)
									continue;
								if (property.GetSetMethod() == null)
									continue;

								//Check if the property has the [DataMember] attribute
								object[] dataMemberAttributes = property.GetCustomAttributes(typeof(DataMemberAttribute), true);
								if (dataMemberAttributes == null || dataMemberAttributes.Length == 0)
									continue;

								Object newValue = property.GetValue(cubeBlock, new object[] { });
								if (newValue == null)
									continue;

								Object oldValue = property.GetValue(baseBlock, new object[] { });
								if (newValue.Equals(oldValue))
									continue;

								property.SetValue(baseBlock, newValue, new object[] { });
							}
							catch (Exception)
							{
								//Do nothing
							}
						}
						break;
					}
					break;
				}
			}
		}
	}
}
