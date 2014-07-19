using Havok;

using Microsoft.Xml.Serialization.GeneratedAssembly;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.Voxels;
using Sandbox.Common.ObjectBuilders.VRageData;
using Sandbox.Game.Weapons;

using SEModAPI.API;
using SEModAPI.API.Definitions;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.API.Utility;
using SEModAPIInternal.Support;

using VRage;
using VRageMath;

namespace SEModAPIInternal.API.Entity
{
	public class BaseEntity : BaseObject
	{
		#region "Attributes"

		private float m_maxLinearVelocity;
		private static Type m_internalType;

		private MyPositionAndOrientation m_positionOrientation;
		private Vector3 m_genericLinearVelocity;
		private Vector3 m_genericAngularVelocity;

		public static string BaseEntityNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string BaseEntityClass = "F6DF01EE4159339113BB9650DEEE1913";
		public static string BaseEntityGetObjectBuilderMethod = "GetObjectBuilder";
		public static string BaseEntityGetPhysicsManagerMethod = "691FA4830C80511C934826203A251981";
		public static string BaseEntityGetEntityIdMethod = "53C3FFA07960404AABBEAAF931E5487E";
		public static string BaseEntityCombineOnMovedEventMethod = "04F6493DF187FBA38C2B379BA9484304";
		public static string BaseEntityCombineOnClosedEventMethod = "C1704F26C9D5D7EBE19DC78AB8923F4E";
		public static string BaseEntityGetIsDisposedMethod = "6D8F627C1C0F9F166031C3B600FEDA60";
		public static string BaseEntityEntityIdField = "F7E51DBA5F2FD0CCF8BBE66E3573BEAC";
		public static string BaseEntityGetOrientationMatrixMethod = "FD50436D896ACC794550210055349FE0";

		public static string PhysicsManagerGetRigidBodyMethod = "634E5EC534E45874230868BD089055B1";

		#endregion

		#region "Constructors and Initializers"

		public BaseEntity(MyObjectBuilder_EntityBase baseEntity)
			: base(baseEntity)
		{
			m_positionOrientation = baseEntity.PositionAndOrientation.GetValueOrDefault();
			m_genericLinearVelocity = new Vector3();
			m_genericAngularVelocity = new Vector3();
			m_maxLinearVelocity = (float)104.7;
		}

		public BaseEntity(MyObjectBuilder_EntityBase baseEntity, Object backingObject)
			: base(baseEntity, backingObject)
		{
			m_positionOrientation = baseEntity.PositionAndOrientation.GetValueOrDefault();
			m_genericLinearVelocity = new Vector3();
			m_genericAngularVelocity = new Vector3();
			m_maxLinearVelocity = (float)104.7;

			Action action = InternalRegisterEntityMovedEvent;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		#endregion

		#region "Properties"

		[Browsable(false)]
		[ReadOnly(true)]
		internal static Type InternalType
		{
			get
			{
				if (m_internalType == null)
					m_internalType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(BaseEntityNamespace, BaseEntityClass);
				return m_internalType;
			}
		}

		/// <summary>
		/// Gets the formatted name of an entity
		/// </summary>
		[Category("Entity")]
		[Browsable(true)]
		[ReadOnly(true)]
		[Description("Formatted Name of an entity")]
		public override string Name
		{
			get
			{
				return GetSubTypeEntity().Name == "" ? GetSubTypeEntity().TypeId.ToString() : GetSubTypeEntity().EntityId.ToString();
			}
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

				if (BackingObject != null)
				{
					Action action = InternalUpdateBaseEntity;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Entity")]
		[ReadOnly(true)]
		public MyPersistentEntityFlags2 PersistentFlags
		{
			get { return GetSubTypeEntity().PersistentFlags; }
		}

		[Category("Entity")]
		[Browsable(false)]
		protected MyPositionAndOrientation PositionAndOrientation
		{
			get
			{
				if (BackingObject != null)
				{
					HkRigidBody body = GetRigidBody();
					if (body != null && !body.IsDisposed)
					{
						m_positionOrientation.Position = body.Position;
						//TODO - Figure out how to transform the orientation quaternion to the position-orientation matrix
					}
				}

				return m_positionOrientation;
			}
			set
			{
				m_positionOrientation = value;
				GetSubTypeEntity().PositionAndOrientation = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdatePosition;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
					Action action2 = InternalUpdateOrientation;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action2);
				}
			}
		}

		[Category("Entity")]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public Vector3Wrapper Position
		{
			get { return PositionAndOrientation.Position; }
			set
			{
				m_positionOrientation.Position = value;
				GetSubTypeEntity().PositionAndOrientation = m_positionOrientation;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdatePosition;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Entity")]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public Vector3Wrapper Up
		{
			get { return PositionAndOrientation.Up; }
			set
			{
				m_positionOrientation.Up = value;
				GetSubTypeEntity().PositionAndOrientation = m_positionOrientation;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateOrientation;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Entity")]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public Vector3Wrapper Forward
		{
			get { return PositionAndOrientation.Forward; }
			set
			{
				m_positionOrientation.Forward = value;
				GetSubTypeEntity().PositionAndOrientation = m_positionOrientation;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateOrientation;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Entity")]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public virtual Vector3Wrapper LinearVelocity
		{
			get
			{
				if (BackingObject != null)
				{
					HkRigidBody body = GetRigidBody();
					if(body != null && !body.IsDisposed)
						m_genericLinearVelocity = body.LinearVelocity;
				}

				return m_genericLinearVelocity;
			}
			set
			{
				m_genericLinearVelocity = value;

				if (BackingObject != null)
				{
					Action action = InternalUpdateLinearVelocity;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Entity")]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public virtual Vector3Wrapper AngularVelocity
		{
			get
			{
				if (BackingObject != null)
				{
					HkRigidBody body = GetRigidBody();
					if (body != null && !body.IsDisposed)
						m_genericAngularVelocity = body.AngularVelocity;
				}

				return m_genericAngularVelocity;
			}
			set
			{
				m_genericAngularVelocity = value;

				if (BackingObject != null)
				{
					Action action = InternalUpdateAngularVelocity;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Entity")]
		public float MaxLinearVelocity
		{
			get { return m_maxLinearVelocity; }
			set
			{
				if (m_maxLinearVelocity == value) return;
				m_maxLinearVelocity = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateBaseEntity;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Entity")]
		[ReadOnly(true)]
		public float Mass
		{
			get
			{
				if (BackingObject == null)
					return 0;

				try
				{
					HkRigidBody body = GetRigidBody();
					if (body == null || body.IsDisposed)
						return 0;

					float mass = body.Mass;
					return mass;
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
					return 0;
				}
			}
		}

		#endregion

		#region "Methods"

		new public void Dispose()
		{
			if (!IsDisposed && BackingObject != null)
			{
				m_isDisposed = true;

				Vector3 currentPosition = Position;
				currentPosition = Vector3.Add(currentPosition, new Vector3(10000000, 10000000, 10000000));
				Position = currentPosition;

				Action action = InternalUpdatePosition;
				SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);

				Thread.Sleep(100);

				Action action2 = InternalRemoveEntity;
				SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action2);

				EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent();
				newEvent.type = EntityEventManager.EntityEventType.OnBaseEntityDeleted;
				newEvent.timestamp = DateTime.Now;
				newEvent.entity = this;
				newEvent.priority = 1;
				EntityEventManager.Instance.AddEvent(newEvent);
			}

			base.Dispose();
		}

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
		new internal MyObjectBuilder_EntityBase GetSubTypeEntity()
		{
			return (MyObjectBuilder_EntityBase)BaseEntity;
		}

		public override void Export(FileInfo fileInfo)
		{
			BaseEntityManager.SaveContentFile<MyObjectBuilder_EntityBase, MyObjectBuilder_EntityBaseSerializer>(GetSubTypeEntity(), fileInfo);
		}

		#region "Internal"

		private Object GetEntityPhysicsObject()
		{
			try
			{
				Object physicsObject = InvokeEntityMethod(BackingObject, BaseEntityGetPhysicsManagerMethod);

				return physicsObject;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		private HkRigidBody GetRigidBody()
		{
			try
			{
				Object physicsObject = GetEntityPhysicsObject();
				if (physicsObject == null)
					return null;
				HkRigidBody rigidBody = (HkRigidBody)InvokeEntityMethod(physicsObject, PhysicsManagerGetRigidBodyMethod);

				return rigidBody;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		private static Object GetEntityPhysicsObject(Object entity)
		{
			try
			{
				Object physicsObject = InvokeEntityMethod(entity, BaseEntityGetPhysicsManagerMethod);

				return physicsObject;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		internal static HkRigidBody GetRigidBody(Object entity)
		{
			try
			{
				Object physicsObject = GetEntityPhysicsObject(entity);
				if (physicsObject == null)
					return null;
				HkRigidBody rigidBody = (HkRigidBody)InvokeEntityMethod(physicsObject, PhysicsManagerGetRigidBodyMethod);

				return rigidBody;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		protected void InternalUpdateBaseEntity()
		{
			try
			{
				//TODO - Change this to a method to set the entity id instead of just setting the field
				FieldInfo entityIdField = GetEntityField(BackingObject, BaseEntityEntityIdField);
				entityIdField.SetValue(BackingObject, EntityId);

				HkRigidBody havokBody = GetRigidBody();
				if (havokBody == null)
					return;
				havokBody.MaxLinearVelocity = MaxLinearVelocity;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected void InternalUpdatePosition()
		{
			try
			{
				HkRigidBody havokBody = GetRigidBody();
				if (havokBody == null)
					return;

				Vector3 newPosition = (Vector3)GetSubTypeEntity().PositionAndOrientation.GetValueOrDefault().Position;

				if (SandboxGameAssemblyWrapper.IsDebugging)
				{
					LogManager.APILog.WriteLineAndConsole("Entity - Changing position of '" + Name + "' from '" + havokBody.Position.ToString() + "' to '" + newPosition.ToString() + "'");
				}

				havokBody.Position = newPosition;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected void InternalUpdateOrientation()
		{
			try
			{
				HkRigidBody havokBody = GetRigidBody();
				if (havokBody == null)
					return;
				
				//TODO - Finish this
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected void InternalUpdateLinearVelocity()
		{
			try
			{
				HkRigidBody havokBody = GetRigidBody();
				if (havokBody == null)
					return;

				Vector3 newVelocity = (Vector3)LinearVelocity;

				if (SandboxGameAssemblyWrapper.IsDebugging)
				{
					LogManager.APILog.WriteLineAndConsole("Entity - Changing linear velocity of '" + Name + "' from '" + havokBody.LinearVelocity.ToString() + "' to '" + newVelocity.ToString() + "'");
				}

				havokBody.LinearVelocity = newVelocity;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected void InternalUpdateAngularVelocity()
		{
			try
			{
				HkRigidBody havokBody = GetRigidBody();
				if (havokBody == null)
					return;

				Vector3 newVelocity = (Vector3)AngularVelocity;

				if (SandboxGameAssemblyWrapper.IsDebugging)
				{
					LogManager.APILog.WriteLineAndConsole("Entity - Changing angular velocity of '" + Name + "' from '" + havokBody.AngularVelocity.ToString() + "' to '" + newVelocity.ToString() + "'");
				}

				havokBody.AngularVelocity = newVelocity;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected void InternalRemoveEntity()
		{
			try
			{
				if (SandboxGameAssemblyWrapper.IsDebugging)
					LogManager.APILog.WriteLineAndConsole("Entity '" + EntityId.ToString() + "': Calling 'Close' to remove entity");

				BaseObject.InvokeEntityMethod(BackingObject, "Close");

				//TODO - Figure out what needs to be called to fully broadcast the removal to the clients
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLineAndConsole("Failed to remove entity '" + EntityId.ToString() + "'");
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected void InternalRegisterEntityMovedEvent()
		{
			try
			{
				Action<Object> action = InternalEntityMovedEvent;

				MethodInfo method = BackingObject.GetType().GetMethod(BaseEntityCombineOnMovedEventMethod);
				method.Invoke(BackingObject, new object[] { action });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected void InternalEntityMovedEvent(Object entity)
		{
			try
			{
				if (IsDisposed)
					return;

				EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent();
				newEvent.type = EntityEventManager.EntityEventType.OnBaseEntityMoved;
				newEvent.timestamp = DateTime.Now;
				newEvent.entity = this;
				newEvent.priority = 10;
				EntityEventManager.Instance.AddEvent(newEvent);
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#endregion

		#endregion
	}

	public class BaseEntityManager : BaseObjectManager
	{
		#region "Constructors and Initializers"

		public BaseEntityManager()
		{
		}

		public BaseEntityManager(Object backingSource, string backingSourceMethodName)
			: base(backingSource, backingSourceMethodName)
		{
		}

		#endregion

		#region "Methods"

		public override void LoadDynamic()
		{
			if (IsResourceLocked)
				return;

			IsResourceLocked = true;

			HashSet<Object> rawEntities = GetBackingDataHashSet();
			Dictionary<long, BaseObject> data = GetInternalData();
			Dictionary<Object, BaseObject> backingData = GetBackingInternalData();

			//Update the main data mapping
			data.Clear();
			foreach (Object entity in rawEntities)
			{
				try
				{
					MyObjectBuilder_EntityBase baseEntity = (MyObjectBuilder_EntityBase)BaseEntity.InvokeEntityMethod(entity, BaseEntity.BaseEntityGetObjectBuilderMethod, new object[] { Type.Missing });

					if (baseEntity == null)
						continue;

					if (data.ContainsKey(baseEntity.EntityId))
						continue;

					BaseEntity matchingEntity = null;

					//If the original data already contains an entry for this, skip creation
					if (backingData.ContainsKey(entity))
					{
						matchingEntity = (BaseEntity)backingData[entity];

						//Update the base entity (not the same as BackingObject which is the internal object)
						matchingEntity.BaseEntity = baseEntity;
					}
					else
					{
						matchingEntity = new BaseEntity(baseEntity, entity);
					}

					if (matchingEntity == null)
						throw new Exception("Failed to match/create entity");

					data.Add(matchingEntity.EntityId, matchingEntity);
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

			IsResourceLocked = false;
		}

		#endregion
	}
}
