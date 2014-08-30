using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract(Name = "PistonEntityProxy")]
	public class PistonEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private PistonNetworkManager m_networkManager;
		private float m_velocity;
		private float m_minLimit;
		private float m_maxLimit;

		public static string PistonNamespace = "AAD9061F948E6A3635200145188D64A9";
		public static string PistonClass = "BB5F18CC2986DBE98E7199353DD42570";

		public static string PistonGetVelocityMethod = "4F68A20C100B8224FB6631B65C0D7F8C";
		public static string PistonSetVelocityMethod = "098E60E484F608B7F8D1C596244A4AB7";
		public static string PistonGetMinLimitMethod = "0E29ADCC5075989BF4A72E552F871425";
		public static string PistonSetMinLimitMethod = "ADF5E62C4A5337CC85009531D442E921";
		public static string PistonGetMaxLimitMethod = "0DDBA2A8D27BC58CBF8A46CCE34ACE82";
		public static string PistonSetMaxLimitMethod = "2E4611FD9E17A856C250DF3C2AEC5C94";
		public static string PistonGetNetworkManagerMethod = "E8902D9F85DF1866E02EA8A0324EFF2C";

		public static string PistonTopBlockEntityIdField = "5565108EDC64BEE4C29B3B4880DB86AC";
		public static string PistonCurrentPositionField = "26E8B71F377542D63B01CBAA525A237A";

		#endregion

		#region "Constructors and Intializers"

		public PistonEntity(CubeGridEntity parent, MyObjectBuilder_PistonBase definition)
			: base(parent, definition)
		{
			m_velocity = definition.Velocity;
			m_minLimit = definition.MinLimit.GetValueOrDefault(0);
			m_maxLimit = definition.MaxLimit.GetValueOrDefault(0);
		}

		public PistonEntity(CubeGridEntity parent, MyObjectBuilder_PistonBase definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
			m_velocity = definition.Velocity;
			m_minLimit = definition.MinLimit.GetValueOrDefault(0);
			m_maxLimit = definition.MaxLimit.GetValueOrDefault(0);

			m_networkManager = new PistonNetworkManager(this, GetPistonNetworkManager());
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Piston")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(PistonNamespace, PistonClass);
				return type;
			}
		}

		[IgnoreDataMember]
		[Category("Piston")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_PistonBase ObjectBuilder
		{
			get { return (MyObjectBuilder_PistonBase)base.ObjectBuilder; }
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[IgnoreDataMember]
		[Category("Piston")]
		[Browsable(false)]
		[ReadOnly(true)]
		public CubeBlockEntity TopBlock
		{
			get
			{
				if (BackingObject == null || ActualObject == null)
					return null;

				long topBlockEntityId = GetTopBlockEntityId();
				if(topBlockEntityId == 0)
					return null;
				BaseObject baseObject = GameEntityManager.GetEntity(topBlockEntityId);
				if (baseObject == null)
					return null;
				if(!(baseObject is CubeBlockEntity))
					return null;
				CubeBlockEntity block = (CubeBlockEntity)baseObject;
				return block;
			}
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category("Piston")]
		[ReadOnly(true)]
		public long TopBlockId
		{
			get
			{
				if (BackingObject == null || ActualObject == null)
					return ObjectBuilder.TopBlockId;

				return GetTopBlockEntityId();
			}
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category("Piston")]
		[ReadOnly(true)]
		public float CurrentPosition
		{
			get
			{
				if (BackingObject == null || ActualObject == null)
					return ObjectBuilder.CurrentPosition;

				return GetPistonCurrentPosition();
			}
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category("Piston")]
		public float Velocity
		{
			get
			{
				if (BackingObject == null || ActualObject == null)
					return ObjectBuilder.Velocity;

				return GetPistonVelocity();
			}
			set
			{
				if (Velocity == value) return;
				ObjectBuilder.Velocity = value;
				m_velocity = value;
				Changed = true;

				if (BackingObject != null && ActualObject != null)
				{
					Action action = SetPistonVelocity;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[DataMember]
		[Category("Piston")]
		public float MinLimit
		{
			get
			{
				if (BackingObject == null || ActualObject == null)
					return ObjectBuilder.MinLimit.GetValueOrDefault(0);

				return GetPistonMinLimit();
			}
			set
			{
				if (MinLimit == value) return;
				ObjectBuilder.MinLimit = value;
				m_minLimit = value;
				Changed = true;

				if (BackingObject != null && ActualObject != null)
				{
					Action action = SetPistonMinLimit;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[DataMember]
		[Category("Piston")]
		public float MaxLimit
		{
			get
			{
				if (BackingObject == null || ActualObject == null)
					return ObjectBuilder.MaxLimit.GetValueOrDefault(0);

				return GetPistonMaxLimit();
			}
			set
			{
				if (MaxLimit == value) return;
				ObjectBuilder.MaxLimit = value;
				m_maxLimit = value;
				Changed = true;

				if (BackingObject != null && ActualObject != null)
				{
					Action action = SetPistonMaxLimit;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		#endregion

		#region "Methods"

		new public static bool ReflectionUnitTest()
		{
			try
			{
				bool result = true;

				Type type = InternalType;
				if (type == null)
					throw new Exception("Could not find internal type for PistonEntity");

				result &= HasMethod(type, PistonGetVelocityMethod);
				result &= HasMethod(type, PistonSetVelocityMethod);
				result &= HasMethod(type, PistonGetMinLimitMethod);
				result &= HasMethod(type, PistonSetMinLimitMethod);
				result &= HasMethod(type, PistonGetMaxLimitMethod);
				result &= HasMethod(type, PistonSetMaxLimitMethod);
				result &= HasMethod(type, PistonGetNetworkManagerMethod);

				result &= HasField(type, PistonTopBlockEntityIdField);
				result &= HasField(type, PistonCurrentPositionField);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		protected Object GetPistonNetworkManager()
		{
			Object result = InvokeEntityMethod(ActualObject, PistonGetNetworkManagerMethod);
			return result;
		}

		protected long GetTopBlockEntityId()
		{
			Object rawResult = GetEntityFieldValue(ActualObject, PistonTopBlockEntityIdField);
			if (rawResult == null)
				return 0;
			long result = (long)rawResult;
			return result;
		}

		protected float GetPistonCurrentPosition()
		{
			Object rawResult = GetEntityFieldValue(ActualObject, PistonCurrentPositionField);
			if (rawResult == null)
				return 0;
			float result = (float)rawResult;
			return result;
		}

		protected float GetPistonVelocity()
		{
			Object rawResult = InvokeEntityMethod(ActualObject, PistonGetVelocityMethod);
			if (rawResult == null)
				return 0;
			float result = (float)rawResult;
			return result;
		}

		protected void SetPistonVelocity()
		{
			InvokeEntityMethod(ActualObject, PistonSetVelocityMethod, new object[] { m_velocity });
			m_networkManager.BroadcastVelocity(m_velocity);
		}

		protected float GetPistonMinLimit()
		{
			Object rawResult = InvokeEntityMethod(ActualObject, PistonGetMinLimitMethod);
			if (rawResult == null)
				return 0;
			float result = (float)rawResult;
			return result;
		}

		protected void SetPistonMinLimit()
		{
			InvokeEntityMethod(ActualObject, PistonSetMinLimitMethod, new object[] { m_minLimit });
			m_networkManager.BroadcastVelocity(m_minLimit);
		}

		protected float GetPistonMaxLimit()
		{
			Object rawResult = InvokeEntityMethod(ActualObject, PistonGetMaxLimitMethod);
			if (rawResult == null)
				return 0;
			float result = (float)rawResult;
			return result;
		}

		protected void SetPistonMaxLimit()
		{
			InvokeEntityMethod(ActualObject, PistonSetMaxLimitMethod, new object[] { m_maxLimit });
			m_networkManager.BroadcastVelocity(m_maxLimit);
		}

		#endregion
	}

	public class PistonNetworkManager
	{
		#region "Attributes"

		private PistonEntity m_parent;
		private Object m_backingObject;

		public static string PistonNetworkManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string PistonNetworkManagerClass = "1067575769BEE523EABB86ACC13B9061";

		public static string PistonNetworkManagerBroadcastVelocity = "349500C6F86B02688A5C5B0A2430B083";
		public static string PistonNetworkManagerBroadcastMinLimit = "4BDAFF5C0AD29EC5225CCE535B8C9985";
		public static string PistonNetworkManagerBroadcastMaxLimit = "B714B58ED3739160E709EB0AA43AC9EB";

		//Packets
		//324 - Top block id
		//325 - Velocity
		//326 - Min limit
		//327 - Max limit
		//328 - Current position

		#endregion

		#region "Constructors and Initializers"

		public PistonNetworkManager(PistonEntity parent, Object backingObject)
		{
			m_parent = parent;
			m_backingObject = backingObject;
		}

		#endregion

		#region "Properties"

		internal static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(PistonNetworkManagerNamespace, PistonNetworkManagerClass);
				return type;
			}
		}

		internal Object BackingObject
		{
			get { return m_backingObject; }
		}

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				bool result = true;

				Type type = InternalType;
				if (type == null)
					throw new Exception("Could not find internal type for PistonNetworkManager");

				result &= BaseObject.HasMethod(type, PistonNetworkManagerBroadcastVelocity);
				result &= BaseObject.HasMethod(type, PistonNetworkManagerBroadcastMinLimit);
				result &= BaseObject.HasMethod(type, PistonNetworkManagerBroadcastMaxLimit);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		internal void BroadcastVelocity(float velocity)
		{
			BaseObject.InvokeEntityMethod(BackingObject, PistonNetworkManagerBroadcastVelocity, new object[] { velocity });
		}

		internal void BroadcastMinLimit(float minLimit)
		{
			BaseObject.InvokeEntityMethod(BackingObject, PistonNetworkManagerBroadcastMinLimit, new object[] { minLimit });
		}

		internal void BroadcastMaxLimit(float maxLimit)
		{
			BaseObject.InvokeEntityMethod(BackingObject, PistonNetworkManagerBroadcastMaxLimit, new object[] { maxLimit });
		}

		#endregion
	}
}
