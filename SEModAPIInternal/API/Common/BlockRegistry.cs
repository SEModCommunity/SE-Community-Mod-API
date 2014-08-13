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
			Register(typeof(MyObjectBuilder_RadioAntenna), typeof(AntennaEntity));
			Register(typeof(MyObjectBuilder_Assembler), typeof(AssemblerEntity));
			Register(typeof(MyObjectBuilder_BatteryBlock), typeof(BatteryBlockEntity));
			Register(typeof(MyObjectBuilder_Beacon), typeof(BeaconEntity));
			Register(typeof(MyObjectBuilder_CargoContainer), typeof(CargoContainerEntity));
			Register(typeof(MyObjectBuilder_Cockpit), typeof(CockpitEntity));
			Register(typeof(MyObjectBuilder_Conveyor), typeof(ConveyorBlockEntity));
			Register(typeof(MyObjectBuilder_ConveyorConnector), typeof(ConveyorTubeEntity));
			Register(typeof(MyObjectBuilder_Door), typeof(DoorEntity));
			Register(typeof(MyObjectBuilder_LargeGatlingTurret), typeof(GatlingTurretEntity));
			Register(typeof(MyObjectBuilder_GravityGenerator), typeof(GravityGeneratorEntity));
			Register(typeof(MyObjectBuilder_Gyro), typeof(GyroEntity));
			Register(typeof(MyObjectBuilder_InteriorLight), typeof(InteriorLightEntity));
			Register(typeof(MyObjectBuilder_LandingGear), typeof(LandingGearEntity));
			Register(typeof(MyObjectBuilder_MedicalRoom), typeof(MedicalRoomEntity));
			Register(typeof(MyObjectBuilder_MergeBlock), typeof(MergeBlockEntity));
			Register(typeof(MyObjectBuilder_LargeMissileTurret), typeof(MissileTurretEntity));
			Register(typeof(MyObjectBuilder_Reactor), typeof(ReactorEntity));
			Register(typeof(MyObjectBuilder_Refinery), typeof(RefineryEntity));
			Register(typeof(MyObjectBuilder_ReflectorLight), typeof(ReflectorLightEntity));
			Register(typeof(MyObjectBuilder_Drill), typeof(ShipDrillEntity));
			Register(typeof(MyObjectBuilder_ShipGrinder), typeof(ShipGrinderEntity));
			Register(typeof(MyObjectBuilder_ShipWelder), typeof(ShipWelderEntity));
			Register(typeof(MyObjectBuilder_SolarPanel), typeof(SolarPanelEntity));
			Register(typeof(MyObjectBuilder_Thrust), typeof(ThrustEntity));
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
