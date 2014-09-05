using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEModAPIExtensions.API.Plugin.Events
{
	public interface ISectorEventHandler
	{
		void OnSectorSaved(object unusedArg);
	}
}
