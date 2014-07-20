using Microsoft.Xml.Serialization.GeneratedAssembly;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.Voxels;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.API.Definitions;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity.Sector;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.API.Utility;
using SEModAPIInternal.Support;

using VRage;

namespace SEModAPIInternal.API.Entity
{
	public class SectorEntity : BaseObject
	{
		#region "Attributes"

		//Sector Events
		private BaseObjectManager m_eventManager;

		//Sector Objects
		private BaseObjectManager m_cubeGridManager;
		private BaseObjectManager m_voxelMapManager;
		private BaseObjectManager m_floatingObjectManager;
		private BaseObjectManager m_meteorManager;
		private BaseObjectManager m_unknownObjectManager;

		#endregion

		#region "Constructors and Initializers"

		public SectorEntity(MyObjectBuilder_Sector definition)
			: base(definition)
		{
			m_eventManager = new BaseObjectManager();
			m_cubeGridManager = new BaseObjectManager();
			m_voxelMapManager = new BaseObjectManager();
			m_floatingObjectManager = new BaseObjectManager();
			m_meteorManager = new BaseObjectManager();
			m_unknownObjectManager = new BaseObjectManager();

			List<Event> events = new List<Event>();
			foreach (var sectorEvent in definition.SectorEvents.Events)
			{
				events.Add(new Event(sectorEvent));
			}

			List<CubeGridEntity> cubeGrids = new List<CubeGridEntity>();
			List<VoxelMap> voxelMaps = new List<VoxelMap>();
			List<FloatingObject> floatingObjects = new List<FloatingObject>();
			List<Meteor> meteors = new List<Meteor>();
			List<BaseEntity> unknowns = new List<BaseEntity>();
			foreach (var sectorObject in definition.SectorObjects)
			{
				if (sectorObject.TypeId == typeof(MyObjectBuilder_CubeGrid))
				{
					cubeGrids.Add(new CubeGridEntity((MyObjectBuilder_CubeGrid)sectorObject));
				}
				else if (sectorObject.TypeId == typeof(MyObjectBuilder_VoxelMap))
				{
					voxelMaps.Add(new VoxelMap((MyObjectBuilder_VoxelMap)sectorObject));
				}
				else if (sectorObject.TypeId == typeof(MyObjectBuilder_FloatingObject))
				{
					floatingObjects.Add(new FloatingObject((MyObjectBuilder_FloatingObject)sectorObject));
				}
				else if (sectorObject.TypeId == typeof(MyObjectBuilder_Meteor))
				{
					meteors.Add(new Meteor((MyObjectBuilder_Meteor)sectorObject));
				}
				else
				{
					unknowns.Add(new BaseEntity(sectorObject));
				}
			}

			//Build the managers from the lists
			m_eventManager.Load(events.ToArray());
			m_cubeGridManager.Load(cubeGrids.ToArray());
			m_voxelMapManager.Load(voxelMaps.ToArray());
			m_floatingObjectManager.Load(floatingObjects.ToArray());
			m_meteorManager.Load(meteors.ToArray());
			m_unknownObjectManager.Load(unknowns.ToArray());
		}

		#endregion

		#region "Properties"

		/// <summary>
		/// API formated name of the object
		/// </summary>
		[Category("Sector")]
		[Browsable(true)]
		[ReadOnly(true)]
		[Description("The formatted name of the object")]
		public override string Name
		{
			get { return "SANDBOX_" + this.Position.X + "_" + this.Position.Y + "_" + this.Position.Z + "_"; }
		}

		[Category("Sector")]
		public VRageMath.Vector3I Position
		{
			get { return GetSubTypeEntity().Position; }
		}

		[Category("Sector")]
		public int AppVersion
		{
			get { return GetSubTypeEntity().AppVersion; }
		}

		[Category("Sector")]
		[Browsable(false)]
		public List<Event> Events
		{
			get
			{
				var newList = m_eventManager.GetTypedInternalData<Event>();
				return newList;
			}
		}

		[Category("Sector")]
		[Browsable(false)]
		public List<CubeGridEntity> CubeGrids
		{
			get
			{
				var newList = m_cubeGridManager.GetTypedInternalData<CubeGridEntity>();
				return newList;
			}
		}

		[Category("Sector")]
		[Browsable(false)]
		public List<VoxelMap> VoxelMaps
		{
			get
			{
				var newList = m_voxelMapManager.GetTypedInternalData<VoxelMap>();
				return newList;
			}
		}

		[Category("Sector")]
		[Browsable(false)]
		public List<FloatingObject> FloatingObjects
		{
			get
			{
				var newList = m_floatingObjectManager.GetTypedInternalData<FloatingObject>();
				return newList;
			}
		}

		[Category("Sector")]
		[Browsable(false)]
		public List<Meteor> Meteors
		{
			get
			{
				var newList = m_meteorManager.GetTypedInternalData<Meteor>();
				return newList;
			}
		}

		[Category("Sector")]
		[Browsable(false)]
		public List<BaseEntity> UnknownObjects
		{
			get
			{
				var newList = m_unknownObjectManager.GetTypedInternalData<BaseEntity>();
				return newList;
			}
		}

		#endregion

		#region "Methods"

		public BaseObject NewEntry(Type newType)
		{
			if (newType == typeof(CubeGridEntity))
				return m_cubeGridManager.NewEntry<CubeGridEntity>();
			if(newType == typeof(VoxelMap))
				return m_voxelMapManager.NewEntry<VoxelMap>();
			if (newType == typeof(FloatingObject))
				return m_floatingObjectManager.NewEntry<FloatingObject>();
			if (newType == typeof(Meteor))
				return m_meteorManager.NewEntry<Meteor>();

			return null;
		}

		public bool DeleteEntry(Object source)
		{
			Type deleteType = source.GetType();
			if (deleteType == typeof(CubeGridEntity))
				return m_cubeGridManager.DeleteEntry((CubeGridEntity)source);
			if (deleteType == typeof(VoxelMap))
				return m_voxelMapManager.DeleteEntry((VoxelMap)source);
			if (deleteType == typeof(FloatingObject))
				return m_floatingObjectManager.DeleteEntry((FloatingObject)source);
			if (deleteType == typeof(Meteor))
				return m_meteorManager.DeleteEntry((Meteor)source);

			return false;
		}

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_Sector GetSubTypeEntity()
		{
			MyObjectBuilder_Sector baseSector = (MyObjectBuilder_Sector)BaseEntity;

			try
			{
				//Update the events in the base definition
				baseSector.SectorEvents.Events.Clear();
				foreach (var item in m_eventManager.GetTypedInternalData<Event>())
				{
					baseSector.SectorEvents.Events.Add(item.GetSubTypeEntity());
				}

				//Update the sector objects in the base definition
				baseSector.SectorObjects.Clear();
				foreach (var item in m_cubeGridManager.GetTypedInternalData<CubeGridEntity>())
				{
					baseSector.SectorObjects.Add(item.GetSubTypeEntity());
				}
				foreach (var item in m_voxelMapManager.GetTypedInternalData<VoxelMap>())
				{
					baseSector.SectorObjects.Add(item.GetSubTypeEntity());
				}
				foreach (var item in m_floatingObjectManager.GetTypedInternalData<FloatingObject>())
				{
					baseSector.SectorObjects.Add(item.GetSubTypeEntity());
				}
				foreach (var item in m_meteorManager.GetTypedInternalData<Meteor>())
				{
					baseSector.SectorObjects.Add(item.GetSubTypeEntity());
				}
				foreach (var item in m_unknownObjectManager.GetTypedInternalData<BaseEntity>())
				{
					baseSector.SectorObjects.Add(item.GetSubTypeEntity());
				}
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
			return baseSector;
		}

		#endregion
	}

	public class SectorObjectManager : BaseObjectManager
	{
		#region "Attributes"

		private static SectorObjectManager m_instance;
		private static BaseEntity m_nextEntityToUpdate;
		private bool m_isShutDown;

		protected Dictionary<Object, MyObjectBuilder_EntityBase> m_rawDataObjectBuilderList;

		public static string ObjectManagerNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string ObjectManagerClass = "CAF1EB435F77C7B77580E2E16F988BED";
		public static string ObjectManagerGetEntityHashSet = "84C54760C0F0DDDA50B0BE27B7116ED8";
		public static string ObjectManagerAddEntity = "E5E18F5CAD1F62BB276DF991F20AE6AF";

		public static string ObjectFactoryNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string ObjectFactoryClass = "E825333D6467D99DD83FB850C600395C";
		public static string ObjectFactoryCreateEntityMethod = "060AD47B4BD57C19FEEC3DED4F8E7F9D";
		public static string ObjectFactoryCreateTypedEntityMethod = "060AD47B4BD57C19FEEC3DED4F8E7F9D";

		//2 Packet Types
		public static string EntityBaseNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string EntityBaseNetManagerClass = "8EFE49A46AB934472427B7D117FD3C64";
		public static string EntityBaseNetManagerSendEntity = "A6B585C993B43E72219511726BBB0649";

		#endregion

		#region "Constructors and Initializers"

		public SectorObjectManager()
		{
			IsDynamic = true;
			m_instance = this;
			m_isShutDown = false;

			m_rawDataObjectBuilderList = new Dictionary<object, MyObjectBuilder_EntityBase>();
		}

		#endregion

		#region "Properties"

		public static SectorObjectManager Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new SectorObjectManager();

				return m_instance;
			}
		}

		public static Type InternalType
		{
			get
			{
				Type objectManagerType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(ObjectManagerNamespace, ObjectManagerClass);
				return objectManagerType;
			}
		}

		public bool IsShutDown
		{
			get { return m_isShutDown; }
			set { m_isShutDown = value; }
		}

		#endregion

		#region "Methods"

		protected override void InternalGetBackingDataHashSet()
		{
			try
			{
				if (m_isInternalResourceLocked)
					return;
				if (WorldManager.Instance.IsAutosaving)
					return;

				m_isInternalResourceLocked = true;

				var rawValue = BaseObject.InvokeStaticMethod(InternalType, ObjectManagerGetEntityHashSet);
				m_rawDataHashSet = UtilityFunctions.ConvertHashSet(rawValue);

				m_rawDataObjectBuilderList.Clear();
				foreach (Object entity in m_rawDataHashSet)
				{
					//Skip disposed entities
					bool isDisposed = (bool)BaseEntity.InvokeEntityMethod(entity, BaseEntity.BaseEntityGetIsDisposedMethod);
					if (isDisposed)
						continue;

					//Skip entities that don't have the physics defined yet
					if (BaseEntity.GetRigidBody(entity) == null)
						continue;

					//Skip entities that don't have a position-orientation matrix defined
					if (BaseEntity.InvokeEntityMethod(entity, BaseEntity.BaseEntityGetOrientationMatrixMethod) == null)
						continue;

					try
					{
						MyObjectBuilder_EntityBase baseEntity = (MyObjectBuilder_EntityBase)BaseEntity.InvokeEntityMethod(entity, BaseEntity.BaseEntityGetObjectBuilderMethod, new object[] { false });
						m_rawDataObjectBuilderList.Add(entity, baseEntity);
					}
					catch (Exception ex)
					{
						LogManager.GameLog.WriteLine(ex);
					}
				}

				m_isInternalResourceLocked = false;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		public override void LoadDynamic()
		{
			if (IsResourceLocked)
				return;
			if (IsShutDown)
				return;
			if (m_isInternalResourceLocked)
				return;

			IsResourceLocked = true;

			HashSet<Object> rawEntities = GetBackingDataHashSet();
			Dictionary<long, BaseObject> data = GetInternalData();
			Dictionary<Object, BaseObject> backingData = GetBackingInternalData();

			m_isInternalResourceLocked = true;

			//Update the main data mapping
			data.Clear();
			int entityCount = 0;
			foreach (Object entity in rawEntities)
			{
				entityCount++;

				try
				{
					//Skip unknowns for now until we get the bugs sorted out with the other types
					Type entityType = entity.GetType();
					if (entityType != CharacterEntity.InternalType &&
						entityType != CubeGridEntity.InternalType &&
						entityType != VoxelMap.InternalType &&
						entityType != FloatingObject.InternalType &&
						entityType != Meteor.InternalType
						)
						continue;

					MyObjectBuilder_EntityBase baseEntity = null;
					if(m_rawDataObjectBuilderList.ContainsKey(entity))
						baseEntity = m_rawDataObjectBuilderList[entity];

					if (baseEntity == null)
						continue;
					if (data.ContainsKey(baseEntity.EntityId))
						continue;

					BaseEntity matchingEntity = null;

					//If the original data already contains an entry for this, skip creation
					if (backingData.ContainsKey(entity))
					{
						matchingEntity = (BaseEntity)backingData[entity];
						if (matchingEntity.IsDisposed)
							continue;

						//Update the base entity (not the same as BackingObject which is the internal object)
						matchingEntity.BaseEntity = baseEntity;
					}

					if(matchingEntity == null)
					{
						if(baseEntity.TypeId == typeof(MyObjectBuilder_Character))
							matchingEntity = new CharacterEntity((MyObjectBuilder_Character)baseEntity, entity);
						else if (baseEntity.TypeId == typeof(MyObjectBuilder_CubeGrid))
							matchingEntity = new CubeGridEntity((MyObjectBuilder_CubeGrid)baseEntity, entity);
						else if (baseEntity.TypeId == typeof(MyObjectBuilder_FloatingObject))
							matchingEntity = new FloatingObject((MyObjectBuilder_FloatingObject)baseEntity, entity);
						else if (baseEntity.TypeId == typeof(MyObjectBuilder_Meteor))
							matchingEntity = new Meteor((MyObjectBuilder_Meteor)baseEntity, entity);
						else if (baseEntity.TypeId == typeof(MyObjectBuilder_VoxelMap))
							matchingEntity = new VoxelMap((MyObjectBuilder_VoxelMap)baseEntity, entity);
						else
							matchingEntity = new BaseEntity(baseEntity, entity);
					}

					if (matchingEntity == null)
						throw new Exception("Failed to match/create sector object entity");

					data.Add(matchingEntity.EntityId, matchingEntity);
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

			m_isInternalResourceLocked = false;
			IsResourceLocked = false;
		}

		public void AddEntity(BaseEntity entity)
		{
			try
			{
				if (SandboxGameAssemblyWrapper.IsDebugging)
					Console.WriteLine("Entity '" + entity.Name + "' is being added ...");

				m_nextEntityToUpdate = entity;

				Action action = InternalAddEntity;
				SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected void InternalAddEntity()
		{
			try
			{
				if (m_nextEntityToUpdate == null)
					return;

				if (SandboxGameAssemblyWrapper.IsDebugging)
					Console.WriteLine("Entity '" + m_nextEntityToUpdate.GetType().Name + "': Adding to scene ...");

				//Create the backing object
				Type entityType = m_nextEntityToUpdate.GetType();
				Type internalType = (Type)BaseEntity.InvokeStaticMethod(entityType, "get_InternalType");
				if (internalType == null)
					throw new Exception("Could not get internal type of entity");
				m_nextEntityToUpdate.BackingObject = Activator.CreateInstance(internalType);

				//Initialize the backing object
				BaseEntity.InvokeEntityMethod(m_nextEntityToUpdate.BackingObject, "Init", new object[] { m_nextEntityToUpdate.GetSubTypeEntity() });

				//Add the backing object to the main game object manager
				BaseEntity.InvokeStaticMethod(InternalType, ObjectManagerAddEntity, new object[] { m_nextEntityToUpdate.BackingObject, true });

				//Broadcast the new entity to the clients
				MyObjectBuilder_EntityBase baseEntity = (MyObjectBuilder_EntityBase)BaseEntity.InvokeEntityMethod(m_nextEntityToUpdate.BackingObject, BaseEntity.BaseEntityGetObjectBuilderMethod, new object[] { Type.Missing });
				Type someManager = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(EntityBaseNetManagerNamespace, EntityBaseNetManagerClass);
				BaseEntity.InvokeStaticMethod(someManager, EntityBaseNetManagerSendEntity, new object[] { baseEntity });

				m_nextEntityToUpdate.BaseEntity = baseEntity;
				m_nextEntityToUpdate = null;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLineAndConsole("Failed to add new entity");
				LogManager.GameLog.WriteLine(ex);
			}
		}
	
		#endregion
	}

	public class SectorManager : BaseObjectManager
	{
		#region "Attributes"

		private SectorEntity m_Sector;

		#endregion

		#region "Constructors and Initializers"

		public SectorManager()
		{
		}

		#endregion

		#region "Properties"

		public SectorEntity Sector
		{
			get { return m_Sector; }
		}

		#endregion

		#region "Methods"

		new public void Load(FileInfo fileInfo)
		{
			//Save the file info to the property
			FileInfo = fileInfo;

			//Read in the sector data
			MyObjectBuilder_Sector data = ReadSpaceEngineersFile<MyObjectBuilder_Sector, MyObjectBuilder_SectorSerializer>(this.FileInfo.FullName);

			//And instantiate the sector with the data
			m_Sector = new SectorEntity(data);
		}

		new public bool Save()
		{
			return WriteSpaceEngineersFile<MyObjectBuilder_Sector, MyObjectBuilder_SectorSerializer>(m_Sector.GetSubTypeEntity(), this.FileInfo.FullName);
		}

		#endregion
	}
}
