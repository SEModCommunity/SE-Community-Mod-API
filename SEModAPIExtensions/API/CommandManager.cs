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
		
		private Dictionary<Guid, CommandBase> m_commands;
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
		
		public Dictionary<Guid, CommandBase> Commands
		{
			get { return m_commands; }
		}
		
		#endregion
		
		#region "Methods"
		
		public void LoadCommand(string instancename = "")
		{
			Console.WriteLine("Loading commands ...");

			try
			{
				//m_commands.Clear();
				m_initialized = false;

				SandboxGameAssemblyWrapper.Instance.InitMyFileSystem(instanceName);
				List<Type> types = m_utils.CheckFileTypes(ICommand, m_commands)
				foreach(Type type in types)
				{
					try
					{
						//Create an instance of the plugin object
						var commandObject = Activator.CreateInstance(type);

						//And add it to the dictionary
						m_commands.Add(guidValue, pluginObject);
						break;
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.ToString());
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}

			Console.WriteLine("Finished loading commands");
		}
		
		#endregion
	}
}
