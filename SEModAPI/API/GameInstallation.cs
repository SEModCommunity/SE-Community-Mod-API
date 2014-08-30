using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using SEModAPI.Support;

namespace SEModAPI.API
{
	/// <summary>
	/// Class dedicated to handle of Space Engineer installation and information
	/// </summary>
	public class GameInstallationInfo
	{
		#region "Attributes"

		private static string m_GamePath;

		internal static readonly string[] CoreSpaceEngineersFiles = 
		{
			"Sandbox.Common.dll",
			"Sandbox.Common.XmlSerializers.dll",
			"VRage.Common.dll",
			"VRage.Library.dll",
			"VRage.Math.dll"
		};

		#endregion

		#region "Constructor and Initializers"

		/// <summary>
		/// <para>Create a new instance of GameInstallationInfo with the automatically detected game path.</para>
		/// <para>
		/// It is not recommanded to purely rely on this function since if the game is not found,
		/// the API will not be able to work
		/// </para>
		/// </summary>
		public GameInstallationInfo()
		{
			m_GamePath = GetGameRegistryPath();
			if (string.IsNullOrEmpty(m_GamePath))
			{
				m_GamePath = GetGameSteamPath();
				if (string.IsNullOrEmpty(m_GamePath))
				{
					m_GamePath = GetGameEXEPath();
					if (string.IsNullOrEmpty(m_GamePath))
					{
						throw new GameInstallationInfoException(GameInstallationInfoExceptionState.EmptyGamePath, "Can't find the game path");
					}
				}
					
			}

			if (!IsValidGamePath(m_GamePath))
				throw new GameInstallationInfoException(GameInstallationInfoExceptionState.BrokenGameDirectory, "The game directory is broken");
		}

		/// <summary>
		/// Create a new instance of GameInstallationInfo with the specified game location
		/// </summary>
		/// <param name="gamePath">Location of the game executable</param>
		public GameInstallationInfo(string gamePath)
		{
			m_GamePath = gamePath;
			if (string.IsNullOrEmpty(m_GamePath))
				throw new GameInstallationInfoException(GameInstallationInfoExceptionState.EmptyGamePath, "The gamePath given is empty");

			if (!IsValidGamePath(m_GamePath))
				throw new GameInstallationInfoException(GameInstallationInfoExceptionState.BrokenGameDirectory, "The gamePath provided is invalid");
		}

		#endregion

		#region "Properties"

		public static string GamePath
		{
			get { return m_GamePath; }
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Check the validity of the gamePath given, which should point on a Space Engineers valid game installation path
		/// </summary>
		/// <param name="gamePath">The path of the game installation</param>
		/// <returns>If the given gamePath is valid</returns>
		public static bool IsValidGamePath(string gamePath)
		{
			if (string.IsNullOrEmpty(gamePath))
			{
				return false;
			}

			if (!Directory.Exists(gamePath))
			{
				return false;
			}

			if (!Directory.Exists(Path.Combine(gamePath, "Content")))
			{
				return false;
			}

			if (!Directory.Exists(Path.Combine(gamePath, "DedicatedServer64")))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Looks for the Space Engineers install location in the Registry, which should return the form:
		/// "[Drive]:\[Steam Path]\steamapps\common\SpaceEngineers"
		/// </summary>
		/// <returns>The absolute path to the game installation</returns>
		public static string GetGameRegistryPath()
		{
			RegistryKey key;
			if (Environment.Is64BitProcess)
				key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 244850", false);
			else
				key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 244850", false);

			if (key != null)
			{
				string path = key.GetValue("InstallLocation") as string;
				if (Directory.Exists(path))
					return path;
				else
					return null;
			}

			return null;
		}

		/// <summary>
		/// Looks for the Steam install location in the Registry, which should return the form:
		/// "[Drive]:\[Steam Path]"
		/// </summary>
		/// <returns>Return the Steam install location, or null if not found</returns>
		public static string GetGameSteamPath()
		{
			RegistryKey key = Registry.LocalMachine.OpenSubKey(Environment.Is64BitProcess ? @"SOFTWARE\Wow6432Node\Valve\Steam" : @"SOFTWARE\Valve\Steam", false);

			if (key == null) return null;

			string steamBasePath = (string)key.GetValue("InstallPath");
			string path = Path.Combine(steamBasePath, "steamapps", "common", "spaceengineers");

			return Directory.Exists(path) ? path : null;
		}

		/// <summary>
		/// Looks for the game install by going to the parent directory of this application
		/// </summary>
		/// <returns>The parent path of this application</returns>
		public static string GetGameEXEPath()
		{
			try
			{
				string codeBase = Assembly.GetExecutingAssembly().CodeBase;
				UriBuilder uri = new UriBuilder(codeBase);
				string path = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
				DirectoryInfo directory = new DirectoryInfo(path);
				string finalPath = directory.Parent.FullName;

				return finalPath;
			}
			catch (Exception)
			{
				return "";
			}
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

		#region "Unused"
		public bool IsBaseAssembliesChanged()
		{
			// We use the Bin64 Path, as these assemblies are marked "All CPU", and will work regardless of processor architecture.
			var baseFilePath = Path.Combine(GamePath, "DedicatedServer64");
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
			// We use the Bin64 Path, as these assemblies are marked "All CPU", and will work regardless of processor architecture.
			var baseFilePath = Path.Combine(GamePath, "DedicatedServer64");
			var appFilePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

			foreach (var filename in CoreSpaceEngineersFiles)
			{
				var sourceFile = Path.Combine(baseFilePath, filename);

				if (File.Exists(sourceFile))
				{
					//File.Copy(sourceFile, Path.Combine(appFilePath, filename), true);
				}
			}

			return true;
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
					if (process != null)
					{
						process.WaitForExit();

						return process.ExitCode;
					}
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

		#endregion
	}
}
