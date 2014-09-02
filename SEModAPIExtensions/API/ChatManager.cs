using SteamSDK;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Server;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;
using SEModAPIInternal.Support;

using VRage;
using VRage.Common.Utils;
using VRageMath;

namespace SEModAPIExtensions.API
{
	[ServiceContract]
	public interface IChatServiceContract
	{
		[OperationContract]
		List<string> GetChatMessages();

		[OperationContract]
		void SendPrivateChatMessage(ulong remoteUserId, string message);

		[OperationContract]
		void SendPublicChatMessage(string message);
	}

	[ServiceBehavior(
		ConcurrencyMode = ConcurrencyMode.Single,
		IncludeExceptionDetailInFaults = true,
		IgnoreExtensionDataObject = true
	)]
	public class ChatService : IChatServiceContract
	{
		public List<string> GetChatMessages()
		{
			return ChatManager.Instance.ChatMessages;
		}

		public void SendPrivateChatMessage(ulong remoteUserId, string message)
		{
			ChatManager.Instance.SendPrivateChatMessage(remoteUserId, message);
		}

		public void SendPublicChatMessage(string message)
		{
			ChatManager.Instance.SendPublicChatMessage(message);
		}
	}

	public class ChatManager
	{
		public struct ChatCommand
		{
			public string command;
			public Action<ChatEvent> callback;
			public bool requiresAdmin;
		}

		public enum ChatEventType
		{
			OnChatReceived,
			OnChatSent,
		}

		public struct ChatEvent
		{
			public ChatEventType type;
			public DateTime timestamp;
			public ulong sourceUserId;
			public ulong remoteUserId;
			public string message;
			public ushort priority;
		}

		#region "Attributes"

		private static ChatManager m_instance;

		private static List<string> m_chatMessages;
		private static List<ChatEvent> m_chatHistory;
		private static bool m_chatHandlerSetup;
		private static FastResourceLock m_resourceLock;

		private List<ChatEvent> m_chatEvents;
		private Dictionary<ChatCommand, Guid> m_chatCommands;

		/////////////////////////////////////////////////////////////////////////////

		public static string ChatMessageStructNamespace = "C42525D7DE28CE4CFB44651F3D03A50D";
		public static string ChatMessageStructClass = "12AEE9CB08C9FC64151B8A094D6BB668";

		public static string ChatMessageMessageField = "EDCBEBB604B287DFA90A5A46DC7AD28D";

		#endregion

		#region "Constructors and Initializers"

		protected ChatManager()
		{
			m_instance = this;

			m_chatMessages = new List<string>();
			m_chatHistory = new List<ChatEvent>();
			m_chatHandlerSetup = false;
			m_resourceLock = new FastResourceLock();
			m_chatEvents = new List<ChatEvent>();
			m_chatCommands = new Dictionary<ChatCommand, Guid>();

			ChatCommand deleteCommand = new ChatCommand();
			deleteCommand.command = "delete";
			deleteCommand.callback = Command_Delete;
			deleteCommand.requiresAdmin = true;

			ChatCommand tpCommand = new ChatCommand();
			tpCommand.command = "tp";
			tpCommand.callback = Command_Teleport;
			tpCommand.requiresAdmin = true;

			ChatCommand stopCommand = new ChatCommand();
			stopCommand.command = "stop";
			stopCommand.callback = Command_Stop;
			stopCommand.requiresAdmin = true;

			ChatCommand getIdCommand = new ChatCommand();
			getIdCommand.command = "getid";
			getIdCommand.callback = Command_GetId;
			getIdCommand.requiresAdmin = true;

			ChatCommand saveCommand = new ChatCommand();
			saveCommand.command = "save";
			saveCommand.callback = Command_Save;
			saveCommand.requiresAdmin = true;

			ChatCommand ownerCommand = new ChatCommand();
			ownerCommand.command = "owner";
			ownerCommand.callback = Command_Owner;
			ownerCommand.requiresAdmin = true;

			ChatCommand exportCommand = new ChatCommand();
			exportCommand.command = "export";
			exportCommand.callback = Command_Export;
			exportCommand.requiresAdmin = true;

			ChatCommand importCommand = new ChatCommand();
			importCommand.command = "import";
			importCommand.callback = Command_Import;
			importCommand.requiresAdmin = true;

			ChatCommand spawnCommand = new ChatCommand();
			spawnCommand.command = "spawn";
			spawnCommand.callback = Command_Spawn;
			spawnCommand.requiresAdmin = true;

			ChatCommand clearCommand = new ChatCommand();
			clearCommand.command = "clear";
			clearCommand.callback = Command_Clear;
			clearCommand.requiresAdmin = true;

			ChatCommand listCommand = new ChatCommand();
			listCommand.command = "list";
			listCommand.callback = Command_List;
			listCommand.requiresAdmin = true;

			ChatCommand offCommand = new ChatCommand();
			offCommand.command = "off";
			offCommand.callback = Command_Off;
			offCommand.requiresAdmin = true;

			ChatCommand kickCommand = new ChatCommand();
			kickCommand.command = "kick";
			kickCommand.callback = Command_Kick;
			kickCommand.requiresAdmin = true;

			ChatCommand banCommand = new ChatCommand();
			banCommand.command = "ban";
			banCommand.callback = Command_Ban;
			banCommand.requiresAdmin = true;

			ChatCommand unbanCommand = new ChatCommand();
			unbanCommand.command = "unban";
			unbanCommand.callback = Command_Unban;
			unbanCommand.requiresAdmin = true;

			RegisterChatCommand(deleteCommand);
			RegisterChatCommand(tpCommand);
			RegisterChatCommand(stopCommand);
			RegisterChatCommand(getIdCommand);
			RegisterChatCommand(saveCommand);
			RegisterChatCommand(ownerCommand);
			RegisterChatCommand(exportCommand);
			RegisterChatCommand(importCommand);
			RegisterChatCommand(spawnCommand);
			RegisterChatCommand(clearCommand);
			RegisterChatCommand(listCommand);
			RegisterChatCommand(offCommand);
			RegisterChatCommand(kickCommand);

			SetupWCFService();

			Console.WriteLine("Finished loading ChatManager");
		}

		private bool SetupWCFService()
		{
			if (!Server.Instance.IsWCFEnabled)
				return true;

			ServiceHost selfHost = null;
			try
			{
				selfHost = Server.CreateServiceHost(typeof(ChatService), typeof(IChatServiceContract), "Chat/", "ChatService");
				selfHost.Open();
			}
			catch (CommunicationException ex)
			{
				LogManager.ErrorLog.WriteLineAndConsole("An exception occurred: " + ex.Message);
				if(selfHost != null)
					selfHost.Abort();
				return false;
			}

			return true;
		}

		#endregion

		#region "Properties"

		public static ChatManager Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new ChatManager();
				}
				return m_instance;
			}
		}

		public List<string> ChatMessages
		{
			get
			{
				if (!m_chatHandlerSetup)
				{
					if (SandboxGameAssemblyWrapper.Instance.IsGameStarted)
					{
						Action action = SetupChatHandlers;
						SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
					}
				}

				return m_chatMessages;
			}
		}

		public List<ChatEvent> ChatHistory
		{
			get
			{
				if (!m_chatHandlerSetup)
				{
					if (SandboxGameAssemblyWrapper.Instance.IsGameStarted)
					{
						Action action = SetupChatHandlers;
						SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
					}
				}

				m_resourceLock.AcquireShared();

				List<ChatEvent> history = new List<ChatEvent>(m_chatHistory);

				m_resourceLock.ReleaseShared();

				return history;
			}
		}

		public List<ChatEvent> ChatEvents
		{
			get
			{
				if (!m_chatHandlerSetup)
				{
					if (SandboxGameAssemblyWrapper.Instance.IsGameStarted)
					{
						Action action = SetupChatHandlers;
						SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
					}
				}

				List<ChatEvent> copy = new List<ChatEvent>(m_chatEvents.ToArray());
				return copy;
			}
		}

		#endregion

		#region "Methods"

		#region "General"

		public static bool ReflectionUnitTest()
		{
			try
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(ChatMessageStructNamespace, ChatMessageStructClass);
				if (type == null)
					throw new Exception("Could not find internal type for ChatMessageStruct");
				bool result = true;
				result &= BaseObject.HasField(type, ChatMessageMessageField);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		private void SetupChatHandlers()
		{
			try
			{
				if (m_chatHandlerSetup == true)
					return;

				var netManager = ServerNetworkManager.GetNetworkManager();
				if (netManager == null)
					return;

				Action<ulong, string, ChatEntryTypeEnum> chatHook = ReceiveChatMessage;
				ServerNetworkManager.Instance.RegisterChatReceiver(chatHook);

				m_chatHandlerSetup = true;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected Object CreateChatMessageStruct(string message)
		{
			Type chatMessageStructType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(ChatMessageStructNamespace, ChatMessageStructClass);
			FieldInfo messageField = chatMessageStructType.GetField(ChatMessageMessageField);

			Object chatMessageStruct = Activator.CreateInstance(chatMessageStructType);
			messageField.SetValue(chatMessageStruct, message);

			return chatMessageStruct;
		}

		protected void ReceiveChatMessage(ulong remoteUserId, string message, ChatEntryTypeEnum entryType)
		{
			string playerName = PlayerMap.Instance.GetPlayerNameFromSteamId(remoteUserId);

			bool commandParsed = ParseChatCommands(message, remoteUserId);

			if (!commandParsed && entryType == ChatEntryTypeEnum.ChatMsg)
			{
				m_chatMessages.Add(playerName + ": " + message);
				LogManager.ChatLog.WriteLineAndConsole("Chat - Client '" + playerName + "': " + message);
			}

			ChatEvent chatEvent = new ChatEvent();
			chatEvent.type = ChatEventType.OnChatReceived;
			chatEvent.timestamp = DateTime.Now;
			chatEvent.sourceUserId = remoteUserId;
			chatEvent.remoteUserId = 0;
			chatEvent.message = message;
			chatEvent.priority = 0;
			ChatManager.Instance.AddEvent(chatEvent);

			m_resourceLock.AcquireExclusive();
			m_chatHistory.Add(chatEvent);
			m_resourceLock.ReleaseExclusive();
		}

		public void SendPrivateChatMessage(ulong remoteUserId, string message)
		{
			if (!SandboxGameAssemblyWrapper.Instance.IsGameStarted)
				return;
			if (string.IsNullOrEmpty(message))
				return;

			try
			{
				if (remoteUserId != 0)
				{
					Object chatMessageStruct = CreateChatMessageStruct(message);
					ServerNetworkManager.Instance.SendStruct(remoteUserId, chatMessageStruct, chatMessageStruct.GetType());
				}

				m_chatMessages.Add("Server: " + message);

				LogManager.ChatLog.WriteLineAndConsole("Chat - Server: " + message);

				ChatEvent chatEvent = new ChatEvent();
				chatEvent.type = ChatEventType.OnChatSent;
				chatEvent.timestamp = DateTime.Now;
				chatEvent.sourceUserId = 0;
				chatEvent.remoteUserId = remoteUserId;
				chatEvent.message = message;
				chatEvent.priority = 0;
				ChatManager.Instance.AddEvent(chatEvent);

				m_resourceLock.AcquireExclusive();
				m_chatHistory.Add(chatEvent);
				m_resourceLock.ReleaseExclusive();
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		public void SendPublicChatMessage(string message)
		{
			if (!SandboxGameAssemblyWrapper.Instance.IsGameStarted)
				return;
			if (string.IsNullOrEmpty(message))
				return;

			bool commandParsed = ParseChatCommands(message);

			try
			{
				if (!commandParsed)
				{
					Object chatMessageStruct = CreateChatMessageStruct(message);
					List<ulong> connectedPlayers = PlayerManager.Instance.ConnectedPlayers;
					foreach (ulong remoteUserId in connectedPlayers)
					{
						ServerNetworkManager.Instance.SendStruct(remoteUserId, chatMessageStruct, chatMessageStruct.GetType());

						ChatEvent chatEvent = new ChatEvent();
						chatEvent.type = ChatEventType.OnChatSent;
						chatEvent.timestamp = DateTime.Now;
						chatEvent.sourceUserId = 0;
						chatEvent.remoteUserId = remoteUserId;
						chatEvent.message = message;
						chatEvent.priority = 0;
						ChatManager.Instance.AddEvent(chatEvent);
					}
					m_chatMessages.Add("Server: " + message);
					LogManager.ChatLog.WriteLineAndConsole("Chat - Server: " + message);
				}

				//Send a loopback chat event for server-sent messages
				ChatEvent selfChatEvent = new ChatEvent();
				selfChatEvent.type = ChatEventType.OnChatSent;
				selfChatEvent.timestamp = DateTime.Now;
				selfChatEvent.sourceUserId = 0;
				selfChatEvent.remoteUserId = 0;
				selfChatEvent.message = message;
				selfChatEvent.priority = 0;
				ChatManager.Instance.AddEvent(selfChatEvent);

				m_resourceLock.AcquireExclusive();
				m_chatHistory.Add(selfChatEvent);
				m_resourceLock.ReleaseExclusive();
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected bool ParseChatCommands(string message, ulong remoteUserId = 0)
		{
			try
			{
				if (string.IsNullOrEmpty(message))
					return false;

				string[] commandParts = message.Split(' ');
				if (commandParts == null || commandParts.Length == 0)
					return false;

				//Skip if message doesn't have leading forward slash
				if (!message.Substring(0, 1).Equals("/"))
					return false;

				//Get the base command and strip off the leading slash
				string command = commandParts[0].ToLower().Substring(1);
				if (string.IsNullOrEmpty(command))
					return false;

				//Search for a matching, registered command
				bool foundMatch = false;
				foreach (ChatCommand chatCommand in m_chatCommands.Keys)
				{
					try
					{
						if (chatCommand.requiresAdmin && remoteUserId != 0 && !PlayerManager.Instance.IsUserAdmin(remoteUserId))
							continue;

						if (command.Equals(chatCommand.command.ToLower()))
						{
							ChatEvent chatEvent = new ChatEvent();
							chatEvent.message = message;
							chatEvent.remoteUserId = remoteUserId;
							chatEvent.timestamp = DateTime.Now;

							chatCommand.callback(chatEvent);

							foundMatch = true;
							break;
						}
					}
					catch (Exception ex)
					{
						LogManager.ErrorLog.WriteLine(ex);
					}
				}

				if (foundMatch)
					return true;
				else
					return false;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return false;
			}
		}

		public void RegisterChatCommand(ChatCommand command)
		{
			//Check if the given command already is registered
			foreach (ChatCommand chatCommand in m_chatCommands.Keys)
			{
				if (chatCommand.command.ToLower().Equals(command.command.ToLower()))
					return;
			}

			GuidAttribute guid = (GuidAttribute)Assembly.GetCallingAssembly().GetCustomAttributes(typeof(GuidAttribute), true)[0];
			Guid guidValue = new Guid(guid.Value);

			m_chatCommands.Add(command, guidValue);
		}

		public void UnregisterChatCommands()
		{
			GuidAttribute guid = (GuidAttribute)Assembly.GetCallingAssembly().GetCustomAttributes(typeof(GuidAttribute), true)[0];
			Guid guidValue = new Guid(guid.Value);

			List<ChatCommand> commandsToRemove = new List<ChatCommand>();
			foreach (var entry in m_chatCommands)
			{
				if (entry.Value.Equals(guidValue))
					commandsToRemove.Add(entry.Key);
			}
			foreach (var entry in commandsToRemove)
			{
				m_chatCommands.Remove(entry);
			}
		}

		public void AddEvent(ChatEvent newEvent)
		{
			m_chatEvents.Add(newEvent);
		}

		public void ClearEvents()
		{
			m_chatEvents.Clear();
		}

		#endregion

		#region "Chat Command Callbacks"

		protected void Command_Delete(ChatEvent chatEvent)
		{
			ulong remoteUserId = chatEvent.remoteUserId;
			string[] commandParts = chatEvent.message.Split(' ');
			int paramCount = commandParts.Length - 1;

			//All entities
			if (paramCount > 1 && commandParts[1].ToLower().Equals("all"))
			{
				//All cube grids that have no beacon or only a beacon with no name
				if (commandParts[2].ToLower().Equals("nobeacon"))
				{
					List<CubeGridEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<CubeGridEntity>();
					List<CubeGridEntity> entitiesToDispose = new List<CubeGridEntity>();
					foreach (CubeGridEntity entity in entities)
					{
						while (entity.CubeBlocks.Count == 0)
						{
							Thread.Sleep(20);
						}
						List<CubeBlockEntity> blocks = entity.CubeBlocks;
						if (blocks.Count > 0)
						{
							bool foundBeacon = false;
							foreach (CubeBlockEntity cubeBlock in blocks)
							{
								if (cubeBlock is BeaconEntity)
								{
									foundBeacon = true;
									break;
								}
							}
							if (!foundBeacon)
							{
								entitiesToDispose.Add(entity);
							}
						}
					}

					foreach (CubeGridEntity entity in entitiesToDispose)
					{
						bool isLinkedShip = false;
						List<CubeBlockEntity> blocks = entity.CubeBlocks;
						foreach (CubeBlockEntity cubeBlock in blocks)
						{
							if (cubeBlock is MergeBlockEntity)
							{
								MergeBlockEntity block = (MergeBlockEntity)cubeBlock;
								if (block.IsAttached)
								{
									if (!entitiesToDispose.Contains(block.AttachedCubeGrid))
									{
										isLinkedShip = true;
										break;
									}
								}
							}
							if (cubeBlock is PistonEntity)
							{
								PistonEntity block = (PistonEntity)cubeBlock;
								CubeBlockEntity topBlock = block.TopBlock;
								if (topBlock != null)
								{
									if (!entitiesToDispose.Contains(topBlock.Parent))
									{
										isLinkedShip = true;
										break;
									}
								}
							}
							if (cubeBlock is RotorEntity)
							{
								RotorEntity block = (RotorEntity)cubeBlock;
								CubeBlockEntity topBlock = block.TopBlock;
								if (topBlock != null)
								{
									if (!entitiesToDispose.Contains(topBlock.Parent))
									{
										isLinkedShip = true;
										break;
									}
								}
							}
						}
						if (isLinkedShip)
							continue;

						entity.Dispose();
					}

					SendPrivateChatMessage(remoteUserId, entitiesToDispose.Count.ToString() + " cube grids have been removed");
				}
				//All cube grids that have no power
				else if (commandParts[2].ToLower().Equals("nopower"))
				{
					List<CubeGridEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<CubeGridEntity>();
					List<CubeGridEntity> entitiesToDispose = new List<CubeGridEntity>();
					foreach (CubeGridEntity entity in entities)
					{
						if (entity.TotalPower <= 0)
						{
							entitiesToDispose.Add(entity);
						}
					}

					foreach (CubeGridEntity entity in entitiesToDispose)
					{
						entity.Dispose();
					}

					SendPrivateChatMessage(remoteUserId, entitiesToDispose.Count.ToString() + " cube grids have been removed");
				}
				else if (commandParts[2].ToLower().Equals("floatingobjects"))	//All floating objects
				{
					List<FloatingObject> entities = SectorObjectManager.Instance.GetTypedInternalData<FloatingObject>();
					int floatingObjectCount = entities.Count;
					foreach (FloatingObject entity in entities)
					{
						entity.Dispose();
					}

					SendPrivateChatMessage(remoteUserId, floatingObjectCount.ToString() + " floating objects have been removed");
				}
				else
				{
					string entityName = commandParts[2];
					if (commandParts.Length > 3)
					{
						for (int i = 3; i < commandParts.Length; i++)
						{
							entityName += " " + commandParts[i];
						}
					}

					int matchingEntitiesCount = 0;
					List<BaseEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>();
					foreach (BaseEntity entity in entities)
					{
						bool isMatch = Regex.IsMatch(entity.Name, entityName, RegexOptions.IgnoreCase);
						if (!isMatch)
							continue;

						entity.Dispose();

						matchingEntitiesCount++;
					}

					SendPrivateChatMessage(remoteUserId, matchingEntitiesCount.ToString() + " objects have been removed");
				}
			}

			//All non-static cube grids
			if (paramCount > 1 && commandParts[1].ToLower().Equals("ship"))
			{
				//That have no beacon or only a beacon with no name
				if (commandParts[2].ToLower().Equals("nobeacon"))
				{
					List<CubeGridEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<CubeGridEntity>();
					List<CubeGridEntity> entitiesToDispose = new List<CubeGridEntity>();
					foreach (CubeGridEntity entity in entities)
					{
						//Skip static cube grids
						if (((CubeGridEntity)entity).IsStatic)
							continue;

						if (entity.Name.Equals(entity.EntityId.ToString()))
						{
							entitiesToDispose.Add(entity);
							continue;
						}

						List<CubeBlockEntity> blocks = entity.CubeBlocks;
						if (blocks.Count > 0)
						{
							bool foundBeacon = false;
							foreach (CubeBlockEntity cubeBlock in entity.CubeBlocks)
							{
								if (cubeBlock is BeaconEntity)
								{
									foundBeacon = true;
									break;
								}
							}
							if (!foundBeacon)
							{
								entitiesToDispose.Add(entity);
							}
						}
					}

					foreach (CubeGridEntity entity in entitiesToDispose)
					{
						entity.Dispose();
					}

					SendPrivateChatMessage(remoteUserId, entitiesToDispose.Count.ToString() + " ships have been removed");
				}
			}

			//All static cube grids
			if (paramCount > 1 && commandParts[1].ToLower().Equals("station"))
			{
				//That have no beacon or only a beacon with no name
				if (commandParts[2].ToLower().Equals("nobeacon"))
				{
					List<CubeGridEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<CubeGridEntity>();
					List<CubeGridEntity> entitiesToDispose = new List<CubeGridEntity>();
					foreach (CubeGridEntity entity in entities)
					{
						//Skip non-static cube grids
						if (!((CubeGridEntity)entity).IsStatic)
							continue;

						if (entity.Name.Equals(entity.EntityId.ToString()))
						{
							entitiesToDispose.Add(entity);
							continue;
						}

						List<CubeBlockEntity> blocks = entity.CubeBlocks;
						if (blocks.Count > 0)
						{
							bool foundBeacon = false;
							foreach (CubeBlockEntity cubeBlock in entity.CubeBlocks)
							{
								if (cubeBlock is BeaconEntity)
								{
									foundBeacon = true;
									break;
								}
							}
							if (!foundBeacon)
							{
								entitiesToDispose.Add(entity);
							}
						}
					}

					foreach (CubeGridEntity entity in entitiesToDispose)
					{
						entity.Dispose();
					}

					SendPrivateChatMessage(remoteUserId, entitiesToDispose.Count.ToString() + " stations have been removed");
				}
			}

			//Prunes defunct player entries in the faction data
			if (paramCount > 1 && commandParts[1].ToLower().Equals("player"))
			{
				List<MyObjectBuilder_Checkpoint.PlayerItem> playersToRemove = new List<MyObjectBuilder_Checkpoint.PlayerItem>();
				int playersRemovedCount = 0;
				if (commandParts[2].ToLower().Equals("dead"))
				{
					List<long> playerIds = PlayerMap.Instance.GetPlayerIds();
					foreach (long playerId in playerIds)
					{
						MyObjectBuilder_Checkpoint.PlayerItem item = PlayerMap.Instance.GetPlayerItemFromPlayerId(playerId);
						if (item.IsDead)
							playersToRemove.Add(item);
					}

					//TODO - This is VERY slow. Need to find a much faster way to do this
					//TODO - Need to find a way to remove the player entries from the main list, not just from the blocks and factions
					foreach (var item in playersToRemove)
					{
						bool playerRemoved = false;

						//Check if any of the players we're about to remove own blocks
						//If so, set the owner to 0 and set the share mode to None
						foreach (var cubeGrid in SectorObjectManager.Instance.GetTypedInternalData<CubeGridEntity>())
						{
							foreach (var cubeBlock in cubeGrid.CubeBlocks)
							{
								if (cubeBlock.Owner == item.PlayerId)
								{
									cubeBlock.Owner = 0;
									cubeBlock.ShareMode = MyOwnershipShareModeEnum.None;

									playerRemoved = true;
								}
							}
						}

						foreach (var entry in FactionsManager.Instance.Factions)
						{
							foreach (var member in entry.Members)
							{
								if (member.PlayerId == item.PlayerId)
								{
									entry.RemoveMember(member.PlayerId);

									playerRemoved = true;
								}
							}
						}

						if (playerRemoved)
							playersRemovedCount++;
					}
				}

				SendPrivateChatMessage(remoteUserId, "Deleted " + playersRemovedCount.ToString() + " player entries");
			}

			//Prunes defunct faction entries in the faction data
			if (paramCount > 1 && commandParts[1].ToLower().Equals("faction"))
			{
				List<Faction> factionsToRemove = new List<Faction>();
				if (commandParts[2].ToLower().Equals("empty"))
				{
					foreach(var entry in FactionsManager.Instance.Factions)
					{
						if (entry.Members.Count == 0)
							factionsToRemove.Add(entry);
					}
				}
				if (commandParts[2].ToLower().Equals("nofounder"))
				{
					foreach (var entry in FactionsManager.Instance.Factions)
					{
						bool founderMatch = false;

						foreach (var member in entry.Members)
						{
							if (member.IsFounder)
							{
								founderMatch = true;
								break;
							}
						}

						if (!founderMatch)
							factionsToRemove.Add(entry);
					}
				}
				if (commandParts[2].ToLower().Equals("noleader"))
				{
					foreach (var entry in FactionsManager.Instance.Factions)
					{
						bool founderMatch = false;

						foreach (var member in entry.Members)
						{
							if (member.IsFounder || member.IsLeader)
							{
								founderMatch = true;
								break;
							}
						}

						if (!founderMatch)
							factionsToRemove.Add(entry);
					}
				}

				foreach (var entry in factionsToRemove)
				{
					FactionsManager.Instance.RemoveFaction(entry.Id);
				}

				SendPrivateChatMessage(remoteUserId, "Deleted " + factionsToRemove.Count.ToString() + " factions");
			}

			//Single entity
			if (paramCount == 1)
			{
				string rawEntityId = commandParts[1];

				try
				{
					long entityId = long.Parse(rawEntityId);

					List<BaseEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>();
					foreach (BaseEntity entity in entities)
					{
						if (entity.EntityId != entityId)
							continue;

						entity.Dispose();
					}
				}
				catch (Exception ex)
				{
					LogManager.ErrorLog.WriteLine(ex);
				}
			}
		}

		protected void Command_Teleport(ChatEvent chatEvent)
		{
			ulong remoteUserId = chatEvent.remoteUserId;
			string[] commandParts = chatEvent.message.Split(' ');
			int paramCount = commandParts.Length - 1;

			if (paramCount == 2)
			{
				string rawEntityId = commandParts[1];
				string rawPosition = commandParts[2];

				try
				{
					long entityId = long.Parse(rawEntityId);

					string[] rawCoordinateValues = rawPosition.Split(',');
					if (rawCoordinateValues.Length < 3)
						return;

					float x = float.Parse(rawCoordinateValues[0]);
					float y = float.Parse(rawCoordinateValues[1]);
					float z = float.Parse(rawCoordinateValues[2]);

					List<BaseEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>();
					foreach (BaseEntity entity in entities)
					{
						if (entity.EntityId != entityId)
							continue;

						Vector3 newPosition = new Vector3(x, y, z);
						entity.Position = newPosition;

						SendPrivateChatMessage(remoteUserId, "Entity '" + entity.EntityId.ToString() + "' has been moved to '" + newPosition.ToString() + "'");
					}
				}
				catch (Exception ex)
				{
					LogManager.ErrorLog.WriteLine(ex);
				}
			}
		}

		protected void Command_Stop(ChatEvent chatEvent)
		{
			ulong remoteUserId = chatEvent.remoteUserId;
			string[] commandParts = chatEvent.message.Split(' ');
			int paramCount = commandParts.Length - 1;

			if (paramCount != 1)
				return;

			if (commandParts[1].ToLower().Equals("all"))
			{
				List<BaseEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>();
				int entitiesStoppedCount = 0;
				foreach (BaseEntity entity in entities)
				{
					double linear = Math.Round(((Vector3)entity.LinearVelocity).LengthSquared(), 1);
					double angular = Math.Round(((Vector3)entity.AngularVelocity).LengthSquared(), 1);

					if (linear > 0 || angular > 0)
					{
						entity.LinearVelocity = Vector3.Zero;
						entity.AngularVelocity = Vector3.Zero;

						entitiesStoppedCount++;
					}
				}
				SendPrivateChatMessage(remoteUserId, entitiesStoppedCount.ToString() + " entities are no longer moving or rotating");
			}
			else
			{
				string rawEntityId = commandParts[1];

				try
				{
					long entityId = long.Parse(rawEntityId);

					List<BaseEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>();
					foreach (BaseEntity entity in entities)
					{
						if (entity.EntityId != entityId)
							continue;

						entity.LinearVelocity = Vector3.Zero;
						entity.AngularVelocity = Vector3.Zero;

						SendPrivateChatMessage(remoteUserId, "Entity '" + entity.EntityId.ToString() + "' is no longer moving or rotating");
					}
				}
				catch (Exception ex)
				{
					LogManager.ErrorLog.WriteLine(ex);
				}
			}
		}

		protected void Command_GetId(ChatEvent chatEvent)
		{
			ulong remoteUserId = chatEvent.remoteUserId;
			string[] commandParts = chatEvent.message.Split(' ');
			int paramCount = commandParts.Length - 1;

			if (paramCount > 0)
			{
				string entityName = commandParts[1];
				if (commandParts.Length > 2)
				{
					for (int i = 2; i < commandParts.Length; i++)
					{
						entityName += " " + commandParts[i];
					}
				}

				List<BaseEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>();
				foreach (BaseEntity entity in entities)
				{
					if (!entity.Name.ToLower().Equals(entityName.ToLower()))
						continue;

					SendPrivateChatMessage(remoteUserId, "Entity ID is '" + entity.EntityId.ToString() + "'");
				}
			}
		}

		protected void Command_Save(ChatEvent chatEvent)
		{
			ulong remoteUserId = chatEvent.remoteUserId;
			string[] commandParts = chatEvent.message.Split(' ');
			int paramCount = commandParts.Length - 1;

			WorldManager.Instance.SaveWorld();

			SendPrivateChatMessage(remoteUserId, "World has been saved!");
		}

		protected void Command_Owner(ChatEvent chatEvent)
		{
			ulong remoteUserId = chatEvent.remoteUserId;
			string[] commandParts = chatEvent.message.Split(' ');
			int paramCount = commandParts.Length - 1;

			if (paramCount == 2)
			{
				string rawEntityId = commandParts[1];
				string rawOwnerId = commandParts[2];

				try
				{
					long entityId = long.Parse(rawEntityId);
					long ownerId = long.Parse(rawOwnerId);

					List<CubeGridEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<CubeGridEntity>();
					foreach (CubeGridEntity cubeGrid in entities)
					{
						if (cubeGrid.EntityId != entityId)
							continue;

						//Update the owner of the blocks on the cube grid
						foreach (CubeBlockEntity cubeBlock in cubeGrid.CubeBlocks)
						{
							//Skip blocks that don't have an entity id
							if (cubeBlock.EntityId == 0)
								continue;

							cubeBlock.Owner = ownerId;
						}

						SendPrivateChatMessage(remoteUserId, "CubeGridEntity '" + cubeGrid.EntityId.ToString() + "' owner has been changed to '" + ownerId.ToString() + "'");
					}
				}
				catch (Exception ex)
				{
					LogManager.ErrorLog.WriteLine(ex);
				}
			}
		}

		protected void Command_Export(ChatEvent chatEvent)
		{
			ulong remoteUserId = chatEvent.remoteUserId;
			string[] commandParts = chatEvent.message.Split(' ');
			int paramCount = commandParts.Length - 1;

			if (paramCount == 1)
			{
				string rawEntityId = commandParts[1];

				try
				{
					long entityId = long.Parse(rawEntityId);

					List<BaseEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>();
					foreach (BaseEntity entity in entities)
					{
						if (entity.EntityId != entityId)
							continue;

						string modPath = MyFileSystem.ModsPath;
						if (!Directory.Exists(modPath))
							break;

						string fileName = entity.Name.ToLower();
						Regex rgx = new Regex("[^a-zA-Z0-9]");
						string cleanFileName = rgx.Replace(fileName, "");

						string exportPath = Path.Combine(modPath, "Exports");
						if (!Directory.Exists(exportPath))
							Directory.CreateDirectory(exportPath);
						FileInfo exportFile = new FileInfo(Path.Combine(exportPath, cleanFileName + ".sbc"));
						entity.Export(exportFile);

						SendPrivateChatMessage(remoteUserId, "Entity '" + entity.EntityId.ToString() + "' has been exported to Mods/Exports");
					}
				}
				catch (Exception ex)
				{
					LogManager.ErrorLog.WriteLine(ex);
				}
			}
		}

		protected void Command_Import(ChatEvent chatEvent)
		{
			ulong remoteUserId = chatEvent.remoteUserId;
			string[] commandParts = chatEvent.message.Split(' ');
			int paramCount = commandParts.Length - 1;

			if (paramCount == 1)
			{
				try
				{
					string fileName = commandParts[1];
					Regex rgx = new Regex("[^a-zA-Z0-9]");
					string cleanFileName = rgx.Replace(fileName, "");

					string modPath = MyFileSystem.ModsPath;
					if (Directory.Exists(modPath))
					{
						string exportPath = Path.Combine(modPath, "Exports");
						if (Directory.Exists(exportPath))
						{
							FileInfo importFile = new FileInfo(Path.Combine(exportPath, cleanFileName));
							if (importFile.Exists)
							{
								string objectBuilderTypeName = "";
								using (XmlReader reader = XmlReader.Create(importFile.OpenText()))
								{
									while (reader.Read())
									{
										if (reader.NodeType == XmlNodeType.XmlDeclaration)
											continue;

										if (reader.NodeType != XmlNodeType.Element)
											continue;

										objectBuilderTypeName = reader.Name;
										break;
									}
								}

								if (string.IsNullOrEmpty(objectBuilderTypeName))
									return;

								switch (objectBuilderTypeName)
								{
									case "MyObjectBuilder_CubeGrid":
										CubeGridEntity cubeGrid = new CubeGridEntity(importFile);
										SectorObjectManager.Instance.AddEntity(cubeGrid);
										break;
									default:
										break;
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.ErrorLog.WriteLine(ex);
				}
			}
		}

		protected void Command_Spawn(ChatEvent chatEvent)
		{
			ulong remoteUserId = chatEvent.remoteUserId;
			string[] commandParts = chatEvent.message.Split(' ');
			int paramCount = commandParts.Length - 1;

			if (paramCount > 1 && commandParts[1].ToLower().Equals("ship"))
			{
				if (commandParts[2].ToLower().Equals("all"))
				{
				}
				if (commandParts[2].ToLower().Equals("exports"))
				{
				}
				if (commandParts[2].ToLower().Equals("cargo"))
				{
					CargoShipManager.Instance.SpawnCargoShipGroup(remoteUserId);
				}
			}
		}

		protected void Command_Clear(ChatEvent chatEvent)
		{
			ulong remoteUserId = chatEvent.remoteUserId;
			string[] commandParts = chatEvent.message.Split(' ');
			int paramCount = commandParts.Length - 1;

			if (paramCount != 1)
				return;

			List<CubeGridEntity> cubeGrids = SectorObjectManager.Instance.GetTypedInternalData<CubeGridEntity>();
			int queueCount = 0;
			foreach (var cubeGrid in cubeGrids)
			{
				foreach (CubeBlockEntity cubeBlock in cubeGrid.CubeBlocks)
				{
					if (commandParts[1].ToLower().Equals("productionqueue") && cubeBlock is ProductionBlockEntity)
					{
						ProductionBlockEntity block = (ProductionBlockEntity)cubeBlock;
						block.ClearQueue();
						queueCount++;
					}
					if (commandParts[1].ToLower().Equals("refineryqueue") && cubeBlock is RefineryEntity)
					{
						RefineryEntity block = (RefineryEntity)cubeBlock;
						block.ClearQueue();
						queueCount++;
					}
					if (commandParts[1].ToLower().Equals("assemblerqueue") && cubeBlock is AssemblerEntity)
					{
						AssemblerEntity block = (AssemblerEntity)cubeBlock;
						block.ClearQueue();
						queueCount++;
					}
				}
			}

			SendPrivateChatMessage(remoteUserId, "Cleared the production queue of " + queueCount.ToString() + " blocks");
		}

		protected void Command_List(ChatEvent chatEvent)
		{
			ulong remoteUserId = chatEvent.remoteUserId;
			string[] commandParts = chatEvent.message.Split(' ');
			int paramCount = commandParts.Length - 1;

			if (paramCount != 1)
				return;

			if (commandParts[1].ToLower().Equals("all"))
			{
				List<BaseEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>();
				LogManager.APILog.WriteLineAndConsole("Total entities: '" + entities.Count.ToString() + "'");

				SendPrivateChatMessage(remoteUserId, "Total entities: '" + entities.Count.ToString() + "'");
			}
			if (commandParts[1].ToLower().Equals("cubegrid"))
			{
				List<CubeGridEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<CubeGridEntity>();
				LogManager.APILog.WriteLineAndConsole("Cubegrid entities: '" + entities.Count.ToString() + "'");

				SendPrivateChatMessage(remoteUserId, "Cubegrid entities: '" + entities.Count.ToString() + "'");
			}
			if (commandParts[1].ToLower().Equals("character"))
			{
				List<CharacterEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<CharacterEntity>();
				LogManager.APILog.WriteLineAndConsole("Character entities: '" + entities.Count.ToString() + "'");

				SendPrivateChatMessage(remoteUserId, "Character entities: '" + entities.Count.ToString() + "'");
			}
			if (commandParts[1].ToLower().Equals("voxelmap"))
			{
				List<VoxelMap> entities = SectorObjectManager.Instance.GetTypedInternalData<VoxelMap>();
				LogManager.APILog.WriteLineAndConsole("Voxelmap entities: '" + entities.Count.ToString() + "'");

				SendPrivateChatMessage(remoteUserId, "Voxelmap entities: '" + entities.Count.ToString() + "'");
			}
			if (commandParts[1].ToLower().Equals("meteor"))
			{
				List<Meteor> entities = SectorObjectManager.Instance.GetTypedInternalData<Meteor>();
				LogManager.APILog.WriteLineAndConsole("Meteor entities: '" + entities.Count.ToString() + "'");

				SendPrivateChatMessage(remoteUserId, "Meteor entities: '" + entities.Count.ToString() + "'");
			}
			if (commandParts[1].ToLower().Equals("floatingobject"))
			{
				List<FloatingObject> entities = SectorObjectManager.Instance.GetTypedInternalData<FloatingObject>();
				LogManager.APILog.WriteLineAndConsole("Floating object entities: '" + entities.Count.ToString() + "'");

				SendPrivateChatMessage(remoteUserId, "Floating object entities: '" + entities.Count.ToString() + "'");
			}
		}

		protected void Command_Off(ChatEvent chatEvent)
		{
			ulong remoteUserId = chatEvent.remoteUserId;
			string[] commandParts = chatEvent.message.Split(' ');
			int paramCount = commandParts.Length - 1;

			if (paramCount != 1)
				return;

			List<CubeGridEntity> cubeGrids = SectorObjectManager.Instance.GetTypedInternalData<CubeGridEntity>();
			int poweredOffCount = 0;
			foreach (var cubeGrid in cubeGrids)
			{
				foreach (CubeBlockEntity cubeBlock in cubeGrid.CubeBlocks)
				{
					if (!(cubeBlock is FunctionalBlockEntity))
						continue;

					FunctionalBlockEntity functionalBlock = (FunctionalBlockEntity)cubeBlock;

					if (commandParts[1].ToLower().Equals("all"))
					{
						functionalBlock.Enabled = false;
						poweredOffCount++;
					}
					if (commandParts[1].ToLower().Equals("production") && cubeBlock is ProductionBlockEntity)
					{
						functionalBlock.Enabled = false;
						poweredOffCount++;
					}
					if (commandParts[1].ToLower().Equals("beacon") && cubeBlock is BeaconEntity)
					{
						functionalBlock.Enabled = false;
						poweredOffCount++;
					}
					if (commandParts[1].ToLower().Equals("tools") && (cubeBlock is ShipToolBaseEntity || cubeBlock is ShipDrillEntity))
					{
						functionalBlock.Enabled = false;
						poweredOffCount++;
					}
				}
			}

			SendPrivateChatMessage(remoteUserId, "Cleared the production queue of " + poweredOffCount.ToString() + " blocks");
		}

		protected void Command_Kick(ChatEvent chatEvent)
		{
			ulong remoteUserId = chatEvent.remoteUserId;
			string[] commandParts = chatEvent.message.Split(' ');
			int paramCount = commandParts.Length - 1;

			if (paramCount != 1)
				return;

			//Get the steam id of the player
			string rawSteamId = commandParts[1];
			ulong steamId = PlayerManager.Instance.PlayerMap.GetSteamIdFromPlayerName(rawSteamId);
			if (steamId == 0)
				return;

			PlayerManager.Instance.KickPlayer(steamId);

			SendPrivateChatMessage(remoteUserId, "Kicked '" + rawSteamId + "' off of the server");
		}

		protected void Command_Ban(ChatEvent chatEvent)
		{
			ulong remoteUserId = chatEvent.remoteUserId;
			string[] commandParts = chatEvent.message.Split(' ');
			int paramCount = commandParts.Length - 1;

			if (paramCount != 1)
				return;

			//Get the steam id of the player
			string rawSteamId = commandParts[1];
			ulong steamId = PlayerManager.Instance.PlayerMap.GetSteamIdFromPlayerName(rawSteamId);
			if (steamId == 0)
				return;

			PlayerManager.Instance.BanPlayer(steamId);

			SendPrivateChatMessage(remoteUserId, "Banned '" + rawSteamId + "' and kicked them off of the server");
		}

		protected void Command_Unban(ChatEvent chatEvent)
		{
			ulong remoteUserId = chatEvent.remoteUserId;
			string[] commandParts = chatEvent.message.Split(' ');
			int paramCount = commandParts.Length - 1;

			if (paramCount != 1)
				return;

			//Get the steam id of the player
			string rawSteamId = commandParts[1];
			ulong steamId = PlayerManager.Instance.PlayerMap.GetSteamIdFromPlayerName(rawSteamId);
			if (steamId == 0)
				return;

			PlayerManager.Instance.UnBanPlayer(steamId);

			SendPrivateChatMessage(remoteUserId, "Unbanned '" + rawSteamId + "'");
		}

		#endregion

		#endregion
	}
}
