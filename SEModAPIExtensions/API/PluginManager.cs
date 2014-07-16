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

using VRage.Common.Utils;

namespace SEModAPIExtensions.API
{
	public class PluginManager
	{
		#region "Attributes"

		private static PluginManager m_instance;

		private Dictionary<Guid, Object> m_plugins;
		private bool m_initialized;
		
		private APIExtensionsUtils m_utils;

		#endregion

		#region "Constructors and Initializers"

		protected PluginManager()
		{
			m_instance = this;

			m_plugins = new Dictionary<Guid, Object>();
			m_initialized = false;

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
		
		protected APIExtensionsUtils Utils
		{
			get { return m_utils; }
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
				List<Type> types = m_utils.CheckFileTypes(IPlugin, m_plugins)
				foreach(Type type in types)
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
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}

			Console.WriteLine("Finished loading plugins");
		}

		public void Init()
		{
			Console.WriteLine("Initializing plugins ...");

			foreach (var plugin in m_plugins.Values)
			{
				try
				{
					MethodInfo initMethod = plugin.GetType().GetMethod("Init");
					initMethod.Invoke(plugin, new object[] { });
				}
				catch (Exception ex)
				{
					LogManager.APILog.WriteLine(ex);
				}
			}

			Console.WriteLine("Finished initializing plugins");

			m_initialized = true;
		}

		public void Update()
		{
			if (!Initialized)
				return;
			if (!SandboxGameAssemblyWrapper.Instance.IsGameStarted)
				return;

			EntityEventManager.Instance.ResourceLocked = true;
			foreach (var plugin in m_plugins.Values)
			{
				//Run entity events
				List<EntityEventManager.EntityEvent> events = EntityEventManager.Instance.EntityEvents;
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
								//TODO - Get the right remote user Id here
								if(updateMethod != null)
									updateMethod.Invoke(plugin, new object[] { (ulong)0, entityEvent.entity });
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
								//TODO - Get the right remote user Id here
								if (updateMethod != null)
									updateMethod.Invoke(plugin, new object[] { (ulong)0, entityEvent.entity });
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
				List<ChatManager.ChatEvent> chatEvents = ChatManager.Instance.ChatEvents;
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

			EntityEventManager.Instance.ClearEvents();
			EntityEventManager.Instance.ResourceLocked = false;
			ChatManager.Instance.ClearEvents();
		}

		#endregion
	}
}
