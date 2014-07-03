using SteamSDK;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using SEModAPIInternal.API.Server;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIExtensions.API
{
	public class ChatManager
	{
		#region "Attributes"

		private static ChatManager m_instance;

		private static List<string> m_chatMessages;
		private static bool m_chatHandlerSetup;

		public static string ChatMessageStruct = "C42525D7DE28CE4CFB44651F3D03A50D.12AEE9CB08C9FC64151B8A094D6BB668";
		public static string ChatMessageMessageField = "EDCBEBB604B287DFA90A5A46DC7AD28D";

		#endregion

		#region "Constructors and Initializers"

		protected ChatManager()
		{
			m_instance = this;

			m_chatMessages = new List<string>();
			m_chatHandlerSetup = false;

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
					if (SandboxGameAssemblyWrapper.GetInstance().IsGameStarted())
					{
						Action action = SetupChatHandlers;
						SandboxGameAssemblyWrapper.EnqueueMainGameAction(action);
					}
				}

				return m_chatMessages;
			}
		}

		#endregion

		#region "Methods"

		public void SetupChatHandlers()
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

		public void ReceiveChatMessage(ulong remoteUserId, string message, ChatEntryTypeEnum entryType)
		{
			string playerName = remoteUserId.ToString();

			if (entryType == ChatEntryTypeEnum.ChatMsg)
			{
				m_chatMessages.Add(playerName + ": " + message);
			}

			LogManager.APILog.WriteLineAndConsole("Chat - Client '" + playerName + "': " + message);
		}

		public void SendPrivateChatMessage(ulong remoteUserId, string message)
		{
			var sandboxGame = SandboxGameAssemblyWrapper.GetInstance();

			if (!sandboxGame.IsGameStarted())
				return;

			try
			{
				Type chatMessageStructType = sandboxGame.GameAssembly.GetType(ChatMessageStruct);
				FieldInfo messageField = chatMessageStructType.GetField(ChatMessageMessageField);

				Object chatMessageStruct = Activator.CreateInstance(chatMessageStructType);
				messageField.SetValue(chatMessageStruct, message);

				ServerNetworkManager.Instance.SendStruct(remoteUserId, chatMessageStruct, chatMessageStructType);

				m_chatMessages.Add("Server: " + message);
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		public void SendPublicChatMessage(string message)
		{
			var sandboxGame = SandboxGameAssemblyWrapper.GetInstance();

			if (!sandboxGame.IsGameStarted())
				return;

			try
			{
				List<ulong> connectedPlayers = ServerNetworkManager.Instance.GetConnectedPlayers();
				foreach (ulong remoteUserId in connectedPlayers)
				{
					SendPrivateChatMessage(remoteUserId, message);
				}
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
