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
		private static Type m_internalType;

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
			ObjectBuilder = BaseObjectManager.LoadContentFile<MyObjectBuilder_CubeGrid, MyObjectBuilder_CubeGridSerializer>(prefabFile);
			ObjectBuilder.EntityId = 0;

			m_cubeBlockManager = new CubeBlockManager(this);
			List<CubeBlockEntity> cubeBlockList = new List<CubeBlockEntity>();
			foreach (var cubeBlock in ((MyObjectBuilder_CubeGrid)ObjectBuilder).CubeBlocks)
			{
				cubeBlock.EntityId = 0;
				cubeBlockList.Add(new CubeBlockEntity(this, cubeBlock));
			}
			m_cubeBlockManager.Load(cubeBlockList);
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
		}

		#endregion

		#region "Properties"

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

		[Category("Cube Grid")]
		[ReadOnly(true)]
		public override string Name
		{
			get
			{
				if (ObjectBuilder == null)
					return "Cube Grid";

				string name = "";
				if (BackingObject == null)
				{
					foreach (var cubeBlock in ObjectBuilder.CubeBlocks)
					{
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
				}
				else
				{
					List<BeaconEntity> beaconBlocks = m_cubeBlockManager.GetTypedInternalData<BeaconEntity>();
					foreach (BeaconEntity cubeBlock in beaconBlocks)
					{
						if (name.Length > 0)
							name += "|";

						string customName = cubeBlock.Name;
						if (customName == "")
							customName = "Beacon";

						name += customName;
					}
				}

				if (name.Length == 0)
					name = ObjectBuilder.EntityId.ToString();

				return name;
			}
		}

		[Category("Cube Grid")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_CubeGrid ObjectBuilder
		{
			get
			{
				return (MyObjectBuilder_CubeGrid)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[Category("Cube Grid")]
		[ReadOnly(true)]
		public MyCubeSize GridSizeEnum
		{
			get { return ObjectBuilder.GridSizeEnum; }
			set
			{
				if (ObjectBuilder.GridSizeEnum == value) return;
				ObjectBuilder.GridSizeEnum = value;
				Changed = true;
			}
		}

		[Category("Cube Grid")]
		[ReadOnly(true)]
		public bool IsStatic
		{
			get { return ObjectBuilder.IsStatic; }
			set
			{
				if (ObjectBuilder.IsStatic == value) return;
				ObjectBuilder.IsStatic = value;
				Changed = true;
			}
		}

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

		[Category("Cube Grid")]
		[Browsable(false)]
		public List<CubeBlockEntity> CubeBlocks
		{
			get
			{
				List<CubeBlockEntity> cubeBlocks = m_cubeBlockManager.GetTypedInternalData<CubeBlockEntity>();
				return cubeBlocks;
			}
		}

		[Category("Cube Grid")]
		[Browsable(false)]
		public List<BoneInfo> Skeleton
		{
			get { return ObjectBuilder.Skeleton; }
		}

		[Category("Cube Grid")]
		[Browsable(false)]
		public List<MyObjectBuilder_ConveyorLine> ConveyorLines
		{
			get { return ObjectBuilder.ConveyorLines; }
		}

		[Category("Cube Grid")]
		[Browsable(false)]
		public List<MyObjectBuilder_BlockGroup> BlockGroups
		{
			get { return ObjectBuilder.BlockGroups; }
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
		}

		#endregion

		#region "Methods"

		public override void Dispose()
		{
			LogManager.APILog.WriteLine("Disposing CubeGridEntity '" + Name + "'");
			/*
			foreach(CubeBlockEntity cubeBlock in CubeBlocks)
			{
				cubeBlock.Dispose();
			}
			*/
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
			RefreshCubeBlocks();

			BaseObjectManager.SaveContentFile<MyObjectBuilder_CubeGrid, MyObjectBuilder_CubeGridSerializer>(ObjectBuilder, fileInfo);
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
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		public void RefreshCubeBlocks()
		{
			MyObjectBuilder_CubeGrid cubeGrid = (MyObjectBuilder_CubeGrid)ObjectBuilder;

			//Refresh the cube blocks content in the cube grid from the cube blocks manager
			cubeGrid.CubeBlocks.Clear();
			foreach (var item in m_cubeBlockManager.GetTypedInternalData<CubeBlockEntity>())
			{
				cubeGrid.CubeBlocks.Add((MyObjectBuilder_CubeBlock)item.ObjectBuilder);
			}
		}

		#region "Internal"

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
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected void InternalUpdateDampenersEnabled()
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
		//24 Packets
		public enum CubeGridPacketIds
		{
			CubeBlockHashSet = 14,				//..AAC558DB3CA968D0D3B965EA00DF05D4
			Packet1_2 = 15,
			Packet1_3 = 16,
			CubeBlockPositionList = 17,			//..5A55EA00576BB526436F3708D1F55455
			Packet1_5 = 18,
			AllPowerStatus = 19,				//..782C8DC19A883BCB6A43C3006F456A2F

			Packet2_1 = 24,
			Packet2_2 = 25,
			CubeBlockItemList = 26,				//..3FD479635EACD6C3047ACB77CBAB645D
			Packet2_4 = 27,
			Packet2_5 = 28,
			Packet2_6 = 29,

			SomePacket1 = 4711,
			SomePacket2 = 4712,
			SomePacket3 = 4713,
			SomePacket4 = 4714,

			Packet3_1 = 15262,
			Packet3_2 = 15263,
			Packet3_3 = 15264,
			CubeBlockOrientationIsh = 15265,	//..69FB43596400BF997D806DF041F2B54D
			CubeBlockFactionData = 15266,		//..090EFC311778552F418C0835D1248D60
			CubeBlockOwnershipMode = 15267,		//..F62F6360C3B7B7D32C525D5987F70A68

			AllPowerStatus2 = 15271,			//..903CC5CD740D130E90DB6CBF79F80F4F

			ShipToggle1 = 15275,				//..4DCFFCEE8D5BA392C7A57ACD6470D7CD
		}

		#region "Attributes"

		private CubeGridEntity m_cubeGrid;
		private Object m_netManager;

		public static string CubeGridGetNetManagerMethod = "AF2DACDED0370C8DBA03A53FDA4E2C47";

		//Definition
		public static string CubeGridNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string CubeGridNetManagerClass = "E727876839B1C8FFEE302CD2A1948CDA";

		//Methods
		public static string CubeGridNetManagerBroadcastCubeBlockFactionDataMethod = "EF45C83059E3CD5A2C5354ABB687D861";

		//Fields
		public static string CubeGridNetManagerCubeBlocksToDestroyField = "8E76EFAC4EED3B61D48795B2CD5AF989";

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
				BaseObject.InvokeEntityMethod(m_netManager, CubeGridNetManagerBroadcastCubeBlockFactionDataMethod, new object[] { m_cubeGrid.BackingObject, cubeBlock.ActualObject, cubeBlock.Owner, cubeBlock.ShareMode });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
