using Microsoft.Xml.Serialization.GeneratedAssembly;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Reflection;
using ProtoBuf;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;

using SEModAPI.Support;

namespace SEModAPI.API.Definitions
{
	/// <summary>
	/// This class is only intended for easy data access and management. It is a wrapper around all MyObjectBuilder_DefinitionsBase children sub type
	/// </summary>
	public abstract class DefinitionOverLayer
	{
		#region "Attributes"

		protected MyObjectBuilder_DefinitionBase m_baseDefinition;

		#endregion

		#region "Constructors and Initializers"

		protected DefinitionOverLayer(MyObjectBuilder_DefinitionBase baseDefinition)
		{
			m_baseDefinition = baseDefinition;
		}

		#endregion

		#region "Properties"

		/// <summary>
		/// Gets the changed status of the object
		/// </summary>
		[Browsable(true)]
		[Description("Represent the state of changes in the object")]
		public virtual bool Changed { get; protected set; }

		/// <summary>
		/// Obtain the API formated name of the object
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		[Description("The formatted name of the object")]
		public abstract string Name { get; }

		/// <summary>
		/// Gets the Internal data of the object
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		[Description("Internal data of the object")]
		protected MyObjectBuilder_DefinitionBase BaseDefinition
		{
			get { return m_baseDefinition; }
		}

		/// <summary>
		/// Gets the internal data of the object
		/// </summary>
		[Browsable(true)]
		[ReadOnly(true)]
		[Description("The value ID representing the type of the object")]
		public MyObjectBuilderType TypeId
		{
			get { return m_baseDefinition.TypeId; }
		}

		/// <summary>
		/// Gets the internal Id of the instance
		/// </summary>
		[Browsable(true)]
		[Description("The Id as SerializableDefinitionId that represent the Object representation")]
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

		/// <summary>
		/// Get the description of the object
		/// </summary>
		[Browsable(true)]
		[Description("Represents the description of the object")]
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
}