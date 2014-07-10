using Microsoft.Xml.Serialization.GeneratedAssembly;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;

using Sandbox.Common;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.API;
using SEModAPI.API.Definitions;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Server;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;
using SEModAPIInternal.API.Utility;
using SEModAPIInternal.Support;

using VRageMath;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
	public class CubeGridEntity : BaseEntity
	{
		#region "Attributes"

		private CubeBlockManager m_cubeBlockManager;
		private CubeGridNetworkManager m_networkManager;
		private PowerManager m_powerManager;

		public static string CubeGridNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string CubeGridClass = "98262C3F38A1199E47F2B9338045794C";

		public static string CubeGridSetDampenersEnabledMethod = "86B66668D555E1C1B744C17D2AFA77F7";
		public static string CubeGridGetCubeBlocksHashSetMethod = "E38F3E9D7A76CD246B99F6AE91CC3E4A";
		public static string CubeGridGetPowerManagerMethod = "D92A57E3478304C8F8F780A554C6D6C4";

		public static string CubeGridIsStaticField = "";
		public static string CubeGridBlockGroupsField = "24E0633A3442A1F605F37D69F241C970";

		#endregion

		#region "Constructors and Initializers"

		public CubeGridEntity(FileInfo prefabFile)
			: base(null)
		{
			BaseEntity = BaseEntityManager.LoadContentFile<MyObjectBuilder_CubeGrid, MyObjectBuilder_CubeGridSerializer>(prefabFile);

			m_cubeBlockManager = new CubeBlockManager(this);
			List<CubeBlockEntity> cubeBlockList = new List<CubeBlockEntity>();
			foreach (var cubeBlock in ((MyObjectBuilder_CubeGrid)BaseEntity).CubeBlocks)
			{
				cubeBlockList.Add(new CubeBlockEntity(this, cubeBlock));
			}
			m_cubeBlockManager.Load(cubeBlockList.ToArray());
		}

		public CubeGridEntity(MyObjectBuilder_CubeGrid definition)
			: base(definition)
		{
			m_cubeBlockManager = new CubeBlockManager(this);
			List<CubeBlockEntity> cubeBlockList = new List<CubeBlockEntity>();
			foreach (var cubeBlock in ((MyObjectBuilder_CubeGrid)BaseEntity).CubeBlocks)
			{
				switch (cubeBlock.TypeId)
				{
					case MyObjectBuilderTypeEnum.CargoContainer:
						cubeBlockList.Add(new CargoContainerEntity(this, (MyObjectBuilder_CargoContainer)cubeBlock));
						break;
					case MyObjectBuilderTypeEnum.Reactor:
						cubeBlockList.Add(new ReactorEntity(this, (MyObjectBuilder_Reactor)cubeBlock));
						break;
					case MyObjectBuilderTypeEnum.MedicalRoom:
						cubeBlockList.Add(new MedicalRoomEntity(this, (MyObjectBuilder_MedicalRoom)cubeBlock));
						break;
					case MyObjectBuilderTypeEnum.Cockpit:
						cubeBlockList.Add(new CockpitEntity(this, (MyObjectBuilder_Cockpit)cubeBlock));
						break;
					case MyObjectBuilderTypeEnum.Beacon:
						cubeBlockList.Add(new BeaconEntity(this, (MyObjectBuilder_Beacon)cubeBlock));
						break;
					case MyObjectBuilderTypeEnum.GravityGenerator:
						cubeBlockList.Add(new GravityGeneratorEntity(this, (MyObjectBuilder_GravityGenerator)cubeBlock));
						break;
					default:
						cubeBlockList.Add(new CubeBlockEntity(this, cubeBlock));
						break;
				}
			}
			m_cubeBlockManager.Load(cubeBlockList);
		}

		public CubeGridEntity(MyObjectBuilder_CubeGrid definition, Object backingObject)
			: base(definition, backingObject)
		{
			m_cubeBlockManager = new CubeBlockManager(this, backingObject, CubeGridGetCubeBlocksHashSetMethod);
			m_cubeBlockManager.LoadDynamic();

			m_networkManager = new CubeGridNetworkManager(this);

			Object powerManager = InvokeEntityMethod(BackingObject, CubeGridGetPowerManagerMethod);
			m_powerManager = new PowerManager(powerManager);

			EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent();
			newEvent.type = EntityEventManager.EntityEventType.OnCubeGridCreated;
			newEvent.timestamp = DateTime.Now;
			newEvent.entity = this;
			newEvent.priority = 1;
			EntityEventManager.Instance.AddEvent(newEvent);

			Action action = InternalRegisterCubeGridClosedEvent;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		#endregion

		#region "Properties"

		[Category("Cube Grid")]
		[ReadOnly(true)]
		public override string Name
		{
			get
			{
				if (GetSubTypeEntity() == null)
					return "";

				string name = "";
				foreach (var cubeBlock in GetSubTypeEntity().CubeBlocks)
				{
					if (cubeBlock.TypeId == MyObjectBuilderTypeEnum.Beacon)
					{
						if (name.Length > 0)
							name += "|";
						name += ((MyObjectBuilder_Beacon)cubeBlock).CustomName;
					}
				}
				if (name.Length == 0)
					return GetSubTypeEntity().EntityId.ToString();
				else
					return name;
			}
		}

		[Category("Cube Grid")]
		[ReadOnly(true)]
		public MyCubeSize GridSizeEnum
		{
			get { return GetSubTypeEntity().GridSizeEnum; }
			set
			{
				if (GetSubTypeEntity().GridSizeEnum == value) return;
				GetSubTypeEntity().GridSizeEnum = value;
				Changed = true;
			}
		}

		[Category("Cube Grid")]
		[ReadOnly(true)]
		public bool IsStatic
		{
			get { return GetSubTypeEntity().IsStatic; }
			set
			{
				if (GetSubTypeEntity().IsStatic == value) return;
				GetSubTypeEntity().IsStatic = value;
				Changed = true;
			}
		}

		[Category("Cube Grid")]
		public bool IsDampenersEnabled
		{
			get { return GetSubTypeEntity().DampenersEnabled; }
			set
			{
				if (GetSubTypeEntity().DampenersEnabled == value) return;
				GetSubTypeEntity().DampenersEnabled = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateDampenersEnabled;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Cube Grid")]
		[Browsable(true)]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public override SerializableVector3 LinearVelocity
		{
			get { return GetSubTypeEntity().LinearVelocity; }
			set
			{
				if (GetSubTypeEntity().LinearVelocity.Equals(value)) return;
				GetSubTypeEntity().LinearVelocity = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateLinearVelocity;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Cube Grid")]
		[Browsable(true)]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public override SerializableVector3 AngularVelocity
		{
			get { return GetSubTypeEntity().AngularVelocity; }
			set
			{
				if (GetSubTypeEntity().AngularVelocity.Equals(value)) return;
				GetSubTypeEntity().AngularVelocity = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateAngularVelocity;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Cube Grid")]
		[Browsable(false)]
		public List<CubeBlockEntity> CubeBlocks
		{
			get
			{
				return m_cubeBlockManager.GetTypedInternalData<CubeBlockEntity>();
			}
		}

		[Category("Cube Grid")]
		[Browsable(false)]
		public List<BoneInfo> Skeleton
		{
			get { return GetSubTypeEntity().Skeleton; }
		}

		[Category("Cube Grid")]
		[Browsable(false)]
		public List<MyObjectBuilder_ConveyorLine> ConveyorLines
		{
			get { return GetSubTypeEntity().ConveyorLines; }
		}

		[Category("Cube Grid")]
		[Browsable(false)]
		public List<MyObjectBuilder_BlockGroup> BlockGroups
		{
			get { return GetSubTypeEntity().BlockGroups; }
		}

		[Category("Cube Grid")]
		[Browsable(false)]
		public CubeGridNetworkManager NetworkManager
		{
			get { return m_networkManager; }
		}

		[Category("Cube Grid")]
		[Browsable(false)]
		public PowerManager PowerManager
		{
			get { return m_powerManager; }
		}

		#endregion

		#region "Methods"

		new public void Dispose()
		{
			LogManager.APILog.WriteLine("Disposing CubeGridEntity '" + Name + "'");

			EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent();
			newEvent.type = EntityEventManager.EntityEventType.OnCubeGridDeleted;
			newEvent.timestamp = DateTime.Now;
			newEvent.entity = this;
			newEvent.priority = 1;
			EntityEventManager.Instance.AddEvent(newEvent);

			m_isDisposed = true;

			base.Dispose();
		}

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new public MyObjectBuilder_CubeGrid GetSubTypeEntity()
		{
			return (MyObjectBuilder_CubeGrid)BaseEntity;
		}

		public CubeBlockEntity GetCubeBlock(Vector3I cubePosition)
		{
			long packedBlockCoordinates = cubePosition.X + cubePosition.Y * 1000 + cubePosition.Z * 1000000;

			return (CubeBlockEntity)m_cubeBlockManager.GetEntry(packedBlockCoordinates);
		}

		public void RefreshCubeBlocks()
		{
			MyObjectBuilder_CubeGrid cubeGrid = (MyObjectBuilder_CubeGrid)BaseEntity;

			//Refresh the cube blocks content in the cube grid from the cube blocks manager
			cubeGrid.CubeBlocks.Clear();
			foreach (var item in m_cubeBlockManager.Definitions)
			{
				cubeGrid.CubeBlocks.Add((MyObjectBuilder_CubeBlock)item.GetSubTypeEntity());
			}
		}

		new public void Export(FileInfo fileInfo)
		{
			RefreshCubeBlocks();

			BaseEntityManager.SaveContentFile<MyObjectBuilder_CubeGrid, MyObjectBuilder_CubeGridSerializer>(GetSubTypeEntity(), fileInfo);
		}

		#region "Internal"

		public bool AddCubeGrid()
		{
			try
			{
				Action action = InternalAddCubeGrid;
				SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);

				return true;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return false;
			}
		}

		public void InternalAddCubeGrid()
		{
			try
			{
				if (SandboxGameAssemblyWrapper.IsDebugging)
					Console.WriteLine("CubeGrid '" + Name + "' is being added ...");

				Type backingType = SandboxGameAssemblyWrapper.Instance.GameAssembly.GetType(CubeGridNamespace + "." + CubeGridClass);

				//Create a blank instance of the base type
				BackingObject = Activator.CreateInstance(backingType);

				//Invoke 'Init' using the sub object of the grid which is the MyObjectBuilder_CubeGrid type
				MethodInfo initMethod = BackingObject.GetType().GetMethod("Init", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				initMethod.Invoke(BackingObject, new object[] { GetSubTypeEntity() });

				//Add the entity to the scene
				SectorObjectManager.Instance.AddEntity(BackingObject);
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		public void InternalCubeGridMovedEvent(Object entity)
		{
			try
			{
				EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent();
				newEvent.type = EntityEventManager.EntityEventType.OnCubeGridMoved;
				newEvent.timestamp = DateTime.Now;
				newEvent.entity = this;
				newEvent.priority = 9;
				EntityEventManager.Instance.AddEvent(newEvent);
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		public void InternalRegisterCubeGridClosedEvent()
		{
			try
			{
				Action<Object> action = InternalCubeGridClosedEvent;

				MethodInfo method = BackingObject.GetType().GetMethod(BaseEntityCombineOnClosedEventMethod);
				method.Invoke(BackingObject, new object[] { action });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		public void InternalCubeGridClosedEvent(Object entity)
		{
			try
			{
				if (IsDisposed)
					return;

				Dispose();
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		public void InternalUpdateDampenersEnabled()
		{
			try
			{
				InvokeEntityMethod(BackingObject, CubeGridSetDampenersEnabledMethod, new object[] { IsDampenersEnabled });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#endregion

		#endregion
	}

	public class CubeGridNetworkManager
	{
		public enum CubeGridPacketIds
		{
			CubeBlockHashSet = 14,				//..AAC558DB3CA968D0D3B965EA00DF05D4
			Packet1_2 = 15,
			Packet1_3 = 16,
			Packet1_4 = 17,
			Packet1_5 = 18,
			Packet1_6 = 19,

			Packet2_1 = 24,
			Packet2_2 = 25,
			Packet2_3 = 26,
			Packet2_4 = 27,
			Packet2_5 = 28,
			Packet2_6 = 29,

			Packet3_1 = 15262,
			Packet3_2 = 15263,
			Packet3_3 = 15264,
			Packet3_4 = 15265,
			CubeBlockFactionData = 15266,		//..090EFC311778552F418C0835D1248D60

			Packet4_1 = 15271,
		}

		#region "Attributes"

		private CubeGridEntity m_cubeGrid;
		private Object m_netManager;

		public static string CubeGridGetNetManagerMethod = "AF2DACDED0370C8DBA03A53FDA4E2C47";

		//18 Packet Types
		public static string CubeGridNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string CubeGridNetManagerClass = "E727876839B1C8FFEE302CD2A1948CDA";
		public static string CubeGridNetManagerCubeBlocksToDestroyField = "8E76EFAC4EED3B61D48795B2CD5AF989";
		public static string CubeGridNetManagerBroadcastCubeBlockFactionDataMethod = "EF45C83059E3CD5A2C5354ABB687D861";

		#endregion

		#region "Constructors and Initializers"

		public CubeGridNetworkManager(CubeGridEntity cubeGrid)
		{
			m_cubeGrid = cubeGrid;
			var entity = m_cubeGrid.BackingObject;
			m_netManager = BaseObject.InvokeEntityMethod(entity, CubeGridGetNetManagerMethod);
		}

		#endregion

		#region "Properties"

		public static Type NetManagerType
		{
			get
			{
				try
				{
					Assembly assembly = SandboxGameAssemblyWrapper.Instance.GameAssembly;
					Type type = assembly.GetType(CubeGridNetManagerNamespace + "." + CubeGridNetManagerClass);
					return type;
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
					return typeof(Object);
				}
			}
		}

		#endregion

		#region "Methods"

		public void BroadcastCubeBlockFactionData(CubeBlockEntity cubeBlock)
		{
			try
			{
				MethodInfo method = NetManagerType.GetMethod(CubeGridNetManagerBroadcastCubeBlockFactionDataMethod, BindingFlags.NonPublic | BindingFlags.Instance);
				method.Invoke(m_netManager, new object[] { cubeBlock.GetParentCubeGrid(), cubeBlock.GetActualObject(), cubeBlock.Owner, cubeBlock.ShareMode });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
