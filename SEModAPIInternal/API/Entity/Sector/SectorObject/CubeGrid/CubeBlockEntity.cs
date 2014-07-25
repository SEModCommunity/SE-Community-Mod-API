using Microsoft.Xml.Serialization.GeneratedAssembly;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

		[Category("Cube Block")]
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

		[Category("Cube Block")]
		[ReadOnly(true)]
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

		[Category("Cube Block")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_CubeBlock ObjectBuilder
		{
			get
			{
				return (MyObjectBuilder_CubeBlock)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[Category("Cube Block")]
		[Browsable(true)]
		[ReadOnly(true)]
		[Description("The unique entity ID representing a functional entity in-game")]
		public long EntityId
		{
			get { return ObjectBuilder.EntityId; }
			set
			{
				if (ObjectBuilder.EntityId == value) return;
				ObjectBuilder.EntityId = value;

				Changed = true;
			}
		}

		[Category("Cube Block")]
		[ReadOnly(true)]
		[TypeConverter(typeof(Vector3ITypeConverter))]
		public SerializableVector3I Min
		{
			get { return ObjectBuilder.Min; }
			set
			{
				if (ObjectBuilder.Min.Equals(value)) return;
				ObjectBuilder.Min = value;
				Changed = true;
			}
		}

		[Category("Cube Block")]
		[Browsable(false)]
		[ReadOnly(true)]
		public MyBlockOrientation BlockOrientation
		{
			get { return ObjectBuilder.BlockOrientation; }
			set
			{
				if (ObjectBuilder.BlockOrientation.Equals(value)) return;
				ObjectBuilder.BlockOrientation = value;
				Changed = true;
			}
		}

		[Category("Cube Block")]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public Vector3Wrapper ColorMaskHSV
		{
			get { return ObjectBuilder.ColorMaskHSV; }
			set
			{
				if (ObjectBuilder.ColorMaskHSV.Equals(value)) return;
				ObjectBuilder.ColorMaskHSV = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateColorMaskHSV;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Cube Block")]
		public float BuildPercent
		{
			get { return ObjectBuilder.BuildPercent; }
			set
			{
				if (ObjectBuilder.BuildPercent == value) return;
				ObjectBuilder.BuildPercent = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateConstructionManager;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Cube Block")]
		public float IntegrityPercent
		{
			get { return ObjectBuilder.IntegrityPercent; }
			set
			{
				if (ObjectBuilder.IntegrityPercent == value) return;
				ObjectBuilder.IntegrityPercent = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateConstructionManager;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Cube Block")]
		public long Owner
		{
			get { return ObjectBuilder.Owner; }
			set
			{
				if (ObjectBuilder.Owner == value) return;
				ObjectBuilder.Owner = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalSetOwner;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Cube Block")]
		public MyOwnershipShareModeEnum ShareMode
		{
			get { return ObjectBuilder.ShareMode; }
			set
			{
				if (ObjectBuilder.ShareMode == value) return;
				ObjectBuilder.ShareMode = value;
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

		public override void Dispose()
		{
			m_isDisposed = true;

			if(SandboxGameAssemblyWrapper.IsDebugging)
				LogManager.APILog.WriteLine("Disposing CubeBlockEntity '" + Name + "'");

			//Only enable events for non-structural blocks, for now
			if (ObjectBuilder.EntityId != 0)
			{
				EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent();
				newEvent.type = EntityEventManager.EntityEventType.OnCubeBlockDeleted;
				newEvent.timestamp = DateTime.Now;
				newEvent.entity = this;
				newEvent.priority = 1;
				EntityEventManager.Instance.AddEvent(newEvent);
			}

			base.Dispose();
		}

		public override void Export(FileInfo fileInfo)
		{
			BaseObjectManager.SaveContentFile<MyObjectBuilder_CubeBlock, MyObjectBuilder_CubeBlockSerializer>(ObjectBuilder, fileInfo);
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

		protected void InternalUpdateConstructionManager()
		{
			try
			{
				//Update construction manager details
				Object constructionManager = GetConstructionManager();
				float maxIntegrity = (float)InvokeEntityMethod(constructionManager, ConstructionManagerGetMaxIntegrityMethod);
				float integrity = IntegrityPercent * maxIntegrity;
				float build = BuildPercent * maxIntegrity;

				InvokeEntityMethod(constructionManager, ConstructionManagerSetIntegrityBuildValuesMethod, new object[] { integrity, build });
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
				FieldInfo field = GetEntityField(BackingObject, CubeBlockColorMaskHSVField);
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
			: base(backingSource, backingSourceMethodName, InternalBackingType.Hashset)
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

		protected override bool IsValidEntity(Object entity)
		{
			try
			{
				if (entity == null)
					return false;

				return true;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return false;
			}
		}

		protected override void InternalRefreshObjectBuilderMap()
		{
			try
			{
				if (m_rawDataObjectBuilderListResourceLock.Owned)
					return;
				if (WorldManager.Instance.IsWorldSaving)
					return;
				if (WorldManager.Instance.InternalGetResourceLock() == null)
					return;
				if (WorldManager.Instance.InternalGetResourceLock().Owned)
					return;

				m_rawDataObjectBuilderListResourceLock.AcquireExclusive();

				m_rawDataObjectBuilderList.Clear();
				foreach (Object entity in GetBackingDataHashSet())
				{
					if (!IsValidEntity(entity))
						continue;

					MyObjectBuilder_CubeBlock baseEntity = (MyObjectBuilder_CubeBlock)CubeBlockEntity.InvokeEntityMethod(entity, CubeBlockEntity.CubeBlockGetObjectBuilderMethod);
					if (baseEntity == null)
						continue;

					m_rawDataObjectBuilderList.Add(entity, baseEntity);
				}

				m_rawDataObjectBuilderListResourceLock.ReleaseExclusive();
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				m_rawDataObjectBuilderListResourceLock.ReleaseExclusive();
			}
		}

		protected override void LoadDynamic()
		{
			try
			{
				Dictionary<Object, MyObjectBuilder_Base> objectBuilderList = GetObjectBuilderMap();
				HashSet<Object> rawEntities = GetBackingDataHashSet();

				if (objectBuilderList.Count != rawEntities.Count)
				{
					if (SandboxGameAssemblyWrapper.IsDebugging)
						LogManager.APILog.WriteLine("CubeBlockManager - Mismatch between raw entities and object builders");
					m_resourceLock.ReleaseExclusive();
					return;
				}

				//Update the main data mapping
				foreach (Object entity in rawEntities)
				{
					try
					{
						if (!IsValidEntity(entity))
							continue;

						if (!objectBuilderList.ContainsKey(entity))
							continue;

						MyObjectBuilder_CubeBlock baseEntity = (MyObjectBuilder_CubeBlock)objectBuilderList[entity];
						if (baseEntity == null)
							continue;

						Vector3I cubePosition = baseEntity.Min;
						long packedBlockCoordinates = (long)cubePosition.X + (long)cubePosition.Y * 10000 + (long)cubePosition.Z * 100000000;

						//If the original data already contains an entry for this, skip creation
						if (GetInternalData().ContainsKey(packedBlockCoordinates))
						{
							CubeBlockEntity matchingCubeBlock = (CubeBlockEntity)GetEntry(packedBlockCoordinates);
							if (matchingCubeBlock.IsDisposed)
								continue;

							//Update the base entity (not the same as BackingObject which is the internal object)
							matchingCubeBlock.ObjectBuilder = baseEntity;
						}
						else
						{
							CubeBlockEntity newCubeBlock = null;

							if (baseEntity.TypeId == typeof(MyObjectBuilder_CargoContainer))
								newCubeBlock = new CargoContainerEntity(m_parent, (MyObjectBuilder_CargoContainer)baseEntity, entity);
							else
							if (baseEntity.TypeId == typeof(MyObjectBuilder_Reactor))
								newCubeBlock = new ReactorEntity(m_parent, (MyObjectBuilder_Reactor)baseEntity, entity);
							else
							if (baseEntity.TypeId == typeof(MyObjectBuilder_Beacon))
								newCubeBlock = new BeaconEntity(m_parent, (MyObjectBuilder_Beacon)baseEntity, entity);
							else
							if (baseEntity.TypeId == typeof(MyObjectBuilder_Cockpit))
								newCubeBlock = new CockpitEntity(m_parent, (MyObjectBuilder_Cockpit)baseEntity, entity);
							else
							if (baseEntity.TypeId == typeof(MyObjectBuilder_GravityGenerator))
								newCubeBlock = new GravityGeneratorEntity(m_parent, (MyObjectBuilder_GravityGenerator)baseEntity, entity);
							else
							if (baseEntity.TypeId == typeof(MyObjectBuilder_MedicalRoom))
								newCubeBlock = new MedicalRoomEntity(m_parent, (MyObjectBuilder_MedicalRoom)baseEntity, entity);
							else
							if (baseEntity.TypeId == typeof(MyObjectBuilder_InteriorLight))
								newCubeBlock = new InteriorLightEntity(m_parent, (MyObjectBuilder_InteriorLight)baseEntity, entity);
							else
							if (baseEntity.TypeId == typeof(MyObjectBuilder_ReflectorLight))
								newCubeBlock = new ReflectorLightEntity(m_parent, (MyObjectBuilder_ReflectorLight)baseEntity, entity);
							else
							if (baseEntity.TypeId == typeof(MyObjectBuilder_BatteryBlock))
								newCubeBlock = new BatteryBlockEntity(m_parent, (MyObjectBuilder_BatteryBlock)baseEntity, entity);
							else
							if (baseEntity.TypeId == typeof(MyObjectBuilder_Door))
								newCubeBlock = new DoorEntity(m_parent, (MyObjectBuilder_Door)baseEntity, entity);
							else
							if (baseEntity.TypeId == typeof(MyObjectBuilder_Refinery))
								newCubeBlock = new RefineryEntity(m_parent, (MyObjectBuilder_Refinery)baseEntity, entity);
							else
							if (baseEntity.TypeId == typeof(MyObjectBuilder_Assembler))
								newCubeBlock = new AssemblerEntity(m_parent, (MyObjectBuilder_Assembler)baseEntity, entity);
							else
							if (baseEntity.TypeId == typeof(MyObjectBuilder_Thrust))
								newCubeBlock = new ThrustEntity(m_parent, (MyObjectBuilder_Thrust)baseEntity, entity);
							else
							if (baseEntity.TypeId == typeof(MyObjectBuilder_MergeBlock))
								newCubeBlock = new MergeBlockEntity(m_parent, (MyObjectBuilder_MergeBlock)baseEntity, entity);
							else
							if (baseEntity.TypeId == typeof(MyObjectBuilder_LandingGear))
								newCubeBlock = new LandingGearEntity(m_parent, (MyObjectBuilder_LandingGear)baseEntity, entity);
							else
							if (baseEntity.TypeId == typeof(MyObjectBuilder_Conveyor))
								newCubeBlock = new ConveyorBlockEntity(m_parent, (MyObjectBuilder_Conveyor)baseEntity, entity);
							else
							if (baseEntity.TypeId == typeof(MyObjectBuilder_ConveyorConnector))
								newCubeBlock = new ConveyorTubeEntity(m_parent, (MyObjectBuilder_ConveyorConnector)baseEntity, entity);
							else
							if (baseEntity.TypeId == typeof(MyObjectBuilder_SolarPanel))
								newCubeBlock = new SolarPanelEntity(m_parent, (MyObjectBuilder_SolarPanel)baseEntity, entity);
							else
							if (baseEntity.TypeId == typeof(MyObjectBuilder_Gyro))
								newCubeBlock = new GyroEntity(m_parent, (MyObjectBuilder_Gyro)baseEntity, entity);
							else
							if (baseEntity.TypeId == typeof(MyObjectBuilder_LargeGatlingTurret))
								newCubeBlock = new GatlingTurretEntity(m_parent, (MyObjectBuilder_LargeGatlingTurret)baseEntity, entity);
							else
							if (baseEntity.TypeId == typeof(MyObjectBuilder_LargeMissileTurret))
								newCubeBlock = new MissileTurretEntity(m_parent, (MyObjectBuilder_LargeMissileTurret)baseEntity, entity);
							else
							if (baseEntity.TypeId == typeof(MyObjectBuilder_ShipGrinder))
								newCubeBlock = new ShipGrinderEntity(m_parent, (MyObjectBuilder_ShipGrinder)baseEntity, entity);
							else
							if (baseEntity.TypeId == typeof(MyObjectBuilder_ShipWelder))
								newCubeBlock = new ShipWelderEntity(m_parent, (MyObjectBuilder_ShipWelder)baseEntity, entity);
							else
							if (baseEntity.TypeId == typeof(MyObjectBuilder_Drill))
								newCubeBlock = new ShipDrillEntity(m_parent, (MyObjectBuilder_Drill)baseEntity, entity);
							else
								newCubeBlock = new CubeBlockEntity(m_parent, baseEntity, entity);

							AddEntry(packedBlockCoordinates, newCubeBlock);
						}
					}
					catch (Exception ex)
					{
						LogManager.GameLog.WriteLine(ex);
					}
				}

				//Cleanup old entities
				foreach (var entry in GetInternalData())
				{
					try
					{
						if (!rawEntities.Contains(entry.Value.BackingObject))
							DeleteEntry(entry.Value);
					}
					catch (Exception ex)
					{
						LogManager.GameLog.WriteLine(ex);
					}
				}

				if(GetInternalData().Count > 0)
					m_isLoading = false;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
