using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEModAPIInternal.API.Entity.Sector.SectorObject;

namespace SEModAPIExtensions.API.Plugin.Events
{
	/// <summary>
	/// Events wrapper interface for CubeGrid
	/// </summary>
	public interface ICubeGridHandler
	{
		#region "Events"

		void OnCubeGridCreated(CubeGridEntity cubeGrid);
		void OnCubeGridDeleted(CubeGridEntity cubeGrid);

		/// <summary>
		/// On CubeGrid moved,
		///		Every game tick where the given CubeGrid is moving
		/// </summary>
		/// <param name="cubeGrid">The moving CubeGrid</param>
		void OnCubeGridMoved(CubeGridEntity cubeGrid);
		void OnCubeGridLoaded(CubeGridEntity cubeGrid);
	}
}
