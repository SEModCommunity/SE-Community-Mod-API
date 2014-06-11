using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;

namespace SEModAPI.API
{
	public class AmmoMagazinesDefinition : ObjectBuilderDefinition<MyObjectBuilder_AmmoMagazineDefinition>
    {
		#region "Constructors and Initializers"

		public AmmoMagazinesDefinition(MyObjectBuilder_AmmoMagazineDefinition definition)
			: base(definition)
		{
		}

		#endregion

        #region "Properties"

        public MyAmmoCategoryEnum Caliber
        {
			get { return m_definition.Category; }
			set
			{
				if (m_definition.Category == value) return;
				m_definition.Category = value;
				Changed = true;
			}
		}

        public int Capacity
        {
			get { return m_definition.Capacity; }
            set
            {
				if (m_definition.Capacity == value) return;
				m_definition.Capacity = value;
                Changed = true;
            }
        }

        public float Mass
        {
			get { return m_definition.Mass; }
            set
            {
				if (m_definition.Mass == value) return;
				m_definition.Mass = value;
                Changed = true;
            }
        }

        public float Volume
        {
			get { return m_definition.Volume.GetValueOrDefault(-1); }
            set
            {
				if (m_definition.Volume == value) return;
				m_definition.Volume = value;
                Changed = true;

            }
        }

        #endregion
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////

	public class AmmoMagazinesDefinitionsWrapper : NameIdIndexedWrapper<MyObjectBuilder_AmmoMagazineDefinition, AmmoMagazinesDefinition>
    {
        #region "Constructors and Initializers"

        public AmmoMagazinesDefinitionsWrapper(MyObjectBuilder_AmmoMagazineDefinition[] definitions)
			: base(definitions)
        {
            int index = 0;
            foreach (var definition in definitions)
            {
				m_nameTypeIndexes.Add(new KeyValuePair<string, SerializableDefinitionId>(definition.DisplayName, definition.Id), index);

                index++;
            }
        }

        #endregion

		#region "Properties"

		new public bool Changed
		{
			get
			{
				foreach (var def in m_definitions)
				{
					if (def.Value.Changed)
						return true;
				}

				return false;
			}
			set
			{
				base.Changed = value;
			}
		}

		public MyObjectBuilder_AmmoMagazineDefinition[] RawDefinitions
		{
			get
			{
				MyObjectBuilder_AmmoMagazineDefinition[] temp = new MyObjectBuilder_AmmoMagazineDefinition[m_definitions.Count];
				AmmoMagazinesDefinition[] definitionsArray = this.Definitions;

				for (int i = 0; i < definitionsArray.Length; i++)
				{
					temp[i] = definitionsArray[i].Definition;
				}

				return temp;
			}
		}

		#endregion
    }
}
