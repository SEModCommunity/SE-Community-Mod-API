using System;
using System.Collections.Generic;
using Sandbox.Common.Localization;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;

namespace SEModAPI.API.Definitions
{
	public class PhysicalItemsDefinition : ObjectOverLayerDefinition<MyObjectBuilder_PhysicalItemDefinition>
	{
		#region "Constructors and Initializers"

        public PhysicalItemsDefinition(MyObjectBuilder_PhysicalItemDefinition definition): base(definition)
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

		public MyTextsWrapperEnum IconSymbol
		{
		    get
            {
                    return m_baseDefinition.IconSymbol.Value;
            } 
			set
			{
                if (m_baseDefinition.IconSymbol == value) return;
                m_baseDefinition.IconSymbol = value;
				Changed = true;
			}
		}

		#endregion

        #region "Methods"

        protected override string GetNameFrom(MyObjectBuilder_PhysicalItemDefinition definition)
        {
            return definition.DisplayName;
        }
		#endregion
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////

    public class PhysicalItemDefinitionsManager : OverLayerDefinitionsManager<MyObjectBuilder_PhysicalItemDefinition, PhysicalItemsDefinition>
	{
		#region "Constructors and Initializers"

        public PhysicalItemDefinitionsManager(MyObjectBuilder_PhysicalItemDefinition[] baseDefinitions): base(baseDefinitions)
		{}

		#endregion

        #region "Methods"

        protected override PhysicalItemsDefinition CreateOverLayerSubTypeInstance(MyObjectBuilder_PhysicalItemDefinition definition)
		{
            return new PhysicalItemsDefinition(definition);
		}

        protected override MyObjectBuilder_PhysicalItemDefinition GetBaseTypeOf(PhysicalItemsDefinition overLayer)
		{
            return overLayer.BaseDefinition;
		}

        protected override bool GetChangedState(PhysicalItemsDefinition overLayer)
		{
            return overLayer.Changed;
		}

		public override void Save()
		{
			if (!this.Changed) return;

			m_configSerializer.PhysicalItemDefinitions = this.ExtractBaseDefinitions().ToArray();
			m_configSerializer.SavePhysicalItemsContentFile();
		}

		#endregion
	}
}
