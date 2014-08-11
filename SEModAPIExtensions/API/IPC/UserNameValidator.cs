using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Selectors;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;

namespace SEModAPIExtensions.API.IPC
{
	public class UserNameValidator : UserNamePasswordValidator
	{
		public override void Validate(string userName, string password)
		{
			if (null == userName || null == password)
			{
				throw new ArgumentNullException();
			}

			//TODO - Create a dynamic password system or abandon this and just use windows username+password instead
			if (!(userName == "test1" && password == "1tset"))
			{
				throw new FaultException("Unknown Username or Incorrect Password");
			}
		}
	}
}
