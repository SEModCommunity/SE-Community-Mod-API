using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Sandbox.Common.ObjectBuilders;
using System.ComponentModel;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;
using VRageMath;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract(Name = "SmallGatlingGunEntityProxy")]
	public class SmallGatlingGunEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private InventoryEntity m_inventory;

		public static string SmallGatlingGunNamespace = "Sandbox.Game.Weapons";
		public static string SmallGatlingGunClass = "MySmallGatlingGun";

		public static string SmallGatlingGunGetInventoryMethod = "GetInventory";
		public static string SmallGatlingGunShootMethod = "Shoot";

		#endregion

		#region "Constructors and Intializers"

		public SmallGatlingGunEntity(CubeGridEntity parent, MyObjectBuilder_SmallGatlingGun definition)
			: base(parent, definition)
		{
			m_inventory = new InventoryEntity(definition.Inventory);
		}

		public SmallGatlingGunEntity(CubeGridEntity parent, MyObjectBuilder_SmallGatlingGun definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
			m_inventory = new InventoryEntity(definition.Inventory, GetInventory());
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Small Gatling Gun")]
		[Browsable(false)]
		[ReadOnly(true)]
		new internal static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(SmallGatlingGunNamespace, SmallGatlingGunClass);
				return type;
			}
		}

		[IgnoreDataMember]
		[Category("Small Gatling Gun")]
		[Browsable(false)]
		new internal MyObjectBuilder_SmallGatlingGun ObjectBuilder
		{
			get { return (MyObjectBuilder_SmallGatlingGun)base.ObjectBuilder; }
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[IgnoreDataMember]
		[Category("Small Gatling Gun")]
		[Browsable(false)]
		[ReadOnly(true)]
		public InventoryEntity Inventory
		{
			get
			{
				return m_inventory;
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
					throw new Exception("Could not find internal type for SmallGatlingGunEntity");

				result &= HasMethod(type, SmallGatlingGunGetInventoryMethod);
				result &= HasMethod(type, SmallGatlingGunShootMethod);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		public void Shoot()
		{
			Action action = InternalShoot;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		protected void InternalShoot()
		{
			try
			{
				//TODO - Fix this to get the proper direction
				Vector3 direction = Parent.LinearVelocity;
				direction.Normalize();

				InvokeEntityMethod(ActualObject, SmallGatlingGunShootMethod, new object[] { null, direction });
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected Object GetInventory()
		{
			Object result = InvokeEntityMethod(ActualObject, SmallGatlingGunGetInventoryMethod, new object[] { 0 });
			return result;
		}

		#endregion
	}
}
