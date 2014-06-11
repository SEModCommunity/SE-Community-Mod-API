using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;

namespace SEModAPI.API.Definitions
{
	public class VoxelMaterialsDefinition : OverLayerDefinition<MyObjectBuilder_VoxelMaterialDefinition>
	{
		#region "Constructors and Initializers"

		public VoxelMaterialsDefinition(MyObjectBuilder_VoxelMaterialDefinition definition): base(definition)
		{}

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

		public string AssetName
		{
			get { return m_baseDefinition.AssetName; }
			set
			{
				if (m_baseDefinition.AssetName == value) return;
				m_baseDefinition.AssetName = value;
				Changed = true;
			}
		}

		public string MinedOre
		{
			get { return m_baseDefinition.MinedOre; }
			set
			{
				if (m_baseDefinition.MinedOre == value) return;
				m_baseDefinition.MinedOre = value;
				Changed = true;
			}
		}

		public float MinedOreRatio
		{
			get { return m_baseDefinition.MinedOreRatio; }
			set
			{
				if (m_baseDefinition.MinedOreRatio == value) return;
				m_baseDefinition.MinedOreRatio = value;
				Changed = true;
			}
		}

		public float DamageRatio
		{
			get { return m_baseDefinition.DamageRatio; }
			set
			{
				if (m_baseDefinition.DamageRatio == value) return;
				m_baseDefinition.DamageRatio = value;
				Changed = true;
			}
		}

		public float SpecularPower
		{
			get { return m_baseDefinition.SpecularPower; }
			set
			{
				if (m_baseDefinition.SpecularPower == value) return;
				m_baseDefinition.SpecularPower = value;
				Changed = true;
			}
		}

		public float SpecularShininess
		{
			get { return m_baseDefinition.SpecularShininess; }
			set
			{
				if (m_baseDefinition.SpecularShininess == value) return;
				m_baseDefinition.SpecularShininess = value;
				Changed = true;
			}
		}

		public bool CanBeHarvested
		{
			get { return m_baseDefinition.CanBeHarvested; }
			set
			{
				if (m_baseDefinition.CanBeHarvested == value) return;
				m_baseDefinition.CanBeHarvested = value;
				Changed = true;
			}
		}

		public bool IsRare
		{
			get { return m_baseDefinition.IsRare; }
			set
			{
				if (m_baseDefinition.IsRare == value) return;
				m_baseDefinition.IsRare = value;
				Changed = true;
			}
		}

		public bool UseTwoTextures
		{
			get { return m_baseDefinition.UseTwoTextures; }
			set
			{
				if (m_baseDefinition.UseTwoTextures == value) return;
				m_baseDefinition.UseTwoTextures = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_VoxelMaterialDefinition definition)
		{
			return definition.Name;
		}

		#endregion
	}

	public class VoxelMaterialDefinitionsManager : OverLayerDefinitionsManager<MyObjectBuilder_VoxelMaterialDefinition, VoxelMaterialsDefinition>
	{
		#region "Constructors and Initializers"

		public VoxelMaterialDefinitionsManager(MyObjectBuilder_VoxelMaterialDefinition[] definitions)
			: base(definitions)
		{
		}

		#endregion

		#region "Methods"

		protected override VoxelMaterialsDefinition CreateOverLayerSubTypeInstance(MyObjectBuilder_VoxelMaterialDefinition definition)
		{
			return new VoxelMaterialsDefinition(definition);
		}

		protected override MyObjectBuilder_VoxelMaterialDefinition GetBaseTypeOf(VoxelMaterialsDefinition overLayer)
		{
			return overLayer.BaseDefinition;
		}

		protected override bool GetChangedState(VoxelMaterialsDefinition overLayer)
		{
			return overLayer.Changed;
		}

		public override void Save()
		{
			if (!this.Changed) return;

			m_configSerializer.VoxelMaterialDefinitions = this.ExtractBaseDefinitions().ToArray();
			m_configSerializer.SaveVoxelMaterialsContentFile();
		}

		#endregion
	}
}
