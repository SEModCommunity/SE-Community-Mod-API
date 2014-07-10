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

		public static string BaseEntityNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string BaseEntityClass = "F6DF01EE4159339113BB9650DEEE1913";
		public static string BaseEntityGetObjectBuilderMethod = "GetObjectBuilder";
		public static string BaseEntityGetPhysicsManagerMethod = "691FA4830C80511C934826203A251981";
		public static string BaseEntityGetEntityIdMethod = "53C3FFA07960404AABBEAAF931E5487E";
		public static string BaseEntityCombineOnMovedEventMethod = "04F6493DF187FBA38C2B379BA9484304";
		public static string BaseEntityCombineOnClosedEventMethod = "C1704F26C9D5D7EBE19DC78AB8923F4E";
		public static string BaseEntityEntityIdField = "F7E51DBA5F2FD0CCF8BBE66E3573BEAC";

		public static string PhysicsManagerGetRigidBodyMethod = "634E5EC534E45874230868BD089055B1";

		#endregion

		#region "Constructors and Initializers"

		public BaseEntity(MyObjectBuilder_EntityBase baseEntity)
			: base(baseEntity)
		{
		}

		public BaseEntity(MyObjectBuilder_EntityBase baseEntity, Object backingObject)
			: base(baseEntity, backingObject)
		{
			try
			{
				Action action = InternalRegisterEntityMovedEvent;
				SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);

				m_maxLinearVelocity = (float)104.7;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#endregion

		#region "Properties"

		/// <summary>
		/// Gets the formatted name of an entity
		/// </summary>
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
		public MyPositionAndOrientation PositionAndOrientation
		{
			get { return GetSubTypeEntity().PositionAndOrientation.GetValueOrDefault(); }
			set
			{
				if (GetSubTypeEntity().PositionAndOrientation.Equals(value)) return;
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
		public SerializableVector3 Position
		{
			get { return GetSubTypeEntity().PositionAndOrientation.GetValueOrDefault().Position; }
			set
			{
				if (Position.Equals(value)) return;
				MyPositionAndOrientation? positionOrientation = new MyPositionAndOrientation(value, Forward, Up);
				GetSubTypeEntity().PositionAndOrientation = positionOrientation;
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
		public SerializableVector3 Up
		{
			get { return GetSubTypeEntity().PositionAndOrientation.GetValueOrDefault().Up; }
			set
			{
				if (Up.Equals(value)) return;
				MyPositionAndOrientation? positionOrientation = new MyPositionAndOrientation(Position, Forward, value);
				GetSubTypeEntity().PositionAndOrientation = positionOrientation;
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
		public SerializableVector3 Forward
		{
			get { return GetSubTypeEntity().PositionAndOrientation.GetValueOrDefault().Forward; }
			set
			{
				if (Forward.Equals(value)) return;
				MyPositionAndOrientation? positionOrientation = new MyPositionAndOrientation(Position, value, Up);
				GetSubTypeEntity().PositionAndOrientation = positionOrientation;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateOrientation;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Entity")]
		[Browsable(false)]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public virtual SerializableVector3 LinearVelocity
		{ get; set; }

		[Category("Entity")]
		[Browsable(false)]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public virtual SerializableVector3 AngularVelocity
		{ get; set; }

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
					if (body == null)
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
				Vector3 currentPosition = Position;
				currentPosition = Vector3.Add(currentPosition, new Vector3(100000, 100000, 100000));
				Position = currentPosition;

				Action action = InternalUpdatePosition;
				SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);

				Thread.Sleep(250);

				Action action2 = InternalRemoveEntity;
				SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action2);

				EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent();
				newEvent.type = EntityEventManager.EntityEventType.OnBaseEntityDeleted;
				newEvent.timestamp = DateTime.Now;
				newEvent.entity = this;
				newEvent.priority = 1;
				EntityEventManager.Instance.AddEvent(newEvent);

				m_isDisposed = true;
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

		new public void Export(FileInfo fileInfo)
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

		protected HkRigidBody GetRigidBody()
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
				havokBody.Position = Position;
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
				havokBody.LinearVelocity = LinearVelocity;
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
				havokBody.AngularVelocity = AngularVelocity;
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

		protected void InternalRegisterEntityClosedEvent()
		{
			try
			{
				Action<Object> action = InternalEntityClosedEvent;

				MethodInfo method = BackingObject.GetType().GetMethod(BaseEntityCombineOnClosedEventMethod);
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

		protected void InternalEntityClosedEvent(Object entity)
		{
			try
			{
				if (IsDisposed)
					return;

				//This is disabled until we find a way to get the delegates in class heirarchy order instead of sequential order
				//Dispose();
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
			HashSet<Object> rawEntities = GetBackingDataHashSet();
			Dictionary<long, BaseObject> data = GetInternalData();
			Dictionary<Object, BaseObject> backingData = GetBackingInternalData();

			//Update the main data mapping
			data.Clear();
			foreach (Object entity in rawEntities)
			{
				try
				{
					MyObjectBuilder_EntityBase baseEntity = (MyObjectBuilder_EntityBase)BaseEntity.InvokeEntityMethod(entity, BaseEntity.BaseEntityGetObjectBuilderMethod, new object[] { false });

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
						switch (baseEntity.TypeId)
						{
							case MyObjectBuilderTypeEnum.CubeGrid:
								matchingEntity = new CubeGridEntity((MyObjectBuilder_CubeGrid)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.Character:
								matchingEntity = new CharacterEntity((MyObjectBuilder_Character)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.FloatingObject:
								matchingEntity = new FloatingObject((MyObjectBuilder_FloatingObject)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.Meteor:
								matchingEntity = new Meteor((MyObjectBuilder_Meteor)baseEntity, entity);
								break;
							case MyObjectBuilderTypeEnum.VoxelMap:
								matchingEntity = new VoxelMap((MyObjectBuilder_VoxelMap)baseEntity, entity);
								break;
							default:
								matchingEntity = new BaseEntity(baseEntity, entity);
								break;
						}
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
		}

		#endregion
	}
}
