using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace SEServerExtender
{
	[RunInstaller(true)]
	public class WindowsServiceInstaller : Installer
	{
		private ServiceInstaller serviceInstaller;
		private ServiceProcessInstaller processInstaller;

		public WindowsServiceInstaller()
		{
			processInstaller = new ServiceProcessInstaller();
			serviceInstaller = new ServiceInstaller();

			processInstaller.Account = ServiceAccount.LocalSystem;
			serviceInstaller.StartType = ServiceStartMode.Manual;
			serviceInstaller.ServiceName = "SEServerExtender";
			serviceInstaller.Description = "Service for running SEServerExtender";

			Installers.Add(serviceInstaller);
			Installers.Add(processInstaller);
		}
	}
}
