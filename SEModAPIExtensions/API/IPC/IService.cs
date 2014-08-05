using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using SEModAPIInternal.API.Entity;

namespace SEModAPIExtensions.API.IPC
{
	[ServiceContract]
	public interface IInternalServiceContract
	{
		[OperationContract]
		List<ulong> GetConnectedPlayers();

		[OperationContract]
		List<BaseEntity> GetSectorEntities();
	}
}
