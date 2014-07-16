using System;

namespace SEModAPIExtensions.API
{
	public class APIExtensionsUtils
	{
		#region "Attributes"
		
		private PluginManager m_pluginmanager;
		private ChatManager m_chatmanager;
		private CommandManager m_commandmanager;
		
		#endregion
		
		#region "Constructors and Intializers"
		
		protected APIExtensionUtils()
		{
			m_pluginmanager = PluginManager.Instance;
			m_chatmanager = ChatManager.Instance;
			m_commandmanager = CommandManager.Instance;
		}
		
		#endregion
		
		#region "Properties"
		
		public PluginManager PluginManager
		{
			get
			{
				return m_pluginmanager;
			}
		}
		
		public ChatManager ChatManager
		{
			get
			{
				return m_chatmanager;
			}
		}
		
		public CommandManager CommandManager
		{
			get
			{
				return m_chatmanager;
			}
		}
		
		#endregion
		
		#region "Methods"
		
		public CheckFileTypes(Type lookfor, Dictionary<Guid, Object> check)
		{
			MyFSPath fsPath = new MyFSPath(MyFSLocationEnum.Mod, "");
			string modsPath = MyFileSystem.GetAbsolutePath(fsPath);
			if (!Directory.Exists(modsPath))
				return;

			string[] subDirectories = Directory.GetDirectories(modsPath);
			foreach (string path in subDirectories)
			{
				string[] files = Directory.GetFiles(path);
				foreach (string file in files)
				{
					try
					{
						FileInfo fileInfo = new FileInfo(file);
						if (!fileInfo.Extension.ToLower().Equals(".dll"))
							continue;

						//Load the assembly
						Assembly objAssembly = Assembly.UnsafeLoadFrom(file);

						//Get the assembly GUID
						GuidAttribute guid = (GuidAttribute)pluginAssembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
						Guid guidValue = new Guid(guid.Value);

						//Look through the exported types to find the one that implements PluginBase
						Type[] types = objnAssembly.GetExportedTypes();
						foreach (Type type in types)
						{
							//Check that we don't have an entry already for this GUID
							if (check.ContainsKey(guidValue))
								break;

							if (type.BaseType == null)
								continue;

							Type[] filteredTypes = type.BaseType.GetInterfaces();
							foreach (Type interfaceType in filteredTypes)
							{
								if (interfaceType.FullName == typeof(lookfor).FullName)
								{
									return lookfor;
								}
							}
						}

						break;
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.ToString());
					}
				}
			}
		}
		
		#endregion
	}
}
