using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEModAPI.API.Internal
{
	public class CubeGridInternalWrapper : BaseInternalWrapper
	{
		#region "Attributes"

		protected new static CubeGridInternalWrapper m_instance;

		#endregion

		#region "Constructors and Initializers"

		protected CubeGridInternalWrapper(string basePath)
			: base(basePath)
		{
			m_instance = this;
		}

		new public static CubeGridInternalWrapper GetInstance(string basePath = "")
		{
			if (m_instance == null)
			{
				m_instance = new CubeGridInternalWrapper(basePath);
			}
			return (CubeGridInternalWrapper)m_instance;
		}

		#endregion
	}
}
