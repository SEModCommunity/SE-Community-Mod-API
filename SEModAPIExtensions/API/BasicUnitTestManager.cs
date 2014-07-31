using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Server;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Entity.Sector;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;
using SEModAPIInternal.API.Utility;
using SEModAPIInternal.Support;

namespace SEModAPIExtensions.API
{
	public class BasicUnitTestManager
	{
		private static BasicUnitTestManager m_instance;

		protected BasicUnitTestManager()
		{
			m_instance = this;
		}

		public static BasicUnitTestManager Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new BasicUnitTestManager();

				return m_instance;
			}
		}

		public bool Run()
		{
			bool result = true;
			result &= RunReflectionUnitTests();

			return result;
		}

		protected bool RunReflectionUnitTests()
		{
			bool result = true;

			if (!BaseObject.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("BaseObject reflection validation failed!");
			}

			if (!BaseEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("BaseEntity reflection validation failed!");
			}

			if (!BaseEntityNetworkManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("BaseEntityNetworkManager reflection validation failed!");
			}

			if (!CubeGridEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("CubeGridEntity reflection validation failed!");
			}

			if (!CubeGridNetworkManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("CubeGridNetworkManager reflection validation failed!");
			}

			if (!CubeBlockEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("CubeBlockEntity reflection validation failed!");
			}

			if (!TerminalBlockEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("TerminalBlockEntity reflection validation failed!");
			}

			if (!FunctionalBlockEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("FunctionalBlockEntity reflection validation failed!");
			}

			if (!SectorObjectManager.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("SectorObjectManager reflection validation failed!");
			}

			if (!CharacterEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("CharacterEntity reflection validation failed!");
			}

			if (!InventoryEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("InventoryEntity reflection validation failed!");
			}

			if (!InventoryItemEntity.ReflectionUnitTest())
			{
				result = false;
				Console.WriteLine("InventoryItemEntity reflection validation failed!");
			}

			return result;
		}
	}
}
