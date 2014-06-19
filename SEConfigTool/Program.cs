using System;
using System.Windows.Forms;
using SEModAPI.Support;

namespace SEConfigTool
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
		        Application.Run(new SEConfigTool());
	        }
	        catch (AutoException eEx)
	        {
				MessageBox.Show(eEx.AdditionnalInfo + "\n\r" + eEx.GetDebugString(), @"SEConfigTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
		        throw;
	        }
	        catch (Exception ex)
	        {
				MessageBox.Show(ex.ToString() , @"SEConfigTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
		        throw;
	        }
        }
    }
}
