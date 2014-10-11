using Havok;

using Microsoft.Xml.Serialization.GeneratedAssembly;

using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;

using Sandbox.Common.ObjectBuilders;

using SEModAPI.API;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Utility;
using SEModAPIInternal.Support;

using VRageMath;

namespace SEModAPIInternal.API.Entity
{
	[DataContract(Name = "BaseEntityProxy")]
	public class BaseEntity : BaseObject
	{
		#region "Attributes"

		private static Type m_internalType;

		private float m_maxLinearVelocity;
		private long m_entityId;
		private MyPositionAndOrientation m_positionOrientation;
		private Vector3 m_linearVelocity;
		private Vector3 m_angularVelocity;
		private BaseEntityNetworkManager m_networkManager;
		private string m_displayName;

		//Definition
		public static string BaseEntityNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string BaseEntityClass = "F6DF01EE4159339113BB9650DEEE1913";

		//Methods
		public static string BaseEntityGetObjectBuilderMethod = "GetObjectBuilder";
		public static string BaseEntityGetPhysicsManagerMethod = "691FA4830C80511C934826203A251981";
		public static string BaseEntityCombineOnMovedEventMethod = "04F6493DF187FBA38C2B379BA9484304";
		public static string BaseEntityCombineOnClosedEventMethod = "C1704F26C9D5D7EBE19DC78AB8923F4E";
		public static string BaseEntityGetIsDisposedMethod = "6D8F627C1C0F9F166031C3B600FEDA60";
		public static string BaseEntityGetOrientationMatrixMethod = "FD50436D896ACC794550210055349FE0";
		public static string BaseEntityGetNetManagerMethod = "F4456F82186EC3AE6C73294FA6C0A11D";
		public static string BaseEntitySetEntityIdMethod = "D3D6702587D6336FEE37725E8D2C52CD";
		public static string BaseEntityGetDisplayNameMethod = "DB913685BC5152DC19A4796E9E8CF659";
		public static string BaseEntitySetDisplayNameMethod = "DFF609C956C433D5F03DAA6AA8814223";

		public static string BaseEntityEntityIdField = "F7E51DBA5F2FD0CCF8BBE66E3573BEAC";

		//////////////////////////////////////////////////////////

		public static string PhysicsManagerNamespace = "D1B6432AAEEF40F9D99F69835A7B23F5";
		public static string PhysicsManagerClass = "5BAA908D4615EC702E28985E09DBEF8F";

		public static string PhysicsManagerGetRigidBodyMethod = "634E5EC534E45874230868BD089055B1";

		#endregion

		#region "Constructors and Initializers"

		public BaseEntity(MyObjectBuilder_EntityBase baseEntity)
			: base(baseEntity)
		{
			if (baseEntity != null)
			{
				m_entityId = baseEntity.EntityId;
				if (baseEntity.PositionAndOrientation != null)
				{
					m_positionOrientation = baseEntity.PositionAndOrientation.GetValueOrDefault();
				}
				else
				{
					m_positionOrientation = new MyPositionAndOrientation();
					m_positionOrientation.Position = UtilityFunctions.GenerateRandomBorderPosition(new Vector3(-500000, -500000, -500000), new Vector3(500000, 500000, 500000));
					m_positionOrientation.Forward = new Vector3(0, 0, 1);
					m_positionOrientation.Up = new Vector3(0, 1, 0);
				}
			}
			else
			{
				m_entityId = 0;
				m_positionOrientation = new MyPositionAndOrientation();
				m_positionOrientation.Position = UtilityFunctions.GenerateRandomBorderPosition(new Vector3(-500000, -500000, -500000), new Vector3(500000, 500000, 500000));
				m_positionOrientation.Forward = new Vector3(0, 0, 1);
				m_positionOrientation.Up = new Vector3(0, 1, 0);
			}

			m_linearVelocity = new Vector3(0, 0, 0);
			m_angularVelocity = new Vector3(0, 0, 0);
			m_maxLinearVelocity = (float)104.7;
		}

		public BaseEntity(MyObjectBuilder_EntityBase baseEntity, Object backingObject)
			: base(baseEntity, backingObject)
		{
			if (baseEntity != null)
			{
				m_entityId = baseEntity.EntityId;
				if (baseEntity.PositionAndOrientation != null)
				{
					m_positionOrientation = baseEntity.PositionAndOrientation.GetValueOrDefault();
				}
				else
				{
					m_positionOrientation = new MyPositionAndOrientation();
					m_positionOrientation.Position = UtilityFunctions.GenerateRandomBorderPosition(new Vector3(-500000, -500000, -500000), new Vector3(500000, 500000, 500000));
					m_positionOrientation.Forward = new Vector3(0, 0, 1);
					m_positionOrientation.Up = new Vector3(0, 1, 0);
				}
			}
			else
			{
				m_entityId = 0;
				m_positionOrientation = new MyPositionAndOrientation();
				m_positionOrientation.Position = UtilityFunctions.GenerateRandomBorderPosition(new Vector3(-500000, -500000, -500000), new Vector3(500000, 500000, 500000));
				m_positionOrientation.Forward = new Vector3(0, 0, 1);
				m_positionOrientation.Up = new Vector3(0, 1, 0);
			}

			m_networkManager = new BaseEntityNetworkManager(this, GetEntityNetworkManager(BackingObject));

			m_linearVelocity = new Vector3(0, 0, 0);
			m_angularVelocity = new Vector3(0, 0, 0);
			m_maxLinearVelocity = (float)104.7;

			if (EntityId != 0)
			{
				GameEntityManager.AddEntity(EntityId, this);
			}

			Action action = InternalRegisterEntityMovedEvent;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Entity")]
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

		[DataMember]
		[Category("Entity")]
		[Browsable(true)]
		[ReadOnly(true)]
		[Description("Formatted Name of an entity")]
		public override string Name
		{
			get
			{
				return ObjectBuilder.Name == "" ? ObjectBuilder.TypeId.ToString() : ObjectBuilder.EntityId.ToString();
			}
		}

		[IgnoreDataMember]
		[Category("Entity")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_EntityBase ObjectBuilder
		{
			get
			{
				MyObjectBuilder_EntityBase entityBase = (MyObjectBuilder_EntityBase)base.ObjectBuilder;
				if (entityBase == null)
					return (MyObjectBuilder_EntityBase)null;

				entityBase.EntityId = m_entityId;
				entityBase.PositionAndOrientation = m_positionOrientation;

				return (MyObjectBuilder_EntityBase)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category("Entity")]
		public virtual string DisplayName
		{
			get { return m_displayName; }
			set
			{
				if (m_displayName == value) return;
				m_displayName = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateDisplayName;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[DataMember]
		[Category("Entity")]
		[Browsable(true)]
		[Description("The unique entity ID representing a functional entity in-game")]
		public long EntityId
		{
			get
			{
				if (BackingObject == null)
					return m_entityId;

				long entityId = BaseEntity.GetEntityId(BackingObject);
				if (entityId == 0)
					return m_entityId;

				return entityId;
			}
			set
			{
				if (m_entityId == value) return;
				m_entityId = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateEntityId;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[IgnoreDataMember]
		[Category("Entity")]
		[Browsable(false)]
		[ReadOnly(true)]
		public MyPersistentEntityFlags2 PersistentFlags
		{
			get { return ObjectBuilder.PersistentFlags; }
			private set
			{
				//Do nothing!
			}
		}

		[IgnoreDataMember]
		[Category("Entity")]
		[Browsable(false)]
		public MyPositionAndOrientation PositionAndOrientation
		{
			get
			{
				if (BackingObject == null)
					return m_positionOrientation;

				HkRigidBody body = PhysicsBody;
				if (body == null || body.IsDisposed)
					return m_positionOrientation;

				Matrix orientationMatrix = Matrix.CreateFromQuaternion(body.Rotation);
				MyPositionAndOrientation positionOrientation = new MyPositionAndOrientation(orientationMatrix);
				positionOrientation.Position = body.Position;

				return positionOrientation;
			}
			set
			{
				m_positionOrientation = value;
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

		[DataMember]
		[Category("Entity")]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public Vector3Wrapper Position
		{
			get { return PositionAndOrientation.Position; }
			set
			{
				m_positionOrientation.Position = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdatePosition;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[IgnoreDataMember]
		[Category("Entity")]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public Vector3Wrapper Up
		{
			get { return PositionAndOrientation.Up; }
			set
			{
				m_positionOrientation.Up = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateOrientation;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[IgnoreDataMember]
		[Category("Entity")]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public Vector3Wrapper Forward
		{
			get { return PositionAndOrientation.Forward; }
			set
			{
				m_positionOrientation.Forward = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateOrientation;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[DataMember]
		[Category("Entity")]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public Vector3Wrapper LinearVelocity
		{
			get
			{
				if (BackingObject == null)
					return m_linearVelocity;

				HkRigidBody body = PhysicsBody;
				if (body == null || body.IsDisposed)
					return m_linearVelocity;

				if (body.LinearVelocity == Vector3.Zero)
					return m_linearVelocity;

				return body.LinearVelocity;
			}
			set
			{
				m_linearVelocity = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateLinearVelocity;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[DataMember]
		[Category("Entity")]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public Vector3Wrapper AngularVelocity
		{
			get
			{
				if (BackingObject == null)
					return m_angularVelocity;

				HkRigidBody body = PhysicsBody;
				if (body == null || body.IsDisposed)
					return m_angularVelocity;

				return body.AngularVelocity;
			}
			set
			{
				m_angularVelocity = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateAngularVelocity;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[IgnoreDataMember]
		[Category("Entity")]
		public float MaxLinearVelocity
		{
			get
			{
				if (BackingObject == null)
					return m_maxLinearVelocity;

				HkRigidBody body = PhysicsBody;
				if (body == null || body.IsDisposed)
					return m_maxLinearVelocity;

				return body.MaxLinearVelocity;
			}
			set
			{
				if (m_maxLinearVelocity == value) return;
				m_maxLinearVelocity = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateMaxLinearVelocity;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[DataMember]
		[Category("Entity")]
		[Browsable(true)]
		[ReadOnly(true)]
		public float Mass
		{
			get
			{
				if (BackingObject == null)
					return 0;

				HkRigidBody body = PhysicsBody;
				if (body == null || body.IsDisposed)
					return 0;

				return body.Mass;
			}
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category("Entity")]
		[Browsable(true)]
		[ReadOnly(true)]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public Vector3 CenterOfMass
		{
			get
			{
				return PhysicsBody.CenterOfMassWorld;
			}
			private set
			{
				//Do nothing!
			}
		}

		[IgnoreDataMember]
		[Category("Entity")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal HkRigidBody PhysicsBody
		{
			get
			{
				if (BackingObject == null)
					return null;

				return BaseEntity.GetRigidBody(BackingObject);
			}
		}

		[IgnoreDataMember]
		[Category("Entity")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal BaseEntityNetworkManager BaseNetworkManager
		{
			get { return m_networkManager; }
		}

		#endregion

		#region "Methods"

		public static long GenerateEntityId()
		{
			return UtilityFunctions.GenerateEntityId();
		}

		public override void Dispose()
		{
			if (IsDisposed)
				return;

			base.Dispose();

			if (BackingObject != null)
			{
				//Only remove if the backing object isn't already disposed
				bool isDisposed = (bool)BaseEntity.InvokeEntityMethod(BackingObject, BaseEntity.BaseEntityGetIsDisposedMethod);
				if (!isDisposed)
				{
					m_networkManager.RemoveEntity();

					Action action = InternalRemoveEntity;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}

			if (EntityId != 0)
			{
				GameEntityManager.RemoveEntity(EntityId);
			}

			EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent();
			newEvent.type = EntityEventManager.EntityEventType.OnBaseEntityDeleted;
			newEvent.timestamp = DateTime.Now;
			newEvent.entity = this;
			newEvent.priority = 1;
			EntityEventManager.Instance.AddEvent(newEvent);
		}

		public override void Export(FileInfo fileInfo)
		{
			BaseObjectManager.SaveContentFile<MyObjectBuilder_EntityBase, MyObjectBuilder_EntityBaseSerializer>(ObjectBuilder, fileInfo);
		}

		new public MyObjectBuilder_EntityBase Export()
		{
			return ObjectBuilder;
		}

		new public static bool ReflectionUnitTest()
		{
			try
			{
				Type type = InternalType;
				if (type == null)
					throw new Exception("Could not find internal type for BaseEntity");
				bool result = true;
				result &= HasMethod(type, BaseEntityGetObjectBuilderMethod);
				result &= HasMethod(type, BaseEntityGetPhysicsManagerMethod);
				result &= HasMethod(type, BaseEntityCombineOnMovedEventMethod);
				result &= HasMethod(type, BaseEntityCombineOnClosedEventMethod);
				result &= HasMethod(type, BaseEntityGetIsDisposedMethod);
				result &= HasMethod(type, BaseEntityGetOrientationMatrixMethod);
				result &= HasMethod(type, BaseEntityGetNetManagerMethod);
				result &= HasMethod(type, BaseEntitySetEntityIdMethod);
				result &= HasMethod(type, BaseEntityGetDisplayNameMethod);
				result &= HasMethod(type, BaseEntitySetDisplayNameMethod);
				result &= HasField(type, BaseEntityEntityIdField);

				Type type2 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(PhysicsManagerNamespace, PhysicsManagerClass);
				if (type2 == null)
					throw new Exception("Could not find physics manager type for BaseEntity");
				result &= HasMethod(type2, PhysicsManagerGetRigidBodyMethod);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		#region "Internal"

		private static Object GetEntityPhysicsObject(Object entity)
		{
			try
			{
				Object physicsObject = InvokeEntityMethod(entity, BaseEntityGetPhysicsManagerMethod);

				return physicsObject;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
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
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		internal static Object GetEntityNetworkManager(Object entity)
		{
			try
			{
				Object result = InvokeEntityMethod(entity, BaseEntityGetNetManagerMethod);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}

		}

		internal static long GetEntityId(Object entity)
		{
			try
			{
				long entityId = 0L;
				try
				{
					entityId = (long)GetEntityFieldValue(entity, BaseEntityEntityIdField);
				}
				catch (Exception ex)
				{
					LogManager.ErrorLog.WriteLine(ex);
				}
				return entityId;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return 0;
			}
		}

		internal static MyObjectBuilder_EntityBase GetObjectBuilder(Object entity)
		{
			MyObjectBuilder_EntityBase objectBuilder = (MyObjectBuilder_EntityBase)BaseEntity.InvokeEntityMethod(entity, BaseEntity.BaseEntityGetObjectBuilderMethod, new object[] { Type.Missing });
			return objectBuilder;
		}

		protected void InternalUpdateMaxLinearVelocity()
		{
			try
			{
				HkRigidBody havokBody = PhysicsBody;
				if (havokBody == null)
					return;
				havokBody.MaxLinearVelocity = m_maxLinearVelocity;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected void InternalUpdateEntityId()
		{
			InvokeEntityMethod(BackingObject, BaseEntitySetEntityIdMethod, new object[] { EntityId });
		}

		protected void InternalUpdatePosition()
		{
			try
			{
				HkRigidBody havokBody = PhysicsBody;
				if (havokBody == null)
					return;

				Vector3 newPosition = m_positionOrientation.Position;

				if (SandboxGameAssemblyWrapper.IsDebugging)
				{
					LogManager.APILog.WriteLine(this.GetType().Name + " - Changing position of '" + Name + "' from '" + havokBody.Position.ToString() + "' to '" + newPosition.ToString() + "'");
				}

				havokBody.Position = newPosition;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected void InternalUpdateOrientation()
		{
			try
			{
				HkRigidBody havokBody = PhysicsBody;
				if (havokBody == null)
					return;

				Matrix orientationMatrix = m_positionOrientation.GetMatrix().GetOrientation();
				Quaternion orientation = Quaternion.CreateFromRotationMatrix(orientationMatrix);
				havokBody.Rotation = orientation;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected void InternalUpdateLinearVelocity()
		{
			try
			{
				HkRigidBody havokBody = PhysicsBody;
				if (havokBody == null)
					return;

				if (SandboxGameAssemblyWrapper.IsDebugging)
				{
					LogManager.APILog.WriteLine(this.GetType().Name + " - Changing linear velocity of '" + Name + "' from '" + havokBody.LinearVelocity.ToString() + "' to '" + m_linearVelocity.ToString() + "'");
				}

				havokBody.LinearVelocity = m_linearVelocity;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected void InternalUpdateAngularVelocity()
		{
			try
			{
				HkRigidBody havokBody = PhysicsBody;
				if (havokBody == null)
					return;

				if (SandboxGameAssemblyWrapper.IsDebugging)
				{
					LogManager.APILog.WriteLine(this.GetType().Name + " - Changing angular velocity of '" + Name + "' from '" + havokBody.AngularVelocity.ToString() + "' to '" + m_angularVelocity.ToString() + "'");
				}

				havokBody.AngularVelocity = m_angularVelocity;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected void InternalRemoveEntity()
		{
			try
			{
				if (SandboxGameAssemblyWrapper.IsDebugging)
					LogManager.APILog.WriteLine(this.GetType().Name + " '" + Name + "': Calling 'Close' to remove entity");

				BaseObject.InvokeEntityMethod(BackingObject, "Close");
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLineAndConsole("Failed to remove entity '" + Name + "'");
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected void InternalRegisterEntityMovedEvent()
		{
			try
			{
				Action<Object> action = InternalEntityMovedEvent;

				InvokeEntityMethod(BackingObject, BaseEntityCombineOnMovedEventMethod, new object[] { action });
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
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
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected void InternalUpdateDisplayName()
		{
			InvokeEntityMethod(BackingObject, BaseEntitySetDisplayNameMethod, new object[] { m_displayName });
		}

		#endregion

		#endregion
	}

	public class BaseEntityNetworkManager
	{
		#region "Attributes"

		private BaseEntity m_parent;
		private Object m_networkManager;

		public static string BaseEntityNetworkManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string BaseEntityNetworkManagerClass = "48D79F8E3C8922F14D85F6D98237314C";

		public static string BaseEntityBroadcastRemovalMethod = "5EBE421019EACEA0F25718E2585CF3D2";

		//Packets
		//10 - ??
		//11 - ??
		//12 - Remove entity
		//5741 - ??

		#endregion

		#region "Constructors and Initializers"

		public BaseEntityNetworkManager(BaseEntity parent, Object netManager)
		{
			m_parent = parent;
			m_networkManager = netManager;
		}

		#endregion

		#region "Properties"

		public static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(BaseEntityNetworkManagerNamespace, BaseEntityNetworkManagerClass);

				return type;
			}
		}

		public Object NetworkManager
		{
			get
			{
				if (m_networkManager == null)
				{
					m_networkManager = BaseEntity.GetEntityNetworkManager(m_parent.BackingObject);
				}

				return m_networkManager;
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
					throw new Exception("Could not find internal type for BaseEntityNetworkManager");
				bool result = BaseObject.HasMethod(type, BaseEntityBroadcastRemovalMethod);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine(ex);
				return false;
			}
		}

		public void RemoveEntity()
		{
			if (NetworkManager == null)
				return;

			Action action = InternalRemoveEntity;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		protected void InternalRemoveEntity()
		{
			try
			{
				if (NetworkManager == null)
					return;

				BaseObject.InvokeEntityMethod(NetworkManager, BaseEntityBroadcastRemovalMethod);
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
