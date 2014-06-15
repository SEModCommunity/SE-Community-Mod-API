using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;
using VRageMath;

namespace SEModAPI.API.Definitions
{
    public class EnvironmentDefinition : OverLayerDefinition<MyObjectBuilder_EnvironmentDefinition>
	{
		#region "Constructors and Initializers"

		public EnvironmentDefinition(MyObjectBuilder_EnvironmentDefinition definition): base(definition)
		{}

		#endregion

		#region "Properties"

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

		protected override string GetNameFrom(MyObjectBuilder_EnvironmentDefinition definition)
        {
            return null;
        }

        #endregion
    }

	public class EnvironmentDefinitionManager : SerializableDefinitionsManager<MyObjectBuilder_EnvironmentDefinition, EnvironmentDefinition>
	{ }
}
