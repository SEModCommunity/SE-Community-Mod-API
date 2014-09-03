using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Server;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Entity.Sector;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;
using SEModAPIInternal.API.Utility;
using SEModAPIInternal.Support;

namespace SEModAPIExtensions.API
{
	public class BasicUnitTestManager
	{
		private static BasicUnitTestManager m_instance;

		protected BasicUnitTestManager()
		{
			m_instance = this;
		}

		public static BasicUnitTestManager Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new BasicUnitTestManager();

				return m_instance;
			}
		}

		public bool Run()
		{
			bool oldDebuggingSetting = SandboxGameAssemblyWrapper.IsDebugging;
			SandboxGameAssemblyWrapper.IsDebugging = true;

			bool result = true;
			result &= RunBaseReflectionUnitTests();
			result &= RunEntityReflectionUnitTests();
			result &= RunCubeBlockReflectionTests();

			SandboxGameAssemblyWrapper.IsDebugging = oldDebuggingSetting;

			return result;
		}

		protected bool RunBaseReflectionUnitTests()
		{
			bool result = true;

			if (!SandboxGameAssemblyWrapper.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("SandboxGameAssemblyWrapper reflection validation failed!");
			}

			if (!ServerAssemblyWrapper.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("ServerAssemblyWrapper reflection validation failed!");
			}

			if (!NetworkManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("NetworkManager reflection validation failed!");
			}

			if (!ServerNetworkManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("ServerNetworkManager reflection validation failed!");
			}

			if (!UtilityFunctions.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("UtilityFunctions reflection validation failed!");
			}

			if (!ChatManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("ChatManager reflection validation failed!");
			}

			if (!PlayerMap.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("PlayerMap reflection validation failed!");
			}

			if (!PlayerManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("PlayerManager reflection validation failed!");
			}

			if (!WorldManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("WorldManager reflection validation failed!");
			}

			if (!RadioManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("RadioManager reflection validation failed!");
			}

			if (!RadioManagerNetworkManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("RadioManagerNetworkManager reflection validation failed!");
			}

			if (!PowerManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("PowerManager reflection validation failed!");
			}

			if (!FactionsManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("FactionsManager reflection validation failed!");
			}

			if (!Faction.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("Faction reflection validation failed!");
			}

			if (!GameEntityManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("GameEntityManager reflection validation failed!");
			}

			if (result)
			{
				Console.WriteLine("All main types passed reflection unit tests!");
			}

			return result;
		}

		protected bool RunEntityReflectionUnitTests()
		{
			bool result = true;

			if (!BaseObject.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("BaseObject reflection validation failed!");
			}

			if (!BaseEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("BaseEntity reflection validation failed!");
			}

			if (!BaseEntityNetworkManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("BaseEntityNetworkManager reflection validation failed!");
			}

			if (!CubeGridEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("CubeGridEntity reflection validation failed!");
			}

			if (!CubeGridManagerManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("CubeGridManagerManager reflection validation failed!");
			}

			if (!CubeGridNetworkManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("CubeGridNetworkManager reflection validation failed!");
			}

			if (!CubeGridThrusterManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("CubeGridThrusterManager reflection validation failed!");
			}

			if (!SectorObjectManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("SectorObjectManager reflection validation failed!");
			}

			if (!CharacterEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("CharacterEntity reflection validation failed!");
			}

			if (!CharacterEntityNetworkManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("CharacterEntityNetworkManager reflection validation failed!");
			}

			if (!FloatingObject.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("FloatingObject reflection validation failed!");
			}

			if (!FloatingObjectManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("FloatingObjectManager reflection validation failed!");
			}

			if (!InventoryEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("InventoryEntity reflection validation failed!");
			}

			if (!InventoryItemEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("InventoryItemEntity reflection validation failed!");
			}

			if (!PowerProducer.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("PowerProducer reflection validation failed!");
			}

			if (!PowerReceiver.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("PowerReceiver reflection validation failed!");
			}

			if (!VoxelMap.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("VoxelMap reflection validation failed!");
			}

			if (!VoxelMapMaterialManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("VoxelMapMaterialManager reflection validation failed!");
			}

			if (result)
			{
				Console.WriteLine("All entity types passed reflection unit tests!");
			}

			return result;
		}

		protected bool RunCubeBlockReflectionTests()
		{
			bool result = true;

			if (!CubeBlockEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("CubeBlockEntity reflection validation failed!");
			}

			if (!TerminalBlockEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("TerminalBlockEntity reflection validation failed!");
			}

			if (!FunctionalBlockEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("FunctionalBlockEntity reflection validation failed!");
			}

			if (!ProductionBlockEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("ProductionBlockEntity reflection validation failed!");
			}

			if (!LightEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("LightEntity reflection validation failed!");
			}

			if (!BatteryBlockEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("BatteryBlockEntity reflection validation failed!");
			}

			if (!BatteryBlockNetworkManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("BatteryBlockNetworkManager reflection validation failed!");
			}

			if (!DoorEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("DoorEntity reflection validation failed!");
			}

			if (!GravityBaseEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("GravityBaseEntity reflection validation failed!");
			}

			if (!GravityGeneratorEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("GravityGeneratorEntity reflection validation failed!");
			}

			if (!GravitySphereEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("GravitySphereEntity reflection validation failed!");
			}

			if (!BeaconEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("BeaconEntity reflection validation failed!");
			}

			if (!AntennaEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("AntennaEntity reflection validation failed!");
			}

			if (!ThrustEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("ThrustEntity reflection validation failed!");
			}

			if (!ThrustNetworkManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("ThrustNetworkManager reflection validation failed!");
			}

			if (!GyroEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("GyroEntity reflection validation failed!");
			}

			if (!GyroNetworkManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("GyroNetworkManager reflection validation failed!");
			}

			if (!CockpitEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("CockpitEntity reflection validation failed!");
			}

			if (!CockpitNetworkManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("CockpitNetworkManager reflection validation failed!");
			}

			if (!TurretBaseEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("TurretBaseEntity reflection validation failed!");
			}

			if (!TurretNetworkManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("TurretNetworkManager reflection validation failed!");
			}

			if (!LandingGearEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("LandingGearEntity reflection validation failed!");
			}

			if (!LandingGearNetworkManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("LandingGearNetworkManager reflection validation failed!");
			}

			if (!ReactorEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("ReactorEntity reflection validation failed!");
			}

			if (!SolarPanelEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("SolarPanelEntity reflection validation failed!");
			}

			if (!SmallGatlingGunEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("SmallGatlingGunEntity reflection validation failed!");
			}

			if (!MergeBlockEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("MergeBlockEntity reflection validation failed!");
			}

			if (!PistonEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("PistonEntity reflection validation failed!");
			}

			if (!PistonNetworkManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("PistonNetworkManager reflection validation failed!");
			}

			if (!RotorEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("RotorEntity reflection validation failed!");
			}

			if (!VirtualMassEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("VirtualMassEntity reflection validation failed!");
			}

			if (!CameraBlockEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("CameraBlockEntity reflection validation failed!");
			}

			if (!OreDetectorEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("OreDetectorEntity reflection validation failed!");
			}

			if (!ButtonPanelEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("ButtonPanelEntity reflection validation failed!");
			}

			if (result)
			{
				Console.WriteLine("All block types passed reflection unit tests!");
			}

			return result;
		}
	}
}
