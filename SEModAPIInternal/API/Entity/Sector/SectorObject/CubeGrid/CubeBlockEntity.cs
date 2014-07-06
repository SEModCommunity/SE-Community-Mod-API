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

		private CubeGridEntity m_parent;
		private bool m_hasGeneratedId;

		public static string CubeBlockNamespace = "6DDCED906C852CFDABA0B56B84D0BD74";
		public static string CubeBlockClass = "54A8BE425EAC4A11BFF922CFB5FF89D0";
		public static string CubeBlockGetObjectBuilder_Method = "CBB75211A3B0B3188541907C9B1B0C5C";
		public static string CubeBlockGetActualBlock_Method = "7D4CAA3CE7687B9A7D20CCF3DE6F5441";
		public static string CubeBlockParentCubeGridField = "7A975CBF89D2763F147297C064B1D764";

		public static string ActualCubeBlockNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string ActualCubeBlockClass = "4E262F069F7C0F85458881743E182B25";
		public static string ActualCubeBlockGetObjectBuilderMethod = "GetObjectBuilderCubeBlock";
		public static string ActualCubeBlockGetFactionsObjectMethod = "3E8AC70E5FAAA9C8C4992B71E12CDE28";
		public static string ActualCubeBlockSetFactionsDataMethod = "7161368A8164DF15904DC82476F7EBBA";

		public static string FactionsDataNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string FactionsDataClass = "0428A90CA95B1CE381A027F8E935681A";
		public static string FactionsDataOwnerField = "9A0535F68700D4E48674829975E95CAB";
		public static string FactionsDataShareModeField = "0436783F3C7FB6B04C88AB4F9097380F";

		#endregion

		#region "Constructors and Initializers"

		public CubeBlockEntity(CubeGridEntity parent, MyObjectBuilder_CubeBlock definition)
			: base(definition)
		{
			m_parent = parent;
			if (definition.EntityId == 0)
			{
				m_hasGeneratedId = true;
				EntityId = GenerateEntityId();
			}
		}

		public CubeBlockEntity(CubeGridEntity parent, MyObjectBuilder_CubeBlock definition, Object backingObject)
			: base(definition, backingObject)
		{
			m_parent = parent;
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

		#region "Internal"

		public Object GetActualObject()
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

		public Object GetParentCubeGrid()
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

		public Object GetFactionData()
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

		#endregion

		#endregion
	}

	public class CubeBlockManager : BaseObjectManager
	{
		#region "Attributes"

		private CubeGridEntity m_parent;

		#endregion

		#region "Constructors and Initializers"

		public CubeBlockManager(CubeGridEntity parent)
		{
			m_parent = parent;
		}

		public CubeBlockManager(CubeGridEntity parent, Object backingSource, string backingSourceMethodName)
			: base(backingSource, backingSourceMethodName)
		{
			m_parent = parent;
		}
		
		#endregion

		#region "Methods"

		public override void LoadDynamic()
		{
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
					MyObjectBuilder_CubeBlock baseEntity = (MyObjectBuilder_CubeBlock)CubeBlockEntity.InvokeEntityMethod(entity, CubeBlockEntity.CubeBlockGetObjectBuilder_Method, new object[] { });

					if (data.ContainsKey(baseEntity.EntityId))
					{
						//We should not be here!
						continue;
					}

					CubeBlockEntity matchingCubeBlock = null;

					//If the original data already contains an entry for this, skip creation
					if (backingData.ContainsKey(entity))
					{
						matchingCubeBlock = (CubeBlockEntity)backingData[entity];

						//Update the base entity (not the same as BackingObject which is the internal object)
						matchingCubeBlock.BaseEntity = baseEntity;
					}
					else
					{
						switch (baseEntity.TypeId)
						{
							case MyObjectBuilderTypeEnum.CubeBlock:
								matchingCubeBlock = new CubeBlockEntity(m_parent, baseEntity, entity);
								break;
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
							default:
								matchingCubeBlock = new CubeBlockEntity(m_parent, baseEntity, entity);
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
