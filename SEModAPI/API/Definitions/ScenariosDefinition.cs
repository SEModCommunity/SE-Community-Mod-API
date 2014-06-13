using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;

namespace SEModAPI.API.Definitions
{
	public class ScenariosDefinition : OverLayerDefinition<MyObjectBuilder_ScenarioDefinition>
	{
		#region "Attributes"

		private AsteroidClustersConfig m_asteroidClustersConfig;

		#endregion

		#region "Constructors and Initializers"

		public ScenariosDefinition(MyObjectBuilder_ScenarioDefinition definition)
			: base(definition)
		{
			m_asteroidClustersConfig = new AsteroidClustersConfig(definition.AsteroidClusters);
		}

		#endregion

		#region "Properties"

		public string Id
		{
			get { return m_baseDefinition.Id.ToString(); }
		}

		public AsteroidClustersConfig AsteroidClusters
		{
			get { return m_asteroidClustersConfig; }
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_ScenarioDefinition definition)
		{
			return definition.Id.SubtypeId;
		}

		#endregion
	}

	public class AsteroidClustersConfig
	{
		#region "Attributes"

		MyObjectBuilder_ScenarioDefinition.AsteroidClustersSettings m_AsteroidClustersSetting;

		#endregion

		#region "Constructors and Initializers"

		public AsteroidClustersConfig(MyObjectBuilder_ScenarioDefinition.AsteroidClustersSettings asteroidClustersSettings)
		{
			m_AsteroidClustersSetting = asteroidClustersSettings;
		}

		#endregion

		#region "Properties"

		public bool Enabled
		{
			get { return m_AsteroidClustersSetting.Enabled; }
			set { m_AsteroidClustersSetting.Enabled = value; }
		}

		public float Offset
		{
			get { return m_AsteroidClustersSetting.Offset; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException();
				m_AsteroidClustersSetting.Offset = value;
			}
		}

		public bool CentralCluster
		{
			get { return m_AsteroidClustersSetting.CentralCluster; }
			set { m_AsteroidClustersSetting.CentralCluster = value; }
		}

		#endregion
	}

	public class ScenariosDefinitionsManager : SerializableDefinitionsManager<MyObjectBuilder_ScenarioDefinition, ScenariosDefinition>
	{
	}


	//public class PlayerStartingStates
	//{
	//	#region "Attributes"

	//	MyPlayerStartingState m_playerStartingState;

	//	#endregion

	//	#region "Constructors and Initializers"

	//	public PlayerStartingStates(MyPlayerStartingState definition)
	//	{
	//		m_playerStartingState = definition;
	//	}

	//	#endregion

	//	#region "Methods"

	//	#endregion
	//}


	//public class PlayerStartingStatesManager
	//{
	//	#region "Attributes"

	//	PlayerStartingState

	//	#endregion


	//	#region "Methods"


	//	#endregion

	//}
}
