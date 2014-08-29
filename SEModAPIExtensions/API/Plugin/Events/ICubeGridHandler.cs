using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEModAPIInternal.API.Entity.Sector.SectorObject;

namespace SEModAPIExtensions.API.Plugin.Events
{
	public interface ICubeGridHandler
	{
		void OnCubeGridCreated(CubeGridEntity cubeGrid);
		void OnCubeGridDeleted(CubeGridEntity cubeGrid);
		void OnCubeGridMoved(CubeGridEntity cubeGrid);
		void OnCubeGridLoaded(CubeGridEntity cubeGrid);
	}
}
