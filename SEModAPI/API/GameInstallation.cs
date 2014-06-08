using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;


namespace SEModAPI
{
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class dedicated to handle of Space Engineer installation and information
    /// </summary>
    public class GameInstallationInfo
    {
        #region "Attributes"

        private string m_GamePath;
        public string GamePath { get { return m_GamePath; } }

        private string m_SteamPath;
        public string SteamPath { get { return m_SteamPath; } }

        internal static readonly string[] CoreSpaceEngineersFiles = 
        {
            "Sandbox.Common.dll",
            "Sandbox.Common.XmlSerializers.dll",
            "VRage.Common.dll",
            "VRage.Library.dll",
            "VRage.Math.dll"
        };

        #endregion

        #region "Constructor & Validators"

        public GameInstallationInfo()
        {
            m_GamePath = GetGameRegistryPath();
            if (m_GamePath == "") { throw new AutoException(new GameInstallationInfoException(GameInstallationInfoExceptionState.GameNotRegistered));}

            m_SteamPath = GetSteamPath();
            if (m_GamePath == "") { throw new AutoException(new GameInstallationInfoException(GameInstallationInfoExceptionState.SteamNotRegistered)); }

            if (!ValidateSpaceEngineersInstall()) { throw new AutoException(new GameInstallationInfoException(GameInstallationInfoExceptionState.BrokenGameDirectory)); }
        }

        /// <summary>
        /// Checks for key directory existance in the root game folder.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private bool ValidateSpaceEngineersInstall()
        {
            if (string.IsNullOrEmpty(m_GamePath)) { return false; }
            if (!Directory.Exists(m_GamePath)) return false;
            if (!Directory.Exists(Path.Combine(m_GamePath, "Bin64"))) { return false; }
            if (!Directory.Exists(Path.Combine(m_GamePath, "Content"))) { return false; }

            // Skip checking for the .exe. Not required currently.
            return true;
        }

        #endregion

        #region "Methods"
        /// <summary>
        /// Looks for the Space Engineers install location in the Registry, which should return the form:
        /// "C:\Program Files (x86)\Steam\steamapps\common\SpaceEngineers"
        /// </summary>
        /// <returns>The absolute path to the game installation</returns>
        public string GetGameRegistryPath()
        {
            RegistryKey key;
            if (Environment.Is64BitProcess)
                key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 244850", false);
            else
                key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 244850", false);

            if (key != null)
            {
                return key.GetValue("InstallLocation") as string;
            }

            // Backup check, but no choice if the above goes to pot.
            // Using the [Software\Valve\Steam\SteamPath] as a base for "\steamapps\common\SpaceEngineers", is unreliable, as the Steam Library is customizable and could be on another drive and directory.
            var steamPath = GetSteamPath();
            if (!string.IsNullOrEmpty(steamPath))
            {
                return Path.Combine(steamPath, @"SteamApps\common\SpaceEngineers");
            }

            return null;
        }

        public void GetSettings()
        {
        }

        /// <summary>
        /// Looks for the Steam install location in the Registry, which should return the form:
        /// "C:\Program Files (x86)\Steam"
        /// </summary>
        /// <returns></returns>
        public string GetSteamPath()
        {
            RegistryKey key;

            if (Environment.Is64BitProcess)
                key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Valve\Steam", false);
            else
                key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Valve\Steam", false);

            if (key != null)
            {
                return (string)key.GetValue("InstallPath");
            }

            return null;
        }

        public bool IsBaseAssembliesChanged()
        {
            // We use the Bin64 Path, as these assemblies are marked "AllCPU", and will work regardless of processor architecture.
            var baseFilePath = Path.Combine(GamePath, "Bin64");

            var appFilePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            foreach (var filename in CoreSpaceEngineersFiles)
            {
                if (DoFilesDiffer(baseFilePath, appFilePath, filename))
                    return true;
            }

            return false;
        }

        public bool UpdateBaseFiles()
        {
            // We use the Bin64 Path, as these assemblies are marked "AllCPU", and will work regardless of processor architecture.
            var baseFilePath = Path.Combine(GamePath, "Bin64");
            var appFilePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            foreach (var filename in CoreSpaceEngineersFiles)
            {
                var sourceFile = Path.Combine(baseFilePath, filename);

                if (File.Exists(sourceFile))
                {
                    File.Copy(sourceFile, Path.Combine(appFilePath, filename), true);
                }
            }

            return true;
        }

        public static bool DoFilesDiffer(string directory1, string directory2, string filename)
        {
            return DoFilesDiffer(Path.Combine(directory1, filename), Path.Combine(directory2, filename));
        }

        public static bool DoFilesDiffer(string file1, string file2)
        {
            if (File.Exists(file1) != File.Exists(file2))
                return false;

            var buffer1 = File.ReadAllBytes(file1);
            var buffer2 = File.ReadAllBytes(file2);

            if (buffer1.Length != buffer2.Length)
                return true;

            var ass1 = Assembly.Load(buffer1);
            var guid1 = ass1.ManifestModule.ModuleVersionId;

            var ass2 = Assembly.Load(buffer2);
            var guid2 = ass2.ManifestModule.ModuleVersionId;

            return guid1 != guid2;
        }


        internal static bool CheckIsRuningElevated()
        {
            var pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            return pricipal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        internal static int? RunElevated(string fileName, string arguments, bool elevate, bool waitForExit)
        {
            var processInfo = new ProcessStartInfo(fileName, arguments);

            if (elevate)
            {
                processInfo.Verb = "runas";
            }

            try
            {
                var process = Process.Start(processInfo);

                if (waitForExit)
                {
                    process.WaitForExit();

                    return process.ExitCode;
                }

                return 0;
            }
            catch (Win32Exception)
            {
                // Do nothing. Probably the user canceled the UAC window
                return null;
            }
        }

        #endregion
    }
}
