using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using VRageMath;

using SEModAPI.Support;
using SEModAPI.API.Definitions;
using SEModAPI.API.Definitions.CubeBlocks;

namespace SEModAPI.API
{

	/// <summary>
	/// Wrapper to help Vector3 work with PropertyGrid
	/// </summary>
	public struct Vector3Wrapper
	{
		private Vector3 vector;

		#region "Constructors and Initializers"
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="x">Vectorial X value</param>
		/// <param name="y">Vectorial Y value</param>
		/// <param name="z">Vectorial Z value</param>
		public Vector3Wrapper(float x, float y, float z)
		{
			vector.X = x;
			vector.Y = y;
			vector.Z = z;
		}

		/// <summary>
		/// Converts a SerializableVector3 into a PropertyGrid frendly Vector
		/// </summary>
		/// <param name="v">The instance of SerializableVector3 to be converted</param>
		public Vector3Wrapper(SerializableVector3 v)
		{
			vector = v;
		}

		/// <summary>
		/// Converts a SerializableVector3 into a PropertyGrid frendly Vector
		/// </summary>
		/// <param name="v">The instance of SerializableVector3 to be converted</param>
		public Vector3Wrapper(Vector3 v)
		{
			vector = v;
		}
		#endregion

		#region "Properties"
		public float X
		{
			get { return vector.X; }
			set { vector.X = value; }
		}

		public float Y
		{
			get { return vector.Y; }
			set { vector.Y = value; }
		}

		public float Z
		{
			get { return vector.Z; }
			set { vector.Z = value; }
		}
		#endregion

		#region "Cast operators"
		public static implicit operator Vector3Wrapper(SerializableVector3 v)
		{
			return new Vector3Wrapper(v);
		}

		public static implicit operator Vector3Wrapper(Vector3 v)
		{
			return new Vector3Wrapper(v);
		}

		public static implicit operator SerializableVector3(Vector3Wrapper v)
		{
			return new SerializableVector3(v.X, v.Y, v.Z);
		}

		public static implicit operator Vector3(Vector3Wrapper v)
		{
			return new Vector3(v.X, v.Y, v.Z);
		}
		#endregion

		public override string ToString()
		{
			return vector.ToString();
		}
	}

	public class Vector3TypeConverter : ExpandableObjectConverter
	{
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues)
		{
			return new Vector3Wrapper((float)propertyValues["X"], (float)propertyValues["Y"], (float)propertyValues["Z"]);
		}
	}

	public class Vector3ITypeConverter : ExpandableObjectConverter
	{
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues)
		{
			return new Vector3I((int)propertyValues["X"], (int)propertyValues["Y"], (int)propertyValues["Z"]);
		}
	}
}
