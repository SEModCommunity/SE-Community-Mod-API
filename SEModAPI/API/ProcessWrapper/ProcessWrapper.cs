using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace SEModAPI.API.ProcessWrapper
{
	public class ProcessWrapper
	{
		private Process m_gameProcess;
		private ProcessModule m_gameExecutable;
		private ProcessStartInfo m_gameStartInfo;

		public ProcessWrapper()
		{
		}

		public void StartGame()
		{
			try
			{
				string exePath = Path.Combine(GameInstallationInfo.GamePath, "Bin64", "SpaceEngineers.exe");

				m_gameStartInfo = new ProcessStartInfo(exePath);

				m_gameProcess = Process.Start(m_gameStartInfo);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
	}
}
