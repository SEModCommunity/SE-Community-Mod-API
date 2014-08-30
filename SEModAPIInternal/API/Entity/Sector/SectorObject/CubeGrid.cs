using Microsoft.Xml.Serialization.GeneratedAssembly;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;

using Sandbox.Common.ObjectBuilders;

using Sandbox.Definitions;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;
using SEModAPIInternal.Support;

using VRage;
using VRageMath;
using SEModAPIInternal.API.Utility;
using System.Reflection;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
	[DataContract(Name = "CubeGridEntityProxy")]
	[KnownType("KnownTypes")]
	public class CubeGridEntity : BaseEntity
	{
		#region "Attributes"

		private CubeBlockManager m_cubeBlockManager;
		private CubeGridNetworkManager m_networkManager;
		private CubeGridManagerManager m_managerManager;

		private static Type m_internalType;
		private string m_name;
		private DateTime m_lastNameRefresh;
		private DateTime m_lastBaseCubeBlockRefresh;

		private CubeBlockEntity m_cubeBlockToAddRemove;

		public static string CubeGridNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string CubeGridClass = "98262C3F38A1199E47F2B9338045794C";

		public static string CubeGridGetCubeBlocksHashSetMethod = "E38F3E9D7A76CD246B99F6AE91CC3E4A";
		public static string CubeGridAddCubeBlockMethod = "2B757AF5C8F1CC2EC5F738B54EFBDF23";
		public static string CubeGridRemoveCubeBlockMethod = "5980C21045AAAAEC22416165FF409455";
		public static string CubeGridGetManagerManagerMethod = "D17C9BE5AC3B00727F465C2305BA92CE";

		public static string CubeGridBlockGroupsField = "24E0633A3442A1F605F37D69F241C970";

		//////////////////////////////////////////////////////////////

		public static string CubeGridPackedCubeBlockClass = "904EBABB7F499A29EBFA14472321E896";

		#endregion

		#region "Constructors and Initializers"

		protected CubeGridEntity()
			: base(new MyObjectBuilder_CubeGrid())
		{
			m_cubeBlockManager = new CubeBlockManager(this);
			m_lastNameRefresh = DateTime.Now;
			m_lastBaseCubeBlockRefresh = DateTime.Now;
			m_name = "Cube Grid";
		}

		public CubeGridEntity(FileInfo prefabFile)
			: base(BaseObjectManager.LoadContentFile<MyObjectBuilder_CubeGrid, MyObjectBuilder_CubeGridSerializer>(prefabFile))
		{
			EntityId = 0;
			ObjectBuilder.EntityId = 0;
			if(ObjectBuilder.PositionAndOrientation != null)
				PositionAndOrientation = ObjectBuilder.PositionAndOrientation.GetValueOrDefault();

			m_cubeBlockManager = new CubeBlockManager(this);
			List<CubeBlockEntity> cubeBlockList = new List<CubeBlockEntity>();
			foreach (var cubeBlock in ObjectBuilder.CubeBlocks)
			{
				cubeBlock.EntityId = 0;
				cubeBlockList.Add(new CubeBlockEntity(this, cubeBlock));
			}
			m_cubeBlockManager.Load(cubeBlockList);

			m_lastNameRefresh = DateTime.Now;
			m_lastBaseCubeBlockRefresh = DateTime.Now;
			m_name = "Cube Grid";
		}

		public CubeGridEntity(MyObjectBuilder_CubeGrid definition)
			: base(definition)
		{
			m_cubeBlockManager = new CubeBlockManager(this);
			List<CubeBlockEntity> cubeBlockList = new List<CubeBlockEntity>();
			foreach (var cubeBlock in definition.CubeBlocks)
			{
				cubeBlock.EntityId = 0;
				cubeBlockList.Add(new CubeBlockEntity(this, cubeBlock));
			}
			m_cubeBlockManager.Load(cubeBlockList);

			m_lastNameRefresh = DateTime.Now;
			m_lastBaseCubeBlockRefresh = DateTime.Now;
			m_name = "Cube Grid";
		}

		public CubeGridEntity(MyObjectBuilder_CubeGrid definition, Object backingObject)
			: base(definition, backingObject)
		{
			m_cubeBlockManager = new CubeBlockManager(this, backingObject, CubeGridGetCubeBlocksHashSetMethod);
			m_cubeBlockManager.Refresh();

			m_networkManager = new CubeGridNetworkManager(this);
			m_managerManager = new CubeGridManagerManager(this, GetManagerManager());

			EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent();
			newEvent.type = EntityEventManager.EntityEventType.OnCubeGridCreated;
			newEvent.timestamp = DateTime.Now;
			newEvent.entity = this;
			newEvent.priority = 1;
			EntityEventManager.Instance.AddEvent(newEvent);

			m_lastNameRefresh = DateTime.Now;
			m_lastBaseCubeBlockRefresh = DateTime.Now;
			m_name = "Cube Grid";
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Cube Grid")]
		[Browsable(false)]
		[ReadOnly(true)]
		new internal static Type InternalType
		{
			get
			{
				if (m_internalType == null)
					m_internalType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(CubeGridNamespace, CubeGridClass);
				return m_internalType;
			}
		}

		[DataMember]
		[Category("Cube Grid")]
		[ReadOnly(true)]
		public override string Name
		{
			get
			{
				if (ObjectBuilder == null)
					return "Cube Grid";

				string name = "";
				TimeSpan timeSinceLastNameRefresh = DateTime.Now - m_lastNameRefresh;
				if (timeSinceLastNameRefresh.TotalSeconds < 1)
				{
					name = m_name;
				}
				else
				{
					m_lastNameRefresh = DateTime.Now;

					List<MyObjectBuilder_CubeBlock> blocks = new List<MyObjectBuilder_CubeBlock>(ObjectBuilder.CubeBlocks);
					foreach (var cubeBlock in blocks)
					{
						try
						{
							if (cubeBlock == null)
								continue;
							if (cubeBlock.TypeId == typeof(MyObjectBuilder_Beacon))
							{
								if (name.Length > 0)
									name += "|";

								string customName = ((MyObjectBuilder_Beacon)cubeBlock).CustomName;
								if (customName == "")
									customName = "Beacon";
								name += customName;
							}
						}
						catch (Exception ex)
						{
							LogManager.ErrorLog.WriteLine(ex);
						}
					}
				}

				if (name.Length == 0)
					name = DisplayName;

				if (name.Length == 0)
					name = ObjectBuilder.EntityId.ToString();

				m_name = name;

				return name;
			}
		}

		[DataMember]
		[Category("Cube Grid")]
		[ReadOnly(true)]
		public override string DisplayName
		{
			get { return ObjectBuilder.DisplayName; }
			set
			{
				if (ObjectBuilder.DisplayName == value) return;
				ObjectBuilder.DisplayName = value;
				Changed = true;

				base.DisplayName = value;
			}
		}

		[IgnoreDataMember]
		[Category("Cube Grid")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_CubeGrid ObjectBuilder
		{
			get
			{
				MyObjectBuilder_CubeGrid objectBuilder = (MyObjectBuilder_CubeGrid)base.ObjectBuilder;
				if (objectBuilder == null)
				{
					objectBuilder = new MyObjectBuilder_CubeGrid();
					ObjectBuilder = objectBuilder;
				}

				if (BackingObject != null)
				{
					TimeSpan timeSinceLastBaseCubeBlockRefresh = DateTime.Now - m_lastBaseCubeBlockRefresh;
					if (timeSinceLastBaseCubeBlockRefresh.TotalSeconds > 30)
					{
						m_lastBaseCubeBlockRefresh = DateTime.Now;
						RefreshBaseCubeBlocks();
					}
				}

				objectBuilder.LinearVelocity = LinearVelocity;
				objectBuilder.AngularVelocity = AngularVelocity;

				return objectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category("Cube Grid")]
		[ReadOnly(true)]
		public MyCubeSize GridSizeEnum
		{
			get { return ObjectBuilder.GridSizeEnum; }
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category("Cube Grid")]
		[ReadOnly(true)]
		public bool IsStatic
		{
			get { return ObjectBuilder.IsStatic; }
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category("Cube Grid")]
		public bool IsDampenersEnabled
		{
			get { return ObjectBuilder.DampenersEnabled; }
			set
			{
				if (ObjectBuilder.DampenersEnabled == value) return;
				ObjectBuilder.DampenersEnabled = value;
				Changed = true;

				if (ThrusterManager != null)
				{
					ThrusterManager.DampenersEnabled = value;
				}
			}
		}

		[IgnoreDataMember]
		[Category("Cube Grid")]
		[Browsable(false)]
		[ReadOnly(true)]
		public List<CubeBlockEntity> CubeBlocks
		{
			get
			{
				List<CubeBlockEntity> cubeBlocks = m_cubeBlockManager.GetTypedInternalData<CubeBlockEntity>();
				return cubeBlocks;
			}
			private set
			{
				//Do nothing!
			}
		}

		[IgnoreDataMember]
		[Category("Cube Grid")]
		[Browsable(false)]
		public List<MyObjectBuilder_CubeBlock> BaseCubeBlocks
		{
			get
			{
				List<MyObjectBuilder_CubeBlock> cubeBlocks = ObjectBuilder.CubeBlocks;
				return cubeBlocks;
			}
		}

		[IgnoreDataMember]
		[Category("Cube Grid")]
		[Browsable(false)]
		public List<BoneInfo> Skeleton
		{
			get { return ObjectBuilder.Skeleton; }
		}

		[IgnoreDataMember]
		[Category("Cube Grid")]
		[Browsable(false)]
		public List<MyObjectBuilder_ConveyorLine> ConveyorLines
		{
			get { return ObjectBuilder.ConveyorLines; }
		}

		[IgnoreDataMember]
		[Category("Cube Grid")]
		[Browsable(false)]
		public List<MyObjectBuilder_BlockGroup> BlockGroups
		{
			get { return ObjectBuilder.BlockGroups; }
		}

		[IgnoreDataMember]
		[Category("Cube Grid")]
		[Browsable(false)]
		[ReadOnly(true)]
		public CubeGridNetworkManager NetworkManager
		{
			get { return m_networkManager; }
			private set
			{
				//Do nothing!
			}
		}

		[IgnoreDataMember]
		[Category("Cube Grid")]
		[Browsable(false)]
		[ReadOnly(true)]
		public PowerManager PowerManager
		{
			get { return m_managerManager.PowerManager; }
			private set
			{
				//Do nothing!
			}
		}

		[IgnoreDataMember]
		[Category("Cube Grid")]
		[Browsable(false)]
		[ReadOnly(true)]
		public CubeGridThrusterManager ThrusterManager
		{
			get { return m_managerManager.ThrusterManager; }
			private set
			{
				//Do nothing!
			}
		}

		[IgnoreDataMember]
		[Category("Cube Grid")]
		[Browsable(false)]
		public bool IsLoading
		{
			get
			{
				bool isLoading = true;

				isLoading = isLoading && m_cubeBlockManager.IsLoading;

				return isLoading;
			}
			private set
			{
				//Do nothing!
			}
		}

		[IgnoreDataMember]
		[Category("Cube Grid")]
		[ReadOnly(true)]
		public float TotalPower
		{
			get { return PowerManager.TotalPower; }
			private set
			{
				//Do nothing!
			}
		}

		[IgnoreDataMember]
		[Category("Cube Grid")]
		[ReadOnly(true)]
		public float AvailablePower
		{
			get { return PowerManager.AvailablePower; }
			private set
			{
				//Do nothing!
			}
		}

		#endregion

		#region "Methods"

		public static List<Type> KnownTypes()
		{
			return UtilityFunctions.GetObjectBuilderTypes();
		}

		public override void Dispose()
		{
			if(SandboxGameAssemblyWrapper.IsDebugging)
				LogManager.APILog.WriteLine("Disposing CubeGridEntity '" + Name + "' ...");

			//Dispose the cube grid by disposing all of the blocks
			//This may be slow but it's reliable ... so far
			List<CubeBlockEntity> blocks = CubeBlocks;
			int blockCount = blocks.Count;
			foreach (CubeBlockEntity cubeBlock in blocks)
			{
				cubeBlock.Dispose();
			}

			if (SandboxGameAssemblyWrapper.IsDebugging)
				LogManager.APILog.WriteLine("Disposed " + blockCount.ToString() + " blocks on CubeGridEntity '" + Name + "'");

			//Broadcast the removal to the clients just to save processing time for the clients
			BaseNetworkManager.RemoveEntity();

			m_isDisposed = true;

			if (EntityId != 0)
			{
				GameEntityManager.RemoveEntity(EntityId);
			}

			EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent();
			newEvent.type = EntityEventManager.EntityEventType.OnCubeGridDeleted;
			newEvent.timestamp = DateTime.Now;
			newEvent.entity = this;
			newEvent.priority = 1;
			EntityEventManager.Instance.AddEvent(newEvent);
		}

		public override void Export(FileInfo fileInfo)
		{
			RefreshBaseCubeBlocks();

			BaseObjectManager.SaveContentFile<MyObjectBuilder_CubeGrid, MyObjectBuilder_CubeGridSerializer>(ObjectBuilder, fileInfo);
		}

		new public static bool ReflectionUnitTest()
		{
			try
			{
				Type type = InternalType;
				if (type == null)
					throw new Exception("Could not find internal type for CubeGridEntity");
				bool result = true;
				result &= HasMethod(type, CubeGridGetCubeBlocksHashSetMethod);
				result &= HasMethod(type, CubeGridAddCubeBlockMethod);
				result &= HasMethod(type, CubeGridRemoveCubeBlockMethod);
				result &= HasMethod(type, CubeGridGetManagerManagerMethod);
				result &= HasField(type, CubeGridBlockGroupsField);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine(ex);
				return false;
			}
		}

		public CubeBlockEntity GetCubeBlock(Vector3I cubePosition)
		{
			try
			{
				long packedBlockCoordinates = (long)cubePosition.X + (long)cubePosition.Y * 10000 + (long)cubePosition.Z * 100000000;
				CubeBlockEntity cubeBlock = (CubeBlockEntity)m_cubeBlockManager.GetEntry(packedBlockCoordinates);

				return cubeBlock;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		public void AddCubeBlock(CubeBlockEntity cubeBlock)
		{
			m_cubeBlockToAddRemove = cubeBlock;

			Action action = InternalAddCubeBlock;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		public void DeleteCubeBlock(CubeBlockEntity cubeBlock)
		{
			m_cubeBlockToAddRemove = cubeBlock;

			Action action = InternalRemoveCubeBlock;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		protected void RefreshBaseCubeBlocks()
		{
			MyObjectBuilder_CubeGrid cubeGrid = (MyObjectBuilder_CubeGrid)ObjectBuilder;

			//Refresh the cube blocks content in the cube grid from the cube blocks manager
			cubeGrid.CubeBlocks.Clear();
			foreach (var item in CubeBlocks)
			{
				cubeGrid.CubeBlocks.Add((MyObjectBuilder_CubeBlock)item.ObjectBuilder);
			}
		}

		#region "Internal"

		protected Object GetManagerManager()
		{
			Object result = InvokeEntityMethod(BackingObject, CubeGridGetManagerManagerMethod);
			return result;
		}

		protected void InternalCubeGridMovedEvent(Object entity)
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
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected void InternalAddCubeBlock()
		{
			if (m_cubeBlockToAddRemove == null)
				return;

			try
			{
				MyObjectBuilder_CubeBlock objectBuilder = m_cubeBlockToAddRemove.ObjectBuilder;
				MyCubeBlockDefinition blockDef = MyDefinitionManager.Static.GetCubeBlockDefinition(objectBuilder);

				NetworkManager.BroadcastAddCubeBlock(m_cubeBlockToAddRemove);

				Object result = InvokeEntityMethod(BackingObject, CubeGridAddCubeBlockMethod, new object[] { objectBuilder, true, blockDef });
				m_cubeBlockToAddRemove.BackingObject = result;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}

			m_cubeBlockToAddRemove = null;
		}

		protected void InternalRemoveCubeBlock()
		{
			if (m_cubeBlockToAddRemove == null)
				return;

			//NOTE - We don't broadcast the removal because the game internals take care of that by broadcasting the removal delta lists every frame update

			InvokeEntityMethod(BackingObject, CubeGridRemoveCubeBlockMethod, new object[] { m_cubeBlockToAddRemove.BackingObject, Type.Missing });

			m_cubeBlockToAddRemove = null;
		}

		#endregion

		#endregion
	}

	public class CubeGridManagerManager
	{
		#region "Attributes"

		private CubeGridEntity m_parent;
		private Object m_backingObject;

		private PowerManager m_powerManager;
		private CubeGridThrusterManager m_thrusterManager;

		public static string CubeGridManagerManagerNamespace = "6DDCED906C852CFDABA0B56B84D0BD74";
		public static string CubeGridManagerManagerClass = "0A0120EAD12D237F859BDAB2D84DA72B";

		public static string CubeGridManagerManagerGetPowerManagerMethod = "F05ACB25E5255DA110249186EE895C73";
		public static string CubeGridManagerManagerGetThrusterManagerMethod = "0EF76C91FA04B0B200A3F3AC155F089D";

		#endregion

		#region "Constructors and Initializers"

		public CubeGridManagerManager(CubeGridEntity parent, Object backingObject)
		{
			m_parent = parent;
			m_backingObject = backingObject;

			m_powerManager = new PowerManager(GetPowerManager());
			m_thrusterManager = new CubeGridThrusterManager(GetThrusterManager(), m_parent);
		}

		#endregion

		#region "Properties"

		public static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(CubeGridManagerManagerNamespace, CubeGridManagerManagerClass);
				return type;
			}
		}

		public Object BackingObject
		{
			get { return m_backingObject; }
		}

		public PowerManager PowerManager
		{
			get { return m_powerManager; }
		}

		public CubeGridThrusterManager ThrusterManager
		{
			get { return m_thrusterManager; }
		}

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				Type type = InternalType;
				if (type == null)
					throw new Exception("Could not find internal type for CubeGridManagerManager");
				bool result = true;
				result &= BaseObject.HasMethod(type, CubeGridManagerManagerGetPowerManagerMethod);
				result &= BaseObject.HasMethod(type, CubeGridManagerManagerGetThrusterManagerMethod);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine(ex);
				return false;
			}
		}

		private Object GetPowerManager()
		{
			Object manager = BaseObject.InvokeEntityMethod(BackingObject, CubeGridManagerManagerGetPowerManagerMethod);
			return manager;
		}

		private Object GetThrusterManager()
		{
			Object manager = BaseObject.InvokeEntityMethod(BackingObject, CubeGridManagerManagerGetThrusterManagerMethod);
			return manager;
		}

		#endregion
	}

	public class CubeGridNetworkManager
	{
		//28 Packets
		public enum CubeGridPacketIds
		{
			CubeBlockHashSet = 14,				//..AAC558DB3CA968D0D3B965EA00DF05D4
			Packet1_2 = 15,
			Packet1_3 = 16,
			CubeBlockPositionList = 17,			//..5A55EA00576BB526436F3708D1F55455
			CubeBlockRemoveLists = 18,			//..94E4EFFF7257EEC85C3D8FA0F1EC9E69
			AllPowerStatus = 19,				//..782C8DC19A883BCB6A43C3006F456A2F

			//Construction/Item packets
			CubeBlockBuildIntegrityValues = 25,	//..EF2D90F50F1E378F0495FFB906D1C6C6
			CubeBlockItemList = 26,				//..3FD479635EACD6C3047ACB77CBAB645D
			Packet2_4 = 27,
			Packet2_5 = 28,
			Packet2_6 = 29,

			Packet3_1 = 4711,
			NewCubeBlock = 4712,				//..64F0E2C1B88DAB5903379AB2206F9A43
			Packet3_3 = 4713,
			Packet3_4 = 4714,

			ThrusterOverrideVector = 11212,		//..08CDB5B2B7DD39CF2E3D29D787045D83

			ThrusterGyroForceVectors = 15262,	//..632113536EC30663C6FF30251EFE637A
			Packet5_2 = 15263,
			Packet5_3 = 15264,
			CubeBlockOrientationIsh = 15265,	//..69FB43596400BF997D806DF041F2B54D
			CubeBlockFactionData = 15266,		//..090EFC311778552F418C0835D1248D60
			CubeBlockOwnershipMode = 15267,		//..F62F6360C3B7B7D32C525D5987F70A68

			AllPowerStatus2 = 15271,			//..903CC5CD740D130E90DB6CBF79F80F4F

			HandbrakeStatus = 15275,			//..4DCFFCEE8D5BA392C7A57ACD6470D7CD
			Packet7_1 = 15276,

			Packet8_1 = 15278,
			Packet8_2 = 15279,
			Packet8_3 = 15280,
		}

		#region "Attributes"

		private CubeGridEntity m_cubeGrid;
		private Object m_netManager;

		private static bool m_isRegistered;

		public static string CubeGridGetNetManagerMethod = "AF2DACDED0370C8DBA03A53FDA4E2C47";

		//Definition
		public static string CubeGridNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string CubeGridNetManagerClass = "E727876839B1C8FFEE302CD2A1948CDA";

		//Methods
		public static string CubeGridNetManagerBroadcastCubeBlockBuildIntegrityValuesMethod = "F7C40254F25941842EA6558205FAC160";
		public static string CubeGridNetManagerBroadcastCubeBlockFactionDataMethod = "EF45C83059E3CD5A2C5354ABB687D861";
		public static string CubeGridNetManagerBroadcastCubeBlockRemoveListsMethod = "4A75379DE89606408396FDADD89822F3";
		public static string CubeGridNetManagerBroadcastAddCubeBlockMethod = "0B27B2B92323D75DF73055AD0A6DB4B6";

		//Fields
		public static string CubeGridNetManagerCubeBlocksToDestroyField = "8E76EFAC4EED3B61D48795B2CD5AF989";

		//////////////////////////////////////////////////////////////////

		public static string CubeGridIntegrityChangeEnumNamespace = CubeGridEntity.CubeGridNamespace + "." + CubeGridEntity.CubeGridClass;
		public static string CubeGridIntegrityChangeEnumClass = "55D3513B52D474C7AF161242E01FB9A9";

		#endregion

		#region "Constructors and Initializers"

		public CubeGridNetworkManager(CubeGridEntity cubeGrid)
		{
			m_cubeGrid = cubeGrid;
			var entity = m_cubeGrid.BackingObject;
			m_netManager = BaseObject.InvokeEntityMethod(entity, CubeGridGetNetManagerMethod);

			Action action = RegisterPacketHandlers;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		#endregion

		#region "Properties"

		public static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(CubeGridNetManagerNamespace, CubeGridNetManagerClass);
				return type;
			}
			private set
			{
				//Do nothing!
			}
		}

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				Type type = InternalType;
				if (type == null)
					throw new Exception("Could not find internal type for CubeGridNetworkManager");
				bool result = true;
				result &= BaseObject.HasMethod(type, CubeGridNetManagerBroadcastCubeBlockBuildIntegrityValuesMethod);
				result &= BaseObject.HasMethod(type, CubeGridNetManagerBroadcastCubeBlockFactionDataMethod);
				result &= BaseObject.HasMethod(type, CubeGridNetManagerBroadcastCubeBlockRemoveListsMethod);
				result &= BaseObject.HasMethod(type, CubeGridNetManagerBroadcastAddCubeBlockMethod);
				result &= BaseObject.HasField(type, CubeGridNetManagerCubeBlocksToDestroyField);

				Type type2 = CubeGridEntity.InternalType.GetNestedType(CubeGridIntegrityChangeEnumClass);
				if (type2 == null)
					throw new Exception("Could not find type for CubeGridNetworkManager-CubeGridIntegrityChangeEnum");

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		public void BroadcastCubeBlockFactionData(CubeBlockEntity cubeBlock)
		{
			try
			{
				BaseObject.InvokeEntityMethod(m_netManager, CubeGridNetManagerBroadcastCubeBlockFactionDataMethod, new object[] { m_cubeGrid.BackingObject, cubeBlock.ActualObject, cubeBlock.Owner, cubeBlock.ShareMode });
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		public void BroadcastCubeBlockBuildIntegrityValues(CubeBlockEntity cubeBlock)
		{
			try
			{
				Type someEnum = CubeGridEntity.InternalType.GetNestedType(CubeGridIntegrityChangeEnumClass);
				Array someEnumValues = someEnum.GetEnumValues();
				Object enumValue = someEnumValues.GetValue(0);
				BaseObject.InvokeEntityMethod(m_netManager, CubeGridNetManagerBroadcastCubeBlockBuildIntegrityValuesMethod, new object[] { cubeBlock.BackingObject, enumValue, 0L });
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		public void BroadcastCubeBlockRemoveLists()
		{
			BaseObject.InvokeEntityMethod(m_netManager, CubeGridNetManagerBroadcastCubeBlockRemoveListsMethod);
		}

		public void BroadcastAddCubeBlock(CubeBlockEntity cubeBlock)
		{
			try
			{
				Type packedStructType = CubeGridEntity.InternalType.GetNestedType(CubeGridEntity.CubeGridPackedCubeBlockClass);
				Object packedStruct = Activator.CreateInstance(packedStructType);
				MyCubeBlockDefinition def = MyDefinitionManager.Static.GetCubeBlockDefinition(cubeBlock.ObjectBuilder);

				//Set def id
				BaseObject.SetEntityFieldValue(packedStruct, "35E024D9E3B721592FB9B6FC1A1E239A", (DefinitionIdBlit)def.Id);

				//Set position
				BaseObject.SetEntityFieldValue(packedStruct, "5C3938C9B8CED1D0057CCF12F04329AB", cubeBlock.Position);

				//Set block size
				BaseObject.SetEntityFieldValue(packedStruct, "0DDB53EB9299ECC9826DF9A47E5E4F38", new Vector3UByte(def.Size));

				//Set block margins
				BaseObject.SetEntityFieldValue(packedStruct, "4045ED59A8C93DE0B41218EF2E947E55", new Vector3B(0, 0, 0));
				BaseObject.SetEntityFieldValue(packedStruct, "096897446D5BD5243D3D6E5C53CE1772", new Vector3B(0, 0, 0));

				//Set block margin scale
				BaseObject.SetEntityFieldValue(packedStruct, "E28B9725868E18B339D1E0594EF14444", new Vector3B(0, 0, 0));

				//Set orientation
				Quaternion rot;
				cubeBlock.BlockOrientation.GetQuaternion(out rot);
				BaseObject.SetEntityFieldValue(packedStruct, "F1AAFF5C8F200592F313BC7E02140A38", Base6Directions.GetForward(rot));
				BaseObject.SetEntityFieldValue(packedStruct, "E80AA7B84131E39F9F88209A109EED59", Base6Directions.GetUp(rot));

				//Set color
				BaseObject.SetEntityFieldValue(packedStruct, "556976F2528411FF5F95FC75DC13FEED", ColorExtensions.PackHSVToUint(cubeBlock.ColorMaskHSV));

				object[] parameters = {
					packedStruct,
					new HashSet<Vector3UByte>(),
					cubeBlock.EntityId,
					MyRandom.Instance.CreateRandomSeed()
				};
				BaseObject.InvokeEntityMethod(m_netManager, CubeGridNetManagerBroadcastAddCubeBlockMethod, parameters);
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected static void RegisterPacketHandlers()
		{
			try
			{
				if (m_isRegistered)
					return;

				bool result = true;

				//Skip the overrides for now until we figure out more about client controlled position packets
				/*
				Type packetType = InternalType.GetNestedType("08CDB5B2B7DD39CF2E3D29D787045D83", BindingFlags.Public | BindingFlags.NonPublic);
				MethodInfo method = typeof(CubeGridNetworkManager).GetMethod("ReceiveThrusterManagerVectorPacket", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
				result &= NetworkManager.RegisterCustomPacketHandler(PacketRegistrationType.Instance, packetType, method, InternalType);
				Type packetType2 = InternalType.GetNestedType("632113536EC30663C6FF30251EFE637A", BindingFlags.Public | BindingFlags.NonPublic);
				MethodInfo method2 = typeof(CubeGridNetworkManager).GetMethod("ReceiveThrusterGyroForceVectorPacket", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
				result &= NetworkManager.RegisterCustomPacketHandler(PacketRegistrationType.Instance, packetType2, method2, InternalType);
				*/

				if (!result)
					return;

				m_isRegistered = true;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected static void ReceiveThrusterManagerVectorPacket<T>(Object instanceNetManager, ref T packet, Object masterNetManager) where T : struct
		{
			try
			{
				//For now we ignore any inbound packets that set the positionorientation
				//This prevents the clients from having any control over the actual ship position
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected static void ReceiveThrusterGyroForceVectorPacket<T>(Object instanceNetManager, ref T packet, Object masterNetManager) where T : struct
		{
			try
			{
				//For now we ignore any inbound packets that set the positionorientation
				//This prevents the clients from having any control over the actual ship position
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		#endregion
	}

	public class CubeGridThrusterManager
	{
		#region "Attributes"

		private Object m_thrusterManager;
		private CubeGridEntity m_parent;

		private bool m_dampenersEnabled;

		public static string CubeGridThrusterManagerNamespace = "8EAF60352312606996BD8147B0A3C880";
		public static string CubeGridThrusterManagerClass = "958ADAA3423FBDC5DE98C1A32DE7258C";

		public static string CubeGridThrusterManagerGetEnabled = "51FDDFF9224B3F717EEFFEBEA5F1BAF6";
		public static string CubeGridThrusterManagerSetEnabled = "86B66668D555E1C1B744C17D2AFA77F7";
		public static string CubeGridThrusterManagerSetControlEnabled = "BC83851AFAE183711CFB864BA6F62CC6";

		#endregion

		#region "Constructors and Initializers"

		public CubeGridThrusterManager(Object thrusterManager, CubeGridEntity parent)
		{
			m_thrusterManager = thrusterManager;
			m_parent = parent;
		}

		#endregion

		#region "Properties"

		public Object BackingObject
		{
			get { return m_thrusterManager; }
		}

		public bool DampenersEnabled
		{
			get { return InternalGetDampenersEnabled(); }
			set
			{
				m_dampenersEnabled = value;

				Action action = InternalUpdateDampenersEnabled;
				SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
			}
		}

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				bool result = true;
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(CubeGridThrusterManagerNamespace, CubeGridThrusterManagerClass);
				if (type == null)
					throw new Exception("Could not find type for CubeGridThrusterManager");
				result &= BaseObject.HasMethod(type, CubeGridThrusterManagerGetEnabled);
				result &= BaseObject.HasMethod(type, CubeGridThrusterManagerSetEnabled);
				result &= BaseObject.HasMethod(type, CubeGridThrusterManagerSetControlEnabled);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine(ex);
				return false;
			}
		}

		protected bool InternalGetDampenersEnabled()
		{
			bool result = (bool)BaseObject.InvokeEntityMethod(BackingObject, CubeGridThrusterManagerGetEnabled);
			return result;
		}

		protected void InternalUpdateDampenersEnabled()
		{
			foreach (CubeBlockEntity cubeBlock in m_parent.CubeBlocks)
			{
				if (cubeBlock is CockpitEntity)
				{
					CockpitEntity cockpit = (CockpitEntity)cubeBlock;
					if (cockpit.IsPassengerSeat)
						continue;

					cockpit.NetworkManager.BroadcastDampenersStatus(m_dampenersEnabled);
					break;
				}
			}

			BaseObject.InvokeEntityMethod(BackingObject, CubeGridThrusterManagerSetEnabled, new object[] { m_dampenersEnabled });
			//BaseObject.InvokeEntityMethod(BackingObject, CubeGridThrusterManagerSetControlEnabled, new object[] { m_dampenersEnabled });
		}

		#endregion
	}
}
