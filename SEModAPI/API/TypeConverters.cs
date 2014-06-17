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
	
	public class SerializableVector3ITypeConverter : TypeConverter
	{
		public SerializableVector3ITypeConverter()
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
			if (destinationType == typeof(SerializableVector3I))
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
					int x = Convert.ToInt32(sourceParts[0]);
					int y = Convert.ToInt32(sourceParts[1]);
					int z = Convert.ToInt32(sourceParts[2]);
					SerializableVector3I vector = new SerializableVector3I(x, y, z);
					return vector;
				}
			}

			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string) && value is SerializableVector3I)
			{
				SerializableVector3I vector = (SerializableVector3I)value;

				string result = vector.X + "," + vector.Y + "," + vector.Z;
				return result;
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}

	public class ItemSerializableDefinitionIdTypeConverter : TypeConverter
	{
		private static PhysicalItemDefinitionsManager m_physicalItemsManager;
		private static ComponentDefinitionsManager m_componentsManager;
		private static AmmoMagazinesDefinitionsManager m_ammoManager;

		private static List<SerializableDefinitionId> m_idList;

		public ItemSerializableDefinitionIdTypeConverter()
		{
			//Load up the static item managers
			if (m_physicalItemsManager == null)
			{
				m_physicalItemsManager = new PhysicalItemDefinitionsManager();
				m_physicalItemsManager.Load(PhysicalItemDefinitionsManager.GetContentDataFile("PhysicalItems.sbc"));
			}
			if (m_componentsManager == null)
			{
				m_componentsManager = new ComponentDefinitionsManager();
				m_componentsManager.Load(ComponentDefinitionsManager.GetContentDataFile("Components.sbc"));
			}
			if (m_ammoManager == null)
			{
				m_ammoManager = new AmmoMagazinesDefinitionsManager();
				m_ammoManager.Load(AmmoMagazinesDefinitionsManager.GetContentDataFile("AmmoMagazines.sbc"));
			}

			//Populate the static list with the ids from the items
			if (m_idList == null)
			{
				m_idList = new List<SerializableDefinitionId>();
				foreach (var def in m_physicalItemsManager.Definitions)
				{
					m_idList.Add(def.Id);
				}
				foreach (var def in m_componentsManager.Definitions)
				{
					m_idList.Add(def.Id);
				}
				foreach (var def in m_ammoManager.Definitions)
				{
					m_idList.Add(def.Id);
				}
			}
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(m_idList);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;
			else
				return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value.GetType() == typeof(string))
			{
				string source = (string)value;
				source = source.Replace(" ", "");
				string[] sourceParts = source.Split('/');
				if (sourceParts.Length == 2)
				{
					MyObjectBuilderTypeEnum typeId;
					if (Enum.TryParse<MyObjectBuilderTypeEnum>(sourceParts[0], out typeId))
					{
						SerializableDefinitionId result = new SerializableDefinitionId(typeId, sourceParts[1]);
						return result;
					}
				}
			}

			return base.ConvertFrom(context, culture, value);
		}
	}
	
	public class ObjectSerializableDefinitionIdTypeConverter : TypeConverter
	{
		private static CubeBlockDefinitionsManager m_blocksManager;
		private static List<SerializableDefinitionId> m_idList;

		public ObjectSerializableDefinitionIdTypeConverter()
		{
			//Load up the static item managers
			if (m_blocksManager == null)
			{
				m_blocksManager = new CubeBlockDefinitionsManager();
				m_blocksManager.Load(CubeBlockDefinitionsManager.GetContentDataFile("CubeBlocks.sbc"));
			}

			//Populate the static list with the ids from the items
			if (m_idList == null)
			{
				m_idList = new List<SerializableDefinitionId>();
				foreach (var def in m_blocksManager.Definitions)
				{
					m_idList.Add(def.Id);
				}

				m_idList.Add(new SerializableDefinitionId(MyObjectBuilderTypeEnum.CubeGrid, ""));
				m_idList.Add(new SerializableDefinitionId(MyObjectBuilderTypeEnum.VoxelMap, ""));
				m_idList.Add(new SerializableDefinitionId(MyObjectBuilderTypeEnum.FloatingObject, ""));
				m_idList.Add(new SerializableDefinitionId(MyObjectBuilderTypeEnum.Meteor, ""));
			}
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(m_idList);
		}
		
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;
			else
				return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value.GetType() == typeof(string))
			{
				string source = (string)value;
				source = source.Replace(" ", "");
				string[] sourceParts = source.Split('/');
				if (sourceParts.Length == 2)
				{
					MyObjectBuilderTypeEnum typeId;
					if (Enum.TryParse<MyObjectBuilderTypeEnum>(sourceParts[0], out typeId))
					{
						SerializableDefinitionId result = new SerializableDefinitionId(typeId, sourceParts[1]);
						return result;
					}
				}
			}

			return base.ConvertFrom(context, culture, value);
		}
	}
}
