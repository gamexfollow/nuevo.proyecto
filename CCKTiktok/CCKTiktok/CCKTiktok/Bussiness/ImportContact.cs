namespace CCKTiktok.Bussiness
{
	internal class ImportContact
	{
		public bool Status { get; set; }

		public string FileUrl { get; set; }

		public int Number { get; set; }

		public ImportContact()
		{
			Status = false;
			FileUrl = "";
			Number = 0;
		}
	}
}
