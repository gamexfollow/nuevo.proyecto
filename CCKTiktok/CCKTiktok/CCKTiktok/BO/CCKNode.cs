using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using CCKTiktok.Bussiness;
using CCKTiktok.Helper;

namespace CCKTiktok.BO
{
	public class CCKNode
	{
		public class CCKLocation
		{
			public int X { get; set; }

			public int Y { get; set; }

			public CCKLocation()
			{
				X = 0;
				Y = 0;
			}
		}

		public class CCKSize
		{
			public int Width { get; set; }

			public int Height { get; set; }

			public CCKSize()
			{
				Width = 0;
				Height = 0;
			}
		}

		public CCKLocation Location { get; set; }

		public CCKSize Size { get; set; }

		private string DeviceId { get; set; }

		private XmlNode node { get; set; }

		public string Text => node.Attributes["text"].Value.ToString();

		public string ContentDesc => (node.Attributes["content-desc"] != null) ? node.Attributes["content-desc"].Value.ToString() : "";

		public bool Displayed { get; set; }

		public bool Enabled { get; set; }

		public bool Checked
		{
			get
			{
				return Convert.ToBoolean(node.Attributes["checked"].Value);
			}
			set
			{
				node.Attributes["checked"].Value = value.ToString().ToLower();
			}
		}

		public CCKNode()
		{
		}

		public CCKNode(string DeviceId, XmlNode node)
		{
			this.DeviceId = DeviceId;
			this.node = node;
			string input = node.Attributes["bounds"].Value.ToString();
			Regex regex = new Regex("([0-9]+)");
			MatchCollection matchCollection = regex.Matches(input);
			int x = 0;
			int y = 0;
			if (matchCollection.Count == 4)
			{
				x = (Utils.Convert2Int(matchCollection[0].Value) + Utils.Convert2Int(matchCollection[2].Value)) / 2;
				y = (Utils.Convert2Int(matchCollection[1].Value) + Utils.Convert2Int(matchCollection[3].Value)) / 2;
			}
			Location = new CCKLocation
			{
				X = x,
				Y = y
			};
			Size = new CCKSize
			{
				Height = Utils.Convert2Int(matchCollection[3].Value) - Utils.Convert2Int(matchCollection[1].Value),
				Width = Utils.Convert2Int(matchCollection[2].Value) - Utils.Convert2Int(matchCollection[0].Value)
			};
		}

		public void Click()
		{
			string input = node.Attributes["bounds"].Value.ToString();
			Regex regex = new Regex("([0-9]+)");
			MatchCollection matchCollection = regex.Matches(input);
			int num = 0;
			int num2 = 0;
			if (matchCollection.Count == 4)
			{
				num = (Utils.Convert2Int(matchCollection[0].Value) + Utils.Convert2Int(matchCollection[2].Value)) / 2;
				num2 = (Utils.Convert2Int(matchCollection[1].Value) + Utils.Convert2Int(matchCollection[3].Value)) / 2;
			}
			ADBHelperCCK.ExecuteCMD(DeviceId, $"shell input touchscreen tap {num} {num2}");
		}

		internal void CCKSendKeys(string input)
		{
			ADBHelperCCK.ExecuteCMD(DeviceId, "shell ime set com.android.adbkeyboard/.AdbIME");
			Click();
			ADBHelperCCK.InputTextUnicode(DeviceId, input);
		}

		internal List<CCKNode> FindElements(string p)
		{
			try
			{
				XmlNodeList xmlNodeList = node.SelectNodes(p);
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
			}
			catch
			{
			}
			return null;
		}

		internal void Clear()
		{
			ADBHelperCCK.ExecuteCMD(DeviceId, "shell input keyevent KEYCODE_MOVE_END");
			string text = Text;
			if (text != "")
			{
				for (int i = 0; i < text.Length; i++)
				{
					ADBHelperCCK.ExecuteCMD(DeviceId, "shell input keyevent --longpress KEYCODE_DEL");
				}
			}
		}

		public string GetAttribute(string attribute)
		{
			return node.Attributes[attribute].Value.ToString();
		}
	}
}
