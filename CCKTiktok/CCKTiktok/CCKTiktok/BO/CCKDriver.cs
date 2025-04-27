using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using CCKTiktok.Helper;

namespace CCKTiktok.BO
{
	public class CCKDriver : IDisposable
	{
		private XmlDocument xmlDocument = new XmlDocument();

		private string DeviceId { get; set; }

		public string PageSource => DumpXMLSource(DeviceId);

		public CCKNode FindElement(string xpath)
		{
			List<CCKNode> list = FindElements(xpath);
			if (list != null && list.Count > 0)
			{
				return list[0];
			}
			return null;
		}

		public List<CCKNode> FindElements(string xpath)
		{
			string xml = DumpXMLSource(DeviceId);
			xmlDocument.LoadXml(xml);
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes(xpath);
			if (xmlNodeList != null)
			{
				List<CCKNode> list = new List<CCKNode>();
				foreach (XmlNode item2 in xmlNodeList)
				{
					CCKNode item = new CCKNode(DeviceId, item2);
					list.Add(item);
				}
				return list;
			}
			return null;
		}

		public CCKNode FindElement(string xpath, string xml)
		{
			xmlDocument.LoadXml(xml);
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes(xpath);
			if (xmlNodeList == null)
			{
				return null;
			}
			new List<CCKNode>();
			IEnumerator enumerator = xmlNodeList.GetEnumerator();
			try
			{
				if (enumerator.MoveNext())
				{
					XmlNode node = (XmlNode)enumerator.Current;
					return new CCKNode(DeviceId, node);
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return null;
		}

		public List<CCKNode> FindElements(string xpath, string xml)
		{
			xmlDocument.LoadXml(xml);
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes(xpath);
			if (xmlNodeList == null)
			{
				return null;
			}
			List<CCKNode> list = new List<CCKNode>();
			foreach (XmlNode item2 in xmlNodeList)
			{
				CCKNode item = new CCKNode(DeviceId, item2);
				list.Add(item);
			}
			return list;
		}

		public CCKDriver(string DeviceId)
		{
			this.DeviceId = DeviceId;
		}

		private string DumpXMLSource(string deviceID)
		{
			int num = 5;
			string text;
			string text2;
			while (true)
			{
				text = Application.StartupPath + "\\cck" + ADBHelperCCK.NormalizeDeviceName(deviceID) + ".xml";
				if (File.Exists(text))
				{
					File.Delete(text);
				}
				text2 = ExecuteCMD(deviceID, " shell rm -r /sdcard/uidump.xml");
				text2 = ExecuteCMD(deviceID, " shell uiautomator dump /sdcard/uidump.xml");
				text2 = ExecuteCMD(deviceID, " pull /sdcard/uidump.xml \"" + text + "\"");
				if (!text2.Contains("failed to stat remote object"))
				{
					break;
				}
				ADBHelperCCK.RunCommand("adb kill-server");
				ADBHelperCCK.RunCommand("adb start-server");
				if (num-- != 0)
				{
					Thread.Sleep(1000);
					continue;
				}
				return "";
			}
			text2 = ExecuteCMD(deviceID, " shell rm -r /sdcard/uidump.xml");
			MemoryStream memoryStream = new MemoryStream();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.Unicode);
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				string xml = File.ReadAllText(text);
				xmlDocument.LoadXml(xml);
				if (File.Exists(text))
				{
					File.Delete(text);
				}
			}
			catch (Exception)
			{
				return "";
			}
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlDocument.WriteContentTo(xmlTextWriter);
			xmlTextWriter.Flush();
			memoryStream.Flush();
			memoryStream.Position = 0L;
			StreamReader streamReader = new StreamReader(memoryStream);
			string result = streamReader.ReadToEnd();
			memoryStream.Close();
			xmlTextWriter.Close();
			return result;
		}

		public static string ExecuteCMD(string index, string cmd)
		{
			string result = "";
			try
			{
				string startupPath = Application.StartupPath;
				string text = "";
				try
				{
					Process process = new Process();
					cmd = string.Concat(new string[4] { " -s ", index, " ", cmd });
					process.StartInfo = new ProcessStartInfo
					{
						FileName = startupPath + "\\adb.exe",
						Arguments = cmd,
						UseShellExecute = false,
						WindowStyle = ProcessWindowStyle.Hidden,
						CreateNoWindow = true,
						RedirectStandardError = true,
						RedirectStandardOutput = true
					};
					process.Start();
					StreamReader standardOutput = process.StandardOutput;
					text = standardOutput.ReadToEnd();
					process.WaitForExit();
				}
				catch (Exception)
				{
				}
				result = text;
			}
			catch (Exception)
			{
			}
			return result;
		}

		internal CCKNode FindAndWait(string p)
		{
			CCKNode cCKNode = new CCKNode();
			for (int i = 0; i < 10; i++)
			{
				cCKNode = FindElement(p);
				if (cCKNode == null)
				{
					Thread.Sleep(2000);
					continue;
				}
				return cCKNode;
			}
			return null;
		}

		internal List<CCKNode> FindAndWaitElements(string p)
		{
			List<CCKNode> list = new List<CCKNode>();
			int num = 0;
			while (true)
			{
				if (num < 10)
				{
					list = FindElements(p);
					if (list != null)
					{
						break;
					}
					Thread.Sleep(2000);
					num++;
					continue;
				}
				return null;
			}
			return list;
		}

		internal void Dispose()
		{
		}

		internal void Quit()
		{
		}

		void IDisposable.Dispose()
		{
		}
	}
}
