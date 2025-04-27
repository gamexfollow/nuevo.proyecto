namespace CCKTiktok.Bussiness
{
	public class Status
	{
		private int _Id;

		private string _Name;

		public int Id
		{
			get
			{
				return _Id;
			}
			set
			{
				_Id = value;
			}
		}

		public string Name
		{
			get
			{
				return _Name;
			}
			set
			{
				_Name = value;
			}
		}

		public Status(int id, string name)
		{
			_Id = id;
			_Name = name;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
