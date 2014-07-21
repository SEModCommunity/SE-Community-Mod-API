using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;
using Sandbox.Definitions;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	public class DoorEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		public static string DoorNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string DoorClass = "F0D92F8F3A91716EC613ADD46F36158D";
		public static string DoorGetStateMethod = "EED93169FB8C3235596CF33BD3AA33B8";
		public static string DoorSetStateMethod = "2A0572A89EB6003FDC46A6D8420ECF79";

		#endregion

		#region "Constructors and Initializers"

		public DoorEntity(CubeGridEntity parent, MyObjectBuilder_Door definition)
			: base(parent, definition)
		{
		}

		public DoorEntity(CubeGridEntity parent, MyObjectBuilder_Door definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
		}

		#endregion

		#region "Properties"

		[Category("Door")]
		public float Opening
		{
			get { return GetSubTypeEntity().Opening; }
		}

		[Category("Door")]
		public bool State
		{
			get { return GetSubTypeEntity().State; }
			set
			{
				if (GetSubTypeEntity().State == value) return;
				GetSubTypeEntity().State = value;
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

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_Door GetSubTypeEntity()
		{
			return (MyObjectBuilder_Door)ObjectBuilder;
		}

		#region "Internal"

		protected void InternalUpdateDoor()
		{
			try
			{
				InvokeEntityMethod(BackingObject, DoorSetStateMethod, new object[] { State });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#endregion

		#endregion
	}
}
