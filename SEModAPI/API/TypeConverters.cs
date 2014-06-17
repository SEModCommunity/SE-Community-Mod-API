using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

using Sandbox.Common.ObjectBuilders.VRageData;

using VRageMath;

namespace SEModAPI.API
{
	public class Vector3TypeConverter : TypeConverter
	{
		public Vector3TypeConverter()
		{
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;
			else
				return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(Vector3))
				return true;
			else
				return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value.GetType() == typeof(string))
			{
				string source = (string)value;
				source = source.Replace(" ", "");
				string[] sourceParts = source.Split(',');
				if (sourceParts.Length == 3)
				{
					float x = Convert.ToSingle(sourceParts[0]);
					float y = Convert.ToSingle(sourceParts[1]);
					float z = Convert.ToSingle(sourceParts[2]);
					Vector3 vector = new Vector3(x, y, z);
					return vector;
				}
			}

			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string) && value is Vector3)
			{
				Vector3 vector = (Vector3) value;

				string result = vector.X + "," + vector.Y + "," + vector.Z;
				return result;
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}

	public class SerializableVector3TypeConverter : TypeConverter
	{
		public SerializableVector3TypeConverter()
		{
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;
			else
				return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(SerializableVector3))
				return true;
			else
				return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value.GetType() == typeof(string))
			{
				string source = (string)value;
				source = source.Replace(" ", "");
				string[] sourceParts = source.Split(',');
				if (sourceParts.Length == 3)
				{
					float x = Convert.ToSingle(sourceParts[0]);
					float y = Convert.ToSingle(sourceParts[1]);
					float z = Convert.ToSingle(sourceParts[2]);
					SerializableVector3 vector = new SerializableVector3(x, y, z);
					return vector;
				}
			}

			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string) && value is SerializableVector3)
			{
				SerializableVector3 vector = (SerializableVector3)value;

				string result = vector.X + "," + vector.Y + "," + vector.Z;
				return result;
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
