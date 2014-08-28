using System;
using System.ComponentModel;
using System.Runtime.Serialization;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
	[DataContract(Name = "FloatingObjectProxy")]
	public class FloatingObject : BaseEntity
	{
		#region "Attributes"

		private static Type m_internalType;
		private InventoryItemEntity m_item;

		public static string FloatingObjectNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string FloatingObjectClass = "60663B6C2E735862064C925471BD4138";

		#endregion

		#region "Constructors and Initializers"

		public FloatingObject(MyObjectBuilder_FloatingObject definition)
			: base(definition)
		{
			m_item = new InventoryItemEntity(definition.Item);
		}

		public FloatingObject(MyObjectBuilder_FloatingObject definition, Object backingObject)
			: base(definition, backingObject)
		{
			m_item = new InventoryItemEntity(definition.Item);
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Floating Object")]
		[Browsable(false)]
		[ReadOnly(true)]
		new internal static Type InternalType
		{
			get
			{
				if (m_internalType == null)
					m_internalType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(FloatingObjectNamespace, FloatingObjectClass);
				return m_internalType;
			}
		}

		[DataMember]
		[Category("Floating Object")]
		[Browsable(true)]
		public override string Name
		{
			get { return ObjectBuilder.Item.PhysicalContent.SubtypeName; }
		}

		[DataMember]
		[Category("Floating Object")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_FloatingObject ObjectBuilder
		{
			get
			{
				return (MyObjectBuilder_FloatingObject)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category("Floating Object")]
		[Browsable(false)]
		public InventoryItemEntity Item
		{
			get { return m_item; }
			set
			{
				if (m_item == value) return;
				m_item = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateItem;
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
				Type type = InternalType;
				if (type == null)
					throw new Exception("Could not find internal type for FloatingObject");
				bool result = true;

				return result;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine(ex);
				return false;
			}
		}

		public override void Dispose()
		{
			FloatingObjectManager.Instance.RemoveFloatingObject(this);

			m_isDisposed = true;
		}

		protected void InternalUpdateItem()
		{
			try
			{
				//TODO - Add methods to set item of the floating object
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		#endregion
	}

	public class FloatingObjectManager
	{
		#region "Attributes"

		private static FloatingObjectManager m_instance;
		private static Type m_internalType;

		private FloatingObject m_floatingObjectToChange;

		public static string FloatingObjectManagerNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string FloatingObjectManagerClass = "66E5A072764E86AD0AC8B63304F0DC31";

		public static string FloatingObjectManagerRemoveFloatingObjectMethod = "CDD52493D4DD9E7D7BDB9AFC5512A9E1";

		#endregion

		#region "Constructors and Initializers"

		protected FloatingObjectManager()
		{
			m_instance = this;
		}

		#endregion

		#region "Properties"

		internal static Type InternalType
		{
			get
			{
				if (m_internalType == null)
					m_internalType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(FloatingObjectManagerNamespace, FloatingObjectManagerClass);
				return m_internalType;
			}
		}

		public static FloatingObjectManager Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new FloatingObjectManager();

				return m_instance;
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
					throw new Exception("Could not find internal type for FloatingObjectManager");
				bool result = true;
				result &= BaseObject.HasMethod(type, FloatingObjectManagerRemoveFloatingObjectMethod);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine(ex);
				return false;
			}
		}

		public void RemoveFloatingObject(FloatingObject floatingObject)
		{
			m_floatingObjectToChange = floatingObject;

			Action action = InternalRemoveFloatingObject;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		protected void InternalRemoveFloatingObject()
		{
			if (m_floatingObjectToChange == null)
				return;

			Object backingObject = m_floatingObjectToChange.BackingObject;
			BaseObject.InvokeStaticMethod(InternalType, FloatingObjectManagerRemoveFloatingObjectMethod, new object[] { backingObject });

			m_floatingObjectToChange = null;
		}

		#endregion
	}
}
