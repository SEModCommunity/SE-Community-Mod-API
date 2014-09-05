using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEModAPIInternal.API.Entity;

namespace SEModAPIExtensions.API.Plugin.Events
{
	/// <summary>
	/// Events wrapper interface for Base Entity
	/// </summary>
	public interface IBaseEntityHandler
	{
		#region "Events"

		void OnBaseEntityMoved(BaseEntity entity);

		void OnBaseEntityCreated(BaseEntity entity);

		void OnBaseEntityDeleted(BaseEntity entity);

		#endregion
	}
}
