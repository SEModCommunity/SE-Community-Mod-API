using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Sandbox.Common.ObjectBuilders;

using SEModAPI.API.SaveData;
using SEModAPI.API.SaveData.Entity;

namespace SEModAPI.API.Internal
{
	public class CubeBlockInternalWrapper : BaseInternalWrapper
	{
		#region "Attributes"

		protected new static CubeBlockInternalWrapper m_instance;

		public static string CubeBlockGetObjectBuilderMethod = "CBB75211A3B0B3188541907C9B1B0C5C";

		#endregion

		#region "Constructors and Initializers"

		protected CubeBlockInternalWrapper(string basePath)
			: base(basePath)
		{
			m_instance = this;

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

		public HashSet<Object> GetCubeBlocksHashSet(CubeGrid cubeGrid)
		{
			var rawValue = InvokeEntityMethod(cubeGrid.BackingObject, "E38F3E9D7A76CD246B99F6AE91CC3E4A", new object[] { });
			HashSet<Object> convertedSet = ConvertHashSet(rawValue);

			return convertedSet;
		}

		private List<T> GetAPIEntityCubeBlockList<T, TO>(CubeGrid cubeGrid, MyObjectBuilderTypeEnum type)
			where TO : MyObjectBuilder_CubeBlock
			where T : CubeBlockEntity<TO>
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
						apiEntity.BackingThread = GameObjectManagerWrapper.GetInstance().GameThread;

						list.Add(apiEntity);
					}
				}
				catch (Exception ex)
				{
					SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				}
			}

			return list;
		}

		public List<CubeBlockEntity<MyObjectBuilder_CubeBlock>> GetStructuralBlocks(CubeGrid cubeGrid)
		{
			return GetAPIEntityCubeBlockList<CubeBlockEntity<MyObjectBuilder_CubeBlock>, MyObjectBuilder_CubeBlock>(cubeGrid, MyObjectBuilderTypeEnum.CubeBlock);
		}

		public List<CargoContainerEntity> GetCargoContainerBlocks(CubeGrid cubeGrid)
		{
			return GetAPIEntityCubeBlockList<CargoContainerEntity, MyObjectBuilder_CargoContainer>(cubeGrid, MyObjectBuilderTypeEnum.CargoContainer);
		}

		#endregion
	}
}
