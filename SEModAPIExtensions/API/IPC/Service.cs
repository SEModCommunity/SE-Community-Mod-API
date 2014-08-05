using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.Support;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;

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
			LogManager.APILog.WriteLineAndConsole("WCF Service requested cubeblock entities!");

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
	}
}
