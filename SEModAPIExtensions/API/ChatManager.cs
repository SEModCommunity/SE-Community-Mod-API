using SteamSDK;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using SEModAPIInternal.API.Server;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.Support;

using VRageMath;

namespace SEModAPIExtensions.API
{
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
			string playerName = remoteUserId.ToString();

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
					List<ulong> connectedPlayers = ServerNetworkManager.Instance.GetConnectedPlayers();
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

			//Delete
			if (command.Equals("/delete"))
			{
				//All cube grids
				if (paramCount > 1 && commandParts[1].ToLower().Equals("ship"))
				{
					//That have no beacon or only a beacon with no name
					if (commandParts[2].ToLower().Equals("nobeacon"))
					{
						List<BaseEntity> entities = SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>();
						List<BaseEntity> entitiesToDispose = new List<BaseEntity>();
						foreach (BaseEntity entity in entities)
						{
							if (entity is CubeGridEntity)
							{
								//Skip static cube grids
								if (((CubeGridEntity)entity).IsStatic)
									continue;

								if (entity.Name.Equals(entity.EntityId.ToString()))
								{
									entitiesToDispose.Add(entity);
								}
							}
						}

						foreach (BaseEntity entity in entitiesToDispose)
						{
							entity.Dispose();
						}
					}
				}
			}

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
			}

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
