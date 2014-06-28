using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.API;
using SEModAPI.API.Definitions;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;
using SEModAPIInternal.Support;
using System.Text;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid
{
	public class CubeBlockEntity : BaseObject
	{
		#region "Attributes"

		private Object m_backingObject;
		private long m_entityId;

		#endregion

		#region "Constructors and Initializers"

		public CubeBlockEntity(MyObjectBuilder_CubeBlock definition)
			: base(definition)
		{
			m_entityId = definition.EntityId;
		}

		#endregion

		#region "Properties"

		[Category("Cube Block")]
		[Browsable(false)]
		public Object BackingObject
		{
			get { return m_backingObject; }
			set
			{
				m_backingObject = value;
				Changed = true;
			}
		}

		public override string Name
		{
			get { return Subtype; }
		}

		/// <summary>
		/// Entity ID of the object
		/// </summary>
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
			}
		}

		[Category("Cube Block")]
		[Description("Added as of 1.035.005")]
		public ulong Owner
		{
			get { return GetSubTypeEntity().Owner; }
			set
			{
				if (GetSubTypeEntity().Owner == value) return;
				GetSubTypeEntity().Owner = value;
				Changed = true;
			}
		}

		[Category("Cube Block")]
		[Description("Added as of 1.035.005")]
		public bool ShareWithFaction
		{
			get { return GetSubTypeEntity().ShareWithFaction; }
			set
			{
				if (GetSubTypeEntity().ShareWithFaction == value) return;
				GetSubTypeEntity().ShareWithFaction = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Generates a new in-game entity ID
		/// </summary>
		/// <returns></returns>
		public long GenerateEntityId()
		{
			return BaseEntityManagerWrapper.GenerateEntityId();
		}

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		internal MyObjectBuilder_CubeBlock GetSubTypeEntity()
		{
			return (MyObjectBuilder_CubeBlock)BaseEntity;
		}

		#endregion
	}

	public class CubeBlockManager : BaseObjectManager
	{
		#region "Constructors and Initializers"

		public CubeBlockManager()
		{
		}

		public CubeBlockManager(CubeBlockEntity[] baseDefinitions)
			: base(baseDefinitions)
		{
		}

		public CubeBlockManager(List<CubeBlockEntity> baseDefinitions)
			: base(baseDefinitions.ToArray())
		{
		}
		
		#endregion

		#region "Methods"

		public CubeBlockEntity NewEntry<T, V>(T source)
			where T : MyObjectBuilder_CubeBlock
			where V : CubeBlockEntity
		{
			try
			{
				if (!IsMutable) return default(CubeBlockEntity);

				var newEntryType = typeof(V);

				var newEntry = (V)Activator.CreateInstance(newEntryType, new object[] { source });

				long entityId = newEntry.EntityId;
				if (entityId == 0)
					entityId = newEntry.GenerateEntityId();
				GetInternalData().Add(entityId, newEntry);

				return newEntry;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		#endregion
	}

	public class CubeBlockInternalWrapper : BaseInternalWrapper
	{
		#region "Attributes"

		protected new static CubeBlockInternalWrapper m_instance;

		private static Assembly m_assembly;

		private static Object m_cubeBlockToUpdate;
		private static string m_terminalBlockCustomName;
		private static bool m_functionalBlockEnabled;

		public static string CubeGridGetCubeBlocksHashSetMethod = "E38F3E9D7A76CD246B99F6AE91CC3E4A";

		public static string CubeBlockGetObjectBuilderMethod = "CBB75211A3B0B3188541907C9B1B0C5C";
		public static string CubeBlockGetSpecialBlockMethod = "7D4CAA3CE7687B9A7D20CCF3DE6F5441";

		public static string TerminalBlockClass = "6DDCED906C852CFDABA0B56B84D0BD74.CCFD704C70C3F20F7E84E8EA42D7A730";
		public static string TerminalBlockGetCustomNameMethod = "DE9705A29F3FE6F1E501595879B2E54F";
		public static string TerminalBlockSetCustomNameMethod = "774FC8084C0899CEF5C8DAE867B847FE";

		public static string FunctionalBlockClass = "6DDCED906C852CFDABA0B56B84D0BD74.7085736D64DCC58ED5DCA05FFEEA9664";
		public static string FunctionalBlockSetEnabledMethod = "97EC0047E8B562F4590B905BD8571F51";

		public static string ReactorBlockSetEnabledMethod = "E07EE72F25C9CA3C2EE6888D308A0E8D";

		#endregion

		#region "Constructors and Initializers"

		protected CubeBlockInternalWrapper(string basePath)
			: base(basePath)
		{
			m_instance = this;

			//string assemblyPath = Path.Combine(path, "Sandbox.Game.dll");
			m_assembly = Assembly.UnsafeLoadFrom("Sandbox.Game.dll");

			Console.WriteLine("Finished loading CubeBlockInternalWrapper");
		}

		new public static CubeBlockInternalWrapper GetInstance(string basePath = "")
		{
			if (m_instance == null)
			{
				m_instance = new CubeBlockInternalWrapper(basePath);
			}
			return (CubeBlockInternalWrapper)m_instance;
		}

		#endregion

		#region "Properties"

		new public static bool IsDebugging
		{
			get
			{
				CubeBlockInternalWrapper.GetInstance();
				return m_isDebugging;
			}
			set
			{
				CubeBlockInternalWrapper.GetInstance();
				m_isDebugging = value;
			}
		}

		#endregion

		#region "Methods"

		#region APIEntityLists

		public HashSet<Object> GetCubeBlocksHashSet(CubeGridEntity cubeGrid)
		{
			var rawValue = InvokeEntityMethod(cubeGrid.BackingObject, CubeGridGetCubeBlocksHashSetMethod, new object[] { });
			HashSet<Object> convertedSet = ConvertHashSet(rawValue);

			return convertedSet;
		}

		private List<T> GetAPIEntityCubeBlockList<T, TO>(CubeGridEntity cubeGrid, MyObjectBuilderTypeEnum type)
			where T : CubeBlockEntity
			where TO : MyObjectBuilder_CubeBlock
		{
			HashSet<Object> rawEntities = GetCubeBlocksHashSet(cubeGrid);
			List<T> list = new List<T>();

			foreach (Object entity in rawEntities)
			{
				try
				{
					MyObjectBuilder_CubeBlock baseEntity = (MyObjectBuilder_CubeBlock)InvokeEntityMethod(entity, CubeBlockGetObjectBuilderMethod, new object[] { });

					if (baseEntity.TypeId == type)
					{
						TO objectBuilder = (TO)baseEntity;
						T apiEntity = (T)Activator.CreateInstance(typeof(T), new object[] { objectBuilder });
						apiEntity.BackingObject = entity;

						list.Add(apiEntity);
					}
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex.ToString());
				}
			}

			return list;
		}

		public List<CubeBlockEntity> GetStructuralBlocks(CubeGridEntity cubeGrid)
		{
			return GetAPIEntityCubeBlockList<CubeBlockEntity, MyObjectBuilder_CubeBlock>(cubeGrid, MyObjectBuilderTypeEnum.CubeBlock);
		}

		public List<CargoContainerEntity> GetCargoContainerBlocks(CubeGridEntity cubeGrid)
		{
			return GetAPIEntityCubeBlockList<CargoContainerEntity, MyObjectBuilder_CargoContainer>(cubeGrid, MyObjectBuilderTypeEnum.CargoContainer);
		}

		public List<ReactorEntity> GetReactorBlocks(CubeGridEntity cubeGrid)
		{
			return GetAPIEntityCubeBlockList<ReactorEntity, MyObjectBuilder_Reactor>(cubeGrid, MyObjectBuilderTypeEnum.Reactor);
		}

		#endregion

		public static string GetTerminalBlockCustomName(Object cubeBlockEntity)
		{
			try
			{
				Object specialCubeObject = InvokeEntityMethod(m_cubeBlockToUpdate, CubeBlockGetSpecialBlockMethod);
				StringBuilder customName = (StringBuilder)InvokeEntityMethod(specialCubeObject, TerminalBlockGetCustomNameMethod);

				return customName.ToString();
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex.ToString());
				return "";
			}
		}

		#region Updates

		public bool UpdateTerminalBlockCustomName(Object cubeBlockEntity, string customName)
		{
			try
			{
				m_terminalBlockCustomName = customName;
				m_cubeBlockToUpdate = cubeBlockEntity;

				Action action = InternalSetCustomName;
				SandboxGameAssemblyWrapper.EnqueueMainGameAction(action);

				return true;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex.ToString());
				throw ex;
			}
		}

		public bool UpdateFunctionalBlockEnabled(Object cubeBlockEntity, bool isBlockEnabled)
		{
			try
			{
				m_functionalBlockEnabled = isBlockEnabled;
				m_cubeBlockToUpdate = cubeBlockEntity;

				Action action = InternalSetFunctionalState;
				SandboxGameAssemblyWrapper.EnqueueMainGameAction(action);

				return true;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex.ToString());
				throw ex;
			}
		}

		#endregion

		#region Internal

		public static void InternalSetFunctionalState()
		{
			try
			{
				if (m_cubeBlockToUpdate == null)
					return;

				Object specialCubeObject = InvokeEntityMethod(m_cubeBlockToUpdate, CubeBlockGetSpecialBlockMethod);
				string blockName = GetTerminalBlockCustomName(specialCubeObject);

				if (m_isDebugging)
				{
					if (m_functionalBlockEnabled)
						Console.WriteLine("FunctionalBlock '" + blockName + "': Enabling");
					else
						Console.WriteLine("FunctionalBlock '" + blockName + "': Disabling");
				}

				InvokeEntityMethod(specialCubeObject, FunctionalBlockSetEnabledMethod, new object[] { m_functionalBlockEnabled });
				InvokeEntityMethod(specialCubeObject, ReactorBlockSetEnabledMethod, new object[] { m_functionalBlockEnabled });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex.ToString());
			}
		}

		public static void InternalSetCustomName()
		{
			try
			{
				if (m_cubeBlockToUpdate == null)
					return;

				Object specialCubeObject = InvokeEntityMethod(m_cubeBlockToUpdate, CubeBlockGetSpecialBlockMethod);
				string blockName = GetTerminalBlockCustomName(specialCubeObject);

				if (m_isDebugging)
				{
					Console.WriteLine("TerminalBlock '" + blockName + "': Setting custom name to '" + m_terminalBlockCustomName + "'");
				}

				StringBuilder newCustomName = new StringBuilder(m_terminalBlockCustomName);

				InvokeEntityMethod(specialCubeObject, TerminalBlockSetCustomNameMethod, new object[] { newCustomName });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex.ToString());
			}
		}

		#endregion

		#endregion
	}
}
