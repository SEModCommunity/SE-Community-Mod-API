using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Threading;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;
using Sandbox.Game.Weapons;

using SEModAPI.API;
using SEModAPI.API.Definitions;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;
using SEModAPIInternal.API.Utility;
using SEModAPIInternal.Support;

using VRageMath;
using VRageMath.PackedVector;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid
{
	public class CubeBlockEntity : BaseObject
	{
		#region "Attributes"

		private CubeGridEntity m_parent;
		private static Type m_internalType;

		public static string CubeBlockNamespace = "6DDCED906C852CFDABA0B56B84D0BD74";
		public static string CubeBlockClass = "54A8BE425EAC4A11BFF922CFB5FF89D0";
		public static string CubeBlockGetObjectBuilderMethod = "CBB75211A3B0B3188541907C9B1B0C5C";
		public static string CubeBlockGetActualBlockMethod = "7D4CAA3CE7687B9A7D20CCF3DE6F5441";
		public static string CubeBlockDamageBlockMethod = "165EAAEA972A8C5D69F391D030C48869";
		public static string CubeBlockGetBuildPercentMethod = "BE3EB9D9351E3CB273327FB522FD60E1";
		public static string CubeBlockGetIntegrityPercentMethod = "get_Integrity";
		public static string CubeBlockParentCubeGridField = "7A975CBF89D2763F147297C064B1D764";
		public static string CubeBlockColorMaskHSVField = "80392678992D8667596D700F61290E02";
		public static string CubeBlockConstructionManagerField = "C7EFFDDD3AD38830FE93363F3327C724";

		public static string ActualCubeBlockNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string ActualCubeBlockClass = "4E262F069F7C0F85458881743E182B25";
		public static string ActualCubeBlockGetObjectBuilderMethod = "GetObjectBuilderCubeBlock";
		public static string ActualCubeBlockGetFactionsObjectMethod = "3E8AC70E5FAAA9C8C4992B71E12CDE28";
		public static string ActualCubeBlockSetFactionsDataMethod = "7161368A8164DF15904DC82476F7EBBA";

		public static string FactionsDataNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string FactionsDataClass = "0428A90CA95B1CE381A027F8E935681A";
		public static string FactionsDataOwnerField = "9A0535F68700D4E48674829975E95CAB";
		public static string FactionsDataShareModeField = "0436783F3C7FB6B04C88AB4F9097380F";

		public static string ConstructionManagerNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string ConstructionManagerClass = "2C7EEC6B76DB78A31F836F6C7B1AEC6D";
		public static string ConstructionManagerSetIntegrityBuildValuesMethod = "123634878DED2B96F018A0BA919334D6";
		public static string ConstructionManagerIntegrityValueField = "50F9175E642B77E93F3F348663C098EB";
		public static string ConstructionManagerBuildValueField = "749048B5E6D15707367DC12046920B4D";
		public static string ConstructionManagerGetMaxIntegrityMethod = "CF9727375CAA90567E5BF6CCB8D80793";

		#endregion

		#region "Constructors and Initializers"

		public CubeBlockEntity(CubeGridEntity parent, MyObjectBuilder_CubeBlock definition)
			: base(definition)
		{
			m_parent = parent;
		}

		public CubeBlockEntity(CubeGridEntity parent, MyObjectBuilder_CubeBlock definition, Object backingObject)
			: base(definition, backingObject)
		{
			m_parent = parent;

			//Only enable events for non-structural blocks, for now
			if (definition.EntityId != 0)
			{
				EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent();
				newEvent.type = EntityEventManager.EntityEventType.OnCubeBlockCreated;
				newEvent.timestamp = DateTime.Now;
				newEvent.entity = this;
				newEvent.priority = 1;
				EntityEventManager.Instance.AddEvent(newEvent);
			}
		}

		#endregion

		#region "Properties"

		[Browsable(false)]
		[ReadOnly(true)]
		internal static Type InternalType
		{
			get
			{
				if (m_internalType == null)
					m_internalType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(CubeBlockNamespace, CubeBlockClass);
				return m_internalType;
			}
		}

		[Category("Entity")]
		public override string Name
		{
			get
			{
				String name = Subtype;
				if (name == null || name == "" )
					name = TypeId.ToString();
				if (name == null || name == "")
					name = EntityId.ToString();
				if (name == null || name == "")
					name = "Cube Block";
				return name;
			}
		}

		[Category("Entity")]
		[Browsable(true)]
		[Description("The unique entity ID representing a functional entity in-game")]
		public long EntityId
		{
			get { return GetSubTypeEntity().EntityId; }
			set
			{
				if (GetSubTypeEntity().EntityId == value) return;
				GetSubTypeEntity().EntityId = value;

				Changed = true;
			}
		}

		[Category("Cube Block")]
		[ReadOnly(true)]
		[TypeConverter(typeof(Vector3ITypeConverter))]
		public SerializableVector3I Min
		{
			get { return GetSubTypeEntity().Min; }
			set
			{
				if (GetSubTypeEntity().Min.Equals(value)) return;
				GetSubTypeEntity().Min = value;
				Changed = true;
			}
		}

		[Category("Cube Block")]
		[ReadOnly(true)]
		[Browsable(false)]
		public SerializableBlockOrientation BlockOrientation
		{
			get { return GetSubTypeEntity().BlockOrientation; }
			set
			{
				if (GetSubTypeEntity().BlockOrientation.Equals(value)) return;
				GetSubTypeEntity().BlockOrientation = value;
				Changed = true;
			}
		}

		[Category("Cube Block")]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public SerializableVector3 ColorMaskHSV
		{
			get { return GetSubTypeEntity().ColorMaskHSV; }
			set
			{
				if (GetSubTypeEntity().ColorMaskHSV.Equals(value)) return;
				GetSubTypeEntity().ColorMaskHSV = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdate;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Cube Block")]
		public float BuildPercent
		{
			get { return GetSubTypeEntity().BuildPercent; }
			set
			{
				if (GetSubTypeEntity().BuildPercent == value) return;
				GetSubTypeEntity().BuildPercent = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdate;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Cube Block")]
		public float IntegrityPercent
		{
			get { return GetSubTypeEntity().IntegrityPercent; }
			set
			{
				if (GetSubTypeEntity().IntegrityPercent == value) return;
				GetSubTypeEntity().IntegrityPercent = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdate;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Cube Block")]
		public long Owner
		{
			get { return GetSubTypeEntity().Owner; }
			set
			{
				if (GetSubTypeEntity().Owner == value) return;
				GetSubTypeEntity().Owner = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalSetOwner;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Cube Block")]
		[Description("Added as of 1.037.000")]
		public MyOwnershipShareModeEnum ShareMode
		{
			get { return GetSubTypeEntity().ShareMode; }
			set
			{
				if (GetSubTypeEntity().ShareMode == value) return;
				GetSubTypeEntity().ShareMode = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalSetShareMode;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Cube Block")]
		[Browsable(false)]
		[ReadOnly(true)]
		public CubeGridEntity Parent
		{
			get { return m_parent; }
		}

		[Category("Cube Block")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal Object ActualObject
		{
			get { return GetActualObject(); }
		}

		#endregion

		#region "Methods"

		new public void Dispose()
		{
			if(SandboxGameAssemblyWrapper.IsDebugging)
				LogManager.APILog.WriteLine("Disposing CubeBlockEntity '" + Name + "'");

			//Only enable events for non-structural blocks, for now
			if (GetSubTypeEntity().EntityId != 0)
			{
				EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent();
				newEvent.type = EntityEventManager.EntityEventType.OnCubeBlockDeleted;
				newEvent.timestamp = DateTime.Now;
				newEvent.entity = this;
				newEvent.priority = 1;
				EntityEventManager.Instance.AddEvent(newEvent);
			}

			m_isDisposed = true;

			base.Dispose();
		}

		/// <summary>
		/// Generates a new in-game entity ID
		/// </summary>
		/// <returns></returns>
		public long GenerateEntityId()
		{
			return UtilityFunctions.GenerateEntityId();
		}

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_CubeBlock GetSubTypeEntity()
		{
			return (MyObjectBuilder_CubeBlock)BaseEntity;
		}

		#region "Internal"

		protected Object GetActualObject()
		{
			try
			{
				Object actualCubeObject = InvokeEntityMethod(BackingObject, CubeBlockGetActualBlockMethod);

				return actualCubeObject;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		protected Object GetParentCubeGrid()
		{
			try
			{
				FieldInfo parentGridField = BackingObject.GetType().GetField(CubeBlockParentCubeGridField);
				Object parentGrid = parentGridField.GetValue(BackingObject);

				return parentGrid;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		protected Object GetFactionData()
		{
			try
			{
				Object actualCubeObject = GetActualObject();

				Type actualType = actualCubeObject.GetType();
				while (actualType.Name != ActualCubeBlockClass && actualType.Name != "" && actualType.Name != "Object")
				{
					actualType = actualType.BaseType;
				}

				MethodInfo updateFactionsData = actualType.GetMethod(ActualCubeBlockGetFactionsObjectMethod);
				Object factionData = updateFactionsData.Invoke(actualCubeObject, new object[] { });

				return factionData;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		protected Object GetConstructionManager()
		{
			try
			{
				FieldInfo constructionManagerField = BackingObject.GetType().GetField(CubeBlockConstructionManagerField, BindingFlags.NonPublic | BindingFlags.Instance);
				Object constructionManager = constructionManagerField.GetValue(BackingObject);

				return constructionManager;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		protected void InternalSetOwner()
		{
			try
			{
				Object actualCubeObject = GetActualObject();

				Type actualType = actualCubeObject.GetType();
				while (actualType.Name != ActualCubeBlockClass && actualType.Name != "" && actualType.Name != "Object")
				{
					actualType = actualType.BaseType;
				}

				MethodInfo updateFactionsData = actualType.GetMethod(ActualCubeBlockSetFactionsDataMethod, BindingFlags.NonPublic | BindingFlags.Instance);
				updateFactionsData.Invoke(actualCubeObject, new object[] { Owner, ShareMode });

				m_parent.NetworkManager.BroadcastCubeBlockFactionData(this);
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected void InternalSetShareMode()
		{
			try
			{
				Object actualCubeObject = GetActualObject();

				Type actualType = actualCubeObject.GetType();
				while (actualType.Name != ActualCubeBlockClass && actualType.Name != "" && actualType.Name != "Object")
				{
					actualType = actualType.BaseType;
				}

				MethodInfo updateFactionsData = actualType.GetMethod(ActualCubeBlockSetFactionsDataMethod, BindingFlags.NonPublic | BindingFlags.Instance);
				updateFactionsData.Invoke(actualCubeObject, new object[] { Owner, ShareMode });

				m_parent.NetworkManager.BroadcastCubeBlockFactionData(this);
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected void InternalUpdate()
		{
			try
			{
				//Update construction manager details
				Object constructionManager = GetConstructionManager();
				MethodInfo getIntegrityMethod = constructionManager.GetType().GetMethod(ConstructionManagerGetMaxIntegrityMethod);
				float maxIntegrity = (float)getIntegrityMethod.Invoke(constructionManager, new object[] { });
				float integrity = IntegrityPercent * maxIntegrity;
				float build = BuildPercent * maxIntegrity;
				MethodInfo method = constructionManager.GetType().GetMethod(ConstructionManagerSetIntegrityBuildValuesMethod, BindingFlags.NonPublic | BindingFlags.Instance);
				method.Invoke(constructionManager, new object[] { integrity, build });

				//Update color mask
				InternalUpdateColorMaskHSV();
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected float InternalGetBuildPercent()
		{
			try
			{
				float result = (float)InvokeEntityMethod(BackingObject, CubeBlockGetBuildPercentMethod);
				return result;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return 1;
			}
		}

		protected float InternalGetIntegrityPercent()
		{
			try
			{
				float result = (float)InvokeEntityMethod(BackingObject, CubeBlockGetIntegrityPercentMethod);
				return result;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return 1;
			}
		}

		protected void InternalUpdateColorMaskHSV()
		{
			try
			{
				FieldInfo field = BackingObject.GetType().GetField(CubeBlockColorMaskHSVField);
				field.SetValue(BackingObject, (Vector3)ColorMaskHSV);
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected void InternalDamageBlock()
		{
			try
			{
				float damage = InternalGetIntegrityPercent() - IntegrityPercent;
				damage /= 2.0f - InternalGetIntegrityPercent();		//Counteracts internal damage scaling
				InvokeEntityMethod(BackingObject, CubeBlockDamageBlockMethod, new object[] { damage, MyDamageType.Environment });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#endregion

		#endregion
	}

	public class CubeBlockManager : BaseObjectManager
	{
		#region "Attributes"

		private CubeGridEntity m_parent;
		private bool m_isLoading;

		#endregion

		#region "Constructors and Initializers"

		public CubeBlockManager(CubeGridEntity parent)
		{
			m_parent = parent;
		}

		public CubeBlockManager(CubeGridEntity parent, Object backingSource, string backingSourceMethodName)
			: base(backingSource, backingSourceMethodName)
		{
			m_isLoading = true;
			m_parent = parent;
		}
		
		#endregion

		#region "Properties"

		public bool IsLoading
		{
			get { return m_isLoading; }
		}

		#endregion

		#region "Methods"

		public override void LoadDynamic()
		{
			if (IsResourceLocked)
				return;

			IsResourceLocked = true;

			HashSet<Object> rawEntities = GetBackingDataHashSet();
			Dictionary<long, BaseObject> data = GetInternalData();
			Dictionary<Object, BaseObject> backingData = GetBackingInternalData();

			//Update the main data mapping
			data.Clear();
			int entityCount = 0;
			foreach (Object entity in rawEntities)
			{
				entityCount++;

				try
				{
					MyObjectBuilder_CubeBlock baseEntity = (MyObjectBuilder_CubeBlock)CubeBlockEntity.InvokeEntityMethod(entity, CubeBlockEntity.CubeBlockGetObjectBuilderMethod);

					if (baseEntity == null)
						continue;

					Vector3I cubePosition = baseEntity.Min;
					long packedBlockCoordinates = (long)cubePosition.X + (long)cubePosition.Y * 10000 + (long)cubePosition.Z * 100000000;

					if (data.ContainsKey(packedBlockCoordinates))
						continue;

					CubeBlockEntity matchingCubeBlock = null;

					//If the original data already contains an entry for this, skip creation
					if (backingData.ContainsKey(entity))
					{
						matchingCubeBlock = (CubeBlockEntity)backingData[entity];
						if (matchingCubeBlock.IsDisposed)
							continue;

						//Update the base entity (not the same as BackingObject which is the internal object)
						matchingCubeBlock.BaseEntity = baseEntity;
					}
					else
					{
						switch (baseEntity.TypeId)
						{
							case MyObjectBuilderTypeEnum.CargoContainer:
								matchingCubeBlock = new CargoContainerEntity(m_parent, (MyObjectBuilder_CargoContainer)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.Reactor:
								matchingCubeBlock = new ReactorEntity(m_parent, (MyObjectBuilder_Reactor)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.Beacon:
								matchingCubeBlock = new BeaconEntity(m_parent, (MyObjectBuilder_Beacon)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.Cockpit:
								matchingCubeBlock = new CockpitEntity(m_parent, (MyObjectBuilder_Cockpit)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.GravityGenerator:
								matchingCubeBlock = new GravityGeneratorEntity(m_parent, (MyObjectBuilder_GravityGenerator)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.MedicalRoom:
								matchingCubeBlock = new MedicalRoomEntity(m_parent, (MyObjectBuilder_MedicalRoom)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.InteriorLight:
								matchingCubeBlock = new InteriorLightEntity(m_parent, (MyObjectBuilder_InteriorLight)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.ReflectorLight:
								matchingCubeBlock = new ReflectorLightEntity(m_parent, (MyObjectBuilder_ReflectorLight)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.BatteryBlock:
								matchingCubeBlock = new BatteryBlockEntity(m_parent, (MyObjectBuilder_BatteryBlock)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.Door:
								matchingCubeBlock = new DoorEntity(m_parent, (MyObjectBuilder_Door)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.Refinery:
								matchingCubeBlock = new RefineryEntity(m_parent, (MyObjectBuilder_Refinery)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.Assembler:
								matchingCubeBlock = new AssemblerEntity(m_parent, (MyObjectBuilder_Assembler)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.Thrust:
								matchingCubeBlock = new ThrustEntity(m_parent, (MyObjectBuilder_Thrust)baseEntity, entity);
								break;
							//case MyObjectBuilderTypeEnum.MergeBlock:
								//matchingCubeBlock = new MergeBlockEntity(m_parent, (MyObjectBuilder_MergeBlock)baseEntity, entity);
								//break;
							case MyObjectBuilderTypeEnum.LandingGear:
								matchingCubeBlock = new LandingGearEntity(m_parent, (MyObjectBuilder_LandingGear)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.Conveyor:
								matchingCubeBlock = new ConveyorBlockEntity(m_parent, (MyObjectBuilder_Conveyor)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.ConveyorConnector:
								matchingCubeBlock = new ConveyorTubeEntity(m_parent, (MyObjectBuilder_ConveyorConnector)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.SolarPanel:
								matchingCubeBlock = new SolarPanelEntity(m_parent, (MyObjectBuilder_SolarPanel)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.Gyro:
								matchingCubeBlock = new GyroEntity(m_parent, (MyObjectBuilder_Gyro)baseEntity, entity);
								break;
							//case MyObjectBuilderTypeEnum.LargeGatlingTurret:
								//matchingCubeBlock = new GatlingTurretEntity(m_parent, (MyObjectBuilder_LargeGatlingTurret)baseEntity, entity);
								//break;
							//case MyObjectBuilderTypeEnum.LargeMissileTurret:
								//matchingCubeBlock = new MissileTurretEntity(m_parent, (MyObjectBuilder_LargeMissileTurret)baseEntity, entity);
								//break;
							case MyObjectBuilderTypeEnum.ShipGrinder:
								matchingCubeBlock = new ShipGrinderEntity(m_parent, (MyObjectBuilder_ShipGrinder)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.ShipWelder:
								matchingCubeBlock = new ShipWelderEntity(m_parent, (MyObjectBuilder_ShipWelder)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.Drill:
								matchingCubeBlock = new ShipDrillEntity(m_parent, (MyObjectBuilder_Drill)baseEntity, entity);
								break;
							default:
								matchingCubeBlock = new CubeBlockEntity(m_parent, baseEntity, entity);
								break;
						}
					}

					if (matchingCubeBlock == null)
						throw new Exception("Failed to match/create cube block entity");

					data.Add(packedBlockCoordinates, matchingCubeBlock);
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
				}
			}

			//Update the backing data mapping
			foreach (var key in backingData.Keys)
			{
				var entry = backingData[key];
				if (!data.ContainsValue(entry))
				{
					entry.Dispose();
				}
			}
			backingData.Clear();
			foreach (var key in data.Keys)
			{
				var entry = data[key];
				backingData.Add(entry.BackingObject, entry);
			}

			IsResourceLocked = false;
			m_isLoading = false;
		}

		#endregion
	}
}
