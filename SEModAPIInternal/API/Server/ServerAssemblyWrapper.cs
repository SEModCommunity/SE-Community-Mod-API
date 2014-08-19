using Havok;

using SteamSDK;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

using Sandbox.Input;

using SysUtils.Utils;

using VRage.Common.Utils;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Server
{
	public class ServerAssemblyWrapper
	{
		#region "Attributes"

		private static ServerAssemblyWrapper m_instance;
		private static Assembly m_assembly;

		public static string DedicatedServerNamespace = "83BCBFA49B3A2A6EC1BC99583DA2D399";
		public static string DedicatedServerClass = "49BCFF86BA276A9C7C0D269C2924DE2D";

		public static string DedicatedServerStartupBaseMethod = "26A7ABEA729FAE1F24679E21470F8E98";

		#endregion

		#region "Constructors and Initializers"

		protected ServerAssemblyWrapper()
		{
			m_instance = this;

			m_assembly = Assembly.UnsafeLoadFrom("SpaceEngineersDedicated.exe");

			Console.WriteLine("Finished loading ServerAssemblyWrapper");
		}

		#endregion

		#region "Properties"

		public static ServerAssemblyWrapper Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new ServerAssemblyWrapper();

				return m_instance;
			}
		}

		public static Type InternalType
		{
			get
			{
				if(m_assembly == null)
					m_assembly = Assembly.UnsafeLoadFrom("SpaceEngineersDedicated.exe");

				Type dedicatedServerType = m_assembly.GetType(DedicatedServerNamespace + "." + DedicatedServerClass);
				return dedicatedServerType;
			}
		}

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				Type type1 = InternalType;
				if (type1 == null)
					throw new Exception("Could not find internal type for ServerAssemblyWrapper");
				bool result = true;
				result &= BaseObject.HasMethod(type1, DedicatedServerStartupBaseMethod);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		private void SteamReset()
		{
			if (SteamServerAPI.Instance != null && SteamServerAPI.Instance.GameServer != null && SteamServerAPI.Instance.GameServer.GameDescription != null)
			{
				try
				{
					SteamServerAPI.Instance.GameServer.Dispose();
				}
				catch (Exception ex)
				{
					//Do nothing
				}
			}
			if (SteamAPI.Instance != null)
			{
				SteamAPI.Instance.Dispose();
			}
		}

		private void InputReset()
		{
			try
			{
				if (MyGuiGameControlsHelpers.GetGameControlHelper(MyGameControlEnums.FORWARD) != null)
				{
					MyGuiGameControlsHelpers.UnloadContent();
				}
			}
			catch (Exception ex)
			{
				//Do nothing
			}

			if (MyInput.Static != null)
				MyInput.Static.UnloadData();
		}

		private void PhysicsReset()
		{
			try
			{
				//TODO - Find out the proper way to get Havok to clean everything up so we don't get pointer errors on the next start
				//HkBaseSystem.Quit();
			}
			catch (Exception ex)
			{
				//Do nothing for now
			}
		}

		private void Reset()
		{
			SteamReset();

			MyFileSystem.Reset();

			InputReset();

			PhysicsReset();
		}

		public bool StartServer(string instanceName = "", string overridePath = "", bool useConsole = true)
		{
			try
			{
				//Make sure the log, if running, is closed out before we begin
				if (MyLog.Default != null)
					MyLog.Default.Close();

				SandboxGameAssemblyWrapper.Instance.SetNullRender(true);
				MyFileSystem.Reset();

				//Prepare the parameters
				bool isUsingInstance = false;
				if (instanceName != "")
					isUsingInstance = true;
				object[] methodParams = new object[]
				{
					instanceName,
					overridePath,
					isUsingInstance,
					useConsole
				};

				//Start the server
				MethodInfo serverStartupMethod = InternalType.GetMethod(DedicatedServerStartupBaseMethod, BindingFlags.Static | BindingFlags.NonPublic);
				serverStartupMethod.Invoke(null, methodParams);

				return true;
			}
			catch (Win32Exception ex)
			{
				LogManager.APILog.WriteLine("Win32Exception - Server crashed");

				LogManager.APILog.WriteLine(ex);
				LogManager.APILog.WriteLine(Environment.StackTrace);
				LogManager.ErrorLog.WriteLine(ex);
				LogManager.ErrorLog.WriteLine(Environment.StackTrace);

				return false;
			}
			catch (ExternalException ex)
			{
				LogManager.APILog.WriteLine("ExternalException - Server crashed");

				LogManager.APILog.WriteLine(ex);
				LogManager.APILog.WriteLine(Environment.StackTrace);
				LogManager.ErrorLog.WriteLine(ex);
				LogManager.ErrorLog.WriteLine(Environment.StackTrace);

				return false;
			}
			catch (TargetInvocationException ex)
			{
				LogManager.APILog.WriteLine("TargetInvocationException - Server crashed");

				LogManager.APILog.WriteLine(ex);
				LogManager.APILog.WriteLine(Environment.StackTrace);
				LogManager.ErrorLog.WriteLine(ex);
				LogManager.ErrorLog.WriteLine(Environment.StackTrace);

				return false;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine("Exception - Server crashed");

				LogManager.APILog.WriteLine(ex);
				LogManager.APILog.WriteLine(Environment.StackTrace);
				LogManager.ErrorLog.WriteLine(ex);
				LogManager.ErrorLog.WriteLine(Environment.StackTrace);

				return false;
			}
		}

		public void StopServer()
		{
			try
			{
				/*
				DateTime startedEntityCleanup = DateTime.Now;
				foreach (BaseEntity entity in SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>())
				{
					entity.Dispose();
				}
				TimeSpan cleanupTime = DateTime.Now - startedEntityCleanup;
				Console.WriteLine("Took " + cleanupTime.TotalSeconds.ToString() + " seconds to clean up entities");
				*/
				Object mainGame = SandboxGameAssemblyWrapper.Instance.GetMainGameInstance();
				BaseObject.InvokeEntityMethod(mainGame, "Dispose");

				//Reset();
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
