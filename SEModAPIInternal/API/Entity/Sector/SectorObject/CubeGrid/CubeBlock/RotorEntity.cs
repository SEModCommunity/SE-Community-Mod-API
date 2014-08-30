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
	[DataContract(Name = "RotorEntityProxy")]
	public class RotorEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		public static string RotorNamespace = "6DDCED906C852CFDABA0B56B84D0BD74";
		public static string RotorClass = "28EDB1838133C5D80B080389DBB2C9DB";

		public static string RotorTopBlockEntityIdField = "8B5AF0B1A3FABB9647F639CBBCEE6B9B";

		#endregion

		#region "Constructors and Intializers"

		public RotorEntity(CubeGridEntity parent, MyObjectBuilder_MotorStator definition)
			: base(parent, definition)
		{
		}

		public RotorEntity(CubeGridEntity parent, MyObjectBuilder_MotorStator definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Rotor")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(RotorNamespace, RotorClass);
				return type;
			}
		}

		[IgnoreDataMember]
		[Category("Rotor")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_MotorStator ObjectBuilder
		{
			get { return (MyObjectBuilder_MotorStator)base.ObjectBuilder; }
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[IgnoreDataMember]
		[Category("Rotor")]
		[Browsable(false)]
		[ReadOnly(true)]
		public CubeBlockEntity TopBlock
		{
			get
			{
				if (BackingObject == null || ActualObject == null)
					return null;

				long topBlockEntityId = GetTopBlockEntityId();
				if (topBlockEntityId == 0)
					return null;
				BaseObject baseObject = GameEntityManager.GetEntity(topBlockEntityId);
				if (!(baseObject is CubeBlockEntity))
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
		[Category("Rotor")]
		[ReadOnly(true)]
		public long TopBlockId
		{
			get
			{
				if (BackingObject == null || ActualObject == null)
					return ObjectBuilder.RotorEntityId;

				return GetTopBlockEntityId();
			}
			private set
			{
				//Do nothing!
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
					throw new Exception("Could not find internal type for RotorEntity");

				result &= HasField(type, RotorTopBlockEntityIdField);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		protected long GetTopBlockEntityId()
		{
			Object rawResult = GetEntityFieldValue(ActualObject, RotorTopBlockEntityIdField);
			if (rawResult == null)
				return 0;
			long result = (long)rawResult;
			return result;
		}

		#endregion
	}
}
