using System;
using System.Collections.Generic;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;
using VRageMath;

using SEModAPI.Support;

namespace SEModAPI.API.Definitions
{
	public class TransparentMaterialsDefinition : OverLayerDefinition<MyObjectBuilder_TransparentMaterial>
	{
		#region "Constructors and Initializers"

		public TransparentMaterialsDefinition(MyObjectBuilder_TransparentMaterial definition)
			: base(definition)
		{ }

		#endregion

		#region "Properties"

		new public string Name
		{
			get { return m_baseDefinition.Name; }
			set
			{
				if (m_baseDefinition.Name == value) return;
				m_baseDefinition.Name = value;
				Changed = true;
			}
		}

		public bool AlphaMistingEnable
		{
			get { return m_baseDefinition.AlphaMistingEnable; }
			set
			{
				if (m_baseDefinition.AlphaMistingEnable == value) return;
				m_baseDefinition.AlphaMistingEnable = value;
				Changed = true;
			}
		}

		public bool CanBeAffectedByOtherLights
		{
			get { return m_baseDefinition.CanBeAffectedByOtherLights; }
			set
			{
				if (m_baseDefinition.CanBeAffectedByOtherLights == value) return;
				m_baseDefinition.CanBeAffectedByOtherLights = value;
				Changed = true;
			}
		}

		public float Emissivity
		{
			get { return m_baseDefinition.Emissivity; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException();
				if (m_baseDefinition.Emissivity == value) return;
				m_baseDefinition.Emissivity = value;
				Changed = true;
			}
		}

		public bool IgnoreDepth
		{
			get { return m_baseDefinition.IgnoreDepth; }
			set
			{
				if (m_baseDefinition.IgnoreDepth == value) return;
				m_baseDefinition.IgnoreDepth = value;
				Changed = true;
			}
		}

		public float SoftParticleDistanceScale
		{
			get { return m_baseDefinition.SoftParticleDistanceScale; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException();
				if (m_baseDefinition.SoftParticleDistanceScale == value) return;
				m_baseDefinition.SoftParticleDistanceScale = value;
				Changed = true;
			}
		}

		public string Texture
		{
			get { return m_baseDefinition.Texture; }
			set
			{
				if (m_baseDefinition.Texture == value) return;
				m_baseDefinition.Texture = value;
				Changed = true;
			}
		}

		public bool UseAtlas
		{
			get { return m_baseDefinition.UseAtlas; }
			set
			{
				if (m_baseDefinition.UseAtlas == value) return;
				m_baseDefinition.UseAtlas = value;
				Changed = true;
			}
		}

		public Vector2 UVOffset
		{
			get { return m_baseDefinition.UVOffset; }
			set
			{
				if (m_baseDefinition.UVOffset == value) return; 
				m_baseDefinition.UVOffset = value;
				Changed = true;
			}
		}

		public Vector2 UVSize
		{
			get { return m_baseDefinition.UVSize; }
			set
			{
				if (m_baseDefinition.UVSize == value) return;
				m_baseDefinition.UVSize = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_TransparentMaterial definition)
		{
			return definition.Name;
		}

		#endregion
	}

	public class TransparentMaterialsDefinitionManager : SerializableDefinitionsManager<MyObjectBuilder_TransparentMaterial, TransparentMaterialsDefinition>
	{ }
}
