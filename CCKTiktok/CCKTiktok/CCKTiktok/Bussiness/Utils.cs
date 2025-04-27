using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Xml;
using Aspose.Zip.UnRAR;
using CCKTiktok.DAL;
using CCKTiktok.Entity;
using CCKTiktok.Helper;
using CCKTiktokV32.Bussiness;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace CCKTiktok.Bussiness
{
	public class Utils
	{
		public static List<CCKLanguage> LanguageList = new List<CCKLanguage>();

		public static string CurrentLang = "EN";

		private static Random rnd = new Random();

		public static bool IsVietnamese = true;

		private List<string> ten2 = new List<string>();

		private static List<string> nameCollection = new List<string>();

		private static bool vietnameseOnly = File.Exists(CaChuaConstant.ADD_FRIEND_ALL) && Convert.ToBoolean(ReadTextFile(CaChuaConstant.ADD_FRIEND_ALL));

		private static bool IsLog = ConfigurationSettings.AppSettings["log"] == null || Convert.ToBoolean(ConfigurationSettings.AppSettings["log"]);

		private ArrayList hardDriveDetails = new ArrayList();

		private static List<string> lstIcon = new List<string>();

		private static bool isLoadIcon = false;

		public static object lockFileRead = new object();

		public static string ApiLocation = "http://cachuake.com";

		private static object lockFile = new object();

		public const string vietnamChar = "àáảãạâầấẩẫậăằắẳẵặèéẻẽẹêềếểễệđìíỉĩịòóỏõọôồốổỗộơờớởỡợùúủũụưừứửữựỳýỷỹỵÀÁẢÃẠÂẦẤẨẪẬĂẰẮẲẴẶÈÉẺẼẸÊỀẾỂỄỆĐÌÍỈĨỊÒÓỎÕỌÔỒỐỔỖỘƠỜỚỞỠỢÙÚỦŨỤƯỪỨỬỮỰỲÝỶỸỴÂĂĐÔƠƯÐ";

		public const string uniChars = "_0987654321abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZàáảãạâầấẩẫậăằắẳẵặèéẻẽẹêềếểễệđìíỉĩịòóỏõọôồốổỗộơờớởỡợùúủũụưừứửữựỳýỷỹỵÀÁẢÃẠÂẦẤẨẪẬĂẰẮẲẴẶÈÉẺẼẸÊỀẾỂỄỆĐÌÍỈĨỊÒÓỎÕỌÔỒỐỔỖỘƠỜỚỞỠỢÙÚỦŨỤƯỪỨỬỮỰỲÝỶỸỴÂĂĐÔƠƯ-Ð ";

		public const string KoDauChars = "_0987654321abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZaaaaaaaaaaaaaaaaaeeeeeeeeeeediiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAAEEEEEEEEEEEDIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYYAADOOU-D-";

		public static string validChar = "_abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		private static string validChar_space = "_abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ ";

		private CookieContainer container = new CookieContainer();

		public static void LogFunction(string function, string phonenumber, string appName = "Tiktok")
		{
			try
			{
				GetResponse("https://cck.vn/api/logfunction.aspx?f=" + function + "&p=phonenumber&app=" + appName);
			}
			catch
			{
			}
		}

		public static void ChangeLanguage(Control ctr, List<Type> ctrType)
		{
			foreach (Type item in ctrType)
			{
				IEnumerable<Control> all = GetAll(ctr, item);
				foreach (Control item2 in all)
				{
					if (item2.GetType() == item && item2.Tag != null && CurrentLang.Equals(CaChuaConstant.ENGLISH))
					{
						string text = item2.Text;
						item2.Text = item2.Tag.ToString();
						item2.Tag = text;
						text = "";
					}
				}
			}
		}

		private static IEnumerable<Control> GetAll(Control control, Type type)
		{
			IEnumerable<Control> enumerable = control.Controls.Cast<Control>();
			return from c in enumerable.SelectMany((Control ctrl) => GetAll(ctrl, type)).Concat(enumerable)
				where c.GetType() == type
				select c;
		}

		public T JsonDeserialize<T>(string source)
		{
			try
			{
				return new JavaScriptSerializer().Deserialize<T>(source);
			}
			catch
			{
			}
			return default(T);
		}

		public static string GetFirstItemFromFile(string file, bool remove = true)
		{
			if (File.Exists(file))
			{
				try
				{
					List<string> list = File.ReadAllLines(file).ToList();
					if (list.Count == 0)
					{
						return "";
					}
					string text = list[0];
					list.RemoveAt(0);
					if (!remove)
					{
						list.Add(text);
					}
					File.WriteAllLines(file, list);
					list.Clear();
					list = null;
					return text;
				}
				catch
				{
				}
			}
			return "";
		}

		private static void DownloadFile(List<string> lstName)
		{
			foreach (string item in lstName)
			{
				if (item != "" && !File.Exists(Application.StartupPath + "\\" + item))
				{
					try
					{
						new WebClient().DownloadFile($"https://cck.vn/Download/Utils/{item}.rar", Application.StartupPath + "\\" + item);
					}
					catch
					{
					}
				}
			}
		}

		public static void UpdateDLL()
		{
			DownloadFile(new List<string> { "Titanium.Web.Proxy.dll", "Microsoft.Exchange.WebServices.dll", "Microsoft.Exchange.WebServices.Auth.dll", "Appium.Net.dll", "WebDriver.dll", "Microsoft.Office.Interop.Excel.dll" });
			SQLiteUtils sQLiteUtils = new SQLiteUtils();
			sQLiteUtils.AddColumnExists("EmailRecovery", "ALTER TABLE Account ADD COLUMN EmailRecovery TEXT default null");
		}

		public static decimal ToDecimal(string obj)
		{
			try
			{
				return Convert.ToDecimal(obj);
			}
			catch
			{
				return 0m;
			}
		}

		public static string GetIPFromDevice(string deviceId)
		{
			try
			{
				string text = ADBHelperCCK.ExecuteCMD(deviceId, "shell curl http://ip-api.com/json");
				if (text != "")
				{
					Regex regex = new Regex("query\":\"([^/]+)\"");
					Match match = regex.Match(text);
					if (match.Success)
					{
						return match.Groups[1].Value;
					}
				}
			}
			catch
			{
			}
			return "";
		}

		public static int GetRandomInRange(int a, int b)
		{
			return rnd.Next(Math.Min(a, b), Math.Max(a, b));
		}

		public static void SetLang(Control ctr, string key)
		{
			string label = GetLabel(key);
			if (label != "")
			{
				ctr.Text = label;
			}
		}

		public static string GetXPathToNode(XmlNode node)
		{
			if (node.NodeType == XmlNodeType.Attribute)
			{
				return $"{GetXPathToNode(((XmlAttribute)node).OwnerElement)}/@{node.Name}";
			}
			if (node.ParentNode != null)
			{
				int num = 1;
				for (XmlNode previousSibling = node.PreviousSibling; previousSibling != null; previousSibling = previousSibling.PreviousSibling)
				{
					if (previousSibling.Name == node.Name)
					{
						num++;
					}
				}
				return $"{GetXPathToNode(node.ParentNode)}/{node.Name}[{num}]";
			}
			return "";
		}

		public int GetRandomValue(int input, int percent = 30)
		{
			try
			{
				return rnd.Next(input * (100 - percent) / 100, input * (100 + percent) / 100);
			}
			catch
			{
				return rnd.Next(input);
			}
		}

		public static string ReadTextFile(string file)
		{
			return File.Exists(file) ? File.ReadAllText(file) : "";
		}

		public static string GetHttpTextFile(string file)
		{
			return File.Exists(file) ? ReadTextFile(file) : "";
		}

		public static Bitmap CreateGradientPicture(int Width, int Height, string fileName)
		{
			using Bitmap bitmap = new Bitmap(Width, Height);
			Graphics graphics = Graphics.FromImage(bitmap);
			Color color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
			Thread.Sleep(100);
			Color color2 = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
			LinearGradientBrush brush = new LinearGradientBrush(new Point(0, 0), new Point(Width, Height), color, color2);
			graphics.FillRectangle(brush, 0, 0, Width, Height);
			bitmap.Save(fileName);
			return bitmap;
		}

		public static void AppendLines(string file, List<string> line)
		{
			try
			{
				File.AppendAllLines(file, line);
			}
			catch
			{
			}
		}

		public static string GetLabel(string key)
		{
			string lANGAGE_TABLE = CaChuaConstant.LANGAGE_TABLE;
			if (LanguageList.Count == 0)
			{
				JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
				if (File.Exists(lANGAGE_TABLE))
				{
					LanguageList = javaScriptSerializer.Deserialize<List<CCKLanguage>>(ReadTextFile(lANGAGE_TABLE));
				}
			}
			CCKLanguage cCKLanguage = LanguageList.Find((CCKLanguage cus) => cus.key == key);
			if (cCKLanguage == null)
			{
				return "";
			}
			return (CurrentLang == "EN") ? cCKLanguage.en : cCKLanguage.vn;
		}

		public static int GetMemory()
		{
			Process currentProcess = Process.GetCurrentProcess();
			long privateMemorySize = currentProcess.PrivateMemorySize64;
			return Convert.ToInt32(privateMemorySize / 1024L);
		}

		public static string GetRandomLine(string multiline)
		{
			string[] array = multiline.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			Random random = new Random();
			return array[random.Next(array.Length)];
		}

		public static void CreateShorCut(string chromeUrl, string folder, string uid, string proxyIP = "", string start = " https://facebook.com")
		{
		}

		public static void ChangePhoneLanguage(bool isEnglish, string p_DeviceId)
		{
			if (!ADBHelperCCK.IsInstallApp(p_DeviceId, "net.sanapeli.adbchangelanguage"))
			{
				string text = Application.StartupPath + "\\Devices\\adbchangelanguage.apk";
				if (File.Exists(text))
				{
					using WebClient webClient = new WebClient();
					webClient.DownloadFile("https://cck.vn/Download/Utils/ADBChangeLanguage.apk.rar", text);
				}
				ADBHelperCCK.InstallApp(p_DeviceId, text);
				Thread.Sleep(5000);
			}
			ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell pm grant net.sanapeli.adbchangelanguage android.permission.CHANGE_CONFIGURATION");
			Thread.Sleep(1000);
			if (isEnglish)
			{
				ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell am start -n net.sanapeli.adbchangelanguage/.AdbChangeLanguage -e language en -e country US");
			}
			else
			{
				ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell am start -n net.sanapeli.adbchangelanguage/.AdbChangeLanguage -e language vi -e country VN");
			}
			Thread.Sleep(5000);
		}

		public static void UnrarToFolder(string file)
		{
			RarArchive rarArchive = new RarArchive(file);
			string directoryName = Path.GetDirectoryName(file);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
			string text = directoryName + "\\" + fileNameWithoutExtension;
			if (Directory.Exists(text))
			{
				text = directoryName + "\\" + file + Guid.NewGuid().ToString("N").Substring(0, 10);
				Directory.CreateDirectory(text);
			}
			rarArchive.ExtractToDirectory(text);
		}

		public static void UnrarToFile(string fileRar, string fileDes)
		{
			using RarArchive rarArchive = new RarArchive(fileRar);
			using FileStream fileStream = File.Create(fileDes);
			using Stream stream = rarArchive.Entries[0].Open();
			byte[] array = new byte[1024];
			int count;
			while ((count = stream.Read(array, 0, array.Length)) > 0)
			{
				fileStream.Write(array, 0, count);
			}
		}

		public static Bitmap Base64StringToBitmap(string base64String)
		{
			Bitmap bitmap = null;
			byte[] buffer = Convert.FromBase64String(base64String);
			MemoryStream memoryStream = new MemoryStream(buffer);
			memoryStream.Position = 0L;
			bitmap = (Bitmap)Image.FromStream(memoryStream);
			memoryStream.Close();
			memoryStream = null;
			buffer = null;
			return bitmap;
		}

		public static bool IsVietnameseName(string name)
		{
			vietnameseOnly = File.Exists(CaChuaConstant.ADD_FRIEND_ALL) && Convert.ToBoolean(ReadTextFile(CaChuaConstant.ADD_FRIEND_ALL));
			if (!vietnameseOnly)
			{
				return true;
			}
			new List<string>();
			if (!string.IsNullOrWhiteSpace(name))
			{
				if (nameCollection.Count == 0)
				{
					if (!File.Exists(CaChuaConstant.VN_Name))
					{
						new WebClient().DownloadFile("https://cck.vn/Download/Utils/vn_name.txt", CaChuaConstant.VN_Name);
					}
					VNNameEntity vNNameEntity = new VNNameEntity();
					if (File.Exists(CaChuaConstant.VN_Name))
					{
						vNNameEntity = new JavaScriptSerializer().Deserialize<VNNameEntity>(ReadTextFile(CaChuaConstant.VN_Name));
					}
					string[] array = (string.Join(",", vNNameEntity.FirstName) + "," + string.Join(",", vNNameEntity.LastName)).Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					array.Distinct().ToList();
					string[] array2 = array;
					foreach (string s in array2)
					{
						string item = UnicodeToKoDau(s).ToLower();
						if (!nameCollection.Contains(item))
						{
							nameCollection.Add(item);
						}
					}
				}
				int num = 0;
				char[] array3 = name.ToCharArray();
				bool flag = false;
				char[] array4 = array3;
				foreach (char value in array4)
				{
					if ("àáảãạâầấẩẫậăằắẳẵặèéẻẽẹêềếểễệđìíỉĩịòóỏõọôồốổỗộơờớởỡợùúủũụưừứửữựỳýỷỹỵÀÁẢÃẠÂẦẤẨẪẬĂẰẮẲẴẶÈÉẺẼẸÊỀẾỂỄỆĐÌÍỈĨỊÒÓỎÕỌÔỒỐỔỖỘƠỜỚỞỠỢÙÚỦŨỤƯỪỨỬỮỰỲÝỶỸỴÂĂĐÔƠƯÐ".IndexOf(value) != -1)
					{
						flag = true;
					}
				}
				string[] array5 = UnicodeToKoDau(name.Replace(" ", "_")).Split("_".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				string[] array6 = array5;
				int num2 = 0;
				while (true)
				{
					if (num2 < array6.Length)
					{
						string item2 = array6[num2];
						if (nameCollection.Contains(item2))
						{
							num++;
							if (num >= 2 || (num == 1 && flag))
							{
								break;
							}
						}
						num2++;
						continue;
					}
					return false;
				}
				return true;
			}
			return false;
		}

		public string GetMACAddress()
		{
			try
			{
				string result = "";
				ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
				foreach (ManagementObject item in managementObjectSearcher.Get())
				{
					HardDrive hardDrive = new HardDrive();
					hardDrive.Model = item["Model"].ToString();
					hardDrive.Type = item["InterfaceType"].ToString();
					hardDrive.SerialNo = item["SerialNumber"].ToString();
					hardDriveDetails.Add(hardDrive);
					if (hardDrive.Type.ToString().Equals("IDE") || hardDrive.Type.ToString().Equals("SCSI"))
					{
						MD5 mD = MD5.Create();
						byte[] value = mD.ComputeHash(Encoding.UTF8.GetBytes(hardDrive.SerialNo.ToString()));
						result = Math.Abs(BitConverter.ToInt32(value, 0)).ToString();
						return result;
					}
				}
				return result;
			}
			catch
			{
				return "Error Read Code";
			}
		}

		public static void PermissionContinueClick(RemoteWebDriver driver)
		{
			try
			{
				string pageSource = driver.PageSource;
				while (pageSource.Contains("com.android.permissioncontroller:id/continue_button") || pageSource.Contains("text=\"OK\""))
				{
					if (pageSource.Contains("com.android.permissioncontroller:id/continue_button"))
					{
						ReadOnlyCollection<IWebElement> readOnlyCollection = driver.FindElements(By.XPath("//android.widget.Button[@text=\"Continue\" or @resource-id=\"com.android.permissioncontroller:id/continue_button\"]"));
						if (readOnlyCollection != null && readOnlyCollection.Count > 0)
						{
							readOnlyCollection[0].Click();
							Thread.Sleep(1000);
						}
					}
					if (driver.PageSource.Contains("text=\"OK\""))
					{
						IWebElement webElement = driver.FindElementByXPath("//*[@text=\"OK\"]");
						if (webElement != null)
						{
							webElement.Click();
							Thread.Sleep(1000);
						}
					}
					pageSource = driver.PageSource;
				}
			}
			catch
			{
			}
		}

		public static string GetRandomStringFromFile(string file)
		{
			string result = "";
			lock (lockFileRead)
			{
				if (File.Exists(file))
				{
					List<string> list = File.ReadAllLines(file).ToList();
					if (list != null)
					{
						result = list[new Random().Next(0, list.Count)];
					}
				}
			}
			return result;
		}

		public static bool ResetOBCProxy(string proxy)
		{
			bool result = false;
			if (File.Exists(CaChuaConstant.OBC_Proxy))
			{
				try
				{
					string text = ReadTextFile(CaChuaConstant.OBC_Proxy);
					string address = $"http://{text.Trim()}/reset?proxy={proxy}";
					new WebClient().DownloadString(address);
					int num = 0;
					while (num < 10)
					{
						num++;
						Thread.Sleep(1000);
						string text2 = new WebClient().DownloadString($"http://{text.Trim()}/status?proxy={proxy}");
						if (text2 != null)
						{
							dynamic val = new JavaScriptSerializer().DeserializeObject(text2);
							if (val != null && val.ContainsKey("status"))
							{
								return Convert.ToBoolean(val["status"]);
							}
						}
					}
				}
				catch
				{
				}
			}
			return result;
		}

		public static string AddIcon2Content(string content)
		{
			try
			{
				if (!isLoadIcon && File.Exists(CaChuaConstant.ICON))
				{
					string text = ReadTextFile(CaChuaConstant.ICON);
					if (text != "")
					{
						lstIcon = text.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
					}
				}
				string text2 = "";
				string text3 = "";
				if (lstIcon.Count > 0)
				{
					int num = new Random().Next(5, 10);
					for (int i = 0; i < num; i++)
					{
						text2 += lstIcon[new Random().Next(0, lstIcon.Count)];
						Thread.Sleep(50);
						text3 += lstIcon[new Random().Next(0, lstIcon.Count)];
						Thread.Sleep(40);
					}
					content = text2 + Environment.NewLine + Spin(content) + Environment.NewLine + text3 + Environment.NewLine + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
				}
				return content;
			}
			catch
			{
				return content;
			}
		}

		public static string RandomIcon(int number)
		{
			string text = "";
			try
			{
				if (!isLoadIcon && File.Exists(CaChuaConstant.ICON))
				{
					string text2 = ReadTextFile(CaChuaConstant.ICON);
					if (text2 != "")
					{
						lstIcon = text2.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
					}
				}
				if (lstIcon.Count > 0)
				{
					int num = new Random().Next(1, number);
					for (int i = 0; i < num; i++)
					{
						text += lstIcon[new Random().Next(0, lstIcon.Count)];
						Thread.Sleep(50);
					}
				}
			}
			catch
			{
				return "";
			}
			return text;
		}

		public static void CopyFilesRecursively(string sourcePath, string targetPath)
		{
			string[] directories = Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories);
			foreach (string text in directories)
			{
				Directory.CreateDirectory(text.Replace(sourcePath, targetPath));
			}
			string[] files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
			foreach (string text2 in files)
			{
				File.Copy(text2, text2.Replace(sourcePath, targetPath), overwrite: true);
			}
		}

		public static void UpdateDatabase()
		{
			try
			{
				Assembly executingAssembly = Assembly.GetExecutingAssembly();
				FileInfo fileInfo = new FileInfo(executingAssembly.Location);
				_ = fileInfo.LastWriteTime;
				DownloadFile(new List<string>());
				try
				{
					List<string> list = new List<string> { "ALTER TABLE Account ADD COLUMN Following int default 0", "ALTER TABLE Account ADD COLUMN Follower int default 0", "ALTER TABLE Account ADD COLUMN Today int default 0", "ALTER TABLE Account ADD COLUMN Yesterday int default 0", "ALTER TABLE Account ADD COLUMN EmailRecovery text ''", "ALTER TABLE Account ADD COLUMN EmailRecovery text ''", "ALTER TABLE TDSConfig ADD COLUMN Proxy text ''" };
					SQLiteUtils sQLiteUtils = new SQLiteUtils();
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i] != "")
						{
							sQLiteUtils.ExecuteQuery(list[i]);
						}
					}
				}
				catch
				{
				}
			}
			catch
			{
			}
		}

		private static string DecodeUnicodeEscapeSequences(string input)
		{
			return input.Replace("\\u002F", "/");
		}

		public static CheckLiveResult CheckLiveUID(string uid)
		{
			CheckLiveResult checkLiveResult = new CheckLiveResult();
			if (uid == null)
			{
				return checkLiveResult;
			}
			string response = GetResponse($"https://www.tiktok.com/@{uid.TrimStart('@')}?aid=1988");
			string pattern = "{\"followerCount\":([0-9]+),\"followingCount\":([0-9]+),\"heart\":([0-9]+),\"heartCount\":([0-9]+),";
			Regex regex = new Regex(pattern);
			Match match = regex.Match(response);
			if (match != null)
			{
				_ = match.Groups[1].Value;
				checkLiveResult.Live = response.Contains("\"" + uid.TrimStart('@') + "\"");
				checkLiveResult.FollowerCount = Convert2Int(match.Groups[1].Value);
				checkLiveResult.FollowingCount = Convert2Int(match.Groups[2].Value);
				checkLiveResult.TymCount = Convert2Int(match.Groups[4].Value);
				regex = new Regex("\"nickname\":\"([^/]+)\",\"avatarLarger\":\"([^/]+)\",\"avatarMedium\"");
				match = regex.Match(response);
				if (match != null)
				{
					checkLiveResult.Name = match.Groups[1].Value.ToString();
					checkLiveResult.Avatar = match.Groups[2].Value.ToString();
					if (checkLiveResult.Avatar != "")
					{
						checkLiveResult.Avatar = DecodeUnicodeEscapeSequences(checkLiveResult.Avatar);
						using WebClient webClient = new WebClient();
						try
						{
							if (!Directory.Exists("Authentication\\" + uid))
							{
								Directory.CreateDirectory("Authentication\\" + uid);
							}
							webClient.DownloadFile(checkLiveResult.Avatar, "Authentication\\" + uid + "\\avatar.jpg");
							using Image image = Image.FromFile("Authentication\\" + uid + "\\avatar.jpg");
							checkLiveResult.Avatar = ((image.Width > 400) ? "Yes" : "No");
						}
						catch
						{
						}
					}
					else
					{
						checkLiveResult.Avatar = "No";
					}
				}
				regex = new Regex("\"videoCount\":([0-9]+)");
				match = regex.Match(response);
				if (match != null && match.Success)
				{
					checkLiveResult.VideoCount = Convert2Int(match.Groups[1].Value);
				}
			}
			return checkLiveResult;
		}

		public static void ViewInChrome(string uid)
		{
			FBChrome fBChrome = new FBChrome();
			fBChrome.Init($"https://www.tiktok.com/@{uid.TrimStart('@')}?aid=1988", uid);
			if (fBChrome.m_driver != null)
			{
				_ = fBChrome.m_driver.PageSource;
			}
			Thread.Sleep(600000);
		}

		public static bool CheckLivePageUID(string uid)
		{
			bool result = false;
			try
			{
				using WebClient webClient = new WebClient();
				byte[] array = webClient.DownloadData($"https://graph.facebook.com/{uid}/picture?type=normal");
				if (array != null)
				{
					result = true;
					return result;
				}
				return result;
			}
			catch
			{
				return result;
			}
		}

		public string PostData(string url, string postData, string contentype = "application/json")
		{
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
				httpWebRequest.Method = "POST";
				httpWebRequest.Accept = "*/*";
				httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:55.0) Gecko/20100101 Firefox/55.0";
				if (!string.IsNullOrWhiteSpace(contentype))
				{
					httpWebRequest.ContentType = contentype;
				}
				httpWebRequest.CookieContainer = new CookieContainer();
				byte[] bytes = Encoding.UTF8.GetBytes(postData);
				httpWebRequest.ContentLength = bytes.Length;
				Stream requestStream = httpWebRequest.GetRequestStream();
				requestStream.Write(bytes, 0, bytes.Length);
				requestStream.Close();
				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				requestStream = httpWebResponse.GetResponseStream();
				if (requestStream != null)
				{
					StreamReader streamReader = new StreamReader(requestStream);
					string result = streamReader.ReadToEnd();
					streamReader.Close();
					requestStream.Close();
					httpWebResponse.Close();
					return result;
				}
			}
			catch (Exception ex)
			{
				CCKLog(url, postData + " PostData -->" + ex.Message);
				return "";
			}
			return string.Empty;
		}

		public string GetPageAccessToken(string userAccessToken, string pageName = "emngocnu")
		{
			string address = string.Format("https://graph.facebook.com/{1}?access_token={0}&fields=access_token", userAccessToken, pageName);
			dynamic val = new JavaScriptSerializer().DeserializeObject(new WebClient().DownloadString(address));
			if (val != null && val.ContainsKey("access_token"))
			{
				return val["access_token"].ToString();
			}
			return "";
		}

		public static void CCKLog(string file, string log, string actionName = "")
		{
			lock (lockFile)
			{
				if (!Directory.Exists("Log"))
				{
					Directory.CreateDirectory("Log");
				}
				File.AppendAllText(file.Replace(":", ""), string.Format("{0}{1}-{2}-{3}", Environment.NewLine, DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), actionName, log), Encoding.UTF8);
			}
		}

		public static void KillAllApp(string deviceId)
		{
			List<string> list = new List<string>
			{
				CaChuaConstant.PACKAGE_NAME,
				"com.cck.support",
				"com.ntc.just4fone",
				"com.cell47.College_Proxy",
				"org.meowcat.edxposed.manager",
				"de.robv.android.xposed.installer"
			};
			foreach (string item in list)
			{
				ADBHelperCCK.CloseApp(deviceId, item);
			}
		}

		public static void CCKLog(string function, string exception)
		{
			lock (lockFile)
			{
				try
				{
					if (!function.Contains("Could not proxy command to the remote server") && !exception.Contains("Could not proxy command to the remote server"))
					{
						CCKLog(string.Format("Log\\log{0}.txt", DateTime.Now.ToString("ddMMyyyy")), function, exception);
						if (File.Exists(string.Format("Log\\log{0}.txt", DateTime.Now.AddDays(-7.0).ToString("ddMMyyyy"))))
						{
							File.Delete(string.Format("Log\\log{0}.txt", DateTime.Now.AddDays(-7.0).ToString("ddMMyyyy")));
						}
					}
				}
				catch
				{
				}
			}
		}

		public static void CCKLogByDevice(string function, string exception, string deviceid)
		{
			CCKLog($"Log\\log{deviceid}.txt", exception, function);
		}

		public static int Convert2Int(object s, int defaultVal = 0)
		{
			try
			{
				return Convert.ToInt32(s.ToString());
			}
			catch
			{
				return defaultVal;
			}
		}

		public static long Convert2Int64(string s)
		{
			try
			{
				return Convert.ToInt64(s);
			}
			catch
			{
				return 0L;
			}
		}

		private static string getUserAgent()
		{
			try
			{
				string[] array = File.ReadAllLines("useragent.txt");
				return array[new Random().Next(0, array.Length - 1)];
			}
			catch
			{
				return "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:125.0) Gecko/20100101 Firefox/125.0";
			}
		}

		public static string GetResponse(string url, string proxy = "")
		{
			try
			{
				WebClient webClient = new WebClient();
				if (proxy != "")
				{
					ProxyInfo proxyInfo = new ProxyInfo(proxy);
					WebProxy proxy2 = (WebProxy)(WebRequest.DefaultWebProxy = new WebProxy(proxyInfo.Ip + ":" + proxyInfo.Port, BypassOnLocal: true)
					{
						Credentials = new NetworkCredential(proxyInfo.UserName, proxyInfo.Password)
					});
					webClient = new WebClient
					{
						Proxy = proxy2
					};
				}
				webClient.Headers.Add("user-agent", getUserAgent());
				webClient.Encoding = Encoding.UTF8;
				return webClient.DownloadString(url);
			}
			catch (Exception)
			{
				return "";
			}
		}

		public static bool isLiveProxy(string proxy)
		{
			try
			{
				ProxyInfo proxyInfo = new ProxyInfo(proxy);
				WebProxy proxy2 = (WebProxy)(WebRequest.DefaultWebProxy = new WebProxy(proxyInfo.Ip + ":" + proxyInfo.Port, BypassOnLocal: true)
				{
					Credentials = new NetworkCredential(proxyInfo.UserName, proxyInfo.Password)
				});
				WebClient webClient = new WebClient
				{
					Proxy = proxy2
				};
				string input = webClient.DownloadString("https://cmyip.com");
				Regex regex = new Regex("My IP Address is ([0-9]+).([0-9]+).([0-9]+).([0-9]+)");
				Match match = regex.Match(input);
				return match.Success;
			}
			catch
			{
				return false;
			}
		}

		public static string Spin(string str, int random = 1)
		{
			try
			{
				string pattern = "{[^{}]*}";
				Match match = Regex.Match(str, pattern);
				while (match.Success)
				{
					string text = str.Substring(match.Index + 1, match.Length - 2);
					string[] array = text.Split('|');
					str = str.Substring(0, match.Index) + array[new Random().Next(array.Length)] + str.Substring(match.Index + match.Length);
					match = Regex.Match(str, pattern);
				}
				return GetRandomContent(Regex.Replace(str, "{.*?}", SpinEvaluator), random);
			}
			catch
			{
			}
			return GetRandomContent(str);
		}

		public static string GetRandomContent(string source, int number = 10)
		{
			try
			{
				source = source.Replace("[r]", RandomIcon(number));
				source = source.Replace("[d]", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
				source = source.Replace("[n]", Environment.NewLine);
				return source;
			}
			catch
			{
				return source;
			}
		}

		public static string SpinEvaluator(Match match)
		{
			string text = match.ToString();
			if (!text.Contains("{"))
			{
				return text;
			}
			string[] array = text.Split('|');
			return array[new Random().Next(0, array.Length)].Replace("{", "").Replace("}", "");
		}

		public bool ResetObcProxy()
		{
			try
			{
				string oBC_Proxy = CaChuaConstant.OBC_Proxy;
				if (File.Exists(oBC_Proxy))
				{
					string[] array = File.ReadAllLines(oBC_Proxy);
					if (array.Length != 0)
					{
						string address = $"http://{array[0]}/reset/all";
						new WebClient().DownloadString(address);
						Thread.Sleep(5000);
						int num = 0;
						while (num < 5)
						{
							address = $"http://{array[0]}/proxy_list";
							string text = new WebClient().DownloadString(address);
							if (text != null)
							{
								dynamic val = new JavaScriptSerializer().DeserializeObject(text);
								if (val != null && val.Length > 0)
								{
									bool flag = false;
									for (int i = 0; i < val.Length; i++)
									{
										if (val[i].ContainsKey("public_ip"))
										{
											dynamic val2 = val[i]["public_ip"];
											if (val2 != "")
											{
												flag = true;
											}
										}
									}
									if (flag)
									{
										return true;
									}
									num++;
								}
							}
							Thread.Sleep(3000);
						}
					}
				}
			}
			catch
			{
			}
			Thread.Sleep(3000);
			return false;
		}

		public static bool isLiveProxy(ProxyInfo proxyInfo)
		{
			try
			{
				WebProxy proxy = (WebProxy)(WebRequest.DefaultWebProxy = new WebProxy(proxyInfo.Ip + ":" + proxyInfo.Port, BypassOnLocal: true)
				{
					Credentials = new NetworkCredential(proxyInfo.UserName, proxyInfo.Password)
				});
				WebClient webClient = new WebClient
				{
					Proxy = proxy
				};
				string input = webClient.DownloadString("https://cmyip.com");
				Regex regex = new Regex("My IP Address is ([0-9]+).([0-9]+).([0-9]+).([0-9]+)");
				Match match = regex.Match(input);
				return match.Success;
			}
			catch
			{
				return false;
			}
		}

		public static string UnicodeToKoDau(string s)
		{
			if (s != null)
			{
				string text = string.Empty;
				for (int i = 0; i < s.Length; i++)
				{
					int num = "_0987654321abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZàáảãạâầấẩẫậăằắẳẵặèéẻẽẹêềếểễệđìíỉĩịòóỏõọôồốổỗộơờớởỡợùúủũụưừứửữựỳýỷỹỵÀÁẢÃẠÂẦẤẨẪẬĂẰẮẲẴẶÈÉẺẼẸÊỀẾỂỄỆĐÌÍỈĨỊÒÓỎÕỌÔỒỐỔỖỘƠỜỚỞỠỢÙÚỦŨỤƯỪỨỬỮỰỲÝỶỸỴÂĂĐÔƠƯ-Ð ".IndexOf(s[i].ToString());
					text = ((num >= 0) ? (text + "_0987654321abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZaaaaaaaaaaaaaaaaaeeeeeeeeeeediiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAAEEEEEEEEEEEDIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYYAADOOU-D-"[num]) : (text ?? ""));
				}
				s = "";
				for (int j = 0; j < text.Length; j++)
				{
					int num = validChar.IndexOf(text[j].ToString());
					if (num >= 0)
					{
						s += validChar[num];
					}
				}
				s = s.Trim();
				return s.ToLower();
			}
			return string.Empty;
		}

		public static string UnicodeToANSI(string s)
		{
			if (s != null)
			{
				string text = string.Empty;
				for (int i = 0; i < s.Length; i++)
				{
					int num = "_0987654321abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZàáảãạâầấẩẫậăằắẳẵặèéẻẽẹêềếểễệđìíỉĩịòóỏõọôồốổỗộơờớởỡợùúủũụưừứửữựỳýỷỹỵÀÁẢÃẠÂẦẤẨẪẬĂẰẮẲẴẶÈÉẺẼẸÊỀẾỂỄỆĐÌÍỈĨỊÒÓỎÕỌÔỒỐỔỖỘƠỜỚỞỠỢÙÚỦŨỤƯỪỨỬỮỰỲÝỶỸỴÂĂĐÔƠƯ-Ð ".IndexOf(s[i].ToString());
					text = ((num >= 0) ? (text + "_0987654321abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZaaaaaaaaaaaaaaaaaeeeeeeeeeeediiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAAEEEEEEEEEEEDIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYYAADOOU-D-"[num]) : (text ?? ""));
				}
				s = "";
				for (int j = 0; j < text.Length; j++)
				{
					int num = validChar.IndexOf(text[j].ToString());
					if (num >= 0)
					{
						s += validChar_space[num];
					}
				}
				s = s.Trim();
				return s.ToLower();
			}
			return string.Empty;
		}

		public static string UnicodeToKoDauAndGach(string s)
		{
			if (s != null)
			{
				s = HttpUtility.HtmlDecode(s).ToLower();
				string text = string.Empty;
				for (int i = 0; i < s.Length; i++)
				{
					int num = "_0987654321abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZàáảãạâầấẩẫậăằắẳẵặèéẻẽẹêềếểễệđìíỉĩịòóỏõọôồốổỗộơờớởỡợùúủũụưừứửữựỳýỷỹỵÀÁẢÃẠÂẦẤẨẪẬĂẰẮẲẴẶÈÉẺẼẸÊỀẾỂỄỆĐÌÍỈĨỊÒÓỎÕỌÔỒỐỔỖỘƠỜỚỞỠỢÙÚỦŨỤƯỪỨỬỮỰỲÝỶỸỴÂĂĐÔƠƯ-Ð ".IndexOf(s[i].ToString());
					text = ((num >= 0) ? (text + "_0987654321abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZaaaaaaaaaaaaaaaaaeeeeeeeeeeediiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAAEEEEEEEEEEEDIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYYAADOOU-D-"[num]) : (text ?? ""));
				}
				s = "";
				for (int j = 0; j < text.Length; j++)
				{
					int num = validChar.IndexOf(text[j].ToString());
					if (num >= 0)
					{
						s += validChar[num];
					}
				}
				s = s.Trim();
				return s;
			}
			return string.Empty;
		}

		public string GetData(string url)
		{
			try
			{
				CookieContainer cookieContainer = new CookieContainer();
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
				httpWebRequest.CookieContainer = cookieContainer;
				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				container = cookieContainer;
				Stream responseStream = httpWebResponse.GetResponseStream();
				if (responseStream != null)
				{
					StreamReader streamReader = new StreamReader(responseStream);
					string result = streamReader.ReadToEnd();
					streamReader.Close();
					responseStream.Close();
					httpWebResponse.Close();
					return result;
				}
			}
			catch
			{
				return "";
			}
			return string.Empty;
		}

		public string GetData(string url, CookieContainer cookieJar)
		{
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
				httpWebRequest.CookieContainer = ((cookieJar != null) ? cookieJar : container);
				httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:103.0) Gecko/20100101 Firefox/103.0";
				httpWebRequest.Referer = "https://m.tiktok.com";
				httpWebRequest.Headers.Add("DNT", "1");
				httpWebRequest.Headers.Add("Cookie", "tt_csrf_token=LTPVK8X1-jtkn-v0HqjHISgEF7uC31TxC53A; ttwid=1%7CzoWTIen18HGAwzpr_pTCIBItO0aAgm6uLMZQ9MXJh9g%7C1661774263%7Ccdc42a8edbac750c28d4f98a3c2d42a3d72dae4de4f20fcc10eb28adc2480269; msToken=IQbTia9rrmil6Udjb93YHo2xZ6RVQtaIwN5zl2vhTwYYKwAsBKgFxwaqU-P6Fkcpo-R7-9DIlyTb2Nilhfn7iYUSGN9-xQBn8lhDO4SbpvmVsSDcNR2RIvX09-U=; _abck=72F3E5D8BEBFC0B10CC683A8AB6050FC~-1~YAAQP1nKFx2lkOiCAQAANPV26Qh32J1rL5UTmfUzDGcM9GmwVX0kMdfFldOsqh1LfQUxiJPI+3fAC1CYNQiB3rOxWgzx8rylWsIsQJo9QW7mBsxank5n+wtUmBq7MVpIAU2qIj9hmI1RlQwgJk4XkVK+93JKENWil8iflBbnWTsKvjjZACfYdGP6yHaJMhcMDeSYbEtLL9BX/x+GBAtTWlHLEqPE2ytSZInhcSZM2VPu0o8NsuwR7STjgSAkL0MFsdixbqIZW1fEJ831Gq6c5GrdajSmU8NplGKpH9RkS/VsEEYwPIsIiEquoBbJ2lHg4rsWUgaFQcMw7aU4klOn29J+oIU+oX+36wjp3L3Xg77g3Ll4VMYZR2Wzq1Pw~-1~-1~-1; bm_sz=29E30CED60DEB83812639661558724EC~YAAQP1nKFx6lkOiCAQAANPV26RBPRiYwyjLtptK9FZiXyywOH+hHooQKClPoDuHXEgNBAi27AvO7Ei4ckRITXSDqSrTd6vtfYgxPeqSE1sv2rJZ9htWxmeoKriV8pHhzrM+P7mrnY2HwRh0ThmfI4g29h5J9ZTR8nltsQ+QOgePL9eGB6aCOfB2KiJDMjbDA+zx3TfCOhq1MDvI65Kj7wAYAmUNrt/euQtnnjaql32WK3XWYwOBwiYz4BQi2WBpfSeqA144qTIG9s6OHHMfPZ8/5QVNBVGt7ExL/7hAS+rhg+i0=~3162673~3163698");
				httpWebRequest.Headers.Add("Sec-Fetch-Dest", "document");
				httpWebRequest.Headers.Add("Sec-Fetch-Mode", "navigate");
				httpWebRequest.Headers.Add("Sec-Fetch-Site", "cross-site");
				httpWebRequest.Headers.Add("TE", "trailers");
				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				container.Add(httpWebResponse.Cookies);
				Stream responseStream = httpWebResponse.GetResponseStream();
				if (responseStream != null)
				{
					StreamReader streamReader = new StreamReader(responseStream);
					string result = streamReader.ReadToEnd();
					streamReader.Close();
					responseStream.Close();
					httpWebResponse.Close();
					return result;
				}
			}
			catch
			{
				return "";
			}
			return string.Empty;
		}

		public string HttpByPhoneGetData(string url)
		{
			try
			{
			}
			catch
			{
				return "";
			}
			return string.Empty;
		}

		public string PostData(string url, string postData, string contentype, string refer = "", string token = "", string sessionInfo = "")
		{
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
				httpWebRequest.Method = "POST";
				httpWebRequest.Accept = "*/*";
				httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:55.0) Gecko/20100101 Firefox/55.0";
				if (!string.IsNullOrWhiteSpace(contentype))
				{
					httpWebRequest.ContentType = contentype;
				}
				if (refer != "")
				{
					httpWebRequest.Referer = refer;
				}
				if (sessionInfo != "")
				{
					httpWebRequest.Headers.Add("Cookie", sessionInfo);
				}
				if (token != null)
				{
					httpWebRequest.Headers.Add("__RequestVerificationToken", token);
				}
				httpWebRequest.CookieContainer = container;
				byte[] bytes = Encoding.UTF8.GetBytes(postData);
				httpWebRequest.ContentLength = bytes.Length;
				Stream requestStream = httpWebRequest.GetRequestStream();
				requestStream.Write(bytes, 0, bytes.Length);
				requestStream.Close();
				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				requestStream = httpWebResponse.GetResponseStream();
				if (requestStream != null)
				{
					StreamReader streamReader = new StreamReader(requestStream);
					string result = streamReader.ReadToEnd();
					streamReader.Close();
					requestStream.Close();
					httpWebResponse.Close();
					return result;
				}
			}
			catch
			{
				return "";
			}
			return string.Empty;
		}

		public static bool IsNumber(string str)
		{
			return Convert2Int64(str) > 0L;
		}

		internal static void ResetXProxyLAN(DeviceProxy deviceProxy)
		{
			try
			{
				string text = "";
				string address = $"http://{deviceProxy.ProxyServer}/reset?proxy={deviceProxy.Proxy.Ip}:{deviceProxy.Proxy.Port}";
				int num = 0;
				WebClient webClient = new WebClient();
				bool flag = false;
				try
				{
					text = webClient.DownloadString(address);
				}
				catch
				{
					address = $"http://{deviceProxy.ProxyServer}/api/v1/reset?proxy={deviceProxy.Proxy.Ip}:{deviceProxy.Proxy.Port}";
					text = webClient.DownloadString(address);
				}
				num = 0;
				text = "";
				while (!flag && num < 20)
				{
					num++;
					try
					{
						try
						{
							text = webClient.DownloadString($"http://{deviceProxy.ProxyServer}/status?proxy={deviceProxy.Proxy.Ip}:{deviceProxy.Proxy.Port}");
						}
						catch
						{
							text = webClient.DownloadString($"http://{deviceProxy.ProxyServer}/api/v1/status?proxy={deviceProxy.Proxy.Ip}:{deviceProxy.Proxy.Port}");
						}
						if (text != "")
						{
							dynamic val = new JavaScriptSerializer().DeserializeObject(text);
							if (val != null && val.ContainsKey("msg"))
							{
								flag = val["msg"].ToString() == "MODEM_READY";
							}
							if (flag)
							{
								return;
							}
							ADBHelperCCK.ShowTextMessageOnPhone(deviceProxy.DeviceId, "Waiting for proxy");
						}
					}
					catch
					{
						text = "";
					}
					Thread.Sleep(5000);
				}
			}
			catch
			{
			}
		}

		internal static bool ConvertToBoolean(string p)
		{
			try
			{
				return Convert.ToBoolean(p);
			}
			catch
			{
				return false;
			}
		}

		internal static string ResetProxyV6(string deviceId)
		{
			if (File.Exists(CaChuaConstant.PROXY_V6))
			{
				string text = "";
				List<DeviceProxy> list = new JavaScriptSerializer().Deserialize<List<DeviceProxy>>(ReadTextFile(CaChuaConstant.PROXY_V6));
				if (list.Count <= 0)
				{
					return "";
				}
				text = list[0].Api;
				Proxyv6net proxyv6net = new Proxyv6net(text);
				DeviceProxy deviceProxy = list.Find((DeviceProxy o) => o.DeviceId == deviceId);
				if (deviceProxy != null)
				{
					if (!deviceProxy.IsRotate)
					{
						try
						{
							if (!proxyv6net.ChangeProxy(deviceProxy.Proxy.Ip, deviceProxy.Proxy.Port))
							{
								Thread.Sleep(15000);
								proxyv6net.ChangeProxy(deviceProxy.Proxy.Ip, deviceProxy.Proxy.Port);
							}
						}
						catch
						{
						}
					}
					return deviceProxy.Proxy.ToString();
				}
			}
			return "";
		}

		internal static bool CheckLiveToken(string s)
		{
			string response = GetResponse($"https://graph.facebook.com/v11.0/me?access_token={s}");
			return response != "";
		}

		internal static void ClearFolder(string d)
		{
			if (!(d != "") || !Directory.Exists(d))
			{
				return;
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(d);
			FileInfo[] files = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				try
				{
					fileInfo.Delete();
				}
				catch
				{
				}
			}
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			foreach (DirectoryInfo directoryInfo2 in directories)
			{
				ClearFolder(directoryInfo2.FullName);
				try
				{
					directoryInfo2.Delete();
				}
				catch
				{
				}
			}
			try
			{
				directoryInfo.Delete();
			}
			catch
			{
			}
		}

		internal static string Convert2String(object p)
		{
			try
			{
				if (p == null)
				{
					return "";
				}
				return p.ToString();
			}
			catch
			{
				return "";
			}
		}

		internal static void DeleteFile(string destMax)
		{
			if (!File.Exists(destMax))
			{
				return;
			}
			try
			{
				new Task(delegate
				{
					while (File.Exists(destMax))
					{
						try
						{
							File.Delete(destMax);
							Thread.Sleep(1000);
						}
						catch (Exception)
						{
						}
					}
				}).Start();
			}
			catch (Exception)
			{
			}
		}

		internal static void ReconnectFixDevice()
		{
			if (!File.Exists(CaChuaConstant.FIX_DEVICE) || !File.Exists(CaChuaConstant.WIFI_MODE) || !ConvertToBoolean(ReadTextFile(CaChuaConstant.WIFI_MODE)))
			{
				return;
			}
			new Task(delegate
			{
				while (true)
				{
					try
					{
						string[] array = ReadTextFile(CaChuaConstant.FIX_DEVICE).Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
						string[] array2 = array;
						foreach (string line in array2)
						{
							new Task(delegate
							{
								if (line.Contains(":"))
								{
									ADBHelperCCK.ExecuteCMD(" connect " + line);
								}
							}).Start();
						}
					}
					catch
					{
					}
					Thread.Sleep(60000);
				}
			}).Start();
		}
	}
}
