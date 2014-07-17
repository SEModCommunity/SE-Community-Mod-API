using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;

using SEModAPIInternal.API.Entity;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Common
{
	public class Faction
	{
		#region "Attributes"

		private MyObjectBuilder_Faction m_faction;
		private Dictionary<long, FactionMember> m_members;
		private Dictionary<long, FactionMember> m_joinRequests;

		public static string FactionNamespace = "AAC05F537A6F0F6775339593FBDFC564";
		public static string FactionClass = "1ECF0BCA9FD77A768222E28B816597E6";

		#endregion

		#region "Constructors and Initializers"

		public Faction(MyObjectBuilder_Faction faction)
		{
			m_faction = faction;

			m_members = new Dictionary<long, FactionMember>();
			foreach (var member in m_faction.Members)
			{
				m_members.Add(member.PlayerId, new FactionMember(member));
			}

			m_joinRequests = new Dictionary<long, FactionMember>();
			foreach (var member in m_faction.JoinRequests)
			{
				m_joinRequests.Add(member.PlayerId, new FactionMember(member));
			}
		}

		#endregion

		#region "Properties"

		public Object BackingObject
		{
			get
			{
				return FactionsManager.Instance.InternalGetFactionById(m_faction.FactionId);
			}
		}

		public MyObjectBuilder_Faction BaseEntity
		{
			get { return m_faction; }
		}

		[Browsable(false)]
		public List<FactionMember> Members
		{
			get
			{
				RefreshFactionMembers();

				List<FactionMember> memberList = new List<FactionMember>(m_members.Values);
				return memberList;
			}
		}

		[Browsable(false)]
		public List<FactionMember> JoinRequests
		{
			get
			{
				RefreshFactionJoinRequests();

				List<FactionMember> memberList = new List<FactionMember>(m_joinRequests.Values);
				return memberList;
			}
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

		#region "Methods"

		public MyObjectBuilder_Faction GetSubTypeEntity()
		{
			return m_faction;
		}

		protected void RefreshFactionMembers()
		{
			List<MyObjectBuilder_FactionMember> memberList = GetSubTypeEntity().Members;

			//Cleanup missing members
			List<FactionMember> membersToRemove = new List<FactionMember>();
			foreach (FactionMember member in m_members.Values)
			{
				if (memberList.Contains(member.BaseEntity))
					continue;

				membersToRemove.Add(member);
			}
			foreach (FactionMember member in membersToRemove)
			{
				m_members.Remove(member.PlayerId);
			}

			//Add new members
			foreach (MyObjectBuilder_FactionMember member in memberList)
			{
				if (m_members.ContainsKey(member.PlayerId))
					continue;

				FactionMember newMember = new FactionMember(member);
				m_members.Add(newMember.PlayerId, newMember);
			}
		}

		protected void RefreshFactionJoinRequests()
		{
			List<MyObjectBuilder_FactionMember> memberList = GetSubTypeEntity().JoinRequests;

			//Cleanup missing members
			List<FactionMember> membersToRemove = new List<FactionMember>();
			foreach (FactionMember member in m_joinRequests.Values)
			{
				if (memberList.Contains(member.BaseEntity))
					continue;

				membersToRemove.Add(member);
			}
			foreach (FactionMember member in membersToRemove)
			{
				m_joinRequests.Remove(member.PlayerId);
			}

			//Add new members
			foreach (MyObjectBuilder_FactionMember member in memberList)
			{
				if (m_joinRequests.ContainsKey(member.PlayerId))
					continue;

				FactionMember newMember = new FactionMember(member);
				m_joinRequests.Add(newMember.PlayerId, newMember);
			}
		}

		#endregion
	}

	public class FactionMember
	{
		#region "Attributes"

		private MyObjectBuilder_FactionMember m_member;

		public static string FactionMemberNamespace = "AAC05F537A6F0F6775339593FBDFC564";
		public static string FactionMemberClass = "32F8947D11EDAF4D079FD54C2397A951";

		#endregion

		#region "Constructors and Initializers"

		public FactionMember(MyObjectBuilder_FactionMember definition)
		{
			m_member = definition;
		}

		#endregion

		#region "Properties"

		public MyObjectBuilder_FactionMember BaseEntity
		{
			get { return m_member; }
		}

		public long PlayerId
		{
			get { return m_member.PlayerId; }
		}

		public bool IsFounder
		{
			get { return m_member.IsFounder; }
		}

		public bool IsLeader
		{
			get { return m_member.IsLeader; }
		}

		#endregion
	}

	public class FactionsManager
	{
		#region "Attributes"

		private static FactionsManager m_instance;
		private MyObjectBuilder_FactionCollection m_factionCollection;
		private Dictionary<long, Faction> m_factions;

		protected long m_factionToRemove;

		public static string FactionManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string FactionManagerClass = "17E6A2D4B9414B1D3DDA551E10938751";
		public static string FactionManagerGetFactionCollectionMethod = "C93E560F96FE606D0BE190EDE1AA5005";
		public static string FactionManagerGetFactionByIdMethod = "E82E38AFFB42537CD735F119BC0EDAD7";
		public static string FactionManagerRemoveFactionByIdMethod = "A8A9206AB10ECC19F73F4FF98E874379";

		#endregion

		#region "Constructors and Initializers"

		protected FactionsManager()
		{
			m_instance = this;
			m_factions = new Dictionary<long, Faction>();

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

		public Object BackingObject
		{
			get
			{
				return WorldManager.Instance.InternalGetFactionManager();
			}
		}

		public List<Faction> Factions
		{
			get
			{
				RefreshFactions();

				List<Faction> factionList = new List<Faction>(m_factions.Values);
				return factionList;
			}
		}

		#endregion

		#region "Methods"

		public MyObjectBuilder_FactionCollection GetSubTypeEntity()
		{
			m_factionCollection = (MyObjectBuilder_FactionCollection)BaseObject.InvokeEntityMethod(BackingObject, FactionManagerGetFactionCollectionMethod);

			return m_factionCollection;
		}

		protected void RefreshFactions()
		{
			List<MyObjectBuilder_Faction> factionList = GetSubTypeEntity().Factions;

			//Cleanup missing factions
			List<Faction> factionsToRemove = new List<Faction>();
			foreach (Faction faction in m_factions.Values)
			{
				if (factionList.Contains(faction.BaseEntity))
					continue;

				factionsToRemove.Add(faction);
			}
			foreach (Faction faction in factionsToRemove)
			{
				m_factions.Remove(faction.Id);
			}

			//Add new factions
			foreach (MyObjectBuilder_Faction faction in factionList)
			{
				if (m_factions.ContainsKey(faction.FactionId))
					continue;

				Faction newFaction = new Faction(faction);
				m_factions.Add(newFaction.Id, newFaction);
			}
		}

		public void RemoveFaction(long id)
		{
			m_factionToRemove = id;

			Action action = InternalRemoveFaction;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		#region "Internal"

		internal Object InternalGetFactionById(long id)
		{
			Object internalFaction = BaseObject.InvokeEntityMethod(BackingObject, FactionManagerGetFactionByIdMethod, new object[] { id });

			return internalFaction;
		}

		protected void InternalRemoveFaction()
		{
			if (m_factionToRemove == 0)
				return;

			BaseObject.InvokeEntityMethod(BackingObject, FactionManagerRemoveFactionByIdMethod, new object[] { m_factionToRemove });

			m_factionToRemove = 0;
		}

		#endregion

		#endregion
	}
}
