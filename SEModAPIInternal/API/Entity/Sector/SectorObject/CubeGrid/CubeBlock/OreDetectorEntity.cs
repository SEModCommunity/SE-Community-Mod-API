using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract(Name = "OreDetectorEntityProxy")]
	public class OreDetectorEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private bool m_broadcastUsingAntennas;
		private float m_detectionRadius;

		public static string OreDetectorNamespace = "6DDCED906C852CFDABA0B56B84D0BD74";
		public static string OreDetectorClass = "E12CF12895FC2AA4037DD098DE6979F2";

		public static string OreDetectorGetUsingAntennasMethod = "A86CE4519A974D361F832EFA29F3342C";
		public static string OreDetectorSetUsingAntennasMethod = "CBB6BC8CD97EA91D61606A387C079BD8";
		public static string OreDetectorGetDetectionRadiusMethod = "0D25F25489E9EF61F63427ACE5AF9855";
		public static string OreDetectorSetDetectionRadiusMethod = "375A34828B7F1E5C64132B38ECB59406";

		#endregion

		#region "Constructors and Initializers"

		public OreDetectorEntity(CubeGridEntity parent, MyObjectBuilder_OreDetector definition)
			: base(parent, definition)
		{
			m_broadcastUsingAntennas = definition.BroadcastUsingAntennas;
			m_detectionRadius = definition.DetectionRadius;
		}

		public OreDetectorEntity(CubeGridEntity parent, MyObjectBuilder_OreDetector definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
			m_broadcastUsingAntennas = definition.BroadcastUsingAntennas;
			m_detectionRadius = definition.DetectionRadius;
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Ore Detector")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_OreDetector ObjectBuilder
		{
			get
			{
				return (MyObjectBuilder_OreDetector)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[IgnoreDataMember]
		[Category("Ore Detector")]
		[Browsable(false)]
		[ReadOnly(true)]
		new public static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(OreDetectorNamespace, OreDetectorClass);
				return type;
			}
		}

		[DataMember]
		[Category("Ore Detector")]
		public bool BroadcastUsingAntennas
		{
			get
			{
				if (BackingObject == null || ActualObject == null)
					return ObjectBuilder.BroadcastUsingAntennas;

				return GetUsingAntennas();
			}
			set
			{
				if (BroadcastUsingAntennas == value) return;
				ObjectBuilder.BroadcastUsingAntennas = value;
				m_broadcastUsingAntennas = value;
				Changed = true;

				if (BackingObject != null && ActualObject != null)
				{
					Action action = InternalUpdateUsingAntennas;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[DataMember]
		[Category("Ore Detector")]
		public float DetectionRadius
		{
			get
			{
				if (BackingObject == null || ActualObject == null)
					return ObjectBuilder.DetectionRadius;

				return GetDetectionRadius();
			}
			set
			{
				if (DetectionRadius == value) return;
				ObjectBuilder.DetectionRadius = value;
				m_detectionRadius = value;
				Changed = true;

				if (BackingObject != null && ActualObject != null)
				{
					Action action = InternalUpdateDetectionRadius;
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
					throw new Exception("Could not find internal type for OreDetectorEntity");

				result &= HasMethod(type, OreDetectorGetUsingAntennasMethod);
				result &= HasMethod(type, OreDetectorSetUsingAntennasMethod);
				result &= HasMethod(type, OreDetectorGetDetectionRadiusMethod);
				result &= HasMethod(type, OreDetectorSetDetectionRadiusMethod);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		protected bool GetUsingAntennas()
		{
			Object rawResult = InvokeEntityMethod(ActualObject, OreDetectorGetUsingAntennasMethod);
			if (rawResult == null)
				return false;
			bool result = (bool)rawResult;
			return result;
		}

		protected void InternalUpdateUsingAntennas()
		{
			InvokeEntityMethod(ActualObject, OreDetectorSetUsingAntennasMethod, new object[] { m_broadcastUsingAntennas });
		}

		protected float GetDetectionRadius()
		{
			Object rawResult = InvokeEntityMethod(ActualObject, OreDetectorGetDetectionRadiusMethod);
			if (rawResult == null)
				return 0;
			float result = (float)rawResult;
			return result;
		}

		protected void InternalUpdateDetectionRadius()
		{
			InvokeEntityMethod(ActualObject, OreDetectorSetDetectionRadiusMethod, new object[] { m_detectionRadius });
		}

		#endregion
	}
}
