using System;
using System.Collections.Generic;
using Sandbox.Common.Localization;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;

namespace SEModAPI.API.Definitions
{
	public class ComponentsDefinition : ObjectOverLayerDefinition<MyObjectBuilder_ComponentDefinition>
	{
		#region "Constructors and Initializers"

		public ComponentsDefinition(MyObjectBuilder_ComponentDefinition definition): base(definition)
		{}

		#endregion

        #region "Properties"

		public VRageMath.Vector3 Size
		{
			get { return m_baseDefinition.Size; }
			set
			{
				if (m_baseDefinition.Size == value) return;
				m_baseDefinition.Size = value;
				Changed = true;
			}
		}

		public float Mass
		{
			get { return m_baseDefinition.Mass; }
			set
			{
				if (m_baseDefinition.Mass == value) return;
				m_baseDefinition.Mass = value;
				Changed = true;
			}
		}

		public float Volume
		{
			get { return m_baseDefinition.Volume.Value; }
			set
			{
				if (m_baseDefinition.Volume == value) return;
				m_baseDefinition.Volume = value;
				Changed = true;
			}
		}

		public string Model
		{
			get { return m_baseDefinition.Model; }
			set
			{
				if (m_baseDefinition.Model == value) return;
				m_baseDefinition.Model = value;
				Changed = true;
			}
		}

		public string Icon
		{
			get { return m_baseDefinition.Icon; }
			set
			{
				if (m_baseDefinition.Icon == value) return;
				m_baseDefinition.Icon = value;
				Changed = true;
			}
		}

		public int MaxIntegrity
		{
			get { return m_baseDefinition.MaxIntegrity; }
			set
			{
                if (m_baseDefinition.MaxIntegrity == value) return;
                m_baseDefinition.MaxIntegrity = value;
				Changed = true;
			}
		}

		public float DropProbability
		{
            get { return m_baseDefinition.DropProbability; }
			set
			{
                if (m_baseDefinition.DropProbability == value) return;
                m_baseDefinition.DropProbability = value;
				Changed = true;
			}
		}

		#endregion

        #region "Methods"

        protected override string GetNameFrom(MyObjectBuilder_ComponentDefinition definition)
        {
            return definition.DisplayName;
        }

        #endregion
	}

	public class ComponentDefinitionsManager : SerializableDefinitionsManager<MyObjectBuilder_ComponentDefinition, ComponentsDefinition>
	{
	}
}
