using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;

namespace SEModAPIInternal.API.Common
{
	public class BlockRegistry : GameObjectRegistry
	{
		#region "Attributes"

		private static BlockRegistry m_instance;

		#endregion

		#region "Constructors and Initializers"

		protected BlockRegistry()
		{
			GameObjectCategory structural = new GameObjectCategory();
			structural.id = 1;
			structural.name = "Structural";
			GameObjectCategory containers = new GameObjectCategory();
			containers.id = 2;
			containers.name = "Containers";
			GameObjectCategory production = new GameObjectCategory();
			production.id = 3;
			production.name = "Refinement and Production";
			GameObjectCategory energy = new GameObjectCategory();
			energy.id = 4;
			energy.name = "Energy";
			GameObjectCategory conveyor = new GameObjectCategory();
			conveyor.id = 5;
			conveyor.name = "Conveyor";
			GameObjectCategory utility = new GameObjectCategory();
			utility.id = 6;
			utility.name = "Utility";
			GameObjectCategory weapons = new GameObjectCategory();
			weapons.id = 7;
			weapons.name = "Weapons";
			GameObjectCategory tools = new GameObjectCategory();
			tools.id = 8;
			tools.name = "Tools";
			GameObjectCategory lights = new GameObjectCategory();
			lights.id = 9;
			lights.name = "Lights";
			GameObjectCategory misc = new GameObjectCategory();
			misc.id = 10;
			misc.name = "Misc";

			//Main Types
			Register(typeof(MyObjectBuilder_RadioAntenna), typeof(AntennaEntity), utility);
			Register(typeof(MyObjectBuilder_Assembler), typeof(AssemblerEntity), production);
			Register(typeof(MyObjectBuilder_BatteryBlock), typeof(BatteryBlockEntity), energy);
			Register(typeof(MyObjectBuilder_Beacon), typeof(BeaconEntity), utility);
			Register(typeof(MyObjectBuilder_CargoContainer), typeof(CargoContainerEntity), containers);
			Register(typeof(MyObjectBuilder_Cockpit), typeof(CockpitEntity), utility);
			Register(typeof(MyObjectBuilder_Conveyor), typeof(ConveyorBlockEntity), conveyor);
			Register(typeof(MyObjectBuilder_ConveyorConnector), typeof(ConveyorTubeEntity), conveyor);
			Register(typeof(MyObjectBuilder_Door), typeof(DoorEntity), utility);
			Register(typeof(MyObjectBuilder_LargeGatlingTurret), typeof(GatlingTurretEntity), weapons);
			Register(typeof(MyObjectBuilder_GravityGenerator), typeof(GravityGeneratorEntity), utility);
			Register(typeof(MyObjectBuilder_GravityGeneratorSphere), typeof(GravitySphereEntity), utility);
			Register(typeof(MyObjectBuilder_Gyro), typeof(GyroEntity), utility);
			Register(typeof(MyObjectBuilder_InteriorLight), typeof(InteriorLightEntity), lights);
			Register(typeof(MyObjectBuilder_LandingGear), typeof(LandingGearEntity), utility);
			Register(typeof(MyObjectBuilder_MedicalRoom), typeof(MedicalRoomEntity), utility);
			Register(typeof(MyObjectBuilder_MergeBlock), typeof(MergeBlockEntity), utility);
			Register(typeof(MyObjectBuilder_LargeMissileTurret), typeof(MissileTurretEntity), weapons);
			Register(typeof(MyObjectBuilder_Reactor), typeof(ReactorEntity), energy);
			Register(typeof(MyObjectBuilder_Refinery), typeof(RefineryEntity), production);
			Register(typeof(MyObjectBuilder_ReflectorLight), typeof(ReflectorLightEntity), lights);
			Register(typeof(MyObjectBuilder_Drill), typeof(ShipDrillEntity), tools);
			Register(typeof(MyObjectBuilder_ShipGrinder), typeof(ShipGrinderEntity), tools);
			Register(typeof(MyObjectBuilder_ShipWelder), typeof(ShipWelderEntity), tools);
			Register(typeof(MyObjectBuilder_SolarPanel), typeof(SolarPanelEntity), energy);
			Register(typeof(MyObjectBuilder_Thrust), typeof(ThrustEntity), misc);

			//Base Types
			Register(typeof(MyObjectBuilder_CubeBlock), typeof(CubeBlockEntity), structural);
			Register(typeof(MyObjectBuilder_TerminalBlock), typeof(TerminalBlockEntity), misc);
			Register(typeof(MyObjectBuilder_FunctionalBlock), typeof(FunctionalBlockEntity), misc);
		}

		#endregion

		#region "Properties"

		public static BlockRegistry Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new BlockRegistry();

				return m_instance;
			}
		}

		#endregion

		#region "Methods"

		public override void Register(Type gameType, Type apiType)
		{
			if (apiType == null || gameType == null)
				return;
			if (!typeof(MyObjectBuilder_CubeBlock).IsAssignableFrom(gameType))
				return;
			if (!typeof(CubeBlockEntity).IsAssignableFrom(apiType))
				return;

			base.Register(gameType, apiType);
		}

		#endregion
	}
}
