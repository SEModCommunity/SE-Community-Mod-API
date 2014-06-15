using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.API.Definitions;

namespace SEModAPI.API.Definitions
{
	public class AmmoMagazinesDefinition : ObjectOverLayerDefinition<MyObjectBuilder_AmmoMagazineDefinition>
    {
		#region "Constructors and Initializers"

		public AmmoMagazinesDefinition(MyObjectBuilder_AmmoMagazineDefinition definition): base(definition)
		{}

		#endregion

        #region "Properties"

		public string DisplayName
		{
			get { return m_baseDefinition.DisplayName; }
			set
			{
				if (m_baseDefinition.DisplayName == value) return;
				m_baseDefinition.DisplayName = value;
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

    public class AmmoMagazinesDefinitionsManager : SerializableDefinitionsManager<MyObjectBuilder_AmmoMagazineDefinition, AmmoMagazinesDefinition>
    {
    }
}
