using Havok;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Voxels;
using Sandbox.Game.Weapons;

using SEModAPI.API.SaveData;
using SEModAPI.API.SaveData.Entity;

using VRage;
using VRageMath;

namespace SEModAPI.API.Internal
{
	public class GameObjectManagerWrapper : BaseInternalWrapper
	{
		#region "Attributes"

		protected new static GameObjectManagerWrapper m_instance;

		private Thread m_mainGameThread;

		private static Assembly m_assembly;

		private static Type m_objectManagerType;
		private static Type m_entityBaseType;

		private MethodInfo m_GetEntityHashSet;

		//TODO - Build some sort of Dictionary based structure to hold these temp values
		private static Vector3 m_nextEntityPosition;
		private static Vector3 m_nextEntityVelocity;
		private static Vector3 m_nextEntityAngularVelocity;
		private static Vector3 m_nextEntityUp;
		private static Vector3 m_nextEntityForward;
		private static Object m_nextEntityToUpdate;

		public static string ObjectManagerClass = "5BCAC68007431E61367F5B2CF24E2D6F.CAF1EB435F77C7B77580E2E16F988BED";
		public static string ObjectManagerAction1 = "E017E9CA31926307661D7A6B465C8F96";	//() Object Manager shut down?
		public static string ObjectManagerEntityAction1 = "30E511FF32960AE853909500461285C4";	//(GameEntity) Entity-Close()
		public static string ObjectManagerEntityAction2 = "8C1807427F2EEF4DF981396C4E6A42DD";	//(GameEntity, string, string) Entity-Init()
		public static string ObjectManagerGetResourceLock = "6EF7F983A8061B40A5606D75C890AF07";
		public static string ObjectManagerGetEntityHashSet = "84C54760C0F0DDDA50B0BE27B7116ED8";
		public static string ObjectManagerAddEntity = "E5E18F5CAD1F62BB276DF991F20AE6AF";

		public static string NetworkSerializerClass = "5F381EA9388E0A32A8C817841E192BE8.8EFE49A46AB934472427B7D117FD3C64";
		public static string NetworkSerializerSendEntity = "A6B585C993B43E72219511726BBB0649";

		public static string UtilityClass = "5BCAC68007431E61367F5B2CF24E2D6F.226D9974B43A7269CDD3E322CC8110D5";
		public static string UtilityGenerateEntityId = "3B4924802BEBD1AE13B29920376CE914";

		public static string GameEntityClass = "5BCAC68007431E61367F5B2CF24E2D6F.F6DF01EE4159339113BB9650DEEE1913";
		public static string EntityAction1 = "8CAF5306D8DF29E8140056369D0F1FC1";	//(GameEntity) OnWorldPositionChanged
		public static string EntityAction2 = "1CF14BA21D05D5F9AB6993170E4838FE";	//(GameEntity) UpdateAfterSim - Only if certain flags are set on cube blocks, not sure what yet
		public static string EntityAction3 = "183620F2B4C14EFFC9F34BFBCF35ABCC";	//(GameEntity) ??
		public static string EntityAction4 = "6C1670C128F0A838E0BE20B6EB3FB7C4";	//(GameEntity) ??
		public static string EntityAction5 = "FA752E85660B6101F92B340B994C0F29";	//(GameEntity) ??
		public static string EntityPhysicsObject = "691FA4830C80511C934826203A251981";
		public static string EntityEntityId = "F7E51DBA5F2FD0CCF8BBE66E3573BEAC";

		public static string PhysicsObjectGetRigidBody = "634E5EC534E45874230868BD089055B1";

		#endregion

		#region "Constructors and Initializers"

		protected GameObjectManagerWrapper(string basePath)
			: base(basePath)
		{
			m_instance = this;

			//string assemblyPath = Path.Combine(path, "Sandbox.Game.dll");
			m_assembly = Assembly.UnsafeLoadFrom("Sandbox.Game.dll");

			m_objectManagerType = m_assembly.GetType(ObjectManagerClass);
			m_entityBaseType = m_assembly.GetType(GameEntityClass);

			Console.WriteLine("Finished loading GameObjectManagerWrapper");
		}

		new public static GameObjectManagerWrapper GetInstance(string basePath = "")
		{
			if (m_instance == null)
			{
				m_instance = new GameObjectManagerWrapper(basePath);
			}
			return (GameObjectManagerWrapper)m_instance;
		}

		#endregion

		#region "Properties"

		public static Type ObjectManagerType
		{
			get { return m_objectManagerType; }
		}

		public static Type BaseEntityType
		{
			get { return m_entityBaseType; }
		}

		public Thread GameThread
		{
			get { return m_mainGameThread; }
			set { m_mainGameThread = value; }
		}

		new public static bool IsDebugging
		{
			get
			{
				GameObjectManagerWrapper.GetInstance();
				return m_isDebugging;
			}
			set
			{
				GameObjectManagerWrapper.GetInstance();
				m_isDebugging = value;
			}
		}

		#endregion

		#region "Methods"

		public HashSet<Object> GetObjectManagerHashSetData()
		{
			try
			{
				MethodInfo getEntityHashSet = m_objectManagerType.GetMethod(ObjectManagerGetEntityHashSet, BindingFlags.Public | BindingFlags.Static);
				var rawValue = getEntityHashSet.Invoke(null, new object[] { });
				HashSet<Object> convertedSet = ConvertHashSet(rawValue);

				return convertedSet;
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				return null;
			}
		}

		#region APIEntityLists

		private List<T> GetAPIEntityList<T, TO>(MyObjectBuilderTypeEnum type)
			where TO : MyObjectBuilder_EntityBase
			where T : SectorObject<TO>
		{
			HashSet<Object> rawEntities = GetObjectManagerHashSetData();
			List<T> list = new List<T>();

			//SetupObjectManagerEventHandlers();

			foreach (Object entity in rawEntities)
			{
				try
				{
					MyObjectBuilder_EntityBase baseEntity = (MyObjectBuilder_EntityBase)InvokeEntityMethod(entity, "GetObjectBuilder", new object[] { });

					if (baseEntity.TypeId == type)
					{
						TO objectBuilder = (TO)baseEntity;
						T apiEntity = (T)Activator.CreateInstance(typeof(T), new object[] { objectBuilder });
						apiEntity.BackingObject = entity;
						apiEntity.BackingThread = GameThread;

						//SetupEntityEventHandlers(entity);

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

		public List<CubeGrid> GetCubeGrids()
		{
			return GetAPIEntityList<CubeGrid, MyObjectBuilder_CubeGrid>(MyObjectBuilderTypeEnum.CubeGrid);
		}

		public List<CharacterEntity> GetCharacters()
		{
			return GetAPIEntityList<CharacterEntity, MyObjectBuilder_Character>(MyObjectBuilderTypeEnum.Character);
		}

		public List<VoxelMap> GetVoxelMaps()
		{
			return GetAPIEntityList<VoxelMap, MyObjectBuilder_VoxelMap>(MyObjectBuilderTypeEnum.VoxelMap);
		}

		public List<FloatingObject> GetFloatingObjects()
		{
			return GetAPIEntityList<FloatingObject, MyObjectBuilder_FloatingObject>(MyObjectBuilderTypeEnum.FloatingObject);
		}

		public List<Meteor> GetMeteors()
		{
			return GetAPIEntityList<Meteor, MyObjectBuilder_Meteor>(MyObjectBuilderTypeEnum.Meteor);
		}

		#endregion

		#region Private

		private static FastResourceLock GetResourceLock()
		{
			try
			{
				FieldInfo field = m_objectManagerType.GetField(ObjectManagerGetResourceLock, BindingFlags.Public | BindingFlags.Static);
				FastResourceLock resourceLock = (FastResourceLock)field.GetValue(null);

				return resourceLock;
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				return null;
			}
		}

		private static Object GetEntityPhysicsObject(Object gameEntity)
		{
			try
			{
				MethodInfo getPhysicsObjectMethod = GetEntityMethod(gameEntity, EntityPhysicsObject);
				Object physicsObject = getPhysicsObjectMethod.Invoke(gameEntity, new object[] { });

				return physicsObject;
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				return null;
			}
		}

		private static HkRigidBody GetRigidBody(Object gameEntity)
		{
			try
			{
				Object physicsObject = GetEntityPhysicsObject(gameEntity);
				MethodInfo getRigidBodyMethod = GetEntityMethod(physicsObject, PhysicsObjectGetRigidBody);
				HkRigidBody rigidBody = (HkRigidBody)getRigidBodyMethod.Invoke(physicsObject, new object[] { });

				return rigidBody;
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				return null;
			}
		}

		#endregion

		#region EntityMethods

		public static long GenerateEntityId()
		{
			try
			{
				Type utilityType = m_assembly.GetType(UtilityClass);
				MethodInfo generateIdMethod = utilityType.GetMethod(UtilityGenerateEntityId, BindingFlags.Public | BindingFlags.Static);
				long entityId = (long)generateIdMethod.Invoke(null, new object[] { });

				return entityId;
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				return 0;
			}
		}

		#region "Setup"

		public bool SetupObjectManagerEventHandlers()
		{
			try
			{
				FieldInfo actionField = m_objectManagerType.GetField(ObjectManagerEntityAction1, BindingFlags.NonPublic | BindingFlags.Static);
				Action<Object> newAction1 = ObjectManagerEntityEvent1;
				actionField.SetValue(null, newAction1);
				
				actionField = m_objectManagerType.GetField(ObjectManagerEntityAction2, BindingFlags.NonPublic | BindingFlags.Static);
				Action<Object, string, string> newAction2 = ObjectManagerEntityEvent2;
				actionField.SetValue(null, newAction2);

				FieldInfo actionField3 = m_objectManagerType.GetField(ObjectManagerAction1, BindingFlags.NonPublic | BindingFlags.Static);
				Action newAction3 = ObjectManagerEvent1;
				actionField3.SetValue(null, newAction3);
				
				return true;
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public bool SetupEntityEventHandlers(Object gameEntity)
		{
			try
			{
				FieldInfo actionField = GetEntityField(gameEntity, EntityAction1);
				Action<Object> newAction = EntityOnPositionChanged;
				actionField.SetValue(gameEntity, newAction);
				
				FieldInfo actionField2 = GetEntityField(gameEntity, EntityAction2);
				Action<Object> newAction2 = EntityEvent2;
				actionField2.SetValue(gameEntity, newAction2);
				
				FieldInfo actionField3 = GetEntityField(gameEntity, EntityAction3);
				Action<Object> newAction3 = EntityEvent3;
				actionField3.SetValue(gameEntity, newAction3);
				
				FieldInfo actionField4 = GetEntityField(gameEntity, EntityAction4);
				Action<Object> newAction4 = EntityEvent4;
				actionField4.SetValue(gameEntity, newAction4);
				/*
				FieldInfo actionField5 = GetEntityField(gameEntity, EntityAction5);
				Action<Object> newAction5 = EntityEvent5;
				actionField5.SetValue(gameEntity, newAction5);
				*/

				return true;
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		#endregion

		#region "Updates"

		public static bool UpdateEntityId(Object gameEntity, long entityId)
		{
			try
			{
				FieldInfo entityIdField = GetEntityField(gameEntity, EntityEntityId);
				entityIdField.SetValue(gameEntity, entityId);

				return true;
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public bool UpdateEntityPosition(Object gameEntity, Vector3 position)
		{
			try
			{
				m_nextEntityPosition = position;
				m_nextEntityToUpdate = gameEntity;

				Action action = InternalUpdateEntityPosition;
				SandboxGameAssemblyWrapper.EnqueueMainGameAction(action);

				return true;
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public bool UpdateEntityVelocity(Object gameEntity, Vector3 velocity)
		{
			try
			{
				m_nextEntityVelocity = velocity;
				m_nextEntityToUpdate = gameEntity;

				Action action = InternalUpdateEntityVelocity;
				SandboxGameAssemblyWrapper.EnqueueMainGameAction(action);

				return true;
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public bool UpdateEntityAngularVelocity(Object gameEntity, Vector3 velocity)
		{
			try
			{
				m_nextEntityAngularVelocity = velocity;
				m_nextEntityToUpdate = gameEntity;

				Action action = InternalUpdateEntityAngularVelocity;
				SandboxGameAssemblyWrapper.EnqueueMainGameAction(action);

				return true;
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public bool UpdateEntityUp(Object gameEntity, Vector3 up)
		{
			try
			{
				m_nextEntityUp = up;
				m_nextEntityToUpdate = gameEntity;

				Action action = InternalUpdateEntityUp;
				SandboxGameAssemblyWrapper.EnqueueMainGameAction(action);

				return true;
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public bool UpdateEntityForward(Object gameEntity, Vector3 forward)
		{
			try
			{
				m_nextEntityForward = forward;
				m_nextEntityToUpdate = gameEntity;

				Action action = InternalUpdateEntityForward;
				SandboxGameAssemblyWrapper.EnqueueMainGameAction(action);

				return true;
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public bool AddEntity(Object gameEntity)
		{
			try
			{
				m_nextEntityToUpdate = gameEntity;

				Action action = InternalAddEntity;
				SandboxGameAssemblyWrapper.EnqueueMainGameAction(action);

				return true;
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public bool RemoveEntity(Object gameEntity)
		{
			try
			{
				m_nextEntityToUpdate = gameEntity;

				Action action = InternalRemoveEntity;
				SandboxGameAssemblyWrapper.EnqueueMainGameAction(action);

				return true;
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		#endregion

		#region "Actions"

		public static void ObjectManagerEvent1()
		{
			try
			{
				if (m_isDebugging)
					Console.WriteLine("ObjectManagerEvent - '1'");

				FieldInfo actionField = m_objectManagerType.GetField(ObjectManagerAction1, BindingFlags.NonPublic | BindingFlags.Static);
				Action newAction = ObjectManagerEvent1;
				actionField.SetValue(null, newAction);
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
			}
		}

		public static void ObjectManagerEntityEvent1(Object gameEntity)
		{
			try
			{
				if (m_isDebugging)
					Console.WriteLine("Entity '" + GetEntityId(gameEntity).ToString() + "': ObjectManagerEvent - Entity Closed");

				FieldInfo actionField = m_objectManagerType.GetField(ObjectManagerEntityAction1, BindingFlags.NonPublic | BindingFlags.Static);
				Action<Object> newAction = ObjectManagerEntityEvent1;
				actionField.SetValue(null, newAction);
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
			}
		}

		public static void ObjectManagerEntityEvent2(Object gameEntity, string oldKey, string currentKey)
		{
			try
			{
				if (m_isDebugging)
					Console.WriteLine("Entity '" + GetEntityId(gameEntity).ToString() + "': ObjectManagerEvent - Entity Initialized");

				FieldInfo actionField = m_objectManagerType.GetField(ObjectManagerEntityAction2, BindingFlags.NonPublic | BindingFlags.Static);
				Action<Object, string, string> newAction = ObjectManagerEntityEvent2;
				actionField.SetValue(null, newAction);
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
			}
		}

		public static void EntityOnPositionChanged(Object gameEntity)
		{
			try
			{
				//if(m_isDebugging)
				//Console.WriteLine("Entity '" + GetEntityId(gameEntity).ToString() + "': Event 'PositionChanged' triggered");

				//Make sure the action is still tied to this event handler
				//FieldInfo actionField = GetEntityField(gameEntity, EntityAction1);
				//Action<Object> newAction = EntityOnPositionChanged;
				//actionField.SetValue(gameEntity, newAction);
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
			}
		}

		public static void EntityEvent2(Object gameEntity)
		{
			try
			{
				if (m_isDebugging)
					Console.WriteLine("Entity '" + GetEntityId(gameEntity).ToString() + "': Event '2' triggered");

				FieldInfo actionField = GetEntityField(gameEntity, EntityAction2);
				Action<Object> newAction = EntityEvent2;
				actionField.SetValue(gameEntity, newAction);
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
			}
		}

		public static void EntityEvent3(Object gameEntity)
		{
			try
			{
				if (m_isDebugging)
					Console.WriteLine("Entity '" + GetEntityId(gameEntity).ToString() + "': Event '3' triggered");

				FieldInfo actionField = GetEntityField(gameEntity, EntityAction3);
				Action<Object> newAction = EntityEvent3;
				actionField.SetValue(gameEntity, newAction);
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
			}
		}

		public static void EntityEvent4(Object gameEntity)
		{
			try
			{
				if (m_isDebugging)
					Console.WriteLine("Entity '" + GetEntityId(gameEntity).ToString() + "': Event '4' triggered");

				FieldInfo actionField = GetEntityField(gameEntity, EntityAction4);
				Action<Object> newAction = EntityEvent4;
				actionField.SetValue(gameEntity, newAction);
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
			}
		}

		public static void EntityEvent5(Object gameEntity)
		{
			try
			{
				if (m_isDebugging)
					Console.WriteLine("Entity '" + GetEntityId(gameEntity).ToString() + "': Event '5' triggered");

				FieldInfo actionField = GetEntityField(gameEntity, EntityAction5);
				Action<Object> newAction = EntityEvent5;
				actionField.SetValue(gameEntity, newAction);
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
			}
		}

		public static void InternalUpdateEntityPosition()
		{
			try
			{
				if (m_nextEntityToUpdate == null)
					return;
				if (GetEntityId(m_nextEntityToUpdate) == 0)
					return;

				if(m_isDebugging)
					Console.WriteLine("Entity '" + GetEntityId(m_nextEntityToUpdate).ToString() + "': Updating position to " + m_nextEntityPosition.ToString());

				HkRigidBody havokBody = GetRigidBody(m_nextEntityToUpdate);
				havokBody.Position = m_nextEntityPosition;
				m_nextEntityPosition = Vector3.Zero;
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
			}
		}

		public static void InternalUpdateEntityVelocity()
		{
			try
			{
				if (m_nextEntityToUpdate == null)
					return;
				if (GetEntityId(m_nextEntityToUpdate) == 0)
					return;

				if (m_isDebugging)
					Console.WriteLine("Entity '" + GetEntityId(m_nextEntityToUpdate).ToString() + "': Updating velocity to " + m_nextEntityVelocity.ToString());

				HkRigidBody havokBody = GetRigidBody(m_nextEntityToUpdate);
				havokBody.LinearVelocity = m_nextEntityVelocity;
				m_nextEntityVelocity = Vector3.Zero;
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
			}
		}

		public static void InternalUpdateEntityAngularVelocity()
		{
			try
			{
				if (m_nextEntityToUpdate == null)
					return;
				if (GetEntityId(m_nextEntityToUpdate) == 0)
					return;

				if (m_isDebugging)
					Console.WriteLine("Entity '" + GetEntityId(m_nextEntityToUpdate).ToString() + "': Updating angular velocity to " + m_nextEntityAngularVelocity.ToString());

				HkRigidBody havokBody = GetRigidBody(m_nextEntityToUpdate);
				havokBody.AngularVelocity = m_nextEntityAngularVelocity;
				m_nextEntityAngularVelocity = Vector3.Zero;
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
			}
		}

		public static void InternalUpdateEntityUp()
		{
			try
			{
				if (m_nextEntityToUpdate == null)
					return;
				if (GetEntityId(m_nextEntityToUpdate) == 0)
					return;

				if (m_isDebugging)
					Console.WriteLine("Entity '" + GetEntityId(m_nextEntityToUpdate).ToString() + "': Updating 'up' to " + m_nextEntityUp.ToString());

				HkRigidBody havokBody = GetRigidBody(m_nextEntityToUpdate);
				//TODO - Figure out how to set the rotation from the 'up' vector and the existing 'forward' vector
				m_nextEntityUp = Vector3.Zero;
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
			}
		}

		public static void InternalUpdateEntityForward()
		{
			try
			{
				if (m_nextEntityToUpdate == null)
					return;
				if (GetEntityId(m_nextEntityToUpdate) == 0)
					return;

				if (m_isDebugging)
					Console.WriteLine("Entity '" + GetEntityId(m_nextEntityToUpdate).ToString() + "': Updating 'forward' to " + m_nextEntityForward.ToString());

				HkRigidBody havokBody = GetRigidBody(m_nextEntityToUpdate);
				//TODO - Figure out how to set the rotation from the 'forward' vector and the existing 'up' vector
				m_nextEntityForward = Vector3.Zero;
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
			}
		}

		public static void InternalRemoveEntity()
		{
			try
			{
				if (m_nextEntityToUpdate == null)
					return;
				if (GetEntityId(m_nextEntityToUpdate) == 0)
					return;

				if (m_isDebugging)
					Console.WriteLine("Entity '" + GetEntityId(m_nextEntityToUpdate).ToString() + "': Calling 'Close'");

				MethodInfo removeEntityMethod = m_nextEntityToUpdate.GetType().GetMethod("Close", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
				removeEntityMethod.Invoke(m_nextEntityToUpdate, new object[] { });
				//InvokeEntityMethod(m_nextEntityToUpdate, "Close");

				//TODO - Figure out what needs to be called to broadcast the removal to the clients

				m_nextEntityToUpdate = null;
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
			}
		}

		public static void InternalAddEntity()
		{
			try
			{
				if (m_nextEntityToUpdate == null)
					return;
				if (GetEntityId(m_nextEntityToUpdate) == 0)
					return;

				if (m_isDebugging)
					Console.WriteLine("Entity '" + GetEntityId(m_nextEntityToUpdate).ToString() + "': Adding to scene ...");

				MethodInfo addEntityMethod = m_objectManagerType.GetMethod(ObjectManagerAddEntity, BindingFlags.Public | BindingFlags.Static);
				addEntityMethod.Invoke(null, new object[] { m_nextEntityToUpdate, true });

				MyObjectBuilder_EntityBase baseEntity = (MyObjectBuilder_EntityBase)InvokeEntityMethod(m_nextEntityToUpdate, "GetObjectBuilder", new object[] { });
				Type someManager = m_assembly.GetType(NetworkSerializerClass);
				MethodInfo sendEntityMethod = someManager.GetMethod(NetworkSerializerSendEntity, BindingFlags.Public | BindingFlags.Static);
				sendEntityMethod.Invoke(null, new object[] { baseEntity });

				m_nextEntityToUpdate = null;
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
			}
		}

		#endregion

		#endregion

		#endregion
	}
}
