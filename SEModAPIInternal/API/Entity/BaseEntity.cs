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

		private static Type m_internalType;

		private float m_maxLinearVelocity;
		private long m_entityId;
		private MyPositionAndOrientation m_positionOrientation;
		private Vector3 m_linearVelocity;
		private Vector3 m_angularVelocity;
		private BaseEntityNetworkManager m_networkManager;

		//Definition
		public static string BaseEntityNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string BaseEntityClass = "F6DF01EE4159339113BB9650DEEE1913";

		//Methods
		public static string BaseEntityGetObjectBuilderMethod = "GetObjectBuilder";
		public static string BaseEntityGetPhysicsManagerMethod = "691FA4830C80511C934826203A251981";
		public static string BaseEntityGetEntityIdMethod = "53C3FFA07960404AABBEAAF931E5487E";
		public static string BaseEntityCombineOnMovedEventMethod = "04F6493DF187FBA38C2B379BA9484304";
		public static string BaseEntityCombineOnClosedEventMethod = "C1704F26C9D5D7EBE19DC78AB8923F4E";
		public static string BaseEntityGetIsDisposedMethod = "6D8F627C1C0F9F166031C3B600FEDA60";
		public static string BaseEntityGetOrientationMatrixMethod = "FD50436D896ACC794550210055349FE0";
		public static string BaseEntityRemoveMethod = "EF75CF24B6BD63FC1A42E023BAF658B2";
		public static string BaseEntityGetNetManagerMethod = "F4456F82186EC3AE6C73294FA6C0A11D";

		//Fields
		public static string BaseEntityEntityIdField = "F7E51DBA5F2FD0CCF8BBE66E3573BEAC";

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
					m_positionOrientation = baseEntity.PositionAndOrientation.GetValueOrDefault();
				else
					m_positionOrientation = new MyPositionAndOrientation();
			}
			else
			{
				m_entityId = 0;
				m_positionOrientation = new MyPositionAndOrientation();
			}

			m_networkManager = new BaseEntityNetworkManager(this, GetEntityNetworkManager(BackingObject));

			m_linearVelocity = new Vector3(0, 0, 0);
			m_angularVelocity = new Vector3(0, 0, 0);
			m_maxLinearVelocity = (float)104.7;

			Action action = InternalRegisterEntityMovedEvent;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		#endregion

		#region "Properties"

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

		[Category("Entity")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_EntityBase ObjectBuilder
		{
			get
			{
				MyObjectBuilder_EntityBase entityBase = (MyObjectBuilder_EntityBase)base.ObjectBuilder;
				entityBase.EntityId = m_entityId;
				entityBase.PositionAndOrientation = m_positionOrientation;

				return (MyObjectBuilder_EntityBase)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[Category("Entity")]
		[Browsable(true)]
		[Description("The unique entity ID representing a functional entity in-game")]
		public long EntityId
		{
			get { return m_entityId; }
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

		[Category("Entity")]
		[Browsable(false)]
		[ReadOnly(true)]
		public MyPersistentEntityFlags2 PersistentFlags
		{
			get { return ObjectBuilder.PersistentFlags; }
		}

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

		[Category("Entity")]
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
		}

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
				//Only move and remove if the backing object isn't already disposed
				bool isDisposed = (bool)BaseEntity.InvokeEntityMethod(BackingObject, BaseEntity.BaseEntityGetIsDisposedMethod);
				if (!isDisposed)
				{
					m_networkManager.RemoveEntity();

					Thread.Sleep(250);

					Action action = InternalRemoveEntity;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
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

		internal static Object GetEntityNetworkManager(Object entity)
		{
			try
			{
				Object result = InvokeEntityMethod(entity, BaseEntityGetNetManagerMethod);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}

		}

		internal static long GetEntityId(Object entity)
		{
			try
			{
				//TODO - Change this to a method to get the entity id instead of just getting the field
				FieldInfo entityIdField = GetEntityField(entity, BaseEntityEntityIdField);
				long entityId = (long)entityIdField.GetValue(entity);
				return entityId;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return 0;
			}
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
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected void InternalUpdateEntityId()
		{
			try
			{
				//TODO - Change this to a method to set the entity id instead of just setting the field
				FieldInfo entityIdField = GetEntityField(BackingObject, BaseEntityEntityIdField);
				entityIdField.SetValue(BackingObject, EntityId);
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
				HkRigidBody havokBody = PhysicsBody;
				if (havokBody == null)
					return;

				Vector3 newPosition = m_positionOrientation.Position;

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
				HkRigidBody havokBody = PhysicsBody;
				if (havokBody == null)
					return;

				Matrix orientationMatrix = m_positionOrientation.GetMatrix().GetOrientation();
				Quaternion orientation = Quaternion.CreateFromRotationMatrix(orientationMatrix);
				havokBody.Rotation = orientation;
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
				HkRigidBody havokBody = PhysicsBody;
				if (havokBody == null)
					return;

				if (SandboxGameAssemblyWrapper.IsDebugging)
				{
					LogManager.APILog.WriteLineAndConsole("Entity - Changing linear velocity of '" + Name + "' from '" + havokBody.LinearVelocity.ToString() + "' to '" + m_linearVelocity.ToString() + "'");
				}

				havokBody.LinearVelocity = m_linearVelocity;
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
				HkRigidBody havokBody = PhysicsBody;
				if (havokBody == null)
					return;

				if (SandboxGameAssemblyWrapper.IsDebugging)
				{
					LogManager.APILog.WriteLineAndConsole("Entity - Changing angular velocity of '" + Name + "' from '" + havokBody.AngularVelocity.ToString() + "' to '" + m_angularVelocity.ToString() + "'");
				}

				havokBody.AngularVelocity = m_angularVelocity;
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
					LogManager.APILog.WriteLineAndConsole(this.GetType().Name + " '" + EntityId.ToString() + "': Calling 'Close' to remove entity");

				BaseObject.InvokeEntityMethod(BackingObject, "Close");
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

				InvokeEntityMethod(BackingObject, BaseEntityCombineOnMovedEventMethod, new object[] { action });
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

	public class BaseEntityNetworkManager
	{
		#region "Attributes"

		private BaseEntity m_parent;
		private Object m_networkManager;

		public static string BaseEntityNetworkManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string BaseEntityNetworkManagerClass = "48D79F8E3C8922F14D85F6D98237314C";

		public static string BaseEntityBroadcastRemovalMethod = "B24AB8B6F391B676C87C39C041BCDE1B";

		#endregion

		#region "Constructors and Initializers"

		public BaseEntityNetworkManager(BaseEntity parent, Object netManager)
		{
			m_parent = parent;
			m_networkManager = netManager;
		}

		#endregion

		#region "Properties"

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

		public void RemoveEntity()
		{
			Action action = InternalRemoveEntity;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		protected void InternalRemoveEntity()
		{
			BaseObject.InvokeEntityMethod(NetworkManager, BaseEntityBroadcastRemovalMethod);
		}

		#endregion
	}
}
