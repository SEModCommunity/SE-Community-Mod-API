using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.API.Definitions;
using SEModAPI.API.Internal;
using SEModAPI.API.SaveData;

using VRageMath;

namespace SEModAPI.API.Internal
{
	public class CubeGridInternalWrapper : BaseInternalWrapper
	{
		#region "Attributes"

		protected new static CubeGridInternalWrapper m_instance;

		private static Assembly m_assembly;

		private static CubeGrid m_cubeGridToUpdate;

		private static string CubeGridClass = "5BCAC68007431E61367F5B2CF24E2D6F.98262C3F38A1199E47F2B9338045794C";

		private static Type m_baseCubeGridType;

		#endregion

		#region "Constructors and Initializers"

		protected CubeGridInternalWrapper(string basePath)
			: base(basePath)
		{
			m_instance = this;

			m_assembly = Assembly.UnsafeLoadFrom("Sandbox.Game.dll");

			m_baseCubeGridType = m_assembly.GetType(CubeGridClass);
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

		#region "Methods"

		public static bool UpdateEntityIsStatic(Object gameEntity, bool isStatic)
		{
			try
			{
				throw new Exception("Not yet implemented");

				//TODO - Need to find the field ID for this
				FieldInfo isStaticField = GetEntityField(gameEntity, "");
				isStaticField.SetValue(gameEntity, isStatic);

				return true;
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static bool AddCubeGrid(CubeGrid cubeGrid)
		{
			try
			{
				m_cubeGridToUpdate = cubeGrid;

				Action action = InternalAddCubeGrid;
				SandboxGameAssemblyWrapper.EnqueueMainGameAction(action);

				return true;
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static void InternalAddCubeGrid()
		{
			try
			{
				if (m_cubeGridToUpdate == null)
					return;

				if (m_isDebugging)
					Console.WriteLine("CubeGrid '" + m_cubeGridToUpdate.Name + "' is being added ...");

				m_assembly = Assembly.UnsafeLoadFrom("Sandbox.Game.dll");
				m_baseCubeGridType = m_assembly.GetType(CubeGridClass);

				//Create a blank instance of the base type
				m_cubeGridToUpdate.BackingObject = Activator.CreateInstance(m_baseCubeGridType);

				//Invoke 'Init' using the sub object of the grid which is the MyObjectBuilder_CubeGrid type
				InvokeEntityMethod(m_cubeGridToUpdate.BackingObject, "Init", new object[] { m_cubeGridToUpdate.BaseDefinition });

				//TODO - Need to do more to load it up into the main list

				m_cubeGridToUpdate = null;
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
			}
		}

		#endregion
	}
}
