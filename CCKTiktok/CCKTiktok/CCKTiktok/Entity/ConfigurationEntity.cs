using System;
using System.Collections.Generic;

namespace CCKTiktok.Entity
{
	internal class ConfigurationEntity
	{
		public class ControlTask
		{
			public string ControlName { get; set; }

			public string DisplayName { get; set; }

			public ControlTask()
			{
				ControlName = "";
				DisplayName = "";
			}
		}

		public string Id { get; set; }

		public string Name { get; set; }

		public List<ControlTask> data { get; set; }

		public ConfigurationEntity()
		{
			Id = Guid.NewGuid().ToString("N");
			Name = "";
			data = new List<ControlTask>();
		}
	}
}
