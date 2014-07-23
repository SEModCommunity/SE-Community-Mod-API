using SteamSDK;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Common
{
	//PacketIdEnum Attribute: C42525D7DE28CE4CFB44651F3D03A50D.4C6398741B0F8804D769E5A2E3999E1D

	public enum PacketIds
	{
		CharacterSomething1 = 2,						//..7847F8340783A392D3C9E07718737962
		CharacterSomething2 = 3,						//..93DF818852230694BC15B90773A66870
		CharacterSomething3 = 4,						//..06F1DF314B7D765E189DFBBF84C09B00
		CharacterDamage = 5,							//..D666899DBA69CE94D9A0975FDAB80BEC
		VoxelSomething = 13,							//..8B85E2DD7354BDB570B8F448FF105601
		CharacterSomething6 = 22,						//..0375BD13880985CF806BF38D80ABE4DB
		VoxelSomething2 = 36,							//..630C381F251A7EBADB91135C8E343D17
		EntityBase = 37,								//..4F06732E7F1BD73CCF9AA12C9675A0F6
		EntityBaseSerialized = 38,						//..9163D0037A92C9B6DBF801EF5D53998E
		GravitySettings = 666,							//..4E427F5D20ED55F40EFFE0F6D0E179D8
		TurretTarget = 686,								//..DABFCC05F372B902A14FEFB1B6B889B3
		TurretRange = 687,								//..78F7C65371191C7D0BA7C46BC9F112DE
		TurretSettings = 688,							//..65BD9CBA6B21F560415090186DE389B6
		FloatingObjectPositionOrientation = 1630,		//..D5E5BAF9064D0C9A26E2BB899ED3BED8
		InventoryTransferItem = 2467,					//..5D0E63127AEA2BE91B98D448983B0647
		InventoryUpdateItemAmount = 2468,				//..8305AA2AB275DF34165B55263A6A7AA5
		EnabledConveyorSystem = 2476,					//..696B1F840A189ED6F234D7875793AF6D
		CockpitPositionOrientation = 2480,				//..8368ACD3E728CDA04FE741CDC05B1D16
		CockpitBool = 2481,								//..E59C3103AA8B11FF99C28DA074A47BBA
		CockpitAutopilotBase = 2487,					//..CCA7807F519575CCBA69CA2492CC90ED
		CharacterModelName = 4758,						//..4757FDF8DAF0EB6F0D0346ECEFD6E719
		CharacterSomething5 = 7414,						//..3BEB0A4A04463445218D632E2CD94536
		GyroPower = 7586,								//..BB19174225804BB5035228F5477D82C9
		FloatingObjectAltPositionOrientation = 10150,	//..564E654F19DA5C21E7869B4744304993
		FloatingObjectContents = 10151,					//..0008E59AE36FA0F2E7ED91037507E4E8
		ChatMessage = 13872,							//C42525D7DE28CE4CFB44651F3D03A50D.12AEE9CB08C9FC64151B8A094D6BB668
		TerminalFunctionalBlockEnabled = 15268,			//..7F2B3C2BC4F8C6F50583C135CA112213
		TerminalFunctionalBlockName = 15269,			//..721B404F9CB193B34D5353A019A57DAB
	}

	public abstract class NetworkManager
	{
		#region "Attributes"

		protected static NetworkManager m_instance;

		//This class is just a container for some basic steam game values as well as the actual network manager instance
		public static string NetworkManagerWrapperNamespace = "C42525D7DE28CE4CFB44651F3D03A50D";
		public static string NetworkManagerWrapperClass = "8920513CC2D9F0BEBCDC74DBD637049F";
		public static string NetworkManagerWrapperManagerInstanceField = "8E8199A1194065205F01051DC8B72DE7";

		//This is an abstract class that the actual network managers implement
		public static string NetworkManagerNamespace = "C42525D7DE28CE4CFB44651F3D03A50D";
		public static string NetworkManagerClass = "9CDBE03D49929CA686F49B66EE307DD7";
		public static string NetworkManagerSendStructMethod = "6D24456D3649B6393BA2AF59E656E4BF";
		public static string NetworkManagerRegisterChatReceiverMethod = "8A73057A206BFCA00EC372183441891A";
		public static string NetworkManagerInternalNetManagerField = "E863C8EAD57B154571B7A487C6A39AC6";

		public static string InternalNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string InternalNetManagerClass = "08FBF1782D25BEBDA2070CAF8CE47D72";
		public static string InternalNetManagerPacketRegistryField = "6F79877D9F8B092082EAEF8828D69F98";
		public static string InternalNetManagerSendToAllExceptMethod = "5ED378823191AF1EBAAF484B160C4CBC";
		public static string InternalNetManagerSendToAllMethod = "88BEA4C178343A1B40A23DE1A2F8E0FF";

		public static string PacketRegistryNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string PacketRegistryClass = "4D0D6F8422AC35DCF2A403F1C4B70957";
		public static string PacketRegistryTypeIdMapField = "5C5BB4D88AA04A59AB078CB70049BAC8";

		/////////////////////////////////////////////////

		//1 Packet Type
		public static string GravityGeneratorNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string GravityGeneratorNetManagerClass = "74AB413CFFB499A7945B3E3B84DC56CB";

		//2 Packet Types
		public static string TerminalFunctionalBlocksNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string TerminalFunctionalBlocksNetManagerClass = "850F199A13F4F6D5ED23E89E7F8D99CD";

		//2+ Packet Types
		public static string InventoryNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string InventoryNetManagerClass = "98C1408628C42B9F7FDB1DE7B8FAE776";

		//1 Packet Type
		public static string GyroPowerNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string GyroPowerNetManagerClass = "B4646791D7A57BE4E2EE21A1F22A4364";

		//1 Packet Type
		public static string ConveyorEnabledBlockNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string ConveyorEnabledBlockNetManagerClass = "C866709CB4D18071636E8389BEBA8508";

		//3 Packet Types
		public static string TurretNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string TurretNetManagerClass = "D29C33B546169292854A4DECCDB53895";

		//3 Packet Types
		public static string FloatingObjectNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string FloatingObjectNetManagerClass = "E97FDDC1EF9C912AA82D24410983D7E8";

		//2 Packet Types
		public static string VoxelMapNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string VoxelMapNetManagerClass = "EA51F988BB36804CAE6371053AD2602E";

		//3 Packet Types
		public static string CockpitNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string CockpitNetManagerClass = "F622141FE0D93611A749A5DB71DF471F";

		//7 Packet Types
		public static string CharacterNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string CharacterNetManagerClass = "FA70B722FFD1F55F5A5019DA2499E60B";

		#endregion

		#region "Constructors and Initializers"

		protected NetworkManager()
		{
			m_instance = this;

			Console.WriteLine("Finished loading NetworkManager");
		}

		#endregion

		#region "Properties"

		public static NetworkManager Instance
		{
			get { return m_instance; }
		}

		public Type NetworkManagerType
		{
			get
			{
				Type netManagerType = SandboxGameAssemblyWrapper.Instance.GameAssembly.GetType(NetworkManagerNamespace + "." + NetworkManagerClass);
				return netManagerType;
			}
		}

		#endregion

		#region "Methods"

		public static Object GetNetworkManager()
		{
			try
			{
				Type networkManagerWrapper = SandboxGameAssemblyWrapper.Instance.GameAssembly.GetType(NetworkManagerWrapperNamespace + "." + NetworkManagerWrapperClass);
				FieldInfo networkManagerField = networkManagerWrapper.GetField(NetworkManagerWrapperManagerInstanceField, BindingFlags.NonPublic | BindingFlags.Static);
				Object networkManager = networkManagerField.GetValue(null);

				return networkManager;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		public void SendStruct(ulong remoteUserId, Object data, Type structType)
		{
			try
			{
				MethodInfo sendStructMethod = NetworkManagerType.GetMethod(NetworkManagerSendStructMethod, BindingFlags.NonPublic | BindingFlags.Instance);

				sendStructMethod = sendStructMethod.MakeGenericMethod(structType);

				var netManager = GetNetworkManager();
				sendStructMethod.Invoke(netManager, new object[] { remoteUserId, data });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		public void RegisterChatReceiver(Action<ulong, string, ChatEntryTypeEnum> action)
		{
			try
			{
				MethodInfo registerChatHook = NetworkManagerType.GetMethod(NetworkManagerRegisterChatReceiverMethod);

				var netManager = GetNetworkManager();
				registerChatHook.Invoke(netManager, new object[] { action });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		public abstract List<ulong> GetConnectedPlayers();

		protected Object GetInternalNetManager()
		{
			try
			{
				FieldInfo internalNetManagerField = NetworkManagerType.GetField(NetworkManagerInternalNetManagerField);
				Object internalNetManager = internalNetManagerField.GetValue(GetNetworkManager());

				return internalNetManager;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		protected Object GetPacketRegistry()
		{
			try
			{
				Object internalNetManager = GetInternalNetManager();
				FieldInfo packetRegistryField = internalNetManager.GetType().GetField(InternalNetManagerPacketRegistryField);
				Object packetRegistry = packetRegistryField.GetValue(internalNetManager);

				return packetRegistry;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		public Dictionary<Type, ushort> GetRegisteredPacketTypes()
		{
			try
			{
				Object packetRegistry = GetPacketRegistry();
				FieldInfo typeIdMapField = packetRegistry.GetType().GetField(PacketRegistryTypeIdMapField, BindingFlags.NonPublic | BindingFlags.Static);
				Dictionary<Type, ushort> packetTypeIdMap = (Dictionary<Type, ushort>)typeIdMapField.GetValue(packetRegistry);

				return packetTypeIdMap;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return new Dictionary<Type, ushort>();
			}
		}

		#endregion
	}
}
