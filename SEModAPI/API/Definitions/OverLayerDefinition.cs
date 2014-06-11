using System.Collections.Generic;
using System.Linq;
using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions
{
    /// <summary>
    /// This class is only intended for easy data access and management. It is a wrapper around all MyObjectBuilder_Definitions children sub type
    /// </summary>
    public abstract class OverLayerDefinition<TMyObjectBuilder_Definitions_SubType>
	{
		#region "Attributes"

        protected TMyObjectBuilder_Definitions_SubType m_baseDefinition;
        protected string m_name;

		#endregion

		#region "Constructors and Initializers"

        protected OverLayerDefinition(TMyObjectBuilder_Definitions_SubType baseDefinition)
		{
			m_baseDefinition = baseDefinition;
            m_name = GetNameFrom(m_baseDefinition);
			Changed = false;
		}

		#endregion

		#region "Properties"

		public bool Changed { get; protected set; }

        public string Name { get; protected set; }

        public TMyObjectBuilder_Definitions_SubType BaseDefinition
		{
			get { return m_baseDefinition; }
		}

		#endregion

        #region "Methods"

        /// <summary>
        /// This template method should return the representative name of the sub type underlayed by a children
        /// </summary>
        /// <param name="definition">The definition from which to get the information</param>
        /// <returns></returns>
        protected abstract string GetNameFrom(TMyObjectBuilder_Definitions_SubType definition);

        #endregion
	}

	public abstract class ObjectOverLayerDefinition<TMyObjectBuilder_Definitions_SubType> : OverLayerDefinition<TMyObjectBuilder_Definitions_SubType> where TMyObjectBuilder_Definitions_SubType : MyObjectBuilder_DefinitionBase
	{
		#region "Constructors and Initializers"

		protected ObjectOverLayerDefinition(TMyObjectBuilder_Definitions_SubType baseDefinition)
			: base(baseDefinition)
		{}

		#endregion

		#region "Properties"

		public SerializableDefinitionId Id
		{
			get { return m_baseDefinition.Id; }
			set
			{
				if (m_baseDefinition.Id.ToString() == value.ToString()) return;
				m_baseDefinition.Id = value;
				Changed = true;
			}
		}

		new public string Name
		{
			get { return m_baseDefinition.DisplayName; }
			set
			{
				if (m_baseDefinition.DisplayName == value) return;
				m_baseDefinition.DisplayName = value;
				Changed = true;
			}
		}

		public string Description
		{
			get { return m_baseDefinition.Description; }
			set
			{
				if (m_baseDefinition.Description == value) return;
				m_baseDefinition.Description = value;
				Changed = true;
			}
		}

		#endregion
	}

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public abstract class OverLayerDefinitionsManager<TMyObjectBuilder_Definitions_SubType, TOverLayerDefinition_SubType>
	{
		#region "Attributes"

		protected bool m_changed = false;
        protected long m_index = 0;
		protected ConfigFileSerializer m_configSerializer;

        //Use Long (key) as Id and OverLayerDefinition sub type (value) as Name
        protected Dictionary<long, TOverLayerDefinition_SubType> m_definitions = new Dictionary<long, TOverLayerDefinition_SubType>();

		#endregion

		#region "Constructors and Initializers"

        protected OverLayerDefinitionsManager(TMyObjectBuilder_Definitions_SubType[] baseDefinitions)
		{
			m_changed = false;
			m_configSerializer = new ConfigFileSerializer();

			foreach (var definition in baseDefinitions)
			{
			    AddChildrenFrom(definition);
			}
		}

        public void AddChildrenFrom(TMyObjectBuilder_Definitions_SubType definition)
        {
            m_definitions.Add(m_index, CreateOverLayerSubTypeInstance(definition));
            ++m_index;
        }

		#endregion

		#region "Properties"

        protected bool Changed
        {
            get
            {
                foreach (var def in m_definitions)
                {
                    if (GetChangedState(def.Value))
                        return true;
                }
                return false;
            }
        }

        public TOverLayerDefinition_SubType[] Definitions
		{
            get
            {
                return m_definitions.Values.ToArray();
            }
		}

		#endregion

		#region "Methods"

        /// <summary>
        /// This method is used to extract all instances of MyObjectBuilder_Definitions sub type encapsulated in the manager
        /// This method is slow and should only be used to extract the underlying type
        /// </summary>
        /// <returns>All instances of the MyObjectBuilder_Definitions sub type sub type in the manager</returns>
		public List<TMyObjectBuilder_Definitions_SubType> ExtractBaseDefinitions()
		{
            List<TMyObjectBuilder_Definitions_SubType> list = new List<TMyObjectBuilder_Definitions_SubType>();
            foreach (var def in m_definitions.Values)
		    {
		        list.Add(GetBaseTypeOf(def));
		    }
		    return list;
		}

        /// <summary>
        /// This method is to find the index of a specific Id
        /// </summary>
        /// <param name="id">The Id to search for</param>
        /// <returns>The index where to find the definition</returns>
		public long IndexOf(long id)
		{
		    return m_definitions.ContainsKey(id) ? id : -1;
		}

        private bool IsIndexValid(int index)
		{
            return (index < m_definitions.Keys.Count && index >= 0);
		}

        public TOverLayerDefinition_SubType DefinitionOf(int index)
        {
            return IsIndexValid(index) ? m_definitions.Values.ToArray()[index]: default(TOverLayerDefinition_SubType);
        }

        /// <summary>
        /// Template method that create the instance of the children of OverLayerDefinition sub type
        /// </summary>
        /// <param name="definition">MyObjectBuilder_Definitions object</param>
        /// <returns>An instance representing OverLayerDefinition sub type</returns>
        protected abstract TOverLayerDefinition_SubType CreateOverLayerSubTypeInstance(TMyObjectBuilder_Definitions_SubType definition);

        /// <summary>
        /// This template method is intended to extact the BaseObject inside the overlayer
        /// </summary>
        /// <param name="overLayer">the over layer from which to extract the base object</param>
        /// <returns>MyObjectBuilder_Definitions Sub Type</returns>
        protected abstract TMyObjectBuilder_Definitions_SubType GetBaseTypeOf(TOverLayerDefinition_SubType overLayer);

        /// <summary>
        /// This template method is intended to know if the state of the object insate the overlayer has changed
        /// </summary>
        /// <param name="overLayer">the overlayer from which to know if the base type has changed</param>
        /// <returns>if the underlying object has changed</returns>
        protected abstract bool GetChangedState(TOverLayerDefinition_SubType overLayer);

		/// <summary>
		/// This template method is used to save the definitions out to the file
		/// </summary>
		public abstract void Save();

        #endregion
	}
}
