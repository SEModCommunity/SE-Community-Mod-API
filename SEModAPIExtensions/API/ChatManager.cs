using SteamSDK;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Text.RegularExpressions;

using Sandbox.Common.ObjectBuilders;

using SEModAPI.API.Definitions;

using SEModAPIInternal.API.Server;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
using SEModAPIInternal.API.Utility;
using SEModAPIInternal.Support;

using VRageMath;
using VRage.Common.Utils;
using SEModAPIExtensions.API.IPC;

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
		private static bool m_chatHandlerSetup;

		private List<ChatEvent> m_chatEvents;

		public static string ChatMessageStruct = "C42525D7DE28CE4CFB44651F3D03A50D.12AEE9CB08C9FC64151B8A094D6BB668";
		public static string ChatMessageMessageField = "EDCBEBB604B287DFA90A5A46DC7AD28D";

		#endregion

		#region "Constructors and Initializers"

		protected ChatManager()
		{
			m_instance = this;

			m_chatMessages = new List<string>();
			m_chatHandlerSetup = false;
			m_chatEvents = new List<ChatEvent>();

			Uri baseAddress = new Uri(InternalService.BaseURI + "Chat/");
			ServiceHost selfHost = new ServiceHost(typeof(ChatService), baseAddress);

			try
			{
				selfHost.AddServiceEndpoint(typeof(IChatServiceContract), new WSHttpBinding(), "ChatService");
				ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
				smb.HttpGetEnabled = true;
				selfHost.Description.Behaviors.Add(smb);
				selfHost.Open();
			}
			catch (CommunicationException ex)
			{
				Console.WriteLine("An exception occurred: {0}", ex.Message);
				selfHost.Abort();
			}

			Console.WriteLine("Finished loading ChatManager");
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

		public List<ChatEvent> ChatEvents
		{
			get
			{
				List<ChatEvent> copy = new List<ChatEvent>(m_chatEvents.ToArray());
				return copy;
			}
		}

		#endregion

		#region "Methods"

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
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected Object CreateChatMessageStruct(string message)
		{
			Type chatMessageStructType = SandboxGameAssemblyWrapper.Instance.GameAssembly.GetType(ChatMessageStruct);
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
		}

		public void SendPrivateChatMessage(ulong remoteUserId, string message)
		{
			if (!SandboxGameAssemblyWrapper.Instance.IsGameStarted)
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
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		public void SendPublicChatMessage(string message)
		{
			if (!SandboxGameAssemblyWrapper.Instance.IsGameStarted)
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
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected bool ParseChatCommands(string message, ulong remoteUserId = 0)
		{
			if (remoteUserId != 0 && !SandboxGameAssemblyWrapper.Instance.IsUserAdmin(remoteUserId))
				return false;

			string[] commandParts = message.Split(' ');
			int paramCount = commandParts.Length - 1;
			if (paramCount < 0)
				return false;

			string command = commandParts[0].ToLower();
			if(command[0] != '/')
				return false;

			#region Delete Commands

			//Delete
			if (command.Equals("/delete"))
			{
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
							if (entity.Name.Equals(entity.EntityId.ToString()))
							{
								entitiesToDispose.Add(entity);
							}
						}

						foreach (BaseEntity entity in entitiesToDispose)
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
							}
						}

						foreach (BaseEntity entity in entitiesToDispose)
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
							}
						}

						foreach (BaseEntity entity in entitiesToDispose)
						{
							entity.Dispose();
						}

						SendPrivateChatMessage(remoteUserId, entitiesToDispose.Count.ToString() + " stations have been removed");
					}
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
						LogManager.GameLog.WriteLine(ex);
					}
				}
			}

			#endregion

			#region Utility Commands

			//Utility
			if (command.Equals("/tp"))
			{
				if (paramCount == 2)
				{
					string rawEntityId = commandParts[1];
					string rawPosition = commandParts[2];

					try
					{
						long entityId = long.Parse(rawEntityId);

						string[] rawCoordinateValues = rawPosition.Split(',');
						if (rawCoordinateValues.Length < 3)
							return true;

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
						LogManager.GameLog.WriteLine(ex);
					}
				}
			}
			if (command.Equals("/stop"))
			{
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

							entity.LinearVelocity = Vector3.Zero;
							entity.AngularVelocity = Vector3.Zero;

							SendPrivateChatMessage(remoteUserId, "Entity '" + entity.EntityId.ToString() + "' is no longer moving or rotating");
						}
					}
					catch (Exception ex)
					{
						LogManager.GameLog.WriteLine(ex);
					}
				}
			}
			if (command.Equals("/getid"))
			{
				if (paramCount > 0)
				{
					string entityName = commandParts[1];
					if (commandParts.Length > 2)
					{
						for (int i = 2; i < commandParts.Length; i++ )
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
			if (command.Equals("/save"))
			{
				WorldManager.Instance.SaveWorld();

				SendPrivateChatMessage(remoteUserId, "World has been saved!");
			}
			if (command.Equals("/owner"))
			{
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
						LogManager.GameLog.WriteLine(ex);
					}
				}
			}
			if (command.Equals("/export"))
			{
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
						LogManager.GameLog.WriteLine(ex);
					}
				}
			}
			if (command.Equals("/import"))
			{
				if (paramCount == 1)
				{
					string fileName = commandParts[1];
					Regex rgx = new Regex("[^a-zA-Z0-9]");
					string cleanFileName = rgx.Replace(fileName, "");

					string modPath = MyFileSystem.ModsPath;
					if(Directory.Exists(modPath))
					{
						string exportPath = Path.Combine(modPath, "Exports");
						if (Directory.Exists(exportPath))
						{
							FileInfo importFile = new FileInfo(Path.Combine(exportPath, cleanFileName));
							if (importFile.Exists)
							{
								//TODO - Find a clean way to determine what type of entity is in the file so that we can import

								SendPrivateChatMessage(remoteUserId, "Feature is not yet completed");
							}
						}
					}
				}
			}
			if (command.Equals("/spawn"))
			{
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

			#endregion

			#region Diagnostic Commands

			//Diagnostic
			if (command.Equals("/list"))
			{
				if (paramCount == 1 && commandParts[1].ToLower().Equals("all"))
				{
					List<BaseEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>();
					LogManager.APILog.WriteLineAndConsole("Total entities: '" + entities.Count.ToString() + "'");

					SendPrivateChatMessage(remoteUserId, "Total entities: '" + entities.Count.ToString() + "'");
				}
				if (paramCount == 1 && commandParts[1].ToLower().Equals("cubegrid"))
				{
					List<CubeGridEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<CubeGridEntity>();
					LogManager.APILog.WriteLineAndConsole("Cubegrid entities: '" + entities.Count.ToString() + "'");

					SendPrivateChatMessage(remoteUserId, "Cubegrid entities: '" + entities.Count.ToString() + "'");
				}
				if (paramCount == 1 && commandParts[1].ToLower().Equals("character"))
				{
					List<CharacterEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<CharacterEntity>();
					LogManager.APILog.WriteLineAndConsole("Character entities: '" + entities.Count.ToString() + "'");

					SendPrivateChatMessage(remoteUserId, "Character entities: '" + entities.Count.ToString() + "'");
				}
				if (paramCount == 1 && commandParts[1].ToLower().Equals("voxelmap"))
				{
					List<VoxelMap> entities = SectorObjectManager.Instance.GetTypedInternalData<VoxelMap>();
					LogManager.APILog.WriteLineAndConsole("Voxelmap entities: '" + entities.Count.ToString() + "'");

					SendPrivateChatMessage(remoteUserId, "Voxelmap entities: '" + entities.Count.ToString() + "'");
				}
				if (paramCount == 1 && commandParts[1].ToLower().Equals("meteor"))
				{
					List<Meteor> entities = SectorObjectManager.Instance.GetTypedInternalData<Meteor>();
					LogManager.APILog.WriteLineAndConsole("Meteor entities: '" + entities.Count.ToString() + "'");

					SendPrivateChatMessage(remoteUserId, "Meteor entities: '" + entities.Count.ToString() + "'");
				}
				if (paramCount == 1 && commandParts[1].ToLower().Equals("floatingobject"))
				{
					List<FloatingObject> entities = SectorObjectManager.Instance.GetTypedInternalData<FloatingObject>();
					LogManager.APILog.WriteLineAndConsole("Floating object entities: '" + entities.Count.ToString() + "'");

					SendPrivateChatMessage(remoteUserId, "Floating object entities: '" + entities.Count.ToString() + "'");
				}
			}

			#endregion

			return true;
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
	}
}
