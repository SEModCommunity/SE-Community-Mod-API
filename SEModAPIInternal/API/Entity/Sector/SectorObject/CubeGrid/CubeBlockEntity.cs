using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Threading;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.API;
using SEModAPI.API.Definitions;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;
using SEModAPIInternal.API.Utility;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid
{
	public class CubeBlockEntity : BaseObject
	{
		#region "Attributes"

		private bool m_hasGeneratedId;

		public static string CubeBlockGetObjectBuilder_Method = "CBB75211A3B0B3188541907C9B1B0C5C";
		public static string CubeBlockGetActualBlock_Method = "7D4CAA3CE7687B9A7D20CCF3DE6F5441";

		#endregion

		#region "Constructors and Initializers"

		public CubeBlockEntity(MyObjectBuilder_CubeBlock definition)
			: base(definition)
		{
			if (definition.EntityId == 0)
			{
				m_hasGeneratedId = true;
				EntityId = GenerateEntityId();
			}
		}

		public CubeBlockEntity(MyObjectBuilder_CubeBlock definition, Object backingObject)
			: base(definition, backingObject)
		{
			if (definition.EntityId == 0)
			{
				m_hasGeneratedId = true;
				EntityId = GenerateEntityId();
			}
		}

		#endregion

		#region "Properties"

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
		[Browsable(true)]
		[ReadOnly(true)]
		public bool HasGeneratedId
		{
			get { return m_hasGeneratedId; }
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

		protected Object GetActualObject()
		{
			try
			{
				Type backingType = BackingObject.GetType();
				MethodInfo method = backingType.GetMethod(CubeBlockGetActualBlock_Method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
				Object actualCubeObject = method.Invoke(BackingObject, new object[] { });

				return actualCubeObject;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		#endregion
	}

	public class CubeBlockManager : BaseObjectManager
	{
		#region "Constructors and Initializers"

		public CubeBlockManager()
		{
		}

		public CubeBlockManager(Object backingSource, string backingSourceMethodName)
			: base(backingSource, backingSourceMethodName)
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

		public override void LoadDynamic()
		{
			HashSet<Object> rawEntities = GetBackingDataHashSet();
			Dictionary<long, BaseObject> data = GetInternalData();
			Dictionary<Object, BaseObject> backingData = GetBackingInternalData();

			//Update the main data mapping
			data.Clear();
			foreach (Object entity in rawEntities)
			{
				try
				{
					MyObjectBuilder_CubeBlock baseEntity = (MyObjectBuilder_CubeBlock)CubeBlockEntity.InvokeEntityMethod(entity, CubeBlockEntity.CubeBlockGetObjectBuilder_Method, new object[] { });

					CubeBlockEntity matchingCubeBlock = null;

					//If the original data already contains an entry for this, skip creation
					if (backingData.ContainsKey(entity))
					{
						matchingCubeBlock = (CubeBlockEntity)backingData[entity];
					}
					else
					{
						switch (baseEntity.TypeId)
						{
							case MyObjectBuilderTypeEnum.CubeBlock:
								matchingCubeBlock = new CubeBlockEntity(baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.CargoContainer:
								matchingCubeBlock = new CargoContainerEntity((MyObjectBuilder_CargoContainer)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.Reactor:
								matchingCubeBlock = new ReactorEntity((MyObjectBuilder_Reactor)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.Beacon:
								matchingCubeBlock = new BeaconEntity((MyObjectBuilder_Beacon)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.Cockpit:
								matchingCubeBlock = new CockpitEntity((MyObjectBuilder_Cockpit)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.GravityGenerator:
								matchingCubeBlock = new GravityGeneratorEntity((MyObjectBuilder_GravityGenerator)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.MedicalRoom:
								matchingCubeBlock = new MedicalRoomEntity((MyObjectBuilder_MedicalRoom)baseEntity, entity);
								break;
							default:
								matchingCubeBlock = new CubeBlockEntity(baseEntity, entity);
								break;
						}
					}

					if (matchingCubeBlock == null)
						throw new Exception("Failed to match/create cube block entity");

					data.Add(matchingCubeBlock.EntityId, matchingCubeBlock);
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
				}
			}

			//Update the backing data mapping
			backingData.Clear();
			foreach (var key in data.Keys)
			{
				var entry = data[key];
				backingData.Add(entry.BackingObject, entry);
			}
		}

		#endregion
	}
}
