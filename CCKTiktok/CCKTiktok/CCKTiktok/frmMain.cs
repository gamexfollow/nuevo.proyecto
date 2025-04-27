using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using CCKTiktok.Bussiness;
using CCKTiktok.Component;
using CCKTiktok.DAL;
using CCKTiktok.Entity;
using CCKTiktok.Helper;
using CCKTiktok.Properties;
using CCKTiktokV32.Bussiness;
using MetroFramework.Controls;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Remote;
using Titanium.Web.Proxy;

namespace CCKTiktok
{
	public class frmMain : Form
	{
		private List<Task> mainTask = new List<Task>();

		private static int numberOfDevice = 0;

		private static SQLiteUtils sql = new SQLiteUtils();

		private DateTime startRunningDate = DateTime.Now;

		private static Dictionary<string, string> dicDevices = new Dictionary<string, string>();

		private static Dictionary<string, int> dicDevicesName = new Dictionary<string, int>();

		private static bool isTuongTacClick = false;

		private ActionButton button = ActionButton.Start;

		private static int devicecount = 0;

		private Dictionary<string, RemoteWebDriver> lstServer = new Dictionary<string, RemoteWebDriver>();

		private Queue<string> queueProxy = new Queue<string>();

		private static object lockUid = new object();

		public static Dictionary<string, string> DicTmProxy = new Dictionary<string, string>();

		public static List<string> ActiveDevice = new List<string>();

		public static Dictionary<string, List<DeviceProxy>> dicXproxyLan = new Dictionary<string, List<DeviceProxy>>();

		private Random rnd = new Random();

		private Dictionary<string, string> TDSAPI = new Dictionary<string, string>();

		private Dictionary<string, int> deviceInfo = new Dictionary<string, int>();

		private bool change3GbyCommand = false;

		private static List<string> LoginByLink = new List<string>();

		private static bool IsshowLoginDialog = false;

		private static object myLock = new object();

		private static object lockItem = new object();

		private static object gridLock = new object();

		private static Dictionary<string, DataGridViewRow> dataGridViewIndexByUid = new Dictionary<string, DataGridViewRow>();

		private static List<Task> tasks = new List<Task>();

		private CancellationTokenSource taskStop = new CancellationTokenSource();

		private Dictionary<string, List<string>> lstUid = new Dictionary<string, List<string>>();

		private Dictionary<string, List<string>> DictionUid = new Dictionary<string, List<string>>();

		private Dictionary<string, DataGridViewRow> lstDataGridViewRow = new Dictionary<string, DataGridViewRow>();

		private static bool isRuning = true;

		private DataTable tbl = new DataTable();

		private string selectedUid = "";

		private string selectedDevice = "";

		private bool IsVietnamese = true;

		private Process ffmpegProcess;

		private string outputVideoPath = "output.mp4";

		private IContainer components = null;

		private ToolStripProgressBar progressBar1;

		private OpenFileDialog openFileDialog1;

		private StatusStrip statusStrip1;

		private ToolStripStatusLabel toolStripStatusLabel1;

		private CheckBox cbxchon;

		private DataGridView dataGridView1;

		private GroupBox grbAction;

		private Button btn2fa;

		private Button btnLoadAccount;

		private Button btnStop;

		private CheckedListBox cbxCategory;

		private ToolStripStatusLabel lblKey;

		private ToolStripStatusLabel lblSub;

		private GroupBox groupBox5;

		private LinkLabel lbl4g3g;

		private RadioButton rbtProxy;

		private RadioButton rpt3g;

		private ToolStripStatusLabel toolStripStatusLabel2;

		private Button btnPhone;

		private GroupBox groupBox1;

		private CheckBox cbxError;

		private CheckBox cbxnew;

		private DataGridView dataGridViewPhone;

		private Button btnLoadDevices;

		private LinkLabel lblProxy;

		private ToolStripStatusLabel tssRam;

		private GroupBox groupAction;

		private LinkLabel linkLabel1;

		private CheckBox cbxLike;

		private LinkLabel linkLabel6;

		private CheckBox cbxUpVideo;

		private LinkLabel linkLabel5;

		private CheckBox cbxSeedingVideo;

		private LinkLabel linkLabel4;

		private CheckBox cbxFolowUID;

		private LinkLabel linkLabel3;

		private CheckBox cbxCommentRandom;

		private LinkLabel linkLabel2;

		private CheckBox cbxNewsFeed;

		private Button btnReg;

		private LinkLabel linkLabel11;

		private CheckBox cbxChangeAvatar;

		private LinkLabel linkLabel12;

		private CheckBox cbxChangePass;

		private LinkLabel linkLabel15;

		private CheckBox cbxBio;

		private LinkLabel linkLabel14;

		private CheckBox cbxRename;

		private GroupBox groupProfile;

		private Button button1;

		private RadioButton rbtXProxy;

		private LinkLabel linkLabel8;

		private RadioButton rbtTM;

		private LinkLabel linkLabel7;

		private GroupBox groupShop;

		private LinkLabel linkLabel17;

		private CheckBox cbxTDSFollow;

		private LinkLabel linkLabel16;

		private CheckBox cbxTDSComment;

		private LinkLabel linkLabel9;

		private CheckBox cbxTDSTym;

		private CheckBox cbxPublic;

		private RadioButton cbxWifi;

		private LinkLabel lblViewKeyword;

		private CheckBox cbxKeyword;

		private Button btnReport;

		private Button button2;

		private MetroContextMenu metroContextMenu;

		private ToolStripMenuItem unlockHotmailToolStripMenuItem;

		private GroupBox gbAction;

		private GroupBox groupBox6;

		private GroupBox grDevice;

		private Button button4;

		private Button button3;

		private Button btnShow;

		private GroupBox groupBox3;

		private Button btnSearch;

		private TextBox txtKeyword;

		private ToolStripMenuItem changeCategory;

		private ToolStripMenuItem copyToolStripMenuItem;

		private ToolStripMenuItem copyUIDToolStripMenuItem;

		private ToolStripMenuItem copyPasswordToolStripMenuItem;

		private ToolStripMenuItem copyEmailToolStripMenuItem;

		private ToolStripMenuItem copyPasswordEmailToolStripMenuItem;

		private ToolStripMenuItem copyUIDPassEmailPassEmailDeviceInfoToolStripMenuItem1;

		private ToolStripMenuItem copyEmailPassEmailToolStripMenuItem1;

		private CheckBox cbxUpdate;

		private LinkLabel linkLabel19;

		private CheckBox cbxFollower;

		private LinkLabel linkLabel10;

		private CheckBox cbxFollowSuggested;

		private ToolStripMenuItem checkLiveToolStripMenuItem;

		private ToolStripMenuItem addNoteoolStripMenuItem;

		private ToolStripMenuItem accountToolStripMenuItem;

		private ToolStripMenuItem addToolStripMenuItem;

		private ToolStripMenuItem editToolStripMenuItem;

		private ToolStripMenuItem deleteToolStripMenuItem;

		private ToolStripMenuItem deleteBackupToolStripMenuItem;

		private ToolStripMenuItem loginbyhanTayToolStripMenuItem;

		private CheckBox cbxRecover;

		private ToolStripMenuItem loctrungToolStripMenuItem;

		private RadioButton rbtHMA;

		private LinkLabel linkLabel13;

		private CheckBox cbxEarnSu;

		private ToolStripMenuItem kiemTraBackupToolStripMenuItem;

		private CheckBox cbxAccept;

		private RadioButton rbtShopLive;

		private LinkLabel linkLabel20;

		private ToolStripMenuItem exportBackupAccountToolStripMenuItem;

		private ToolStripMenuItem refreshClearADBToolStripMenuItem;

		private ToolStripMenuItem copyUIDPassEmailPassEmailToolStripMenuItem;

		private CheckBox cbxRotate;

		private ToolStripStatusLabel lblRemain;

		private CheckBox cbxBalance;

		private CheckBox cbxChangeEmail;

		private LinkLabel linkLabel21;

		private CheckBox cbxVeri;

		private LinkLabel linkLabel22;

		private CheckBox cbxTreoNick;

		private LinkLabel linkLabel23;

		private ToolStripStatusLabel lblWorking;

		private LinkLabel lblEarnSu;

		private ToolStripMenuItem gawnsToolStripMenuItem;

		private ToolStripMenuItem xoasToolStripMenuItem;

		private ToolStripMenuItem mnuChiaThietbi;

		private ToolStripMenuItem chiabyhand;

		private RadioButton rbtProxyFB;

		private LinkLabel linkLabel25;

		private Button btnFilter;

		private ToolStripMenuItem xoaLogToolStripMenuItem;

		private LinkLabel linkLabel24;

		private ToolStripMenuItem xoaProxy;

		private CheckBox cbxRemoveAccount;

		private CheckBox cbxDie;

		private CheckBox cbxAddProduct;

		private ToolStripMenuItem cookiesToolStripMenuItem;

		private ToolStripMenuItem xemChromeToolStripMenuItem;

		private ToolStripMenuItem filterToolStripMenuItem;

		private ToolStripMenuItem notesToolStripMenuItem;

		private ToolStripMenuItem logToolStripMenuItem;

		private ToolStripMenuItem copyUIDPassToolStripMenuItem;

		public LoginItem AccLogin { get; set; }

		public bool RegGmail { get; set; }

		public string Session { get; set; }

		public frmMain()
		{
			InitializeComponent();
			AccLogin = new LoginItem();
			Session = "";
			RegGmail = false;
		}

		public frmMain(List<int> lst, int numberOfPhone = 0, bool RegGmail = false)
		{
			InitializeComponent();
			numberOfDevice = numberOfPhone;
			AccLogin = new LoginItem();
			toolStripStatusLabel1.Text = "Not Active";
			toolStripStatusLabel1.ForeColor = Color.Blue;
			this.RegGmail = RegGmail;
		}

		private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://www.youtube.com/channel/UCSgkqao1DfZHpXclBeeEpXw");
		}

		private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://www.facebook.com/groups/cachuake/");
		}

		private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://zalo.me/g/kfvcsm989");
		}

		private void CheckDeviceInfo()
		{
			try
			{
				sql.ExecuteQuery("Select * from tblPhones");
			}
			catch
			{
			}
		}

		private void GetMemory()
		{
			while (isRuning)
			{
				long privateMemorySize = Process.GetCurrentProcess().PrivateMemorySize64;
				long num = privateMemorySize / 1024L / 1024L;
				tssRam.Text = $"| Total Ram: {$"{num.ToString():###,###}"}Mb";
				lblRemain.Text = $"| Waiting to run: {$"{lstUid.Count.ToString():###,###}"}";
				TimeSpan timeSpan = DateTime.Now.Subtract(startRunningDate);
				int hours = timeSpan.Hours;
				int minutes = timeSpan.Minutes;
				int seconds = timeSpan.Seconds;
				lblWorking.Text = string.Format("| Working: {0}:{1}:{2}", (hours < 10) ? ("0" + hours) : hours.ToString(), (minutes < 10) ? ("0" + minutes) : minutes.ToString(), (seconds < 10) ? ("0" + seconds) : seconds.ToString());
				Thread.Sleep(10000);
				string text = ADBHelperCCK.RunCommand("adb devices");
				if (text.Contains("offline"))
				{
					for (int i = 0; i < 5; i++)
					{
						ADBHelperCCK.RunCommand("adb reconnect offline");
						Thread.Sleep(1000);
					}
				}
				if (!isRuning)
				{
					break;
				}
			}
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			Utils.UpdateDatabase();
			new Task(delegate
			{
				try
				{
					string text = Utils.ReadTextFile(Path.GetPathRoot(Environment.SystemDirectory) + "Windows\\System32\\drivers\\etc\\hosts");
					base.Visible = !text.Contains("cachuake.com") && !text.Contains("cck.vn");
					if (base.Visible)
					{
						string text2 = ADBHelperCCK.RunCommand("ping cachuake.com");
						base.Enabled = text2.Contains("124.158.7.175");
					}
				}
				catch
				{
				}
				GetMemory();
			}).Start();
			Control.CheckForIllegalCrossThreadCalls = false;
			Rectangle bounds = Screen.PrimaryScreen.Bounds;
			int num = bounds.Width;
			int num2 = bounds.Height;
			if (num > 1366)
			{
				if (num >= 1920)
				{
					MinimumSize = new Size(1920, 1080);
				}
				else
				{
					MinimumSize = new Size(num - 20, num2 - 50);
				}
				grDevice.Width = num - 1366 + 350;
				dataGridViewPhone.Width = num - 1366 + 330;
			}
			else
			{
				MinimumSize = new Size(1366, 768);
			}
			if (!File.Exists(string.Format("{0}\\DB\\Data\\DataTiktok{1}.db", SQLiteUtils.data_folder, DateTime.Now.ToString("ddMMyyyy"))))
			{
				File.Copy($"{SQLiteUtils.data_folder}\\DB\\Data\\DataTiktok.db", string.Format("{0}\\DB\\Data\\DataTiktok{1}.db", SQLiteUtils.data_folder, DateTime.Now.ToString("ddMMyyyy")));
				if (File.Exists(string.Format("{0}\\DB\\Data\\DataTiktok{1}.db", SQLiteUtils.data_folder, DateTime.Now.AddDays(-7.0).ToString("ddMMyyyy"))))
				{
					File.Delete(string.Format("{0}\\DB\\Data\\DataTiktok{1}.db", SQLiteUtils.data_folder, DateTime.Now.AddDays(-7.0).ToString("ddMMyyyy")));
				}
			}
			SumAccount();
			string mACAddress = new Utils().GetMACAddress();
			if (mACAddress != "")
			{
				lblKey.Text = "Code: CCKTiktok-" + mACAddress + " |";
				lblSub.Text = $"| {numberOfDevice} Phone";
				lblSub.ForeColor = Color.Blue;
			}
			foreach (Permission item in AccLogin.Role)
			{
				if (item.PermissionId == 4046)
				{
					toolStripStatusLabel1.Text = ((!(item.To > new DateTime(2050, 1, 1))) ? ("Expire: " + item.To.ToString("dd/MM/yyyy")) : (Utils.CurrentLang.Equals(CaChuaConstant.TIENGVIET) ? "Lifetime" : "Vĩnh viễn"));
					toolStripStatusLabel1.ForeColor = Color.Red;
				}
				if (item.PermissionId == 4047)
				{
					lblEarnSu.Visible = true;
					cbxEarnSu.Visible = true;
				}
			}
			groupBox6.Enabled = AccLogin.IsPermission(CaChuaConstant.ROLE);
			btn2fa.Visible = AccLogin.IsPermission(CaChuaConstant.ROLE);
			if (!AccLogin.IsPermission(CaChuaConstant.ROLE))
			{
				MessageBox.Show("Contact Admin (+84)904.868.545");
				return;
			}
			Text = string.Format("CCK Tiktok App V32 - Version: {0} - {1} ({2}) {3}", typeof(frmMain).Assembly.GetName().Version, AccLogin.Phone, AccLogin.Code, File.Exists(CaChuaConstant.HIDE_PHONE) ? "" : "-Hotline: 0904.868.545");
			if (Utils.ReadTextFile(CaChuaConstant.APP_PACKAGE) != "")
			{
				CaChuaConstant.PACKAGE_NAME = Utils.ReadTextFile(CaChuaConstant.APP_PACKAGE);
			}
			Control.CheckForIllegalCrossThreadCalls = false;
			LoadCategory();
			btnLoadDevices_Click(null, null);
			InitEnviroment(new List<string> { CaChuaConstant.ICON });
			if (File.Exists(CaChuaConstant.NETWORK))
			{
				switch (new JavaScriptSerializer().Deserialize<Networking>(Utils.ReadTextFile(CaChuaConstant.NETWORK)))
				{
				case Networking.Wifi:
					cbxWifi.Checked = true;
					break;
				case Networking.Proxy:
					rbtProxy.Checked = true;
					break;
				case Networking.XProxy_LAN:
					rbtXProxy.Checked = true;
					break;
				case Networking.ShopLive:
					rbtShopLive.Checked = true;
					break;
				case Networking.HMA:
					rbtHMA.Checked = true;
					break;
				case Networking.ProxyFB:
					rbtProxyFB.Checked = true;
					break;
				case Networking.TMProxy:
					rbtTM.Checked = true;
					break;
				case Networking._3G:
					rpt3g.Checked = true;
					break;
				}
			}
			LoadLabel();
			ShowChecked();
		}

		public void LoadLabel()
		{
			bool flag = Utils.CurrentLang.Equals(CaChuaConstant.ENGLISH);
			IEnumerable<Control> all = GetAll(this, typeof(LinkLabel));
			foreach (Control item in all)
			{
				RemoveUnderline((LinkLabel)item);
				if (item is LinkLabel && item.Tag != null && flag)
				{
					string tag = item.Text;
					item.Text = item.Tag.ToString();
					item.Tag = tag;
					tag = "";
				}
			}
			IEnumerable<Control> all2 = GetAll(this, typeof(CheckBox));
			foreach (Control item2 in all2)
			{
				if (item2 is CheckBox && item2.Tag != null && flag)
				{
					string tag2 = item2.Text;
					item2.Text = item2.Tag.ToString();
					item2.Tag = tag2;
					tag2 = "";
				}
			}
			changeCategory.Text = (flag ? "Change Category" : "Đổi thư mục");
			accountToolStripMenuItem.Text = (flag ? "Account" : "Tài khoản");
			addToolStripMenuItem.Text = (flag ? "Add Account" : "Thêm tài khoản");
			editToolStripMenuItem.Text = (flag ? "Edit Account" : "Sửa tải khoản");
			deleteToolStripMenuItem.Text = (flag ? "Delete Account" : "Xóa tài khoản");
			deleteBackupToolStripMenuItem.Text = (flag ? "Remove Backup" : "Xóa backup");
			loginbyhanTayToolStripMenuItem.Text = (flag ? "Login By Hand" : "Đăng nhập bằng tay");
			addNoteoolStripMenuItem.Text = (flag ? "Add Notes" : "Ghi chú");
			lblViewKeyword.Text = (flag ? "View by keywords" : "Tương tác theo từ khóa");
			try
			{
				new SQLiteUtils().CheckTableExists("TDSConfig", "CREATE TABLE \"TDSConfig\"(\"username\" TEXT, \"password\" TEXT, \"token\" TEXT, \"device_id\" TEXT);");
			}
			catch
			{
			}
		}

		public void RemoveUnderline(LinkLabel link)
		{
			link.LinkBehavior = LinkBehavior.NeverUnderline;
			link.MouseHover += link_MouseHover;
			link.MouseLeave += link_MouseLeave;
		}

		private void link_MouseLeave(object sender, EventArgs e)
		{
			((LinkLabel)sender).LinkBehavior = LinkBehavior.HoverUnderline;
		}

		private void link_MouseHover(object sender, EventArgs e)
		{
			((LinkLabel)sender).LinkBehavior = LinkBehavior.NeverUnderline;
		}

		public IEnumerable<Control> GetAll(Control control, Type type)
		{
			IEnumerable<Control> enumerable = control.Controls.Cast<Control>();
			return from c in enumerable.SelectMany((Control ctrl) => GetAll(ctrl, type)).Concat(enumerable)
				where c.GetType() == type
				select c;
		}

		private void LoadDevices()
		{
			try
			{
				dicDevices.Clear();
				DataTable dataTable = sql.ExecuteQuery("Select deviceId,Name from tblDevices");
				if (dataTable != null)
				{
					foreach (DataRow row in dataTable.Rows)
					{
						string key = row["deviceId"].ToString();
						string value = row["name"].ToString();
						if (!dicDevices.ContainsKey(key))
						{
							dicDevices.Add(key, value);
						}
					}
				}
				sql.Dispose();
			}
			catch
			{
			}
		}

		private void btnLoadDevices_Click(object sender, EventArgs e)
		{
			Task task = new Task(delegate
			{
				try
				{
					if (File.Exists(CaChuaConstant.FIX_DEVICE) && File.Exists(CaChuaConstant.WIFI_MODE) && Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.WIFI_MODE)))
					{
						Utils.ReconnectFixDevice();
					}
					LoadDevices();
					Dictionary<string, string> listSerialDeviceWithName = ADBHelperCCK.GetListSerialDeviceWithName();
					DataTable dataTable = new DataTable();
					if (sql == null)
					{
						sql = new SQLiteUtils();
					}
					dataTable = sql.ExecuteQuery("select  device , count(device) as [Sum] from Account where length(device) > 0 group by device");
					if (dataTable != null)
					{
						dicDevicesName.Clear();
						foreach (DataRow row in dataTable.Rows)
						{
							if (row["device"].ToString() != "")
							{
								dicDevicesName.Add(row["device"].ToString(), Utils.Convert2Int(row["Sum"].ToString()));
							}
						}
					}
					DataTable dataTable2 = new DataTable
					{
						Columns = { "Stt", "Name", "Phone", "Sum", "Account", "Status", "Port", "SystemPort" }
					};
					int num = 4732;
					if (File.Exists(CaChuaConstant.STARTPORT))
					{
						num = Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.STARTPORT));
					}
					if (num < 4723)
					{
						num = 4723;
					}
					List<string> list = new List<string>();
					deviceInfo = new Dictionary<string, int>();
					int num2 = 0;
					foreach (KeyValuePair<string, string> s in listSerialDeviceWithName)
					{
						DataRow dataRow2 = dataTable2.NewRow();
						int num3 = ++num2;
						dataRow2["Stt"] = num3;
						dataRow2["Account"] = "";
						dataRow2["Sum"] = (dicDevicesName.ContainsKey(s.Key) ? dicDevicesName[s.Key] : 0);
						dataRow2["Phone"] = s.Key;
						if (!deviceInfo.ContainsKey(s.Key))
						{
							deviceInfo.Add(s.Key, num3 - 1);
						}
						dataRow2["Name"] = (dicDevices.ContainsKey(s.Key) ? dicDevices[s.Key] : s.Value);
						new Task(delegate
						{
							ADBHelperCCK.SetPortrait(s.Key);
						}).Start();
						dataRow2["Status"] = "Not Root";
						int num4 = num + num2;
						dataRow2["Port"] = num4;
						dataRow2["SystemPort"] = 8200 + num2;
						if (num2 <= numberOfDevice)
						{
							dataTable2.Rows.Add(dataRow2);
							list.Add(s.Key);
						}
					}
					dataTable2.AcceptChanges();
					lock (this)
					{
						File.WriteAllLines(CaChuaConstant.DEVICE_LIST, list);
						list = null;
						DataView defaultView = dataTable2.DefaultView;
						defaultView.Sort = "Name asc";
						dataTable2 = defaultView.ToTable();
						for (int i = 0; i < dataTable2.Rows.Count; i++)
						{
							dataTable2.Rows[i]["Stt"] = i + 1;
						}
						dataTable2.AcceptChanges();
						int num5 = base.Width;
						int left = grDevice.Left;
						grDevice.Width = num5 - left;
						dataGridViewPhone.Width = num5 - left - 20;
						dataGridViewPhone.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
						BindObjectList(dataGridViewPhone, dataTable2);
						dataTable2.Dispose();
						dataGridViewPhone.RowHeadersVisible = false;
						dataGridViewPhone.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
						dataGridViewPhone.Columns[0].Width = 25;
						dataGridViewPhone.Columns[1].Width = 80;
						dataGridViewPhone.Columns[2].Width = 100;
						dataGridViewPhone.Columns[7].Visible = false;
						dataGridViewPhone.Columns[6].Visible = false;
					}
				}
				catch (Exception)
				{
				}
			});
			task.Start();
		}

		private void LoadDeviceByFilter(List<string> lst)
		{
			Task task = new Task(delegate
			{
				try
				{
					LoadDevices();
					Dictionary<string, string> listSerialDeviceWithName = ADBHelperCCK.GetListSerialDeviceWithName();
					DataTable dataTable = new DataTable();
					if (sql == null)
					{
						sql = new SQLiteUtils();
					}
					dataTable = sql.ExecuteQuery("select  device , count(device) as [Sum] from Account where length(device) > 0 group by device");
					if (dataTable != null)
					{
						dicDevicesName.Clear();
						foreach (DataRow row in dataTable.Rows)
						{
							if (row["device"].ToString() != "")
							{
								dicDevicesName.Add(row["device"].ToString(), Utils.Convert2Int(row["Sum"].ToString()));
							}
						}
					}
					DataTable dataTable2 = new DataTable
					{
						Columns = { "Stt", "Name", "Phone", "Sum", "Account", "Status", "Port", "SystemPort" }
					};
					int num = 4732;
					if (File.Exists(CaChuaConstant.STARTPORT))
					{
						num = Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.STARTPORT));
					}
					if (num < 4723)
					{
						num = 4723;
					}
					List<string> list = new List<string>();
					deviceInfo = new Dictionary<string, int>();
					int num2 = 0;
					foreach (KeyValuePair<string, string> s in listSerialDeviceWithName)
					{
						DataRow dataRow2 = dataTable2.NewRow();
						int num3 = ++num2;
						dataRow2["Stt"] = num3;
						dataRow2["Account"] = "";
						dataRow2["Sum"] = (dicDevicesName.ContainsKey(s.Key) ? dicDevicesName[s.Key] : 0);
						dataRow2["Phone"] = s.Key;
						if (!deviceInfo.ContainsKey(s.Key))
						{
							deviceInfo.Add(s.Key, num3 - 1);
						}
						dataRow2["Name"] = (dicDevices.ContainsKey(s.Key) ? dicDevices[s.Key] : s.Value);
						bool flag = ADBHelperCCK.IsRooted(s.Key);
						new Task(delegate
						{
							ADBHelperCCK.SetPortrait(s.Key);
						}).Start();
						dataRow2["Status"] = ((!flag) ? "Not Root" : "Rooted");
						int num4 = num + num2;
						dataRow2["Port"] = num4;
						dataRow2["SystemPort"] = 8200 + num2;
						if (num2 <= numberOfDevice && lst.Contains(s.Key))
						{
							dataTable2.Rows.Add(dataRow2);
							list.Add(s.Key);
						}
					}
					dataTable2.AcceptChanges();
					lock (dataGridViewPhone)
					{
						File.WriteAllLines(CaChuaConstant.DEVICE_LIST, list);
						list = null;
						DataView defaultView = dataTable2.DefaultView;
						defaultView.Sort = "Name asc";
						dataTable2 = defaultView.ToTable();
						for (int i = 0; i < dataTable2.Rows.Count; i++)
						{
							dataTable2.Rows[i]["Stt"] = i + 1;
						}
						dataTable2.AcceptChanges();
						DateTime now = DateTime.Now;
						BindObjectList(dataGridViewPhone, dataTable2);
						_ = DateTime.Now.Subtract(now).TotalMilliseconds;
						dataTable2.Dispose();
						dataGridViewPhone.RowHeadersVisible = false;
						dataGridViewPhone.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
						dataGridViewPhone.Columns[0].Width = 25;
						dataGridViewPhone.Columns[1].Width = 80;
						dataGridViewPhone.Columns[2].Width = 100;
						dataGridViewPhone.Columns[7].Visible = false;
						dataGridViewPhone.Columns[6].Visible = false;
					}
				}
				catch (Exception)
				{
				}
			});
			task.Start();
		}

		private void SumAccount()
		{
			try
			{
				new Task(delegate
				{
					DataTable dataTable = sql.ExecuteQuery("select trangthai,count(trangthai) as c from Account group by trangthai");
					string text = "";
					if (dataTable != null)
					{
						foreach (DataRow row in dataTable.Rows)
						{
							text += string.Format(" {0}: {1} -", row["trangthai"], row["c"]);
							if (row["trangthai"].ToString().Equals("New"))
							{
								if (sql == null)
								{
									sql = new SQLiteUtils();
								}
								sql.ExecuteQuery("Update Account set trangthai='Live' where trangthai='New';Update Account set trangthai='Error' where trangthai <> 'Live'");
							}
						}
					}
					toolStripStatusLabel2.Text = text.TrimEnd('-');
				}).Start();
			}
			catch
			{
			}
		}

		private static void InitEnviroment(List<string> lst)
		{
			string systemVaraiable = ADBHelperCCK.GetSystemVaraiable("ANDROID_HOME");
			if (string.IsNullOrWhiteSpace(systemVaraiable))
			{
				ADBHelperCCK.SetSystemVaraiable("ANDROID_HOME", Application.StartupPath + "\\Config\\Sdk");
			}
			string systemVaraiable2 = ADBHelperCCK.GetSystemVaraiable("JAVA_HOME");
			if (string.IsNullOrWhiteSpace(systemVaraiable2))
			{
				ADBHelperCCK.SetSystemVaraiable("JAVA_HOME", Application.StartupPath + "\\Config\\Jre");
			}
			foreach (string item in lst)
			{
				if (!File.Exists(Application.StartupPath + "\\" + item))
				{
					using WebClient webClient = new WebClient();
					webClient.DownloadFile("http://cachuake.com/Download/Utils/" + item.Split('\\')[1], Application.StartupPath + "\\" + item);
				}
			}
		}

		private void LoadCategory(string muc = "")
		{
			try
			{
				DataTable dataTable = new SQLiteUtils().ExecuteQuery("Select tendanhmuc,id_danhmuc from DanhMuc  order by tendanhmuc asc");
				new List<Category>();
				cbxCategory.Items.Clear();
				cbxCategory.DisplayMember = "tendanhmuc";
				cbxCategory.ValueMember = "id_danhmuc";
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					cbxCategory.Items.Add(new Category
					{
						id_danhmuc = Utils.Convert2Int(dataTable.Rows[i]["id_danhmuc"]),
						tendanhmuc = dataTable.Rows[i]["tendanhmuc"].ToString()
					});
				}
				if (File.Exists(CaChuaConstant.SELECTED_CATEGORY))
				{
					List<string> list = new JavaScriptSerializer().Deserialize<List<string>>(Utils.ReadTextFile(CaChuaConstant.SELECTED_CATEGORY));
					if (list == null || list.Count <= 0)
					{
						return;
					}
					for (int j = 0; j < cbxCategory.Items.Count; j++)
					{
						string text = ((Category)cbxCategory.Items[j]).tendanhmuc.ToString();
						if (list.Contains(text))
						{
							int index = cbxCategory.FindString(text);
							cbxCategory.SetItemChecked(index, value: true);
							cbxCategory.SetSelected(index, value: true);
						}
					}
					return;
				}
				cbxCategory.SetItemChecked(cbxCategory.Items.Count - 1, value: true);
				cbxCategory.SetSelected(cbxCategory.Items.Count - 1, value: true);
			}
			catch (Exception ex)
			{
				Utils.CCKLog("TaskList", Environment.NewLine + ex.Message, "LoadCategory");
			}
		}

		private void groupBox3_Enter(object sender, EventArgs e)
		{
		}

		private void GetChecked()
		{
			List<GroupBox> list = new List<GroupBox>();
			list.Add(groupAction);
			list.Add(groupProfile);
			list.Add(groupShop);
			List<string> list2 = new List<string>();
			foreach (GroupBox item in list)
			{
				foreach (Control control in item.Controls)
				{
					if (control is CheckBox && ((CheckBox)control).Checked)
					{
						list2.Add(control.Name);
					}
				}
			}
			File.WriteAllText(CaChuaConstant.WORK, new JavaScriptSerializer().Serialize(list2));
		}

		private void ShowChecked()
		{
			if (!File.Exists(CaChuaConstant.WORK))
			{
				return;
			}
			List<GroupBox> list = new List<GroupBox>();
			list.Add(groupAction);
			list.Add(groupProfile);
			list.Add(groupShop);
			List<string> list2 = new JavaScriptSerializer().Deserialize<List<string>>(Utils.ReadTextFile(CaChuaConstant.WORK));
			foreach (GroupBox item in list)
			{
				foreach (Control control in item.Controls)
				{
					if (control is CheckBox && list2.Contains(control.Name))
					{
						((CheckBox)control).Checked = true;
					}
				}
			}
		}

		private void btn2fa_Click(object sender, EventArgs e)
		{
			if (Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.SCHEDULE)))
			{
				frmSchedule frmSchedule = new frmSchedule();
				frmSchedule.StartPosition = FormStartPosition.CenterParent;
				frmSchedule.ShowDialog();
				if (frmSchedule.RunTime.Year - DateTime.Now.Year == 100)
				{
					button = ActionButton.Start;
					btn2fa.Text = "Start";
					return;
				}
			}
			GetChecked();
			btnSave_Click_1(null, null);
			if (!isTuongTacClick)
			{
				isTuongTacClick = true;
				isRuning = true;
				MainAction();
			}
			else
			{
				MessageBox.Show("Running ...");
			}
		}

		public Networking GetCurrentNetWork()
		{
			Networking result = Networking._3G;
			if (rbtProxy.Checked)
			{
				result = Networking.Proxy;
			}
			else if (rpt3g.Checked)
			{
				result = Networking._3G;
			}
			else if (cbxWifi.Checked)
			{
				result = Networking.Wifi;
			}
			else if (rbtXProxy.Checked)
			{
				result = Networking.XProxy_LAN;
			}
			else if (rbtTM.Checked)
			{
				result = Networking.TMProxy;
			}
			else if (rbtShopLive.Checked)
			{
				result = Networking.ShopLive;
			}
			else if (!rbtHMA.Checked)
			{
				if (rbtProxyFB.Checked)
				{
					result = Networking.ProxyFB;
				}
			}
			else
			{
				result = Networking.HMA;
			}
			return result;
		}

		private void RunRegCrome(DeviceEntity device)
		{
			string SerialNo = device.DeviceId;
			Task.Run(delegate
			{
				AndroidDriver<AndroidElement> androidDriver;
				do
				{
					androidDriver = ADBHelperCCK.StartPhone(device);
				}
				while (androidDriver == null);
				ChangeGridViewDevice(SerialNo, "Server started : " + DateTime.Now.ToString(), Color.Green);
				while (isRuning)
				{
					try
					{
						TiktokItem tiktokItem = new TiktokItem(androidDriver, device, Guid.NewGuid().ToString(), dataGridViewPhone, Session, null);
						tiktokItem.RegisterAccountChrome(this);
					}
					catch (Exception ex)
					{
						File.WriteAllText("log.txt", ex.Message);
					}
				}
			}).ContinueWith(delegate
			{
				ChangeGridViewDevice(SerialNo, (device.Message != "") ? device.Message : "Done", Color.Green);
			});
		}

		private void RunReg(DeviceEntity device, int index)
		{
			try
			{
				string SerialNo = device.DeviceId;
				if (!ADBHelperCCK.IsInstallApp(SerialNo, "com.google.android.gm"))
				{
					ChangeGridViewDevice(SerialNo, "Chưa cài App GMail", Color.Red);
					return;
				}
				if (!ADBHelperCCK.IsInstallApp(SerialNo, CaChuaConstant.PACKAGE_NAME))
				{
					ChangeGridViewDevice(SerialNo, "Chưa cài App Tiktok", Color.Red);
					return;
				}
				string tiktokVersion = ADBHelperCCK.GetTiktokVersion(SerialNo);
				bool flag = Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.CHECK_VERSION_APP));
				if (!tiktokVersion.Contains("32.9") && !flag)
				{
					ADBHelperCCK.UnInstallApp(SerialNo, CaChuaConstant.PACKAGE_NAME);
					string text = "TikTok_32.9.apk";
					if (!File.Exists("Devices\\" + text))
					{
						frmDownload frmDownload = new frmDownload();
						frmDownload.Download("https://cck.vn/Download/Utils/" + text + ".rar", Application.StartupPath + "\\Devices\\" + text);
						frmDownload.ShowDialog();
						if (frmDownload.DownloadCompleted)
						{
							Thread.Sleep(1000);
						}
					}
					if (File.Exists("Devices\\" + text))
					{
						ADBHelperCCK.InstallApp(SerialNo, Application.StartupPath + "\\Devices\\" + text);
					}
				}
				ADBHelperCCK.ExecuteCMD(SerialNo, "shell settings put global heads_up_notifications_enabled 0");
				ADBHelperCCK.ExecuteCMD(SerialNo, "shell pm clear com.android.providers.contacts");
				ADBHelperCCK.TurnOnScreen(device.DeviceId);
				Task.Run(delegate
				{
					device.Version = ADBHelperCCK.GetAndroidVersion(SerialNo);
					ChangeGridViewDevice(SerialNo, "Start Server", Color.Green);
					AndroidDriver<AndroidElement> androidDriver;
					do
					{
						androidDriver = ADBHelperCCK.StartPhone(device);
					}
					while (androidDriver == null);
					ChangeGridViewDevice(SerialNo, "Server started", Color.Green);
					RegNickEntity regNickEntity = new RegNickEntity();
					if (File.Exists(CaChuaConstant.REG_CONFIG))
					{
						regNickEntity = new JavaScriptSerializer().Deserialize<RegNickEntity>(Utils.ReadTextFile(CaChuaConstant.REG_CONFIG));
					}
					int num = 0;
					for (int i = 0; i < cbxCategory.Items.Count; i++)
					{
						if (cbxCategory.GetItemChecked(i))
						{
							num = Utils.Convert2Int(((Category)cbxCategory.Items[i]).id_danhmuc.ToString());
							break;
						}
					}
					if (num == 0)
					{
						num = Utils.Convert2Int(((Category)cbxCategory.Items[0]).id_danhmuc.ToString());
					}
					while (isRuning)
					{
						try
						{
							string text2 = Utils.ReadTextFile(CaChuaConstant.REG_APP_NAME);
							TiktokItem tiktokItem = new TiktokItem(androidDriver, device, Guid.NewGuid().ToString(), dataGridViewPhone, Session, null);
							tiktokItem.ProcessRootPhone(GetCurrentNetWork(), text2 == "TiktokLite", dataGridViewPhone, this);
							ADBHelperCCK.SetPortrait(device.DeviceId);
							bool flag2 = true;
							if (regNickEntity.RegType == RegAccountType.Email)
							{
								EmailRegisterType emailRegisterType = ((!File.Exists(CaChuaConstant.VERI_TYPE)) ? EmailRegisterType.MaxClone : new JavaScriptSerializer
								{
									MaxJsonLength = int.MaxValue
								}.Deserialize<EmailRegisterType>(File.ReadAllText(CaChuaConstant.VERI_TYPE)));
								if (emailRegisterType == EmailRegisterType.GoogleAppDomain)
								{
									if (text2 == "TiktokLite")
									{
										try
										{
											flag2 = tiktokItem.RegisterTiktokLiteByGmailApp(dataGridViewPhone, this, num, regNickEntity);
											if (!tiktokItem.RunNextAccount)
											{
												return;
											}
										}
										catch (Exception ex2)
										{
											Utils.CCKLog(ex2.Message, "Reg - TiktokLite");
										}
									}
									else
									{
										try
										{
											flag2 = tiktokItem.RegisterMemoTiktokLiteByGmailS7CHplay(dataGridViewPhone, this, num, regNickEntity);
											if (!tiktokItem.RunNextAccount)
											{
												return;
											}
										}
										catch (Exception ex3)
										{
											Utils.CCKLog(ex3.Message, "Reg - Tiktok V32 Google");
										}
									}
								}
								else
								{
									try
									{
										flag2 = tiktokItem.RegisterMemoTiktokByEmail(dataGridViewPhone, this, num, regNickEntity);
									}
									catch (Exception ex4)
									{
										Utils.CCKLog(ex4.Message, "Reg - Tiktok V32 Reg Thường");
									}
								}
							}
							else if (regNickEntity.RegType == RegAccountType.Phone)
							{
								flag2 = tiktokItem.RegisterMemoTiktokByPhone(dataGridViewPhone, this, num, regNickEntity);
							}
							if (!tiktokItem.RunNextAccount)
							{
								return;
							}
							tiktokItem.Info.LoggedIn = flag2;
							if (flag2)
							{
								goto IL_0421;
							}
							if (regNickEntity.ErrorStop)
							{
								return;
							}
							if (!regNickEntity.ErrorContinue)
							{
								goto IL_0421;
							}
							for (int j = 0; j < regNickEntity.ErrorDelay; j += 15)
							{
								Thread.Sleep(15000);
								ADBHelperCCK.ShowTextMessageOnPhone(device.DeviceId, $"Đang chờ Reg tiếp --> còn {regNickEntity.ErrorDelay - j} giây");
							}
							goto end_IL_017c;
							IL_0421:
							List<string> list = new List<string>();
							if (cbxRemoveAccount.Checked)
							{
								list.Add("cbxRemoveAccount");
							}
							if (cbxTreoNick.Checked)
							{
								list.Add("cbxTreoNick");
							}
							if (cbxNewsFeed.Checked)
							{
								list.Add("cbxNewsFeed");
							}
							if (cbxAddProduct.Checked)
							{
								list.Add("cbxAddProduct");
							}
							if (cbxAccept.Checked)
							{
								list.Add("cbxAccept");
							}
							if (cbxEarnSu.Checked)
							{
								list.Add("cbxEarnSu");
							}
							if (cbxUpdate.Checked)
							{
								list.Add("cbxUpdate");
							}
							if (cbxBalance.Checked)
							{
								list.Add("cbxBalance");
							}
							if (cbxKeyword.Checked)
							{
								list.Add("cbxKeyword");
							}
							if (cbxPublic.Checked)
							{
								list.Add("cbxPublic");
							}
							if (cbxTDSTym.Checked)
							{
								list.Add("cbxTDSTym");
							}
							if (cbxTDSComment.Checked)
							{
								list.Add("cbxTDSComment");
							}
							if (cbxTDSFollow.Checked)
							{
								list.Add("cbxTDSFollow");
							}
							if (cbxLike.Checked)
							{
								list.Add("cbxLike");
							}
							if (cbxCommentRandom.Checked)
							{
								list.Add("cbxCommentRandom");
							}
							if (cbxFolowUID.Checked)
							{
								list.Add("cbxFolowUID");
							}
							if (cbxSeedingVideo.Checked)
							{
								list.Add("cbxSeedingVideo");
							}
							if (cbxUpVideo.Checked)
							{
								list.Add("cbxUpVideo");
							}
							if (cbxChangeAvatar.Checked)
							{
								list.Add("cbxChangeAvatar");
							}
							if (cbxChangePass.Checked)
							{
								list.Add("cbxChangePass");
							}
							if (cbxBio.Checked)
							{
								list.Add("cbxBio");
							}
							if (cbxRename.Checked)
							{
								list.Add("cbxRename");
							}
							if (cbxFollower.Checked)
							{
								list.Add("cbxFollower");
							}
							if (cbxFollowSuggested.Checked)
							{
								list.Add("cbxFollowSuggested");
							}
							if (cbxChangeEmail.Checked)
							{
								list.Add("cbxChangeEmail");
							}
							if (cbxVeri.Checked)
							{
								list.Add("cbxVeri");
							}
							try
							{
								while (list.Count > 0 && tiktokItem.Info.LoggedIn)
								{
									string text3 = list[0];
									new List<string>();
									list.RemoveAt(0);
									tiktokItem.GetPageSource();
									switch (text3)
									{
									case "cbxUpdate":
										tiktokItem.UpdateProfileInfo();
										break;
									case "cbxTDSTym":
										tiktokItem.TDSTym();
										break;
									case "cbxBio":
										tiktokItem.ChangeBio();
										break;
									case "cbxCommentRandom":
										tiktokItem.CommentOnPost();
										break;
									case "cbxKeyword":
										tiktokItem.SeachKeyword();
										break;
									case "cbxFollower":
										tiktokItem.FollowByFollower();
										break;
									case "cbxFolowUID":
										tiktokItem.FollowUID();
										break;
									case "cbxSeedingVideo":
										tiktokItem.SeedingVideo();
										break;
									case "cbxTDSFollow":
										tiktokItem.TDSFollow();
										break;
									case "cbxChangeEmail":
										tiktokItem.ChangeEmail();
										break;
									case "cbxBalance":
										tiktokItem.Promotion();
										break;
									case "cbxPublic":
										tiktokItem.PublicInfo();
										break;
									case "cbxLike":
										tiktokItem.LikePost();
										break;
									case "cbxNewsFeed":
										tiktokItem.ViewNewsFeed();
										break;
									case "cbxFollowSuggested":
										tiktokItem.FollowBySuggested();
										break;
									case "cbxUpVideo":
										tiktokItem.Upvideo();
										break;
									case "cbxRename":
										tiktokItem.Rename();
										tiktokItem.AddLogByAction("Đổi tên");
										break;
									case "cbxAddProduct":
										tiktokItem.AddProduct();
										break;
									case "cbxEarnSu":
										tiktokItem.KiemXu();
										break;
									case "cbxVeri":
										tiktokItem.ChangeEmail(change: false);
										break;
									case "cbxChangeAvatar":
										tiktokItem.ChangeAvatar();
										break;
									case "cbxTDSComment":
										tiktokItem.TDSComment();
										break;
									case "cbxChangePass":
										tiktokItem.ChangePass();
										break;
									case "cbx2fa":
										tiktokItem.Change2FA();
										break;
									case "cbxFollowCheo":
										tiktokItem.FollowCheo();
										break;
									case "cbxAccept":
										tiktokItem.AcceptShop();
										break;
									}
								}
							}
							catch (Exception ex5)
							{
								Utils.CCKLog(ex5.Message, "Changne Info after register");
							}
							end_IL_017c:;
						}
						catch (Exception ex6)
						{
							Utils.CCKLog(ex6.Message, "Loi khi dang ky nick");
						}
						finally
						{
							if (DicTmProxy.ContainsKey(device.DeviceId))
							{
								TMProxy tMProxy = new TMProxy(DicTmProxy[device.DeviceId], device.DeviceId);
								tMProxy.RemoveProxy();
								DicTmProxy.Remove(device.DeviceId);
								tMProxy.Dispose();
							}
						}
					}
				}).ContinueWith(delegate
				{
					ChangeGridViewDevice(SerialNo, (device.Message != "") ? device.Message : "Done", Color.Green);
					ADBHelperCCK.TurnScreenOffDevice(SerialNo);
				});
			}
			catch (Exception ex)
			{
				DicTmProxy.Remove(device.DeviceId);
				if (!Directory.Exists("Log"))
				{
					Directory.CreateDirectory("Log");
				}
				Utils.CCKLog("Log\\run.txt", ex.Message);
			}
		}

		private void ShowGridMessage(DataGridViewRow row, string message, string name = "Log")
		{
			if (row != null)
			{
				try
				{
					row.Cells[name].Value = message;
				}
				catch
				{
				}
			}
		}

		private void Run(DeviceEntity device, bool loginOnly = false)
		{
			try
			{
				string text = Utils.ReadTextFile(CaChuaConstant.PHONE_MODE_DATA);
				PhoneMode phoneMode = ((text != "") ? new JavaScriptSerializer().Deserialize<PhoneMode>(text) : PhoneMode.NonRoot);
				device.Rooted = text != "" && (phoneMode == PhoneMode.Root || phoneMode == PhoneMode.TWRP);
				bool rootedPhone = ADBHelperCCK.IsRooted(device.DeviceId);
				ADBHelperCCK.KillProcessByPort(device.SystemPort);
				ADBHelperCCK.KillProcessByPort(device.Port);
				string SerialNo = device.DeviceId;
				new Task(delegate
				{
					ADBHelperCCK.ExecuteCMD(SerialNo, "shell settings put global heads_up_notifications_enabled 0");
					ADBHelperCCK.ExecuteCMD(SerialNo, "shell pm clear com.android.providers.contacts");
					ADBHelperCCK.TurnOnScreen(device.DeviceId);
				}).Start();
				Task.Run(delegate
				{
					device.Version = ADBHelperCCK.GetAndroidVersion(SerialNo);
					ChangeGridViewDevice(SerialNo, "Start Server", Color.Green);
					AndroidDriver<AndroidElement> androidDriver;
					do
					{
						androidDriver = ADBHelperCCK.StartPhone(device);
					}
					while (androidDriver == null);
					ChangeGridViewDevice(SerialNo, "Server started", Color.Green);
					while (isRuning)
					{
						try
						{
							if (lstUid == null || lstUid.Count <= 0)
							{
								return;
							}
							string text2 = "";
							DataGridViewRow row = new DataGridViewRow();
							if (rootedPhone)
							{
								if (!lstUid.ContainsKey("cck") || lstUid["cck"].Count <= 0)
								{
									return;
								}
								text2 = lstUid["cck"][0];
								lstUid["cck"].RemoveAt(0);
								row = (dataGridViewIndexByUid.ContainsKey(text2) ? dataGridViewIndexByUid[text2] : null);
								if (cbxRotate.Checked && lstUid.ContainsKey("cck"))
								{
									lstUid["cck"].Add(text2);
								}
								if (text2 == "")
								{
									return;
								}
							}
							else
							{
								if (lstUid.ContainsKey(device.DeviceId) && lstUid[device.DeviceId].Count > 0)
								{
									text2 = lstUid[device.DeviceId][0];
									lstUid[device.DeviceId].RemoveAt(0);
									if (text2 == "" && lstUid.ContainsKey("cck") && lstUid["cck"].Count > 0)
									{
										text2 = lstUid["cck"][0];
										lstUid["cck"].RemoveAt(0);
										sql.ExecuteQuery($"Update Account set device = '{SerialNo}' where id='{text2}'");
									}
									if (text2 == "")
									{
										return;
									}
									row = (dataGridViewIndexByUid.ContainsKey(text2) ? dataGridViewIndexByUid[text2] : null);
									if (cbxRotate.Checked && lstUid.ContainsKey(device.DeviceId))
									{
										lstUid[device.DeviceId].Add(text2);
									}
								}
								else if (lstUid.ContainsKey(device.DeviceId) && lstUid[device.DeviceId].Count == 0)
								{
									return;
								}
								if (phoneMode == PhoneMode.TWRP)
								{
									device.Rooted = true;
									ADBHelperCCK.ClearAppData(device.DeviceId, CaChuaConstant.PACKAGE_NAME);
								}
							}
							ChangeGridViewDevice(SerialNo, text2, Color.Green, "Phone", "Account");
							TiktokItem tiktokItem = new TiktokItem(androidDriver, device, text2, dataGridViewPhone, Session, row);
							ChangeGridViewDevice(SerialNo, "Change Device", Color.Green);
							_ = DateTime.Now;
							ShowGridMessage(row, "Bắt đầu đổi thông tin thiết bị");
							Networking currentNetWork = GetCurrentNetWork();
							tiktokItem.ProcessRootPhone(currentNetWork, appLite: false, dataGridViewPhone, this);
							ChangeGridViewDevice(SerialNo, "", Color.Green);
							ShowGridMessage(row, "Bắt đầu đăng nhập");
							string outMessage = "";
							bool flag = false;
							if (device.Rooted)
							{
								flag = tiktokItem.Login(out outMessage, cbxRecover.Checked);
								ShowGridMessage(row, outMessage);
								goto IL_07dd;
							}
							if (phoneMode != PhoneMode.NonRoot)
							{
								if (phoneMode == PhoneMode.TWRP)
								{
									string path = $"{Application.StartupPath}\\Authentication\\{tiktokItem.Info.Uid}\\";
									if (Directory.Exists(path))
									{
										DateTime now = DateTime.Now;
										ADBHelperCCK.ClearAppData(device.DeviceId, CaChuaConstant.PACKAGE_NAME);
										ADBHelperCCK.OpenApp(device.DeviceId, CaChuaConstant.PACKAGE_NAME);
										Thread.Sleep(5000);
										tiktokItem.RestoreAccountTiktok(restoreOnly: true);
										ADBHelperCCK.OpenApp(device.DeviceId, CaChuaConstant.PACKAGE_NAME);
										_ = DateTime.Now.Subtract(now).TotalSeconds;
									}
									if (!(flag = tiktokItem.IsLogin()) && (flag = tiktokItem.Login(out outMessage, cbxRecover.Checked)))
									{
										device.Rooted = true;
										tiktokItem.BackupAccountTiktok();
										device.Rooted = false;
										ADBHelperCCK.OpenApp(device.DeviceId, CaChuaConstant.PACKAGE_NAME);
									}
									Thread.Sleep(5000);
								}
								goto IL_07dd;
							}
							SwitchResult switchResult = tiktokItem.SwitchAccount();
							if (switchResult != SwitchResult.Full)
							{
								switch (switchResult)
								{
								case SwitchResult.Success:
									flag = true;
									ShowGridMessage(row, "Đăng nhập thành công");
									break;
								case SwitchResult.Error:
								case SwitchResult.LogIn:
									flag = tiktokItem.Login(out outMessage, cbxRecover.Checked);
									break;
								}
								goto IL_07dd;
							}
							ShowGridMessage(row, "Máy đã đủ 8 nick");
							goto end_IL_0099;
							IL_07dd:
							if (flag)
							{
								ShowGridMessage(row, outMessage);
								tiktokItem.Info.LoggedIn = true;
								tiktokItem.BackupAccountTiktok();
								if (loginOnly)
								{
									return;
								}
								if (tiktokItem.IsLogin())
								{
									if (!flag)
									{
										tiktokItem.AddLogByAction(outMessage);
										ShowGridMessage(row, outMessage);
									}
									else
									{
										ShowGridMessage(row, "Đăng nhập thành công");
										tiktokItem.AddLogByAction("Đăng nhập thành công", isAppend: false);
										ADBHelperCCK.OpenApp(device.DeviceId, CaChuaConstant.PACKAGE_NAME);
										ChangeGridViewDevice(SerialNo, "Logged in", Color.Green);
										DateTime now2 = DateTime.Now;
										List<string> list = new List<string>();
										string firstItemFromFile = Utils.GetFirstItemFromFile(CaChuaConstant.API_TDS, remove: false);
										if (!TDSAPI.ContainsKey(SerialNo) && firstItemFromFile != "")
										{
											TDSAPI.Add(SerialNo, firstItemFromFile);
										}
										if (cbxRemoveAccount.Checked)
										{
											list.Add("cbxRemoveAccount");
										}
										if (cbxTreoNick.Checked)
										{
											list.Add("cbxTreoNick");
										}
										if (cbxNewsFeed.Checked)
										{
											list.Add("cbxNewsFeed");
										}
										if (cbxAddProduct.Checked)
										{
											list.Add("cbxAddProduct");
										}
										if (cbxAccept.Checked)
										{
											list.Add("cbxAccept");
										}
										if (cbxEarnSu.Checked)
										{
											list.Add("cbxEarnSu");
										}
										if (cbxUpdate.Checked)
										{
											list.Add("cbxUpdate");
										}
										if (cbxBalance.Checked)
										{
											list.Add("cbxBalance");
										}
										if (cbxKeyword.Checked)
										{
											list.Add("cbxKeyword");
										}
										if (cbxPublic.Checked)
										{
											list.Add("cbxPublic");
										}
										if (cbxTDSTym.Checked)
										{
											list.Add("cbxTDSTym");
										}
										if (cbxTDSComment.Checked)
										{
											list.Add("cbxTDSComment");
										}
										if (cbxTDSFollow.Checked)
										{
											list.Add("cbxTDSFollow");
										}
										if (cbxLike.Checked)
										{
											list.Add("cbxLike");
										}
										if (cbxCommentRandom.Checked)
										{
											list.Add("cbxCommentRandom");
										}
										if (cbxFolowUID.Checked)
										{
											list.Add("cbxFolowUID");
										}
										if (cbxSeedingVideo.Checked)
										{
											list.Add("cbxSeedingVideo");
										}
										if (cbxUpVideo.Checked)
										{
											list.Add("cbxUpVideo");
										}
										if (cbxChangeAvatar.Checked)
										{
											list.Add("cbxChangeAvatar");
										}
										if (cbxChangePass.Checked)
										{
											list.Add("cbxChangePass");
										}
										if (cbxBio.Checked)
										{
											list.Add("cbxBio");
										}
										if (cbxRename.Checked)
										{
											list.Add("cbxRename");
										}
										if (cbxFollower.Checked)
										{
											list.Add("cbxFollower");
										}
										if (cbxFollowSuggested.Checked)
										{
											list.Add("cbxFollowSuggested");
										}
										if (cbxChangeEmail.Checked)
										{
											list.Add("cbxChangeEmail");
										}
										if (cbxVeri.Checked)
										{
											list.Add("cbxVeri");
										}
										tiktokItem.IsLogin();
										if (cbxRemoveAccount.Checked)
										{
											tiktokItem.RemoveRedundanceAccount();
											return;
										}
										try
										{
											while (list.Count > 0 && tiktokItem.Info.LoggedIn)
											{
												int index = rnd.Next(list.Count);
												string text3 = list[index];
												List<string> list2 = new List<string>();
												list.RemoveAt(index);
												switch (text3)
												{
												case "cbxTDSTym":
													ShowGridMessage(row, "Đang thả tym trao đổi sub");
													tiktokItem.TDSTym();
													break;
												case "cbxUpdate":
													ShowGridMessage(row, "Đang cập nhật thông tin");
													tiktokItem.UpdateProfileInfo();
													break;
												case "cbxBio":
													ShowGridMessage(row, "Đanng đổi thông tin BIO");
													tiktokItem.ChangeBio();
													break;
												case "cbxFollower":
													ShowGridMessage(row, "Đang Follow những người đã Follow mình");
													tiktokItem.FollowByFollower();
													break;
												case "cbxKeyword":
													ShowGridMessage(row, "Đang tìm kiếm theo từ khóa");
													tiktokItem.SeachKeyword();
													break;
												case "cbxCommentRandom":
													ShowGridMessage(row, "Đang bình luận bài viết theo link");
													list2 = tiktokItem.CommentOnPost();
													ShowGridMessage(row, "Đã bình luận được " + list2.Count + " lần");
													break;
												case "cbxPublic":
													ShowGridMessage(row, "Đang công khai thông tin");
													tiktokItem.PublicInfo();
													break;
												case "cbxLike":
													ShowGridMessage(row, "Đang thích bài viết theo link");
													tiktokItem.LikePost();
													break;
												case "cbxBalance":
													ShowGridMessage(row, "Đang kiểm tra hoa hồng");
													tiktokItem.Promotion();
													break;
												case "cbxFolowUID":
													ShowGridMessage(row, "Đang Follow bài viết theo UID");
													tiktokItem.FollowUID();
													break;
												case "cbxSeedingVideo":
													ShowGridMessage(row, "Đang seeding livestream");
													tiktokItem.SeedingVideo();
													break;
												case "cbxTDSFollow":
													ShowGridMessage(row, "Đang Follow trao đổi sub");
													tiktokItem.TDSFollow();
													break;
												case "cbxChangeEmail":
													ShowGridMessage(row, "Đanng đổi Email");
													tiktokItem.ChangeEmail();
													break;
												case "cbxNewsFeed":
													ShowGridMessage(row, "Đang xem video trên tường cá nhân");
													tiktokItem.ViewNewsFeed();
													break;
												case "cbxFollowSuggested":
													ShowGridMessage(row, "Đang Follow theo gợi ý của Tiktok");
													tiktokItem.FollowBySuggested();
													break;
												case "cbxUpVideo":
													ShowGridMessage(row, "Đang đăng Video");
													tiktokItem.Upvideo();
													break;
												case "cbxVeri":
													ShowGridMessage(row, "Đanng xác nhận lại Email");
													tiktokItem.ChangeEmail(change: false);
													break;
												case "cbxEarnSu":
													ShowGridMessage(row, "Đang Kiếm Xu");
													tiktokItem.KiemXu();
													break;
												case "cbxAddProduct":
													ShowGridMessage(row, "Đang thêm sản phẩm vào shop");
													tiktokItem.AddProduct();
													break;
												case "cbxRename":
													ShowGridMessage(row, "Đanng đổi tên");
													tiktokItem.Rename();
													tiktokItem.AddLogByAction("Đổi tên");
													break;
												case "cbxAccept":
													ShowGridMessage(row, "Đang chấp nhận lời mới shop");
													tiktokItem.AcceptShop();
													break;
												case "cbxFollowCheo":
													ShowGridMessage(row, "Đang Follow Chéo");
													tiktokItem.FollowCheo();
													break;
												case "cbx2fa":
													ShowGridMessage(row, "Đanng bật 2FA");
													tiktokItem.Change2FA();
													break;
												case "cbxChangePass":
													ShowGridMessage(row, "Đanng đổi mật khẩu");
													tiktokItem.ChangePass();
													break;
												case "cbxTDSComment":
													ShowGridMessage(row, "Đang bình luận trao đổi sub");
													tiktokItem.TDSComment();
													break;
												case "cbxChangeAvatar":
													ShowGridMessage(row, "Đanng đổi Avatar");
													tiktokItem.ChangeAvatar();
													break;
												}
											}
										}
										catch (Exception ex2)
										{
											Utils.CCKLog(ex2.Message, "");
										}
										finally
										{
											if (cbxTreoNick.Checked)
											{
												int num = Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.TREO_NICK));
												Thread.Sleep(1000 * num);
											}
											if (tiktokItem.Info.LoggedIn)
											{
												tiktokItem.BackupAccountTiktok(zip: true);
											}
											if (phoneMode != PhoneMode.TWRP)
											{
												ADBHelperCCK.StopApp(SerialNo, CaChuaConstant.PACKAGE_NAME);
											}
											else
											{
												ADBHelperCCK.ClearAppData(SerialNo, CaChuaConstant.PACKAGE_NAME);
											}
										}
										ChangeGridViewDevice(SerialNo, text2, Color.Green);
										TimeSpan timeSpan = DateTime.Now.Subtract(now2);
										if (tiktokItem.JobResult.Count <= 0)
										{
											ShowGridMessage(row, "Hoàn thành lúc [" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + string.Format("] - Tổng thời gian {0}{1}{2} giây", (timeSpan.Hours > 0) ? (timeSpan.Hours + " giờ ") : "", (timeSpan.Minutes > 0) ? (timeSpan.Minutes + " phút ") : "", timeSpan.Seconds));
										}
										else
										{
											ShowGridMessage(row, string.Join(", ", tiktokItem.JobResult));
										}
									}
								}
								else
								{
									ShowGridMessage(row, outMessage);
									tiktokItem.AddLogByAction((outMessage != "") ? outMessage : outMessage);
									ChangeGridViewAccount(SerialNo, outMessage, Color.Red);
								}
							}
							else
							{
								tiktokItem.Info.LoggedIn = false;
								ShowGridMessage(row, outMessage);
							}
							end_IL_0099:;
						}
						catch (Exception ex3)
						{
							Utils.CCKLog("Run ", ex3.Message);
						}
						finally
						{
							if (File.Exists(CaChuaConstant.XPROXY_LAN))
							{
								List<DeviceProxy> list3 = new JavaScriptSerializer().Deserialize<List<DeviceProxy>>(Utils.ReadTextFile(CaChuaConstant.XPROXY_LAN));
								if (list3 != null && list3.Count > 0 && list3[0].MultipleDevice)
								{
									foreach (KeyValuePair<string, List<DeviceProxy>> item in dicXproxyLan)
									{
										if (item.Value.Find((DeviceProxy p) => p.DeviceId == SerialNo) != null)
										{
											List<DeviceProxy> list4 = dicXproxyLan[item.Key];
											while (list4.Count > 1 && list4.Count > 0)
											{
												DeviceProxy deviceProxy = list4[list4.Count - 1];
												DeviceProxy deviceProxy2 = dicXproxyLan[item.Key].Find((DeviceProxy p) => p.DeviceId == SerialNo);
												if (deviceProxy2 != null)
												{
													dicXproxyLan[item.Key].Remove(deviceProxy2);
												}
												ADBHelperCCK.ShowTextMessageOnPhone(SerialNo, "Waiting XProxy:" + deviceProxy.Proxy.ToString());
												if (deviceProxy.LastChange < DateTime.Now)
												{
													for (int i = 0; i < dicXproxyLan[item.Key].Count; i++)
													{
														if (dicXproxyLan[item.Key][i].DeviceId != SerialNo)
														{
															dicXproxyLan[item.Key].RemoveAt(i);
														}
													}
													break;
												}
												Thread.Sleep(5000);
											}
											if (list4.Count <= 0)
											{
												Thread.Sleep(5000);
											}
											else
											{
												DeviceProxy deviceProxy3 = list4[0];
												dicXproxyLan[item.Key].Clear();
												ADBHelperCCK.ShowTextMessageOnPhone(SerialNo, "Restarting XProxy:" + deviceProxy3.Proxy.ToString());
												Utils.ResetXProxyLAN(deviceProxy3);
											}
										}
									}
								}
							}
						}
					}
				}).ContinueWith(delegate
				{
					ChangeGridViewDevice(SerialNo, (device.Message != "") ? device.Message : "Done", Color.Green);
					ADBHelperCCK.TurnScreenOffDevice(SerialNo);
				});
			}
			catch (Exception ex)
			{
				DicTmProxy.Remove(device.DeviceId);
				if (!Directory.Exists("Log"))
				{
					Directory.CreateDirectory("Log");
				}
				Utils.CCKLog("Log\\run.txt", ex.Message);
			}
			finally
			{
			}
		}

		public static void LogInGrid(string searchValue, string msg)
		{
		}

		private void CheckLiveHotmail(string searchValue)
		{
			Invoke((Action)delegate
			{
				DongVanFB dongVanFB = new DongVanFB(Utils.ReadTextFile(CaChuaConstant.DONGVANFB));
				int num = -1;
				DataGridViewRow dataGridViewRow = (from DataGridViewRow r in dataGridView1.Rows
					where r.Cells["Email"].Value.ToString().Equals(searchValue)
					select r).First();
				if (dataGridViewRow != null)
				{
					num = dataGridViewRow.Index;
					string status = "";
					dongVanFB.GetEmail(dataGridViewRow.Cells["Email"].Value.ToString(), dataGridViewRow.Cells["PassEmail"].Value.ToString(), out status, loginOnly: true);
					dataGridView1.Rows[num].Cells["Noti"].Value = status;
				}
			});
		}

		private void UnlockMail()
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Stt");
			dataTable.Columns.Add("id");
			dataTable.Columns.Add("Email");
			dataTable.Columns.Add("Pass");
			dataTable.Columns.Add("Date", typeof(DateTime));
			dataTable.Columns.Add("Status");
			dataTable.Columns.Add("Note");
			int num = 0;
			DataTable dataTable2 = sql.ExecuteQuery("Select Email,PassEmail,id from Account where email like '%@outlook%' or email like '%@hotmail%'");
			Dictionary<string, DataRow> dictionary = new Dictionary<string, DataRow>();
			foreach (DataRow row in dataTable2.Rows)
			{
				if (!dictionary.ContainsKey(row["Id"].ToString()))
				{
					dictionary.Add(row["Id"].ToString(), row);
				}
			}
			foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
			{
				if (selectedRow != null && selectedRow.Cells["id"].Value != null && selectedRow.Index != -1 && dictionary.ContainsKey(selectedRow.Cells["id"].Value.ToString()))
				{
					DataRow dataRow2 = dictionary[selectedRow.Cells["id"].Value.ToString()];
					DataRow dataRow3 = dataTable.NewRow();
					dataRow3["Stt"] = ++num;
					dataRow3["id"] = dataRow2["id"].ToString();
					dataRow3["Email"] = dataRow2["Email"].ToString();
					dataRow3["Pass"] = dataRow2["PassEmail"].ToString();
					if (dataRow2["Email"].ToString().Contains("@outlook") || dataRow2["Email"].ToString().Contains("@hotmail"))
					{
						dataTable.Rows.Add(dataRow3);
					}
					dataTable.AcceptChanges();
				}
			}
			frmUnlockHotmail frmUnlockHotmail = new frmUnlockHotmail(dataTable);
			frmUnlockHotmail.StartPosition = FormStartPosition.CenterScreen;
			frmUnlockHotmail.ShowDialog();
		}

		private void ChangeGridViewDevice(string searchValue, string msg, Color c, string findColmn = "Phone", string columnName = "Status", string acc = "")
		{
			Invoke((Action)delegate
			{
				try
				{
					int num = -1;
					if (dataGridViewPhone != null)
					{
						bool allowUserToAddRows = dataGridViewPhone.AllowUserToAddRows;
						dataGridViewPhone.AllowUserToAddRows = false;
						DataGridViewRow dataGridViewRow = (from DataGridViewRow r in dataGridViewPhone.Rows
							where r.Cells[findColmn].Value.ToString().Equals(searchValue)
							select r).FirstOrDefault();
						if (dataGridViewRow != null && dataGridViewRow != null)
						{
							num = dataGridViewRow.Index;
							dataGridViewPhone.AllowUserToAddRows = allowUserToAddRows;
							if (num > -1 && num < dataGridViewPhone.Rows.Count)
							{
								DataGridViewRow dataGridViewRow2 = dataGridViewPhone.Rows[num];
								dataGridViewRow2.Cells[columnName].Value = msg + ((columnName != "Account") ? (" -> " + DateTime.Now.ToString("HH:mm:ss")) : "");
								dataGridViewRow2.DefaultCellStyle.ForeColor = c;
								if (acc != "")
								{
									dataGridViewRow2.Cells["Account"].Value = acc;
								}
								if (acc != null && acc != "")
								{
									ChangeGridViewAccount(acc, msg, c);
								}
							}
						}
					}
				}
				catch
				{
				}
			});
		}

		private void ChangeGridViewAccount(string searchValue, string msg, Color c, string findColmn = "id", string columnName = "Log")
		{
			try
			{
				Invoke((Action)delegate
				{
					int num = -1;
					if (dataGridView1 != null)
					{
						bool allowUserToAddRows = dataGridView1.AllowUserToAddRows;
						dataGridView1.AllowUserToAddRows = false;
						DataGridViewRow dataGridViewRow = (from DataGridViewRow r in dataGridView1.Rows
							where r.Cells[findColmn].Value.ToString().Equals(searchValue)
							select r).FirstOrDefault();
						if (dataGridViewRow != null && dataGridViewRow != null)
						{
							num = dataGridViewRow.Index;
							dataGridView1.AllowUserToAddRows = allowUserToAddRows;
							if (num > -1 && num < dataGridView1.Rows.Count)
							{
								DataGridViewRow dataGridViewRow2 = dataGridView1.Rows[num];
								dataGridViewRow2.Cells[columnName].Value = msg;
								dataGridViewRow2.Cells["Active Date"].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
								dataGridViewRow2.DefaultCellStyle.ForeColor = c;
							}
						}
					}
				});
			}
			catch
			{
			}
		}

		private void LoginByHand(DeviceEntity device)
		{
		}

		private DeviceEntity GetDeivcePort(string searchValue)
		{
			try
			{
				DeviceEntity item = new DeviceEntity();
				Invoke((Action)delegate
				{
					foreach (DataGridViewRow item2 in (IEnumerable)dataGridViewPhone.Rows)
					{
						if (item2.Cells["Phone"].Value != null && item2.Cells["Phone"].Value.ToString().Equals(searchValue))
						{
							item = new DeviceEntity
							{
								DeviceId = item2.Cells["Phone"].Value.ToString(),
								Name = item2.Cells["Phone"].Value.ToString(),
								Port = Convert.ToInt32(item2.Cells["Port"].Value),
								SystemPort = Convert.ToInt32(item2.Cells["SystemPort"].Value),
								Rooted = ADBHelperCCK.IsRooted(item2.Cells["Phone"].Value.ToString())
							};
						}
					}
				});
				return item;
			}
			catch (Exception ex)
			{
				Utils.CCKLog(CaChuaConstant.LOG_ACTION, Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException.Message, "GetDeivcePort 2");
			}
			return new DeviceEntity();
		}

		public void MainAction()
		{
			taskStop = new CancellationTokenSource();
			CancellationToken ct = taskStop.Token;
			dataGridViewIndexByUid.Clear();
			lstUid.Clear();
			Invoke((Action)delegate
			{
				Utils.ReconnectFixDevice();
				string text2 = Utils.ReadTextFile(CaChuaConstant.PHONE_MODE_DATA);
				bool flag2 = text2 != "" && new JavaScriptSerializer().Deserialize<PhoneMode>(text2) == PhoneMode.Root;
				dataGridView1.AllowUserToAddRows = false;
				foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
				{
					string text3 = ((selectedRow.Cells["id"] != null) ? selectedRow.Cells["id"].Value.ToString() : "");
					if (!(text3 == ""))
					{
						string text4 = selectedRow.Cells["device"].Value.ToString();
						if (text4 == "" || flag2)
						{
							text4 = "cck";
						}
						if (text3 != null)
						{
							if (!lstUid.ContainsKey(text4))
							{
								lstUid.Add(text4, new List<string> { text3 });
							}
							else
							{
								lstUid[text4].Add(text3);
							}
						}
						if (!dataGridViewIndexByUid.ContainsKey(text3.ToString()))
						{
							dataGridViewIndexByUid.Add(text3.ToString(), selectedRow);
						}
						try
						{
							selectedRow.Cells["Log"].Value = "";
						}
						catch
						{
						}
					}
				}
			});
			if (lstUid.Count != 0)
			{
				progressBar1.Minimum = 0;
				progressBar1.Maximum = lstUid.Count;
				Task.Factory.StartNew(delegate
				{
					Utils.GetMemory();
					string text = Utils.ReadTextFile(CaChuaConstant.PHONE_MODE_DATA);
					bool flag = text != "" && new JavaScriptSerializer().Deserialize<PhoneMode>(text) == PhoneMode.Root;
					int num = 1;
					if (File.Exists(CaChuaConstant.PHONE_DELAY))
					{
						num = Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.PHONE_DELAY));
					}
					foreach (KeyValuePair<string, List<string>> item2 in lstUid)
					{
						item2.Value.Reverse();
					}
					if (lstUid.Count > 0 && isRuning)
					{
						List<string> list = new List<string>();
						foreach (DataGridViewRow item3 in (IEnumerable)dataGridViewPhone.Rows)
						{
							if (item3.Cells["Phone"].Value != null)
							{
								list.Add(item3.Cells["Phone"].Value.ToString());
							}
						}
						ThreadPool.SetMinThreads(100, 4);
						ServicePointManager.DefaultConnectionLimit = 500;
						int num2 = 0;
						num2 = ((!flag) ? ((lstUid.Count >= Convert.ToInt32(list.Count)) ? Convert.ToInt32(list.Count) : lstUid.Count) : ((lstUid["cck"].Count >= Convert.ToInt32(list.Count)) ? Convert.ToInt32(list.Count) : lstUid["cck"].Count));
						tasks = new List<Task>();
						for (int i = 0; i < num2; i++)
						{
							string searchValue = list[i].ToString();
							DeviceEntity d = GetDeivcePort(searchValue);
							if (d.Port > 0)
							{
								ChangeGridViewDevice(searchValue, "Start", Color.Green);
								Task item = Task.Factory.StartNew(delegate
								{
									Run(d);
								}, ct);
								tasks.Add(item);
							}
							Thread.Sleep(1000 * num);
						}
						Task task = Task.WhenAll(tasks);
						try
						{
							task.Wait();
						}
						catch (Exception ex)
						{
							Utils.CCKLog(CaChuaConstant.LOG_ACTION, Environment.NewLine + ex.Message, "MainAction");
						}
						Thread.Sleep(2000);
						if (task.Status == TaskStatus.RanToCompletion)
						{
							isTuongTacClick = false;
							if (lstUid.Count == 0)
							{
								Process[] processesByName = Process.GetProcessesByName("node.exe");
								foreach (Process process in processesByName)
								{
									process.Kill();
								}
							}
						}
					}
				}, ct);
			}
			else
			{
				isTuongTacClick = false;
			}
		}

		public void MainActionByHand()
		{
			taskStop = new CancellationTokenSource();
			CancellationToken ct = taskStop.Token;
			dataGridViewIndexByUid.Clear();
			lstUid.Clear();
			Invoke((Action)delegate
			{
				string text2 = Utils.ReadTextFile(CaChuaConstant.PHONE_MODE_DATA);
				bool flag2 = text2 != "" && new JavaScriptSerializer().Deserialize<PhoneMode>(text2) == PhoneMode.Root;
				dataGridView1.AllowUserToAddRows = false;
				foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
				{
					string text3 = ((selectedRow.Cells["id"] != null) ? selectedRow.Cells["id"].Value.ToString() : "");
					if (!(text3 == ""))
					{
						string text4 = selectedRow.Cells["device"].Value.ToString();
						if (text4 == "" || flag2)
						{
							text4 = "cck";
						}
						if (text3 != null)
						{
							if (lstUid.ContainsKey(text4))
							{
								lstUid[text4].Add(text3);
							}
							else
							{
								lstUid.Add(text4, new List<string> { text3 });
							}
						}
						if (!dataGridViewIndexByUid.ContainsKey(text3.ToString()))
						{
							dataGridViewIndexByUid.Add(text3.ToString(), selectedRow);
						}
					}
				}
			});
			if (lstUid.Count == 0)
			{
				isTuongTacClick = false;
				return;
			}
			progressBar1.Minimum = 0;
			progressBar1.Maximum = lstUid.Count;
			Task.Factory.StartNew(delegate
			{
				Utils.GetMemory();
				string text = Utils.ReadTextFile(CaChuaConstant.PHONE_MODE_DATA);
				bool flag = text != "" && new JavaScriptSerializer().Deserialize<PhoneMode>(text) == PhoneMode.Root;
				int num = 1;
				if (File.Exists(CaChuaConstant.PHONE_DELAY))
				{
					num = Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.PHONE_DELAY));
				}
				lstUid.Reverse();
				if (lstUid.Count > 0 && isRuning)
				{
					List<string> list = new List<string>();
					foreach (DataGridViewRow item2 in (IEnumerable)dataGridViewPhone.Rows)
					{
						if (item2.Cells["Phone"].Value != null)
						{
							list.Add(item2.Cells["Phone"].Value.ToString());
						}
					}
					ThreadPool.SetMinThreads(100, 4);
					ServicePointManager.DefaultConnectionLimit = 500;
					int num2 = 0;
					num2 = ((!flag) ? ((lstUid.Count >= Convert.ToInt32(list.Count)) ? Convert.ToInt32(list.Count) : lstUid.Count) : ((lstUid["cck"].Count >= Convert.ToInt32(list.Count)) ? Convert.ToInt32(list.Count) : lstUid["cck"].Count));
					tasks = new List<Task>();
					for (int i = 0; i < num2; i++)
					{
						string searchValue = list[i].ToString();
						DeviceEntity d = GetDeivcePort(searchValue);
						if (d.Port > 0)
						{
							ChangeGridViewDevice(searchValue, "Start", Color.Green);
							Task item = Task.Factory.StartNew(delegate
							{
								Run(d, loginOnly: true);
							}, ct);
							tasks.Add(item);
						}
						Thread.Sleep(1000 * num);
					}
					Task task = Task.WhenAll(tasks);
					try
					{
						task.Wait();
					}
					catch (Exception ex)
					{
						Utils.CCKLog(CaChuaConstant.LOG_ACTION, Environment.NewLine + ex.Message, "MainAction");
					}
					Thread.Sleep(2000);
					if (task.Status == TaskStatus.RanToCompletion)
					{
						isTuongTacClick = false;
						if (lstUid.Count == 0)
						{
							Process[] processesByName = Process.GetProcessesByName("node.exe");
							foreach (Process process in processesByName)
							{
								process.Kill();
							}
						}
					}
				}
			}, ct);
		}

		public void MainActionReg()
		{
			taskStop = new CancellationTokenSource();
			CancellationToken ct = taskStop.Token;
			Task.Factory.StartNew(delegate
			{
				Utils.GetMemory();
				int num = 1;
				if (File.Exists(CaChuaConstant.PHONE_DELAY))
				{
					num = Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.PHONE_DELAY));
				}
				if (isRuning)
				{
					List<string> list = new List<string>();
					foreach (DataGridViewRow item2 in (IEnumerable)dataGridViewPhone.Rows)
					{
						if (item2.Cells["Phone"].Value != null)
						{
							list.Add(item2.Cells["Phone"].Value.ToString());
						}
					}
					int num2 = Convert.ToInt32(list.Count);
					tasks = new List<Task>();
					for (int i = 0; i < num2; i++)
					{
						string searchValue = list[i].ToString();
						DeviceEntity d = GetDeivcePort(searchValue);
						if (d.Port > 0)
						{
							ChangeGridViewDevice(searchValue, "Start", Color.Green);
							Task item = Task.Factory.StartNew(delegate
							{
								RunReg(d, i);
							}, ct);
							tasks.Add(item);
						}
						Thread.Sleep(1000 * num);
					}
					Task task = Task.WhenAll(tasks);
					try
					{
						task.Wait();
					}
					catch (Exception ex)
					{
						Utils.CCKLog(CaChuaConstant.LOG_ACTION, Environment.NewLine + ex.Message, "MainAction");
					}
					Thread.Sleep(2000);
					if (task.Status == TaskStatus.RanToCompletion)
					{
						isTuongTacClick = false;
						if (lstUid.Count == 0)
						{
							Process[] processesByName = Process.GetProcessesByName("node.exe");
							foreach (Process process in processesByName)
							{
								process.Kill();
							}
						}
					}
				}
			}, ct);
		}

		private bool ResetObcProxy()
		{
			throw new NotImplementedException();
		}

		private bool Reset4GProxy()
		{
			throw new NotImplementedException();
		}

		private string GetFieldOnDictionnary(Dictionary<string, string> dic)
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, string> item in dic)
			{
				list.Add(item.Value);
			}
			return string.Join(",", list);
		}

		private void btnLoadAccount_Click(object sender, EventArgs e)
		{
			//IL_0079: Incompatible stack heights: 0 vs 1
			tbl = new DataTable();
			Cursor.Current = Cursors.WaitCursor;
			List<string> list = new List<string>();
			if (cbxnew.Checked)
			{
				list.Add("'Live'");
			}
			if (cbxDie.Checked)
			{
				list.Add("'Die'");
			}
			if (cbxError.Checked)
			{
				list.Add("'Error'");
			}
			if (File.Exists(CaChuaConstant.SHOW_PASS))
			{
				Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.SHOW_PASS));
			}
			new List<string>();
			SQLiteUtils sQLiteUtils = new SQLiteUtils();
			for (int i = 0; i < cbxCategory.Items.Count; i++)
			{
				if (cbxCategory.GetItemChecked(i))
				{
					DataTable dataTable = sQLiteUtils.ExecuteQuery(string.Format("Select 0 as Stt,a.id as [ID], a.name as [Name],password as [Password], email as [Email],passemail, cookies as [Cookie], PrivateKey as [PrivateKey], postcount as [Video], proxy as [Proxy], b.tendanhmuc as [Category] , Brand,trangthai as [Status],likecount as Like, today as [Today], yesterday as [Yesterday], banhang as [Hoa Hong], follower as Follower,following as Following, avatar as [Avatar], year as [Year], profile as [Public],datecreate as [Created], tuongtacngay as [Active Date],ghichu as [Ghi Chu],c.name as [Phone Name],Device,Noti as Log from Account a inner join danhmuc b on a.id_danhmuc = b.id_danhmuc left join tblDevices c on a.Device = c.deviceid  Where b.tendanhmuc='" + ((Category)cbxCategory.Items[i]).tendanhmuc.ToString() + "' and trangthai in (" + string.Join(",", list) + ")", ""));
					if (dataTable != null && dataTable.Rows.Count > 0)
					{
						tbl.Merge(dataTable);
					}
					tbl.AcceptChanges();
				}
			}
			sQLiteUtils = null;
			if (tbl != null && tbl.Rows.Count > 0)
			{
				if (!tbl.Columns.Contains("Stt"))
				{
					DataColumn dataColumn = tbl.Columns.Add("Stt", Type.GetType("System.Int32"));
					dataColumn.SetOrdinal(0);
				}
				for (int j = 0; j < tbl.Rows.Count; j++)
				{
					tbl.Rows[j]["Stt"] = j + 1;
				}
			}
			BindObjectList(dataGridView1, tbl);
			Cursor.Current = Cursors.Default;
			List<string> selectedCategory = GetSelectedCategory();
			File.WriteAllText(CaChuaConstant.SELECTED_CATEGORY, new JavaScriptSerializer().Serialize(selectedCategory));
			btnLoadDevices_Click(null, null);
			AddLogToContextMenu();
		}

		private void AddLogToContextMenu()
		{
			List<string> list = GetDistinctLogValues(tbl, "Log").ToList();
			logToolStripMenuItem.DropDownItems.Clear();
			foreach (string item in list)
			{
				if (item != null && item != "")
				{
					logToolStripMenuItem.DropDownItems.Add(item);
				}
			}
			logToolStripMenuItem.DropDownItems.Add("(Blank)");
			foreach (ToolStripItem dropDownItem in logToolStripMenuItem.DropDownItems)
			{
				dropDownItem.Click += Item_Click;
			}
			logToolStripMenuItem.Visible = logToolStripMenuItem.DropDownItems.Count > 0;
			list = GetDistinctLogValues(tbl, "Ghi Chu").ToList();
			notesToolStripMenuItem.DropDownItems.Clear();
			foreach (string item2 in list)
			{
				if (item2 != null && item2.Trim() != "")
				{
					notesToolStripMenuItem.DropDownItems.Add(item2);
				}
			}
			notesToolStripMenuItem.DropDownItems.Add("(Blank)");
			foreach (ToolStripItem dropDownItem2 in notesToolStripMenuItem.DropDownItems)
			{
				dropDownItem2.Click += ItemNote_Click;
			}
			notesToolStripMenuItem.Visible = notesToolStripMenuItem.DropDownItems.Count > 0;
		}

		private void Item_Click(object sender, EventArgs e)
		{
			string text = sender.ToString();
			DataTable table = new DataTable();
			if (!(text == "(Blank)"))
			{
				try
				{
					table = tbl.Select("[Log]='" + text + "'").ToList().CopyToDataTable();
				}
				catch
				{
				}
			}
			else
			{
				IEnumerable<DataRow> source = from DataRow row in tbl.Rows
					where string.IsNullOrEmpty(row["Log"] as string)
					select row;
				table = source.CopyToDataTable();
			}
			BindObjectList(dataGridView1, table);
		}

		private void ItemNote_Click(object sender, EventArgs e)
		{
			string text = sender.ToString();
			try
			{
				DataTable dataTable = new DataTable();
				if (text == "(Blank)")
				{
					IEnumerable<DataRow> source = from DataRow row in tbl.Rows
						where string.IsNullOrEmpty(row["Ghi Chu"] as string)
						select row;
					dataTable = source.CopyToDataTable();
				}
				else
				{
					dataTable = tbl.Select("[Ghi Chu]='" + text + "'").ToList().CopyToDataTable();
				}
				BindObjectList(dataGridView1, dataTable);
			}
			catch
			{
			}
		}

		private List<string> GetSelectedCategory()
		{
			List<string> list = new List<string>();
			for (int i = 0; i < cbxCategory.Items.Count; i++)
			{
				if (cbxCategory.GetItemChecked(i))
				{
					list.Add(((Category)cbxCategory.Items[i]).tendanhmuc.ToString());
				}
			}
			return list;
		}

		private new static void DoubleBuffered(DataGridView dgv, bool setting)
		{
			Type type = dgv.GetType();
			PropertyInfo property = type.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
			property.SetValue(dgv, setting, null);
		}

		protected static void BindObjectList(DataGridView dataGridView, DataTable table, bool fixmode = true)
		{
			if (table == null)
			{
				return;
			}
			try
			{
				dataGridView.Invoke((Action)delegate
				{
					if (fixmode)
					{
						dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
						dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
						dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
						dataGridView.EnableHeadersVisualStyles = false;
						DoubleBuffered(dataGridView, setting: true);
					}
					dataGridView.DataSource = table;
					foreach (DataGridViewColumn column in dataGridView.Columns)
					{
						column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
						column.HeaderCell.Style.Font = new Font("Arial", 12f, FontStyle.Bold, GraphicsUnit.Pixel);
						column.HeaderCell.Style.BackColor = Color.DarkGray;
					}
					if (fixmode)
					{
						dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
					}
					if (dataGridView.Columns.Contains("Log"))
					{
						dataGridView.Columns["Log"].Width = (int)((double)dataGridView.Width * 0.2);
					}
					if (dataGridView.Columns.Contains("passemail"))
					{
						dataGridView.Columns["passemail"].Visible = false;
					}
					if (dataGridView.Columns.Contains("Brand"))
					{
						dataGridView.Columns["Brand"].Visible = false;
					}
					if (dataGridView.Columns.Contains("Device"))
					{
						dataGridView.Columns["Device"].Visible = false;
					}
					table = null;
				});
			}
			catch
			{
			}
		}

		private void cbxchon_CheckedChanged(object sender, EventArgs e)
		{
			for (int i = 0; i < cbxCategory.Items.Count; i++)
			{
				cbxCategory.SetItemChecked(i, cbxchon.Checked);
			}
		}

		private void pbPrecentage(ToolStripProgressBar pb, string msg = "")
		{
			int num = (int)((double)(pb.Value - pb.Minimum) / (double)(pb.Maximum - pb.Minimum) * 100.0);
			using (Graphics graphics = pb.ProgressBar.CreateGraphics())
			{
				graphics.DrawString(msg + num + "%", SystemFonts.DefaultFont, Brushes.Black, new PointF((float)(pb.Width / 2) - graphics.MeasureString(num + "%", SystemFonts.DefaultFont).Width / 2f, (float)(pb.Height / 2) - graphics.MeasureString(num + "%", SystemFonts.DefaultFont).Height / 2f));
			}
			Application.DoEvents();
		}

		private void btnStop_Click(object sender, EventArgs e)
		{
			DialogResult dialogResult = MessageBox.Show("Do you want to stop", "Stop App", MessageBoxButtons.YesNo);
			if (dialogResult != DialogResult.No)
			{
				bool isStop = File.Exists(CaChuaConstant.STOP_APP) && Convert.ToBoolean(Utils.ReadTextFile(CaChuaConstant.STOP_APP));
				isTuongTacClick = false;
				isRuning = false;
				Task.Run(delegate
				{
					StopAllApp(isStop);
				}).ContinueWith(delegate
				{
					Application.Restart();
				});
			}
		}

		private void StopAllApp(bool isStop)
		{
			if (isStop)
			{
				return;
			}
			string s;
			foreach (DataGridViewRow item in (IEnumerable)dataGridViewPhone.Rows)
			{
				try
				{
					if (item.Cells["Phone"].Value != null)
					{
						s = item.Cells["Phone"].Value.ToString();
						new Task(delegate
						{
							ADBHelperCCK.CloseApp(s, CaChuaConstant.PACKAGE_NAME);
							ADBHelperCCK.TurnScreenOffDevice(s);
						}).Start();
					}
				}
				catch
				{
				}
			}
			Thread.Sleep(3000);
			if (isStop)
			{
				Process[] processesByName = Process.GetProcessesByName("node.exe");
				foreach (Process process in processesByName)
				{
					process.Kill();
				}
				new Task(delegate
				{
					Utils.ReconnectFixDevice();
				}).Start();
			}
		}

		private void lblCCKProxy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string oBC_Proxy = CaChuaConstant.OBC_Proxy;
			frmInputControl frmInputControl = new frmInputControl();
			frmInputControl.Text = "Link OBC Proxy";
			if (File.Exists(oBC_Proxy))
			{
				frmInputControl.Result = Utils.ReadTextFile(oBC_Proxy);
				frmInputControl.SetText(frmInputControl.Result);
			}
			frmInputControl.ShowDialog();
			if (frmInputControl.Result != "")
			{
				Regex regex = new Regex("([0-9]+)\\.([0-9]+)\\.([0-9]+)\\.([0-9]+):([0-9]+)");
				Match match = regex.Match(frmInputControl.Result);
				if (match.Success)
				{
					File.WriteAllText(oBC_Proxy, match.Groups[0].Value);
				}
			}
			else
			{
				File.WriteAllText(oBC_Proxy, "");
			}
		}

		private void lbl4G_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void lblFollow_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string lIKE_PAGE = CaChuaConstant.LIKE_PAGE;
			frmInputControl frmInputControl = new frmInputControl(isSingleLine: false, "Mỗi ID Page 1 dòng");
			frmInputControl.Text = "Like Page";
			if (File.Exists(lIKE_PAGE))
			{
				frmInputControl.Result = Utils.ReadTextFile(lIKE_PAGE);
				frmInputControl.SetText(frmInputControl.Result);
			}
			frmInputControl.ShowDialog();
			File.WriteAllText(lIKE_PAGE, frmInputControl.Result);
		}

		private void lblLiveStream_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void lblReview_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void lblLikePost_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string lIKE_POST = CaChuaConstant.LIKE_POST;
			frmInputControl frmInputControl = new frmInputControl(isSingleLine: false, "Mỗi Group ID 1 dòng");
			frmInputControl.Text = "Like Post";
			if (File.Exists(lIKE_POST))
			{
				frmInputControl.Result = Utils.ReadTextFile(lIKE_POST);
				frmInputControl.SetText(frmInputControl.Result);
			}
			frmInputControl.ShowDialog();
			File.WriteAllText(lIKE_POST, frmInputControl.Result);
		}

		private void lblAddFriendUid_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void lblInviteFriend_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void lblCMSN_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string bIRTHDAY = CaChuaConstant.BIRTHDAY;
			frmInputControl frmInputControl = new frmInputControl(isSingleLine: false, "Mỗi câu chúc mừng 1 dòng");
			frmInputControl.Text = "Happy Birthday";
			if (File.Exists(bIRTHDAY))
			{
				frmInputControl.Result = Utils.ReadTextFile(bIRTHDAY);
				frmInputControl.SetText(frmInputControl.Result);
			}
			frmInputControl.ShowDialog();
			File.WriteAllText(bIRTHDAY, frmInputControl.Result);
		}

		private void cbxhpbd_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void cbxReview_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void cbxLiveStream_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void lblComment_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void lblLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
		{
		}

		private void CheckLiveAccount()
		{
			List<string> lstChecklive = new List<string>();
			Dictionary<string, string> dic = new Dictionary<string, string>();
			Invoke((Action)delegate
			{
				bool flag = dataGridView1.Columns.Contains("Status");
				foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
				{
					if (selectedRow.Cells["id"].Value != null)
					{
						string text2 = selectedRow.Cells["id"].Value.ToString();
						lstChecklive.Add(text2);
						if (flag)
						{
							dic.Add(text2, selectedRow.Cells["Status"].Value.ToString());
						}
						else
						{
							dic.Add(text2, "Live");
						}
					}
				}
			});
			progressBar1.Maximum = lstChecklive.Count;
			List<string> LiveAccount = new List<string>();
			List<string> DieAccount = new List<string>();
			Task.Factory.StartNew(delegate
			{
				if (lstChecklive.Count > 0 && isRuning)
				{
					try
					{
						int num = 100;
						num = Math.Min(lstChecklive.Count, 100);
						List<Task> list = new List<Task>();
						for (int i = 0; i < num; i++)
						{
							Task item = Task.Factory.StartNew(delegate
							{
								while (lstChecklive.Count > 0)
								{
									if (lstChecklive.Count % 10 == 0)
									{
										progressBar1.Value = progressBar1.Maximum - lstChecklive.Count;
									}
									string text = lstChecklive[0].Trim();
									lstChecklive.RemoveAt(0);
									CheckLiveResult checkLiveResult = Utils.CheckLiveUID(text);
									if (checkLiveResult.Live && text != null)
									{
										LiveAccount.Add(text);
										if (dic.ContainsKey(text) && dic[text] != "Live")
										{
											sql.SetDieAccount(text, "Live", "Live");
										}
									}
									else
									{
										sql.SetDieAccount(text, "Die");
										DieAccount.Add(text);
									}
								}
							});
							list.Add(item);
						}
						Task task = Task.WhenAll(list);
						try
						{
							task.Wait();
						}
						catch (Exception ex)
						{
							Utils.CCKLog(CaChuaConstant.LOG_ACTION, Environment.NewLine + ex.Message, "CheckLiveAccount");
						}
						if (task.Status == TaskStatus.RanToCompletion)
						{
							progressBar1.Value = progressBar1.Maximum;
							MessageBox.Show($"Total: {LiveAccount.Count + DieAccount.Count} - Live: {LiveAccount.Count} - Die: {DieAccount.Count}");
							Thread.Sleep(2000);
							btnLoadAccount_Click(null, null);
						}
					}
					catch (Exception ex2)
					{
						Utils.CCKLog(CaChuaConstant.LOG_ACTION, Environment.NewLine + ex2.Message, "CheckLiveAccount");
					}
				}
			});
		}

		private void FaceBookCreator_Click(object sender, EventArgs e)
		{
			string name = ((MenuItem)sender).Name;
			switch (name)
			{
			case "removeproxy":
				RemoveProxy();
				break;
			case "deletebackup":
				lstUid.Clear();
				new Task(delegate
				{
					progressBar1.Maximum = dataGridView1.SelectedRows.Count;
					progressBar1.Minimum = 0;
					progressBar1.Value = 0;
					foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
					{
						if (selectedRow.Cells["id"].Value != null)
						{
							progressBar1.Value++;
							string text = Application.StartupPath + "\\Authentication\\" + selectedRow.Cells["id"].Value.ToString().Trim();
							Utils.CCKLog("thu muc", text);
							clearFolder(text);
						}
					}
					progressBar1.Value = progressBar1.Maximum;
				}).Start();
				break;
			case "delete":
				new Task(delegate
				{
					DataGridViewSelectedRowCollection selectedRows = dataGridView1.SelectedRows;
					List<string> list5 = new List<string>();
					foreach (DataGridViewRow item in selectedRows)
					{
						try
						{
							if (item != null && item.Cells["id"].Value != null)
							{
								list5.Add(item.Cells["id"].Value.ToString());
							}
						}
						catch
						{
						}
					}
					progressBar1.Maximum = selectedRows.Count;
					progressBar1.Minimum = 0;
					progressBar1.Value = 0;
					DialogResult dialogResult2 = MessageBox.Show("Do you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
					Cursor.Current = Cursors.WaitCursor;
					if (dialogResult2.Equals(DialogResult.OK))
					{
						foreach (string item2 in list5)
						{
							try
							{
								if (item2 != null)
								{
									sql.ExecuteQuery("Delete From Account Where id='" + item2 + "'");
									progressBar1.Value++;
									if (item2 != "")
									{
										clearFolder(Application.StartupPath + "\\Authentication\\" + item2);
									}
								}
							}
							catch
							{
							}
						}
						progressBar1.Value = progressBar1.Maximum;
						btnLoadAccount_Click(null, null);
						Cursor.Current = Cursors.Default;
						MessageBox.Show("Deleted successfully.");
					}
				}).Start();
				break;
			case "pasteproxy":
				PasteProxy();
				break;
			case "Configuration":
				btnPhone_Click(null, null);
				break;
			case "updatepass":
			{
				frmAccountInfo frmAccountInfo3 = new frmAccountInfo();
				frmAccountInfo3.Uid = selectedUid;
				frmAccountInfo3.ShowDialog();
				break;
			}
			case "listgroup":
				ExportJoinedGroup();
				break;
			case "copyuid":
				new Task(delegate
				{
					List<string> lstTmp2 = new List<string>();
					foreach (DataGridViewRow selectedRow2 in dataGridView1.SelectedRows)
					{
						if (selectedRow2.Cells["id"].Value != null)
						{
							lstTmp2.Add(selectedRow2.Cells["id"].Value.ToString());
						}
					}
					Invoke((Action)delegate
					{
						Clipboard.SetText(string.Join(Environment.NewLine, lstTmp2));
					});
					lstTmp2.Clear();
				}).Start();
				break;
			case "viewinchrome":
				ViewInChrome(selectedUid);
				break;
			case "passupdate":
				BulkUpdate();
				break;
			case "twofactor":
			{
				DataRow accountById4 = sql.GetAccountById(selectedUid);
				if (accountById4 != null)
				{
					object obj2 = accountById4["privatekey"];
					new TwoFactorAuthenticator();
					if (obj2.ToString() != "")
					{
						string currentOtp = TimeSensitivePassCode.GetCurrentOtp(obj2.ToString());
						Clipboard.SetText(currentOtp);
					}
					else
					{
						MessageBox.Show("Chưa bật mã 2FA");
					}
				}
				break;
			}
			case "exportaccountwithmail":
			{
				List<string> list3 = new List<string>();
				foreach (DataGridViewRow selectedRow3 in dataGridView1.SelectedRows)
				{
					if (selectedRow3.Cells["id"].Value != null)
					{
						DataRow accountById3 = sql.GetAccountById(selectedRow3.Cells["id"].Value.ToString());
						if (accountById3 != null)
						{
							list3.Add(string.Format("{0}|{1}|{2}|{3}|{4}|{5}", accountById3["id"], accountById3["password"], accountById3["cookies"], accountById3["privatekey"], accountById3["email"], accountById3["passemail"]));
						}
					}
				}
				if (list3.Count > 0)
				{
					Clipboard.SetText(string.Join(Environment.NewLine, list3));
					list3.Clear();
				}
				break;
			}
			case "chrome":
				OpenInChrome(selectedUid);
				break;
			case "viewphone":
				new Task(delegate
				{
					foreach (DataGridViewRow row in dataGridViewPhone.SelectedRows)
					{
						if (row.Cells["phone"].Value != null)
						{
							new Task(delegate
							{
								Rectangle bounds = Screen.PrimaryScreen.Bounds;
								_ = bounds.Width;
								int num2 = bounds.Height;
								ADBHelperCCK.DisplayPhone(row.Cells["phone"].Value.ToString(), row.Cells["phone"].Value.ToString(), 50, 30, (num2 - 100) * 1440 / 3040, num2 - 100);
							}).Start();
						}
					}
				}).Start();
				break;
			case "checkava":
				CheckAvatar();
				break;
			case "mappinguid":
			{
				string uidClone = Clipboard.GetText();
				if (uidClone == null || uidClone.Length <= 0)
				{
					break;
				}
				new Task(delegate
				{
					string[] array = uidClone.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					List<string> list7 = new List<string>();
					progressBar1.Maximum = dataGridView1.SelectedRows.Count;
					progressBar1.Value = 0;
					progressBar1.Minimum = 0;
					foreach (DataGridViewRow selectedRow4 in dataGridView1.SelectedRows)
					{
						if (selectedRow4.Cells["id"].Value != null)
						{
							list7.Add(selectedRow4.Cells["id"].Value.ToString());
						}
					}
					int num3 = Math.Min(list7.Count, array.Length);
					for (int i = 0; i < num3; i++)
					{
						progressBar1.Value++;
						if (Utils.IsNumber(list7[i]) && Utils.IsNumber(array[i]))
						{
							sql.MappingID(list7[i], array[i]);
						}
					}
					progressBar1.Value = progressBar1.Maximum;
					MessageBox.Show("Done");
				}).Start();
				break;
			}
			case "CheckLiveAccount":
				new Task(delegate
				{
					CheckLiveAccount();
				}).Start();
				break;
			case "unlockhotmail":
				UnlockMail();
				break;
			case "copyfull":
				new Task(delegate
				{
					List<string> lstTmp = new List<string>();
					foreach (DataGridViewRow selectedRow5 in dataGridView1.SelectedRows)
					{
						if (selectedRow5.Cells["id"].Value != null)
						{
							lstTmp.Add(Utils.Convert2String(selectedRow5.Cells["id"].Value) + "|" + Utils.Convert2String(selectedRow5.Cells["password"].Value) + "|" + Utils.Convert2String(selectedRow5.Cells["Email"].Value) + "|" + Utils.Convert2String(selectedRow5.Cells["passemail"].Value));
						}
					}
					Invoke((Action)delegate
					{
						Clipboard.SetText(string.Join(Environment.NewLine, lstTmp));
					});
					lstTmp.Clear();
				}).Start();
				break;
			case "note":
			{
				frmInputControl frmInputControl = new frmInputControl(isSingleLine: false, "Add Notes");
				frmInputControl.ShowDialog();
				string ret = frmInputControl.Result;
				if (!(ret != ""))
				{
					if (MessageBox.Show("Do you want to clear notes?", "", MessageBoxButtons.YesNo) != DialogResult.Yes)
					{
						break;
					}
					new Task(delegate
					{
						foreach (DataGridViewRow selectedRow6 in dataGridView1.SelectedRows)
						{
							if (selectedRow6.Cells["id"].Value != null)
							{
								string arg2 = selectedRow6.Cells["id"].Value.ToString();
								sql.ExecuteQuery(string.Format("Update Account set ghichu='{0}' where id='{1}'", "", arg2));
							}
						}
						MessageBox.Show("Done");
					}).Start();
					break;
				}
				new Task(delegate
				{
					foreach (DataGridViewRow selectedRow7 in dataGridView1.SelectedRows)
					{
						if (selectedRow7.Cells["id"].Value != null)
						{
							string arg = selectedRow7.Cells["id"].Value.ToString();
							sql.ExecuteQuery(string.Format("Update Account set ghichu='{0}' where id='{1}'", ret.Replace("'", ""), arg));
						}
					}
					MessageBox.Show("Done");
				}).Start();
				break;
			}
			case "getuid":
				GetUIDfromBackup();
				break;
			case "startmanualphone":
				if (lstUid.Count > 0)
				{
					StartSeletedPhone();
				}
				break;
			case "exportaccount":
			{
				List<string> list4 = new List<string>();
				foreach (DataGridViewRow selectedRow8 in dataGridView1.SelectedRows)
				{
					if (selectedRow8.Cells["id"].Value != null)
					{
						DataRow accountById5 = sql.GetAccountById(selectedRow8.Cells["id"].Value.ToString());
						if (accountById5 != null)
						{
							list4.Add(string.Format("{0}|{1}|{2}", accountById5["id"], accountById5["password"], accountById5["privatekey"]));
						}
					}
				}
				if (list4.Count > 0)
				{
					Clipboard.SetText(string.Join(Environment.NewLine, list4));
					list4.Clear();
				}
				break;
			}
			case "exportbackup":
			{
				string path = "";
				using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
				{
					DialogResult dialogResult = folderBrowserDialog.ShowDialog();
					if (dialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
					{
						path = folderBrowserDialog.SelectedPath;
					}
				}
				if (string.IsNullOrWhiteSpace(path))
				{
					break;
				}
				new Task(delegate
				{
					int num = 0;
					progressBar1.Maximum = dataGridView1.SelectedRows.Count;
					progressBar1.Minimum = 0;
					List<CCKBackupItem> list6 = new List<CCKBackupItem>();
					foreach (DataGridViewRow selectedRow9 in dataGridView1.SelectedRows)
					{
						if (selectedRow9.Cells["id"].Value != null)
						{
							string text2 = selectedRow9.Cells["id"].Value.ToString();
							if (Directory.Exists(Application.StartupPath + "\\Authentication\\" + text2))
							{
								if (!Directory.Exists(path + "\\" + text2))
								{
									Directory.CreateDirectory(path + "\\" + text2);
								}
								Utils.CopyFilesRecursively(Application.StartupPath + "\\Authentication\\" + text2, path + "\\" + text2);
								progressBar1.Value = num++;
							}
							DataRow accountById6 = sql.GetAccountById(text2);
							if (accountById6 != null)
							{
								list6.Add(new CCKBackupItem
								{
									Uid = accountById6["id"].ToString(),
									Password = accountById6["password"].ToString(),
									Email = accountById6["Email"].ToString(),
									PassEmail = accountById6["PassEmail"].ToString(),
									PrivateKey = accountById6["privatekey"].ToString(),
									Brand = accountById6["brand"].ToString()
								});
							}
						}
					}
					progressBar1.Value = progressBar1.Maximum;
					File.WriteAllText(path + "\\backup.cck", new JavaScriptSerializer().Serialize(list6));
					Process.Start(path);
				}).Start();
				break;
			}
			case "copyid":
			{
				List<string> list2 = new List<string>();
				foreach (DataGridViewRow selectedRow10 in dataGridView1.SelectedRows)
				{
					if (selectedRow10.Cells["id"].Value != null)
					{
						DataRow accountById2 = sql.GetAccountById(selectedRow10.Cells["id"].Value.ToString());
						if (accountById2 != null)
						{
							list2.Add(string.Format("{0}", accountById2["id"]));
						}
					}
				}
				if (list2.Count > 0)
				{
					try
					{
						Clipboard.SetText(string.Join(Environment.NewLine, list2));
					}
					catch
					{
						Clipboard.SetText(string.Join(Environment.NewLine, list2));
					}
					list2.Clear();
				}
				break;
			}
			case "copy":
			{
				List<string> list = new List<string>();
				foreach (DataGridViewRow selectedRow11 in dataGridView1.SelectedRows)
				{
					if (selectedRow11.Cells["id"].Value != null)
					{
						DataRow accountById = sql.GetAccountById(selectedRow11.Cells["id"].Value.ToString());
						if (accountById != null)
						{
							list.Add(string.Format("{0}", accountById["password"]));
						}
					}
				}
				if (list.Count > 0)
				{
					Clipboard.SetText(string.Join(Environment.NewLine, list));
					list.Clear();
				}
				break;
			}
			case "bootlop":
				if (MessageBox.Show("Do you want to remove bootloop?", "", MessageBoxButtons.YesNo) != DialogResult.Yes)
				{
					break;
				}
				new Task(delegate
				{
					foreach (DataGridViewRow row2 in dataGridViewPhone.SelectedRows)
					{
						if (row2.Cells["phone"].Value != null)
						{
							new Task(delegate
							{
								ADBHelperCCK.ExecuteCMD(row2.Cells["phone"].Value.ToString(), "shell magisk --remove-modules");
							}).Start();
						}
					}
				}).Start();
				break;
			case "addaccount":
				if (Utils.Convert2Int(cbxCategory.SelectedValue.ToString()) > 0)
				{
					frmAccountInfo frmAccountInfo2 = new frmAccountInfo();
					frmAccountInfo2.catid = Convert.ToInt32(cbxCategory.SelectedValue);
					frmAccountInfo2.ShowDialog();
					btnLoadAccount_Click(null, null);
				}
				else
				{
					MessageBox.Show("Chưa chọn danh mục");
					if (cbxCategory.Items.Count > 0)
					{
						cbxCategory.SelectedIndex = 0;
					}
				}
				break;
			case "changecategory":
				break;
			case "advancecopy":
				break;
			case "phonegroup":
				break;
			}
		}

		private void ExportJoinedGroup()
		{
		}

		private void StartSeletedPhone()
		{
			new Task(delegate
			{
				foreach (DataGridViewRow row in dataGridViewPhone.SelectedRows)
				{
					if (row.Cells["Phone"].Value != null)
					{
						new Task(delegate
						{
							Run(new DeviceEntity
							{
								DeviceId = row.Cells["Phone"].Value.ToString(),
								Name = row.Cells["Phone"].Value.ToString(),
								Port = Convert.ToInt32(row.Cells["Port"].Value),
								SystemPort = Convert.ToInt32(row.Cells["SystemPort"].Value)
							});
						}).Start();
					}
				}
			}).Start();
		}

		private void GetUIDfromBackup()
		{
			new Task(delegate
			{
				progressBar1.Maximum = dataGridView1.SelectedRows.Count;
				progressBar1.Value = 0;
				for (int i = 0; i < dataGridView1.SelectedRows.Count; i++)
				{
					object value = dataGridView1.SelectedRows[i].Cells["id"].Value;
					if (value != null)
					{
						selectedUid = value.ToString();
						string uidFromBackup = ADBHelperCCK.GetUidFromBackup(selectedUid);
						if (uidFromBackup != "" && uidFromBackup != selectedUid && selectedUid != "")
						{
							progressBar1.Value++;
							sql.ExecuteQuery($"Update Account set id='{uidFromBackup}' where id = '{selectedUid}'");
							try
							{
								Directory.Move(Application.StartupPath + "\\\\Authentication\\\\" + selectedUid, Application.StartupPath + "\\\\Authentication\\\\" + uidFromBackup);
							}
							catch
							{
							}
							Thread.Sleep(100);
						}
					}
				}
				progressBar1.Value = progressBar1.Maximum;
			}).Start();
		}

		private void ViewInChrome(string selectedUid)
		{
			new Task(delegate
			{
				for (int i = 0; i < dataGridView1.SelectedRows.Count; i++)
				{
					object value = dataGridView1.SelectedRows[i].Cells["id"].Value;
					if (value != null)
					{
						selectedUid = value.ToString();
					}
					DataRow accountById = sql.GetAccountById(selectedUid);
					if (accountById != null)
					{
						FBChrome fBChrome = new FBChrome();
						string text = Utils.ReadTextFile(CaChuaConstant.TM_Proxy);
						string proxyIp = "";
						int proxyPort = 0;
						if (text != "")
						{
							string api = Utils.ReadTextFile(CaChuaConstant.TM_Proxy).Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
							TMProxy tMProxy = new TMProxy(api, "");
							TMProxyResult newProxy = tMProxy.GetNewProxy();
							if (newProxy.data.https != "" && newProxy.data.https.Split(':').Length == 2)
							{
								proxyIp = newProxy.data.https.Split(':')[0];
								proxyPort = Utils.Convert2Int(newProxy.data.https.Split(':')[1]);
							}
						}
						fBChrome.Init("https://www.facebook.com", selectedUid, capchat: false, proxyIp, proxyPort);
						fBChrome.Login(accountById["id"].ToString(), accountById["password"].ToString(), accountById["privatekey"].ToString());
						if (File.Exists(CaChuaConstant.PageProfile_Check) && Convert.ToBoolean(Utils.ReadTextFile(CaChuaConstant.PageProfile_Check)) && fBChrome.CheckPage5(selectedUid))
						{
							sql.ExecuteQuery($"Update Account set ghichu='Page5' where id='{selectedUid}'");
						}
					}
				}
			}).Start();
		}

		private void BulkUpdate()
		{
			new Task(delegate
			{
				frmInputControl frmInputControl = new frmInputControl(isSingleLine: false, "Nhập UID và Pass mỗi nick / dòng", "UID | Pass - cập nhật password cho tài khoản số lượng lớn");
				frmInputControl.ShowDialog();
				if (frmInputControl.Result != "")
				{
					string[] array = frmInputControl.Result.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.None);
					progressBar1.Maximum = array.Length;
					progressBar1.Minimum = 0;
					progressBar1.Value = 0;
					string[] array2 = array;
					foreach (string text in array2)
					{
						string[] array3 = text.Split('|');
						if (array3.Length >= 2 && array3[0] != "" && array3[1] != "")
						{
							sql.UpdatePass(array3[0].Trim(), array3[1].Trim());
							Thread.Sleep(100);
							progressBar1.Value++;
						}
					}
					progressBar1.Value = progressBar1.Maximum;
					MessageBox.Show("Update successful");
				}
			}).Start();
		}

		private void OpenInChrome(string selectedUid)
		{
			string text = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";
			string text2 = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe";
			foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
			{
				if (selectedRow.Cells["id"].Value == null)
				{
					continue;
				}
				DataRow accountById = sql.GetAccountById(selectedRow.Cells["id"].Value.ToString());
				if (accountById != null)
				{
					string text3 = "";
					if (File.Exists(text))
					{
						text3 = text;
					}
					else if (File.Exists(text2))
					{
						text3 = text2;
					}
					if (!(text3 != ""))
					{
					}
				}
			}
			Process.Start("explorer.exe", "/select, " + Application.StartupPath + "\\Shortcut\\");
		}

		private void RestoreAccount()
		{
		}

		private void CheckAvatar()
		{
		}

		private void RemoveProxy()
		{
			new Task(delegate
			{
				progressBar1.Maximum = dataGridView1.SelectedRows.Count;
				progressBar1.Value = 0;
				foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
				{
					progressBar1.Value++;
					object value = selectedRow.Cells["id"].Value;
					sql.UpdateProxy(value.ToString(), "");
				}
				progressBar1.Value = progressBar1.Maximum;
			}).Start();
		}

		private void PasteProxy()
		{
			List<string> clip = Clipboard.GetText().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
			new Task(delegate
			{
				progressBar1.Maximum = dataGridView1.SelectedRows.Count;
				progressBar1.Value = 0;
				foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
				{
					progressBar1.Value++;
					object value = selectedRow.Cells["id"].Value;
					if (value != null && clip.Count > 0)
					{
						string text = clip[0];
						sql.UpdateProxy(value.ToString(), text);
						clip.RemoveAt(0);
						clip.Add(text);
					}
				}
				progressBar1.Value = progressBar1.Maximum;
			}).Start();
		}

		private void clearFolder(string FolderName)
		{
			if (!(FolderName != "") || !Directory.Exists(FolderName))
			{
				return;
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(FolderName);
			FileInfo[] files = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				try
				{
					fileInfo.Delete();
				}
				catch (Exception ex)
				{
					Utils.CCKLog("Xoa file trong thu muc", ex.Message);
				}
			}
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			foreach (DirectoryInfo directoryInfo2 in directories)
			{
				clearFolder(directoryInfo2.FullName);
				try
				{
					directoryInfo2.Delete();
				}
				catch (Exception ex2)
				{
					Utils.CCKLog("Xoa thu muc con ", ex2.Message);
				}
			}
			try
			{
				directoryInfo.Delete();
			}
			catch (Exception ex3)
			{
				Utils.CCKLog("Xoa thu muc con", ex3.Message);
			}
		}

		private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://bit.ly/cckfone");
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			frmMain_FormClosing(null, null);
		}

		private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			frmLogin.isRunning = false;
			isRuning = false;
			UpdateConfig();
			if (ADBHelperCCK.lstStaticProxy.Count > 0)
			{
				foreach (ProxyServer item in ADBHelperCCK.lstStaticProxy)
				{
					item.Stop();
				}
				ADBHelperCCK.lstStaticProxy.Clear();
			}
			bool isStop = File.Exists(CaChuaConstant.STOP_APP) && Convert.ToBoolean(Utils.ReadTextFile(CaChuaConstant.STOP_APP));
			new Task(delegate
			{
				StopAllApp(isStop);
				Process.GetCurrentProcess().Kill();
			}).Start();
		}

		private void UpdateConfig()
		{
		}

		private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
		{
			try
			{
				if (e.RowIndex != -1)
				{
					if (dataGridView1.Columns.Contains("id") && dataGridView1.Rows[e.RowIndex].Selected)
					{
						selectedUid = dataGridView1.Rows[e.RowIndex].Cells["id"].Value.ToString();
					}
					else
					{
						selectedUid = "";
					}
				}
			}
			catch (Exception ex)
			{
				Utils.CCKLog(CaChuaConstant.LOG_ACTION, Environment.NewLine + ex.Message, "dataGridView1_CellMouseDown");
			}
		}

		private void btnEnableModule_Click(object sender, EventArgs e)
		{
		}

		private void btnDisableModule_Click(object sender, EventArgs e)
		{
		}

		private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void linkLabel9_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void lblConfigReg_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void lblWallview_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void nudToYear_ValueChanged(object sender, EventArgs e)
		{
		}

		private void btnStopV_Click(object sender, EventArgs e)
		{
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
		}

		private void btnLoad_Click(object sender, EventArgs e)
		{
		}

		private void button2_Click(object sender, EventArgs e)
		{
		}

		private void btnStartV_Click(object sender, EventArgs e)
		{
		}

		private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{
			Utils.CCKLog("DataError", e.Exception.Message);
			e.ThrowException = false;
		}

		private void dataGridViewPhone_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{
			Utils.CCKLog("dataGridViewPhone_DataError", e.Exception.Message);
			e.ThrowException = false;
		}

		private void lblWallview_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void lblAva_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void lblCover_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void cbxCover_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void cbxAvatar_CheckedChanged(object sender, EventArgs e)
		{
			string text = Application.StartupPath + "\\Data\\";
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			if (!Directory.Exists(text + "\\Avatar"))
			{
				Directory.CreateDirectory(text + "\\Avatar");
			}
			if (Directory.GetFiles(text + "\\Avatar").Length == 0)
			{
				MessageBox.Show("Copy file Avatar file to the folder");
				Process.Start(text + "\\Avatar");
			}
		}

		private void linkLabel10_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmAccountInfo frmAccountInfo2 = new frmAccountInfo();
			frmAccountInfo2.ShowDialog();
			btnLoadAccount_Click(null, null);
		}

		private void RepairAppium(string p_DeviceId)
		{
			ADBHelperCCK.UnInstallApp(p_DeviceId, "io.appium.uiautomator2.server");
			if (!File.Exists(Application.StartupPath + "\\Devices\\uiautomator2.server.apk"))
			{
				try
				{
					new WebClient().DownloadFile("http://cachuake.com/Download/Utils/uiautomator2.server.apk.rar", Application.StartupPath + "\\Devices\\uiautomator2.server.apk");
				}
				catch (Exception ex)
				{
					Utils.CCKLog(CaChuaConstant.LOG_ACTION, Environment.NewLine + ex.Message, "RepairAppium");
				}
			}
			if (File.Exists(Application.StartupPath + "\\Devices\\uiautomator2.server.apk"))
			{
				ADBHelperCCK.InstallApp(p_DeviceId, Application.StartupPath + "\\Devices\\uiautomator2.server.apk");
				ADBHelperCCK.ShowTextMessageOnPhone(p_DeviceId, "Setup uiautomator2.server Finished");
			}
			ADBHelperCCK.UnInstallApp(p_DeviceId, "io.appium.uiautomator2.server.test");
			if (!File.Exists(Application.StartupPath + "\\Devices\\uiautomator2.server.test.apk"))
			{
				try
				{
					new WebClient().DownloadFile("http://cachuake.com/Download/Utils/uiautomator2.server.test.apk.rar", Application.StartupPath + "\\Devices\\uiautomator2.server.test.apk");
				}
				catch (Exception ex2)
				{
					Utils.CCKLog(CaChuaConstant.LOG_ACTION, Environment.NewLine + ex2.Message, "RepairAppium");
				}
			}
			if (File.Exists(Application.StartupPath + "\\Devices\\uiautomator2.server.test.apk"))
			{
				ADBHelperCCK.InstallApp(p_DeviceId, Application.StartupPath + "\\Devices\\uiautomator2.server.test.apk");
			}
		}

		private void lblCommentUID_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void checkBox2_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
		{
		}

		private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
		}

		private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
		{
			try
			{
				if (e.RowIndex != -1)
				{
					if (dataGridView1.Columns.Contains("id") && dataGridView1.Rows[e.RowIndex].Selected)
					{
						selectedUid = dataGridView1.Rows[e.RowIndex].Cells["id"].Value.ToString();
					}
					else
					{
						selectedUid = "";
					}
				}
			}
			catch (Exception ex)
			{
				Utils.CCKLog(CaChuaConstant.LOG_ACTION, Environment.NewLine + ex.Message, "dataGridView1_CellMouseUp");
			}
		}

		private void btnCaiDat_Click(object sender, EventArgs e)
		{
		}

		private void lblRom_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://mirrorbits.lineageos.org/full/gts4lvwifi/20210329/lineage-17.1-20210329-nightly-gts4lvwifi-signed.zip");
		}

		private void linkLabel9_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string tUONG_TAC_PAGE = CaChuaConstant.TUONG_TAC_PAGE;
			frmInputControl frmInputControl = new frmInputControl(isSingleLine: false, "Mỗi ID Page 1 dòng");
			frmInputControl.Text = "Tương Tác Page";
			if (File.Exists(tUONG_TAC_PAGE))
			{
				frmInputControl.Result = Utils.ReadTextFile(tUONG_TAC_PAGE);
				frmInputControl.SetText(frmInputControl.Result);
			}
			frmInputControl.ShowDialog();
			File.WriteAllText(tUONG_TAC_PAGE, frmInputControl.Result);
		}

		private void linkLabel11_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string tUONG_TAC_GROUP = CaChuaConstant.TUONG_TAC_GROUP;
			frmInputControl frmInputControl = new frmInputControl(isSingleLine: false, "Mỗi ID Group 1 dòng");
			frmInputControl.Text = "Tương Tác Group";
			if (File.Exists(tUONG_TAC_GROUP))
			{
				frmInputControl.Result = Utils.ReadTextFile(tUONG_TAC_GROUP);
				frmInputControl.SetText(frmInputControl.Result);
			}
			frmInputControl.ShowDialog();
			File.WriteAllText(tUONG_TAC_GROUP, frmInputControl.Result);
		}

		private void btnWifiSetup_Click(object sender, EventArgs e)
		{
			frmInputControl ip = new frmInputControl(isSingleLine: true, "Nhập tên Wifi | Mật khẩu", "Tên Wifi và mật khẩu cách nhau dâu |");
			ip.ShowDialog();
			if (!(ip.Result != ""))
			{
				return;
			}
			new Task(delegate
			{
				string[] array = ip.Result.Split("|,;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				if (array.Length == 2)
				{
					List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
					foreach (string item in listSerialDevice)
					{
						int num = ADBHelperCCK.Connect2Wifi(item, array[0].Trim(), array[1].Trim());
						if (num < 0)
						{
							ChangeGridViewDevice(item, "Disconnected", Color.Red);
						}
						else
						{
							ChangeGridViewDevice(item, "Connected", Color.Green);
						}
					}
				}
			}).Start();
		}

		private void btnAppium_Click(object sender, EventArgs e)
		{
			frmInputControl frmInputControl = new frmInputControl();
			frmInputControl.ShowDialog();
			string appName = frmInputControl.Result;
			if (!(appName != ""))
			{
				return;
			}
			new Task(delegate
			{
				List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
				foreach (string item in listSerialDevice)
				{
					ADBHelperCCK.UnInstallApp(item, appName);
					ChangeGridViewDevice(item, "Gỡ xong", Color.Green);
				}
			}).Start();
		}

		private void lblPass_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmInputControl frmInputControl = new frmInputControl("Default Password");
			if (File.Exists(CaChuaConstant.PASS))
			{
				frmInputControl.SetText(Utils.ReadTextFile(CaChuaConstant.PASS));
			}
			frmInputControl.ShowDialog();
			string result = frmInputControl.Result;
			File.WriteAllText(CaChuaConstant.PASS, result);
		}

		private void lblHotmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void cbxVerify_CheckedChanged(object sender, EventArgs e)
		{
			if (!Directory.Exists(CaChuaConstant.VCF_Folder))
			{
				Directory.CreateDirectory(CaChuaConstant.VCF_Folder);
			}
		}

		private void rpt3g_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.NETWORK, new JavaScriptSerializer().Serialize(Networking._3G));
		}

		private void rbtProxy_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.NETWORK, new JavaScriptSerializer().Serialize(Networking.Proxy));
		}

		private void rbtWifi_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.NETWORK, new JavaScriptSerializer().Serialize(Networking.Wifi));
		}

		private void rbtXproxy_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.NETWORK, new JavaScriptSerializer().Serialize(Networking.Proxy_V6NET));
		}

		private void rbtBocProxy_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.NETWORK, new JavaScriptSerializer().Serialize(Networking.OBCProxy));
		}

		private void txtXproxy_TextChanged(object sender, EventArgs e)
		{
		}

		private void txtObcProxy_TextChanged(object sender, EventArgs e)
		{
		}

		private void lblSeedingGroup_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void btnPhone_Click(object sender, EventArgs e)
		{
			frmDevices frmDevices = new frmDevices(numberOfDevice);
			frmDevices.ShowDialog();
		}

		private void help_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://bit.ly/hdCCKTiktok");
		}

		private void btnCheck_Click(object sender, EventArgs e)
		{
			foreach (DataGridViewRow item in (IEnumerable)dataGridViewPhone.Rows)
			{
				if (item.Cells["Phone"].Value == null)
				{
					continue;
				}
				string phone = item.Cells["Phone"].Value.ToString();
				int port = Convert.ToInt32(item.Cells["Port"].Value.ToString());
				ChangeGridViewDevice(phone, "Connecting ..", Color.Blue);
				new Task(delegate
				{
					AppiumInfo appiumInfo = ADBHelperCCK.START_APPIUM_SERVER(phone, port);
					if (appiumInfo.is_started)
					{
						List<string> list = new List<string>();
						if (!ADBHelperCCK.IsInstallApp(phone, CaChuaConstant.PACKAGE_NAME))
						{
							list.Add("facebook");
						}
						if (!ADBHelperCCK.IsInstallApp(phone, "com.cck.support"))
						{
							list.Add("support");
						}
						if (!ADBHelperCCK.IsInstallApp(phone, "com.ntc.just4fone"))
						{
							list.Add("just4fone");
						}
						if (!ADBHelperCCK.IsRooted(phone))
						{
							list.Add("Not Root");
						}
						if (list.Count == 0)
						{
							ChangeGridViewDevice(phone, "Ready", Color.Green);
						}
						else
						{
							ChangeGridViewDevice(phone, string.Join(",", list), Color.Red);
						}
					}
					else
					{
						ChangeGridViewDevice(phone, "Disconnected", Color.Red);
					}
				}).Start();
			}
		}

		private void lblHotmail_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void btnReg_Click(object sender, EventArgs e)
		{
		}

		private void dataGridViewPhone_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
		{
		}

		private void lblDisplay_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void btnShowPhone_Click(object sender, EventArgs e)
		{
		}

		private void lblInbox_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void lblRename_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void lblSpam_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void btnRegister_Click(object sender, EventArgs e)
		{
		}

		private void txtXproxyLan_TextChanged(object sender, EventArgs e)
		{
		}

		private void lblXproxyLan_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void rbtXproxyLan_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.NETWORK, new JavaScriptSerializer().Serialize(Networking.XProxy_LAN));
			if (!File.Exists(CaChuaConstant.XPROXY_LAN))
			{
				lblXproxyLan_LinkClicked(null, null);
			}
		}

		private void lblRegNick_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void lblKeywordFriend_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string aDD_FRIEND_BY_DEYWORD = CaChuaConstant.ADD_FRIEND_BY_DEYWORD;
			frmInputControl frmInputControl = new frmInputControl(isSingleLine: false, "One keyword per line");
			frmInputControl.Text = "Add Friend By Keyword";
			if (File.Exists(aDD_FRIEND_BY_DEYWORD))
			{
				frmInputControl.Result = Utils.ReadTextFile(aDD_FRIEND_BY_DEYWORD);
				frmInputControl.SetText(frmInputControl.Result);
			}
			frmInputControl.ShowDialog();
			File.WriteAllText(aDD_FRIEND_BY_DEYWORD, frmInputControl.Result);
		}

		private void linkLabel6_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void groupBox6_Enter(object sender, EventArgs e)
		{
		}

		private void lblInviteFried_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string iNVITE_LIKEPAGE = CaChuaConstant.INVITE_LIKEPAGE;
			frmInputControl frmInputControl = new frmInputControl(isSingleLine: true, "Page ID");
			frmInputControl.Text = "Invite Friend Like Page";
			if (File.Exists(iNVITE_LIKEPAGE))
			{
				frmInputControl.Result = Utils.ReadTextFile(iNVITE_LIKEPAGE);
				frmInputControl.SetText(frmInputControl.Result);
			}
			frmInputControl.ShowDialog();
			File.WriteAllText(iNVITE_LIKEPAGE, frmInputControl.Result);
		}

		private void lblJoinGroup_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string iNVITE_JOINGROUP = CaChuaConstant.INVITE_JOINGROUP;
			frmInputControl frmInputControl = new frmInputControl(isSingleLine: true, "Group ID");
			frmInputControl.Text = "Invite Friend Join Group";
			if (File.Exists(iNVITE_JOINGROUP))
			{
				frmInputControl.Result = Utils.ReadTextFile(iNVITE_JOINGROUP);
				frmInputControl.SetText(frmInputControl.Result);
			}
			frmInputControl.ShowDialog();
			File.WriteAllText(iNVITE_JOINGROUP, frmInputControl.Result);
		}

		private void lblExtension_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void lblproxyv6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Process.Start("https://cck.vn/web/Request.aspx");
		}

		private void linkLabel7_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void lblFollowUID_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			deviceInfo = new Dictionary<string, int>();
			int num = 1;
			Cursor.Current = Cursors.WaitCursor;
			progressBar1.Value = 0;
			progressBar1.Maximum = dataGridView1.Rows.Count;
			foreach (DataGridViewRow item in (IEnumerable)dataGridView1.Rows)
			{
				if (item.Cells["Stt"].Value != null)
				{
					item.Cells["Stt"].Value = num++;
					if (!deviceInfo.ContainsKey(item.Cells["Id"].Value.ToString()))
					{
						deviceInfo.Add(item.Cells["Id"].Value.ToString(), Convert.ToInt32(item.Cells["Stt"].Value));
					}
				}
				progressBar1.Value++;
			}
			progressBar1.Value = progressBar1.Maximum;
			Cursor.Current = Cursors.Default;
		}

		private void lblLi_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void lblProxy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmCheckLiveProxy frmCheckLiveProxy = new frmCheckLiveProxy();
			frmCheckLiveProxy.StartPosition = FormStartPosition.CenterScreen;
			frmCheckLiveProxy.ShowDialog();
		}

		private void linkLabel12_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void cbxRandomComment_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void linkLabel13_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string oBC_Proxy = CaChuaConstant.OBC_Proxy;
			frmInputControl frmInputControl = new frmInputControl(isSingleLine: false, "OBC Proxy");
			frmInputControl.Text = "OBC Proxy";
			if (File.Exists(oBC_Proxy))
			{
				frmInputControl.Result = Utils.ReadTextFile(oBC_Proxy);
				frmInputControl.SetText(frmInputControl.Result);
			}
			frmInputControl.ShowDialog();
			string input = frmInputControl.Result.Trim();
			Regex regex = new Regex("([0-9]+)\\.([0-9]+)\\.([0-9]+)\\.([0-9]+):([0-9]+)");
			Match match = regex.Match(input);
			if (!match.Success)
			{
				File.WriteAllText(CaChuaConstant.OBC_Proxy, "");
				return;
			}
			input = match.Groups[0].Value;
			File.WriteAllText(CaChuaConstant.OBC_Proxy, input);
		}

		private void button1_Click_1(object sender, EventArgs e)
		{
		}

		private void groupBox4_Enter(object sender, EventArgs e)
		{
		}

		private void lblGroupKKD_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void lblInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void linkLabel14_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string tM_Proxy = CaChuaConstant.TM_Proxy;
			frmInputControl frmInputControl = new frmInputControl(isSingleLine: false, "TM Proxy");
			frmInputControl.Text = "TM Proxy";
			if (File.Exists(tM_Proxy))
			{
				frmInputControl.Result = Utils.ReadTextFile(tM_Proxy);
				frmInputControl.SetText(frmInputControl.Result);
			}
			frmInputControl.ShowDialog();
			string result = frmInputControl.Result;
			File.WriteAllText(tM_Proxy, result);
		}

		private void rbtTmproxy_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.NETWORK, new JavaScriptSerializer().Serialize(Networking.TMProxy));
		}

		private void lblwatch_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void lblNuoiGroup_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string sEARCH_GROUP = CaChuaConstant.SEARCH_GROUP;
			frmInputControl frmInputControl = new frmInputControl(isSingleLine: false, "Search Group By Keywords");
			frmInputControl.Text = "Search Group By Keywords";
			if (File.Exists(sEARCH_GROUP))
			{
				frmInputControl.Result = Utils.ReadTextFile(sEARCH_GROUP);
				frmInputControl.SetText(frmInputControl.Result);
			}
			frmInputControl.ShowDialog();
			string result = frmInputControl.Result;
			File.WriteAllText(sEARCH_GROUP, result);
		}

		private void lbltimnhom_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void lblEnglish_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Utils.CurrentLang = (Utils.CurrentLang.Equals(CaChuaConstant.TIENGVIET) ? CaChuaConstant.ENGLISH : CaChuaConstant.TIENGVIET);
			LoadLabel();
		}

		private void btnReport_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Click AutoUpdateCCKTiktok.exe to update to the latest version");
		}

		private void txtSearch_DoubleClick(object sender, EventArgs e)
		{
		}

		private void txtSearch_TextChanged(object sender, EventArgs e)
		{
		}

		private void lblViewProfile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void lblRandomgroupview_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void lblgroupactivity_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void Dashboard_Click(object sender, EventArgs e)
		{
		}

		private void lblAddFriendSuggestion_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void lblProxySeller_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string proxy_Seller = CaChuaConstant.Proxy_Seller;
			frmInputControl frmInputControl = new frmInputControl(isSingleLine: false, "Proxy Seller");
			frmInputControl.Text = "Proxy Seller";
			if (File.Exists(proxy_Seller))
			{
				frmInputControl.Result = Utils.ReadTextFile(proxy_Seller);
				frmInputControl.SetText(frmInputControl.Result);
			}
			frmInputControl.ShowDialog();
			string result = frmInputControl.Result;
			File.WriteAllText(proxy_Seller, result);
		}

		private void rbtProxySeller_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.NETWORK, new JavaScriptSerializer().Serialize(Networking.ProxySeller));
		}

		private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void linkLabel2_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void linkLabel4_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string mARKET_PLACE_INBOX = CaChuaConstant.MARKET_PLACE_INBOX;
			frmInputControl frmInputControl = new frmInputControl(isSingleLine: false, "Common reply inbox");
			frmInputControl.Text = "Common reply inbox";
			if (File.Exists(mARKET_PLACE_INBOX))
			{
				frmInputControl.Result = Utils.ReadTextFile(mARKET_PLACE_INBOX);
				frmInputControl.SetText(frmInputControl.Result);
			}
			frmInputControl.ShowDialog();
			string result = frmInputControl.Result;
			File.WriteAllText(mARKET_PLACE_INBOX, result);
		}

		private void lblUnlockCheckpoint_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void cbxCPHotmail_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void linkLabel6_LinkClicked_2(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string rEPORT_ID = CaChuaConstant.REPORT_ID;
			frmInputControl frmInputControl = new frmInputControl(isSingleLine: true, "Page ID");
			frmInputControl.Text = "ID Page Report";
			if (File.Exists(rEPORT_ID))
			{
				frmInputControl.Result = Utils.ReadTextFile(rEPORT_ID);
				frmInputControl.SetText(frmInputControl.Result);
			}
			frmInputControl.ShowDialog();
			string result = frmInputControl.Result;
			File.WriteAllText(rEPORT_ID, result);
		}

		private void lblCloneInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void lblCreatePage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void lblPageProfile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void lbl4g3g_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void rbtStatic_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.NETWORK, new JavaScriptSerializer().Serialize(Networking.VPN));
		}

		private void linkLabel11_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string tIMER_LOGIN = CaChuaConstant.TIMER_LOGIN;
			frmInputControl frmInputControl = new frmInputControl(isSingleLine: true, "Timer in second");
			frmInputControl.Text = "Login with timer in second";
			if (File.Exists(tIMER_LOGIN))
			{
				frmInputControl.Result = Utils.ReadTextFile(tIMER_LOGIN);
				frmInputControl.SetText(frmInputControl.Result);
			}
			frmInputControl.ShowDialog();
			string result = frmInputControl.Result;
			File.WriteAllText(tIMER_LOGIN, result);
		}

		private void linkLabel12_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void btnOrder_Click(object sender, EventArgs e)
		{
		}

		private void linkLabel1_LinkClicked_2(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmLikePost frmLikePost = new frmLikePost();
			frmLikePost.StartPosition = FormStartPosition.CenterScreen;
			frmLikePost.ShowDialog();
		}

		private void txtLink_TextChanged(object sender, EventArgs e)
		{
		}

		private void btnSave_Click_1(object sender, EventArgs e)
		{
		}

		private void btnReg_Click_1(object sender, EventArgs e)
		{
			if (isTuongTacClick)
			{
				MessageBox.Show("Running ...");
				return;
			}
			isTuongTacClick = true;
			isRuning = true;
			MainActionReg();
		}

		private void linkLabel8_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmVerifyEmail frmVerifyEmail = new frmVerifyEmail();
			frmVerifyEmail.StartPosition = FormStartPosition.CenterScreen;
			frmVerifyEmail.ShowDialog();
		}

		private void linkLabel2_LinkClicked_2(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmNewsfeed frmNewsfeed = new frmNewsfeed();
			frmNewsfeed.StartPosition = FormStartPosition.CenterScreen;
			frmNewsfeed.ShowDialog();
		}

		private void linkLabel3_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmCommentRandom frmCommentRandom = new frmCommentRandom();
			frmCommentRandom.StartPosition = FormStartPosition.CenterScreen;
			frmCommentRandom.ShowDialog();
		}

		private void linkLabel4_LinkClicked_2(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmFollowUID frmFollowUID = new frmFollowUID();
			frmFollowUID.StartPosition = FormStartPosition.CenterScreen;
			frmFollowUID.ShowDialog();
		}

		private void linkLabel7_LinkClicked_2(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void linkLabel5_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmSeedingLive frmSeedingLive = new frmSeedingLive();
			frmSeedingLive.StartPosition = FormStartPosition.CenterScreen;
			frmSeedingLive.ShowDialog();
		}

		private void linkLabel6_LinkClicked_3(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmArticles frmArticles = new frmArticles();
			frmArticles.StartPosition = FormStartPosition.CenterScreen;
			frmArticles.ShowDialog();
		}

		private void button1_Click_2(object sender, EventArgs e)
		{
			frmVerifyEmail frmVerifyEmail = new frmVerifyEmail();
			frmVerifyEmail.StartPosition = FormStartPosition.CenterScreen;
			frmVerifyEmail.ShowDialog();
		}

		private void linkLabel8_LinkClicked_2(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmProxyLAN frmProxyLAN = new frmProxyLAN();
			frmProxyLAN.StartPosition = FormStartPosition.CenterScreen;
			frmProxyLAN.ShowDialog();
		}

		private void rbtXProxy_CheckedChanged_1(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.NETWORK, new JavaScriptSerializer().Serialize(Networking.XProxy_LAN));
		}

		private void linkLabel9_LinkClicked_2(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void linkLabel11_LinkClicked_2(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmOpenFolderDialog frmOpenFolderDialog = new frmOpenFolderDialog();
			frmOpenFolderDialog.StartPosition = FormStartPosition.CenterScreen;
			frmOpenFolderDialog.ShowDialog();
		}

		private void linkLabel12_LinkClicked_2(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string pASS = CaChuaConstant.PASS;
			frmInputControl frmInputControl = new frmInputControl(isSingleLine: true, "Để rỗng là mật khẩu ngẫu nhiên");
			frmInputControl.Text = "Nhập mật khẩu";
			if (File.Exists(pASS))
			{
				frmInputControl.Result = Utils.ReadTextFile(pASS);
				frmInputControl.SetText(frmInputControl.Result);
			}
			frmInputControl.ShowDialog();
			File.WriteAllText(pASS, frmInputControl.Result);
		}

		private void linkLabel7_LinkClicked_3(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmCheckLiveProxyTM frmCheckLiveProxyTM = new frmCheckLiveProxyTM();
			frmCheckLiveProxyTM.StartPosition = FormStartPosition.CenterScreen;
			frmCheckLiveProxyTM.ShowDialog();
		}

		private void rbtTM_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.NETWORK, new JavaScriptSerializer().Serialize(Networking.TMProxy));
		}

		private void linkLabel9_LinkClicked_3(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmTDSControl frmTDSControl = new frmTDSControl("Cấu hình thả Tym", CaChuaConstant.TDS_TYM);
			frmTDSControl.StartPosition = FormStartPosition.CenterScreen;
			frmTDSControl.ShowDialog();
		}

		private void linkLabel16_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmTDSControl frmTDSControl = new frmTDSControl("Cấu hình bình luận", CaChuaConstant.TDS_COMMENT);
			frmTDSControl.StartPosition = FormStartPosition.CenterScreen;
			frmTDSControl.ShowDialog();
		}

		private void linkLabel17_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmTDSControl frmTDSControl = new frmTDSControl("Cấu hình Follow", CaChuaConstant.TDS_FOLLOW);
			frmTDSControl.StartPosition = FormStartPosition.CenterScreen;
			frmTDSControl.ShowDialog();
		}

		private void cbxWifi_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.NETWORK, new JavaScriptSerializer().Serialize(Networking.Wifi));
		}

		private void linkLabel18_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmKeyword frmKeyword = new frmKeyword();
			frmKeyword.StartPosition = FormStartPosition.CenterScreen;
			frmKeyword.ShowDialog();
		}

		private void linkLabel15_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string bIO = CaChuaConstant.BIO;
			frmInputControl frmInputControl = new frmInputControl(isSingleLine: false, "Danh sách BIO");
			frmInputControl.Text = "Danh sách BIO";
			if (File.Exists(bIO))
			{
				frmInputControl.Result = Utils.ReadTextFile(bIO);
				frmInputControl.SetText(frmInputControl.Result);
			}
			frmInputControl.ShowDialog();
			File.WriteAllText(bIO, frmInputControl.Result);
		}

		private void btnReport_Click_1(object sender, EventArgs e)
		{
			frmUpdate frmUpdate = new frmUpdate("");
			frmUpdate.StartPosition = FormStartPosition.CenterScreen;
			frmUpdate.ShowDialog();
		}

		private void button2_Click_1(object sender, EventArgs e)
		{
			tdsConfig tdsConfig = new tdsConfig();
			tdsConfig.StartPosition = FormStartPosition.CenterScreen;
			tdsConfig.ShowDialog();
		}

		private void addAccount_Click(object sender, EventArgs e)
		{
		}

		private void editAccount_Click(object sender, EventArgs e)
		{
		}

		private void deleteMenu_Click(object sender, EventArgs e)
		{
		}

		private void copyUIDPassEmailPassEmailDeviceInfoToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private void copyUIDPassToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private void copyEmailPassEmailToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private void unlockHotmailToolStripMenuItem_Click(object sender, EventArgs e)
		{
			UnlockMail();
		}

		private void button4_Click(object sender, EventArgs e)
		{
			frmAccountInfo frmAccountInfo2 = new frmAccountInfo();
			frmAccountInfo2.ShowDialog();
			btnLoadAccount_Click(null, null);
		}

		private void button3_Click(object sender, EventArgs e)
		{
			frmCategory frmCategory = new frmCategory();
			frmCategory.IsAdd = true;
			frmCategory.StartPosition = FormStartPosition.CenterScreen;
			frmCategory.ShowDialog();
			LoadCategory();
		}

		private void btnShow_Click(object sender, EventArgs e)
		{
			frmDownload frmDownload = new frmDownload();
			if (!File.Exists("ffmppeg.exe"))
			{
				frmDownload.Download("https://cck.vn/Download/Update/ffmppeg.exe", Application.StartupPath.TrimEnd('\\') + "\\ffmppeg.exe");
				frmDownload.ShowDialog();
			}
			if (!File.Exists("ffplay.exe"))
			{
				frmDownload.Download("https://cck.vn/Download/Update/ffplay.exe", Application.StartupPath.TrimEnd('\\') + "\\ffplay.exe");
				frmDownload.ShowDialog();
			}
			if (!File.Exists("ffprobe.exe"))
			{
				frmDownload.Download("https://cck.vn/Download/Update/ffprobe.exe", Application.StartupPath.TrimEnd('\\') + "\\ffprobe.exe");
				frmDownload.ShowDialog();
			}
			frmRender frmRender = new frmRender();
			frmRender.StartPosition = FormStartPosition.CenterParent;
			frmRender.ShowDialog();
		}

		private void toolStripTextBox1_Click(object sender, EventArgs e)
		{
		}

		private void btnSearch_Click(object sender, EventArgs e)
		{
			tbl = sql.ExecuteQuery(string.Format("Select 0 as Stt,a.id as [ID], a.name as [Name],password as [Password], email as [Email],passemail, cookies as [Cookie], PrivateKey as [PrivateKey], postcount as [Video], proxy as [Proxy] ,b.tendanhmuc as [Category], Brand,trangthai as [Status],likecount as Like, banhang as [Hoa Hong], today as [Today], yesterday as [Yesterday], follower as Follower,following as Following, avatar as [Avatar], year as [Year], profile as [Public],datecreate as [Created], tuongtacngay as [Active Date],ghichu as [Ghi Chu],c.name as [Phone Name],Device,Noti as Log from Account a inner join danhmuc b on a.id_danhmuc = b.id_danhmuc left join tblDevices c on a.Device = c.deviceid  Where a.id in ('" + txtKeyword.Text + "') or email in ('" + txtKeyword.Text + "')"));
			if (tbl != null && tbl.Rows.Count > 0)
			{
				if (!tbl.Columns.Contains("Stt"))
				{
					DataColumn dataColumn = tbl.Columns.Add("Stt", Type.GetType("System.Int32"));
					dataColumn.SetOrdinal(0);
				}
				for (int i = 0; i < tbl.Rows.Count; i++)
				{
					tbl.Rows[i]["Stt"] = i + 1;
				}
			}
			BindObjectList(dataGridView1, tbl);
			AddLogToContextMenu();
		}

		private void changeCategory_Click(object sender, EventArgs e)
		{
			List<string> list = new List<string>();
			foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
			{
				if (selectedRow.Cells["id"].Value != null)
				{
					string item = selectedRow.Cells["id"].Value.ToString();
					list.Add(item);
				}
			}
			if (list.Count > 0)
			{
				frmChangeCateogry frmChangeCateogry = new frmChangeCateogry(list);
				frmChangeCateogry.StartPosition = FormStartPosition.CenterParent;
				frmChangeCateogry.ShowDialog();
				btnLoadAccount_Click(null, null);
			}
		}

		private void copyUIDPassEmailPassEmailDeviceInfoToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				List<string> lstTmp = new List<string>();
				foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
				{
					if (selectedRow.Cells["id"].Value != null)
					{
						lstTmp.Add(Utils.Convert2String(selectedRow.Cells["id"].Value) + "|" + Utils.Convert2String(selectedRow.Cells["password"].Value) + "|" + Utils.Convert2String(selectedRow.Cells["Email"].Value) + "|" + Utils.Convert2String(selectedRow.Cells["passemail"].Value) + "|" + Utils.Convert2String(selectedRow.Cells["Brand"].Value).Replace("\n", "^").Replace("\r", "^"));
					}
				}
				Invoke((Action)delegate
				{
					Clipboard.SetText(string.Join(Environment.NewLine, lstTmp));
				});
				lstTmp.Clear();
			}).Start();
		}

		private void copyUIDToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				List<string> lstTmp = new List<string>();
				foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
				{
					if (selectedRow.Cells["id"].Value != null)
					{
						lstTmp.Add(selectedRow.Cells["id"].Value.ToString());
					}
				}
				if (lstTmp.Count > 0)
				{
					Invoke((Action)delegate
					{
						Clipboard.SetText(string.Join(Environment.NewLine, lstTmp));
					});
				}
				lstTmp.Clear();
			}).Start();
		}

		private void copyPasswordToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				List<string> lstTmp = new List<string>();
				foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
				{
					if (selectedRow.Cells["Password"].Value != null)
					{
						lstTmp.Add(selectedRow.Cells["Password"].Value.ToString());
					}
				}
				if (lstTmp.Count > 0)
				{
					Invoke((Action)delegate
					{
						Clipboard.SetText(string.Join(Environment.NewLine, lstTmp));
					});
				}
				lstTmp.Clear();
			}).Start();
		}

		private void copyEmailToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				List<string> lstTmp = new List<string>();
				foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
				{
					if (selectedRow.Cells["Email"].Value != null)
					{
						lstTmp.Add(selectedRow.Cells["Email"].Value.ToString());
					}
				}
				if (lstTmp.Count > 0)
				{
					Invoke((Action)delegate
					{
						Clipboard.SetText(string.Join(Environment.NewLine, lstTmp));
					});
				}
				lstTmp.Clear();
			}).Start();
		}

		private void copyPasswordEmailToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				List<string> lstTmp = new List<string>();
				foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
				{
					if (selectedRow.Cells["PassEmail"].Value != null)
					{
						lstTmp.Add(selectedRow.Cells["PassEmail"].Value.ToString());
					}
				}
				if (lstTmp.Count > 0)
				{
					Invoke((Action)delegate
					{
						Clipboard.SetText(string.Join(Environment.NewLine, lstTmp));
					});
				}
				lstTmp.Clear();
			}).Start();
		}

		private void copyEmailPassEmailToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				List<string> lstTmp = new List<string>();
				foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
				{
					if (selectedRow.Cells["email"].Value != null)
					{
						lstTmp.Add(selectedRow.Cells["email"].Value.ToString() + "|" + selectedRow.Cells["passemail"].Value.ToString());
					}
				}
				if (lstTmp.Count > 0)
				{
					Invoke((Action)delegate
					{
						Clipboard.SetText(string.Join(Environment.NewLine, lstTmp));
					});
				}
				lstTmp.Clear();
			}).Start();
		}

		private void linkLabel19_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmFollowers frmFollowers = new frmFollowers(CaChuaConstant.FOLLOWERS, "Cấu hình Followers");
			frmFollowers.StartPosition = FormStartPosition.CenterScreen;
			frmFollowers.ShowDialog();
		}

		private void linkLabel10_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmFollowers frmFollowers = new frmFollowers(CaChuaConstant.FOLLOWERS_SUGGESTED, "Cấu hình Followers Suggested");
			frmFollowers.StartPosition = FormStartPosition.CenterScreen;
			frmFollowers.ShowDialog();
		}

		private void checkLiveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<string> lstTmp;
			Dictionary<string, int> dicIndex;
			int die;
			int live;
			SQLiteUtils sqla;
			new Task(delegate
			{
				try
				{
					lstTmp = new List<string>();
					dicIndex = new Dictionary<string, int>();
					foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
					{
						string text = ((selectedRow.Cells["id"].Value != null) ? selectedRow.Cells["id"].Value.ToString() : "");
						if (!lstTmp.Contains(text) && !string.IsNullOrWhiteSpace(text))
						{
							lstTmp.Add(selectedRow.Cells["id"].Value.ToString());
							dicIndex.Add(selectedRow.Cells["id"].Value.ToString(), selectedRow.Index);
						}
					}
					progressBar1.Maximum = lstTmp.Count;
					progressBar1.Minimum = 0;
					progressBar1.Value = 0;
					int num = 1;
					Task[] array = new Task[1];
					die = 0;
					live = 0;
					sqla = new SQLiteUtils();
					sqla.OpenConnection();
					for (int i = 0; i < num; i++)
					{
						array[i] = Task.Run(delegate
						{
							while (lstTmp.Count > 0)
							{
								if (lstTmp.Count > 0)
								{
									string uid = lstTmp[0];
									lstTmp.RemoveAt(0);
									try
									{
										CheckLiveResult checkLiveResult = Utils.CheckLiveUID(uid);
										sqla.BatchUpdateTrangThai(uid, checkLiveResult.Live ? "Live" : "Die", checkLiveResult.Live ? "Live" : "Error", checkLiveResult);
										if (checkLiveResult.Live)
										{
											live++;
										}
										else
										{
											die++;
										}
										progressBar1.Value = progressBar1.Maximum - lstTmp.Count;
									}
									catch
									{
									}
								}
								if (lstTmp.Count == 0)
								{
									dicIndex.Clear();
								}
							}
						});
					}
					Task.WaitAll(array);
					sqla.CloseConnection();
					sqla = null;
					progressBar1.Value = progressBar1.Maximum;
					MessageBox.Show($"Total: {die + live} --> Live: {live} --> Die {die}");
				}
				catch (Exception ex)
				{
					Utils.CCKLog("Check Live", ex.Message);
				}
			}).Start();
		}

		private void addNoteoolStripMenuItem_Click(object sender, EventArgs e)
		{
			frmInputControl frmInputControl = new frmInputControl(isSingleLine: false, "Add Notes");
			frmInputControl.ShowDialog();
			string ret = frmInputControl.Result;
			if (!(ret != ""))
			{
				if (MessageBox.Show("Do you want to clear notes?", "", MessageBoxButtons.YesNo) != DialogResult.Yes)
				{
					return;
				}
				new Task(delegate
				{
					foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
					{
						if (selectedRow.Cells["id"].Value != null)
						{
							string arg2 = selectedRow.Cells["id"].Value.ToString();
							sql.ExecuteQuery(string.Format("Update Account set ghichu='{0}' where id='{1}'", "", arg2));
						}
					}
					MessageBox.Show("Done");
				}).Start();
				return;
			}
			new Task(delegate
			{
				foreach (DataGridViewRow selectedRow2 in dataGridView1.SelectedRows)
				{
					if (selectedRow2.Cells["id"].Value != null)
					{
						string arg = selectedRow2.Cells["id"].Value.ToString();
						sql.ExecuteQuery(string.Format("Update Account set ghichu='{0}' where id='{1}'", ret.Replace("'", ""), arg));
					}
				}
				MessageBox.Show("Done");
			}).Start();
		}

		private void editToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DataGridViewSelectedRowCollection selectedRows = dataGridView1.SelectedRows;
			if (selectedRows.Count >= 1)
			{
				string uid = Utils.Convert2String(selectedRows[0].Cells["id"].Value);
				frmAccountInfo frmAccountInfo2 = new frmAccountInfo();
				frmAccountInfo2.Uid = uid;
				frmAccountInfo2.ShowDialog();
			}
		}

		private void addToolStripMenuItem_Click(object sender, EventArgs e)
		{
			frmAccountInfo frmAccountInfo2 = new frmAccountInfo();
			frmAccountInfo2.ShowDialog();
			btnLoadAccount_Click(null, null);
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!MessageBox.Show("Do you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk).Equals(DialogResult.OK))
			{
				return;
			}
			new Task(delegate
			{
				DataGridViewSelectedRowCollection selectedRows = dataGridView1.SelectedRows;
				List<string> list = new List<string>();
				foreach (DataGridViewRow item in selectedRows)
				{
					try
					{
						if (item != null && item.Cells["id"].Value != null)
						{
							list.Add(item.Cells["id"].Value.ToString());
						}
					}
					catch
					{
					}
				}
				progressBar1.Maximum = selectedRows.Count;
				progressBar1.Minimum = 0;
				progressBar1.Value = 0;
				Cursor.Current = Cursors.WaitCursor;
				foreach (string item2 in list)
				{
					try
					{
						if (item2 != null)
						{
							sql.ExecuteQuery("Delete From Account Where id='" + item2 + "'");
							progressBar1.Value++;
							if (item2 != "")
							{
								clearFolder(Application.StartupPath + "\\Authentication\\" + item2);
							}
						}
					}
					catch
					{
					}
				}
				progressBar1.Value = progressBar1.Maximum;
				btnLoadAccount_Click(null, null);
				Cursor.Current = Cursors.Default;
				MessageBox.Show("Deleted successfully.");
			}).Start();
		}

		private void deleteBackupToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!MessageBox.Show("Do you want to remove backup?", "Remove backup", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk).Equals(DialogResult.OK))
			{
				return;
			}
			new Task(delegate
			{
				DataGridViewSelectedRowCollection selectedRows = dataGridView1.SelectedRows;
				List<string> list = new List<string>();
				foreach (DataGridViewRow item in selectedRows)
				{
					try
					{
						if (item != null && item.Cells["id"].Value != null)
						{
							list.Add(item.Cells["id"].Value.ToString());
						}
					}
					catch
					{
					}
				}
				progressBar1.Maximum = selectedRows.Count;
				progressBar1.Minimum = 0;
				progressBar1.Value = 0;
				Cursor.Current = Cursors.WaitCursor;
				foreach (string item2 in list)
				{
					try
					{
						if (item2 != null)
						{
							progressBar1.Value++;
							if (item2 != "")
							{
								clearFolder(Application.StartupPath + "\\Authentication\\" + item2.TrimStart('@'));
							}
						}
					}
					catch
					{
					}
				}
				progressBar1.Value = progressBar1.Maximum;
				btnLoadAccount_Click(null, null);
				Cursor.Current = Cursors.Default;
				MessageBox.Show("Removed successfully.");
			}).Start();
		}

		private void loginbyhanTayToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MainActionByHand();
		}

		private void linkLabel14_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmVerifyEmail frmVerifyEmail = new frmVerifyEmail();
			frmVerifyEmail.StartPosition = FormStartPosition.CenterScreen;
			frmVerifyEmail.ShowDialog();
		}

		private void loctrungToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				sql.ExecuteQuery("Update Account set trangthai = 'New' where trangthai not in ('New','Error')");
				DataTable dataTable = sql.ExecuteQuery("select id_account, id, email from Account order by id_account asc");
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				List<string> list = new List<string>();
				foreach (DataRow row in dataTable.Rows)
				{
					if (dictionary.ContainsKey(row["id"].ToString().ToLower()))
					{
						if (row["id"].ToString().ToLower() != "")
						{
							list.Add(row["id_account"].ToString());
						}
					}
					else
					{
						dictionary.Add(row["id"].ToString().ToLower(), 1);
					}
					if (row["email"].ToString().ToLower() != "")
					{
						if (dictionary.ContainsKey(row["email"].ToString().ToLower()))
						{
							if (row["email"].ToString().ToLower() != "")
							{
								list.Add(row["id_account"].ToString());
							}
						}
						else
						{
							dictionary.Add(row["email"].ToString().ToLower(), 1);
						}
					}
				}
				if (list.Count > 0)
				{
					sql.ExecuteQuery(string.Format("delete from  Account where id_account in ({0})", string.Join(",", list)));
				}
				dataTable.Clear();
				btnLoadAccount_Click(null, null);
			}).Start();
		}

		private void linkLabel13_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmHMA frmHMA = new frmHMA();
			frmHMA.StartPosition = FormStartPosition.CenterScreen;
			frmHMA.ShowDialog();
		}

		private void rbtHMA_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.NETWORK, new JavaScriptSerializer().Serialize(Networking.HMA));
		}

		private void kiemTraBackupToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				bool flag = false;
				DataGridViewSelectedRowCollection selectedRows = dataGridView1.SelectedRows;
				new List<string>();
				progressBar1.Maximum = selectedRows.Count;
				progressBar1.Minimum = 0;
				progressBar1.Value = 0;
				Cursor.Current = Cursors.WaitCursor;
				for (int num = selectedRows.Count - 1; num >= 0; num--)
				{
					try
					{
						DataGridViewRow dataGridViewRow = selectedRows[num];
						if (dataGridViewRow != null && dataGridViewRow.Cells["id"].Value != null)
						{
							progressBar1.Value++;
							flag = Directory.Exists(Application.StartupPath + "\\Authentication\\" + dataGridViewRow.Cells["id"].Value.ToString().Replace("@", ""));
							dataGridViewRow.Cells["Log"].Value = (flag ? "Yes" : "No");
						}
					}
					catch
					{
					}
				}
				progressBar1.Value = progressBar1.Maximum;
				Cursor.Current = Cursors.Default;
			}).Start();
		}

		private void linkLabel18_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void linkLabel20_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string shoplive_Proxy = CaChuaConstant.Shoplive_Proxy;
			frmInputControl frmInputControl = new frmInputControl(isSingleLine: false, "ShopLike Proxy Key | Location | provider");
			frmInputControl.Text = "ShopLike Proxy mỗi key 1 dòng";
			if (File.Exists(shoplive_Proxy))
			{
				frmInputControl.Result = Utils.ReadTextFile(shoplive_Proxy);
				frmInputControl.SetText(frmInputControl.Result);
			}
			frmInputControl.ShowDialog();
			File.WriteAllText(shoplive_Proxy, frmInputControl.Result);
		}

		private void rbtShopLive_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.NETWORK, new JavaScriptSerializer().Serialize(Networking.ShopLive));
		}

		private void exportBackupAccountToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string path = "";
			using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
			{
				DialogResult dialogResult = folderBrowserDialog.ShowDialog();
				if (dialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
				{
					path = folderBrowserDialog.SelectedPath;
				}
			}
			if (string.IsNullOrWhiteSpace(path))
			{
				return;
			}
			new Task(delegate
			{
				int num = 0;
				progressBar1.Maximum = dataGridView1.SelectedRows.Count;
				progressBar1.Minimum = 0;
				List<CCKBackupItem> list = new List<CCKBackupItem>();
				foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
				{
					if (selectedRow.Cells["id"].Value != null)
					{
						string text = selectedRow.Cells["id"].Value.ToString();
						if (Directory.Exists(Application.StartupPath + "\\Authentication\\" + text))
						{
							if (!Directory.Exists(path + "\\" + text))
							{
								Directory.CreateDirectory(path + "\\" + text);
							}
							Utils.CopyFilesRecursively(Application.StartupPath + "\\Authentication\\" + text, path + "\\" + text);
							progressBar1.Value = num++;
						}
						DataRow accountById = sql.GetAccountById(text);
						if (accountById != null)
						{
							list.Add(new CCKBackupItem
							{
								Uid = accountById["id"].ToString(),
								Password = accountById["password"].ToString(),
								Email = accountById["Email"].ToString(),
								PassEmail = accountById["PassEmail"].ToString(),
								PrivateKey = accountById["privatekey"].ToString(),
								Brand = accountById["brand"].ToString()
							});
						}
					}
				}
				progressBar1.Value = progressBar1.Maximum;
				File.WriteAllText(path + "\\backup_tiktok.cck", new JavaScriptSerializer().Serialize(list));
				Process.Start(path);
			}).Start();
		}

		private void refreshClearADBToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process[] processesByName = Process.GetProcessesByName("node.exe");
			foreach (Process process in processesByName)
			{
				process.Kill();
			}
			new Task(delegate
			{
				ADBHelperCCK.RunCommand("taskkill /f /im node.exe");
				Utils.ReconnectFixDevice();
			}).Start();
		}

		private void copyUIDPassEmailPassEmailToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				List<string> lstTmp = new List<string>();
				foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
				{
					if (selectedRow.Cells["id"].Value != null)
					{
						lstTmp.Add(Utils.Convert2String(selectedRow.Cells["id"].Value) + "|" + Utils.Convert2String(selectedRow.Cells["password"].Value) + "|" + Utils.Convert2String(selectedRow.Cells["Email"].Value) + "|" + Utils.Convert2String(selectedRow.Cells["passemail"].Value) + "|" + Utils.Convert2String(selectedRow.Cells["cookie"].Value));
					}
				}
				Invoke((Action)delegate
				{
					Clipboard.SetText(string.Join(Environment.NewLine, lstTmp));
				});
				lstTmp.Clear();
			}).Start();
		}

		private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
		{
			string text = ((dataGridView1.Rows[e.RowIndex].Cells["Status"].Value != null) ? dataGridView1.Rows[e.RowIndex].Cells["Status"].Value.ToString() : "Live");
			dataGridView1.Columns["Stt"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dataGridView1.BackgroundColor = Color.FromArgb(212, 236, 184);
			if (!(text == "Live"))
			{
				dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 217);
			}
			else
			{
				dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(212, 236, 184);
			}
		}

		private void linkLabel21_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmVerifyEmail frmVerifyEmail = new frmVerifyEmail();
			frmVerifyEmail.StartPosition = FormStartPosition.CenterScreen;
			frmVerifyEmail.ShowDialog();
		}

		private void checkLiveHotmailToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<string> lstMail = new List<string>();
			new Task(delegate
			{
				_ = dataGridView1.SelectedRows;
				for (int i = 0; i < dataGridView1.SelectedRows.Count; i++)
				{
					DataGridViewRow dataGridViewRow = dataGridView1.SelectedRows[i];
					if (dataGridViewRow.Cells["Email"].Value != null)
					{
						string text = dataGridViewRow.Cells["Email"].Value.ToString();
						string text2 = dataGridViewRow.Cells["PassEmail"].Value.ToString();
						if (text.Contains("@hotmail") || text.Contains("@outlook"))
						{
							lstMail.Add(text + "," + text2);
						}
					}
				}
				new DongVanFB("");
				for (int j = 0; j < 10; j++)
				{
					new Task(delegate
					{
						while (lstMail.Count > 0)
						{
							string text3 = lstMail[0];
							string[] array = text3.Split(',');
							lstMail.RemoveAt(0);
							try
							{
								CheckLiveHotmail(array[0]);
							}
							catch (Exception)
							{
							}
						}
					}).Start();
					Thread.Sleep(200);
				}
			}).Start();
		}

		private void linkLabel22_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string tREO_NICK = CaChuaConstant.TREO_NICK;
			frmInputControl frmInputControl = new frmInputControl(isSingleLine: true, "Sleeping time (second)");
			frmInputControl.Text = "Sleeping time (second)";
			if (File.Exists(tREO_NICK))
			{
				frmInputControl.Result = Utils.ReadTextFile(tREO_NICK);
				frmInputControl.SetText(frmInputControl.Result);
			}
			frmInputControl.ShowDialog();
			File.WriteAllText(tREO_NICK, frmInputControl.Result);
		}

		private void linkLabel23_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmAddProduct frmAddProduct = new frmAddProduct();
			frmAddProduct.ShowDialog();
		}

		private void linkLabel24_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string shopLink = CaChuaConstant.ShopLink;
			frmInputControl frmInputControl = new frmInputControl(isSingleLine: false, "Mỗi link 1 sản phẩm", "Mỗi link 1 sản phẩm");
			frmInputControl.Text = "Link sản phẩm";
			if (File.Exists(shopLink))
			{
				frmInputControl.Result = Utils.ReadTextFile(shopLink);
				frmInputControl.SetText(frmInputControl.Result);
			}
			frmInputControl.ShowDialog();
			File.WriteAllText(shopLink, frmInputControl.Result);
		}

		private void linkLabel24_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmFollowCheo frmFollowCheo = new frmFollowCheo();
			frmFollowCheo.StartPosition = FormStartPosition.CenterScreen;
			frmFollowCheo.ShowDialog();
		}

		private void gawnsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			frmInputControl frmInputControl = new frmInputControl(isSingleLine: false, "Nhập danh sách Proxy cần gắn cho nick", "Mỗi proxy 1 dòng");
			frmInputControl.ShowDialog();
			if (!(frmInputControl.Result != ""))
			{
				return;
			}
			string result = frmInputControl.Result;
			if (result == null || !(result != ""))
			{
				return;
			}
			string[] array = result.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			DataGridViewSelectedRowCollection selectedRows = dataGridView1.SelectedRows;
			Math.Min(array.Length, selectedRows.Count);
			for (int i = 0; i < selectedRows.Count; i++)
			{
				try
				{
					string id = selectedRows[i].Cells["Id"].Value.ToString();
					if (sql == null)
					{
						sql = new SQLiteUtils();
					}
					sql.UpdateProxy(id, array[i]);
				}
				catch
				{
				}
			}
			btnLoadAccount_Click(null, null);
		}

		private void xoasToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				DataGridViewSelectedRowCollection selectedRows = dataGridView1.SelectedRows;
				progressBar1.Maximum = selectedRows.Count;
				progressBar1.Minimum = 0;
				foreach (DataGridViewRow item in selectedRows)
				{
					string text = item.Cells["id"].Value.ToString();
					if (sql == null)
					{
						sql = new SQLiteUtils();
					}
					if (text != "")
					{
						sql.ExecuteQuery($"Update Account set device='' where id='{text}'");
						if (progressBar1.Value < progressBar1.Maximum)
						{
							progressBar1.Value++;
						}
					}
				}
				btnLoadAccount_Click(null, null);
			}).Start();
		}

		private void mnuChiaThietbi_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				try
				{
					List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
					Dictionary<string, int> dictionary = new Dictionary<string, int>();
					Queue<string> queue = new Queue<string>();
					foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
					{
						string item = selectedRow.Cells["id"].Value.ToString();
						string text = ((selectedRow.Cells["device"].Value != null) ? selectedRow.Cells["device"].Value.ToString() : "");
						if (!(text == ""))
						{
							if (!dictionary.ContainsKey(text))
							{
								dictionary.Add(text, 1);
							}
							else
							{
								dictionary[text]++;
							}
						}
						else
						{
							queue.Enqueue(item);
						}
					}
					foreach (string item2 in listSerialDevice)
					{
						if (!dictionary.ContainsKey(item2))
						{
							dictionary.Add(item2, 0);
						}
					}
					progressBar1.Maximum = queue.Count;
					progressBar1.Minimum = 0;
					progressBar1.Value = 0;
					if (!File.Exists(CaChuaConstant.PHONE_MODE_DATA))
					{
						File.WriteAllText(CaChuaConstant.PHONE_MODE_DATA, new JavaScriptSerializer().Serialize(PhoneMode.NonRoot));
						Thread.Sleep(1000);
					}
					int num = ((new JavaScriptSerializer().Deserialize<PhoneMode>(Utils.ReadTextFile(CaChuaConstant.PHONE_MODE_DATA)) == PhoneMode.NonRoot) ? 8 : int.MaxValue);
					while (queue != null && queue.Count > 0)
					{
						int num2 = num;
						string text2 = "";
						foreach (KeyValuePair<string, int> item3 in dictionary)
						{
							if (item3.Value < num2)
							{
								num2 = item3.Value;
								text2 = item3.Key;
							}
						}
						if (!dictionary.ContainsKey(text2))
						{
							progressBar1.Value = progressBar1.Maximum;
							break;
						}
						dictionary[text2]++;
						string arg = queue.Dequeue();
						sql.ExecuteQuery(string.Format("Update Account set device='{1}' where id='{0}'", arg, text2));
						if (progressBar1.Value < progressBar1.Maximum)
						{
							progressBar1.Value++;
						}
					}
				}
				catch (Exception ex)
				{
					Utils.CCKLog(ex.Message, "mnuChiaThietbi_Click");
				}
				btnLoadAccount_Click(null, null);
			}).Start();
		}

		private void chiabyhand_Click(object sender, EventArgs e)
		{
			frmPhoneDialog frmPhoneDialog = new frmPhoneDialog();
			frmPhoneDialog.ShowDialog();
			string ret = frmPhoneDialog.DeviceId;
			if (ret == null)
			{
				return;
			}
			new Task(delegate
			{
				DataGridViewSelectedRowCollection selectedRows = dataGridView1.SelectedRows;
				progressBar1.Maximum = selectedRows.Count;
				progressBar1.Minimum = 0;
				foreach (DataGridViewRow item in selectedRows)
				{
					string text = item.Cells["id"].Value.ToString();
					if (sql == null)
					{
						sql = new SQLiteUtils();
					}
					if (text != "")
					{
						sql.ExecuteQuery(string.Format("Update Account set device='{1}' where id='{0}'", text, ret));
						if (progressBar1.Value < progressBar1.Maximum)
						{
							progressBar1.Value++;
						}
					}
				}
				MessageBox.Show("Hoàn thành");
				btnLoadAccount_Click(null, null);
			}).Start();
		}

		private void rbtProxyFB_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.NETWORK, new JavaScriptSerializer().Serialize(Networking.ProxyFB));
		}

		private void linkLabel25_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string proxyFB_COM = CaChuaConstant.ProxyFB_COM;
			frmInputControl frmInputControl = new frmInputControl(isSingleLine: false, "ProxyFB Proxy Key");
			frmInputControl.Text = "ProxyFB mỗi key 1 dòng";
			if (File.Exists(proxyFB_COM))
			{
				frmInputControl.Result = Utils.ReadTextFile(proxyFB_COM);
				frmInputControl.SetText(frmInputControl.Result);
			}
			frmInputControl.ShowDialog();
			File.WriteAllText(proxyFB_COM, frmInputControl.Result);
		}

		private void btnFilter_Click(object sender, EventArgs e)
		{
			frmPhoneDialogMulti frmPhoneDialogMulti = new frmPhoneDialogMulti();
			frmPhoneDialogMulti.ShowDialog();
			List<string> deviceId = frmPhoneDialogMulti.DeviceId;
			if (deviceId == null || deviceId.Count <= 0)
			{
				return;
			}
			new SQLiteUtils();
			List<string> list = new List<string>();
			for (int i = 0; i < cbxCategory.Items.Count; i++)
			{
				if (cbxCategory.GetItemChecked(i))
				{
					list.Add(((Category)cbxCategory.Items[i]).tendanhmuc.ToString().Replace("'", ""));
				}
			}
			tbl = new SQLiteUtils().ExecuteQuery(string.Format("Select 0 as Stt,a.id as [ID], a.name as [Name],password as [Password], email as [Email],passemail, cookies as [Cookie], PrivateKey as [PrivateKey], postcount as [Video], proxy as [Proxy], b.tendanhmuc as [Category]  , Brand,trangthai as [Status],likecount as Like, banhang as [Hoa Hong], today as [Today], yesterday as [Yesterday], follower as Follower,following as Following, avatar as [Avatar], year as [Year], profile as [Public],datecreate as [Created], tuongtacngay as [Active Date],ghichu as [Ghi Chu],c.name as [Phone Name],Device,Noti as Log from Account a inner join danhmuc b on a.id_danhmuc = b.id_danhmuc left join tblDevices c on a.Device = c.deviceid Where b.tendanhmuc in ('{1}') and Device in ('{0}')", string.Join("','", deviceId), string.Join("','", list)));
			if (tbl != null && tbl.Rows.Count > 0)
			{
				if (!tbl.Columns.Contains("Stt"))
				{
					DataColumn dataColumn = tbl.Columns.Add("Stt", Type.GetType("System.Int32"));
					dataColumn.SetOrdinal(0);
				}
				for (int j = 0; j < tbl.Rows.Count; j++)
				{
					tbl.Rows[j]["Stt"] = j + 1;
				}
			}
			BindObjectList(dataGridView1, tbl);
			LoadDeviceByFilter(deviceId);
			AddLogToContextMenu();
		}

		private void xoaLogToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				DataGridViewSelectedRowCollection selectedRows = dataGridView1.SelectedRows;
				progressBar1.Maximum = selectedRows.Count;
				progressBar1.Minimum = 0;
				foreach (DataGridViewRow item in selectedRows)
				{
					string text = item.Cells["id"].Value.ToString();
					if (sql == null)
					{
						sql = new SQLiteUtils();
					}
					if (text != "")
					{
						sql.ExecuteQuery($"Update Account set Noti='' where id='{text}'");
						if (progressBar1.Value < progressBar1.Maximum)
						{
							progressBar1.Value++;
						}
					}
				}
			}).Start();
		}

		private void txtKeyword_TextChanged(object sender, EventArgs e)
		{
		}

		private void txtKeyword_Click(object sender, EventArgs e)
		{
			frmInputControl frmInputControl = new frmInputControl(isSingleLine: false, "Tìm kiếm theo nhiều UID, Email", "Tìm kiếm theo nhiều UID, mỗi UID hoặc Email 1 dòng");
			frmInputControl.ShowDialog();
			txtKeyword.Text = string.Join("','", frmInputControl.Result.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
			if (txtKeyword.Text != "")
			{
				btnSearch_Click(null, null);
			}
		}

		private void linkLabel24_LinkClicked_2(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmUpdateInfo frmUpdateInfo = new frmUpdateInfo();
			frmUpdateInfo.StartPosition = FormStartPosition.CenterScreen;
			frmUpdateInfo.ShowDialog();
		}

		private void xoaProxy_Click(object sender, EventArgs e)
		{
			DataGridViewSelectedRowCollection selectedRows = dataGridView1.SelectedRows;
			progressBar1.Maximum = selectedRows.Count;
			progressBar1.Minimum = 0;
			progressBar1.Value = 0;
			for (int i = 0; i < selectedRows.Count; i++)
			{
				try
				{
					string id = selectedRows[i].Cells["Id"].Value.ToString();
					if (sql == null)
					{
						sql = new SQLiteUtils();
					}
					sql.UpdateProxy(id, "");
					if (progressBar1.Value < progressBar1.Maximum)
					{
						progressBar1.Value++;
					}
				}
				catch
				{
				}
			}
		}

		private void copyUIDPasswordToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				List<string> lstTmp = new List<string>();
				foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
				{
					if (selectedRow.Cells["id"].Value != null)
					{
						lstTmp.Add(Utils.Convert2String(selectedRow.Cells["id"].Value) + "|" + Utils.Convert2String(selectedRow.Cells["password"].Value));
					}
				}
				Invoke((Action)delegate
				{
					Clipboard.SetText(string.Join(Environment.NewLine, lstTmp));
				});
				lstTmp.Clear();
			}).Start();
		}

		private void dataGridViewPhone_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Right)
			{
				return;
			}
			ContextMenu i = new ContextMenu();
			MenuItem menuItem = new MenuItem("Start");
			menuItem.Name = "startmanualphone";
			if (lstUid.Count > 0)
			{
				i.MenuItems.Add(menuItem);
			}
			menuItem = new MenuItem("Xem điện thoại");
			menuItem.Name = "viewphone";
			i.MenuItems.Add(menuItem);
			menuItem = new MenuItem("Devices Configuration");
			menuItem.Name = "Configuration";
			i.MenuItems.Add(menuItem);
			foreach (MenuItem menuItem2 in i.MenuItems)
			{
				menuItem2.Click += FaceBookCreator_Click;
			}
			Invoke((Action)delegate
			{
				i.Show(dataGridViewPhone, new Point(e.X, e.Y));
			});
		}

		private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex != -1)
			{
				string uid = dataGridView1.Rows[e.RowIndex].Cells["id"].Value.ToString();
				frmAccountInfo frmAccountInfo2 = new frmAccountInfo();
				frmAccountInfo2.Uid = uid;
				frmAccountInfo2.ShowDialog();
			}
		}

		private void cookiesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string ccFolder = Application.StartupPath + "\\Cookies";
			if (!Directory.Exists(ccFolder))
			{
				Directory.CreateDirectory(ccFolder);
			}
			Task.Run(delegate
			{
				bool flag = false;
				List<string> lstTmp = new List<string>();
				foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
				{
					if (selectedRow.Cells["Cookie"].Value != null)
					{
						string text = selectedRow.Cells["Cookie"].Value.ToString();
						string text2 = Application.StartupPath + "\\Authentication\\" + text + "\\cck_cookie.txt";
						if (File.Exists(text2))
						{
							try
							{
								flag = true;
								File.Copy(text2, ccFolder + "\\" + text + ".txt", overwrite: true);
							}
							catch
							{
							}
						}
						lstTmp.Add(selectedRow.Cells["Cookie"].Value.ToString());
					}
				}
				if (lstTmp.Count > 0)
				{
					Invoke((Action)delegate
					{
						Clipboard.SetText(string.Join(Environment.NewLine, lstTmp));
						MessageBox.Show("Đã Copy / Copied");
					});
				}
				lstTmp.Clear();
				if (flag)
				{
					Process.Start(ccFolder);
				}
			});
		}

		private void xemChromeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				List<string> lstTmp = new List<string>();
				Dictionary<string, int> dicIndex = new Dictionary<string, int>();
				foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
				{
					if (selectedRow.Cells["id"].Value != null)
					{
						lstTmp.Add(selectedRow.Cells["id"].Value.ToString());
						dicIndex.Add(selectedRow.Cells["id"].Value.ToString(), selectedRow.Index);
					}
				}
				for (int i = 0; i < 6; i++)
				{
					new Task(delegate
					{
						while (lstTmp.Count > 0)
						{
							if (lstTmp.Count > 0)
							{
								string uid = lstTmp[0];
								lstTmp.RemoveAt(0);
								try
								{
									Utils.ViewInChrome(uid);
								}
								catch
								{
								}
							}
							if (lstTmp.Count == 0)
							{
								dicIndex.Clear();
							}
						}
					}).Start();
				}
			}).Start();
		}

		private void btnRender_Click(object sender, EventArgs e)
		{
			StartRendering();
		}

		private void StartRendering()
		{
			string value = "ffmpeg -i input.mp4 -vf hflip -c:a copy " + outputVideoPath;
			ffmpegProcess = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "cmd.exe",
					RedirectStandardInput = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true
				},
				EnableRaisingEvents = true
			};
			ffmpegProcess.OutputDataReceived += OutputHandler;
			ffmpegProcess.Start();
			ffmpegProcess.BeginOutputReadLine();
			ffmpegProcess.StandardInput.WriteLine(value);
			ffmpegProcess.StandardInput.WriteLine("exit");
			ffmpegProcess.WaitForExit();
			ShowRenderedVideo();
		}

		private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
		{
			if (!string.IsNullOrEmpty(outLine.Data))
			{
				Console.WriteLine(outLine.Data);
				if (!outLine.Data.Contains("frame="))
				{
				}
			}
		}

		private void ShowRenderedVideo()
		{
		}

		private static IEnumerable<string> GetDistinctLogValues(DataTable dataTable, string fields)
		{
			return (from row in dataTable.AsEnumerable()
				select row.Field<string>(fields)).Distinct();
		}

		private void notesToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private void logToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private void copyUIDPassToolStripMenuItem_Click_1(object sender, EventArgs e)
		{
			new Task(delegate
			{
				List<string> lstTmp = new List<string>();
				foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
				{
					if (selectedRow.Cells["Password"].Value != null)
					{
						lstTmp.Add(selectedRow.Cells["Id"].Value.ToString() + "|" + selectedRow.Cells["Password"].Value.ToString());
					}
				}
				if (lstTmp.Count > 0)
				{
					Invoke((Action)delegate
					{
						Clipboard.SetText(string.Join(Environment.NewLine, lstTmp));
					});
				}
				lstTmp.Clear();
			}).Start();
		}

		private void groupProfile_Enter(object sender, EventArgs e)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CCKTiktok.frmMain));
			progressBar1 = new System.Windows.Forms.ToolStripProgressBar();
			openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			statusStrip1 = new System.Windows.Forms.StatusStrip();
			toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
			lblKey = new System.Windows.Forms.ToolStripStatusLabel();
			toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			lblSub = new System.Windows.Forms.ToolStripStatusLabel();
			tssRam = new System.Windows.Forms.ToolStripStatusLabel();
			lblRemain = new System.Windows.Forms.ToolStripStatusLabel();
			lblWorking = new System.Windows.Forms.ToolStripStatusLabel();
			cbxchon = new System.Windows.Forms.CheckBox();
			grbAction = new System.Windows.Forms.GroupBox();
			groupBox6 = new System.Windows.Forms.GroupBox();
			btnFilter = new System.Windows.Forms.Button();
			btnShow = new System.Windows.Forms.Button();
			button4 = new System.Windows.Forms.Button();
			button3 = new System.Windows.Forms.Button();
			button2 = new System.Windows.Forms.Button();
			btnReport = new System.Windows.Forms.Button();
			btnLoadDevices = new System.Windows.Forms.Button();
			btnPhone = new System.Windows.Forms.Button();
			button1 = new System.Windows.Forms.Button();
			btnStop = new System.Windows.Forms.Button();
			btnReg = new System.Windows.Forms.Button();
			btn2fa = new System.Windows.Forms.Button();
			gbAction = new System.Windows.Forms.GroupBox();
			groupBox3 = new System.Windows.Forms.GroupBox();
			btnSearch = new System.Windows.Forms.Button();
			txtKeyword = new System.Windows.Forms.TextBox();
			groupAction = new System.Windows.Forms.GroupBox();
			linkLabel22 = new System.Windows.Forms.LinkLabel();
			linkLabel19 = new System.Windows.Forms.LinkLabel();
			cbxTreoNick = new System.Windows.Forms.CheckBox();
			cbxFollower = new System.Windows.Forms.CheckBox();
			linkLabel10 = new System.Windows.Forms.LinkLabel();
			cbxFollowSuggested = new System.Windows.Forms.CheckBox();
			linkLabel5 = new System.Windows.Forms.LinkLabel();
			cbxSeedingVideo = new System.Windows.Forms.CheckBox();
			linkLabel4 = new System.Windows.Forms.LinkLabel();
			cbxFolowUID = new System.Windows.Forms.CheckBox();
			linkLabel3 = new System.Windows.Forms.LinkLabel();
			cbxCommentRandom = new System.Windows.Forms.CheckBox();
			linkLabel2 = new System.Windows.Forms.LinkLabel();
			cbxNewsFeed = new System.Windows.Forms.CheckBox();
			linkLabel1 = new System.Windows.Forms.LinkLabel();
			cbxLike = new System.Windows.Forms.CheckBox();
			lblViewKeyword = new System.Windows.Forms.LinkLabel();
			cbxKeyword = new System.Windows.Forms.CheckBox();
			groupBox5 = new System.Windows.Forms.GroupBox();
			rbtProxyFB = new System.Windows.Forms.RadioButton();
			linkLabel25 = new System.Windows.Forms.LinkLabel();
			rbtShopLive = new System.Windows.Forms.RadioButton();
			linkLabel20 = new System.Windows.Forms.LinkLabel();
			rbtHMA = new System.Windows.Forms.RadioButton();
			linkLabel13 = new System.Windows.Forms.LinkLabel();
			cbxWifi = new System.Windows.Forms.RadioButton();
			rbtTM = new System.Windows.Forms.RadioButton();
			linkLabel7 = new System.Windows.Forms.LinkLabel();
			rbtXProxy = new System.Windows.Forms.RadioButton();
			linkLabel8 = new System.Windows.Forms.LinkLabel();
			rbtProxy = new System.Windows.Forms.RadioButton();
			rpt3g = new System.Windows.Forms.RadioButton();
			lblProxy = new System.Windows.Forms.LinkLabel();
			lbl4g3g = new System.Windows.Forms.LinkLabel();
			groupProfile = new System.Windows.Forms.GroupBox();
			cbxRemoveAccount = new System.Windows.Forms.CheckBox();
			cbxVeri = new System.Windows.Forms.CheckBox();
			cbxChangeEmail = new System.Windows.Forms.CheckBox();
			cbxRecover = new System.Windows.Forms.CheckBox();
			cbxUpdate = new System.Windows.Forms.CheckBox();
			linkLabel11 = new System.Windows.Forms.LinkLabel();
			linkLabel24 = new System.Windows.Forms.LinkLabel();
			linkLabel15 = new System.Windows.Forms.LinkLabel();
			cbxBio = new System.Windows.Forms.CheckBox();
			linkLabel21 = new System.Windows.Forms.LinkLabel();
			linkLabel6 = new System.Windows.Forms.LinkLabel();
			cbxChangeAvatar = new System.Windows.Forms.CheckBox();
			cbxUpVideo = new System.Windows.Forms.CheckBox();
			linkLabel14 = new System.Windows.Forms.LinkLabel();
			cbxChangePass = new System.Windows.Forms.CheckBox();
			cbxRename = new System.Windows.Forms.CheckBox();
			linkLabel12 = new System.Windows.Forms.LinkLabel();
			groupShop = new System.Windows.Forms.GroupBox();
			lblEarnSu = new System.Windows.Forms.LinkLabel();
			linkLabel23 = new System.Windows.Forms.LinkLabel();
			cbxAddProduct = new System.Windows.Forms.CheckBox();
			cbxRotate = new System.Windows.Forms.CheckBox();
			cbxEarnSu = new System.Windows.Forms.CheckBox();
			cbxAccept = new System.Windows.Forms.CheckBox();
			cbxPublic = new System.Windows.Forms.CheckBox();
			cbxBalance = new System.Windows.Forms.CheckBox();
			linkLabel17 = new System.Windows.Forms.LinkLabel();
			cbxTDSFollow = new System.Windows.Forms.CheckBox();
			linkLabel16 = new System.Windows.Forms.LinkLabel();
			cbxTDSComment = new System.Windows.Forms.CheckBox();
			linkLabel9 = new System.Windows.Forms.LinkLabel();
			cbxTDSTym = new System.Windows.Forms.CheckBox();
			cbxCategory = new System.Windows.Forms.CheckedListBox();
			grDevice = new System.Windows.Forms.GroupBox();
			dataGridViewPhone = new System.Windows.Forms.DataGridView();
			btnLoadAccount = new System.Windows.Forms.Button();
			groupBox1 = new System.Windows.Forms.GroupBox();
			cbxDie = new System.Windows.Forms.CheckBox();
			cbxError = new System.Windows.Forms.CheckBox();
			cbxnew = new System.Windows.Forms.CheckBox();
			dataGridView1 = new System.Windows.Forms.DataGridView();
			metroContextMenu = new MetroFramework.Controls.MetroContextMenu(components);
			changeCategory = new System.Windows.Forms.ToolStripMenuItem();
			accountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			deleteBackupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			loginbyhanTayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			loctrungToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			kiemTraBackupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			exportBackupAccountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			gawnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			xoaProxy = new System.Windows.Forms.ToolStripMenuItem();
			xoasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			mnuChiaThietbi = new System.Windows.Forms.ToolStripMenuItem();
			chiabyhand = new System.Windows.Forms.ToolStripMenuItem();
			xemChromeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			copyUIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			copyPasswordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			copyEmailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			copyPasswordEmailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			copyUIDPassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			copyUIDPassEmailPassEmailDeviceInfoToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			copyUIDPassEmailPassEmailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			copyEmailPassEmailToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			cookiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			checkLiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			unlockHotmailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			addNoteoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			refreshClearADBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			xoaLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			filterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			notesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			statusStrip1.SuspendLayout();
			grbAction.SuspendLayout();
			groupBox6.SuspendLayout();
			gbAction.SuspendLayout();
			groupBox3.SuspendLayout();
			groupAction.SuspendLayout();
			groupBox5.SuspendLayout();
			groupProfile.SuspendLayout();
			groupShop.SuspendLayout();
			grDevice.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)dataGridViewPhone).BeginInit();
			groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
			metroContextMenu.SuspendLayout();
			SuspendLayout();
			progressBar1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			progressBar1.ForeColor = System.Drawing.Color.Red;
			progressBar1.Name = "progressBar1";
			progressBar1.Size = new System.Drawing.Size(500, 16);
			progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			openFileDialog1.FileName = "openFileDialog1";
			statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[8] { toolStripStatusLabel2, progressBar1, lblKey, toolStripStatusLabel1, lblSub, tssRam, lblRemain, lblWorking });
			statusStrip1.Location = new System.Drawing.Point(0, 599);
			statusStrip1.Name = "statusStrip1";
			statusStrip1.Size = new System.Drawing.Size(1382, 22);
			statusStrip1.TabIndex = 27;
			statusStrip1.Text = "statusStrip1";
			toolStripStatusLabel2.Name = "toolStripStatusLabel2";
			toolStripStatusLabel2.Size = new System.Drawing.Size(32, 17);
			toolStripStatusLabel2.Text = "Total";
			lblKey.LinkColor = System.Drawing.Color.Red;
			lblKey.Name = "lblKey";
			lblKey.Size = new System.Drawing.Size(65, 17);
			lblKey.Text = "Expire date";
			toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			toolStripStatusLabel1.Size = new System.Drawing.Size(67, 17);
			toolStripStatusLabel1.Text = "CCK Phone";
			lblSub.ActiveLinkColor = System.Drawing.Color.Blue;
			lblSub.Name = "lblSub";
			lblSub.Size = new System.Drawing.Size(77, 17);
			lblSub.Text = "Gói 10 Phone";
			tssRam.LinkColor = System.Drawing.Color.Red;
			tssRam.Name = "tssRam";
			tssRam.Size = new System.Drawing.Size(31, 17);
			tssRam.Text = "Ram";
			lblRemain.Name = "lblRemain";
			lblRemain.Size = new System.Drawing.Size(13, 17);
			lblRemain.Text = "0";
			lblWorking.Name = "lblWorking";
			lblWorking.Size = new System.Drawing.Size(13, 17);
			lblWorking.Text = "0";
			cbxchon.AutoSize = true;
			cbxchon.Location = new System.Drawing.Point(20, 201);
			cbxchon.Margin = new System.Windows.Forms.Padding(2);
			cbxchon.Name = "cbxchon";
			cbxchon.Size = new System.Drawing.Size(70, 17);
			cbxchon.TabIndex = 45;
			cbxchon.Text = "Select All";
			cbxchon.UseVisualStyleBackColor = true;
			cbxchon.CheckedChanged += new System.EventHandler(cbxchon_CheckedChanged);
			grbAction.Controls.Add(groupBox6);
			grbAction.Controls.Add(gbAction);
			grbAction.Dock = System.Windows.Forms.DockStyle.Top;
			grbAction.Location = new System.Drawing.Point(0, 0);
			grbAction.Margin = new System.Windows.Forms.Padding(0);
			grbAction.Name = "grbAction";
			grbAction.Padding = new System.Windows.Forms.Padding(0);
			grbAction.Size = new System.Drawing.Size(1382, 296);
			grbAction.TabIndex = 26;
			grbAction.TabStop = false;
			grbAction.Enter += new System.EventHandler(groupBox3_Enter);
			groupBox6.Controls.Add(btnFilter);
			groupBox6.Controls.Add(btnShow);
			groupBox6.Controls.Add(button4);
			groupBox6.Controls.Add(button3);
			groupBox6.Controls.Add(button2);
			groupBox6.Controls.Add(btnReport);
			groupBox6.Controls.Add(btnLoadDevices);
			groupBox6.Controls.Add(btnPhone);
			groupBox6.Controls.Add(button1);
			groupBox6.Controls.Add(btnStop);
			groupBox6.Controls.Add(btnReg);
			groupBox6.Controls.Add(btn2fa);
			groupBox6.Dock = System.Windows.Forms.DockStyle.Bottom;
			groupBox6.Location = new System.Drawing.Point(0, 251);
			groupBox6.Margin = new System.Windows.Forms.Padding(2);
			groupBox6.Name = "groupBox6";
			groupBox6.Padding = new System.Windows.Forms.Padding(2);
			groupBox6.Size = new System.Drawing.Size(1382, 45);
			groupBox6.TabIndex = 150;
			groupBox6.TabStop = false;
			btnFilter.BackColor = System.Drawing.SystemColors.ControlDark;
			btnFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			btnFilter.ForeColor = System.Drawing.Color.White;
			btnFilter.Image = CCKTiktok.Properties.Resources.e_learning_25px;
			btnFilter.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			btnFilter.Location = new System.Drawing.Point(987, 9);
			btnFilter.Margin = new System.Windows.Forms.Padding(2);
			btnFilter.Name = "btnFilter";
			btnFilter.Size = new System.Drawing.Size(112, 32);
			btnFilter.TabIndex = 161;
			btnFilter.Text = "Chọn Phone";
			btnFilter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			btnFilter.UseVisualStyleBackColor = false;
			btnFilter.Click += new System.EventHandler(btnFilter_Click);
			btnShow.BackColor = System.Drawing.SystemColors.ControlDark;
			btnShow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnShow.Font = new System.Drawing.Font("Microsoft Sans Serif", 8f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			btnShow.ForeColor = System.Drawing.Color.White;
			btnShow.Image = CCKTiktok.Properties.Resources.reset_hma;
			btnShow.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			btnShow.Location = new System.Drawing.Point(1103, 9);
			btnShow.Margin = new System.Windows.Forms.Padding(2);
			btnShow.Name = "btnShow";
			btnShow.Size = new System.Drawing.Size(121, 32);
			btnShow.TabIndex = 152;
			btnShow.Text = "Render Video";
			btnShow.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			btnShow.UseVisualStyleBackColor = false;
			btnShow.Click += new System.EventHandler(btnShow_Click);
			button4.BackColor = System.Drawing.SystemColors.ControlDark;
			button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			button4.ForeColor = System.Drawing.Color.White;
			button4.Image = CCKTiktok.Properties.Resources.add_user_group_man_man_25px;
			button4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			button4.Location = new System.Drawing.Point(127, 9);
			button4.Margin = new System.Windows.Forms.Padding(2);
			button4.Name = "button4";
			button4.Size = new System.Drawing.Size(119, 32);
			button4.TabIndex = 151;
			button4.Text = "Add Account";
			button4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			button4.UseVisualStyleBackColor = false;
			button4.Click += new System.EventHandler(button4_Click);
			button3.BackColor = System.Drawing.SystemColors.ControlDark;
			button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			button3.ForeColor = System.Drawing.Color.White;
			button3.Image = CCKTiktok.Properties.Resources.double_tick_25px;
			button3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			button3.Location = new System.Drawing.Point(3, 9);
			button3.Margin = new System.Windows.Forms.Padding(2);
			button3.Name = "button3";
			button3.Size = new System.Drawing.Size(119, 32);
			button3.TabIndex = 150;
			button3.Text = "Add Category";
			button3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			button3.UseVisualStyleBackColor = false;
			button3.Click += new System.EventHandler(button3_Click);
			button2.BackColor = System.Drawing.SystemColors.ControlDark;
			button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			button2.ForeColor = System.Drawing.Color.White;
			button2.Image = CCKTiktok.Properties.Resources.money_bag_25px;
			button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			button2.Location = new System.Drawing.Point(586, 9);
			button2.Margin = new System.Windows.Forms.Padding(2);
			button2.Name = "button2";
			button2.Size = new System.Drawing.Size(81, 32);
			button2.TabIndex = 149;
			button2.Text = "Money";
			button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			button2.UseVisualStyleBackColor = false;
			button2.Click += new System.EventHandler(button2_Click_1);
			btnReport.BackColor = System.Drawing.SystemColors.Highlight;
			btnReport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			btnReport.ForeColor = System.Drawing.Color.White;
			btnReport.Image = CCKTiktok.Properties.Resources.new_25px;
			btnReport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			btnReport.Location = new System.Drawing.Point(898, 9);
			btnReport.Margin = new System.Windows.Forms.Padding(2);
			btnReport.Name = "btnReport";
			btnReport.Size = new System.Drawing.Size(85, 32);
			btnReport.TabIndex = 148;
			btnReport.Text = "Update";
			btnReport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			btnReport.UseVisualStyleBackColor = false;
			btnReport.Click += new System.EventHandler(btnReport_Click_1);
			btnLoadDevices.BackColor = System.Drawing.SystemColors.ControlDark;
			btnLoadDevices.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnLoadDevices.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			btnLoadDevices.ForeColor = System.Drawing.Color.White;
			btnLoadDevices.Image = CCKTiktok.Properties.Resources.find_and_replace_25px;
			btnLoadDevices.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			btnLoadDevices.Location = new System.Drawing.Point(782, 9);
			btnLoadDevices.Margin = new System.Windows.Forms.Padding(2);
			btnLoadDevices.Name = "btnLoadDevices";
			btnLoadDevices.Size = new System.Drawing.Size(112, 32);
			btnLoadDevices.TabIndex = 110;
			btnLoadDevices.Text = "Load Phone";
			btnLoadDevices.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			btnLoadDevices.UseVisualStyleBackColor = false;
			btnLoadDevices.Click += new System.EventHandler(btnLoadDevices_Click);
			btnPhone.BackColor = System.Drawing.SystemColors.ControlDark;
			btnPhone.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnPhone.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			btnPhone.ForeColor = System.Drawing.Color.Transparent;
			btnPhone.Image = CCKTiktok.Properties.Resources.checked_checkbox_25px;
			btnPhone.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			btnPhone.Location = new System.Drawing.Point(403, 9);
			btnPhone.Margin = new System.Windows.Forms.Padding(2);
			btnPhone.Name = "btnPhone";
			btnPhone.Size = new System.Drawing.Size(90, 32);
			btnPhone.TabIndex = 104;
			btnPhone.Text = "Devices";
			btnPhone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			btnPhone.UseVisualStyleBackColor = false;
			btnPhone.Click += new System.EventHandler(btnPhone_Click);
			button1.BackColor = System.Drawing.SystemColors.ControlDark;
			button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			button1.ForeColor = System.Drawing.Color.Transparent;
			button1.Image = CCKTiktok.Properties.Resources.connection_status_off_25px;
			button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			button1.Location = new System.Drawing.Point(498, 9);
			button1.Margin = new System.Windows.Forms.Padding(2);
			button1.Name = "button1";
			button1.Size = new System.Drawing.Size(83, 32);
			button1.TabIndex = 104;
			button1.Text = "Config";
			button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			button1.UseVisualStyleBackColor = false;
			button1.Click += new System.EventHandler(button1_Click_2);
			btnStop.BackColor = System.Drawing.Color.Red;
			btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			btnStop.ForeColor = System.Drawing.Color.White;
			btnStop.Image = CCKTiktok.Properties.Resources.cancel_25px;
			btnStop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			btnStop.Location = new System.Drawing.Point(330, 9);
			btnStop.Margin = new System.Windows.Forms.Padding(2);
			btnStop.Name = "btnStop";
			btnStop.Size = new System.Drawing.Size(68, 32);
			btnStop.TabIndex = 16;
			btnStop.Text = "Stop";
			btnStop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			btnStop.UseVisualStyleBackColor = false;
			btnStop.Click += new System.EventHandler(btnStop_Click);
			btnReg.BackColor = System.Drawing.SystemColors.ControlDark;
			btnReg.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnReg.Font = new System.Drawing.Font("Microsoft Sans Serif", 8f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			btnReg.ForeColor = System.Drawing.Color.White;
			btnReg.Image = CCKTiktok.Properties.Resources.add_user_male_25px;
			btnReg.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			btnReg.Location = new System.Drawing.Point(672, 9);
			btnReg.Margin = new System.Windows.Forms.Padding(2);
			btnReg.Name = "btnReg";
			btnReg.Size = new System.Drawing.Size(94, 32);
			btnReg.TabIndex = 143;
			btnReg.Text = "Register";
			btnReg.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			btnReg.UseVisualStyleBackColor = false;
			btnReg.Click += new System.EventHandler(btnReg_Click_1);
			btn2fa.BackColor = System.Drawing.Color.Green;
			btn2fa.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btn2fa.Font = new System.Drawing.Font("Microsoft Sans Serif", 8f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			btn2fa.ForeColor = System.Drawing.Color.White;
			btn2fa.Image = CCKTiktok.Properties.Resources.icons8_play_23px;
			btn2fa.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			btn2fa.Location = new System.Drawing.Point(251, 9);
			btn2fa.Margin = new System.Windows.Forms.Padding(2);
			btn2fa.Name = "btn2fa";
			btn2fa.Size = new System.Drawing.Size(74, 32);
			btn2fa.TabIndex = 7;
			btn2fa.Text = "Start";
			btn2fa.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			btn2fa.UseVisualStyleBackColor = false;
			btn2fa.Visible = false;
			btn2fa.Click += new System.EventHandler(btn2fa_Click);
			gbAction.Controls.Add(groupBox3);
			gbAction.Controls.Add(groupAction);
			gbAction.Controls.Add(groupBox5);
			gbAction.Controls.Add(groupProfile);
			gbAction.Controls.Add(groupShop);
			gbAction.Controls.Add(cbxCategory);
			gbAction.Controls.Add(grDevice);
			gbAction.Controls.Add(btnLoadAccount);
			gbAction.Controls.Add(groupBox1);
			gbAction.Controls.Add(cbxchon);
			gbAction.Dock = System.Windows.Forms.DockStyle.Top;
			gbAction.Location = new System.Drawing.Point(0, 13);
			gbAction.Name = "gbAction";
			gbAction.Size = new System.Drawing.Size(1382, 239);
			gbAction.TabIndex = 151;
			gbAction.TabStop = false;
			groupBox3.Controls.Add(btnSearch);
			groupBox3.Controls.Add(txtKeyword);
			groupBox3.Location = new System.Drawing.Point(633, 170);
			groupBox3.Name = "groupBox3";
			groupBox3.Size = new System.Drawing.Size(140, 52);
			groupBox3.TabIndex = 148;
			groupBox3.TabStop = false;
			groupBox3.Text = "Search";
			btnSearch.Image = CCKTiktok.Properties.Resources.find_and_replace_25px;
			btnSearch.Location = new System.Drawing.Point(101, 14);
			btnSearch.Name = "btnSearch";
			btnSearch.Size = new System.Drawing.Size(32, 32);
			btnSearch.TabIndex = 1;
			btnSearch.UseVisualStyleBackColor = true;
			btnSearch.Click += new System.EventHandler(btnSearch_Click);
			txtKeyword.Location = new System.Drawing.Point(7, 21);
			txtKeyword.Name = "txtKeyword";
			txtKeyword.Size = new System.Drawing.Size(88, 20);
			txtKeyword.TabIndex = 0;
			txtKeyword.Click += new System.EventHandler(txtKeyword_Click);
			txtKeyword.TextChanged += new System.EventHandler(txtKeyword_TextChanged);
			groupAction.Controls.Add(linkLabel22);
			groupAction.Controls.Add(linkLabel19);
			groupAction.Controls.Add(cbxTreoNick);
			groupAction.Controls.Add(cbxFollower);
			groupAction.Controls.Add(linkLabel10);
			groupAction.Controls.Add(cbxFollowSuggested);
			groupAction.Controls.Add(linkLabel5);
			groupAction.Controls.Add(cbxSeedingVideo);
			groupAction.Controls.Add(linkLabel4);
			groupAction.Controls.Add(cbxFolowUID);
			groupAction.Controls.Add(linkLabel3);
			groupAction.Controls.Add(cbxCommentRandom);
			groupAction.Controls.Add(linkLabel2);
			groupAction.Controls.Add(cbxNewsFeed);
			groupAction.Controls.Add(linkLabel1);
			groupAction.Controls.Add(cbxLike);
			groupAction.Controls.Add(lblViewKeyword);
			groupAction.Controls.Add(cbxKeyword);
			groupAction.Location = new System.Drawing.Point(207, 10);
			groupAction.Name = "groupAction";
			groupAction.Size = new System.Drawing.Size(145, 212);
			groupAction.TabIndex = 111;
			groupAction.TabStop = false;
			groupAction.Text = "Action";
			linkLabel22.AutoSize = true;
			linkLabel22.Location = new System.Drawing.Point(23, 189);
			linkLabel22.Name = "linkLabel22";
			linkLabel22.Size = new System.Drawing.Size(52, 13);
			linkLabel22.TabIndex = 161;
			linkLabel22.TabStop = true;
			linkLabel22.Tag = "Sleeping";
			linkLabel22.Text = "Treo nick";
			linkLabel22.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel22_LinkClicked);
			linkLabel19.AutoSize = true;
			linkLabel19.Location = new System.Drawing.Point(23, 167);
			linkLabel19.Name = "linkLabel19";
			linkLabel19.Size = new System.Drawing.Size(76, 13);
			linkLabel19.TabIndex = 145;
			linkLabel19.TabStop = true;
			linkLabel19.Tag = "Follow each other";
			linkLabel19.Text = "Theo dõi chéo";
			linkLabel19.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel19_LinkClicked);
			cbxTreoNick.AutoSize = true;
			cbxTreoNick.Location = new System.Drawing.Point(6, 189);
			cbxTreoNick.Margin = new System.Windows.Forms.Padding(2);
			cbxTreoNick.Name = "cbxTreoNick";
			cbxTreoNick.Size = new System.Drawing.Size(15, 14);
			cbxTreoNick.TabIndex = 160;
			cbxTreoNick.UseVisualStyleBackColor = true;
			cbxFollower.AutoSize = true;
			cbxFollower.Location = new System.Drawing.Point(6, 168);
			cbxFollower.Margin = new System.Windows.Forms.Padding(2);
			cbxFollower.Name = "cbxFollower";
			cbxFollower.Size = new System.Drawing.Size(15, 14);
			cbxFollower.TabIndex = 144;
			cbxFollower.UseVisualStyleBackColor = true;
			linkLabel10.AutoSize = true;
			linkLabel10.Location = new System.Drawing.Point(22, 146);
			linkLabel10.Name = "linkLabel10";
			linkLabel10.Size = new System.Drawing.Size(120, 13);
			linkLabel10.TabIndex = 145;
			linkLabel10.TabStop = true;
			linkLabel10.Tag = "Follow UID by suggestion";
			linkLabel10.Text = "Theo dõi UID theo gợi ý";
			linkLabel10.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel10_LinkClicked_1);
			cbxFollowSuggested.AutoSize = true;
			cbxFollowSuggested.Location = new System.Drawing.Point(6, 147);
			cbxFollowSuggested.Margin = new System.Windows.Forms.Padding(2);
			cbxFollowSuggested.Name = "cbxFollowSuggested";
			cbxFollowSuggested.Size = new System.Drawing.Size(15, 14);
			cbxFollowSuggested.TabIndex = 144;
			cbxFollowSuggested.UseVisualStyleBackColor = true;
			linkLabel5.AutoSize = true;
			linkLabel5.Location = new System.Drawing.Point(23, 104);
			linkLabel5.Name = "linkLabel5";
			linkLabel5.Size = new System.Drawing.Size(112, 13);
			linkLabel5.TabIndex = 139;
			linkLabel5.TabStop = true;
			linkLabel5.Tag = "Seeding Livestream";
			linkLabel5.Text = "Tương tác LiveStream";
			linkLabel5.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel5_LinkClicked_1);
			cbxSeedingVideo.AutoSize = true;
			cbxSeedingVideo.Location = new System.Drawing.Point(6, 105);
			cbxSeedingVideo.Margin = new System.Windows.Forms.Padding(2);
			cbxSeedingVideo.Name = "cbxSeedingVideo";
			cbxSeedingVideo.Size = new System.Drawing.Size(15, 14);
			cbxSeedingVideo.TabIndex = 138;
			cbxSeedingVideo.UseVisualStyleBackColor = true;
			linkLabel4.AutoSize = true;
			linkLabel4.Location = new System.Drawing.Point(23, 84);
			linkLabel4.Name = "linkLabel4";
			linkLabel4.Size = new System.Drawing.Size(71, 13);
			linkLabel4.TabIndex = 137;
			linkLabel4.TabStop = true;
			linkLabel4.Tag = "Follow UID";
			linkLabel4.Text = "Theo dõi UID";
			linkLabel4.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel4_LinkClicked_2);
			cbxFolowUID.AutoSize = true;
			cbxFolowUID.Location = new System.Drawing.Point(6, 84);
			cbxFolowUID.Margin = new System.Windows.Forms.Padding(2);
			cbxFolowUID.Name = "cbxFolowUID";
			cbxFolowUID.Size = new System.Drawing.Size(15, 14);
			cbxFolowUID.TabIndex = 136;
			cbxFolowUID.UseVisualStyleBackColor = true;
			linkLabel3.AutoSize = true;
			linkLabel3.Location = new System.Drawing.Point(23, 63);
			linkLabel3.Name = "linkLabel3";
			linkLabel3.Size = new System.Drawing.Size(88, 13);
			linkLabel3.TabIndex = 135;
			linkLabel3.TabStop = true;
			linkLabel3.Tag = "Comment Post";
			linkLabel3.Text = "Bình luận bài viết";
			linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel3_LinkClicked_1);
			cbxCommentRandom.AutoSize = true;
			cbxCommentRandom.Location = new System.Drawing.Point(6, 63);
			cbxCommentRandom.Margin = new System.Windows.Forms.Padding(2);
			cbxCommentRandom.Name = "cbxCommentRandom";
			cbxCommentRandom.Size = new System.Drawing.Size(15, 14);
			cbxCommentRandom.TabIndex = 134;
			cbxCommentRandom.UseVisualStyleBackColor = true;
			linkLabel2.AutoSize = true;
			linkLabel2.Location = new System.Drawing.Point(23, 21);
			linkLabel2.Name = "linkLabel2";
			linkLabel2.Size = new System.Drawing.Size(58, 13);
			linkLabel2.TabIndex = 133;
			linkLabel2.TabStop = true;
			linkLabel2.Tag = "View Newsfeed";
			linkLabel2.Text = "Lướt tường";
			linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel2_LinkClicked_2);
			cbxNewsFeed.AutoSize = true;
			cbxNewsFeed.Location = new System.Drawing.Point(6, 21);
			cbxNewsFeed.Margin = new System.Windows.Forms.Padding(2);
			cbxNewsFeed.Name = "cbxNewsFeed";
			cbxNewsFeed.Size = new System.Drawing.Size(15, 14);
			cbxNewsFeed.TabIndex = 132;
			cbxNewsFeed.UseVisualStyleBackColor = true;
			linkLabel1.AutoSize = true;
			linkLabel1.Location = new System.Drawing.Point(23, 41);
			linkLabel1.Name = "linkLabel1";
			linkLabel1.Size = new System.Drawing.Size(88, 13);
			linkLabel1.TabIndex = 130;
			linkLabel1.TabStop = true;
			linkLabel1.Tag = "View and Like Post";
			linkLabel1.Text = "Xem và thích bài";
			linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel1_LinkClicked_2);
			cbxLike.AutoSize = true;
			cbxLike.Location = new System.Drawing.Point(6, 42);
			cbxLike.Margin = new System.Windows.Forms.Padding(2);
			cbxLike.Name = "cbxLike";
			cbxLike.Size = new System.Drawing.Size(15, 14);
			cbxLike.TabIndex = 129;
			cbxLike.UseVisualStyleBackColor = true;
			lblViewKeyword.AutoSize = true;
			lblViewKeyword.Location = new System.Drawing.Point(23, 125);
			lblViewKeyword.Name = "lblViewKeyword";
			lblViewKeyword.Size = new System.Drawing.Size(119, 13);
			lblViewKeyword.TabIndex = 143;
			lblViewKeyword.TabStop = true;
			lblViewKeyword.Tag = "Interactive by keyword";
			lblViewKeyword.Text = "Tương tác theo từ khóa";
			lblViewKeyword.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel18_LinkClicked);
			cbxKeyword.AutoSize = true;
			cbxKeyword.Location = new System.Drawing.Point(6, 126);
			cbxKeyword.Margin = new System.Windows.Forms.Padding(2);
			cbxKeyword.Name = "cbxKeyword";
			cbxKeyword.Size = new System.Drawing.Size(15, 14);
			cbxKeyword.TabIndex = 142;
			cbxKeyword.UseVisualStyleBackColor = true;
			groupBox5.Controls.Add(rbtProxyFB);
			groupBox5.Controls.Add(linkLabel25);
			groupBox5.Controls.Add(rbtShopLive);
			groupBox5.Controls.Add(linkLabel20);
			groupBox5.Controls.Add(rbtHMA);
			groupBox5.Controls.Add(linkLabel13);
			groupBox5.Controls.Add(cbxWifi);
			groupBox5.Controls.Add(rbtTM);
			groupBox5.Controls.Add(linkLabel7);
			groupBox5.Controls.Add(rbtXProxy);
			groupBox5.Controls.Add(linkLabel8);
			groupBox5.Controls.Add(rbtProxy);
			groupBox5.Controls.Add(rpt3g);
			groupBox5.Controls.Add(lblProxy);
			groupBox5.Controls.Add(lbl4g3g);
			groupBox5.Location = new System.Drawing.Point(633, 53);
			groupBox5.Margin = new System.Windows.Forms.Padding(2);
			groupBox5.Name = "groupBox5";
			groupBox5.Padding = new System.Windows.Forms.Padding(2);
			groupBox5.Size = new System.Drawing.Size(140, 107);
			groupBox5.TabIndex = 43;
			groupBox5.TabStop = false;
			groupBox5.Text = "Network";
			rbtProxyFB.AutoSize = true;
			rbtProxyFB.Location = new System.Drawing.Point(58, 84);
			rbtProxyFB.Margin = new System.Windows.Forms.Padding(2);
			rbtProxyFB.Name = "rbtProxyFB";
			rbtProxyFB.Size = new System.Drawing.Size(14, 13);
			rbtProxyFB.TabIndex = 120;
			rbtProxyFB.UseVisualStyleBackColor = true;
			rbtProxyFB.CheckedChanged += new System.EventHandler(rbtProxyFB_CheckedChanged);
			linkLabel25.AutoSize = true;
			linkLabel25.Location = new System.Drawing.Point(73, 84);
			linkLabel25.Name = "linkLabel25";
			linkLabel25.Size = new System.Drawing.Size(65, 13);
			linkLabel25.TabIndex = 121;
			linkLabel25.TabStop = true;
			linkLabel25.Text = "Proxyfb.com";
			linkLabel25.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel25_LinkClicked);
			rbtShopLive.AutoSize = true;
			rbtShopLive.Location = new System.Drawing.Point(59, 61);
			rbtShopLive.Margin = new System.Windows.Forms.Padding(2);
			rbtShopLive.Name = "rbtShopLive";
			rbtShopLive.Size = new System.Drawing.Size(14, 13);
			rbtShopLive.TabIndex = 118;
			rbtShopLive.UseVisualStyleBackColor = true;
			rbtShopLive.CheckedChanged += new System.EventHandler(rbtShopLive_CheckedChanged);
			linkLabel20.AutoSize = true;
			linkLabel20.Location = new System.Drawing.Point(74, 61);
			linkLabel20.Name = "linkLabel20";
			linkLabel20.Size = new System.Drawing.Size(52, 13);
			linkLabel20.TabIndex = 119;
			linkLabel20.TabStop = true;
			linkLabel20.Text = "ShopLike";
			linkLabel20.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel20_LinkClicked);
			rbtHMA.AutoSize = true;
			rbtHMA.Location = new System.Drawing.Point(9, 84);
			rbtHMA.Margin = new System.Windows.Forms.Padding(2);
			rbtHMA.Name = "rbtHMA";
			rbtHMA.Size = new System.Drawing.Size(14, 13);
			rbtHMA.TabIndex = 117;
			rbtHMA.UseVisualStyleBackColor = true;
			rbtHMA.CheckedChanged += new System.EventHandler(rbtHMA_CheckedChanged);
			linkLabel13.AutoSize = true;
			linkLabel13.Location = new System.Drawing.Point(25, 84);
			linkLabel13.Name = "linkLabel13";
			linkLabel13.Size = new System.Drawing.Size(31, 13);
			linkLabel13.TabIndex = 116;
			linkLabel13.TabStop = true;
			linkLabel13.Text = "HMA";
			linkLabel13.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel13_LinkClicked_1);
			cbxWifi.AutoSize = true;
			cbxWifi.Location = new System.Drawing.Point(9, 60);
			cbxWifi.Margin = new System.Windows.Forms.Padding(2);
			cbxWifi.Name = "cbxWifi";
			cbxWifi.Size = new System.Drawing.Size(43, 17);
			cbxWifi.TabIndex = 105;
			cbxWifi.Text = "Wifi";
			cbxWifi.UseVisualStyleBackColor = true;
			cbxWifi.CheckedChanged += new System.EventHandler(cbxWifi_CheckedChanged);
			rbtTM.AutoSize = true;
			rbtTM.Location = new System.Drawing.Point(59, 40);
			rbtTM.Margin = new System.Windows.Forms.Padding(2);
			rbtTM.Name = "rbtTM";
			rbtTM.Size = new System.Drawing.Size(14, 13);
			rbtTM.TabIndex = 103;
			rbtTM.UseVisualStyleBackColor = true;
			rbtTM.CheckedChanged += new System.EventHandler(rbtTM_CheckedChanged);
			linkLabel7.AutoSize = true;
			linkLabel7.Location = new System.Drawing.Point(74, 40);
			linkLabel7.Name = "linkLabel7";
			linkLabel7.Size = new System.Drawing.Size(52, 13);
			linkLabel7.TabIndex = 104;
			linkLabel7.TabStop = true;
			linkLabel7.Text = "TM Proxy";
			linkLabel7.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel7_LinkClicked_3);
			rbtXProxy.AutoSize = true;
			rbtXProxy.Location = new System.Drawing.Point(59, 19);
			rbtXProxy.Margin = new System.Windows.Forms.Padding(2);
			rbtXProxy.Name = "rbtXProxy";
			rbtXProxy.Size = new System.Drawing.Size(14, 13);
			rbtXProxy.TabIndex = 101;
			rbtXProxy.UseVisualStyleBackColor = true;
			rbtXProxy.CheckedChanged += new System.EventHandler(rbtXProxy_CheckedChanged_1);
			linkLabel8.AutoSize = true;
			linkLabel8.Location = new System.Drawing.Point(74, 19);
			linkLabel8.Name = "linkLabel8";
			linkLabel8.Size = new System.Drawing.Size(40, 13);
			linkLabel8.TabIndex = 102;
			linkLabel8.TabStop = true;
			linkLabel8.Text = "XProxy";
			linkLabel8.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel8_LinkClicked_2);
			rbtProxy.AutoSize = true;
			rbtProxy.Location = new System.Drawing.Point(9, 40);
			rbtProxy.Margin = new System.Windows.Forms.Padding(2);
			rbtProxy.Name = "rbtProxy";
			rbtProxy.Size = new System.Drawing.Size(14, 13);
			rbtProxy.TabIndex = 4;
			rbtProxy.UseVisualStyleBackColor = true;
			rbtProxy.CheckedChanged += new System.EventHandler(rbtProxy_CheckedChanged);
			rpt3g.AutoSize = true;
			rpt3g.Checked = true;
			rpt3g.Location = new System.Drawing.Point(9, 20);
			rpt3g.Margin = new System.Windows.Forms.Padding(2);
			rpt3g.Name = "rpt3g";
			rpt3g.Size = new System.Drawing.Size(14, 13);
			rpt3g.TabIndex = 2;
			rpt3g.TabStop = true;
			rpt3g.UseVisualStyleBackColor = true;
			rpt3g.CheckedChanged += new System.EventHandler(rpt3g_CheckedChanged);
			lblProxy.AutoSize = true;
			lblProxy.Location = new System.Drawing.Point(24, 39);
			lblProxy.Name = "lblProxy";
			lblProxy.Size = new System.Drawing.Size(33, 13);
			lblProxy.TabIndex = 100;
			lblProxy.TabStop = true;
			lblProxy.Text = "Proxy";
			lblProxy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(lblProxy_LinkClicked);
			lbl4g3g.AutoSize = true;
			lbl4g3g.Location = new System.Drawing.Point(26, 20);
			lbl4g3g.Name = "lbl4g3g";
			lbl4g3g.Size = new System.Drawing.Size(21, 13);
			lbl4g3g.TabIndex = 100;
			lbl4g3g.TabStop = true;
			lbl4g3g.Text = "4G";
			lbl4g3g.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(lbl4g3g_LinkClicked);
			groupProfile.Controls.Add(cbxRemoveAccount);
			groupProfile.Controls.Add(cbxVeri);
			groupProfile.Controls.Add(cbxChangeEmail);
			groupProfile.Controls.Add(cbxRecover);
			groupProfile.Controls.Add(cbxUpdate);
			groupProfile.Controls.Add(linkLabel11);
			groupProfile.Controls.Add(linkLabel24);
			groupProfile.Controls.Add(linkLabel15);
			groupProfile.Controls.Add(cbxBio);
			groupProfile.Controls.Add(linkLabel21);
			groupProfile.Controls.Add(linkLabel6);
			groupProfile.Controls.Add(cbxChangeAvatar);
			groupProfile.Controls.Add(cbxUpVideo);
			groupProfile.Controls.Add(linkLabel14);
			groupProfile.Controls.Add(cbxChangePass);
			groupProfile.Controls.Add(cbxRename);
			groupProfile.Controls.Add(linkLabel12);
			groupProfile.Location = new System.Drawing.Point(358, 9);
			groupProfile.Name = "groupProfile";
			groupProfile.Size = new System.Drawing.Size(129, 212);
			groupProfile.TabIndex = 146;
			groupProfile.TabStop = false;
			groupProfile.Text = "Profile";
			groupProfile.Enter += new System.EventHandler(groupProfile_Enter);
			cbxRemoveAccount.AutoSize = true;
			cbxRemoveAccount.Location = new System.Drawing.Point(10, 191);
			cbxRemoveAccount.Margin = new System.Windows.Forms.Padding(2);
			cbxRemoveAccount.Name = "cbxRemoveAccount";
			cbxRemoveAccount.Size = new System.Drawing.Size(87, 17);
			cbxRemoveAccount.TabIndex = 159;
			cbxRemoveAccount.Tag = "Remove Account";
			cbxRemoveAccount.Text = "Gỡ nick thừa";
			cbxRemoveAccount.UseVisualStyleBackColor = true;
			cbxVeri.AutoSize = true;
			cbxVeri.Location = new System.Drawing.Point(10, 170);
			cbxVeri.Margin = new System.Windows.Forms.Padding(2);
			cbxVeri.Name = "cbxVeri";
			cbxVeri.Size = new System.Drawing.Size(100, 17);
			cbxVeri.TabIndex = 158;
			cbxVeri.Tag = "Verify Email";
			cbxVeri.Text = "Xác nhận Email";
			cbxVeri.UseVisualStyleBackColor = true;
			cbxChangeEmail.AutoSize = true;
			cbxChangeEmail.Location = new System.Drawing.Point(10, 151);
			cbxChangeEmail.Margin = new System.Windows.Forms.Padding(2);
			cbxChangeEmail.Name = "cbxChangeEmail";
			cbxChangeEmail.Size = new System.Drawing.Size(15, 14);
			cbxChangeEmail.TabIndex = 157;
			cbxChangeEmail.UseVisualStyleBackColor = true;
			cbxRecover.AutoSize = true;
			cbxRecover.Location = new System.Drawing.Point(10, 132);
			cbxRecover.Margin = new System.Windows.Forms.Padding(2);
			cbxRecover.Name = "cbxRecover";
			cbxRecover.Size = new System.Drawing.Size(116, 17);
			cbxRecover.TabIndex = 157;
			cbxRecover.Text = "Recover Password";
			cbxRecover.UseVisualStyleBackColor = true;
			cbxUpdate.AutoSize = true;
			cbxUpdate.Location = new System.Drawing.Point(10, 93);
			cbxUpdate.Margin = new System.Windows.Forms.Padding(2);
			cbxUpdate.Name = "cbxUpdate";
			cbxUpdate.Size = new System.Drawing.Size(15, 14);
			cbxUpdate.TabIndex = 156;
			cbxUpdate.UseVisualStyleBackColor = true;
			linkLabel11.AutoSize = true;
			linkLabel11.Location = new System.Drawing.Point(27, 20);
			linkLabel11.Name = "linkLabel11";
			linkLabel11.Size = new System.Drawing.Size(57, 13);
			linkLabel11.TabIndex = 145;
			linkLabel11.TabStop = true;
			linkLabel11.Tag = "Change Avatar";
			linkLabel11.Text = "Đổi Avatar";
			linkLabel11.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel11_LinkClicked_2);
			linkLabel24.AutoSize = true;
			linkLabel24.Location = new System.Drawing.Point(27, 93);
			linkLabel24.Name = "linkLabel24";
			linkLabel24.Size = new System.Drawing.Size(94, 13);
			linkLabel24.TabIndex = 153;
			linkLabel24.TabStop = true;
			linkLabel24.Tag = "Update Information";
			linkLabel24.Text = "Cập nhật thông tin";
			linkLabel24.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel24_LinkClicked_2);
			linkLabel15.AutoSize = true;
			linkLabel15.Location = new System.Drawing.Point(27, 75);
			linkLabel15.Name = "linkLabel15";
			linkLabel15.Size = new System.Drawing.Size(47, 13);
			linkLabel15.TabIndex = 153;
			linkLabel15.TabStop = true;
			linkLabel15.Tag = "Edit BIO";
			linkLabel15.Text = "Sửa BIO";
			linkLabel15.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel15_LinkClicked);
			cbxBio.AutoSize = true;
			cbxBio.Location = new System.Drawing.Point(10, 75);
			cbxBio.Margin = new System.Windows.Forms.Padding(2);
			cbxBio.Name = "cbxBio";
			cbxBio.Size = new System.Drawing.Size(15, 14);
			cbxBio.TabIndex = 152;
			cbxBio.UseVisualStyleBackColor = true;
			linkLabel21.AutoSize = true;
			linkLabel21.Location = new System.Drawing.Point(25, 151);
			linkLabel21.Name = "linkLabel21";
			linkLabel21.Size = new System.Drawing.Size(51, 13);
			linkLabel21.TabIndex = 141;
			linkLabel21.TabStop = true;
			linkLabel21.Tag = "Change Email";
			linkLabel21.Text = "Đổi Email";
			linkLabel21.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel21_LinkClicked);
			linkLabel6.AutoSize = true;
			linkLabel6.Location = new System.Drawing.Point(26, 113);
			linkLabel6.Name = "linkLabel6";
			linkLabel6.Size = new System.Drawing.Size(63, 13);
			linkLabel6.TabIndex = 141;
			linkLabel6.TabStop = true;
			linkLabel6.Tag = "Up Video";
			linkLabel6.Text = "Đăng Video";
			linkLabel6.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel6_LinkClicked_3);
			cbxChangeAvatar.AutoSize = true;
			cbxChangeAvatar.Location = new System.Drawing.Point(10, 21);
			cbxChangeAvatar.Margin = new System.Windows.Forms.Padding(2);
			cbxChangeAvatar.Name = "cbxChangeAvatar";
			cbxChangeAvatar.Size = new System.Drawing.Size(15, 14);
			cbxChangeAvatar.TabIndex = 144;
			cbxChangeAvatar.UseVisualStyleBackColor = true;
			cbxUpVideo.AutoSize = true;
			cbxUpVideo.Location = new System.Drawing.Point(10, 114);
			cbxUpVideo.Margin = new System.Windows.Forms.Padding(2);
			cbxUpVideo.Name = "cbxUpVideo";
			cbxUpVideo.Size = new System.Drawing.Size(15, 14);
			cbxUpVideo.TabIndex = 140;
			cbxUpVideo.UseVisualStyleBackColor = true;
			linkLabel14.AutoSize = true;
			linkLabel14.Location = new System.Drawing.Point(27, 56);
			linkLabel14.Name = "linkLabel14";
			linkLabel14.Size = new System.Drawing.Size(41, 13);
			linkLabel14.TabIndex = 151;
			linkLabel14.TabStop = true;
			linkLabel14.Tag = "Rename";
			linkLabel14.Text = "Đổi tên";
			linkLabel14.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel14_LinkClicked_1);
			cbxChangePass.AutoSize = true;
			cbxChangePass.Location = new System.Drawing.Point(10, 39);
			cbxChangePass.Margin = new System.Windows.Forms.Padding(2);
			cbxChangePass.Name = "cbxChangePass";
			cbxChangePass.Size = new System.Drawing.Size(15, 14);
			cbxChangePass.TabIndex = 146;
			cbxChangePass.UseVisualStyleBackColor = true;
			cbxRename.AutoSize = true;
			cbxRename.Location = new System.Drawing.Point(10, 57);
			cbxRename.Margin = new System.Windows.Forms.Padding(2);
			cbxRename.Name = "cbxRename";
			cbxRename.Size = new System.Drawing.Size(15, 14);
			cbxRename.TabIndex = 150;
			cbxRename.UseVisualStyleBackColor = true;
			linkLabel12.AutoSize = true;
			linkLabel12.Location = new System.Drawing.Point(27, 38);
			linkLabel12.Name = "linkLabel12";
			linkLabel12.Size = new System.Drawing.Size(70, 13);
			linkLabel12.TabIndex = 147;
			linkLabel12.TabStop = true;
			linkLabel12.Tag = "Change Password";
			linkLabel12.Text = "Đổi mật khẩu";
			linkLabel12.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel12_LinkClicked_2);
			groupShop.Controls.Add(lblEarnSu);
			groupShop.Controls.Add(linkLabel23);
			groupShop.Controls.Add(cbxAddProduct);
			groupShop.Controls.Add(cbxRotate);
			groupShop.Controls.Add(cbxEarnSu);
			groupShop.Controls.Add(cbxAccept);
			groupShop.Controls.Add(cbxPublic);
			groupShop.Controls.Add(cbxBalance);
			groupShop.Controls.Add(linkLabel17);
			groupShop.Controls.Add(cbxTDSFollow);
			groupShop.Controls.Add(linkLabel16);
			groupShop.Controls.Add(cbxTDSComment);
			groupShop.Controls.Add(linkLabel9);
			groupShop.Controls.Add(cbxTDSTym);
			groupShop.Location = new System.Drawing.Point(499, 9);
			groupShop.Name = "groupShop";
			groupShop.Size = new System.Drawing.Size(119, 213);
			groupShop.TabIndex = 147;
			groupShop.TabStop = false;
			groupShop.Text = "TDS và Shop";
			lblEarnSu.AutoSize = true;
			lblEarnSu.Location = new System.Drawing.Point(26, 184);
			lblEarnSu.Name = "lblEarnSu";
			lblEarnSu.Size = new System.Drawing.Size(82, 13);
			lblEarnSu.TabIndex = 161;
			lblEarnSu.TabStop = true;
			lblEarnSu.Text = "Check Groupon";
			lblEarnSu.Visible = false;
			lblEarnSu.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel24_LinkClicked);
			linkLabel23.AutoSize = true;
			linkLabel23.Location = new System.Drawing.Point(27, 102);
			linkLabel23.Name = "linkLabel23";
			linkLabel23.Size = new System.Drawing.Size(83, 13);
			linkLabel23.TabIndex = 163;
			linkLabel23.TabStop = true;
			linkLabel23.Tag = "Add Product";
			linkLabel23.Text = "Thêm sản phẩm";
			linkLabel23.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel23_LinkClicked);
			cbxAddProduct.AutoSize = true;
			cbxAddProduct.Location = new System.Drawing.Point(11, 102);
			cbxAddProduct.Margin = new System.Windows.Forms.Padding(2);
			cbxAddProduct.Name = "cbxAddProduct";
			cbxAddProduct.Size = new System.Drawing.Size(15, 14);
			cbxAddProduct.TabIndex = 162;
			cbxAddProduct.UseVisualStyleBackColor = true;
			cbxRotate.AutoSize = true;
			cbxRotate.Location = new System.Drawing.Point(11, 163);
			cbxRotate.Margin = new System.Windows.Forms.Padding(2);
			cbxRotate.Name = "cbxRotate";
			cbxRotate.Size = new System.Drawing.Size(102, 17);
			cbxRotate.TabIndex = 161;
			cbxRotate.Tag = "Rotation Account";
			cbxRotate.Text = "Chạy xoay vòng";
			cbxRotate.UseVisualStyleBackColor = true;
			cbxEarnSu.AutoSize = true;
			cbxEarnSu.Location = new System.Drawing.Point(11, 185);
			cbxEarnSu.Margin = new System.Windows.Forms.Padding(2);
			cbxEarnSu.Name = "cbxEarnSu";
			cbxEarnSu.Size = new System.Drawing.Size(15, 14);
			cbxEarnSu.TabIndex = 159;
			cbxEarnSu.UseVisualStyleBackColor = true;
			cbxEarnSu.Visible = false;
			cbxAccept.AutoSize = true;
			cbxAccept.Location = new System.Drawing.Point(11, 143);
			cbxAccept.Margin = new System.Windows.Forms.Padding(2);
			cbxAccept.Name = "cbxAccept";
			cbxAccept.Size = new System.Drawing.Size(104, 17);
			cbxAccept.TabIndex = 160;
			cbxAccept.Tag = "Accept Shop";
			cbxAccept.Text = "Chấp nhận shop";
			cbxAccept.UseVisualStyleBackColor = true;
			cbxPublic.AutoSize = true;
			cbxPublic.Location = new System.Drawing.Point(11, 18);
			cbxPublic.Margin = new System.Windows.Forms.Padding(2);
			cbxPublic.Name = "cbxPublic";
			cbxPublic.Size = new System.Drawing.Size(76, 17);
			cbxPublic.TabIndex = 155;
			cbxPublic.Text = "Public Info";
			cbxPublic.UseVisualStyleBackColor = true;
			cbxBalance.AutoSize = true;
			cbxBalance.Location = new System.Drawing.Point(11, 123);
			cbxBalance.Margin = new System.Windows.Forms.Padding(2);
			cbxBalance.Name = "cbxBalance";
			cbxBalance.Size = new System.Drawing.Size(75, 17);
			cbxBalance.TabIndex = 158;
			cbxBalance.Tag = "Commission";
			cbxBalance.Text = "Hoa Hồng";
			cbxBalance.UseVisualStyleBackColor = true;
			linkLabel17.AutoSize = true;
			linkLabel17.Location = new System.Drawing.Point(28, 81);
			linkLabel17.Name = "linkLabel17";
			linkLabel17.Size = new System.Drawing.Size(37, 13);
			linkLabel17.TabIndex = 159;
			linkLabel17.TabStop = true;
			linkLabel17.Text = "Follow";
			linkLabel17.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel17_LinkClicked);
			cbxTDSFollow.AutoSize = true;
			cbxTDSFollow.Location = new System.Drawing.Point(11, 81);
			cbxTDSFollow.Margin = new System.Windows.Forms.Padding(2);
			cbxTDSFollow.Name = "cbxTDSFollow";
			cbxTDSFollow.Size = new System.Drawing.Size(15, 14);
			cbxTDSFollow.TabIndex = 158;
			cbxTDSFollow.UseVisualStyleBackColor = true;
			linkLabel16.AutoSize = true;
			linkLabel16.Location = new System.Drawing.Point(28, 61);
			linkLabel16.Name = "linkLabel16";
			linkLabel16.Size = new System.Drawing.Size(51, 13);
			linkLabel16.TabIndex = 157;
			linkLabel16.TabStop = true;
			linkLabel16.Tag = "Comment";
			linkLabel16.Text = "Bình luận";
			linkLabel16.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel16_LinkClicked);
			cbxTDSComment.AutoSize = true;
			cbxTDSComment.Location = new System.Drawing.Point(11, 60);
			cbxTDSComment.Margin = new System.Windows.Forms.Padding(2);
			cbxTDSComment.Name = "cbxTDSComment";
			cbxTDSComment.Size = new System.Drawing.Size(15, 14);
			cbxTDSComment.TabIndex = 156;
			cbxTDSComment.UseVisualStyleBackColor = true;
			linkLabel9.AutoSize = true;
			linkLabel9.Location = new System.Drawing.Point(28, 39);
			linkLabel9.Name = "linkLabel9";
			linkLabel9.Size = new System.Drawing.Size(49, 13);
			linkLabel9.TabIndex = 145;
			linkLabel9.TabStop = true;
			linkLabel9.Tag = "Click Tym";
			linkLabel9.Text = "Thả Tym";
			linkLabel9.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel9_LinkClicked_3);
			cbxTDSTym.AutoSize = true;
			cbxTDSTym.Location = new System.Drawing.Point(11, 39);
			cbxTDSTym.Margin = new System.Windows.Forms.Padding(2);
			cbxTDSTym.Name = "cbxTDSTym";
			cbxTDSTym.Size = new System.Drawing.Size(15, 14);
			cbxTDSTym.TabIndex = 144;
			cbxTDSTym.UseVisualStyleBackColor = true;
			cbxCategory.CheckOnClick = true;
			cbxCategory.FormattingEnabled = true;
			cbxCategory.Location = new System.Drawing.Point(20, 12);
			cbxCategory.Margin = new System.Windows.Forms.Padding(2);
			cbxCategory.Name = "cbxCategory";
			cbxCategory.Size = new System.Drawing.Size(165, 169);
			cbxCategory.TabIndex = 12;
			grDevice.Controls.Add(dataGridViewPhone);
			grDevice.Location = new System.Drawing.Point(782, 8);
			grDevice.Name = "grDevice";
			grDevice.Size = new System.Drawing.Size(504, 212);
			grDevice.TabIndex = 115;
			grDevice.TabStop = false;
			dataGridViewPhone.AllowUserToAddRows = false;
			dataGridViewPhone.AllowUserToResizeColumns = false;
			dataGridViewPhone.AllowUserToResizeRows = false;
			dataGridViewPhone.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			dataGridViewPhone.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridViewPhone.Dock = System.Windows.Forms.DockStyle.Fill;
			dataGridViewPhone.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnF2;
			dataGridViewPhone.Location = new System.Drawing.Point(3, 16);
			dataGridViewPhone.Margin = new System.Windows.Forms.Padding(2);
			dataGridViewPhone.Name = "dataGridViewPhone";
			dataGridViewPhone.ReadOnly = true;
			dataGridViewPhone.RowTemplate.Height = 28;
			dataGridViewPhone.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			dataGridViewPhone.ShowCellErrors = false;
			dataGridViewPhone.ShowEditingIcon = false;
			dataGridViewPhone.ShowRowErrors = false;
			dataGridViewPhone.Size = new System.Drawing.Size(498, 193);
			dataGridViewPhone.TabIndex = 109;
			dataGridViewPhone.MouseClick += new System.Windows.Forms.MouseEventHandler(dataGridViewPhone_MouseClick);
			btnLoadAccount.BackColor = System.Drawing.SystemColors.ControlDark;
			btnLoadAccount.FlatAppearance.BorderSize = 0;
			btnLoadAccount.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnLoadAccount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			btnLoadAccount.ForeColor = System.Drawing.Color.White;
			btnLoadAccount.Image = CCKTiktok.Properties.Resources.find_and_replace_25px;
			btnLoadAccount.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			btnLoadAccount.Location = new System.Drawing.Point(115, 189);
			btnLoadAccount.Margin = new System.Windows.Forms.Padding(2);
			btnLoadAccount.Name = "btnLoadAccount";
			btnLoadAccount.Size = new System.Drawing.Size(70, 32);
			btnLoadAccount.TabIndex = 17;
			btnLoadAccount.Text = "Load";
			btnLoadAccount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			btnLoadAccount.UseVisualStyleBackColor = false;
			btnLoadAccount.Click += new System.EventHandler(btnLoadAccount_Click);
			groupBox1.Controls.Add(cbxDie);
			groupBox1.Controls.Add(cbxError);
			groupBox1.Controls.Add(cbxnew);
			groupBox1.Location = new System.Drawing.Point(633, 9);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(140, 42);
			groupBox1.TabIndex = 113;
			groupBox1.TabStop = false;
			groupBox1.Text = "Status";
			cbxDie.AutoSize = true;
			cbxDie.Location = new System.Drawing.Point(96, 17);
			cbxDie.Name = "cbxDie";
			cbxDie.Size = new System.Drawing.Size(42, 17);
			cbxDie.TabIndex = 2;
			cbxDie.Text = "Die";
			cbxDie.UseVisualStyleBackColor = true;
			cbxError.AutoSize = true;
			cbxError.Location = new System.Drawing.Point(51, 17);
			cbxError.Name = "cbxError";
			cbxError.Size = new System.Drawing.Size(48, 17);
			cbxError.TabIndex = 1;
			cbxError.Text = "Error";
			cbxError.UseVisualStyleBackColor = true;
			cbxnew.AutoSize = true;
			cbxnew.Checked = true;
			cbxnew.CheckState = System.Windows.Forms.CheckState.Checked;
			cbxnew.Location = new System.Drawing.Point(4, 17);
			cbxnew.Name = "cbxnew";
			cbxnew.Size = new System.Drawing.Size(46, 17);
			cbxnew.TabIndex = 0;
			cbxnew.Text = "Live";
			cbxnew.UseVisualStyleBackColor = true;
			dataGridView1.AllowUserToAddRows = false;
			dataGridView1.AllowUserToDeleteRows = false;
			dataGridView1.AllowUserToResizeRows = false;
			dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
			dataGridView1.BackgroundColor = System.Drawing.Color.FromArgb(212, 236, 184);
			dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridView1.ContextMenuStrip = metroContextMenu;
			dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
			dataGridView1.GridColor = System.Drawing.SystemColors.ActiveBorder;
			dataGridView1.ImeMode = System.Windows.Forms.ImeMode.On;
			dataGridView1.Location = new System.Drawing.Point(0, 296);
			dataGridView1.Margin = new System.Windows.Forms.Padding(2);
			dataGridView1.Name = "dataGridView1";
			dataGridView1.ReadOnly = true;
			dataGridView1.RowTemplate.Height = 28;
			dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			dataGridView1.Size = new System.Drawing.Size(1382, 303);
			dataGridView1.TabIndex = 25;
			dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(dataGridView1_CellClick);
			dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(dataGridView1_CellDoubleClick);
			dataGridView1.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(dataGridView1_CellMouseDown);
			dataGridView1.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(dataGridView1_CellMouseUp);
			dataGridView1.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(dataGridView1_ColumnHeaderMouseClick);
			dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(dataGridView1_DataError);
			dataGridView1.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(dataGridView1_RowPrePaint);
			dataGridView1.MouseClick += new System.Windows.Forms.MouseEventHandler(dataGridView1_MouseClick);
			metroContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[9] { changeCategory, accountToolStripMenuItem, copyToolStripMenuItem, checkLiveToolStripMenuItem, unlockHotmailToolStripMenuItem, addNoteoolStripMenuItem, refreshClearADBToolStripMenuItem, xoaLogToolStripMenuItem, filterToolStripMenuItem });
			metroContextMenu.Name = "metroContextMenu";
			metroContextMenu.Size = new System.Drawing.Size(178, 202);
			changeCategory.Image = CCKTiktok.Properties.Resources.change_25px;
			changeCategory.Name = "changeCategory";
			changeCategory.Size = new System.Drawing.Size(177, 22);
			changeCategory.Text = "Đổi chuyên mục";
			changeCategory.Click += new System.EventHandler(changeCategory_Click);
			accountToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[14]
			{
				addToolStripMenuItem, editToolStripMenuItem, deleteToolStripMenuItem, deleteBackupToolStripMenuItem, loginbyhanTayToolStripMenuItem, loctrungToolStripMenuItem, kiemTraBackupToolStripMenuItem, exportBackupAccountToolStripMenuItem, gawnsToolStripMenuItem, xoaProxy,
				xoasToolStripMenuItem, mnuChiaThietbi, chiabyhand, xemChromeToolStripMenuItem
			});
			accountToolStripMenuItem.Image = CCKTiktok.Properties.Resources.add_user_male_25px;
			accountToolStripMenuItem.Name = "accountToolStripMenuItem";
			accountToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
			accountToolStripMenuItem.Text = "Tải khoản";
			addToolStripMenuItem.Image = CCKTiktok.Properties.Resources.add_user_group_man_man_25px;
			addToolStripMenuItem.Name = "addToolStripMenuItem";
			addToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
			addToolStripMenuItem.Text = "Thêm tài khoản";
			addToolStripMenuItem.Click += new System.EventHandler(addToolStripMenuItem_Click);
			editToolStripMenuItem.Image = CCKTiktok.Properties.Resources.id_not_verified_25px;
			editToolStripMenuItem.Name = "editToolStripMenuItem";
			editToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
			editToolStripMenuItem.Text = "Chỉnh sửa tài khoản";
			editToolStripMenuItem.Click += new System.EventHandler(editToolStripMenuItem_Click);
			deleteToolStripMenuItem.Image = CCKTiktok.Properties.Resources.delete_25px;
			deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			deleteToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
			deleteToolStripMenuItem.Text = "Xóa tài khoản";
			deleteToolStripMenuItem.Click += new System.EventHandler(deleteToolStripMenuItem_Click);
			deleteBackupToolStripMenuItem.Image = CCKTiktok.Properties.Resources.delete_sign_25px;
			deleteBackupToolStripMenuItem.Name = "deleteBackupToolStripMenuItem";
			deleteBackupToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
			deleteBackupToolStripMenuItem.Text = "Xóa Backup";
			deleteBackupToolStripMenuItem.Click += new System.EventHandler(deleteBackupToolStripMenuItem_Click);
			loginbyhanTayToolStripMenuItem.Image = CCKTiktok.Properties.Resources.client_management_25px;
			loginbyhanTayToolStripMenuItem.Name = "loginbyhanTayToolStripMenuItem";
			loginbyhanTayToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
			loginbyhanTayToolStripMenuItem.Text = "Đăng nhập bằng tay";
			loginbyhanTayToolStripMenuItem.Click += new System.EventHandler(loginbyhanTayToolStripMenuItem_Click);
			loctrungToolStripMenuItem.Image = CCKTiktok.Properties.Resources.checklist_25px;
			loctrungToolStripMenuItem.Name = "loctrungToolStripMenuItem";
			loctrungToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
			loctrungToolStripMenuItem.Text = "Lọc trùng";
			loctrungToolStripMenuItem.Click += new System.EventHandler(loctrungToolStripMenuItem_Click);
			kiemTraBackupToolStripMenuItem.Image = CCKTiktok.Properties.Resources.data_backup_25px;
			kiemTraBackupToolStripMenuItem.Name = "kiemTraBackupToolStripMenuItem";
			kiemTraBackupToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
			kiemTraBackupToolStripMenuItem.Text = "Kiểm tra Backup";
			kiemTraBackupToolStripMenuItem.Click += new System.EventHandler(kiemTraBackupToolStripMenuItem_Click);
			exportBackupAccountToolStripMenuItem.Image = CCKTiktok.Properties.Resources.export_25px;
			exportBackupAccountToolStripMenuItem.Name = "exportBackupAccountToolStripMenuItem";
			exportBackupAccountToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
			exportBackupAccountToolStripMenuItem.Text = "Export Backup Account";
			exportBackupAccountToolStripMenuItem.Click += new System.EventHandler(exportBackupAccountToolStripMenuItem_Click);
			gawnsToolStripMenuItem.Image = CCKTiktok.Properties.Resources.joyent_25px;
			gawnsToolStripMenuItem.Name = "gawnsToolStripMenuItem";
			gawnsToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
			gawnsToolStripMenuItem.Text = "Gắn Proxy cứng cho từng nick";
			gawnsToolStripMenuItem.Click += new System.EventHandler(gawnsToolStripMenuItem_Click);
			xoaProxy.Image = CCKTiktok.Properties.Resources.cancel_25px;
			xoaProxy.Name = "xoaProxy";
			xoaProxy.Size = new System.Drawing.Size(234, 22);
			xoaProxy.Text = "Xóa Proxy của nick";
			xoaProxy.Click += new System.EventHandler(xoaProxy_Click);
			xoasToolStripMenuItem.Image = CCKTiktok.Properties.Resources.delete_sign_25px;
			xoasToolStripMenuItem.Name = "xoasToolStripMenuItem";
			xoasToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
			xoasToolStripMenuItem.Text = "Xóa thiết bị";
			xoasToolStripMenuItem.Click += new System.EventHandler(xoasToolStripMenuItem_Click);
			mnuChiaThietbi.Image = CCKTiktok.Properties.Resources.reset_hma;
			mnuChiaThietbi.Name = "mnuChiaThietbi";
			mnuChiaThietbi.Size = new System.Drawing.Size(234, 22);
			mnuChiaThietbi.Text = "Chia thiết bị tự động";
			mnuChiaThietbi.Click += new System.EventHandler(mnuChiaThietbi_Click);
			chiabyhand.Image = CCKTiktok.Properties.Resources.gender_neutral_user_25px;
			chiabyhand.Name = "chiabyhand";
			chiabyhand.Size = new System.Drawing.Size(234, 22);
			chiabyhand.Text = "Chia thiết bị thủ công";
			chiabyhand.Click += new System.EventHandler(chiabyhand_Click);
			xemChromeToolStripMenuItem.Image = CCKTiktok.Properties.Resources.e_learning_25px;
			xemChromeToolStripMenuItem.Name = "xemChromeToolStripMenuItem";
			xemChromeToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
			xemChromeToolStripMenuItem.Text = "Xem trên Chrome";
			xemChromeToolStripMenuItem.Click += new System.EventHandler(xemChromeToolStripMenuItem_Click);
			copyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[9] { copyUIDToolStripMenuItem, copyPasswordToolStripMenuItem, copyEmailToolStripMenuItem, copyPasswordEmailToolStripMenuItem, copyUIDPassToolStripMenuItem, copyUIDPassEmailPassEmailDeviceInfoToolStripMenuItem1, copyUIDPassEmailPassEmailToolStripMenuItem, copyEmailPassEmailToolStripMenuItem1, cookiesToolStripMenuItem });
			copyToolStripMenuItem.Image = CCKTiktok.Properties.Resources.copy_25px;
			copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			copyToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
			copyToolStripMenuItem.Text = "Copy";
			copyUIDToolStripMenuItem.Image = CCKTiktok.Properties.Resources.add_user_male_25px;
			copyUIDToolStripMenuItem.Name = "copyUIDToolStripMenuItem";
			copyUIDToolStripMenuItem.Size = new System.Drawing.Size(334, 22);
			copyUIDToolStripMenuItem.Text = "Copy UID";
			copyUIDToolStripMenuItem.Click += new System.EventHandler(copyUIDToolStripMenuItem_Click);
			copyPasswordToolStripMenuItem.Image = CCKTiktok.Properties.Resources.lock_25px;
			copyPasswordToolStripMenuItem.Name = "copyPasswordToolStripMenuItem";
			copyPasswordToolStripMenuItem.Size = new System.Drawing.Size(334, 22);
			copyPasswordToolStripMenuItem.Text = "Copy Password";
			copyPasswordToolStripMenuItem.Click += new System.EventHandler(copyPasswordToolStripMenuItem_Click);
			copyEmailToolStripMenuItem.Image = CCKTiktok.Properties.Resources.gmail_25px;
			copyEmailToolStripMenuItem.Name = "copyEmailToolStripMenuItem";
			copyEmailToolStripMenuItem.Size = new System.Drawing.Size(334, 22);
			copyEmailToolStripMenuItem.Text = "Copy Email";
			copyEmailToolStripMenuItem.Click += new System.EventHandler(copyEmailToolStripMenuItem_Click);
			copyPasswordEmailToolStripMenuItem.Image = CCKTiktok.Properties.Resources.lock_25px;
			copyPasswordEmailToolStripMenuItem.Name = "copyPasswordEmailToolStripMenuItem";
			copyPasswordEmailToolStripMenuItem.Size = new System.Drawing.Size(334, 22);
			copyPasswordEmailToolStripMenuItem.Text = "Copy Password Email";
			copyPasswordEmailToolStripMenuItem.Click += new System.EventHandler(copyPasswordEmailToolStripMenuItem_Click);
			copyUIDPassToolStripMenuItem.Image = CCKTiktok.Properties.Resources.checked_checkbox_25px;
			copyUIDPassToolStripMenuItem.Name = "copyUIDPassToolStripMenuItem";
			copyUIDPassToolStripMenuItem.Size = new System.Drawing.Size(334, 22);
			copyUIDPassToolStripMenuItem.Text = "Copy UID - Pass";
			copyUIDPassToolStripMenuItem.Click += new System.EventHandler(copyUIDPassToolStripMenuItem_Click_1);
			copyUIDPassEmailPassEmailDeviceInfoToolStripMenuItem1.Image = CCKTiktok.Properties.Resources.password_25px;
			copyUIDPassEmailPassEmailDeviceInfoToolStripMenuItem1.Name = "copyUIDPassEmailPassEmailDeviceInfoToolStripMenuItem1";
			copyUIDPassEmailPassEmailDeviceInfoToolStripMenuItem1.Size = new System.Drawing.Size(334, 22);
			copyUIDPassEmailPassEmailDeviceInfoToolStripMenuItem1.Text = "Copy UID - Pass - Email - Pass Email - Device Info";
			copyUIDPassEmailPassEmailDeviceInfoToolStripMenuItem1.Click += new System.EventHandler(copyUIDPassEmailPassEmailDeviceInfoToolStripMenuItem1_Click);
			copyUIDPassEmailPassEmailToolStripMenuItem.Image = CCKTiktok.Properties.Resources.copy_25px;
			copyUIDPassEmailPassEmailToolStripMenuItem.Name = "copyUIDPassEmailPassEmailToolStripMenuItem";
			copyUIDPassEmailPassEmailToolStripMenuItem.Size = new System.Drawing.Size(334, 22);
			copyUIDPassEmailPassEmailToolStripMenuItem.Text = "Copy UID - Pass - Email - Pass Email - Cookies";
			copyUIDPassEmailPassEmailToolStripMenuItem.Click += new System.EventHandler(copyUIDPassEmailPassEmailToolStripMenuItem_Click);
			copyEmailPassEmailToolStripMenuItem1.Image = CCKTiktok.Properties.Resources.copy_25px;
			copyEmailPassEmailToolStripMenuItem1.Name = "copyEmailPassEmailToolStripMenuItem1";
			copyEmailPassEmailToolStripMenuItem1.Size = new System.Drawing.Size(334, 22);
			copyEmailPassEmailToolStripMenuItem1.Text = "Copy Email - Pass Email";
			copyEmailPassEmailToolStripMenuItem1.Click += new System.EventHandler(copyEmailPassEmailToolStripMenuItem1_Click);
			cookiesToolStripMenuItem.Image = CCKTiktok.Properties.Resources.cookie_25px;
			cookiesToolStripMenuItem.Name = "cookiesToolStripMenuItem";
			cookiesToolStripMenuItem.Size = new System.Drawing.Size(334, 22);
			cookiesToolStripMenuItem.Text = "Cookies";
			cookiesToolStripMenuItem.Click += new System.EventHandler(cookiesToolStripMenuItem_Click);
			checkLiveToolStripMenuItem.Image = CCKTiktok.Properties.Resources.double_tick_25px;
			checkLiveToolStripMenuItem.Name = "checkLiveToolStripMenuItem";
			checkLiveToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
			checkLiveToolStripMenuItem.Text = "Check Live";
			checkLiveToolStripMenuItem.Click += new System.EventHandler(checkLiveToolStripMenuItem_Click);
			unlockHotmailToolStripMenuItem.Image = CCKTiktok.Properties.Resources.lock_25px;
			unlockHotmailToolStripMenuItem.Name = "unlockHotmailToolStripMenuItem";
			unlockHotmailToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
			unlockHotmailToolStripMenuItem.Text = "Unlock Hotmail";
			unlockHotmailToolStripMenuItem.Click += new System.EventHandler(unlockHotmailToolStripMenuItem_Click);
			addNoteoolStripMenuItem.Image = CCKTiktok.Properties.Resources.literature_25px;
			addNoteoolStripMenuItem.Name = "addNoteoolStripMenuItem";
			addNoteoolStripMenuItem.Size = new System.Drawing.Size(177, 22);
			addNoteoolStripMenuItem.Text = "Thêm ghi chú";
			addNoteoolStripMenuItem.Click += new System.EventHandler(addNoteoolStripMenuItem_Click);
			refreshClearADBToolStripMenuItem.Image = CCKTiktok.Properties.Resources.reset_hma;
			refreshClearADBToolStripMenuItem.Name = "refreshClearADBToolStripMenuItem";
			refreshClearADBToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
			refreshClearADBToolStripMenuItem.Text = "Refresh - Clear ADB";
			refreshClearADBToolStripMenuItem.Click += new System.EventHandler(refreshClearADBToolStripMenuItem_Click);
			xoaLogToolStripMenuItem.Image = CCKTiktok.Properties.Resources.cancel_25px;
			xoaLogToolStripMenuItem.Name = "xoaLogToolStripMenuItem";
			xoaLogToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
			xoaLogToolStripMenuItem.Text = "Xóa Log";
			xoaLogToolStripMenuItem.Click += new System.EventHandler(xoaLogToolStripMenuItem_Click);
			filterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[2] { notesToolStripMenuItem, logToolStripMenuItem });
			filterToolStripMenuItem.Image = CCKTiktok.Properties.Resources.find_and_replace_25px;
			filterToolStripMenuItem.Name = "filterToolStripMenuItem";
			filterToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
			filterToolStripMenuItem.Text = "Filter / Lọc";
			notesToolStripMenuItem.Image = CCKTiktok.Properties.Resources.google_groups_25px;
			notesToolStripMenuItem.Name = "notesToolStripMenuItem";
			notesToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			notesToolStripMenuItem.Text = "Notes / Ghi chú";
			notesToolStripMenuItem.Click += new System.EventHandler(notesToolStripMenuItem_Click);
			logToolStripMenuItem.Image = CCKTiktok.Properties.Resources.literature_25px;
			logToolStripMenuItem.Name = "logToolStripMenuItem";
			logToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			logToolStripMenuItem.Text = "Log";
			logToolStripMenuItem.Click += new System.EventHandler(logToolStripMenuItem_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(1382, 621);
			base.Controls.Add(dataGridView1);
			base.Controls.Add(statusStrip1);
			base.Controls.Add(grbAction);
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.Margin = new System.Windows.Forms.Padding(2);
			base.Name = "frmMain";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "CCK Tiktok - 0904.868.545";
			base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(frmMain_FormClosing);
			base.Load += new System.EventHandler(frmMain_Load);
			statusStrip1.ResumeLayout(false);
			statusStrip1.PerformLayout();
			grbAction.ResumeLayout(false);
			groupBox6.ResumeLayout(false);
			gbAction.ResumeLayout(false);
			gbAction.PerformLayout();
			groupBox3.ResumeLayout(false);
			groupBox3.PerformLayout();
			groupAction.ResumeLayout(false);
			groupAction.PerformLayout();
			groupBox5.ResumeLayout(false);
			groupBox5.PerformLayout();
			groupProfile.ResumeLayout(false);
			groupProfile.PerformLayout();
			groupShop.ResumeLayout(false);
			groupShop.PerformLayout();
			grDevice.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)dataGridViewPhone).EndInit();
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
			metroContextMenu.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
