using System.Collections.Generic;

namespace CCKTiktok.Bussiness
{
	public class MemuHelper
	{
		private static string MENU_PATH = "";

		public void ChangeInfo(string name)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("linenum", "");
			dictionary.Add("imei", "");
			dictionary.Add("imsi", "");
			dictionary.Add("simserial", "");
			dictionary.Add("microvirt_vm_brand", "");
			dictionary.Add("microvirt_vm_manufacturer", "");
			dictionary.Add("microvirt_vm_model", "");
			dictionary.Add("longitude", "");
			dictionary.Add("latitude", "");
			dictionary.Add("macaddress", "");
			dictionary.Add("ssid", "");
		}
	}
}
