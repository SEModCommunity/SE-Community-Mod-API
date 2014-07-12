using System;
using System.Reflection;
using System.Windows.Forms;

using SEModAPI.Support;
using SEModAPIInternal.Support;

namespace SEServerExtender
{
	static class Program
	{
		/// <summary>
		/// Main entry point of the application
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			try
			{
				bool autoStart = false;
				bool autoStop = false;
				string instanceName = "";
				foreach (string arg in args)
				{
					if (arg.Split('=').Length > 1)
					{
						string argName = arg.Split('=')[0];
						string argValue = arg.Split('=')[1];

						Console.WriteLine("Name-Value Arg: name='" + argName + "' value='" + argValue + "'");

						if (argName.ToLower().Equals("instance"))
						{
							instanceName = argValue;
						}
					}
					else
					{
						if (arg.ToLower().Equals("autostart"))
						{
							autoStart = true;
						}
						if (arg.ToLower().Equals("autostop"))
						{
							autoStop = true;
						}
					}
				}

				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new SEServerExtender(autoStart, autoStop, instanceName));
			}
			catch (AutoException eEx)
			{
				MessageBox.Show(eEx.AdditionnalInfo + "\n\r" + eEx.GetDebugString(), @"SEServerExtender", MessageBoxButtons.OK, MessageBoxIcon.Error);
				LogManager.GameLog.WriteLine(eEx);
				//throw;
			}
			catch (TargetInvocationException ex)
			{
				MessageBox.Show(ex.ToString() + "\n\r" + ex.InnerException.ToString(), @"SEServerExtender", MessageBoxButtons.OK, MessageBoxIcon.Error);
				LogManager.GameLog.WriteLine(ex);
				//throw;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), @"SEServerExtender", MessageBoxButtons.OK, MessageBoxIcon.Error);
				LogManager.GameLog.WriteLine(ex);
				//throw;
			}
		}
	}
}
