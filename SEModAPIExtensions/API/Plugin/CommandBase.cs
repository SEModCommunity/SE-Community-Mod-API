using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace SEModAPIExtensions.API.Plugin
{
	public abstract class CommandBase : ICommand
	{
	        #region "Attributes"
	        
	        protected Object m_owner;
	        protected Object m_executor;
	        
		#endregion
		
		#region "Constructors and Initializers"
		
		public CommandBase(Object owner)
		{
			this.m_owner = owner;
			this.m_executor = owner;
		}
		
		#endregion
		
		#region "Properties"
		
		public Object Owner
		{
			get
			{
				return m_owner;
			}
		}
		
		public Object Executor
		{
			get
			{
				return m_executor;
			}
		}
		
		#endregion
		
		#region "Methods"
		
		public abstract bool execute(string cmd, string[] args);
		
		#endregion
	}
}
