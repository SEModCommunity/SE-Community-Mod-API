using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEModAPIInternal.API.Common;

namespace SEModAPIInternal.API.Client
{
	public class ClientNetworkManager : NetworkManager
	{
		#region "Attributes"

		new protected static ClientNetworkManager m_instance;

		public static string ClientNetworkManagerNamespace = "C42525D7DE28CE4CFB44651F3D03A50D";
		public static string ClientNetworkManagerClass = "917D70698518C7E9DB7763365C3831D7";
		public static string ClientNetworkManagerConnectedPlayersField = "0A6298E827EEFE85ABBA7D3FA0A0EFCA";

		#endregion

		#region "Properties"

		new public static ClientNetworkManager Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new ClientNetworkManager();

				return m_instance;
			}
		}

		#endregion

		#region "Methods"

		public override List<ulong> GetConnectedPlayers()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
