using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace SEModAPIExtensions.API.Plugin
{
	public abstract class PluginBase : IPlugin
	{
		#region "Attributes"

		protected Guid m_pluginId;

		#endregion

		#region "Constructors and Initializers"

		public PluginBase()
		{
			GuidAttribute attr = (GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), true)[0];
			m_pluginId = new Guid(attr.Value);
		}

		#endregion

		#region "Properties"

		public Guid Id
		{
			get { return m_pluginId; }
		}

		#endregion

		#region "Methods"

		public abstract void Init();
		public abstract void Update();

		#endregion
	}
}
