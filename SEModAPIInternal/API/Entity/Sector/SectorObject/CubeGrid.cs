using Microsoft.Xml.Serialization.GeneratedAssembly;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

using Sandbox.Common;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using Sandbox.Definitions;

using SEModAPI.API;
using SEModAPI.API.Definitions;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Server;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;
using SEModAPIInternal.API.Utility;
using SEModAPIInternal.Support;

using VRage;
using VRageMath;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
	[DataContract(Name = "CubeGridEntityProxy")]
	[KnownType("KnownTypes")]
	public class CubeGridEntity : BaseEntity
	{
		#region "Attributes"

		private CubeBlockManager m_cubeBlockManager;
		private CubeGridNetworkManager m_networkManager;
		private PowerManager m_powerManager;

		private static Type m_internalType;
		private string m_name;
		private DateTime m_lastNameRefresh;
		private DateTime m_lastBaseCubeBlockRefresh;

		private CubeBlockEntity m_cubeBlockToAddRemove;

		public static string CubeGridNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string CubeGridClass = "98262C3F38A1199E47F2B9338045794C";

		public static string CubeGridGetCubeBlocksHashSetMethod = "E38F3E9D7A76CD246B99F6AE91CC3E4A";
		public static string CubeGridGetPowerManagerMethod = "D92A57E3478304C8F8F780A554C6D6C4";
		public static string CubeGridGetDampenersPowerReceiverMethod = "0D142C5CB93281BA2431FB266E8E3CA8";
		public static string CubeGridAddCubeBlockMethod = "2B757AF5C8F1CC2EC5F738B54EFBDF23";
		public static string CubeGridRemoveCubeBlockMethod = "5980C21045AAAAEC22416165FF409455";

		public static string CubeGridIsStaticField = "";
		public static string CubeGridBlockGroupsField = "24E0633A3442A1F605F37D69F241C970";

		//////////////////////////////////////////////////////////////

		public static string CubeGridDampenersPowerReceiverNamespace = "8EAF60352312606996BD8147B0A3C880";
		public static string CubeGridDampenersPowerReceiverClass = "958ADAA3423FBDC5DE98C1A32DE7258C";

		public static string CubeGridDampenersPowerReceiverGetEnabled = "51FDDFF9224B3F717EEFFEBEA5F1BAF6";
		public static string CubeGridDampenersPowerReceiverSetEnabled = "86B66668D555E1C1B744C17D2AFA77F7";

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

			Object powerManager = InvokeEntityMethod(BackingObject, CubeGridGetPowerManagerMethod);
			m_powerManager = new PowerManager(powerManager);

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
		internal static Type InternalType
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

					foreach (var cubeBlock in ObjectBuilder.CubeBlocks)
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

				if (BackingObject != null)
				{
					Action action = InternalUpdateDampenersEnabled;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
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
			get { return m_powerManager; }
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
			List<Type> types = new List<Type>();

			Assembly assembly = Assembly.GetAssembly(typeof(MyObjectBuilder_Base));
			foreach (Type type in assembly.GetTypes())
			{
				if (typeof(MyObjectBuilder_Base).IsAssignableFrom(type))
					types.Add(type);
			}
			return types;
		}

		public override void Dispose()
		{
			LogManager.APILog.WriteLine("Disposing CubeGridEntity '" + Name + "'");
			base.Dispose();
			/*
			EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent();
			newEvent.type = EntityEventManager.EntityEventType.OnCubeGridDeleted;
			newEvent.timestamp = DateTime.Now;
			newEvent.entity = this;
			newEvent.priority = 1;
			EntityEventManager.Instance.AddEvent(newEvent);*/
		}

		public override void Export(FileInfo fileInfo)
		{
			RefreshBaseCubeBlocks();

			BaseObjectManager.SaveContentFile<MyObjectBuilder_CubeGrid, MyObjectBuilder_CubeGridSerializer>(ObjectBuilder, fileInfo);
		}

		public static bool ReflectionUnitTest()
		{
			try
			{
				Type type = InternalType;
				if (type == null)
					throw new Exception("Could not find internal type for CubeGridEntity");
				bool result = true;
				result &= HasMethod(type, CubeGridGetCubeBlocksHashSetMethod);
				result &= HasMethod(type, CubeGridGetCubeBlocksHashSetMethod);
				result &= HasMethod(type, CubeGridGetCubeBlocksHashSetMethod);
				result &= HasMethod(type, CubeGridAddCubeBlockMethod);
				result &= HasMethod(type, CubeGridRemoveCubeBlockMethod);
				result &= HasField(type, CubeGridBlockGroupsField);

				Type type2 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(CubeGridDampenersPowerReceiverNamespace, CubeGridDampenersPowerReceiverClass);
				if (type2 == null)
					throw new Exception("Could not find type for CubeGridEntity-CubeGridDampenersPowerReceiver");
				result &= HasMethod(type2, CubeGridDampenersPowerReceiverGetEnabled);
				result &= HasMethod(type2, CubeGridDampenersPowerReceiverSetEnabled);

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

		protected Object GetDampenersPowerReceiver()
		{
			Object result = InvokeEntityMethod(BackingObject, CubeGridGetDampenersPowerReceiverMethod);

			return result;
		}

		protected void InternalUpdateDampenersEnabled()
		{
			InvokeEntityMethod(GetDampenersPowerReceiver(), CubeGridDampenersPowerReceiverSetEnabled, new object[] { IsDampenersEnabled });
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

			Packet4_1 = 11212,

			Packet5_1 = 15262,
			Packet5_2 = 15263,
			Packet5_3 = 15264,
			CubeBlockOrientationIsh = 15265,	//..69FB43596400BF997D806DF041F2B54D
			CubeBlockFactionData = 15266,		//..090EFC311778552F418C0835D1248D60
			CubeBlockOwnershipMode = 15267,		//..F62F6360C3B7B7D32C525D5987F70A68

			AllPowerStatus2 = 15271,			//..903CC5CD740D130E90DB6CBF79F80F4F

			ShipToggle1 = 15275,				//..4DCFFCEE8D5BA392C7A57ACD6470D7CD
			Packet7_1 = 15276,

			Packet8_1 = 15278,
			Packet8_2 = 15279,
			Packet8_3 = 15280,
		}

		#region "Attributes"

		private CubeGridEntity m_cubeGrid;
		private Object m_netManager;

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
					LogManager.ErrorLog.WriteLine(ex);
					return typeof(Object);
				}
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
				Type type = NetManagerType;
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
				//TODO - Get this working. This code was a valiant effort but still isn't working >.<

				Type packedStructType = CubeGridEntity.InternalType.GetNestedType(CubeGridEntity.CubeGridPackedCubeBlockClass);
				Object packedStruct = Activator.CreateInstance(packedStructType);
				MyCubeBlockDefinition def = MyDefinitionManager.Static.GetCubeBlockDefinition(cubeBlock.ObjectBuilder);

				//Set def id
				FieldInfo field = BaseObject.GetEntityField(packedStruct, "35E024D9E3B721592FB9B6FC1A1E239A");
				field.SetValue(packedStruct, (DefinitionIdBlit)def.Id);

				//Set position
				field = BaseObject.GetEntityField(packedStruct, "5C3938C9B8CED1D0057CCF12F04329AB");
				field.SetValue(packedStruct, (Vector3I)cubeBlock.Min);

				//Set block size
				field = BaseObject.GetEntityField(packedStruct, "0DDB53EB9299ECC9826DF9A47E5E4F38");
				field.SetValue(packedStruct, new Vector3UByte(def.Size));

				//Set block margins
				field = BaseObject.GetEntityField(packedStruct, "4045ED59A8C93DE0B41218EF2E947E55");
				field.SetValue(packedStruct, new Vector3B(0, 0, 0));
				field = BaseObject.GetEntityField(packedStruct, "096897446D5BD5243D3D6E5C53CE1772");
				field.SetValue(packedStruct, new Vector3B(0, 0, 0));

				//Set block margin scale
				field = BaseObject.GetEntityField(packedStruct, "E28B9725868E18B339D1E0594EF14444");
				field.SetValue(packedStruct, new Vector3B(0, 0, 0));

				//Set orientation
				Quaternion rot;
				cubeBlock.BlockOrientation.GetQuaternion(out rot);
				field = BaseObject.GetEntityField(packedStruct, "F1AAFF5C8F200592F313BC7E02140A38");
				field.SetValue(packedStruct, Base6Directions.GetForward(rot));
				field = BaseObject.GetEntityField(packedStruct, "E80AA7B84131E39F9F88209A109EED59");
				field.SetValue(packedStruct, Base6Directions.GetUp(rot));

				//Set color
				field = BaseObject.GetEntityField(packedStruct, "556976F2528411FF5F95FC75DC13FEED");
				field.SetValue(packedStruct, ColorExtensions.PackHSVToUint(cubeBlock.ColorMaskHSV));

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

		#endregion
	}
}
