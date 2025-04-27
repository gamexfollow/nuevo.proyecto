using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace CCKTiktok.Bussiness
{
	public static class List2Table
	{
		public static DataTable ToDataTable<T>(this IList<T> data)
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
			DataTable dataTable = new DataTable();
			for (int i = 0; i < properties.Count; i++)
			{
				PropertyDescriptor propertyDescriptor = properties[i];
				dataTable.Columns.Add(propertyDescriptor.Name, propertyDescriptor.PropertyType);
			}
			object[] array = new object[properties.Count];
			foreach (T datum in data)
			{
				for (int j = 0; j < array.Length; j++)
				{
					array[j] = properties[j].GetValue(datum);
				}
				dataTable.Rows.Add(array);
			}
			return dataTable;
		}
	}
}
