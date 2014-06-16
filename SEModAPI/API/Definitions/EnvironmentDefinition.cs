using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;
using VRageMath;

namespace SEModAPI.API.Definitions
{
    public class EnvironmentDefinition
	{
		#region "Attributes"

		MyObjectBuilder_EnvironmentDefinition m_baseDefinition;

		#endregion

		#region "Constructors and Initializers"

		public EnvironmentDefinition()
		{}

		#endregion

		#region "Properties"

		public bool Changed
		{
			get;
			private set;
		}

		public Vector3 SunDirection
		{
			get { return m_baseDefinition.SunDirection; }
			set
			{
				if (m_baseDefinition.SunDirection == value) return;
				m_baseDefinition.SunDirection = value;
				Changed = true;
			}
		}

		public string EnvironmentTexture
		{
			get { return m_baseDefinition.EnvironmentTexture; }
			set
			{
				if (m_baseDefinition.EnvironmentTexture == value) return;
				m_baseDefinition.EnvironmentTexture = value;
				Changed = true;
			}
		}

		public MyOrientation EnvironmentOrientation
		{
			get { return m_baseDefinition.EnvironmentOrientation; }
			set
			{
				if (m_baseDefinition.EnvironmentOrientation.Pitch == value.Pitch &&
					m_baseDefinition.EnvironmentOrientation.Roll == value.Roll &&
					m_baseDefinition.EnvironmentOrientation.Yaw == value.Yaw) 
					return;
				m_baseDefinition.EnvironmentOrientation = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

        #endregion
    }
}
