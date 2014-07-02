using SteamSDK;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Common
{
	public class ChatManager
	{
		#region "Attributes"

		private static ChatManager m_instance;

		private static List<string> m_chatMessages;
		private static bool m_chatHandlerSetup;

		#endregion

		#region "Constructors and Initializers"

		protected ChatManager()
		{
			m_instance = this;

			m_chatMessages = new List<string>();
			m_chatHandlerSetup = false;

			Console.WriteLine("Finished loading ChatManager");
		}

		public static ChatManager GetInstance()
		{
			if (m_instance == null)
			{
				m_instance = new ChatManager();
			}
			return (ChatManager)m_instance;
		}

		#endregion

		#region "Properties"

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
				var sandboxGame = SandboxGameAssemblyWrapper.GetInstance();

				if (m_chatHandlerSetup == true)
					return;

				if (sandboxGame.GetSteamInterface() == null)
					return;

				if (sandboxGame.GetServerSteamManager() == null)
					return;

				Action<ulong, string, ChatEntryTypeEnum> chatHook = ReceiveChatMessage;
				RegisterChatReceiver(chatHook);

				m_chatHandlerSetup = true;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		public void RegisterChatReceiver(Action<ulong, string, ChatEntryTypeEnum> action)
		{
			var sandboxGame = SandboxGameAssemblyWrapper.GetInstance();
			Type serverSteamManagerType = sandboxGame.GameAssembly.GetType("C42525D7DE28CE4CFB44651F3D03A50D.3B0B7A338600A7B9313DE1C3723DAD14");
			MethodInfo registerChatHook = serverSteamManagerType.BaseType.GetMethod("8A73057A206BFCA00EC372183441891A");
			registerChatHook.Invoke(sandboxGame.GetServerSteamManager(), new object[] { action });
		}

		public void ReceiveChatMessage(ulong remoteUserId, string message, ChatEntryTypeEnum entryType)
		{
			string playerName = remoteUserId.ToString();

			if (entryType == ChatEntryTypeEnum.ChatMsg)
			{
				//TODO - Find a way to do this without causing the server to freak out and crash
				/*
				try
				{
					if (SteamAPI.Instance != null)
					{
						SteamAPI api = SteamAPI.Instance;
						if (api.Friends != null)
						{
							Friends friends = api.Friends;
							if (friends.HasFriend(remoteUserId))
							{
								playerName = friends.GetPersonaName(remoteUserId);
							}
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
				}
				*/

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
				Object serverSteamManager = sandboxGame.GetServerSteamManager();
				MethodInfo sendStructMethod = serverSteamManager.GetType().BaseType.GetMethod("6D24456D3649B6393BA2AF59E656E4BF", BindingFlags.NonPublic | BindingFlags.Instance);

				Type chatMessageStructType = sandboxGame.GameAssembly.GetType("C42525D7DE28CE4CFB44651F3D03A50D.12AEE9CB08C9FC64151B8A094D6BB668");
				FieldInfo messageField = chatMessageStructType.GetField("EDCBEBB604B287DFA90A5A46DC7AD28D");

				Object chatMessageStruct = Activator.CreateInstance(chatMessageStructType);
				messageField.SetValue(chatMessageStruct, message);

				sendStructMethod = sendStructMethod.MakeGenericMethod(chatMessageStructType);

				sendStructMethod.Invoke(serverSteamManager, new object[] { remoteUserId, chatMessageStruct });

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
				List<ulong> connectedPlayers = sandboxGame.GetConnectedPlayers();
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
