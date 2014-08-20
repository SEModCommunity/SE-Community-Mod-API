using System.ComponentModel;
using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class CockpitDefinition : BlockDefinition
	{
		#region "Constructors and Initializers"

		public CockpitDefinition(MyObjectBuilder_CockpitDefinition definition)
			: base(definition)
		{ }

		#endregion

		#region "Properties"

		/// <summary>
		/// The current character animation for this Cockpit
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current character animation for this Cockpit.")]
		public string CharacterAnimation
		{
			get { return GetSubTypeDefinition().CharacterAnimation; }
			set
			{
				if (GetSubTypeDefinition().CharacterAnimation == value) return;
				GetSubTypeDefinition().CharacterAnimation = value;
				Changed = true;
			}
		}

		/// <summary>
		/// Set or Get the possibility to enable first person
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the possibility to enable first person.")]
		public bool EnableFirstPerson
		{
			get { return GetSubTypeDefinition().EnableFirstPerson; }
			set
			{
				if (GetSubTypeDefinition().EnableFirstPerson == value) return;
				GetSubTypeDefinition().EnableFirstPerson = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Cockpit glass model
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Cockpit glass model.")]
		public string GlassModel
		{
			get { return GetSubTypeDefinition().GlassModel; }
			set
			{
				if (GetSubTypeDefinition().GlassModel == value) return;
				GetSubTypeDefinition().GlassModel = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Cockpit interior model
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Cockpit interior model.")]
		public string InteriorModel
		{
			get { return GetSubTypeDefinition().InteriorModel; }
			set
			{
				if (GetSubTypeDefinition().InteriorModel == value) return;
				GetSubTypeDefinition().InteriorModel = value;
				Changed = true;
			}
		}

		/// <summary>
		/// Set or Get the possibility to enable ship control
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the possibility to enable ship control.")]
		public bool EnableShipControl
		{
			get { return GetSubTypeDefinition().EnableShipControl; }
			set
			{
				if (GetSubTypeDefinition().EnableShipControl == value) return;
				GetSubTypeDefinition().EnableShipControl = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new public MyObjectBuilder_CockpitDefinition GetSubTypeDefinition()
		{
			return (MyObjectBuilder_CockpitDefinition)m_baseDefinition;
		}

		#endregion


	}
}