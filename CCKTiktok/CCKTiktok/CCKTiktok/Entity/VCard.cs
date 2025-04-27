using System;
using System.Text;

namespace CCKTiktok.Entity
{
	public class VCard
	{
		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Organization { get; set; }

		public string JobTitle { get; set; }

		public string StreetAddress { get; set; }

		public string Zip { get; set; }

		public string City { get; set; }

		public string CountryName { get; set; }

		public string Phone { get; set; }

		public string Mobile { get; set; }

		public string Email { get; set; }

		public string HomePage { get; set; }

		public byte[] Image { get; set; }

		public string GetFullName()
		{
			return FirstName + LastName;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("BEGIN:VCARD");
			stringBuilder.AppendLine("VERSION:2.1");
			stringBuilder.Append("N:").Append(LastName).Append(";")
				.AppendLine(FirstName);
			stringBuilder.Append("FN:").Append(FirstName).Append(" ")
				.AppendLine(LastName);
			stringBuilder.Append("ADR;HOME;PREF:;;").Append(StreetAddress).Append(";")
				.Append(City)
				.Append(";")
				.Append(Zip)
				.Append(";")
				.AppendLine(CountryName);
			stringBuilder.Append("ORG:").AppendLine(Organization);
			stringBuilder.Append("TITLE:").AppendLine(JobTitle);
			stringBuilder.Append("TEL;WORK;VOICE:").AppendLine(Phone);
			stringBuilder.Append("TEL;CELL;VOICE:").AppendLine(Mobile);
			stringBuilder.Append("URL:").AppendLine(HomePage);
			stringBuilder.Append("EMAIL;PREF;INTERNET:").AppendLine(Email);
			if (Image != null)
			{
				stringBuilder.AppendLine("PHOTO;ENCODING=BASE64;TYPE=JPEG:");
				stringBuilder.AppendLine(Convert.ToBase64String(Image));
				stringBuilder.AppendLine(string.Empty);
			}
			stringBuilder.AppendLine("END:VCARD");
			return stringBuilder.ToString();
		}
	}
}
