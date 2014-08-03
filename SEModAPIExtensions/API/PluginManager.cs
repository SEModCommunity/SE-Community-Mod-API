using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Threading;

using Sandbox.Common.ObjectBuilders;

using SEModAPIExtensions.API.Plugin;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;
using SEModAPIInternal.Support;

using VRage.Common.Utils;

namespace SEModAPIExtensions.API
{
	public class PluginManager
	{
		#region "Attributes"

		private static PluginManager m_instance;

		private Dictionary<Guid, Object> m_plugins;
		private Dictionary<Guid, bool> m_pluginState;
		private bool m_initialized;
		private DateTime m_lastUpdate;
		private TimeSpan m_lastUpdateTime;
		private double m_averageUpdateInterval;
		private double m_averageUpdateTime;
		private DateTime m_lastAverageOutput;
		private double m_averageEvents;
		private List<ulong> m_lastConnectedPlayerList;

		#endregion

		#region "Constructors and Initializers"

		protected PluginManager()
		{
			m_instance = this;

			m_plugins = new Dictionary<Guid, Object>();
			m_pluginState = new Dictionary<Guid, bool>();
			m_initialized = false;
			m_lastUpdate = DateTime.Now;
			m_lastUpdateTime = DateTime.Now - m_lastUpdate;
			m_averageUpdateInterval = 0;
			m_averageUpdateTime = 0;
			m_lastAverageOutput = DateTime.Now;
			m_averageEvents = 0;
			m_lastConnectedPlayerList = new List<ulong>();

			Console.WriteLine("Finished loading PluginManager");
		}

		#endregion

		#region "Properties"

		public static PluginManager Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new PluginManager();

				return m_instance;
			}
		}

		public bool Initialized
		{
			get { return m_initialized; }
		}

		public Dictionary<Guid, Object> Plugins
		{
			get { return m_plugins; }
		}

		#endregion

		#region "Methods"

		private bool CompareInterfaces(Type type, Object criteria)
		{
			if (type.ToString() == criteria.ToString())
				return true;
			else
				return false;
		}

		public void LoadPlugins(string instanceName = "")
		{
			Console.WriteLine("Loading plugins ...");

			try
			{
				//m_plugins.Clear();
				m_initialized = false;

				SandboxGameAssemblyWrapper.Instance.InitMyFileSystem(instanceName);

				string modsPath = MyFileSystem.ModsPath;
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
							Assembly pluginAssembly = Assembly.UnsafeLoadFrom(file);

							//Get the assembly GUID
							GuidAttribute guid = (GuidAttribute)pluginAssembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
							Guid guidValue = new Guid(guid.Value);

							//Look through the exported types to find the one that implements PluginBase
							Type[] types = pluginAssembly.GetExportedTypes();
							foreach (Type type in types)
							{
								//Check that we don't have an entry already for this GUID
								if (m_plugins.ContainsKey(guidValue))
									break;

								if (type.BaseType == null)
									continue;

								Type[] filteredTypes = type.BaseType.GetInterfaces();
								foreach (Type interfaceType in filteredTypes)
								{
									if (interfaceType.FullName == typeof(IPlugin).FullName)
									{
										try
										{
											//Create an instance of the plugin object
											var pluginObject = Activator.CreateInstance(type);

											//And add it to the dictionary
											m_plugins.Add(guidValue, pluginObject);
											break;
										}
										catch (Exception ex)
										{
											Console.WriteLine(ex.ToString());
										}
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
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}

			Console.WriteLine("Finished loading plugins");
		}

		public void Init()
		{
			Console.WriteLine("Initializing plugins ...");
			m_initialized = true;

			foreach (var key in m_plugins.Keys)
			{
				
				InitPlugin(key);

			}

			Console.WriteLine("Finished initializing plugins");

			
		}

		public void Update()
		{
			if (!Initialized)
				return;
			if (!SandboxGameAssemblyWrapper.Instance.IsGameStarted)
				return;

			m_lastUpdateTime = DateTime.Now - m_lastUpdate;
			m_averageUpdateInterval = (m_averageUpdateTime + m_lastUpdateTime.TotalMilliseconds) / 2;
			m_lastUpdate = DateTime.Now;

			EntityEventManager.Instance.ResourceLocked = true;

			List<EntityEventManager.EntityEvent> events = EntityEventManager.Instance.EntityEvents;
			List<ChatManager.ChatEvent> chatEvents = ChatManager.Instance.ChatEvents;
			//Generate the player join/leave events here
			List<ulong> connectedPlayers = PlayerManager.Instance.ConnectedPlayers;
			try
			{
				foreach (ulong steamId in connectedPlayers)
				{
					if (!m_lastConnectedPlayerList.Contains(steamId))
					{
						EntityEventManager.EntityEvent playerEvent = new EntityEventManager.EntityEvent();
						playerEvent.priority = 1;
						playerEvent.timestamp = DateTime.Now;
						playerEvent.type = EntityEventManager.EntityEventType.OnPlayerJoined;
						//TODO - Find a way to stall the event long enough for a linked character entity to exist - this is impossible because of cockpits and respawnships
						//For now, create a dummy entity just for passing the player's steam id along
						playerEvent.entity = (Object)steamId;
						events.Add(playerEvent);
					}
				}
				foreach (ulong steamId in m_lastConnectedPlayerList)
				{
					if (!connectedPlayers.Contains(steamId))
					{
						EntityEventManager.EntityEvent playerEvent = new EntityEventManager.EntityEvent();
						playerEvent.priority = 1;
						playerEvent.timestamp = DateTime.Now;
						playerEvent.type = EntityEventManager.EntityEventType.OnPlayerLeft;
						//TODO - Find a way to stall the event long enough for a linked character entity to exist - this is impossible because of cockpits and respawnships
						//For now, create a dummy entity just for passing the player's steam id along
						playerEvent.entity = (Object)steamId;
						events.Add(playerEvent);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine("PluginManager.Update() Exception in player discovery: " + ex.ToString());
			}
			m_lastConnectedPlayerList = new List<ulong>(connectedPlayers);
			foreach (var key in m_plugins.Keys)
			{
				var plugin = m_plugins[key];

				if (!m_pluginState.ContainsKey(key))
					continue;
				PluginManagerThreadParams parameters = new PluginManagerThreadParams();
				parameters.plugin = plugin;
				parameters.key = key;
				parameters.events = new List<EntityEventManager.EntityEvent>(events);
				parameters.chatEvents = new List<ChatManager.ChatEvent>(chatEvents);

				Thread pluginThread = new Thread(doUpdate);
				pluginThread.Start(parameters);

			}

			m_averageEvents = (m_averageEvents + (events.Count + chatEvents.Count)) / 2;

			TimeSpan updateTime = DateTime.Now - m_lastUpdate;
			m_averageUpdateTime = (m_averageUpdateTime + updateTime.TotalMilliseconds) / 2;

			TimeSpan timeSinceAverageOutput = DateTime.Now - m_lastAverageOutput;
			if (timeSinceAverageOutput.TotalSeconds > 30)
			{
				m_lastAverageOutput = DateTime.Now;

				if (SandboxGameAssemblyWrapper.IsDebugging)
				{
					LogManager.APILog.WriteLine("PluginManager - Update interval = " + m_averageUpdateInterval.ToString() + "ms");
					LogManager.APILog.WriteLine("PluginManager - Update time = " + m_averageUpdateTime.ToString() + "ms");
					LogManager.APILog.WriteLine("PluginManager - Events per update = " + m_averageEvents.ToString());
				}
			}

			EntityEventManager.Instance.ClearEvents();
			EntityEventManager.Instance.ResourceLocked = false;
			ChatManager.Instance.ClearEvents();
		}
		public static void doUpdate(object _parameters)
		{
			if (_parameters == null) return;
			PluginManagerThreadParams parameters = (PluginManagerThreadParams)_parameters;

			List<EntityEventManager.EntityEvent> events = parameters.events;
			List<ChatManager.ChatEvent> chatEvents = parameters.chatEvents;
			Object plugin = parameters.plugin;

			//Run entity events
			foreach (EntityEventManager.EntityEvent entityEvent in events)
			{
				//If this is a cube block created event and the parent cube grid is still loading then defer the event
				if (entityEvent.type == EntityEventManager.EntityEventType.OnCubeBlockCreated)
				{
					CubeBlockEntity cubeBlock = (CubeBlockEntity)entityEvent.entity;
					if (cubeBlock.Parent.IsLoading)
					{
						EntityEventManager.Instance.AddEvent(entityEvent);
						continue;
					}
				}

				switch (entityEvent.type)
				{
					case EntityEventManager.EntityEventType.OnPlayerJoined:
						try
						{
							MethodInfo updateMethod = plugin.GetType().GetMethod("OnPlayerJoined");
							if (updateMethod != null)
							{
								//FIXME - Temporary hack to pass along the player's steam id
								ulong steamId = (ulong)entityEvent.entity;
								updateMethod.Invoke(plugin, new object[] { steamId });
							}
						}
						catch (Exception ex)
						{
							LogManager.GameLog.WriteLine(ex);
						}
						break;
					case EntityEventManager.EntityEventType.OnPlayerLeft:
						try
						{
							MethodInfo updateMethod = plugin.GetType().GetMethod("OnPlayerLeft");
							if (updateMethod != null)
							{
								//FIXME - Temporary hack to pass along the player's steam id
								ulong steamId = (ulong)entityEvent.entity;
								updateMethod.Invoke(plugin, new object[] { steamId });
							}
						}
						catch (Exception ex)
						{
							LogManager.GameLog.WriteLine(ex);
						}
						break;
					default:
						try
						{
							string methodName = entityEvent.type.ToString();
							MethodInfo updateMethod = plugin.GetType().GetMethod(methodName);
							if (updateMethod != null)
								updateMethod.Invoke(plugin, new object[] { entityEvent.entity });
						}
						catch (Exception ex)
						{
							LogManager.GameLog.WriteLine(ex);
						}
						break;
				}
			}

			//Run chat events
			foreach (ChatManager.ChatEvent chatEvent in chatEvents)
			{
				try
				{
					string methodName = chatEvent.type.ToString();
					MethodInfo updateMethod = plugin.GetType().GetMethod(methodName);
					if (updateMethod != null)
						updateMethod.Invoke(plugin, new object[] { chatEvent });
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
				}
			}

			//Run update
			try
			{
				MethodInfo updateMethod = plugin.GetType().GetMethod("Update");
				updateMethod.Invoke(plugin, new object[] { });
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine(ex);
			}
		}
		public void Shutdown()
		{
			foreach (var key in m_plugins.Keys)
			{
				UnloadPlugin(key);
			}
		}

		public void InitPlugin(Guid key)
		{
			if (m_pluginState.ContainsKey(key))
				return;

			LogManager.APILog.WriteLineAndConsole("Initializing plugin '" + key.ToString() + "' ...");

			try
			{
				var plugin = m_plugins[key];
				MethodInfo initMethod = plugin.GetType().GetMethod("Init");
				initMethod.Invoke(plugin, new object[] { });

				m_pluginState.Add(key, true);
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine(ex);
			}
		}

		public void UnloadPlugin(Guid key)
		{
			if (key == null)
				return;

			//Skip if the plugin doesn't exist
			if (!m_plugins.ContainsKey(key))
				return;

			//Skip if the pluing is already unloaded
			if (!m_pluginState.ContainsKey(key))
				return;

			LogManager.APILog.WriteLineAndConsole("Unloading plugin '" + key.ToString() + "'");

			var plugin = m_plugins[key];
			try
			{
				MethodInfo initMethod = plugin.GetType().GetMethod("Shutdown");
				initMethod.Invoke(plugin, new object[] { });
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine(ex);
			}

			m_pluginState.Remove(key);
		}

		public bool GetPluginState(Guid key)
		{
			if (!m_pluginState.ContainsKey(key))
				return false;

			return m_pluginState[key];
		}

		#endregion
	}
}
