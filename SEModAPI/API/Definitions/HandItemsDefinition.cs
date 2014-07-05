using System;
using System.Collections.Generic;
using System.ComponentModel;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;
using VRageMath;

namespace SEModAPI.API.Definitions
{
	public class HandItemsDefinition : OverLayerDefinition<MyObjectBuilder_HandItemDefinition>
	{
		#region "Constructors and Initializers"

		public HandItemsDefinition(MyObjectBuilder_HandItemDefinition definition)
			: base(definition)
		{ }

		#endregion

		#region "Properties"

		[Browsable(true)]
		[ReadOnly(true)]
		[Description("Get the formatted name of the hand item")]
		new public string Name
		{
			get { return m_baseDefinition.Id.TypeId.ToString(); }
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the orientation of the item in the left hand. (Need details)")]
		public Quaternion LeftHandOrientation
		{
			get { return m_baseDefinition.LeftHandOrientation; }
			set
			{
				if (m_baseDefinition.LeftHandOrientation == value) return;
				m_baseDefinition.LeftHandOrientation = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the position of the item in the left hand. (Need details)")]
		public Vector3 LeftHandPosition
		{
			get { return m_baseDefinition.LeftHandPosition; }
			set
			{
				if (m_baseDefinition.LeftHandPosition == value) return;
				m_baseDefinition.LeftHandPosition = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the orientation of the item in the right hand. (Need details)")]
		public Quaternion RightHandOrientation
		{
			get { return m_baseDefinition.RightHandOrientation; }
			set
			{
				if (m_baseDefinition.RightHandOrientation == value) return;
				m_baseDefinition.RightHandOrientation = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the orientation of the item in the right hand. (Need details)")]
		public Vector3 RightHandPosition
		{
			get { return m_baseDefinition.RightHandPosition; }
			set
			{
				if (m_baseDefinition.RightHandPosition == value) return;
				m_baseDefinition.RightHandPosition = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the position of the item. (Need details)")]
		public Quaternion ItemOrientation
		{
			get { return m_baseDefinition.ItemOrientation; }
			set
			{
				if (m_baseDefinition.ItemOrientation == value) return;
				m_baseDefinition.ItemOrientation = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the orientation of the item. (Need details)")]
		public Vector3 ItemPosition
		{
			get { return m_baseDefinition.ItemPosition; }
			set
			{
				if (m_baseDefinition.ItemPosition == value) return;
				m_baseDefinition.ItemPosition = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the blend time. (Need details)")]
		public float BlendTime
		{
			get { return m_baseDefinition.BlendTime; }
			set
			{
				if (m_baseDefinition.BlendTime == value) return;
				m_baseDefinition.BlendTime = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the amplitude offset on the X axis. (Need details)")]
		public float XAmplitudeOffset
		{
			get { return m_baseDefinition.XAmplitudeOffset; }
			set
			{
				if (m_baseDefinition.XAmplitudeOffset == value) return;
				m_baseDefinition.XAmplitudeOffset = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the amplitude offset on the Y axis. (Need details)")]
		public float YAmplitudeOffset
		{
			get { return m_baseDefinition.YAmplitudeOffset; }
			set
			{
				if (m_baseDefinition.YAmplitudeOffset == value) return;
				m_baseDefinition.YAmplitudeOffset = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the amplitude offset on the Z axis. (Need details)")]
		public float ZAmplitudeOffset
		{
			get { return m_baseDefinition.ZAmplitudeOffset; }
			set
			{
				if (m_baseDefinition.ZAmplitudeOffset == value) return;
				m_baseDefinition.ZAmplitudeOffset = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the amplitude scale on the X axis. (Need details)")]
		public float XAmplitudeScale
		{
			get { return m_baseDefinition.XAmplitudeScale; }
			set
			{
				if (m_baseDefinition.XAmplitudeScale == value) return;
				m_baseDefinition.XAmplitudeScale = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the amplitude scale on the Y axis. (Need details)")]
		public float YAmplitudeScale
		{
			get { return m_baseDefinition.YAmplitudeScale; }
			set
			{
				if (m_baseDefinition.YAmplitudeScale == value) return;
				m_baseDefinition.YAmplitudeScale = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the amplitude scale on the Z axis. (Need details)")]
		public float ZAmplitudeScale
		{
			get { return m_baseDefinition.ZAmplitudeScale; }
			set
			{
				if (m_baseDefinition.ZAmplitudeScale == value) return;
				m_baseDefinition.ZAmplitudeScale = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the run multiplier. (Need details)")]
		public float RunMultiplier
		{
			get { return m_baseDefinition.RunMultiplier; }
			set
			{
				if (m_baseDefinition.RunMultiplier == value) return;
				m_baseDefinition.RunMultiplier = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the item walking orientation in 3rd person. (Need details AND confirmation)")]
		public Quaternion ItemWalkingOrientation3rd
		{
			get { return m_baseDefinition.ItemWalkingOrientation3rd; }
			set
			{
				if (m_baseDefinition.ItemWalkingOrientation3rd == value) return;
				m_baseDefinition.ItemWalkingOrientation3rd = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the item walking position in 3rd person. (Need details AND confirmation)")]
		public Vector3 ItemWalkingPosition3rd
		{
			get { return m_baseDefinition.ItemWalkingPosition3rd; }
			set
			{
				if (m_baseDefinition.ItemWalkingPosition3rd == value) return;
				m_baseDefinition.ItemWalkingPosition3rd = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the fingers animation.")]
		public string FingersAnimation
		{
			get { return m_baseDefinition.FingersAnimation; }
			set
			{
				if (m_baseDefinition.FingersAnimation == value) return;
				m_baseDefinition.FingersAnimation = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the shooting orientation. (Need details)")]
		public Quaternion ItemShootOrientation
		{
			get { return m_baseDefinition.ItemShootOrientation; }
			set
			{
				if (m_baseDefinition.ItemShootOrientation == value) return;
				m_baseDefinition.ItemShootOrientation = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the shooting position. (Need details)")]
		public Vector3 ItemShootPosition
		{
			get { return m_baseDefinition.ItemShootPosition; }
			set
			{
				if (m_baseDefinition.ItemShootPosition == value) return;
				m_baseDefinition.ItemShootPosition = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the shooting orientation in 3rd person. (Need details AND confirmation)")]
		public Quaternion ItemShootOrientation3rd
		{
			get { return m_baseDefinition.ItemShootOrientation3rd; }
			set
			{
				if (m_baseDefinition.ItemShootOrientation3rd == value) return;
				m_baseDefinition.ItemShootOrientation3rd = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the shooting position in 3rd person. (Need details AND confirmation)")]
		public Vector3 ItemShootPosition3rd
		{
			get { return m_baseDefinition.ItemShootPosition3rd; }
			set
			{
				if (m_baseDefinition.ItemShootPosition3rd == value) return;
				m_baseDefinition.ItemShootPosition3rd = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the shoot blend. (Need details)")]
		public float ShootBlend
		{
			get { return m_baseDefinition.ShootBlend; }
			set
			{
				if (m_baseDefinition.ShootBlend == value) return;
				m_baseDefinition.ShootBlend = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the muzzle position. (Need details)")]
		public Vector3 MuzzlePosition
		{
			get { return m_baseDefinition.MuzzlePosition; }
			set
			{
				if (m_baseDefinition.MuzzlePosition == value) return;
				m_baseDefinition.MuzzlePosition = value;
				Changed = true;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the shooting scatter. (Need details)")]
		public Vector3 ShootScatter
		{
			get { return m_baseDefinition.ShootScatter; }
			set
			{
				if (m_baseDefinition.ShootScatter == value) return;
				m_baseDefinition.ShootScatter = value;
				return;
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the scatter speed. (Need details)")]
		public float ScatterSpeed
		{
			get { return m_baseDefinition.ScatterSpeed; }
			set
			{
				if (m_baseDefinition.ScatterSpeed == value) return;
				m_baseDefinition.ScatterSpeed = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_HandItemDefinition definition)
		{
			return definition.DisplayName;
		}

		#endregion
	}
}
