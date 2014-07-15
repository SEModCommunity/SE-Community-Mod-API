using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;

using SEModAPIExtensions.API.Plugin;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;
using SEModAPIInternal.Support;

namespace SEModAPIExtensions.API
{
	public class CommandManager
	{
		#region "Attributes"
		
		private static CommandManager m_instance;
		
		private List<CommandBase> m_commands;
		private bool m_initialized;
		
		#endregion
		
		#region "Constructors and Initializers"
		
		protected CommandManager()
		{
			m_instance = this;
			
			m_commands = new List<CommandBase>();
			m_initialized = false;
			
			Console.WriteLine("Finished loading CommandManager");
		}
		
		#endregion
		
		#region "Properties"
		
		public static CommandManager Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new CommandManager();

				return m_instance;
			}
		}

		public bool Initialized
		{
			get { return m_initialized; }
		}
		
		public List<CommandBase> Commands
		{
			get { return m_commands; }
		}
		
		#endregion
		
		#region "Methods"
		
		
		
		#endregion
	}
}
