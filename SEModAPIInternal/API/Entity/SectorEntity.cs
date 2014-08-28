using Microsoft.Xml.Serialization.GeneratedAssembly;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Voxels;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity.Sector;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.API.Utility;
using SEModAPIInternal.Support;

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

			List<Event> events = new List<Event>();
			foreach (var sectorEvent in definition.SectorEvents.Events)
			{
				events.Add(new Event(sectorEvent));
			}

			List<CubeGridEntity> cubeGrids = new List<CubeGridEntity>();
			List<VoxelMap> voxelMaps = new List<VoxelMap>();
			List<FloatingObject> floatingObjects = new List<FloatingObject>();
			List<Meteor> meteors = new List<Meteor>();
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
			}

			//Build the managers from the lists
			m_eventManager.Load(events);
			m_cubeGridManager.Load(cubeGrids);
			m_voxelMapManager.Load(voxelMaps);
			m_floatingObjectManager.Load(floatingObjects);
			m_meteorManager.Load(meteors);
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
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_Sector ObjectBuilder
		{
			get
			{
				MyObjectBuilder_Sector baseSector = (MyObjectBuilder_Sector)base.ObjectBuilder;

				try
				{
					//Update the events in the base definition
					baseSector.SectorEvents.Events.Clear();
					foreach (var item in m_eventManager.GetTypedInternalData<Event>())
					{
						baseSector.SectorEvents.Events.Add(item.ObjectBuilder);
					}

					//Update the sector objects in the base definition
					baseSector.SectorObjects.Clear();
					foreach (var item in m_cubeGridManager.GetTypedInternalData<CubeGridEntity>())
					{
						baseSector.SectorObjects.Add(item.ObjectBuilder);
					}
					foreach (var item in m_voxelMapManager.GetTypedInternalData<VoxelMap>())
					{
						baseSector.SectorObjects.Add(item.ObjectBuilder);
					}
					foreach (var item in m_floatingObjectManager.GetTypedInternalData<FloatingObject>())
					{
						baseSector.SectorObjects.Add(item.ObjectBuilder);
					}
					foreach (var item in m_meteorManager.GetTypedInternalData<Meteor>())
					{
						baseSector.SectorObjects.Add(item.ObjectBuilder);
					}
				}
				catch (Exception ex)
				{
					LogManager.ErrorLog.WriteLine(ex);
				}
				return baseSector;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[Category("Sector")]
		public VRageMath.Vector3I Position
		{
			get { return ObjectBuilder.Position; }
		}

		[Category("Sector")]
		public int AppVersion
		{
			get { return ObjectBuilder.AppVersion; }
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

		#endregion
	}

	public class SectorObjectManager : BaseObjectManager
	{
		#region "Attributes"

		private static SectorObjectManager m_instance;
		private static Queue<BaseEntity> m_addEntityQueue = new Queue<BaseEntity>();

		public static string ObjectManagerNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string ObjectManagerClass = "CAF1EB435F77C7B77580E2E16F988BED";
		public static string ObjectManagerGetEntityHashSet = "84C54760C0F0DDDA50B0BE27B7116ED8";
		public static string ObjectManagerAddEntity = "E5E18F5CAD1F62BB276DF991F20AE6AF";

		/////////////////////////////////////////////////////////////////

		public static string ObjectFactoryNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string ObjectFactoryClass = "E825333D6467D99DD83FB850C600395C";

		/////////////////////////////////////////////////////////////////

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

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				Type type = InternalType;
				if (type == null)
					throw new Exception("Could not find internal type for SectorObjectManager");
				bool result = true;
				result &= BaseObject.HasMethod(type, ObjectManagerGetEntityHashSet);
				result &= BaseObject.HasMethod(type, ObjectManagerAddEntity);

				Type type2 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(ObjectFactoryNamespace, ObjectFactoryClass);
				if (type2 == null)
					throw new Exception("Could not find object factory type for SectorObjectManager");

				Type type3 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(EntityBaseNetManagerNamespace, EntityBaseNetManagerClass);
				if (type3 == null)
					throw new Exception("Could not find entity base network manager type for SectorObjectManager");
				result &= BaseObject.HasMethod(type3, EntityBaseNetManagerSendEntity);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine(ex);
				return false;
			}
		}

		protected override bool IsValidEntity(Object entity)
		{
			try
			{
				if (entity == null)
					return false;

				//Skip unknowns for now until we get the bugs sorted out with the other types
				Type entityType = entity.GetType();
				if (entityType != CharacterEntity.InternalType &&
					entityType != CubeGridEntity.InternalType &&
					entityType != VoxelMap.InternalType &&
					entityType != FloatingObject.InternalType &&
					entityType != Meteor.InternalType
					)
					return false;

				//Skip disposed entities
				bool isDisposed = (bool)BaseEntity.InvokeEntityMethod(entity, BaseEntity.BaseEntityGetIsDisposedMethod);
				if (isDisposed)
					return false;

				//Skip entities that have invalid physics objects
				if (BaseEntity.GetRigidBody(entity) == null || BaseEntity.GetRigidBody(entity).IsDisposed)
					return false;

				//Skip entities that don't have a position-orientation matrix defined
				if (BaseEntity.InvokeEntityMethod(entity, BaseEntity.BaseEntityGetOrientationMatrixMethod) == null)
					return false;

				return true;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return false;
			}
		}

		protected override void InternalRefreshBackingDataHashSet()
		{
			try
			{
				if (!CanRefresh)
					return;

				m_rawDataHashSetResourceLock.AcquireExclusive();

				var rawValue = BaseObject.InvokeStaticMethod(InternalType, ObjectManagerGetEntityHashSet);
				if (rawValue == null)
					return;

				//Create/Clear the hash set
				if (m_rawDataHashSet == null)
					m_rawDataHashSet = new HashSet<object>();
				else
					m_rawDataHashSet.Clear();

				//Only allow valid entities in the hash set
				foreach (var entry in UtilityFunctions.ConvertHashSet(rawValue))
				{
					if (!IsValidEntity(entry))
						continue;

					m_rawDataHashSet.Add(entry);
				}

				m_rawDataHashSetResourceLock.ReleaseExclusive();
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				if(m_rawDataHashSetResourceLock.Owned)
					m_rawDataHashSetResourceLock.ReleaseExclusive();
			}
		}

		protected override void InternalRefreshObjectBuilderMap()
		{
			try
			{
				if (!CanRefresh)
					return;

				m_rawDataHashSetResourceLock.AcquireShared();
				m_rawDataObjectBuilderListResourceLock.AcquireExclusive();

				m_rawDataObjectBuilderList.Clear();
				foreach (Object entity in GetBackingDataHashSet())
				{
					try
					{
						//TODO - Find a faster way to get updated data. This call takes ~0.15ms per entity which adds up quickly
						MyObjectBuilder_EntityBase baseEntity = BaseEntity.GetObjectBuilder(entity);
						if (baseEntity == null)
							continue;

						m_rawDataObjectBuilderList.Add(entity, baseEntity);
					}
					catch (Exception ex)
					{
						LogManager.ErrorLog.WriteLine(ex);
					}
				}

				m_rawDataHashSetResourceLock.ReleaseShared();
				m_rawDataObjectBuilderListResourceLock.ReleaseExclusive();
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				if (m_rawDataHashSetResourceLock.Owned)
					m_rawDataHashSetResourceLock.ReleaseShared();
				if (m_rawDataObjectBuilderListResourceLock.Owned)
					m_rawDataObjectBuilderListResourceLock.ReleaseExclusive();
			}
		}

		protected override void LoadDynamic()
		{
			try
			{
				Dictionary<Object, MyObjectBuilder_Base> objectBuilderList = GetObjectBuilderMap();
				HashSet<Object> rawEntities = GetBackingDataHashSet();
				Dictionary<long, BaseObject> internalDataCopy = new Dictionary<long, BaseObject>(GetInternalData());

				if (objectBuilderList.Count != rawEntities.Count)
				{
					if(SandboxGameAssemblyWrapper.IsDebugging)
						LogManager.ErrorLog.WriteLine("SectorObjectManager - Mismatch between raw entities and object builders");
					return;
				}

				//Update the main data mapping
				foreach (Object entity in rawEntities)
				{
					try
					{
						long entityId = BaseEntity.GetEntityId(entity);
						if (entityId == 0)
							continue;

						if (!IsValidEntity(entity))
							continue;

						if (!objectBuilderList.ContainsKey(entity))
							continue;

						MyObjectBuilder_EntityBase baseEntity = (MyObjectBuilder_EntityBase)objectBuilderList[entity];
						if (baseEntity == null)
							continue;
						if (!EntityRegistry.Instance.ContainsGameType(baseEntity.TypeId))
							continue;

						//If the original data already contains an entry for this, skip creation and just update values
						if (GetInternalData().ContainsKey(entityId))
						{
							BaseEntity matchingEntity = (BaseEntity)GetEntry(entityId);
							if (matchingEntity == null || matchingEntity.IsDisposed)
								continue;

							//Update the base entity (not the same as BackingObject which is the internal object)
							matchingEntity.ObjectBuilder = baseEntity;
						}
						else
						{
							BaseEntity newEntity = null;

							//Get the matching API type from the registry
							Type apiType = EntityRegistry.Instance.GetAPIType(baseEntity.TypeId);

							//Create a new API entity
							newEntity = (BaseEntity) Activator.CreateInstance(apiType, new object[] { baseEntity, entity });

							if(newEntity != null)
								AddEntry(newEntity.EntityId, newEntity);
						}
					}
					catch (Exception ex)
					{
						LogManager.ErrorLog.WriteLine(ex);
					}
				}

				//Cleanup old entities
				foreach (var entry in internalDataCopy)
				{
					try
					{
						if (!rawEntities.Contains(entry.Value.BackingObject))
							DeleteEntry(entry.Value);
					}
					catch (Exception ex)
					{
						LogManager.ErrorLog.WriteLine(ex);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		public void AddEntity(BaseEntity entity)
		{
			try
			{
				if (SandboxGameAssemblyWrapper.IsDebugging)
					Console.WriteLine(entity.GetType().Name + " '" + entity.Name + "' is being added ...");

				m_addEntityQueue.Enqueue(entity);

				Action action = InternalAddEntity;
				SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected void InternalAddEntity()
		{
			try
			{
				if (m_addEntityQueue.Count == 0)
					return;

				BaseEntity entityToAdd = m_addEntityQueue.Dequeue();

				if (SandboxGameAssemblyWrapper.IsDebugging)
					Console.WriteLine(entityToAdd.GetType().Name + " '" + entityToAdd.GetType().Name + "': Adding to scene ...");

				//Create the backing object
				Type entityType = entityToAdd.GetType();
				Type internalType = (Type)BaseEntity.InvokeStaticMethod(entityType, "get_InternalType");
				if (internalType == null)
					throw new Exception("Could not get internal type of entity");
				entityToAdd.BackingObject = Activator.CreateInstance(internalType);

				//Initialize the backing object
				BaseEntity.InvokeEntityMethod(entityToAdd.BackingObject, "Init", new object[] { entityToAdd.ObjectBuilder });

				//Add the backing object to the main game object manager
				BaseEntity.InvokeStaticMethod(InternalType, ObjectManagerAddEntity, new object[] { entityToAdd.BackingObject, true });

				if (entityToAdd is FloatingObject)
				{
					try
					{
						//Broadcast the new entity to the clients
						MyObjectBuilder_EntityBase baseEntity = (MyObjectBuilder_EntityBase)BaseEntity.InvokeEntityMethod(entityToAdd.BackingObject, BaseEntity.BaseEntityGetObjectBuilderMethod, new object[] { Type.Missing });
						//TODO - Do stuff

						entityToAdd.ObjectBuilder = baseEntity;
					}
					catch (Exception ex)
					{
						LogManager.APILog.WriteLineAndConsole("Failed to broadcast new floating object");
						LogManager.ErrorLog.WriteLine(ex);
					}
				}
				else
				{
					try
					{
						//Broadcast the new entity to the clients
						MyObjectBuilder_EntityBase baseEntity = (MyObjectBuilder_EntityBase)BaseEntity.InvokeEntityMethod(entityToAdd.BackingObject, BaseEntity.BaseEntityGetObjectBuilderMethod, new object[] { Type.Missing });
						Type someManager = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(EntityBaseNetManagerNamespace, EntityBaseNetManagerClass);
						BaseEntity.InvokeStaticMethod(someManager, EntityBaseNetManagerSendEntity, new object[] { baseEntity });

						entityToAdd.ObjectBuilder = baseEntity;
					}
					catch (Exception ex)
					{
						LogManager.APILog.WriteLineAndConsole("Failed to broadcast new entity");
						LogManager.ErrorLog.WriteLine(ex);
					}
				}

				if (SandboxGameAssemblyWrapper.IsDebugging)
				{
					Type type = entityToAdd.GetType();
					Console.WriteLine(type.Name + " '" + entityToAdd.Name + "': Finished adding to scene");
				}
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLineAndConsole("Failed to add new entity");
				LogManager.ErrorLog.WriteLine(ex);
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

		public void Load(FileInfo fileInfo)
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
			return WriteSpaceEngineersFile<MyObjectBuilder_Sector, MyObjectBuilder_SectorSerializer>(m_Sector.ObjectBuilder, this.FileInfo.FullName);
		}

		#endregion
	}
}
