using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
using System.ServiceModel.Web;
using System.Security.Permissions;

namespace SEModAPIExtensions.API.IPC
{
	/// <summary>
	/// Interface defining the contract for the differents http web requests as "Routes". (url for the http requests are the name of the functions)
	/// </summary>
	[ServiceContract]
	public interface IWebServiceContract
	{
		#region "Routes"

		[OperationContract]
		[WebInvoke(Method = "OPTIONS", UriTemplate = "")]
		void GetOptions();

		/// <summary>
		/// Route to get the Sector Entities
		/// </summary>
		/// <returns>All the entities in the sector</returns>
		[OperationContract]
		[WebGet]
		List<BaseEntity> GetSectorEntities();

		/// <summary>
		/// Route to get the Sector CubeGrids Entities
		/// </summary>
		/// <returns>All the Cube Grid entities</returns>
		[OperationContract]
		[WebGet]
		List<CubeGridEntity> GetSectorCubeGridEntities();

		/// <summary>
		/// Route to get all the Blocks on a specific CubeGrid
		/// </summary>
		/// <param name="cubeGridEntityId"></param>
		/// <returns>All the cube blocks on the specified CubeGrid</returns>
		[OperationContract]
		[WebGet(UriTemplate = "GetCubeBlocks/{cubeGridEntityId}")]
		List<CubeBlockEntity> GetCubeBlocks(string cubeGridEntityId);

		#endregion
	}

	/// <summary>
	/// Interface defining the contract for the differents internal http requests as "Routes". (url for the http requests are the name of the functions)
	/// </summary>
	[ServiceContract]
	public interface IInternalServiceContract
	{
		#region "Routes"

		/// <summary>
		/// Route to get all the Players connected to the server
		/// </summary>
		/// <returns>Al the connected players</returns>
		[OperationContract]
		List<ulong> GetConnectedPlayers();

		/// <summary>
		/// Route to get the PlayerName defined by the given steamId
		/// </summary>
		/// <param name="steamId">a steamId</param>
		/// <returns>The specified player name</returns>
		[OperationContract]
		string GetPlayerName(ulong steamId);


		/// <summary>
		/// Route to get the Sector Entities
		/// </summary>
		/// <returns>All the entities in the sector</returns>
		[OperationContract]
		List<BaseEntity> GetSectorEntities();

		/// <summary>
		/// Route to get the Sector CubeGrids Entities
		/// </summary>
		/// <returns>All the Cube Grid entities</returns>
		[OperationContract]
		List<CubeGridEntity> GetSectorCubeGridEntities();

		/// <summary>
		/// Route to get all the Blocks on a specific CubeGrid
		/// </summary>
		/// <param name="cubeGridEntityId"></param>
		/// <returns>All the cube blocks on the specified CubeGrid</returns>
		[OperationContract]
		List<CubeBlockEntity> GetCubeBlocks(long cubeGridEntityId);

		/// <summary>
		/// Route to get all the items inside the specified container
		/// </summary>
		/// <param name="cubeGridEntityId"></param>
		/// <returns>All the items inside the specified container</returns>
		[OperationContract]
		List<InventoryItemEntity> GetInventoryItems(long cubeGridEntityId, long containerBlockEntityId, ushort inventoryIndex);

		/// <summary>
		/// Route to update an entity with the entity in parameter
		/// </summary>
		/// <param name="entity">The entity to update with</param>
		[OperationContract]
		void UpdateEntity(BaseEntity entity);

		/// <summary>
		/// Route to update a CubeBlock on a CubeGrid
		/// </summary>
		/// <param name="parent">The entity where this CubeBlock is</param>
		/// <param name="cubeBlock">The CubeBlock to update with</param>
		[OperationContract]
		void UpdateCubeBlock(CubeGridEntity parent, CubeBlockEntity cubeBlock);

		#endregion
	}
}
