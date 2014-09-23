using System;
using System.Collections.Generic;
using System.IO;
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
	[ServiceContract]
	public interface IWebServiceContract
	{
		[OperationContract]
		[WebInvoke(Method = "OPTIONS", UriTemplate = "")]
		void GetOptions();

		[OperationContract]
		[WebGet]
		List<BaseEntity> GetSectorEntities();

		[OperationContract]
		[WebGet]
		List<CubeGridEntity> GetSectorCubeGridEntities();

		[OperationContract]
		[WebGet(UriTemplate = "GetCubeBlocks/{cubeGridEntityId}")]
		List<CubeBlockEntity> GetCubeBlocks(string cubeGridEntityId);
	}

	[ServiceContract]
	public interface IInternalServiceContract
	{
		[OperationContract]
		List<ulong> GetConnectedPlayers();

		[OperationContract]
		string GetPlayerName(ulong steamId);

		[OperationContract]
		List<BaseEntity> GetSectorEntities();

		[OperationContract]
		List<CubeGridEntity> GetSectorCubeGridEntities();

		[OperationContract]
		List<CubeBlockEntity> GetCubeBlocks(long cubeGridEntityId);

		[OperationContract]
		List<InventoryItemEntity> GetInventoryItems(long cubeGridEntityId, long containerBlockEntityId, ushort inventoryIndex);

		[OperationContract]
		void UpdateEntity(BaseEntity entity);

		[OperationContract]
		void UpdateCubeBlock(CubeGridEntity parent, CubeBlockEntity cubeBlock);
	}

	[ServiceContract]
	public interface IPolicyRetriever
	{
		[OperationContract, WebGet(UriTemplate = "/clientaccesspolicy.xml")]
		Stream GetSilverlightPolicy();
		[OperationContract, WebGet(UriTemplate = "/crossdomain.xml")]
		Stream GetFlashPolicy();
	}
}
