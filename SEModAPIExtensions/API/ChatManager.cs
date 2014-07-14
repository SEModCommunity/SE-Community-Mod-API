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

			if (entryType == ChatEntryTypeEnum.ChatMsg)
			{
				m_chatMessages.Add(playerName + ": " + message);
			}

			LogManager.APILog.WriteLineAndConsole("Chat - Client '" + playerName + "': " + message);

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
				Object chatMessageStruct = CreateChatMessageStruct(message);

				ServerNetworkManager.Instance.SendStruct(remoteUserId, chatMessageStruct, chatMessageStruct.GetType());

				m_chatMessages.Add("Server: " + message);

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
			if (commandParsed)
				return;

			try
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
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected bool ParseChatCommands(string message)
		{
			string[] commandParts = message.Split(' ');
			int paramCount = commandParts.Length - 1;
			if (paramCount < 1)
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
						foreach (BaseEntity entity in entities)
						{
							if (entity is CubeGridEntity)
							{
								//Skip static cube grids
								if (((CubeGridEntity)entity).IsStatic)
									continue;

								if (entity.Name.Equals(entity.EntityId.ToString()))
								{
									entity.Dispose();
								}
							}
						}
					}
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
