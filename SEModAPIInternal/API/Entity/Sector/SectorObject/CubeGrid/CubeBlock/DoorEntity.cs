using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;
using Sandbox.Definitions;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract(Name = "DoorEntityProxy")]
	public class DoorEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private bool m_state;

		public static string DoorNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string DoorClass = "F0D92F8F3A91716EC613ADD46F36158D";

		public static string DoorGetStateMethod = "EED93169FB8C3235596CF33BD3AA33B8";
		public static string DoorSetStateMethod = "2A0572A89EB6003FDC46A6D8420ECF79";
		public static string DoorBroadcastStateMethod = "89F6DE95D0A6749BEC3F5A2D5C1F451C";

		#endregion

		#region "Constructors and Initializers"

		public DoorEntity(CubeGridEntity parent, MyObjectBuilder_Door definition)
			: base(parent, definition)
		{
			m_state = definition.State;
		}

		public DoorEntity(CubeGridEntity parent, MyObjectBuilder_Door definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
			m_state = definition.State;
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Door")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_Door ObjectBuilder
		{
			get
			{
				MyObjectBuilder_Door door = (MyObjectBuilder_Door)base.ObjectBuilder;

				return door;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[IgnoreDataMember]
		[Category("Door")]
		public float Opening
		{
			get { return ObjectBuilder.Opening; }
		}

		[DataMember]
		[Category("Door")]
		public bool State
		{
			get
			{
				if(BackingObject == null || ActualObject == null)
					return ObjectBuilder.State;

				return GetDoorState();
			}
			set
			{
				if (m_state == value) return;
				m_state = value;
				ObjectBuilder.State = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateDoor;
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

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(DoorNamespace, DoorClass);
				if (type == null)
					throw new Exception("Could not find internal type for DoorEntity");
				result &= HasMethod(type, DoorGetStateMethod);
				result &= HasMethod(type, DoorSetStateMethod);
				result &= HasMethod(type, DoorBroadcastStateMethod);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		#region "Internal"

		protected bool GetDoorState()
		{
			try
			{
				bool result = (bool)InvokeEntityMethod(ActualObject, DoorGetStateMethod);
				return result;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return m_state;
			}
		}

		protected void InternalUpdateDoor()
		{
			try
			{
				InvokeEntityMethod(ActualObject, DoorBroadcastStateMethod, new object[] { m_state });
				InvokeEntityMethod(ActualObject, DoorSetStateMethod, new object[] { m_state });
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		#endregion

		#endregion
	}
}
