using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using SEModAPI.API;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Server;

using VRage.Common.Utils;

namespace SEServerExtender.API
{
	public class ProcessWrapper
	{
		#region "Attributes"

		private ServerAssemblyWrapper m_serverWrapper;

		private static Thread m_runServerThread;

		private static bool m_serverRunning;
		private static string m_worldName;

		#endregion

		#region "Constructors and Initializers"

		public ProcessWrapper()
		{
			m_serverRunning = false;
		}

		public void StartGame(string worldName)
		{
			try
			{
				if (m_serverRunning)
					return;

				string basePath = Path.Combine(GameInstallationInfo.GamePath, "DedicatedServer64");
				m_serverWrapper = ServerAssemblyWrapper.GetInstance(basePath);

				m_worldName = worldName;

				MyFileSystem.Reset();

				string contentPath = Path.Combine(new FileInfo(MyFileSystem.ExePath).Directory.FullName, "Content");
				MyFileSystem.Init(contentPath, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpaceEngineersDedicated"));
				MyFileSystem.InitUserSpecific((string)null);

				m_runServerThread = new Thread(new ThreadStart(this.RunServer));
				m_runServerThread.Start();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		#endregion

		#region "Methods"

		private void RunServer()
		{
			m_serverRunning = m_serverWrapper.StartServer(m_worldName);
		}

		#endregion
	}
}
