using System.IO;
using System.Windows.Forms;
using System.Xml;
using CCKTiktok.Helper;

namespace CCKTiktok.Bussiness
{
	public class CCKXml
	{
		private XmlDocument doc = new XmlDocument();

		public XmlDocument Load(string src)
		{
			doc.Load(src);
			return doc;
		}

		public string GetPageSouce(string deviceId)
		{
			string text = ADBHelperCCK.ExecuteCMD(deviceId, " shell uiautomator dump /sdcard/window_dump.xml");
			if (text.Contains("dumped to"))
			{
				text = ADBHelperCCK.ExecuteCMD(deviceId, $" pull /sdcard/window_dump.xml \"{Application.StartupPath}\\window_dump.xml\"");
			}
			if (text.Contains("bytes"))
			{
				if (File.Exists(Application.StartupPath + "\\window_dump.xml"))
				{
					string result = Utils.ReadTextFile(Application.StartupPath + "\\window_dump.xml");
					File.Delete(Application.StartupPath + "\\window_dump.xml");
					return result;
				}
				return "";
			}
			return "";
		}

		public XmlNode FindElement(string xpath)
		{
			return doc.SelectSingleNode(xpath);
		}
	}
}
