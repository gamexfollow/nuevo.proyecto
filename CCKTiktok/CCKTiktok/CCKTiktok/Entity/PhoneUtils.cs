using System;
using System.Collections.Generic;
using System.Data;
using CCKTiktok.Bussiness;
using CCKTiktok.DAL;

namespace CCKTiktok.Entity
{
	public class PhoneUtils
	{
		private static DeviceEntity Mapping(DataRow row)
		{
			if (row == null)
			{
				return new DeviceEntity();
			}
			return new DeviceEntity
			{
				DeviceId = row["deviceid"].ToString(),
				Name = row["name"].ToString(),
				Port = Convert.ToInt32(row["port"].ToString()),
				SystemPort = Convert.ToInt32(row["systemport"].ToString())
			};
		}

		public static DeviceEntity SelectOne(string deviceId)
		{
			DataTable dataTable = new SQLiteUtils().ExecuteQuery($"Select * from tblPhones where deviceid='{deviceId}'");
			return (dataTable == null || dataTable.Rows.Count <= 0) ? null : Mapping(dataTable.Rows[0]);
		}

		public static void UpdateName(string deviceId, string Name)
		{
			new SQLiteUtils().ExecuteQuery($"Update tblPhones set Name='{Name}' where deviceid='{deviceId}'");
		}

		public static List<DeviceEntity> GetAll()
		{
			List<DeviceEntity> list = new List<DeviceEntity>();
			DataTable dataTable = new SQLiteUtils().ExecuteQuery("Select * from tblPhones");
			int num = 1;
			foreach (DataRow row in dataTable.Rows)
			{
				DeviceEntity deviceEntity = Mapping(row);
				deviceEntity.Id = num++;
				list.Add(deviceEntity);
			}
			return list;
		}
	}
}
