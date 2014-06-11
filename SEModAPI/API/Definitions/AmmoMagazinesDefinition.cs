using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.API.Definitions;

namespace SEModAPI.API.Definitions
{
    public class AmmoMagazinesDefinition : OverLayerDefinition<MyObjectBuilder_AmmoMagazineDefinition>
    {
		#region "Constructors and Initializers"

		public AmmoMagazinesDefinition(MyObjectBuilder_AmmoMagazineDefinition definition): base(definition)
		{}

		#endregion

        #region "Properties"

        public MyAmmoCategoryEnum Caliber
        {
            get { return m_baseDefinition.Category; }
			set
			{
                if (m_baseDefinition.Category == value) return;
                m_baseDefinition.Category = value;
				Changed = true;
			}
		}

        public int Capacity
        {
            get { return m_baseDefinition.Capacity; }
            set
            {
                if (m_baseDefinition.Capacity == value) return;
                m_baseDefinition.Capacity = value;
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
            get { return m_baseDefinition.Volume.GetValueOrDefault(-1); }
            set
            {
                if (m_baseDefinition.Volume == value) return;
                m_baseDefinition.Volume = value;
                Changed = true;

            }
        }

        #endregion

        #region "Methods"

        protected override string GetNameFrom(MyObjectBuilder_AmmoMagazineDefinition definition)
        {
            return definition.DisplayName;
    }

        #endregion
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////

    public class AmmoMagazinesDefinitionsManager : OverLayerDefinitionsManager<MyObjectBuilder_AmmoMagazineDefinition, AmmoMagazinesDefinition>
    {
        #region "Constructors and Initializers"

        public AmmoMagazinesDefinitionsManager(MyObjectBuilder_AmmoMagazineDefinition[] definitions): base(definitions)
        {}

        #endregion

        #region "Methods"

        protected override AmmoMagazinesDefinition CreateOverLayerSubTypeInstance(MyObjectBuilder_AmmoMagazineDefinition definition)
		{
            return new AmmoMagazinesDefinition(definition);
		}

        protected override MyObjectBuilder_AmmoMagazineDefinition GetBaseTypeOf(AmmoMagazinesDefinition overLayer)
		{
            return overLayer.BaseDefinition;
		}

        protected override bool GetChangedState(AmmoMagazinesDefinition overLayer)
		{
            return overLayer.Changed;
		}

		public override void Save()
		{
			if (!this.Changed) return;

			m_configSerializer.AmmoMagazineDefinitions = this.ExtractBaseDefinitions().ToArray();
			m_configSerializer.SaveAmmoMagazinesContentFile();
		}

		#endregion
    }
}
