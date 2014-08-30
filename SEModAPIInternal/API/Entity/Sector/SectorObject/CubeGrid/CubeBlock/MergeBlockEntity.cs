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
	[DataContract(Name = "MergeBlockEntityProxy")]
	public class MergeBlockEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		public static string MergeBlockNamespace = "AAD9061F948E6A3635200145188D64A9";
		public static string MergeBlockClass = "D6D88AF33E0073B53DC2A3445C9F12EC";

		public static string MergeBlockConnectedMergeBlockField = "0599F03E0870E6CBD4773A36B6866FA6";

		#endregion

		#region "Constructors and Intializers"

		public MergeBlockEntity(CubeGridEntity parent, MyObjectBuilder_MergeBlock definition)
			: base(parent, definition)
		{
		}

		public MergeBlockEntity(CubeGridEntity parent, MyObjectBuilder_MergeBlock definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Merge Block")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(MergeBlockNamespace, MergeBlockClass);
				return type;
			}
		}

		[IgnoreDataMember]
		[Category("Merge Block")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_MergeBlock ObjectBuilder
		{
			get { return (MyObjectBuilder_MergeBlock)base.ObjectBuilder; }
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[IgnoreDataMember]
		[Category("Merge Block")]
		[Browsable(false)]
		[ReadOnly(true)]
		public CubeGridEntity AttachedCubeGrid
		{
			get
			{
				if (BackingObject == null || ActualObject == null)
					return null;

				CubeGridEntity attachedGrid = null;
				try
				{
					Object connectedMergeBlock = GetConnectedBlock();
					if (connectedMergeBlock != null)
					{
						Object backingGrid = CubeBlockEntity.GetInternalParentCubeGrid(connectedMergeBlock);
						long entityId = BaseEntity.GetEntityId(backingGrid);
						BaseObject matchedObject = GameEntityManager.GetEntity(entityId);
						if (matchedObject is CubeGridEntity)
						{
							attachedGrid = (CubeGridEntity)matchedObject;
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.ErrorLog.WriteLine(ex);
				}
				return attachedGrid;
			}
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category("Merge Block")]
		[ReadOnly(true)]
		public bool IsAttached
		{
			get
			{
				if (BackingObject == null || ActualObject == null)
					return false;

				return (GetConnectedBlock() != null);
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
					throw new Exception("Could not find internal type for MergeBlockEntity");

				result &= HasField(type, MergeBlockConnectedMergeBlockField);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		protected Object GetConnectedBlock()
		{
			Object result = GetEntityFieldValue(ActualObject, MergeBlockConnectedMergeBlockField);
			return result;
		}

		#endregion
	}
}
