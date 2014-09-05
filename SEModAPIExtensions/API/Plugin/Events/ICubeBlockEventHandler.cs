using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;

namespace SEModAPIExtensions.API.Plugin.Events
{
	/// <summary>
	/// Events wrapper interface for CubeBlocks
	/// </summary>
	public interface ICubeBlockEventHandler
	{
		#region "Events"

		void OnCubeBlockCreated(CubeBlockEntity entity);

		void OnCubeBlockDeleted(CubeBlockEntity entity);

		#endregion
	}
}
