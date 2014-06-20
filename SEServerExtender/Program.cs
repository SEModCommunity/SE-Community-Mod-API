using System;
using System.Windows.Forms;
using SEModAPI.Support;

namespace SEServerExtender
{
	static class Program
	{
		/// <summary>
		/// Main entry point of the application
		/// </summary>
		[STAThread]
		static void Main()
		{
			try
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new SEServerExtender());
			}
			catch (AutoException eEx)
			{
				MessageBox.Show(eEx.AdditionnalInfo + "\n\r" + eEx.GetDebugString(), @"SEServerExtender", MessageBoxButtons.OK, MessageBoxIcon.Error);
				throw;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), @"SEServerExtender", MessageBoxButtons.OK, MessageBoxIcon.Error);
				throw;
			}
		}
	}
}
