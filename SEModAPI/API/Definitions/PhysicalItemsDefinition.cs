using System;
using System.Collections.Generic;
using Sandbox.Common.Localization;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;

namespace SEModAPI.API.Definitions
{
	class PhysicalItemsDefinition<T> : ObjectBuilderDefinition<T> where T : MyObjectBuilder_PhysicalItemDefinition
	{
		#region "Constructors and Initializers"

		public PhysicalItemsDefinition(T definition)
			: base(definition)
		{
		}

		#endregion

        #region "Properties"

        public VRageMath.Vector3 Size
        {
            get { return m_definition.Size; }
            set
            {
				if (m_definition.Size == value) return;
				m_definition.Size = value;
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
			get { return m_definition.Volume.Value; }
			set
			{
				if (m_definition.Volume == value) return;
				m_definition.Volume = value;
				Changed = true;
			}
		}

		public string Model
		{
			get { return m_definition.Model; }
			set
			{
				if (m_definition.Model == value) return;
				m_definition.Model = value;
				Changed = true;
			}
		}

		public string Icon
		{
			get { return m_definition.Icon; }
			set
			{
				if (m_definition.Icon == value) return;
				m_definition.Icon = value;
				Changed = true;
			}
		}

		public MyTextsWrapperEnum IconSymbol
		{
			get { return m_definition.IconSymbol.Value; }
			set
			{
				if (m_definition.IconSymbol == value) return;
				m_definition.IconSymbol = value;
				Changed = true;
			}
		}

		#endregion
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////

	public class PhysicalItemDefinitionsWrapper<T> : NameIdIndexedWrapper<T, PhysicalItemsDefinition<T>> where T : MyObjectBuilder_PhysicalItemDefinition
	{
		#region "Constructors and Initializers"

		public PhysicalItemDefinitionsWrapper(T[] definitions)
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

		public T[] RawDefinitions
		{
			get
			{
				T[] temp = new T[m_definitions.Count];
				PhysicalItemsDefinition<T>[] definitionsArray = this.Definitions;

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
