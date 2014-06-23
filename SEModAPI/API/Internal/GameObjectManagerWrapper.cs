using Havok;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

using Sandbox.Common.ObjectBuilders;

using SEModAPI.API.SaveData;
using SEModAPI.API.SaveData.Entity;

using VRage;
using VRageMath;

namespace SEModAPI.API.Internal
{
	public class GameObjectManagerWrapper
	{
		#region "Attributes"

		private Thread m_mainGameThread;

		private Assembly m_assembly;

		private Type m_objectManagerType;

		private MethodInfo m_GetObjectBuilderEntities;
		private MethodInfo m_RemoveEntity;
		private MethodInfo m_GetEntityHashSet;

		#endregion

		#region "Constructors and Initializers"

		public GameObjectManagerWrapper(string basePath)
		{
			//string assemblyPath = Path.Combine(path, "Sandbox.Game.dll");
			m_assembly = Assembly.UnsafeLoadFrom("Sandbox.Game.dll");

			m_objectManagerType = m_assembly.GetType("5BCAC68007431E61367F5B2CF24E2D6F.CAF1EB435F77C7B77580E2E16F988BED");

			m_GetObjectBuilderEntities = m_objectManagerType.GetMethod("0A1670B270D5F8417447CFCBA7BF0FA8", BindingFlags.NonPublic | BindingFlags.Static);
			m_RemoveEntity = m_objectManagerType.GetMethod("E02368B53672686387A0DE0CF91F60B7", BindingFlags.Public | BindingFlags.Static);
			m_GetEntityHashSet = m_objectManagerType.GetMethod("84C54760C0F0DDDA50B0BE27B7116ED8", BindingFlags.Public | BindingFlags.Static);
		}

		#endregion

		#region "Properties"

		public Type ObjectManagerType
		{
			get { return m_objectManagerType; }
		}

		public Thread GameThread
		{
			get { return m_mainGameThread; }
			set { m_mainGameThread = value; }
		}

		#endregion

		#region "Methods"

		public List<MyObjectBuilder_EntityBase> GetObjectBuilderEntities()
		{
			return (List<MyObjectBuilder_EntityBase>)m_GetObjectBuilderEntities.Invoke(null, new object[] { });
		}

		public HashSet<Object> GetObjectManagerHashSetData()
		{
			var rawValue = m_GetEntityHashSet.Invoke(null, new object[] { });
			HashSet<Object> convertedSet = ConvertHashSet(rawValue);

			return convertedSet;
		}

		public List<CubeGrid> GetCubeGrids()
		{
			HashSet<Object> rawEntities = GetObjectManagerHashSetData();
			List<CubeGrid> cubeGridList = new List<CubeGrid>();

			foreach (Object entity in rawEntities)
			{
				Type entityType = entity.GetType();
				MethodInfo objectBuilderMethod = entityType.GetMethod("GetObjectBuilder");
				MyObjectBuilder_EntityBase baseEntity = (MyObjectBuilder_EntityBase)objectBuilderMethod.Invoke(entity, new object[] { });

				if (baseEntity.TypeId == MyObjectBuilderTypeEnum.CubeGrid)
				{
					MyObjectBuilder_CubeGrid objectBuilder = (MyObjectBuilder_CubeGrid)baseEntity;
					CubeGrid cubeGrid = new CubeGrid(objectBuilder);
					cubeGrid.BackingObject = entity;
					cubeGrid.BackingThread = GameThread;
					cubeGrid.BackingObjectManager = this;

					cubeGridList.Add(cubeGrid);
				}
			}

			return cubeGridList;
		}

		public List<CharacterEntity> GetCharacters()
		{
			HashSet<Object> rawEntities = GetObjectManagerHashSetData();
			List<CharacterEntity> characterList = new List<CharacterEntity>();

			foreach (Object entity in rawEntities)
			{
				try
				{
					MethodInfo objectBuilderMethod = GetEntityMethod(entity, "GetObjectBuilder");
					MyObjectBuilder_EntityBase baseEntity = (MyObjectBuilder_EntityBase)objectBuilderMethod.Invoke(entity, new object[] { });

					if (baseEntity.TypeId == MyObjectBuilderTypeEnum.Character)
					{
						MyObjectBuilder_Character objectBuilder = (MyObjectBuilder_Character)baseEntity;
						CharacterEntity character = new CharacterEntity(objectBuilder);
						character.BackingObject = entity;
						character.BackingThread = GameThread;
						character.BackingObjectManager = this;

						characterList.Add(character);
					}
				}
				catch (Exception ex)
				{
					//TODO - Do something about the exception here
				}
			}

			return characterList;
		}

		#region Private

		private HashSet<Object> ConvertHashSet(Object source)
		{
			Type rawType = source.GetType();
			Type[] genericArgs = rawType.GetGenericArguments();
			MethodInfo conversion = this.GetType().GetMethod("ConvertEntityHashSet", BindingFlags.NonPublic | BindingFlags.Instance);
			conversion = conversion.MakeGenericMethod(genericArgs[0]);
			HashSet<Object> result = (HashSet<Object>)conversion.Invoke(this, new object[] { source });

			return result;
		}

		private HashSet<Object> ConvertEntityHashSet<T>(IEnumerable<T> source)
		{
			HashSet<Object> dataSet = new HashSet<Object>();
			foreach (var rawEntity in source)
			{
				dataSet.Add(rawEntity);
			}

			return dataSet;
		}

		private FastResourceLock GetResourceLock()
		{
			FieldInfo lockField = m_objectManagerType.GetField("A409DEE43296E7444E7F83583E7407F9", BindingFlags.NonPublic | BindingFlags.Static);
			FastResourceLock result = (FastResourceLock)lockField.GetValue(null);

			return result;
		}

		private FieldInfo GetEntityField(Object gameEntity, string fieldName)
		{
			try
			{
				FieldInfo field = gameEntity.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
				return field;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		private MethodInfo GetEntityMethod(Object gameEntity, string methodName)
		{
			try
			{
				MethodInfo method = gameEntity.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
				return method;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		private Object InvokeEntityMethod(Object gameEntity, string methodName, Object[] parameters)
		{
			try
			{
				MethodInfo method = GetEntityMethod(gameEntity, methodName);
				Object result = method.Invoke(gameEntity, parameters);

				return result;
			}
			catch (AccessViolationException ex)
			{
				return null;
			}
			catch (TargetInvocationException ex)
			{
				return null;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		private Object GetEntityPhysicsObject(Object gameEntity)
		{
			try
			{
				MethodInfo getPhysicsObjectMethod = GetEntityMethod(gameEntity, "691FA4830C80511C934826203A251981");
				Object physicsObject = getPhysicsObjectMethod.Invoke(gameEntity, new object[] { });

				return physicsObject;
			}
			catch (Exception ex)
			{
				//TODO - Find a better way to handle an exception here
				return null;
			}
		}

		private HkRigidBody GetRigidBody(Object gameEntity)
		{
			try
			{
				Object physicsObject = GetEntityPhysicsObject(gameEntity);
				MethodInfo getRigidBodyMethod = GetEntityMethod(physicsObject, "634E5EC534E45874230868BD089055B1");
				HkRigidBody rigidBody = (HkRigidBody)getRigidBodyMethod.Invoke(physicsObject, new object[] { });

				return rigidBody;
			}
			catch (Exception ex)
			{
				//TODO - Find a better way to handle an exception here
				return null;
			}
		}

		#endregion

		#region EntityMethods

		public bool UpdateEntityId(Object gameEntity, long entityId)
		{
			try
			{
				FieldInfo entityIdField = GetEntityField(gameEntity, "F7E51DBA5F2FD0CCF8BBE66E3573BEAC");
				entityIdField.SetValue(gameEntity, entityId);

				return true;
			}
			catch (AccessViolationException ex)
			{
				return false;
			}
			catch (TargetInvocationException ex)
			{
				return false;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public bool UpdateEntityPosition(Object gameEntity, Vector3 position)
		{
			try
			{
				using (FastResourceLockExtensions.AcquireSharedUsing(GetResourceLock()))
					InvokeEntityMethod(gameEntity, "C48126915FC17C83D48E111D3AA53F85", new object[] { position });

				return true;
			}
			catch (AccessViolationException ex)
			{
				return false;
			}
			catch (TargetInvocationException ex)
			{
				return false;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public bool UpdateEntityVelocity(Object gameEntity, Vector3 velocity)
		{
			try
			{
				//if (BackingObject == null) return false;
				//Object physicsObject = GetPhysicsObject();
				//if (physicsObject == null) return false;
				//MethodInfo setLinearVelocityMethod = physicsObject.GetType().GetMethod("1BDC8CBC9E17066B1A322FCCABBF9691");
				//if (setLinearVelocityMethod == null) return false;
				//setLinearVelocityMethod.Invoke(physicsObject, new object[] { velocity });

				HkRigidBody havokBody = GetRigidBody(gameEntity);
				havokBody.LinearVelocity = velocity;

				return true;
			}
			catch (AccessViolationException ex)
			{
				//TODO - Find a better way to handle an exception here
				return false;
			}
			catch (TargetInvocationException ex)
			{
				//TODO - Find a better way to handle an exception here
				return false;
			}
			catch (Exception ex)
			{
				//TODO - Find a better way to handle an exception here
				return false;
			}
		}

		protected void RunEntityAction(Object gameEntity, Action action)
		{
			//TODO - Find out which of the Action<T> fields is the right one to use
			InvokeEntityMethod(gameEntity, "04F6493DF187FBA38C2B379BA9484304", new object[] { action });
		}

		public bool RemoveEntity(Object gameEntity)
		{
			//TODO - There is more to this than just 1 call. Find out what else
			//m_RemoveEntity.Invoke(null, new object[] { gameEntity });

			return false;
		}

		public Matrix GetEntityPositionOrientationMatrix(Object gameEntity)
		{
			try
			{
				MethodInfo getMatrixMethod = GetEntityMethod(gameEntity, "A6F18C43CE18BB2AFD5A747779494087");
				Matrix result = (Matrix)getMatrixMethod.Invoke(gameEntity, new object[] { });

				return result;
			}
			catch (Exception ex)
			{
				//TODO - Find a better way to handle an exception here
				return Matrix.Identity;
			}
		}

		public bool SetEntityPositionOrientationMatrix(Object gameEntity, Matrix source)
		{
			try
			{
				//TODO - Check if this is the correct method for setting entity matrix
				InvokeEntityMethod(gameEntity, "48A0FC587D5A503495FA5105B828D18E", new object[] { source });

				return true;
			}
			catch (Exception ex)
			{
				//TODO - Find a better way to handle an exception here
				return false;
			}
		}

		#endregion

		#region CharacterEntity

		public float GetCharacterHealth(CharacterEntity character)
		{
			float health = (float)InvokeEntityMethod(character.BackingObject, "7047AFF5D44FC8A44572E92DBAD13011", new object[] { });

			return health;
		}

		public bool UpdateCharacterHealth(CharacterEntity character, float health)
		{
			try
			{
				InvokeEntityMethod(character.BackingObject, "92A0500FD8772AB1AC3A6F79FD2A1C72", new object[] { health });

				return true;
			}
			catch (Exception ex)
			{
				//TODO - Find a better way to handle an exception here
				return false;
			}
		}

		public Object GetCharacterBattery(CharacterEntity character)
		{
			Object battery = InvokeEntityMethod(character.BackingObject, "CF72A89940254CB8F535F177150FC743", new object[] { });

			return battery;
		}

		public bool UpdateCharacterBatteryLevel(CharacterEntity character, float capacity)
		{
			try
			{
				Object battery = GetCharacterBattery(character);
				InvokeEntityMethod(battery, "C3BF60F3540A8A48CB8FEE0CDD3A95C6", new object[] { capacity });

				return true;
			}
			catch (Exception ex)
			{
				//TODO - Find a better way to handle an exception here
				return false;
			}
		}

		#endregion

		#endregion
	}
}
