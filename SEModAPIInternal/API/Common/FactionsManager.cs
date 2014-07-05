using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPIInternal.API.Common
{
	public class Faction
	{
		#region "Attributes"

		private MyObjectBuilder_Faction m_faction;

		#endregion

		#region "Constructors and Initializers"

		public Faction(MyObjectBuilder_Faction faction)
		{
			m_faction = faction;
		}

		#endregion

		#region "Properties"

		[Browsable(false)]
		public List<MyObjectBuilder_FactionMember> Members
		{
			get { return m_faction.Members; }
		}

		[Browsable(false)]
		public List<MyObjectBuilder_FactionMember> JoinRequests
		{
			get { return m_faction.JoinRequests; }
		}

		public long Id
		{
			get { return m_faction.FactionId; }
		}

		public string Name
		{
			get { return m_faction.Name; }
		}

		public string Description
		{
			get { return m_faction.Description; }
		}

		public string PrivateInfo
		{
			get { return m_faction.PrivateInfo; }
		}

		public string Tag
		{
			get { return m_faction.Tag; }
		}

		#endregion
	}

	public class FactionsManager
	{
		#region "Attributes"

		private static FactionsManager m_instance;
		private bool m_isInitialized;
		private MyObjectBuilder_Checkpoint m_checkpoint;
		private Dictionary<long, Faction> m_factions;

		#endregion

		#region "Constructors and Initializers"

		protected FactionsManager()
		{
			m_isInitialized = false;
			m_factions = new Dictionary<long, Faction>();

			m_instance = this;

			Console.WriteLine("Finished loading FactionsManager");
		}

		#endregion

		#region "Properties"

		public static FactionsManager Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new FactionsManager();

				return m_instance;
			}
		}

		public bool IsInitialized
		{
			get { return m_isInitialized; }
		}

		public List<Faction> Factions
		{
			get
			{
				List<Faction> factionList = new List<Faction>(m_factions.Values);
				return factionList;
			}
		}

		#endregion

		#region "Methods"

		public void Init(string worldName)
		{
			if (m_isInitialized)
				return;

			ulong worldId = 0;
			m_checkpoint = CheckpointManager.Instance.GetServerCheckpoint(worldName, out worldId);

			foreach (MyObjectBuilder_Faction faction in m_checkpoint.Factions.Factions)
			{
				Faction newFaction = new Faction(faction);
				m_factions.Add(newFaction.Id, newFaction);
			}

			m_isInitialized = true;
		}

		#endregion
	}
}
