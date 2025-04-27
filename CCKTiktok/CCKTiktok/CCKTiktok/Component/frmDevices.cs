using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Xml;
using CCKTiktok.BO;
using CCKTiktok.Bussiness;
using CCKTiktok.DAL;
using CCKTiktok.Entity;
using CCKTiktok.Helper;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Remote;

namespace CCKTiktok.Component
{
	public class frmDevices : Form
	{
		private int numberOfDevice;

		private static object myLock = new object();

		private static Dictionary<string, string> dicDevices = new Dictionary<string, string>();

		private string selectedDevice = "";

		private string selectedUid = "";

		private bool isRunning = true;

		private IContainer components = null;

		private Button btnCheck;

		private Button btnLoadDevices;

		private Button btnBri;

		private Button btnturnoff;

		private Button btnTurnon;

		private CheckBox cbxVolumn;

		private NumericUpDown nudTimeout;

		private Label label8;

		private NumericUpDown nudBright;

		private Button btnSetup;

		private Button btnSetupApk;

		private Button btnAppium;

		private DataGridView dataGridViewPhone;

		private Button btnClean;

		private Button btnShowPhone;

		private NumericUpDown numLine;

		private Label label4;

		private Button btnReboot;

		private NumericUpDown nudPhoneDelay;

		private Label label5;

		private Button button3;

		private Button btnedXposed;

		private Button btnLang;

		private Button button4;

		private Button btnAir;

		private Button btnAirOff;

		private CheckBox cbxWifi;

		private LinkLabel linkLabel1;

		private Button button5;

		private Button btnshutdown;

		private Button btnportrait;

		private CheckBox cbxWifiMode;

		private LinkLabel linkLabel3;

		private Button button2;

		private Button button6;

		private Button button7;

		private TextBox txtId;

		private TextBox txtName;

		private Label label1;

		private Label label2;

		private Button btnRename;

		private GroupBox groupBox1;

		private Button btnCCK;

		private Button button8;

		private Button button9;

		private Button button10;

		private Button button11;

		private Button button12;

		private Button button13;

		private NumericUpDown numericUpDown1;

		private Label label3;

		private Button button14;

		private GroupBox groupBox3;

		private RadioButton rbtRoot;

		private RadioButton rbtNoroot;

		private CheckBox cbxLogVideo;

		private Button btnTimeZone;

		private Button btnshutdowna;

		private CheckBox checkBox1;

		private CheckBox cbxSchedule;

		private RadioButton rbtTWrp;

		private Button button1;

		public frmDevices(int numberOfDevice)
		{
			InitializeComponent();
			this.numberOfDevice = numberOfDevice;
		}

		private void ChangeGridViewDevice(string searchValue, string msg, Color color)
		{
			ADBHelperCCK.ShowTextMessageOnPhone(searchValue, msg);
			lock (myLock)
			{
				int num = -1;
				try
				{
					DataGridViewRow dataGridViewRow = (from DataGridViewRow r in dataGridViewPhone.Rows
						where r.Cells["Phone"] != null && r.Cells["Phone"].Value.ToString().Equals(searchValue)
						select r).FirstOrDefault();
					num = dataGridViewRow.Index;
					if (num != -1 && num < dataGridViewPhone.Rows.Count)
					{
						dataGridViewPhone.Rows[num].Cells["Message"].Value = msg;
						dataGridViewPhone.Rows[num].DefaultCellStyle.ForeColor = color;
					}
				}
				catch (Exception)
				{
				}
			}
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
						if (!ADBHelperCCK.IsInstallApp(phone, "com.facebook.katana"))
						{
							list.Add("facebook");
						}
						if (!ADBHelperCCK.IsInstallApp(phone, "com.cck.support"))
						{
							list.Add("support");
						}
						if (list.Count != 0)
						{
							ChangeGridViewDevice(phone, "Chưa cài: " + string.Join(",", list), Color.Red);
						}
						else
						{
							ChangeGridViewDevice(phone, "Ready", Color.Green);
						}
					}
					else
					{
						ChangeGridViewDevice(phone, "Disconnected", Color.Red);
					}
				}).Start();
			}
		}

		private void btnLoadDevices_Click(object sender, EventArgs e)
		{
			try
			{
				LoadDevices();
				new Task(delegate
				{
					List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice(fullInfo: true);
					DataTable dataTable = new DataTable
					{
						Columns = { "Stt", "Phone", "Name", "Status", "Message", "Port", "SystemPort" }
					};
					List<string> list = new List<string>();
					int num = 0;
					foreach (string item in listSerialDevice)
					{
						string[] array = item.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
						DataRow dataRow = dataTable.NewRow();
						dataRow["Stt"] = ++num;
						dataRow["Phone"] = array[0];
						dataRow["Name"] = (dicDevices.ContainsKey(array[0]) ? dicDevices[array[0]] : array[0]);
						dataRow["Status"] = ((array.Length == 1) ? "Live" : array[1]);
						dataRow["Port"] = 4723 + num;
						dataRow["SystemPort"] = 8200 + num;
						if (num <= numberOfDevice)
						{
							dataTable.Rows.Add(dataRow);
							list.Add(item);
						}
					}
					dataTable.AcceptChanges();
					DataView defaultView = dataTable.DefaultView;
					defaultView.Sort = "Name asc";
					dataTable = defaultView.ToTable();
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						dataTable.Rows[i]["Stt"] = i + 1;
					}
					dataTable.AcceptChanges();
					BindObjectList(dataGridViewPhone, dataTable);
					dataGridViewPhone.RowHeadersVisible = false;
					dataGridViewPhone.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
				}).Start();
			}
			catch (Exception)
			{
			}
		}

		protected static void BindObjectList(DataGridView dataGridView, DataTable table)
		{
			try
			{
				if (table != null)
				{
					dataGridView.Invoke((Action)delegate
					{
						dataGridView.DataSource = table;
						dataGridView.Columns[dataGridView.ColumnCount - 1].Visible = false;
						dataGridView.Columns[dataGridView.ColumnCount - 2].Visible = false;
					});
				}
			}
			catch
			{
			}
		}

		private void frmDevices_Load(object sender, EventArgs e)
		{
			if (File.Exists(CaChuaConstant.STARTPORT))
			{
				numericUpDown1.Value = Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.STARTPORT));
			}
			if (numericUpDown1.Value < 4723m)
			{
				numericUpDown1.Value = 4723m;
			}
			cbxWifiMode.Checked = Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.WIFI_MODE));
			if (File.Exists(CaChuaConstant.ADD_FRIEND_ALL))
			{
			}
			if (File.Exists(CaChuaConstant.PHONE_DELAY))
			{
				nudPhoneDelay.Value = Utils.ToDecimal(Utils.ReadTextFile(CaChuaConstant.PHONE_DELAY));
			}
			if (File.Exists(CaChuaConstant.SCHEDULE))
			{
				cbxSchedule.Checked = Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.SCHEDULE));
			}
			if (File.Exists(CaChuaConstant.PHONE_WIDTH))
			{
				numLine.Value = Utils.ToDecimal(Utils.ReadTextFile(CaChuaConstant.PHONE_WIDTH));
				if (numLine.Value == 0m)
				{
					numLine.Value = 240m;
				}
			}
			if (File.Exists(CaChuaConstant.PHONE_MODE_DATA))
			{
				PhoneMode phoneMode = new JavaScriptSerializer().Deserialize<PhoneMode>(Utils.ReadTextFile(CaChuaConstant.PHONE_MODE_DATA));
				rbtRoot.Checked = phoneMode == PhoneMode.Root;
				rbtNoroot.Checked = phoneMode == PhoneMode.NonRoot;
				rbtTWrp.Checked = phoneMode == PhoneMode.TWRP;
			}
			checkBox1.Checked = Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.CLEARPROXY));
			btnLoadDevices_Click(null, null);
		}

		private void btnSetupApk_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Apk Files|*.apk";
			string filename = "";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				filename = openFileDialog.FileName;
			}
			if (string.IsNullOrWhiteSpace(filename))
			{
				return;
			}
			new Task(delegate
			{
				ThreadPool.SetMinThreads(100, 4);
				ServicePointManager.DefaultConnectionLimit = 500;
				foreach (DataGridViewRow item in (IEnumerable)dataGridViewPhone.Rows)
				{
					object phone = item.Cells["Phone"].Value;
					if (phone != null)
					{
						new Task(delegate
						{
							ChangeGridViewDevice(phone.ToString(), "Start", Color.Green);
							ADBHelperCCK.InstallApp(phone.ToString(), filename);
							ChangeGridViewDevice(phone.ToString(), "Finished", Color.Green);
						}).Start();
					}
				}
			}).Start();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Apk Files|*.apk";
			string filename = "";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				filename = openFileDialog.FileName;
			}
			if (string.IsNullOrWhiteSpace(filename))
			{
				return;
			}
			new Task(delegate
			{
				List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
				foreach (string item in listSerialDevice)
				{
					SetUpCCKSupport(filename, item);
				}
			}).Start();
		}

		private void SetUpCCKSupport(string filename, string p_DeviceId)
		{
			new Task(delegate
			{
				ADBHelperCCK.StopApp(p_DeviceId, "com.cck.support");
				ADBHelperCCK.UnInstallApp(p_DeviceId, "com.cck.support");
				ChangeGridViewDevice(p_DeviceId, "Removed", Color.Green);
				ADBHelperCCK.InstallApp(p_DeviceId, filename);
				ChangeGridViewDevice(p_DeviceId, "Start Server", Color.Green);
				try
				{
					Thread.Sleep(2000);
					DeviceEntity deivcePort = GetDeivcePort(p_DeviceId);
					AndroidDriver<AndroidElement> androidDriver = ADBHelperCCK.StartPhone(deivcePort);
					ChangeGridViewDevice(p_DeviceId, "Server Started", Color.Green);
					string pageSource = androidDriver.PageSource;
					while (pageSource.Contains("text=\"Continue\"") || pageSource.Contains("text=\"OK\""))
					{
						try
						{
							if (pageSource.Contains("text=\"Continue\""))
							{
								ReadOnlyCollection<AndroidElement> readOnlyCollection = androidDriver.FindElements(By.XPath("//*[@text=\"Continue\"]"));
								if (readOnlyCollection != null && readOnlyCollection.Count > 0)
								{
									readOnlyCollection[0].Click();
									Thread.Sleep(500);
								}
							}
							pageSource = androidDriver.PageSource;
							if (pageSource.Contains("\"OK\""))
							{
								ReadOnlyCollection<AndroidElement> readOnlyCollection2 = androidDriver.FindElements(By.XPath("//*[@text=\"OK\"]"));
								if (readOnlyCollection2 != null && readOnlyCollection2.Count > 0 && readOnlyCollection2[0].Enabled && readOnlyCollection2[0].Displayed)
								{
									readOnlyCollection2[0].Click();
									Thread.Sleep(1000);
								}
							}
							pageSource = androidDriver.PageSource;
						}
						catch
						{
							androidDriver = ADBHelperCCK.StartPhone(deivcePort);
						}
					}
					try
					{
						ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell rm -f /sdcard/cckbg.png");
						string text = Application.StartupPath + "\\Devices\\cckbg.png";
						if (!File.Exists(text))
						{
							new WebClient().DownloadFile("https://cck.vn/Download/Update/cckbg.png", text);
						}
						ADBHelperCCK.PushInfoFile(p_DeviceId, $"\"{text}\" \"/sdcard/cckbg.png\"");
						ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell am startservice com.cck.support/.CckServiceBackground");
						Thread.Sleep(3000);
						ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell rm -r \"/sdcard/cckbg.png\"");
					}
					catch
					{
					}
					ChangeGridViewDevice(p_DeviceId, "Finished", Color.Green);
				}
				catch (Exception ex)
				{
					_ = ex.Message + filename + "_" + p_DeviceId;
				}
			}).Start();
		}

		private DeviceEntity GetDeivcePort(string searchValue)
		{
			int num = -1;
			try
			{
				DataGridViewRow dataGridViewRow = (from DataGridViewRow r in dataGridViewPhone.Rows
					where r.Cells["Phone"].Value.ToString().Equals(searchValue)
					select r).FirstOrDefault();
				num = dataGridViewRow.Index;
				return new DeviceEntity
				{
					DeviceId = dataGridViewPhone.Rows[num].Cells["Phone"].Value.ToString(),
					Name = dataGridViewPhone.Rows[num].Cells["Phone"].Value.ToString(),
					Port = Convert.ToInt32(dataGridViewPhone.Rows[num].Cells["Port"].Value),
					SystemPort = Convert.ToInt32(dataGridViewPhone.Rows[num].Cells["SystemPort"].Value)
				};
			}
			catch (Exception ex)
			{
				Utils.CCKLog("TaskList", ex.Message, "GetDeivcePort");
			}
			return new DeviceEntity();
		}

		private void btnturnoff_Click(object sender, EventArgs e)
		{
			ThreadPool.SetMinThreads(100, 4);
			ServicePointManager.DefaultConnectionLimit = 500;
			new Task(delegate
			{
				foreach (DataGridViewRow item in (IEnumerable)dataGridViewPhone.Rows)
				{
					object phone = item.Cells["Phone"].Value;
					if (phone != null)
					{
						new Task(delegate
						{
							ADBHelperCCK.TurnScreenOffDevice(phone.ToString());
						}).Start();
					}
				}
			}).Start();
		}

		private void btnTurnon_Click(object sender, EventArgs e)
		{
			ThreadPool.SetMinThreads(100, 4);
			ServicePointManager.DefaultConnectionLimit = 500;
			new Task(delegate
			{
				foreach (DataGridViewRow item in (IEnumerable)dataGridViewPhone.Rows)
				{
					object phone = item.Cells["Phone"].Value;
					if (phone != null)
					{
						new Task(delegate
						{
							ADBHelperCCK.TurnScreenOnDevice(phone.ToString());
						}).Start();
					}
				}
			}).Start();
		}

		private void SetupPhone(List<string> lst)
		{
			int sPort = 4723;
			ThreadPool.SetMinThreads(100, 4);
			ServicePointManager.DefaultConnectionLimit = 500;
			string file_apk = "TikTok_32.9.apk";
			if (!File.Exists("Devices\\" + file_apk))
			{
				frmDownload frmDownload2 = new frmDownload();
				frmDownload2.Download("https://cck.vn/Download/Utils/" + file_apk + ".rar", Application.StartupPath + "\\Devices\\" + file_apk);
				frmDownload2.ShowDialog();
				if (frmDownload2.DownloadCompleted)
				{
					Thread.Sleep(1000);
				}
			}
			file_apk = "TikTok_cck_Lite.apk";
			if (!File.Exists("Devices\\" + file_apk))
			{
				frmDownload frmDownload3 = new frmDownload();
				frmDownload3.Download("https://cck.vn/Download/Utils/" + file_apk + ".rar", Application.StartupPath + "\\Devices\\" + file_apk);
				frmDownload3.ShowDialog();
				if (frmDownload3.DownloadCompleted)
				{
					Thread.Sleep(1000);
				}
			}
			file_apk = "CollegeProxy_10.1.1_Mod.apk";
			if (!File.Exists("Devices\\" + file_apk))
			{
				frmDownload frmDownload4 = new frmDownload();
				frmDownload4.Download("https://cck.vn/Download/Utils/" + file_apk + ".rar", Application.StartupPath + "\\Devices\\" + file_apk);
				frmDownload4.ShowDialog();
				if (frmDownload4.DownloadCompleted)
				{
					Thread.Sleep(1000);
				}
			}
			file_apk = "ADBCLanguage.apk";
			if (!File.Exists("Devices\\" + file_apk))
			{
				frmDownload frmDownload5 = new frmDownload();
				frmDownload5.Download("https://cck.vn/Download/Utils/" + file_apk + ".rar", Application.StartupPath + "\\Devices\\" + file_apk);
				frmDownload5.ShowDialog();
				if (frmDownload5.DownloadCompleted)
				{
					Thread.Sleep(1000);
				}
			}
			foreach (string p_DeviceId in lst)
			{
				try
				{
					new Task(delegate
					{
						ADBHelperCCK.SetPortrait(p_DeviceId);
						ADBHelperCCK.UnInstallApp(p_DeviceId, "com.cck.support");
						ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell settings put system accelerometer_rotation 0");
						DeviceEntity obj = new DeviceEntity
						{
							Name = p_DeviceId,
							DeviceId = p_DeviceId,
							SystemPort = 12923 - sPort
						};
						int num = sPort;
						sPort = num + 1;
						obj.Port = num;
						obj.Version = ADBHelperCCK.GetAndroidVersion(p_DeviceId);
						DeviceEntity device = obj;
						ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell rm -f /sdcard/*.txt");
						ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell rm -f /sdcard/*.vcf");
						ADBHelperCCK.TurnScreenOnDevice(p_DeviceId);
						if (cbxVolumn.Checked)
						{
							ChangeGridViewDevice(p_DeviceId, "Tắt tiếng", Color.Green);
							ADBHelperCCK.SetVolumnMute(p_DeviceId);
						}
						List<string> list = new List<string>();
						ChangeGridViewDevice(p_DeviceId, "Start", Color.Green);
						ADBHelperCCK.InstallApp(p_DeviceId, Application.StartupPath + "\\Devices\\ADBCLanguage.apk");
						ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell pm grant net.sanapeli.adbchangelanguage android.permission.CHANGE_CONFIGURATION");
						ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell am start -n net.sanapeli.adbchangelanguage/.AdbChangeLanguage -e language en");
						string tiktokVersion = ADBHelperCCK.GetTiktokVersion(p_DeviceId);
						if (!tiktokVersion.Contains("32.9"))
						{
							ADBHelperCCK.UnInstallApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME);
							file_apk = "TikTok_32.9.apk";
							ChangeGridViewDevice(p_DeviceId, "Đang Setup Tiktok...", Color.Green);
							if (File.Exists("Devices\\" + file_apk))
							{
								ADBHelperCCK.InstallApp(p_DeviceId, Application.StartupPath + "\\Devices\\" + file_apk);
								ChangeGridViewDevice(p_DeviceId, "Setup Tiktok xong", Color.Green);
							}
						}
						if (!ADBHelperCCK.IsInstallApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME_LITE))
						{
							file_apk = "TikTok_cck_Lite.apk";
							ChangeGridViewDevice(p_DeviceId, "Đang Setup Tiktok Lite...", Color.Green);
							if (File.Exists("Devices\\" + file_apk))
							{
								ADBHelperCCK.InstallApp(p_DeviceId, Application.StartupPath + "\\Devices\\" + file_apk);
								ChangeGridViewDevice(p_DeviceId, "Setup Tiktok Lite xong", Color.Green);
							}
						}
						if (ADBHelperCCK.IsInstallApp(p_DeviceId, CaChuaConstant.PACKAGE_NAME))
						{
							list.Add("Tiktok");
						}
						if (Utils.Convert2Int(ADBHelperCCK.GetAndroidVersion(p_DeviceId)) > 8)
						{
							if (!File.Exists(Application.StartupPath + "\\Devices\\adb_root.zip"))
							{
								try
								{
									new WebClient().DownloadFile("https://cck.vn/Download/Utils/adb_root.zip", Application.StartupPath + "\\Devices\\adb_root.zip");
									Thread.Sleep(1000);
								}
								catch (Exception ex2)
								{
									Utils.CCKLog("TaskList", ex2.Message, "SetupPhone");
								}
							}
							if (File.Exists(Application.StartupPath + "\\Devices\\adb_root.zip"))
							{
								ADBHelperCCK.PushInfoFile(p_DeviceId, "\"" + Application.StartupPath + "\\Devices\\adb_root.zip\" /sdcard/");
							}
						}
						ADBHelperCCK.StartPhone(device);
						string text = p_DeviceId;
						ADBHelperCCK.ExecuteCMD(text, " uninstall com.cck.support");
						if (!File.Exists(Application.StartupPath + "\\Devices\\cck_device_changer3.0.3.apk"))
						{
							ChangeGridViewDevice(p_DeviceId, "Cài Change App", Color.Green);
							if (!Directory.Exists("Devices"))
							{
								Directory.CreateDirectory("Devices");
							}
							try
							{
								ADBHelperCCK.ExecuteCMD(text, " uninstall com.cck.support");
								if (!File.Exists(Application.StartupPath + "\\Devices\\cck_device_changer3.0.3.apk"))
								{
									new WebClient().DownloadFile("https://cck.vn/Download/Utils/cck_device_changer3.0.3.apk.rar", Application.StartupPath + "\\Devices\\cck_device_changer3.0.3.apk");
								}
							}
							catch (Exception ex3)
							{
								Utils.CCKLog("TaskList", ex3.Message, "SetupPhone");
							}
							ADBHelperCCK.ExecuteCMD(text, "shell pm grant com.cck.support android.permission.READ_EXTERNAL_STORAGE");
							ADBHelperCCK.ExecuteCMD(text, "shell pm grant com.cck.support android.permission.WRITE_EXTERNAL_STORAGE");
							ADBHelperCCK.ExecuteCMD(text, "shell pm grant com.cck.support android.permission.READ_CONTACTS");
							ADBHelperCCK.ExecuteCMD(text, "shell pm grant com.cck.support android.permission.WRITE_CONTACTS");
							Thread.Sleep(3000);
						}
						ADBHelperCCK.GetAndroidVersionNumber(p_DeviceId);
						if (!ADBHelperCCK.IsInstallApp(text, "com.cck.support") && File.Exists(Application.StartupPath + "\\Devices\\cck_device_changer3.0.3.apk"))
						{
							ADBHelperCCK.ExecuteCMD(text, " install \"" + Application.StartupPath + "\\Devices\\cck_device_changer3.0.3.apk\"");
							ADBHelperCCK.OpenApp(p_DeviceId, "com.cck.support");
							Thread.Sleep(1000);
							ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell pm grant com.cck.support android.permission.READ_EXTERNAL_STORAGE");
							ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell pm grant com.cck.support android.permission.WRITE_EXTERNAL_STORAGE");
							ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell pm grant com.cck.support android.permission.READ_CONTACTS");
							ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell pm grant com.cck.support android.permission.WRITE_CONTACTS");
						}
						try
						{
							ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell rm -f /sdcard/cckbg.png");
							string text2 = Application.StartupPath + "\\Devices\\cckbg.png";
							if (File.Exists(text2))
							{
								File.Delete(text2);
							}
							if (!File.Exists(text2))
							{
								new WebClient().DownloadFile("https://cck.vn/Download/Update/cckbg.png", text2);
							}
							ADBHelperCCK.PushInfoFile(p_DeviceId, $"\"{text2}\" \"/sdcard/cckbg.png\"");
							ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell am startservice com.cck.support/.CckServiceBackground");
						}
						catch
						{
						}
						ADBHelperCCK.UnInstallApp(p_DeviceId, "hotspotshield.android.vpn");
						if (!ADBHelperCCK.IsInstallApp(p_DeviceId, "hotspotshield.android.vpn"))
						{
							tiktokVersion = "hotspotshield.apk";
							ChangeGridViewDevice(p_DeviceId, "Cài hotspotshield", Color.Green);
							if (!File.Exists(Application.StartupPath + "\\Devices\\" + tiktokVersion))
							{
								try
								{
									new WebClient().DownloadFile($"https://cck.vn/Download/Utils/{tiktokVersion}.rar", Application.StartupPath + "\\Devices\\" + tiktokVersion);
								}
								catch (Exception ex4)
								{
									Utils.CCKLog("TaskList", ex4.Message, "SetupPhone");
								}
							}
							ADBHelperCCK.InstallApp(p_DeviceId, Application.StartupPath + "\\Devices\\" + tiktokVersion);
							Thread.Sleep(2000);
						}
						ADBHelperCCK.UnInstallApp(p_DeviceId, "com.cell47.College_Proxy");
						if (!ADBHelperCCK.IsInstallApp(p_DeviceId, "com.cell47.College_Proxy"))
						{
							tiktokVersion = "CollegeProxy_10.1.1_Mod.apk";
							ChangeGridViewDevice(p_DeviceId, "Cài CCK College_Proxy", Color.Green);
							if (!File.Exists(Application.StartupPath + "\\Devices\\" + tiktokVersion))
							{
								try
								{
									new WebClient().DownloadFile(string.Format("https://cck.vn/Download/Utils/{0}.rar", "College_Proxy.apk"), Application.StartupPath + "\\Devices\\" + tiktokVersion);
								}
								catch (Exception ex5)
								{
									Utils.CCKLog("TaskList", ex5.Message, "SetupPhone");
								}
							}
							ADBHelperCCK.InstallApp(p_DeviceId, Application.StartupPath + "\\Devices\\" + tiktokVersion);
							Thread.Sleep(2000);
						}
						if (ADBHelperCCK.IsInstallApp(p_DeviceId, "com.cell47.College_Proxy"))
						{
							list.Add("College_Proxy");
						}
						ChangeGridViewDevice(p_DeviceId, "Cài ADB Keyboard", Color.Green);
						if (!File.Exists(Application.StartupPath + "\\Devices\\ADBKeyboard.apk"))
						{
							try
							{
								new WebClient().DownloadFile("https://cck.vn/Download/Utils/ADBKeyboard.apk.rar", Application.StartupPath + "\\Devices\\ADBKeyboard.apk");
							}
							catch (Exception ex6)
							{
								Utils.CCKLog("TaskList", ex6.Message, "SetupPhone");
							}
						}
						if (File.Exists(Application.StartupPath + "\\Devices\\ADBKeyboard.apk"))
						{
							ADBHelperCCK.InstallApp(p_DeviceId, Application.StartupPath + "\\Devices\\ADBKeyboard.apk");
							ADBHelperCCK.ShowTextMessageOnPhone(p_DeviceId, "ADB Keyboard");
							Thread.Sleep(1000);
						}
						if (list.Count == 0)
						{
							ChangeGridViewDevice(p_DeviceId, "Chưa cài được gì", Color.Red);
						}
						else
						{
							ChangeGridViewDevice(p_DeviceId, string.Join(", ", list), Color.Green);
						}
						ADBHelperCCK.StartInstallRoot(device);
					}).Start();
				}
				catch (Exception ex)
				{
					Utils.CCKLog("Setup " + p_DeviceId, ex.Message);
				}
			}
		}

		private void SetupPhoneAndroid8S7(List<string> lst)
		{
			ThreadPool.SetMinThreads(100, 4);
			ServicePointManager.DefaultConnectionLimit = 500;
			int sPort = 4723;
			foreach (string p_DeviceId in lst)
			{
				try
				{
					new Task(delegate
					{
						DeviceEntity obj2 = new DeviceEntity
						{
							Name = p_DeviceId,
							DeviceId = p_DeviceId,
							SystemPort = 12923 - sPort
						};
						int num = sPort;
						sPort = num + 1;
						obj2.Port = num;
						obj2.Version = ADBHelperCCK.GetAndroidVersion(p_DeviceId);
						DeviceEntity device = obj2;
						AndroidDriver<AndroidElement> driver = ADBHelperCCK.StartPhone(device);
						ShowData(p_DeviceId, driver, device);
					}).Start();
				}
				catch
				{
				}
			}
		}

		public string GetPageSource(RemoteWebDriver driver, DeviceEntity device)
		{
			try
			{
				return driver.PageSource;
			}
			catch
			{
				for (int i = 0; i < 5; i++)
				{
					driver = ADBHelperCCK.StartPhone(device);
					if (driver != null)
					{
						try
						{
							return driver.PageSource;
						}
						catch
						{
							return GetPageSource(driver, device);
						}
					}
				}
			}
			return "";
		}

		private void ShowData(string p_DeviceId, RemoteWebDriver driver, DeviceEntity device)
		{
			ConnectWifi(driver, device, null);
			ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell rm -f /sdcard/*.txt");
			ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell rm -f /sdcard/*.vcf");
			ADBHelperCCK.TurnScreenOnDevice(p_DeviceId);
			try
			{
				string text = "";
				text = GetPageSource(driver, device);
				ADBHelperCCK.ExecuteCMD(p_DeviceId, " shell am start -a android.settings.SETTINGS");
				Thread.Sleep(1000);
				if (driver != null)
				{
					text = GetPageSource(driver, device);
					IWebElement webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Screen lock, fingerprint\"]", driver);
					if (webElement != null)
					{
						webElement.Click();
						Thread.Sleep(1000);
						webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Screen lock\"]", driver);
						if (webElement != null)
						{
							webElement.Click();
							Thread.Sleep(1000);
							webElement = ADBHelperCCK.AppGetObject("//*[@text=\"None\"]", driver);
							if (webElement != null)
							{
								webElement.Click();
								Thread.Sleep(1000);
							}
						}
					}
				}
				Point screenResolution = ADBHelperCCK.GetScreenResolution(p_DeviceId);
				ADBHelperCCK.ExecuteCMD(p_DeviceId, " shell am start -a com.android.settings.APPLICATION_DEVELOPMENT_SETTINGS");
				int num = 0;
				while (!driver.PageSource.Contains("Root access") && num < 10)
				{
					num++;
					ADBHelperCCK.Swipe(p_DeviceId, screenResolution.X / 2, screenResolution.Y * 3 / 4, screenResolution.X / 2, screenResolution.Y / 4, 1000);
				}
				Thread.Sleep(1000);
				IWebElement webElement2 = ADBHelperCCK.AppGetObject("//*[@text=\"Root access\"]", driver);
				if (webElement2 != null)
				{
					webElement2.Click();
					Thread.Sleep(1000);
					webElement2 = ADBHelperCCK.AppGetObject("//*[@text=\"ADB only\"]", driver);
					if (webElement2 != null)
					{
						webElement2.Click();
						Thread.Sleep(1000);
						webElement2 = ADBHelperCCK.AppGetObject("//*[@text=\"OK\"]", driver);
						if (webElement2 != null)
						{
							webElement2.Click();
							Thread.Sleep(1000);
						}
					}
				}
				if (Utils.Convert2Int(ADBHelperCCK.GetAndroidVersion(p_DeviceId)) < 9 && !ADBHelperCCK.IsInstallApp(p_DeviceId, "de.robv.android.xposed.installer"))
				{
					if (!File.Exists(Application.StartupPath + "\\Devices\\edxposed.manager.apk"))
					{
						try
						{
							new WebClient().DownloadFile("https://cck.vn/Download/Utils/xposed.installer.3.1.5.apk.rar", Application.StartupPath + "\\Devices\\xposed.installer.3.1.5.apk");
						}
						catch (Exception ex)
						{
							Utils.CCKLog("TaskList", ex.Message, "SetupPhone");
						}
					}
					if (File.Exists(Application.StartupPath + "\\Devices\\xposed.installer.3.1.5.apk"))
					{
						ADBHelperCCK.InstallApp(p_DeviceId, Application.StartupPath + "\\Devices\\xposed.installer.3.1.5.apk");
						ADBHelperCCK.ShowTextMessageOnPhone(p_DeviceId, "Setup xposed.installer Finished");
						Thread.Sleep(5000);
					}
				}
				while (true)
				{
					ADBHelperCCK.CloseApp(p_DeviceId, "de.robv.android.xposed.installer");
					ADBHelperCCK.OpenApp(p_DeviceId, "de.robv.android.xposed.installer");
					Thread.Sleep(2000);
					text = driver.PageSource;
					IWebElement webElement3 = ADBHelperCCK.AppGetObject("//*[@text=\"OK\"]", driver);
					if (webElement3 == null)
					{
						break;
					}
					webElement3.Click();
					Thread.Sleep(1000);
					webElement3 = ADBHelperCCK.AppGetObject("//*[@text=\"Version 90-beta3\"]", driver);
					if (webElement3 == null)
					{
						break;
					}
					webElement3.Click();
					Thread.Sleep(1000);
					webElement3 = ADBHelperCCK.AppGetObject("//*[@text=\"Install\"]", driver);
					if (webElement3 == null)
					{
						break;
					}
					webElement3.Click();
					Thread.Sleep(1000);
					while (true)
					{
						Thread.Sleep(1000);
						text = driver.PageSource;
						if (!text.Contains("Failed to get root access"))
						{
							if (!text.Contains("Grand"))
							{
							}
							if (!text.Contains("/system/lib/libart.so"))
							{
								if (text.Contains(""))
								{
								}
								continue;
							}
							ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell magisk --remove-modules");
							Thread.Sleep(60000);
							break;
						}
						ADBHelperCCK.CloseApp(p_DeviceId, "com.topjohnwu.magisk");
						ADBHelperCCK.OpenApp(p_DeviceId, "com.topjohnwu.magisk");
						Thread.Sleep(5000);
						webElement3 = ADBHelperCCK.AppGetObject("//*[@content-desc=\"Superuser\"]", driver);
						if (webElement3 == null)
						{
							break;
						}
						webElement3.Click();
						Thread.Sleep(1000);
						List<IWebElement> list = ADBHelperCCK.AppGetObjects("//android.widget.Switch", driver);
						foreach (IWebElement item in list)
						{
							if (item.GetAttribute("text") == "OFF")
							{
								item.Click();
							}
						}
						break;
					}
				}
				ADBHelperCCK.CloseApp(p_DeviceId, "com.topjohnwu.magisk");
				ADBHelperCCK.OpenApp(p_DeviceId, "com.topjohnwu.magisk");
				Thread.Sleep(1000);
				string[] array = new string[2] { "Riru.zip", "Riru-edXposed.zip" };
				string[] array2 = array;
				foreach (string text2 in array2)
				{
					IWebElement webElement4 = ADBHelperCCK.AppGetObject("//*[@content-desc=\"Modules\"]", driver);
					if (webElement4 == null)
					{
						continue;
					}
					webElement4.Click();
					Thread.Sleep(2000);
					webElement4 = ADBHelperCCK.AppGetObject("//*[@text=\"Install from storage\"]", driver);
					if (webElement4 == null)
					{
						continue;
					}
					webElement4.Click();
					Thread.Sleep(2000);
					text = GetPageSource(driver, device);
					if (text.ToUpper().Contains("TEXT=\"ALLOW\""))
					{
						webElement4 = ADBHelperCCK.AppGetObject("//*[upper-case(@text)=\"ALLOW\"]", driver);
						if (webElement4 != null)
						{
							webElement4.Click();
							Thread.Sleep(2000);
						}
					}
					webElement4 = ADBHelperCCK.AppGetObject("//*[@content-desc=\"More options\"]", driver);
					if (webElement4 == null)
					{
						continue;
					}
					webElement4.Click();
					Thread.Sleep(2000);
					text = GetPageSource(driver, device);
					if (text.ToLower().Contains("\"show internal storage\""))
					{
						webElement4 = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"show internal storage\"]", driver);
						if (webElement4 != null)
						{
							webElement4.Click();
							Thread.Sleep(1000);
						}
					}
					else
					{
						ADBHelperCCK.Tap(p_DeviceId, screenResolution.X / 2, screenResolution.Y / 2);
					}
					webElement4 = ADBHelperCCK.AppGetObject("//*[lower-case(@content-desc)=\"show roots\"]", driver);
					if (webElement4 != null)
					{
						webElement4.Click();
						Thread.Sleep(1000);
					}
					webElement4 = ADBHelperCCK.AppGetObject("//*[@resource-id=\"android:id/summary\"]", driver);
					if (webElement4 != null)
					{
						webElement4.Click();
						Thread.Sleep(1000);
					}
					webElement4 = ADBHelperCCK.AppGetObject($"//*[@text=\"{text2}\"]", driver);
					if (webElement4 == null)
					{
						continue;
					}
					webElement4.Click();
					while (!driver.PageSource.Contains("text=\"Reboot\""))
					{
						Thread.Sleep(5000);
					}
					if (!(text2 == "Riru.zip"))
					{
						IWebElement webElement5 = ADBHelperCCK.AppGetObject("//*[@text=\"Reboot\"]", driver);
						if (webElement5 != null)
						{
							webElement5.Click();
							Thread.Sleep(60000);
						}
					}
					else
					{
						ADBHelperCCK.Back(p_DeviceId);
					}
				}
			}
			catch
			{
			}
		}

		private void ConnectWifi1(RemoteWebDriver m_driver, DeviceEntity entity)
		{
			ADBHelperCCK.ExecuteCMD(entity.DeviceId, " shell am start -a android.settings.WIRELESS_SETTINGS");
			Thread.Sleep(2000);
			IWebElement webElement = ADBHelperCCK.AppGetObject("//*[@content-desc=\"Wi‑Fi\"]", m_driver);
			if (webElement == null)
			{
				return;
			}
			if (webElement.Text == "OFF")
			{
				webElement.Click();
			}
			Thread.Sleep(1000);
			webElement = ADBHelperCCK.AppGetObject("//*[@text=\"Wi‑Fi\"]", m_driver);
			ADBHelperCCK.TapLong(entity.DeviceId, webElement.Location.X + webElement.Size.Width / 2, webElement.Location.Y + webElement.Size.Height / 2);
			Thread.Sleep(5000);
			List<string> list = Utils.ReadTextFile(CaChuaConstant.WIFI).Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
			if (list != null && list.Count == 0)
			{
				return;
			}
			string[] array = list[new Random().Next(list.Count)].Split("|;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			string text = ((array.Length != 0) ? array[0] : "");
			string text2 = ((array.Length > 1) ? array[1] : "");
			IWebElement webElement2 = ADBHelperCCK.WaitMe("//*[lower-case(@text)=\"" + text.ToLower() + "\"]", m_driver);
			if (webElement2 == null)
			{
				return;
			}
			webElement2.Click();
			Thread.Sleep(3000);
			string pageSource = GetPageSource(m_driver, entity);
			if (pageSource.Contains("android.widget.EditText"))
			{
				IWebElement webElement3 = ADBHelperCCK.AppGetObject("//android.widget.EditText", m_driver);
				if (webElement3 != null)
				{
					webElement3.SendKeys(text2);
					Thread.Sleep(1000);
					ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"connect\" or lower-case(@content-desc)=\"connect\"]", m_driver)?.Click();
				}
			}
			pageSource = GetPageSource(m_driver, entity);
			while (!pageSource.Contains("text=\"Connected\""))
			{
				Thread.Sleep(2000);
				pageSource = GetPageSource(m_driver, entity);
				ADBHelperCCK.AppGetObject("//*[@resource-id=\"com.android.settings:id/wifi_menu_loadinglist\"]", m_driver)?.Click();
				if (pageSource.Contains("text=\"Saved\""))
				{
					ADBHelperCCK.AppGetObject("//*[contains(@content-desc,',Saved,')]", m_driver)?.Click();
				}
				if (pageSource.ToLower().Contains("no internet"))
				{
					ADBHelperCCK.AppGetObject("//*[contains(lower-case(@content-desc),'no internet')]", m_driver)?.Click();
				}
			}
			Thread.Sleep(1000);
		}

		private void ConnectWifi(RemoteWebDriver m_driver, DeviceEntity entity, DataGridViewRow row)
		{
			List<string> list = Utils.ReadTextFile(CaChuaConstant.WIFI).Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
			if (list != null && list.Count == 0)
			{
				return;
			}
			if (row != null)
			{
				row.Cells["Message"].Value = "Open android.settings.WIRELESS_SETTINGS";
			}
			string[] array = list[new Random().Next(list.Count)].Split("|;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			ADBHelperCCK.ExecuteCMD(entity.DeviceId, " shell am start -a android.settings.WIRELESS_SETTINGS");
			Thread.Sleep(2000);
			IWebElement webElement = ADBHelperCCK.WaitMe("//*[@content-desc=\"Wi-Fi\" or @text=\"Wi-Fi\"]", m_driver);
			if (webElement != null)
			{
				if (webElement.Text != null && webElement.Text.ToUpper() == "OFF")
				{
					webElement.Click();
				}
				Thread.Sleep(1000);
				webElement = ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"wi-fi\"]", m_driver);
				if (webElement != null)
				{
					ADBHelperCCK.TapLong(entity.DeviceId, webElement.Location.X + webElement.Size.Width / 2, webElement.Location.Y + webElement.Size.Height / 2);
				}
				Thread.Sleep(5000);
				string text = ((array.Length != 0) ? array[0] : "");
				string text2 = ((array.Length > 1) ? array[1] : "");
				IWebElement webElement2 = ADBHelperCCK.WaitMeCount("//*[lower-case(@text)=\"" + text.ToLower() + "\"]", m_driver);
				if (webElement2 == null)
				{
					return;
				}
				webElement2.Click();
				Thread.Sleep(3000);
				string pageSource = GetPageSource(m_driver, entity);
				if (pageSource.Contains("android.widget.EditText"))
				{
					IWebElement webElement3 = ADBHelperCCK.AppGetObject("//android.widget.EditText", m_driver);
					if (webElement3 != null)
					{
						webElement3.SendKeys(text2);
						Thread.Sleep(1000);
						ADBHelperCCK.AppGetObject("//*[lower-case(@text)=\"connect\" or lower-case(@content-desc)=\"connect\"]", m_driver)?.Click();
					}
				}
				pageSource = GetPageSource(m_driver, entity);
				while (true)
				{
					if (!pageSource.Contains("text=\"Connected\""))
					{
						Thread.Sleep(2000);
						pageSource = GetPageSource(m_driver, entity);
						ADBHelperCCK.AppGetObject("//*[@resource-id=\"com.android.settings:id/wifi_menu_loadinglist\"]", m_driver)?.Click();
						if (pageSource.Contains("text=\"Saved\""))
						{
							ADBHelperCCK.AppGetObject("//*[contains(@content-desc,',Saved,')]", m_driver)?.Click();
						}
						if (!pageSource.ToLower().Contains("text=\"forget\""))
						{
							if (pageSource.ToLower().Contains("no internet"))
							{
								ADBHelperCCK.AppGetObject("//*[contains(lower-case(@content-desc),'no internet')]", m_driver)?.Click();
							}
							continue;
						}
						break;
					}
					Thread.Sleep(1000);
					if (row != null)
					{
						row.Cells["Message"].Value = "Kiểm tra lại Wifi xem đã OK hết chưa";
					}
					break;
				}
			}
			else
			{
				File.AppendAllText("wifi_" + entity.DeviceId + ".txt", m_driver.PageSource);
				if (row != null)
				{
					row.Cells["Message"].Value = "Không mở được Setting Wifi";
				}
			}
		}

		private void DownloadAndInstallNodeJs()
		{
			if (!IsNodeInstalled())
			{
				DownloadAndInstallNodeJs("https://nodejs.org/dist/v20.4.0/node-v20.4.0-x64.msi");
			}
			InstallAppium("1.20.1");
		}

		private static bool IsNodeInstalled()
		{
			try
			{
				Process process = new Process();
				process.StartInfo.FileName = "node";
				process.StartInfo.Arguments = "--version";
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.CreateNoWindow = true;
				process.Start();
				string value = process.StandardOutput.ReadToEnd().Trim();
				process.WaitForExit();
				process.Close();
				return !string.IsNullOrEmpty(value);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Lỗi khi kiểm tra Node.js: " + ex.Message);
				return false;
			}
		}

		private static void DownloadAndInstallNodeJs(string url)
		{
			try
			{
				string text = Path.Combine(Path.GetTempPath(), "node.msi");
				using (WebClient webClient = new WebClient())
				{
					webClient.DownloadFile(url, text);
				}
				Process process = new Process();
				process.StartInfo.FileName = "msiexec";
				process.StartInfo.Arguments = "/i " + text + " /quiet /qn";
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.Start();
				process.WaitForExit();
				process.Close();
				File.Delete(text);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Lỗi khi cài đặt Node.js: " + ex.Message);
			}
		}

		private static bool IsAppiumInstalled(string version)
		{
			try
			{
				Process process = new Process();
				process.StartInfo.FileName = "appium";
				process.StartInfo.Arguments = " --version";
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.CreateNoWindow = true;
				process.Start();
				string text = process.StandardOutput.ReadToEnd().Trim();
				process.WaitForExit();
				process.Close();
				return text == version;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Lỗi khi kiểm tra Appium: " + ex.Message);
				return false;
			}
		}

		private static void InstallAppium(string version)
		{
			try
			{
				Process process = new Process();
				process.StartInfo.FileName = "npm";
				process.StartInfo.Arguments = "install -g appium@" + version;
				process.Start();
				process.WaitForExit();
				process.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Lỗi khi cài đặt Appium: " + ex.Message);
			}
		}

		private void btnSetup_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
				DataGridViewSelectedRowCollection selectedRows = dataGridViewPhone.SelectedRows;
				if (selectedRows.Count > 0)
				{
					listSerialDevice.Clear();
					foreach (DataGridViewRow item in selectedRows)
					{
						if (item.Cells["Phone"].Value != null)
						{
							listSerialDevice.Add(item.Cells["Phone"].Value.ToString());
						}
					}
				}
				try
				{
					SetupPhone(listSerialDevice);
				}
				catch
				{
				}
			}).Start();
		}

		private void LoadDevices()
		{
			try
			{
				dicDevices.Clear();
				DataTable dataTable = new SQLiteUtils().ExecuteQuery("Select deviceId,Name from tblDevices");
				if (dataTable == null)
				{
					return;
				}
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
			catch
			{
			}
		}

		private void dataGridViewPhone_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
		{
			try
			{
				if (e.RowIndex != -1)
				{
					if (dataGridViewPhone.Columns.Contains("Phone") && dataGridViewPhone.Rows[e.RowIndex].Selected)
					{
						selectedDevice = dataGridViewPhone.Rows[e.RowIndex].Cells["Phone"].Value.ToString();
						txtId.Text = selectedDevice;
						txtId.ReadOnly = true;
						txtName.Text = dataGridViewPhone.Rows[e.RowIndex].Cells["Name"].Value.ToString();
					}
					else
					{
						selectedDevice = "";
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private void dataGridViewPhone_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Right)
			{
				return;
			}
			ContextMenu contextMenu = new ContextMenu();
			new ContextMenu();
			new ContextMenu();
			MenuItem menuItem = new MenuItem("Remove Bootloop");
			menuItem.Name = "bootlop";
			contextMenu.MenuItems.Add(menuItem);
			menuItem = new MenuItem("Bật bàn phím");
			menuItem.Name = "turnonkeyboard";
			contextMenu.MenuItems.Add(menuItem);
			menuItem = new MenuItem("Reboot Download Mode");
			menuItem.Name = "adbrootdownload";
			contextMenu.MenuItems.Add(menuItem);
			menuItem = new MenuItem("Reset Factory J7Pro");
			menuItem.Name = "resetfactory";
			contextMenu.MenuItems.Add(menuItem);
			menuItem = new MenuItem("Tắt tất cả các app đang mở");
			menuItem.Name = "closeapp";
			contextMenu.MenuItems.Add(menuItem);
			menuItem = new MenuItem("Kiểm tra kết nối Internet");
			menuItem.Name = "internet";
			contextMenu.MenuItems.Add(menuItem);
			menuItem = new MenuItem("Tắt NFC");
			menuItem.Name = "nfc";
			contextMenu.MenuItems.Add(menuItem);
			menuItem = new MenuItem("Thêm Proxy");
			menuItem.Name = "addproxy";
			contextMenu.MenuItems.Add(menuItem);
			menuItem = new MenuItem("Setup ADB Root");
			menuItem.Name = "adbroot";
			contextMenu.MenuItems.Add(menuItem);
			menuItem = new MenuItem("Setup Enviroment");
			menuItem.Name = "setup";
			contextMenu.MenuItems.Add(menuItem);
			menuItem = new MenuItem("Report Error");
			menuItem.Name = "copyerror";
			contextMenu.MenuItems.Add(menuItem);
			menuItem = new MenuItem("View Phone");
			menuItem.Name = "start";
			contextMenu.MenuItems.Add(menuItem);
			menuItem = new MenuItem("Clear Proxy On Phone");
			menuItem.Name = "clearproxy";
			contextMenu.MenuItems.Add(menuItem);
			menuItem = new MenuItem("Copy File from Computer");
			menuItem.Name = "copypic";
			contextMenu.MenuItems.Add(menuItem);
			menuItem = new MenuItem("Import danh bạ");
			menuItem.Name = "import";
			contextMenu.MenuItems.Add(menuItem);
			menuItem = new MenuItem("Show Phone IP");
			menuItem.Name = "ip";
			contextMenu.MenuItems.Add(menuItem);
			menuItem = new MenuItem("Chụp ảnh màn hình");
			menuItem.Name = "capture";
			contextMenu.MenuItems.Add(menuItem);
			menuItem = new MenuItem("Change Device Info");
			menuItem.Name = "change";
			contextMenu.MenuItems.Add(menuItem);
			menuItem = new MenuItem("Enable ADB Over Network");
			menuItem.Name = "adbon";
			contextMenu.MenuItems.Add(menuItem);
			menuItem = new MenuItem("Show Device Name");
			menuItem.Name = "devicename";
			contextMenu.MenuItems.Add(menuItem);
			menuItem = new MenuItem("Check Tiktok Version");
			menuItem.Name = "version";
			contextMenu.MenuItems.Add(menuItem);
			menuItem = new MenuItem("Dev Tool - Copy");
			menuItem.Name = "xml";
			contextMenu.MenuItems.Add(menuItem);
			foreach (MenuItem menuItem2 in contextMenu.MenuItems)
			{
				menuItem2.Click += dataGridViewPhoneCreator_Click;
			}
			contextMenu.Show(dataGridViewPhone, new Point(e.X, e.Y));
		}

		private void dataGridViewPhoneCreator_Click(object sender, EventArgs e)
		{
			string name = ((MenuItem)sender).Name;
			new SQLiteUtils().GetAccountById(selectedUid);
			switch (name)
			{
			case "clearproxy":
				new Task(delegate
				{
					foreach (DataGridViewRow row6 in dataGridViewPhone.SelectedRows)
					{
						new Task(delegate
						{
							string text4 = row6.Cells["Phone"].Value.ToString();
							ChangeGridViewDevice(text4, "Start Clear", Color.Green);
							ADBHelperCCK.ClearProxy(text4);
							ChangeGridViewDevice(text4, "Done", Color.Green);
						}).Start();
					}
				}).Start();
				break;
			case "capture":
				new Task(delegate
				{
					string folder = Application.StartupPath + "\\ScreenCap";
					if (!Directory.Exists(folder))
					{
						Directory.CreateDirectory(folder);
					}
					List<Task> list4 = new List<Task>();
					foreach (DataGridViewRow row15 in dataGridViewPhone.SelectedRows)
					{
						if (row15.Cells["phone"].Value != null)
						{
							list4.Add(Task.Run(delegate
							{
								string text11 = row15.Cells["phone"].Value.ToString();
								string text12 = folder + "\\" + text11.Replace(":", "_") + ".png";
								ADBHelperCCK.ExecuteCMD(text11, " shell screencap -p /sdcard/screencap.png");
								ADBHelperCCK.ExecuteCMD(text11, " pull /sdcard/screencap.png \"" + text12 + "\"");
								ChangeGridViewDevice(text11, "Xong", Color.Green);
							}));
						}
					}
					Task.WhenAll(list4);
					Process.Start(folder);
				}).Start();
				break;
			case "bootlop":
				if (!(selectedDevice != ""))
				{
					break;
				}
				new Task(delegate
				{
					foreach (DataGridViewRow row7 in dataGridViewPhone.SelectedRows)
					{
						if (row7.Cells["phone"].Value != null)
						{
							new Task(delegate
							{
								ADBHelperCCK.ExecuteCMD(row7.Cells["phone"].Value.ToString(), "shell magisk --remove-modules");
							}).Start();
						}
					}
				}).Start();
				break;
			case "adbroot":
				new Task(delegate
				{
					frmModule frmModule2 = new frmModule();
					frmModule2.ShowDialog();
					List<string> lstModule = frmModule2.lst;
					foreach (DataGridViewRow row8 in dataGridViewPhone.SelectedRows)
					{
						new Task(delegate
						{
							object value2 = row8.Cells["phone"].Value;
							if (value2 != null)
							{
								StartInstallRoot(value2.ToString(), lstModule);
							}
						}).Start();
					}
				}).Start();
				break;
			case "adbrootdownload":
				new Task(delegate
				{
					foreach (DataGridViewRow row12 in dataGridViewPhone.SelectedRows)
					{
						if (row12.Cells["phone"].Value != null)
						{
							Task.Run(delegate
							{
								string text8 = row12.Cells["phone"].Value.ToString();
								ChangeGridViewDevice(text8, "Start", Color.Green);
								ADBHelperCCK.ExecuteCMD(text8, " reboot download");
								ChangeGridViewDevice(text8, "Done", Color.Green);
							});
						}
					}
				}).Start();
				break;
			case "xml":
				if (selectedDevice != "")
				{
					string text = ADBHelperCCK.DumpXml(selectedDevice);
					Clipboard.SetText(text);
					MessageBox.Show("Copied to Clipboard");
				}
				break;
			case "change":
				foreach (DataGridViewRow row in dataGridViewPhone.SelectedRows)
				{
					new Task(delegate
					{
						object value = row.Cells["phone"].Value;
						if (value != null)
						{
							DateTime now = DateTime.Now;
							ChangeDevice(value.ToString());
							double totalMilliseconds = DateTime.Now.Subtract(now).TotalMilliseconds;
							ChangeGridViewDevice(value.ToString(), "Changed: " + totalMilliseconds, Color.Green);
						}
					}).Start();
				}
				break;
			case "start":
				new Task(delegate
				{
					Rectangle bounds = Screen.PrimaryScreen.Bounds;
					_ = bounds.Width;
					int num = bounds.Height;
					ADBHelperCCK.DisplayPhone(selectedDevice, selectedDevice, 50, 30, (num - 100) * 1440 / 3040, num - 100);
				}).Start();
				break;
			case "devicename":
				new Task(delegate
				{
					foreach (DataGridViewRow row3 in dataGridViewPhone.SelectedRows)
					{
						object phoneId = row3.Cells["phone"].Value;
						if (phoneId != null)
						{
							new Task(delegate
							{
								for (int i = 0; i < 5; i++)
								{
									ADBHelperCCK.ShowTextMessageOnPhone(phoneId.ToString(), row3.Cells["Name"].Value.ToString());
									Thread.Sleep(2000);
								}
							}).Start();
						}
					}
				}).Start();
				break;
			case "internet":
				Task.Run(delegate
				{
					frmInputControl frmInputControl2 = new frmInputControl("Web");
					frmInputControl2.SetText("https://domains.google.com/checkip");
					frmInputControl2.ShowDialog();
					string result = frmInputControl2.Result;
					foreach (DataGridViewRow row4 in dataGridViewPhone.SelectedRows)
					{
						Task.Run(delegate
						{
							string text2 = row4.Cells["phone"].Value.ToString();
							ChangeGridViewDevice(text2, "Start", Color.Green);
							ADBHelperCCK.OpenLinkWeb(text2, result ?? "https://www.whatismyip.com/");
							ChangeGridViewDevice(text2, "Done", Color.Green);
						});
					}
				});
				break;
			case "resetfactory":
				new Task(delegate
				{
					foreach (DataGridViewRow row5 in dataGridViewPhone.SelectedRows)
					{
						if (row5.Cells["phone"].Value != null)
						{
							Task.Run(delegate
							{
								string text3 = row5.Cells["phone"].Value.ToString();
								ChangeGridViewDevice(text3, "Start", Color.Green);
								FactoryReset(text3);
								ChangeGridViewDevice(text3, "Done", Color.Green);
							});
						}
					}
				}).Start();
				break;
			case "copypic":
			{
				if (!(selectedDevice != ""))
				{
					break;
				}
				OpenFileDialog openFileDialog = new OpenFileDialog();
				if (openFileDialog.ShowDialog() != DialogResult.OK)
				{
					break;
				}
				List<string> files = openFileDialog.FileNames.ToList();
				if (files.Count <= 0)
				{
					break;
				}
				new Task(delegate
				{
					foreach (DataGridViewRow row9 in dataGridViewPhone.SelectedRows)
					{
						string p_DeviceId = row9.Cells["phone"].Value.ToString();
						if (row9.Cells["phone"].Value != null)
						{
							new Task(delegate
							{
								ADBHelperCCK.SetStoragePermission(row9.Cells["phone"].Value.ToString());
								foreach (string item2 in files)
								{
									ADBHelperCCK.PushInfoFile(row9.Cells["phone"].Value.ToString(), "\"" + item2 + "\" \"/sdcard/" + Path.GetFileName(item2) + "\"");
									ADBHelperCCK.ExecuteCMD(p_DeviceId, " shell am broadcast -a android.intent.action.MEDIA_SCANNER_SCAN_FILE -d file:///mnt/sdcard/" + Path.GetFileName(item2));
									ADBHelperCCK.ShowTextMessageOnPhone(row9.Cells["phone"].Value.ToString(), Path.GetFileName(item2));
								}
								ADBHelperCCK.ShowTextMessageOnPhone(row9.Cells["phone"].Value.ToString(), "Done");
							}).Start();
						}
					}
				}).Start();
				break;
			}
			case "version":
				Task.Run(delegate
				{
					foreach (DataGridViewRow row10 in dataGridViewPhone.SelectedRows)
					{
						if (row10.Cells["phone"].Value != null)
						{
							new Task(delegate
							{
								string text5 = row10.Cells["phone"].Value.ToString();
								string tiktokVersion = ADBHelperCCK.GetTiktokVersion(text5);
								ChangeGridViewDevice(text5, tiktokVersion, Color.Green);
							}).Start();
						}
					}
				});
				break;
			case "turnonkeyboard":
				new Task(delegate
				{
					foreach (DataGridViewRow row11 in dataGridViewPhone.SelectedRows)
					{
						if (row11.Cells["phone"].Value != null)
						{
							new Task(delegate
							{
								string text6 = row11.Cells["phone"].Value.ToString();
								ADBHelperCCK.UnInstallApp(text6, "io.appium.settings");
								ADBHelperCCK.UnInstallApp(text6, "io.appium.uiautomator2.server.test");
								ADBHelperCCK.UnInstallApp(text6, "io.appium.uiautomator2.server");
								ChangeGridViewDevice(text6, "Bật bàn phím xong", Color.Green);
							}).Start();
						}
					}
				}).Start();
				break;
			case "nfc":
				new Task(delegate
				{
					try
					{
						int sPort = 4723;
						foreach (DataGridViewRow item in (IEnumerable)dataGridViewPhone.Rows)
						{
							Task.Run(delegate
							{
								try
								{
									if (item.Cells["Phone"].Value != null)
									{
										string text7 = item.Cells["Phone"].Value.ToString();
										DeviceEntity obj3 = new DeviceEntity
										{
											Name = text7,
											DeviceId = text7,
											SystemPort = 3477 + sPort
										};
										int num2 = sPort;
										sPort = num2 + 1;
										obj3.Port = num2;
										obj3.Version = ADBHelperCCK.GetAndroidVersion(text7);
										DeviceEntity deviceEntity = obj3;
										ADBHelperCCK.CloseApp(deviceEntity.DeviceId, "com.android.settings");
										ADBHelperCCK.ExecuteCMD(deviceEntity.DeviceId, " shell am start -a android.settings.WIRELESS_SETTINGS");
										AndroidDriver<AndroidElement> androidDriver = ADBHelperCCK.StartPhone(deviceEntity);
										if (androidDriver != null)
										{
											IWebElement webElement = ADBHelperCCK.AppFindAndGetObjectDown("//*[lower-case(@text) = 'nfc and payment']", androidDriver, text7);
											if (webElement != null)
											{
												IWebElement webElement2 = ADBHelperCCK.AppGetObject("//*[contains(lower-case(@text),'card mode')]", androidDriver);
												if (webElement2 != null)
												{
													webElement.Click();
												}
											}
										}
									}
								}
								catch (Exception)
								{
								}
							});
						}
					}
					catch
					{
					}
				}).Start();
				break;
			case "import":
				if (!(selectedDevice != ""))
				{
					break;
				}
				try
				{
					ChangeGridViewDevice(selectedDevice, "Starting ...", Color.Red);
					List<string> list = new List<string> { "shell pm clear com.android.settings", "shell pm clear com.facebook.katana", "shell  pm grant com.facebook.katana android.permission.READ_CONTACTS", "shell  pm grant com.facebook.katana android.permission.CALL_PHONE", "shell  pm grant com.facebook.katana android.permission.CAMERA", "shell  pm grant com.facebook.katana android.permission.ACCESS_FINE_LOCATION", "shell  pm grant com.facebook.katana android.permission.READ_EXTERNAL_STORAGE" };
					foreach (string item3 in list)
					{
						ADBHelperCCK.ExecuteCMD(selectedDevice, item3);
						string[] array = item3.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
						ADBHelperCCK.ShowTextMessageOnPhone(selectedDevice, array[array.Length - 1]);
						ChangeGridViewDevice(selectedDevice, "Set permission " + array[array.Length - 1], Color.Red);
					}
					if (!Directory.Exists("Vcf"))
					{
						Directory.CreateDirectory("Vcf");
					}
					string path = string.Format(CaChuaConstant.VCF_FileFormat, selectedDevice);
					List<string> list2 = File.ReadLines(CaChuaConstant.SuggessPhone).ToList();
					VNNameEntity vNNameEntity = new VNNameEntity();
					if (File.Exists(CaChuaConstant.VN_Name))
					{
						vNNameEntity = new JavaScriptSerializer().Deserialize<VNNameEntity>(Utils.ReadTextFile(CaChuaConstant.VN_Name));
					}
					List<Contacts> list3 = vCardContacts.CreateList(list2, vNNameEntity.FirstName, vNNameEntity.LastName);
					if (list3.Count > 0)
					{
						File.WriteAllText(path, new JavaScriptSerializer().Serialize(list3));
						Thread.Sleep(100);
					}
					vNNameEntity = null;
					list2.Clear();
					list2 = null;
					ChangeGridViewDevice(selectedDevice, "Create Contact", Color.Red);
					ADBHelperCCK.AddContact2Phone(selectedDevice, selectedDevice);
					ChangeGridViewDevice(selectedDevice, "Successful contact creation", Color.Red);
					ADBHelperCCK.ShowTextMessageOnPhone(selectedDevice, "Successful contact creation");
					ChangeGridViewDevice(selectedDevice, "Change Device", Color.Red);
					ChangeDevice(selectedDevice.ToString());
					ChangeGridViewDevice(selectedDevice, "Successful", Color.Red);
				}
				catch
				{
				}
				break;
			case "closeapp":
				Task.Run(delegate
				{
					foreach (DataGridViewRow row13 in dataGridViewPhone.SelectedRows)
					{
						Task.Run(delegate
						{
							string text9 = row13.Cells["phone"].Value.ToString();
							ChangeGridViewDevice(text9, "Start", Color.Green);
							string input = ADBHelperCCK.ExecuteCMD(text9, "shell dumpsys activity activities | find \"Run #\"");
							Regex regex = new Regex("u([0-9]) ([^/]+)\\/");
							MatchCollection matchCollection = regex.Matches(input);
							foreach (Match item4 in matchCollection)
							{
								if (item4.Success)
								{
									string value3 = item4.Groups[1].Value;
									ADBHelperCCK.CloseApp(text9, value3);
									ChangeGridViewDevice(text9, "Close app: " + value3, Color.Green);
								}
							}
							ChangeGridViewDevice(text9, "Done", Color.Green);
						});
					}
				});
				break;
			case "adbon":
				new Task(delegate
				{
					foreach (DataGridViewRow row14 in dataGridViewPhone.SelectedRows)
					{
						if (row14.Cells["phone"].Value != null)
						{
							new Task(delegate
							{
								string text10 = row14.Cells["phone"].Value.ToString();
								ADBHelperCCK.ExecuteCMD(text10, " shell 'setprop service.adb.tcp.port 5555'");
								ChangeGridViewDevice(text10, "Enable ADB Over Network", Color.Green);
							}).Start();
						}
					}
				}).Start();
				break;
			case "setup":
				foreach (DataGridViewRow row2 in dataGridViewPhone.SelectedRows)
				{
					new Task(delegate
					{
						if (row2.Cells["phone"].Value != null)
						{
							SetupPhone(new List<string> { row2.Cells["phone"].Value.ToString() });
						}
					}).Start();
				}
				break;
			case "ip":
				if (!(selectedDevice != ""))
				{
					break;
				}
				new Task(delegate
				{
					foreach (DataGridViewRow row16 in dataGridViewPhone.SelectedRows)
					{
						if (row16.Cells["phone"].Value != null)
						{
							new Task(delegate
							{
								string clientIP = FaceBookHelper.GetClientIP(row16.Cells["phone"].Value.ToString(), selectedUid);
								for (int j = 0; j < 5; j++)
								{
									ADBHelperCCK.ShowTextMessageOnPhone(row16.Cells["phone"].Value.ToString(), clientIP);
									Thread.Sleep(2000);
								}
							}).Start();
						}
					}
				}).Start();
				break;
			case "addproxy":
				new Task(delegate
				{
					int sPort2 = 4723;
					frmInputControl frmInputControl3 = new frmInputControl(isSingleLine: false);
					frmInputControl3.ShowDialog();
					List<string> proxy = frmInputControl3.Result.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
					foreach (DataGridViewRow row17 in dataGridViewPhone.SelectedRows)
					{
						if (row17.Cells["phone"].Value != null && proxy.Count > 0)
						{
							Task.Run(delegate
							{
								string text13 = proxy[0];
								proxy.Add(text13);
								proxy.RemoveAt(0);
								string text14 = row17.Cells["phone"].Value.ToString();
								ChangeGridViewDevice(text14, "Start", Color.Green);
								DeviceEntity obj4 = new DeviceEntity
								{
									Name = text14,
									DeviceId = text14,
									SystemPort = 3477 + sPort2
								};
								int num3 = sPort2;
								sPort2 = num3 + 1;
								obj4.Port = num3;
								obj4.Version = ADBHelperCCK.GetAndroidVersion(text14);
								DeviceEntity device = obj4;
								string[] array2 = text13.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
								AndroidDriver<AndroidElement> p_driver = ((array2.Length == 4) ? ADBHelperCCK.StartPhone(device) : null);
								ADBHelperCCK.SetProxy(text14, text13, p_driver);
								ChangeGridViewDevice(text14, "Proxy: " + text13, Color.Green);
							});
						}
					}
				}).Start();
				break;
			case "copyerror":
				new Task(delegate
				{
					Cursor.Current = Cursors.WaitCursor;
					Application.DoEvents();
					foreach (DataGridViewRow selectedRow in dataGridViewPhone.SelectedRows)
					{
						object value4 = selectedRow.Cells["phone"].Value;
						if (value4 != null && value4 != "")
						{
							string text15 = ADBHelperCCK.DumpXmlFile(value4.ToString(), "Phone_" + value4, captureScreen: true);
							Thread.Sleep(100);
							if (File.Exists(text15))
							{
								string value5 = Utils.ReadTextFile(text15);
								Dictionary<string, string> obj5 = new Dictionary<string, string> { { "data", value5 } };
								string text16 = new Utils().PostData(Utils.ApiLocation + "/Api/Upload.aspx?file=Phone_" + value4, new JavaScriptSerializer().Serialize(obj5));
								if (text16 != "")
								{
									new WebClient().UploadFile(Utils.ApiLocation + "/Api/Upload.aspx?file=" + text16.Trim(), Path.GetDirectoryName(text15) + "\\" + Path.GetFileNameWithoutExtension(text15) + ".png");
								}
							}
							ADBHelperCCK.ShowTextMessageOnPhone(value4.ToString(), value4.ToString());
						}
					}
					Cursor.Current = Cursors.Default;
					Application.DoEvents();
					MessageBox.Show("Báo lỗi xong, nhắn qua Zalo 0904.868.545 cho Quý biết luôn nhé");
				}).Start();
				break;
			}
		}

		private void FactoryReset(string SerialNo)
		{
			DeviceEntity deivcePort = GetDeivcePort(SerialNo);
			deivcePort.Version = ADBHelperCCK.GetAndroidVersion(SerialNo);
			RemoteWebDriver remoteWebDriver = null;
			int num = 0;
			while (remoteWebDriver == null && num < 30)
			{
				ChangeGridViewDevice(SerialNo, "Start Phone : " + DateTime.Now.ToString(), Color.Green);
				remoteWebDriver = ADBHelperCCK.StartPhone(deivcePort);
				if (remoteWebDriver != null)
				{
					break;
				}
				num++;
				Thread.Sleep(2000);
			}
			if (remoteWebDriver == null)
			{
				return;
			}
			TiktokItem tiktokItem = new TiktokItem(remoteWebDriver, deivcePort, "", null, "", null);
			tiktokItem.RemoveSystemAccount(remoteWebDriver);
			ADBHelperCCK.CloseApp(SerialNo, "com.android.settings");
			ADBHelperCCK.OpenApp(SerialNo, "com.android.settings");
			Thread.Sleep(2000);
			IWebElement webElement = ADBHelperCCK.AppFindAndGetObjectDown("//*[contains(lower-case(@text),\"management\")]", remoteWebDriver, SerialNo, 20);
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(2000);
			List<IWebElement> list = ADBHelperCCK.AppFindAndGetObjectsDown("//*[lower-case(@text)=\"reset\"]", remoteWebDriver, SerialNo);
			if (list != null && list.Count > 0)
			{
				list[list.Count - 1].Click();
				Thread.Sleep(2000);
			}
			webElement = ADBHelperCCK.AppFindAndGetObjectDown("//*[lower-case(@text)=\"factory data reset\"]", remoteWebDriver, SerialNo);
			if (webElement != null)
			{
				webElement.Click();
				Thread.Sleep(2000);
			}
			webElement = ADBHelperCCK.AppFindAndGetObjectDown("//*[lower-case(@text)=\"reset\"]", remoteWebDriver, SerialNo, 50);
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(30000);
			webElement = ADBHelperCCK.AppFindAndGetObjectDown("//*[lower-case(@text)=\"delete all\"]", remoteWebDriver, SerialNo);
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			string cmd = "shell getprop sys.boot_completed";
			string text = "";
			DateTime dateTime = DateTime.Now.AddMinutes(5.0);
			while (text != "1")
			{
				text = ADBHelperCCK.ExecuteCMD(SerialNo, cmd).ToString().Trim();
				Thread.Sleep(5000);
				if (DateTime.Now > dateTime)
				{
					break;
				}
			}
			SetupPhone(new List<string> { SerialNo });
		}

		private void ChangeDevice(string p_DeviceId)
		{
			ChangeGridViewDevice(p_DeviceId, "Starting...", Color.Green);
			_ = DateTime.Now;
			DeviceHelper.ProduceTxtAddNew(p_DeviceId, Guid.NewGuid().ToString());
			ADBHelperCCK.PushInfoFile(p_DeviceId, "\"" + Application.StartupPath + "\\Devices\\CCKInfo_" + ADBHelperCCK.NormalizeDeviceName(p_DeviceId) + ".txt\" /sdcard/CCKInfo_" + ADBHelperCCK.NormalizeDeviceName(p_DeviceId) + ".txt");
			Thread.Sleep(1000);
			ADBHelperCCK.StartServiceFake(p_DeviceId);
			Thread.Sleep(5000);
			ADBHelperCCK.DeleteInfoDevice(p_DeviceId);
			ChangeGridViewDevice(p_DeviceId, "Successfully changed", Color.Green);
			int androidVersionNumber = ADBHelperCCK.GetAndroidVersionNumber(p_DeviceId);
			ADBHelperCCK.StopApp(p_DeviceId, (androidVersionNumber <= 8) ? "de.robv.android.xposed.installer" : "org.meowcat.edxposed.manager");
			ADBHelperCCK.OpenApp(p_DeviceId, (androidVersionNumber <= 8) ? "de.robv.android.xposed.installer" : "org.meowcat.edxposed.manager");
		}

		public void StartInstallRoot(string SerialNo, List<string> lst)
		{
			DeviceEntity deivcePort = GetDeivcePort(SerialNo);
			deivcePort.Version = ADBHelperCCK.GetAndroidVersion(SerialNo);
			RemoteWebDriver remoteWebDriver = null;
			int num = 0;
			while (remoteWebDriver == null && num < 30)
			{
				ChangeGridViewDevice(SerialNo, "Start Phone : " + DateTime.Now.ToString(), Color.Green);
				remoteWebDriver = ADBHelperCCK.StartPhone(deivcePort);
				if (remoteWebDriver != null)
				{
					break;
				}
				num++;
				Thread.Sleep(2000);
			}
			if (remoteWebDriver == null)
			{
				return;
			}
			if (lst.Count > 0 && lst[0] == "cbxTurnOffModule")
			{
				lst.RemoveAt(0);
				ADBHelperCCK.BackToHome(SerialNo);
				ADBHelperCCK.EnableModules(SerialNo);
			}
			ADBHelperCCK.BackToHome(SerialNo);
			if (lst.Count <= 0)
			{
				return;
			}
			ADBHelperCCK.OpenApp(SerialNo, "com.topjohnwu.magisk");
			Thread.Sleep(2000);
			string pageSource = remoteWebDriver.PageSource;
			num = 0;
			while (!pageSource.Contains("content-desc=\"Modules\"") && num++ < 30)
			{
				Thread.Sleep(2000);
				pageSource = remoteWebDriver.PageSource;
			}
			if (!pageSource.Contains("content-desc=\"Modules\""))
			{
				return;
			}
			IWebElement webElement = ADBHelperCCK.AppGetObject("//*[@content-desc=\"Modules\"]", remoteWebDriver);
			if (webElement == null)
			{
				return;
			}
			webElement.Click();
			Thread.Sleep(2000);
			ADBHelperCCK.Swipe(SerialNo, 200, 100, 200, 800);
			for (int i = 0; i < lst.Count; i++)
			{
				pageSource = remoteWebDriver.PageSource;
				if (!pageSource.Contains("Install from storage"))
				{
					continue;
				}
				IWebElement webElement2 = ADBHelperCCK.AppGetObject("//*[@text=\"Install from storage\"]", remoteWebDriver);
				if (webElement2 == null)
				{
					continue;
				}
				webElement2.Click();
				Thread.Sleep(2000);
				if (!remoteWebDriver.PageSource.Contains("content-desc=\"Search\""))
				{
					continue;
				}
				webElement2 = ADBHelperCCK.AppGetObject("//*[@content-desc=\"Search\"]", remoteWebDriver);
				if (webElement2 == null)
				{
					continue;
				}
				webElement2.Click();
				Thread.Sleep(1000);
				pageSource = remoteWebDriver.PageSource;
				if (!pageSource.Contains("text=\"Search…\""))
				{
					continue;
				}
				webElement2 = ADBHelperCCK.AppGetObject("//*[@text=\"Search…\"]", remoteWebDriver);
				if (webElement2 == null)
				{
					continue;
				}
				webElement2.SendKeys(lst[i]);
				Thread.Sleep(2000);
				pageSource = remoteWebDriver.PageSource;
				List<IWebElement> list = ADBHelperCCK.AppGetObjects($"//*[@text=\"{lst[i]}\"]", remoteWebDriver);
				if (list == null)
				{
					continue;
				}
				list[list.Count - 1].Click();
				while (!remoteWebDriver.PageSource.Contains("text=\"Done!\""))
				{
					Thread.Sleep(2000);
				}
				Thread.Sleep(2000);
				if (i < lst.Count - 1)
				{
					ADBHelperCCK.Back(SerialNo);
					continue;
				}
				list = ADBHelperCCK.AppGetObjects(string.Format("//*[@text=\"{0}\"]", "Reboot"), remoteWebDriver);
				if (list != null && list.Count > 0)
				{
					list[list.Count - 1].Click();
				}
			}
		}

		private void btnBri_Click(object sender, EventArgs e)
		{
			List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
			foreach (string p_DeviceId in listSerialDevice)
			{
				new Task(delegate
				{
					ADBHelperCCK.TurnScreenOnDevice(p_DeviceId);
					ADBHelperCCK.ExecuteCMD(p_DeviceId, " shell settings put system screen_brightness_mode 0");
					ADBHelperCCK.ExecuteCMD(p_DeviceId, $" shell settings put system screen_brightness {Math.Round((double)(int)nudBright.Value * 2.55)}");
				}).Start();
			}
		}

		private void cbxClearAccount_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void btnUpdateName_Click(object sender, EventArgs e)
		{
			btnLoadDevices_Click(null, null);
		}

		private void cbx4GOption_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void cbxshowPas_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void btnClean_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				foreach (DataGridViewRow item in (IEnumerable)dataGridViewPhone.Rows)
				{
					object phone = item.Cells["Phone"].Value;
					if (phone != null)
					{
						new Task(delegate
						{
							ADBHelperCCK.ExecuteCMD(phone.ToString(), " shell rm -r /sdcard/*.txt");
							ADBHelperCCK.ExecuteCMD(phone.ToString(), " shell rm -r /sdcard/*.mp4");
							ADBHelperCCK.ExecuteCMD(phone.ToString(), " shell rm -r /sdcard/*.jpg");
							ADBHelperCCK.ExecuteCMD(phone.ToString(), " shell rm -r /sdcard/*.png");
							ADBHelperCCK.ExecuteCMD(phone.ToString(), " shell rm -r /sdcard/Pictures/*.*");
						}).Start();
					}
				}
			}).Start();
		}

		private void btnShowPhone_Click(object sender, EventArgs e)
		{
			frmViewPhoneAll frmViewPhoneAll2 = new frmViewPhoneAll(numberOfDevice);
			frmViewPhoneAll2.Show();
		}

		private void btnReboot_Click(object sender, EventArgs e)
		{
			ThreadPool.SetMinThreads(100, 4);
			ServicePointManager.DefaultConnectionLimit = 500;
			new Task(delegate
			{
				List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
				foreach (string SerialNo in listSerialDevice)
				{
					new Task(delegate
					{
						ADBHelperCCK.ExecuteCMD(SerialNo, " reboot");
					}).Start();
				}
			}).Start();
		}

		private void nudPhoneDelay_ValueChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.PHONE_DELAY, nudPhoneDelay.Value.ToString());
		}

		private void cbxServer_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void cbxAutoupdate_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void cbxAddFriendAll_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void button2_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
				foreach (string SerialNo in listSerialDevice)
				{
					new Task(delegate
					{
						DeviceEntity deivcePort = GetDeivcePort(SerialNo);
						deivcePort.Version = ADBHelperCCK.GetAndroidVersion(SerialNo);
						RemoteWebDriver remoteWebDriver = null;
						int num = 0;
						while (remoteWebDriver == null && num < 5)
						{
							ChangeGridViewDevice(SerialNo, "Start Phone : " + DateTime.Now.ToString(), Color.Green);
							remoteWebDriver = ADBHelperCCK.StartPhone(deivcePort);
							if (remoteWebDriver != null)
							{
								break;
							}
							num++;
						}
						if (remoteWebDriver != null)
						{
							ADBHelperCCK.EnableModules(SerialNo);
							ADBHelperCCK.ExecuteCMD(SerialNo, "adb shell reboot -p");
						}
					}).Start();
				}
			}).Start();
		}

		private void btnAppium_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				frmSelectApp ctr = new frmSelectApp();
				ctr.StartPosition = FormStartPosition.CenterParent;
				ctr.ShowDialog();
				if (!(ctr.Text == ""))
				{
					List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
					foreach (string SerialNo in listSerialDevice)
					{
						new Task(delegate
						{
							if (ctr.Text.Trim() != "")
							{
								ADBHelperCCK.UnInstallApp(SerialNo, ctr.Text.Trim());
								ChangeGridViewDevice(SerialNo, "Removing ...", Color.Green);
							}
							ADBHelperCCK.UnInstallApp(SerialNo, "io.appium.settings");
							ADBHelperCCK.UnInstallApp(SerialNo, "io.appium.uiautomator2.server.test");
							ADBHelperCCK.UnInstallApp(SerialNo, "io.appium.uiautomator2.server");
							ChangeGridViewDevice(SerialNo, "Successful", Color.Green);
						}).Start();
					}
				}
			}).Start();
		}

		private void button3_Click(object sender, EventArgs e)
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
			MessageBox.Show("Đã tắt hết ADB & Node Load lại Devices");
		}

		private void btnedXposed_Click(object sender, EventArgs e)
		{
			int systemPort = 8201;
			int localPort = 4723;
			ThreadPool.SetMinThreads(100, 4);
			ServicePointManager.DefaultConnectionLimit = 500;
			new Task(delegate
			{
				foreach (DataGridViewRow item in (IEnumerable)dataGridViewPhone.Rows)
				{
					if (item.Cells["Phone"].Value != null)
					{
						string p_DeviceId = item.Cells["Phone"].Value.ToString();
						new Task(delegate
						{
							ADBHelperCCK.EnableModules(p_DeviceId);
							int num = Utils.Convert2Int(ADBHelperCCK.GetAndroidVersion(p_DeviceId));
							if (num <= 8)
							{
								ADBHelperCCK.StopApp(p_DeviceId, "de.robv.android.xposed.installer");
							}
							else
							{
								ADBHelperCCK.StopApp(p_DeviceId, "org.meowcat.edxposed.manager");
							}
							try
							{
								ChangeDevice(p_DeviceId.ToString());
							}
							catch (Exception)
							{
							}
							if (num <= 8)
							{
								ADBHelperCCK.StopApp(p_DeviceId, "de.robv.android.xposed.installer");
								Thread.Sleep(1000);
								ADBHelperCCK.OpenApp(p_DeviceId, "de.robv.android.xposed.installer");
								ADBHelperCCK.CloseApp(p_DeviceId, "de.robv.android.xposed.installer");
								ADBHelperCCK.OpenApp(p_DeviceId, "de.robv.android.xposed.installer");
							}
							else
							{
								ADBHelperCCK.StopApp(p_DeviceId, "org.meowcat.edxposed.manager");
								Thread.Sleep(1000);
								ADBHelperCCK.OpenApp(p_DeviceId, "org.meowcat.edxposed.manager");
								ADBHelperCCK.CloseApp(p_DeviceId, "org.meowcat.edxposed.manager");
								ADBHelperCCK.OpenApp(p_DeviceId, "org.meowcat.edxposed.manager");
							}
							DeviceEntity obj = new DeviceEntity
							{
								DeviceId = p_DeviceId,
								Version = ADBHelperCCK.GetAndroidVersionNumber(p_DeviceId).ToString(),
								Name = p_DeviceId
							};
							int num2 = systemPort;
							systemPort = num2 + 1;
							obj.SystemPort = num2;
							num2 = localPort;
							localPort = num2 + 1;
							obj.Port = num2;
							DeviceEntity deviceEntity = obj;
							CCKDriver cCKDriver = new CCKDriver(deviceEntity.DeviceId);
							CCKNode cCKNode = cCKDriver.FindElement("//*[@resource-id='org.meowcat.edxposed.manager:id/ic_manufacturer']");
							if (cCKNode == null)
							{
								if (num <= 8)
								{
									ADBHelperCCK.StopApp(p_DeviceId, "de.robv.android.xposed.installer");
									Thread.Sleep(1000);
									ADBHelperCCK.OpenApp(p_DeviceId, "de.robv.android.xposed.installer");
								}
								else
								{
									ADBHelperCCK.StopApp(p_DeviceId, "org.meowcat.edxposed.manager");
									Thread.Sleep(1000);
									ADBHelperCCK.OpenApp(p_DeviceId, "org.meowcat.edxposed.manager");
								}
							}
							Point screenResolution = ADBHelperCCK.GetScreenResolution(p_DeviceId);
							ADBHelperCCK.Swipe(p_DeviceId, screenResolution.X / 2, screenResolution.Y * 3 / 4, screenResolution.X / 2, screenResolution.Y / 4);
							cCKNode = cCKDriver.FindElement("//*[@resource-id='org.meowcat.edxposed.manager:id/ic_manufacturer']");
							if (cCKNode != null)
							{
								string msg = cCKNode.Text;
								ChangeGridViewDevice(deviceEntity.DeviceId, msg, Color.Green);
							}
							else
							{
								ChangeGridViewDevice(deviceEntity.DeviceId, "EdXposed Manager Error", Color.Red);
							}
						}).Start();
					}
				}
			}).Start();
		}

		private void TestChangeDevice(string p_DeviceId)
		{
			new Task(delegate
			{
				ADBHelperCCK.EnableModules(p_DeviceId);
				int num = Utils.Convert2Int(ADBHelperCCK.GetAndroidVersion(p_DeviceId));
				if (num <= 8)
				{
					ADBHelperCCK.StopApp(p_DeviceId, "de.robv.android.xposed.installer");
				}
				else
				{
					ADBHelperCCK.StopApp(p_DeviceId, "org.meowcat.edxposed.manager");
				}
				try
				{
					ChangeDevice(p_DeviceId.ToString());
				}
				catch (Exception)
				{
				}
				if (num > 8)
				{
					ADBHelperCCK.StopApp(p_DeviceId, "org.meowcat.edxposed.manager");
					ADBHelperCCK.OpenApp(p_DeviceId, "org.meowcat.edxposed.manager");
				}
				else
				{
					ADBHelperCCK.StopApp(p_DeviceId, "de.robv.android.xposed.installer");
					ADBHelperCCK.OpenApp(p_DeviceId, "de.robv.android.xposed.installer");
				}
			}).Start();
		}

		private void cbxAdbStop_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void btnLang_Click(object sender, EventArgs e)
		{
			frmInputControl frmInputControl2 = new frmInputControl(isSingleLine: false, "SQL Query");
			frmInputControl2.Text = "SQL Query";
			frmInputControl2.ShowDialog();
			SQLiteUtils sQLiteUtils = new SQLiteUtils();
			if (frmInputControl2.Result != "" && MessageBox.Show("Are you sure you want to do it?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				try
				{
					sQLiteUtils.ExecuteQuery(frmInputControl2.Result);
					MessageBox.Show("OK");
					Close();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
		}

		private void rbt4G_CheckedChanged(object sender, EventArgs e)
		{
			if (MessageBox.Show("Do you want to change the Network on your phone to 4G", "", MessageBoxButtons.YesNo) != DialogResult.Yes)
			{
				return;
			}
			List<string> lst = ADBHelperCCK.GetListSerialDevice();
			new Task(delegate
			{
				foreach (string item in lst)
				{
					ADBHelperCCK.ExecuteCMD(item, "shell \"svc wifi disable\"");
					ADBHelperCCK.TurnOffAirplane(item);
				}
			}).Start();
		}

		private void rbtWifi_CheckedChanged(object sender, EventArgs e)
		{
			if (MessageBox.Show("Do you want to change the Network on your phone to WIFI", "", MessageBoxButtons.YesNo) != DialogResult.Yes)
			{
				return;
			}
			List<string> lst = ADBHelperCCK.GetListSerialDevice();
			new Task(delegate
			{
				foreach (string item in lst)
				{
					ADBHelperCCK.TurnOffAirplane(item);
					ADBHelperCCK.ExecuteCMD(item, "shell \"svc wifi enable\"");
				}
			}).Start();
		}

		private void cbxChangeDevice_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void numLine_ValueChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.PHONE_WIDTH, numLine.Value.ToString());
		}

		private void button4_Click(object sender, EventArgs e)
		{
			string arg = "CCK";
			List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
			int num = 1;
			foreach (string item in listSerialDevice)
			{
				new SQLiteUtils().ExecuteQuery(string.Format("Delete from tblDevices where deviceId='{1}'; Insert Into  tblDevices(name, deviceId) values ('{0} - {2}', '{1}')", arg, item, (num < 10) ? ("0" + num).ToString() : num.ToString()));
				num++;
			}
			btnLoadDevices_Click(null, null);
		}

		private void btnCheckS7_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
				try
				{
					SetupPhoneAndroid8S7(listSerialDevice);
				}
				catch
				{
				}
			}).Start();
		}

		private void CheckMagick(string p_DeviceId)
		{
			ADBHelperCCK.OpenApp(p_DeviceId, "com.topjohnwu.magisk");
		}

		private void CheckXposeAndroid8(string p_DeviceId)
		{
			ADBHelperCCK.CloseApp(p_DeviceId, "de.robv.android.xposed.installer");
			ADBHelperCCK.OpenApp(p_DeviceId, "de.robv.android.xposed.installer");
		}

		private void btnAir_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
				try
				{
					foreach (string item in listSerialDevice)
					{
						ADBHelperCCK.TurnOnAirplane(item, null);
						ChangeGridViewDevice(item, "Airplain On", Color.Green);
					}
				}
				catch
				{
				}
			}).Start();
		}

		private void btnAirOff_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
				try
				{
					foreach (string item in listSerialDevice)
					{
						ADBHelperCCK.TurnOffAirplane(item);
						ChangeGridViewDevice(item, "Airplain Off", Color.Green);
					}
				}
				catch
				{
				}
			}).Start();
		}

		private void cbxKillPort_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void cbxFixLogin_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmInputControl frmInputControl2 = new frmInputControl(isSingleLine: true, "Input Wifi UserName and Password - UserName|Password", "Nhập mật khẩu | Password");
			if (File.Exists(CaChuaConstant.WIFI))
			{
				frmInputControl2.SetText(Utils.ReadTextFile(CaChuaConstant.WIFI));
			}
			frmInputControl2.ShowDialog();
			File.WriteAllText(CaChuaConstant.WIFI, frmInputControl2.Result);
		}

		private void button5_Click(object sender, EventArgs e)
		{
			frmInputControl c = new frmInputControl(isSingleLine: true, "Input Wifi UserName and Password - UserName|Password", "Nhập mật khẩu | Password");
			if (File.Exists(CaChuaConstant.WIFI))
			{
				c.SetText(Utils.ReadTextFile(CaChuaConstant.WIFI));
			}
			c.ShowDialog();
			File.WriteAllText(CaChuaConstant.WIFI, c.Result);
			string apk = "adb-join-wifi.apk";
			if (!File.Exists("Devices\\" + apk))
			{
				frmDownload frmDownload2 = new frmDownload();
				frmDownload2.Download("https://cck.vn/Download/Utils/" + apk + ".rar", Application.StartupPath.TrimEnd('\\') + "\\Devices\\" + apk);
				frmDownload2.ShowDialog();
			}
			new Task(delegate
			{
				List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
				try
				{
					string[] acc = c.Result.Split('|');
					foreach (string p_DeviceId in listSerialDevice)
					{
						new Task(delegate
						{
							ChangeGridViewDevice(p_DeviceId, "Connecting...", Color.Green);
							if (!ADBHelperCCK.IsInstallApp(p_DeviceId, "com.steinwurf.adbjoinwifi"))
							{
								ADBHelperCCK.InstallApp(p_DeviceId, Application.StartupPath.TrimEnd('\\') + "\\Devices\\" + apk);
							}
							ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell svc wifi enable");
							ADBHelperCCK.ClearProxy(p_DeviceId);
							if (acc != null && acc.Length == 1)
							{
								ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell am start -n com.steinwurf.adbjoinwifi/com.steinwurf.adbjoinwifi.MainActivity -e ssid \"\\\"" + acc[0] + "\\\"\"");
							}
							else if (acc != null && acc.Length == 2)
							{
								ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell am start -n com.steinwurf.adbjoinwifi/com.steinwurf.adbjoinwifi.MainActivity -e ssid \"\\\"" + acc[0] + "\\\"\" -e password_type WPA -e password \\\"\"" + acc[1] + "\\\"\"");
							}
							ChangeGridViewDevice(p_DeviceId, "Done", Color.Green);
						}).Start();
					}
				}
				catch
				{
				}
			}).Start();
		}

		private void cbxRomCook_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void btnRemove_Click(object sender, EventArgs e)
		{
			ThreadPool.SetMinThreads(100, 4);
			ServicePointManager.DefaultConnectionLimit = 500;
			new Task(delegate
			{
				ADBHelperCCK.GetListSerialDevice();
				foreach (DataGridViewRow row in dataGridViewPhone.SelectedRows)
				{
					new Task(delegate
					{
						string text = row.Cells["Phone"].Value.ToString();
						ChangeGridViewDevice(text, "Tắt tiếng", Color.Green);
						ADBHelperCCK.SetVolumnMute(text);
						ChangeGridViewDevice(text, "Done", Color.Green);
					}).Start();
				}
			}).Start();
		}

		private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://bit.ly/3KJRaS1");
		}

		private void button1_Click_1(object sender, EventArgs e)
		{
			frmInputControl frmInputControl2 = new frmInputControl(isSingleLine: true, "Open App");
			frmInputControl2.ShowDialog();
			string appName = frmInputControl2.Result;
			if (!(appName != ""))
			{
				return;
			}
			new Task(delegate
			{
				List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
				foreach (string SerialNo in listSerialDevice)
				{
					new Task(delegate
					{
						ADBHelperCCK.TurnOnScreen(SerialNo);
						ADBHelperCCK.OpenApp(SerialNo, appName);
					}).Start();
				}
			}).Start();
		}

		private void btnportrait_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
				foreach (string SerialNo in listSerialDevice)
				{
					new Task(delegate
					{
						ADBHelperCCK.SetPortrait(SerialNo);
						ChangeGridViewDevice(SerialNo, "Done", Color.Green);
					}).Start();
				}
			}).Start();
		}

		private void btnFindNetwork_Click(object sender, EventArgs e)
		{
		}

		private void cbxWifiMode_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.WIFI_MODE, cbxWifiMode.Checked.ToString());
		}

		private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			frmInputControl frmInputControl2 = new frmInputControl(isSingleLine: false, "Danh sách IP:Port của các điện thoại");
			if (File.Exists(CaChuaConstant.FIX_DEVICE))
			{
				frmInputControl2.SetText(Utils.ReadTextFile(CaChuaConstant.FIX_DEVICE));
			}
			frmInputControl2.ShowDialog();
			File.WriteAllText(CaChuaConstant.FIX_DEVICE, frmInputControl2.Result);
			string[] array = frmInputControl2.Result.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
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

		private void button2_Click_1(object sender, EventArgs e)
		{
			new Task(delegate
			{
				List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
				foreach (string SerialNo in listSerialDevice)
				{
					new Task(delegate
					{
						ChangeGridViewDevice(SerialNo, "Start Clear", Color.Green);
						ADBHelperCCK.ClearProxy(SerialNo, delete: true);
						ChangeGridViewDevice(SerialNo, "Done", Color.Green);
					}).Start();
				}
			}).Start();
		}

		private void button6_Click(object sender, EventArgs e)
		{
			Queue<string> lstDevice = new Queue<string>();
			foreach (DataGridViewRow item in (IEnumerable)dataGridViewPhone.Rows)
			{
				if (item.Cells["Phone"].Value != null)
				{
					lstDevice.Enqueue(item.Cells["Phone"].Value.ToString());
				}
			}
			while (lstDevice.Count > 0)
			{
				new Task(delegate
				{
					if (lstDevice.Count != 0)
					{
						string text = lstDevice.Dequeue();
						string text2 = text;
						ChangeGridViewDevice(text, "Removing ...", Color.Green);
						ADBHelperCCK.ExecuteCMD(text2, " uninstall com.cck.support");
						if (!File.Exists(Application.StartupPath + "\\Devices\\cck_device_changer3.0.3.apk"))
						{
							ChangeGridViewDevice(text, "Cài Change App", Color.Green);
							if (!Directory.Exists("Devices"))
							{
								Directory.CreateDirectory("Devices");
							}
							try
							{
								ADBHelperCCK.ExecuteCMD(text2, " uninstall com.cck.support");
								if (!File.Exists(Application.StartupPath + "\\Devices\\cck_device_changer3.0.3.apk"))
								{
									new WebClient().DownloadFile("https://cck.vn/Download/Utils/cck_device_changer3.0.3.apk.rar", Application.StartupPath + "\\Devices\\cck_device_changer3.0.3.apk");
								}
							}
							catch (Exception ex)
							{
								Utils.CCKLog("TaskList", ex.Message, "SetupPhone");
							}
						}
						ADBHelperCCK.GetAndroidVersionNumber(text);
						if (!ADBHelperCCK.IsInstallApp(text2, "com.cck.support"))
						{
							ChangeGridViewDevice(text, "Starting ...", Color.Green);
							if (File.Exists(Application.StartupPath + "\\Devices\\cck_device_changer3.0.3.apk"))
							{
								ADBHelperCCK.ExecuteCMD(text2, " install \"" + Application.StartupPath + "\\Devices\\cck_device_changer3.0.3.apk\"");
								ADBHelperCCK.OpenApp(text, "com.cck.support");
								Thread.Sleep(1000);
								ADBHelperCCK.ExecuteCMD(text, "shell pm grant com.cck.support android.permission.READ_EXTERNAL_STORAGE");
								ADBHelperCCK.ExecuteCMD(text, "shell pm grant com.cck.support android.permission.WRITE_EXTERNAL_STORAGE");
								ADBHelperCCK.ExecuteCMD(text, "shell pm grant com.cck.support android.permission.READ_CONTACTS");
								ADBHelperCCK.ExecuteCMD(text, "shell pm grant com.cck.support android.permission.WRITE_CONTACTS");
								ADBHelperCCK.ExecuteCMD(text, "shell am start -a android.settings.APPLICATION_DETAILS_SETTINGS -d com.cck.support");
							}
							ChangeGridViewDevice(text, "Done", Color.Green);
						}
					}
				}).Start();
			}
		}

		private void btnRename_Click(object sender, EventArgs e)
		{
			new SQLiteUtils().ExecuteQuery(string.Format("Delete from tblDevices where deviceId='{1}'; Insert Into  tblDevices(name, deviceId) values ('{0}', '{1}')", txtName.Text.Trim(), txtId.Text));
			btnLoadDevices_Click(null, null);
		}

		private void nudCheckLive_ValueChanged(object sender, EventArgs e)
		{
		}

		private void ChangeBackground(string bgFile, string p_DeviceId, int i)
		{
			new Task(delegate
			{
				if (!File.Exists(Application.StartupPath + "\\Devices\\cck_device_changer_3.apk"))
				{
					ChangeGridViewDevice(p_DeviceId, "Cài Change App", Color.Green);
					if (!Directory.Exists("Devices"))
					{
						Directory.CreateDirectory("Devices");
					}
					try
					{
						ADBHelperCCK.ExecuteCMD(p_DeviceId, " uninstall com.cck.support");
						if (!File.Exists(Application.StartupPath + "\\Devices\\cck_device_changer_3.apk"))
						{
							new WebClient().DownloadFile("https://cck.vn/Download/Utils/cck_device_changer_3.apk.rar", Application.StartupPath + "\\Devices\\cck_device_changer_3.apk");
						}
					}
					catch (Exception ex)
					{
						Utils.CCKLog("TaskList", ex.Message, "SetupPhone");
					}
				}
				ADBHelperCCK.GetAndroidVersionNumber(p_DeviceId);
				if (!ADBHelperCCK.IsInstallApp(p_DeviceId, "com.cck.support"))
				{
					ChangeGridViewDevice(p_DeviceId, "Starting ...", Color.Green);
					if (File.Exists(Application.StartupPath + "\\Devices\\cck_device_changer_3.apk"))
					{
						ADBHelperCCK.ExecuteCMD(p_DeviceId, " install \"" + Application.StartupPath + "\\Devices\\cck_device_changer_3.apk\"");
					}
				}
				string text = RenderPicture(i + 1, bgFile);
				ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell  pm grant com.cck.support android.permission.READ_EXTERNAL_STORAGE");
				ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell  pm grant com.cck.support android.permission.WRITE_EXTERNAL_STORAGE");
				ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell  pm grant com.cck.support android.permission.READ_CONTACTS");
				ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell  pm grant com.cck.support android.permission.WRITE_CONTACTS");
				ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell rm -f /sdcard/cckbg.png");
				ADBHelperCCK.PushInfoFile(p_DeviceId, string.Format("\"{0}\" \"/sdcard/cckbg.png\"", text.Replace("Devices\\cckbg.png", text)));
				Thread.Sleep(1000);
				ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell am startservice com.cck.support/.CckServiceBackground");
				Thread.Sleep(3000);
				ADBHelperCCK.ExecuteCMD(p_DeviceId, "shell rm -r \"/sdcard/cckbg.png\"");
				ADBHelperCCK.BackToHome(p_DeviceId);
				ChangeGridViewDevice(p_DeviceId, "OK", Color.Green);
			}).Start();
		}

		private void btnCCK_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				string text = Application.StartupPath + "\\Devices\\cckbg.png";
				if (File.Exists(text))
				{
					File.Delete(text);
				}
				new WebClient().DownloadFile("https://cck.vn/Download/Update/cckbg.png", text);
				List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
				for (int i = 0; i < listSerialDevice.Count; i++)
				{
					ChangeGridViewDevice(listSerialDevice[i], "Start ...", Color.Green);
					ChangeBackground(text, listSerialDevice[i], i);
				}
			}).Start();
		}

		private string RenderPicture(int sequenceNumber, string imagePath)
		{
			try
			{
				if (File.Exists(imagePath))
				{
					using (Bitmap bitmap = new Bitmap(imagePath))
					{
						using (Graphics graphics = Graphics.FromImage(bitmap))
						{
							Font font = new Font("Arial", 300f);
							SolidBrush brush = new SolidBrush(Color.White);
							StringFormat format = new StringFormat
							{
								Alignment = StringAlignment.Center,
								LineAlignment = StringAlignment.Center
							};
							float num = bitmap.Width / 2;
							float num2 = 400f;
							graphics.DrawString(sequenceNumber.ToString(), font, brush, num, num2, format);
						}
						if (File.Exists($"Devices\\cckbg{sequenceNumber}.png"))
						{
							File.Delete($"Devices\\cckbg{sequenceNumber}.png");
						}
						bitmap.Save($"Devices\\cckbg{sequenceNumber}.png", ImageFormat.Png);
						return $"Devices\\cckbg{sequenceNumber}.png";
					}
				}
			}
			catch
			{
			}
			return "";
		}

		private void button8_Click(object sender, EventArgs e)
		{
			frmInputControl frmInputControl2 = new frmInputControl(isSingleLine: false, "Email List");
			frmInputControl2.Text = "Email List";
			if (File.Exists(Application.StartupPath + "\\Config\\cck_log_email.txt"))
			{
				frmInputControl2.SetText(Utils.ReadTextFile(Application.StartupPath + "\\Config\\cck_log_email.txt"));
			}
			frmInputControl2.ShowDialog();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			List<string> list = frmInputControl2.Result.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
			foreach (string item in list)
			{
				if (item != "")
				{
					string key = item.Split('|')[0].ToLower();
					if (!dictionary.ContainsKey(key))
					{
						dictionary.Add(key, item);
					}
				}
			}
			List<string> list2 = new List<string>();
			DataTable dataTable = new SQLiteUtils().ExecuteQuery("Select distinct Email from Account");
			if (dataTable != null)
			{
				foreach (DataRow row in dataTable.Rows)
				{
					string key2 = row[0].ToString().Trim().ToLower();
					if (dictionary.ContainsKey(key2))
					{
						dictionary.Remove(key2);
					}
				}
			}
			foreach (KeyValuePair<string, string> item2 in dictionary)
			{
				list2.Add(item2.Value);
			}
			File.WriteAllLines("tmp.txt", list2);
			Thread.Sleep(1000);
			Process.Start("tmp.txt");
		}

		private void button9_Click(object sender, EventArgs e)
		{
			frmInputControl frmInputControl2 = new frmInputControl(isSingleLine: true, "Link Video");
			frmInputControl2.Text = "Link Video";
			frmInputControl2.ShowDialog();
			string link = frmInputControl2.Result;
			if (!(link != ""))
			{
				return;
			}
			new Task(delegate
			{
				List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
				foreach (string SerialNo in listSerialDevice)
				{
					new Task(delegate
					{
						ADBHelperCCK.OpenLink(SerialNo, link);
					}).Start();
				}
			}).Start();
		}

		private void button10_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				List<string> list = new List<string>();
				if (MessageBox.Show("Bạn có muốn xóa dữ liệu đã đăng nhập không.", "Máy nào được chọn sẽ bị xóa trắng tài khoản Tiktok", MessageBoxButtons.YesNo) != DialogResult.No)
				{
					if (dataGridViewPhone.SelectedRows.Count == 0)
					{
						MessageBox.Show("Bạn chưa chọn máy để xóa dữ liệu");
					}
					else
					{
						foreach (DataGridViewRow selectedRow in dataGridViewPhone.SelectedRows)
						{
							if (selectedRow.Cells["Phone"].Value != null)
							{
								list.Add(selectedRow.Cells["Phone"].Value.ToString());
							}
						}
						foreach (string SerialNo in list)
						{
							new Task(delegate
							{
								ADBHelperCCK.ClearAppData(SerialNo, CaChuaConstant.PACKAGE_NAME);
								ChangeGridViewDevice(SerialNo, "Xóa dữ liệu xong", Color.Green);
							}).Start();
						}
					}
				}
			}).Start();
		}

		private void button11_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				string path = Application.StartupPath + "\\Authentication";
				string[] directories = Directory.GetDirectories(path);
				try
				{
					if (File.Exists("Log\\wrongaccountn.txt"))
					{
						File.Delete("Log\\wrongaccountn.txt");
					}
				}
				catch
				{
				}
				Cursor.Current = Cursors.WaitCursor;
				string[] array = directories;
				foreach (string text in array)
				{
					try
					{
						string text2 = text.Substring(text.LastIndexOf("\\") + 1);
						string text3 = Application.StartupPath + $"\\Authentication\\{text2}\\com.ss.android.ugc.trill\\shared_prefs\\aweme_user.xml";
						if (File.Exists(text3))
						{
							XmlDocument xmlDocument = new XmlDocument();
							xmlDocument.LoadXml(Utils.ReadTextFile(text3));
							XmlNode xmlNode = xmlDocument.SelectSingleNode("//*[@name=\"current_foreground_uid\"]");
							if (xmlNode != null)
							{
								string innerText = xmlNode.InnerText;
								XmlNode xmlNode2 = xmlDocument.SelectSingleNode($"//*[@name=\"{innerText}_account_user_info\"]");
								_ = xmlNode2.InnerText;
								xmlNode2 = xmlDocument.SelectSingleNode($"//*[@name=\"{innerText}_significant_user_info\"]");
								string innerText2 = xmlNode2.InnerText;
								object obj2 = new JavaScriptSerializer().DeserializeObject(innerText2);
								if ((dynamic)obj2 != null && ((dynamic)obj2).ContainsKey("unique_id"))
								{
									object obj3 = ((dynamic)obj2)["unique_id"].ToString();
									if ((dynamic)obj3 != text2)
									{
										Utils.ClearFolder(Application.StartupPath + $"\\Authentication\\{xmlNode}");
										Utils.CCKLog("Log\\wrongaccountn.txt", (dynamic)obj3, text2);
									}
								}
							}
						}
					}
					catch
					{
					}
				}
				Cursor.Current = Cursors.Default;
				if (File.Exists("Log\\wrongaccountn.txt"))
				{
					Process.Start("Log\\wrongaccountn.txt");
				}
				else
				{
					MessageBox.Show("Xong! không sao đâu");
				}
			}).Start();
		}

		private void button12_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
				frmInputControl frmInputControl2 = new frmInputControl("ADB Command");
				frmInputControl2.ShowDialog();
				string command = frmInputControl2.Result.Trim();
				foreach (string p_DeviceId in listSerialDevice)
				{
					new Task(delegate
					{
						try
						{
							if (command != "")
							{
								ChangeGridViewDevice(p_DeviceId, "Start", Color.Green);
								if (command.ToLower().StartsWith("adb"))
								{
									command = command.Substring(4);
									ADBHelperCCK.ExecuteCMD(p_DeviceId, command.Trim());
									ChangeGridViewDevice(p_DeviceId, "Done", Color.Green);
								}
							}
						}
						catch
						{
						}
					}).Start();
				}
			}).Start();
		}

		private void button13_Click(object sender, EventArgs e)
		{
			frmInputControl frmInputControl2 = new frmInputControl(isSingleLine: false, "Danh sach ID Phone");
			frmInputControl2.Text = "Danh sach ID Phone";
			if (File.Exists(CaChuaConstant.DEVICE_LIST_FIX))
			{
				frmInputControl2.SetText(Utils.ReadTextFile(CaChuaConstant.DEVICE_LIST_FIX));
			}
			frmInputControl2.ShowDialog();
			string result = frmInputControl2.Result;
			File.WriteAllText(CaChuaConstant.DEVICE_LIST_FIX, result);
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.STARTPORT, numericUpDown1.Value.ToString());
		}

		private void button14_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				if (!File.Exists("Config\\off_mode_charge.rc"))
				{
					string contents = "# /system/etc/init/off_mode_charge.rc\r\n                                              on charger\r\n                                                  setprop sys.powerctl reboot,leaving-off-mode-charging";
					File.WriteAllText("Config\\off_mode_charge.rc", contents);
				}
				List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
				foreach (string SerialNo in listSerialDevice)
				{
					new Task(delegate
					{
						ADBHelperCCK.ExecuteCMD(SerialNo, "shell dumpsys battery set level 100");
						string text = ADBHelperCCK.ExecuteCMD(SerialNo, " shell mount -o rw,remount /");
						text = ADBHelperCCK.ExecuteCMD(SerialNo, " shell mount -o rw,remount /system");
						text += ADBHelperCCK.ExecuteCMD(SerialNo, " push \"" + Application.StartupPath + "\\Config\\off_mode_charge.rc\" /system/etc/init/");
						text = ADBHelperCCK.ExecuteCMD(SerialNo, " ls /system/etc/init/");
						bool flag;
						if (flag = text.Contains("off_mode_charge.rc"))
						{
						}
						ChangeGridViewDevice(SerialNo, flag ? "Successful" : "Error", flag ? Color.Green : Color.Red);
					}).Start();
				}
			}).Start();
		}

		private void rbtRoot_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.PHONE_MODE_DATA, new JavaScriptSerializer().Serialize(PhoneMode.Root));
		}

		private void rbtNoroot_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.PHONE_MODE_DATA, new JavaScriptSerializer().Serialize(PhoneMode.NonRoot));
		}

		private void cbxLogVideo_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.LOG_VIDEO, cbxLogVideo.Checked.ToString());
		}

		private void button7_Click(object sender, EventArgs e)
		{
			ADBHelperCCK.ExecuteCMDReconnectOffline();
		}

		private void frmDevices_FormClosing(object sender, FormClosingEventArgs e)
		{
			isRunning = false;
		}

		private void btnTimeZone_Click(object sender, EventArgs e)
		{
			string text = "ADBCLanguage.apk";
			if (!File.Exists("Devices\\" + text))
			{
				frmDownload frmDownload2 = new frmDownload();
				frmDownload2.Download("https://cck.vn/Download/Utils/" + text + ".rar", Application.StartupPath + "\\Devices\\" + text);
				frmDownload2.ShowDialog();
				if (frmDownload2.DownloadCompleted)
				{
					Thread.Sleep(1000);
				}
			}
			List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
			foreach (string device in listSerialDevice)
			{
				new Task(delegate
				{
					ChangeGridViewDevice(device, "Start", Color.Green);
					ADBHelperCCK.InstallApp(device, Application.StartupPath + "\\Devices\\ADBCLanguage.apk");
					if (ADBHelperCCK.IsInstallApp(device, "net.sanapeli.adbchangelanguage"))
					{
						ADBHelperCCK.ExecuteCMD(device, "shell pm grant net.sanapeli.adbchangelanguage android.permission.CHANGE_CONFIGURATION");
						ADBHelperCCK.ExecuteCMD(device, "shell am start -n net.sanapeli.adbchangelanguage/.AdbChangeLanguage -e language en");
					}
					ChangeGridViewDevice(device, "Done", Color.Green);
				}).Start();
			}
		}

		private void dataGridViewPhone_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
		{
			if (dataGridViewPhone.Rows[e.RowIndex].Cells["Status"].Value != null && dataGridViewPhone.Rows[e.RowIndex].Cells["Status"].Value.ToString() != "Live")
			{
				dataGridViewPhone.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Red;
			}
			else
			{
				dataGridViewPhone.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Green;
			}
		}

		private void btnshutdowna_Click(object sender, EventArgs e)
		{
			ThreadPool.SetMinThreads(100, 4);
			ServicePointManager.DefaultConnectionLimit = 500;
			new Task(delegate
			{
				ADBHelperCCK.GetListSerialDevice();
				foreach (DataGridViewRow row in dataGridViewPhone.SelectedRows)
				{
					new Task(delegate
					{
						string text = row.Cells["Phone"].Value.ToString();
						ChangeGridViewDevice(text, "Starting...", Color.Green);
						ADBHelperCCK.ExecuteCMD(text, "shell reboot -p");
						ChangeGridViewDevice(text, "Done", Color.Green);
					}).Start();
				}
			}).Start();
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.CLEARPROXY, checkBox1.Checked.ToString());
		}

		private void cbxSchedule_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.SCHEDULE, cbxSchedule.Checked.ToString());
		}

		private void rbtTWrp_CheckedChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.PHONE_MODE_DATA, new JavaScriptSerializer().Serialize(PhoneMode.TWRP));
		}

		private void button1_Click_2(object sender, EventArgs e)
		{
			ChangeTimeZone changeTimeZone = new ChangeTimeZone();
			changeTimeZone.StartPosition = FormStartPosition.CenterScreen;
			changeTimeZone.ShowDialog();
			if (string.IsNullOrEmpty(Utils.ReadTextFile(CaChuaConstant.TIME_ZONE)))
			{
				return;
			}
			List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
			foreach (string device in listSerialDevice)
			{
				new Task(delegate
				{
					ChangeGridViewDevice(device, "Start", Color.Green);
					ADBHelperCCK.StopApp(device, "android.settings.DATE_SETTINGS");
					ADBHelperCCK.ExecuteCMD(device, " shell am start -a android.settings.DATE_SETTINGS");
					Thread.Sleep(1000);
					CCKDriver cCKDriver = new CCKDriver(device);
					_ = cCKDriver.PageSource;
					List<CCKNode> list = cCKDriver.FindElements("//*[@class=\"android.widget.Switch\" and @checked=\"true\"]");
					if (list != null && list.Count > 0)
					{
						foreach (CCKNode item in list)
						{
							item.Click();
							Thread.Sleep(500);
						}
					}
					CCKNode cCKNode = cCKDriver.FindElement("//*[@text='Select time zone' or @content-desc='Select time zone' or @text='Time zone' or @content-desc='Time zone']");
					if (cCKNode != null)
					{
						cCKNode.Click();
						Thread.Sleep(1000);
					}
					cCKNode = cCKDriver.FindElement("//*[@text='Region' or @content-desc='Region']");
					if (cCKNode != null)
					{
						cCKNode.Click();
						Thread.Sleep(1000);
					}
					cCKNode = cCKDriver.FindElement("//*[@text='Search region' or @content-desc='Search region' or @text='Search regions' or @content-desc='Search regions']");
					if (cCKNode != null)
					{
						cCKNode.Click();
						cCKNode.CCKSendKeys(Utils.ReadTextFile(CaChuaConstant.TIME_ZONE).Split('|')[0]);
						Thread.Sleep(1000);
						List<CCKNode> list2 = cCKDriver.FindElements(string.Format("//*[@text='{0}' or @content-desc='{0}']", Utils.ReadTextFile(CaChuaConstant.TIME_ZONE).Split('|')[0]));
						if (list2 != null && list2.Count > 1)
						{
							list2[list2.Count - 1].Click();
							Thread.Sleep(1000);
							_ = cCKDriver.PageSource;
							string[] array = Utils.ReadTextFile(CaChuaConstant.TIME_ZONE).Split('|');
							if (array != null && array.Length > 1)
							{
								cCKNode = cCKDriver.FindElement(string.Format("//*[contains(@text,'{0}') or contains(@content-desc,'{0}')]", Utils.ReadTextFile(CaChuaConstant.TIME_ZONE).Split('|')[1]));
								if (cCKNode != null)
								{
									cCKNode.Click();
									Thread.Sleep(1000);
								}
							}
						}
					}
					ADBHelperCCK.StopApp(device, "android.settings.DATE_SETTINGS");
					ChangeGridViewDevice(device, "Done", Color.Green);
				}).Start();
			}
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
			btnCheck = new System.Windows.Forms.Button();
			btnLoadDevices = new System.Windows.Forms.Button();
			btnBri = new System.Windows.Forms.Button();
			btnturnoff = new System.Windows.Forms.Button();
			btnTurnon = new System.Windows.Forms.Button();
			cbxVolumn = new System.Windows.Forms.CheckBox();
			nudTimeout = new System.Windows.Forms.NumericUpDown();
			label8 = new System.Windows.Forms.Label();
			nudBright = new System.Windows.Forms.NumericUpDown();
			btnSetup = new System.Windows.Forms.Button();
			btnSetupApk = new System.Windows.Forms.Button();
			btnAppium = new System.Windows.Forms.Button();
			dataGridViewPhone = new System.Windows.Forms.DataGridView();
			button4 = new System.Windows.Forms.Button();
			btnClean = new System.Windows.Forms.Button();
			btnShowPhone = new System.Windows.Forms.Button();
			numLine = new System.Windows.Forms.NumericUpDown();
			label4 = new System.Windows.Forms.Label();
			btnReboot = new System.Windows.Forms.Button();
			nudPhoneDelay = new System.Windows.Forms.NumericUpDown();
			label5 = new System.Windows.Forms.Label();
			button3 = new System.Windows.Forms.Button();
			btnedXposed = new System.Windows.Forms.Button();
			btnLang = new System.Windows.Forms.Button();
			btnAir = new System.Windows.Forms.Button();
			btnAirOff = new System.Windows.Forms.Button();
			cbxWifi = new System.Windows.Forms.CheckBox();
			linkLabel1 = new System.Windows.Forms.LinkLabel();
			button5 = new System.Windows.Forms.Button();
			btnshutdown = new System.Windows.Forms.Button();
			btnportrait = new System.Windows.Forms.Button();
			cbxWifiMode = new System.Windows.Forms.CheckBox();
			linkLabel3 = new System.Windows.Forms.LinkLabel();
			button2 = new System.Windows.Forms.Button();
			button6 = new System.Windows.Forms.Button();
			button7 = new System.Windows.Forms.Button();
			txtId = new System.Windows.Forms.TextBox();
			txtName = new System.Windows.Forms.TextBox();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			btnRename = new System.Windows.Forms.Button();
			groupBox1 = new System.Windows.Forms.GroupBox();
			btnCCK = new System.Windows.Forms.Button();
			button8 = new System.Windows.Forms.Button();
			button9 = new System.Windows.Forms.Button();
			button10 = new System.Windows.Forms.Button();
			button11 = new System.Windows.Forms.Button();
			button12 = new System.Windows.Forms.Button();
			button13 = new System.Windows.Forms.Button();
			numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			label3 = new System.Windows.Forms.Label();
			button14 = new System.Windows.Forms.Button();
			groupBox3 = new System.Windows.Forms.GroupBox();
			rbtTWrp = new System.Windows.Forms.RadioButton();
			rbtNoroot = new System.Windows.Forms.RadioButton();
			rbtRoot = new System.Windows.Forms.RadioButton();
			cbxLogVideo = new System.Windows.Forms.CheckBox();
			btnTimeZone = new System.Windows.Forms.Button();
			btnshutdowna = new System.Windows.Forms.Button();
			checkBox1 = new System.Windows.Forms.CheckBox();
			cbxSchedule = new System.Windows.Forms.CheckBox();
			button1 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)nudTimeout).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudBright).BeginInit();
			((System.ComponentModel.ISupportInitialize)dataGridViewPhone).BeginInit();
			((System.ComponentModel.ISupportInitialize)numLine).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudPhoneDelay).BeginInit();
			groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
			groupBox3.SuspendLayout();
			SuspendLayout();
			btnCheck.Location = new System.Drawing.Point(202, 506);
			btnCheck.Margin = new System.Windows.Forms.Padding(2);
			btnCheck.Name = "btnCheck";
			btnCheck.Size = new System.Drawing.Size(95, 25);
			btnCheck.TabIndex = 107;
			btnCheck.Text = "Check Server";
			btnCheck.UseVisualStyleBackColor = true;
			btnCheck.Click += new System.EventHandler(btnCheck_Click);
			btnLoadDevices.BackColor = System.Drawing.Color.Red;
			btnLoadDevices.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnLoadDevices.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 254);
			btnLoadDevices.ForeColor = System.Drawing.Color.White;
			btnLoadDevices.Location = new System.Drawing.Point(7, 505);
			btnLoadDevices.Margin = new System.Windows.Forms.Padding(2);
			btnLoadDevices.Name = "btnLoadDevices";
			btnLoadDevices.Size = new System.Drawing.Size(187, 25);
			btnLoadDevices.TabIndex = 105;
			btnLoadDevices.Text = "Load Devices";
			btnLoadDevices.UseVisualStyleBackColor = false;
			btnLoadDevices.Click += new System.EventHandler(btnLoadDevices_Click);
			btnBri.Location = new System.Drawing.Point(818, 136);
			btnBri.Margin = new System.Windows.Forms.Padding(2);
			btnBri.Name = "btnBri";
			btnBri.Size = new System.Drawing.Size(91, 28);
			btnBri.TabIndex = 121;
			btnBri.Text = "Chỉnh sáng";
			btnBri.UseVisualStyleBackColor = true;
			btnBri.Click += new System.EventHandler(btnBri_Click);
			btnturnoff.Location = new System.Drawing.Point(163, 543);
			btnturnoff.Margin = new System.Windows.Forms.Padding(2);
			btnturnoff.Name = "btnturnoff";
			btnturnoff.Size = new System.Drawing.Size(91, 28);
			btnturnoff.TabIndex = 119;
			btnturnoff.Text = "Turn Off Screen";
			btnturnoff.UseVisualStyleBackColor = true;
			btnturnoff.Click += new System.EventHandler(btnturnoff_Click);
			btnTurnon.Location = new System.Drawing.Point(258, 543);
			btnTurnon.Margin = new System.Windows.Forms.Padding(2);
			btnTurnon.Name = "btnTurnon";
			btnTurnon.Size = new System.Drawing.Size(95, 28);
			btnTurnon.TabIndex = 118;
			btnTurnon.Text = "Turn On Screen";
			btnTurnon.UseVisualStyleBackColor = true;
			btnTurnon.Click += new System.EventHandler(btnTurnon_Click);
			cbxVolumn.AutoSize = true;
			cbxVolumn.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			cbxVolumn.Checked = true;
			cbxVolumn.CheckState = System.Windows.Forms.CheckState.Checked;
			cbxVolumn.Location = new System.Drawing.Point(837, 291);
			cbxVolumn.Margin = new System.Windows.Forms.Padding(2);
			cbxVolumn.Name = "cbxVolumn";
			cbxVolumn.Size = new System.Drawing.Size(103, 17);
			cbxVolumn.TabIndex = 115;
			cbxVolumn.Text = "Turn Off Volumn";
			cbxVolumn.UseVisualStyleBackColor = true;
			nudTimeout.Location = new System.Drawing.Point(913, 177);
			nudTimeout.Margin = new System.Windows.Forms.Padding(2);
			nudTimeout.Name = "nudTimeout";
			nudTimeout.Size = new System.Drawing.Size(55, 20);
			nudTimeout.TabIndex = 113;
			nudTimeout.Value = new decimal(new int[4] { 60, 0, 0, 0 });
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(810, 181);
			label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(92, 13);
			label8.TabIndex = 112;
			label8.Text = "Screen timeout (s)";
			nudBright.Location = new System.Drawing.Point(913, 140);
			nudBright.Margin = new System.Windows.Forms.Padding(2);
			nudBright.Name = "nudBright";
			nudBright.Size = new System.Drawing.Size(55, 20);
			nudBright.TabIndex = 111;
			nudBright.Value = new decimal(new int[4] { 30, 0, 0, 0 });
			btnSetup.BackColor = System.Drawing.Color.Red;
			btnSetup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnSetup.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 254);
			btnSetup.ForeColor = System.Drawing.Color.White;
			btnSetup.Location = new System.Drawing.Point(10, 609);
			btnSetup.Margin = new System.Windows.Forms.Padding(2);
			btnSetup.Name = "btnSetup";
			btnSetup.Size = new System.Drawing.Size(159, 28);
			btnSetup.TabIndex = 108;
			btnSetup.Text = "Start Setup New Phone";
			btnSetup.UseVisualStyleBackColor = false;
			btnSetup.Click += new System.EventHandler(btnSetup_Click);
			btnSetupApk.Location = new System.Drawing.Point(524, 543);
			btnSetupApk.Margin = new System.Windows.Forms.Padding(2);
			btnSetupApk.Name = "btnSetupApk";
			btnSetupApk.Size = new System.Drawing.Size(68, 28);
			btnSetupApk.TabIndex = 120;
			btnSetupApk.Text = "Setup APK";
			btnSetupApk.UseVisualStyleBackColor = true;
			btnSetupApk.Click += new System.EventHandler(btnSetupApk_Click);
			btnAppium.Location = new System.Drawing.Point(438, 543);
			btnAppium.Margin = new System.Windows.Forms.Padding(2);
			btnAppium.Name = "btnAppium";
			btnAppium.Size = new System.Drawing.Size(82, 28);
			btnAppium.TabIndex = 123;
			btnAppium.Text = "Remove App";
			btnAppium.UseVisualStyleBackColor = true;
			btnAppium.Click += new System.EventHandler(btnAppium_Click);
			dataGridViewPhone.AllowUserToAddRows = false;
			dataGridViewPhone.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			dataGridViewPhone.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridViewPhone.Location = new System.Drawing.Point(5, 5);
			dataGridViewPhone.Margin = new System.Windows.Forms.Padding(2);
			dataGridViewPhone.Name = "dataGridViewPhone";
			dataGridViewPhone.ReadOnly = true;
			dataGridViewPhone.RowTemplate.Height = 28;
			dataGridViewPhone.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			dataGridViewPhone.Size = new System.Drawing.Size(789, 489);
			dataGridViewPhone.TabIndex = 106;
			dataGridViewPhone.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(dataGridViewPhone_CellMouseUp);
			dataGridViewPhone.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(dataGridViewPhone_RowPrePaint);
			dataGridViewPhone.MouseClick += new System.Windows.Forms.MouseEventHandler(dataGridViewPhone_MouseClick);
			button4.Location = new System.Drawing.Point(88, 78);
			button4.Margin = new System.Windows.Forms.Padding(2);
			button4.Name = "button4";
			button4.Size = new System.Drawing.Size(79, 25);
			button4.TabIndex = 133;
			button4.Text = "Auto Index";
			button4.UseVisualStyleBackColor = true;
			button4.Click += new System.EventHandler(button4_Click);
			btnClean.Location = new System.Drawing.Point(222, 575);
			btnClean.Margin = new System.Windows.Forms.Padding(2);
			btnClean.Name = "btnClean";
			btnClean.Size = new System.Drawing.Size(92, 28);
			btnClean.TabIndex = 136;
			btnClean.Text = "Clean Sdcard";
			btnClean.UseVisualStyleBackColor = true;
			btnClean.Click += new System.EventHandler(btnClean_Click);
			btnShowPhone.BackColor = System.Drawing.Color.Red;
			btnShowPhone.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnShowPhone.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			btnShowPhone.ForeColor = System.Drawing.Color.White;
			btnShowPhone.Location = new System.Drawing.Point(303, 506);
			btnShowPhone.Margin = new System.Windows.Forms.Padding(2);
			btnShowPhone.Name = "btnShowPhone";
			btnShowPhone.Size = new System.Drawing.Size(129, 25);
			btnShowPhone.TabIndex = 137;
			btnShowPhone.Text = "Show Phone";
			btnShowPhone.UseVisualStyleBackColor = false;
			btnShowPhone.Click += new System.EventHandler(btnShowPhone_Click);
			numLine.Location = new System.Drawing.Point(524, 510);
			numLine.Margin = new System.Windows.Forms.Padding(2);
			numLine.Maximum = new decimal(new int[4] { 1000, 0, 0, 0 });
			numLine.Name = "numLine";
			numLine.Size = new System.Drawing.Size(55, 20);
			numLine.TabIndex = 138;
			numLine.Value = new decimal(new int[4] { 240, 0, 0, 0 });
			numLine.ValueChanged += new System.EventHandler(numLine_ValueChanged);
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(447, 514);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(66, 13);
			label4.TabIndex = 139;
			label4.Text = "Phone width";
			btnReboot.BackColor = System.Drawing.Color.Blue;
			btnReboot.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnReboot.ForeColor = System.Drawing.Color.White;
			btnReboot.Location = new System.Drawing.Point(596, 543);
			btnReboot.Margin = new System.Windows.Forms.Padding(2);
			btnReboot.Name = "btnReboot";
			btnReboot.Size = new System.Drawing.Size(101, 28);
			btnReboot.TabIndex = 140;
			btnReboot.Text = "Reboot All Phone";
			btnReboot.UseVisualStyleBackColor = false;
			btnReboot.Click += new System.EventHandler(btnReboot_Click);
			nudPhoneDelay.Location = new System.Drawing.Point(913, 212);
			nudPhoneDelay.Margin = new System.Windows.Forms.Padding(2);
			nudPhoneDelay.Maximum = new decimal(new int[4] { 10000, 0, 0, 0 });
			nudPhoneDelay.Name = "nudPhoneDelay";
			nudPhoneDelay.Size = new System.Drawing.Size(55, 20);
			nudPhoneDelay.TabIndex = 142;
			nudPhoneDelay.Value = new decimal(new int[4] { 10, 0, 0, 0 });
			nudPhoneDelay.ValueChanged += new System.EventHandler(nudPhoneDelay_ValueChanged);
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(834, 216);
			label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(68, 13);
			label5.TabIndex = 141;
			label5.Text = "Phone Delay";
			button3.BackColor = System.Drawing.Color.MediumVioletRed;
			button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			button3.ForeColor = System.Drawing.Color.White;
			button3.Location = new System.Drawing.Point(320, 575);
			button3.Margin = new System.Windows.Forms.Padding(2);
			button3.Name = "button3";
			button3.Size = new System.Drawing.Size(107, 28);
			button3.TabIndex = 147;
			button3.Text = "Refresh Phone";
			button3.UseVisualStyleBackColor = false;
			button3.Click += new System.EventHandler(button3_Click);
			btnedXposed.BackColor = System.Drawing.Color.DeepSkyBlue;
			btnedXposed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnedXposed.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			btnedXposed.ForeColor = System.Drawing.Color.White;
			btnedXposed.Location = new System.Drawing.Point(7, 574);
			btnedXposed.Margin = new System.Windows.Forms.Padding(2);
			btnedXposed.Name = "btnedXposed";
			btnedXposed.Size = new System.Drawing.Size(139, 28);
			btnedXposed.TabIndex = 148;
			btnedXposed.Text = "Test Change Device";
			btnedXposed.UseVisualStyleBackColor = false;
			btnedXposed.Click += new System.EventHandler(btnedXposed_Click);
			btnLang.Location = new System.Drawing.Point(150, 575);
			btnLang.Margin = new System.Windows.Forms.Padding(2);
			btnLang.Name = "btnLang";
			btnLang.Size = new System.Drawing.Size(68, 28);
			btnLang.TabIndex = 150;
			btnLang.Text = "Dev Tool";
			btnLang.UseVisualStyleBackColor = true;
			btnLang.Click += new System.EventHandler(btnLang_Click);
			btnAir.Location = new System.Drawing.Point(7, 543);
			btnAir.Margin = new System.Windows.Forms.Padding(2);
			btnAir.Name = "btnAir";
			btnAir.Size = new System.Drawing.Size(75, 28);
			btnAir.TabIndex = 156;
			btnAir.Text = "Airplane On";
			btnAir.UseVisualStyleBackColor = true;
			btnAir.Click += new System.EventHandler(btnAir_Click);
			btnAirOff.Location = new System.Drawing.Point(86, 543);
			btnAirOff.Margin = new System.Windows.Forms.Padding(2);
			btnAirOff.Name = "btnAirOff";
			btnAirOff.Size = new System.Drawing.Size(73, 28);
			btnAirOff.TabIndex = 157;
			btnAirOff.Text = "Airplane Off";
			btnAirOff.UseVisualStyleBackColor = true;
			btnAirOff.Click += new System.EventHandler(btnAirOff_Click);
			cbxWifi.AutoSize = true;
			cbxWifi.Location = new System.Drawing.Point(927, 315);
			cbxWifi.Margin = new System.Windows.Forms.Padding(2);
			cbxWifi.Name = "cbxWifi";
			cbxWifi.Size = new System.Drawing.Size(15, 14);
			cbxWifi.TabIndex = 160;
			cbxWifi.UseVisualStyleBackColor = true;
			linkLabel1.AutoSize = true;
			linkLabel1.Location = new System.Drawing.Point(855, 315);
			linkLabel1.Name = "linkLabel1";
			linkLabel1.Size = new System.Drawing.Size(61, 13);
			linkLabel1.TabIndex = 161;
			linkLabel1.TabStop = true;
			linkLabel1.Text = "Kết nối Wifi";
			linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel1_LinkClicked);
			button5.Location = new System.Drawing.Point(357, 543);
			button5.Margin = new System.Windows.Forms.Padding(2);
			button5.Name = "button5";
			button5.Size = new System.Drawing.Size(77, 28);
			button5.TabIndex = 162;
			button5.Text = "Connect Wifi";
			button5.UseVisualStyleBackColor = true;
			button5.Click += new System.EventHandler(button5_Click);
			btnshutdown.Location = new System.Drawing.Point(431, 575);
			btnshutdown.Margin = new System.Windows.Forms.Padding(2);
			btnshutdown.Name = "btnshutdown";
			btnshutdown.Size = new System.Drawing.Size(66, 28);
			btnshutdown.TabIndex = 163;
			btnshutdown.Text = "Tắt tiếng";
			btnshutdown.UseVisualStyleBackColor = true;
			btnshutdown.Click += new System.EventHandler(btnRemove_Click);
			btnportrait.Location = new System.Drawing.Point(802, 543);
			btnportrait.Margin = new System.Windows.Forms.Padding(2);
			btnportrait.Name = "btnportrait";
			btnportrait.Size = new System.Drawing.Size(78, 28);
			btnportrait.TabIndex = 165;
			btnportrait.Text = "Set Portrait";
			btnportrait.UseVisualStyleBackColor = true;
			btnportrait.Click += new System.EventHandler(btnportrait_Click);
			cbxWifiMode.AutoSize = true;
			cbxWifiMode.Location = new System.Drawing.Point(927, 335);
			cbxWifiMode.Margin = new System.Windows.Forms.Padding(2);
			cbxWifiMode.Name = "cbxWifiMode";
			cbxWifiMode.Size = new System.Drawing.Size(15, 14);
			cbxWifiMode.TabIndex = 160;
			cbxWifiMode.UseVisualStyleBackColor = true;
			cbxWifiMode.CheckedChanged += new System.EventHandler(cbxWifiMode_CheckedChanged);
			linkLabel3.AutoSize = true;
			linkLabel3.Location = new System.Drawing.Point(858, 334);
			linkLabel3.Name = "linkLabel3";
			linkLabel3.Size = new System.Drawing.Size(55, 13);
			linkLabel3.TabIndex = 161;
			linkLabel3.TabStop = true;
			linkLabel3.Text = "Wifi Mode";
			linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel3_LinkClicked);
			button2.Location = new System.Drawing.Point(637, 575);
			button2.Margin = new System.Windows.Forms.Padding(2);
			button2.Name = "button2";
			button2.Size = new System.Drawing.Size(119, 28);
			button2.TabIndex = 166;
			button2.Text = "Clear Proxy On Phone";
			button2.UseVisualStyleBackColor = true;
			button2.Click += new System.EventHandler(button2_Click_1);
			button6.BackColor = System.Drawing.Color.BlueViolet;
			button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			button6.ForeColor = System.Drawing.Color.White;
			button6.Location = new System.Drawing.Point(173, 609);
			button6.Margin = new System.Windows.Forms.Padding(2);
			button6.Name = "button6";
			button6.Size = new System.Drawing.Size(136, 28);
			button6.TabIndex = 167;
			button6.Text = "Update Device Changer";
			button6.UseVisualStyleBackColor = false;
			button6.Click += new System.EventHandler(button6_Click);
			button7.Location = new System.Drawing.Point(315, 609);
			button7.Margin = new System.Windows.Forms.Padding(2);
			button7.Name = "button7";
			button7.Size = new System.Drawing.Size(116, 28);
			button7.TabIndex = 165;
			button7.Text = "Reconnect Offline";
			button7.UseVisualStyleBackColor = true;
			button7.Click += new System.EventHandler(button7_Click);
			txtId.Location = new System.Drawing.Point(58, 23);
			txtId.Name = "txtId";
			txtId.Size = new System.Drawing.Size(100, 20);
			txtId.TabIndex = 168;
			txtName.Location = new System.Drawing.Point(58, 52);
			txtName.Name = "txtName";
			txtName.Size = new System.Drawing.Size(100, 20);
			txtName.TabIndex = 169;
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(34, 26);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(18, 13);
			label1.TabIndex = 170;
			label1.Text = "ID";
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(17, 55);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(35, 13);
			label2.TabIndex = 171;
			label2.Text = "Name";
			btnRename.Location = new System.Drawing.Point(8, 78);
			btnRename.Margin = new System.Windows.Forms.Padding(2);
			btnRename.Name = "btnRename";
			btnRename.Size = new System.Drawing.Size(65, 25);
			btnRename.TabIndex = 172;
			btnRename.Text = "Rename";
			btnRename.UseVisualStyleBackColor = true;
			btnRename.Click += new System.EventHandler(btnRename_Click);
			groupBox1.Controls.Add(txtName);
			groupBox1.Controls.Add(btnRename);
			groupBox1.Controls.Add(button4);
			groupBox1.Controls.Add(txtId);
			groupBox1.Controls.Add(label2);
			groupBox1.Controls.Add(label1);
			groupBox1.Location = new System.Drawing.Point(807, 5);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(178, 115);
			groupBox1.TabIndex = 173;
			groupBox1.TabStop = false;
			groupBox1.Text = "Update Device Name";
			btnCCK.BackColor = System.Drawing.Color.Red;
			btnCCK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnCCK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 254);
			btnCCK.ForeColor = System.Drawing.Color.White;
			btnCCK.Location = new System.Drawing.Point(435, 609);
			btnCCK.Margin = new System.Windows.Forms.Padding(2);
			btnCCK.Name = "btnCCK";
			btnCCK.Size = new System.Drawing.Size(88, 28);
			btnCCK.TabIndex = 174;
			btnCCK.Text = "Change BG";
			btnCCK.UseVisualStyleBackColor = false;
			btnCCK.Click += new System.EventHandler(btnCCK_Click);
			button8.BackColor = System.Drawing.Color.Red;
			button8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			button8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 254);
			button8.ForeColor = System.Drawing.Color.White;
			button8.Location = new System.Drawing.Point(529, 609);
			button8.Margin = new System.Windows.Forms.Padding(2);
			button8.Name = "button8";
			button8.Size = new System.Drawing.Size(77, 28);
			button8.TabIndex = 175;
			button8.Text = "Lọc Email";
			button8.UseVisualStyleBackColor = false;
			button8.Click += new System.EventHandler(button8_Click);
			button9.Location = new System.Drawing.Point(610, 609);
			button9.Margin = new System.Windows.Forms.Padding(2);
			button9.Name = "button9";
			button9.Size = new System.Drawing.Size(80, 28);
			button9.TabIndex = 176;
			button9.Text = "Open Link";
			button9.UseVisualStyleBackColor = true;
			button9.Click += new System.EventHandler(button9_Click);
			button10.Location = new System.Drawing.Point(760, 575);
			button10.Margin = new System.Windows.Forms.Padding(2);
			button10.Name = "button10";
			button10.Size = new System.Drawing.Size(120, 28);
			button10.TabIndex = 177;
			button10.Text = "Xóa dữ liệu Tiktok";
			button10.UseVisualStyleBackColor = true;
			button10.Click += new System.EventHandler(button10_Click);
			button11.Location = new System.Drawing.Point(694, 609);
			button11.Margin = new System.Windows.Forms.Padding(2);
			button11.Name = "button11";
			button11.Size = new System.Drawing.Size(93, 28);
			button11.TabIndex = 176;
			button11.Text = "Check Backup";
			button11.UseVisualStyleBackColor = true;
			button11.Click += new System.EventHandler(button11_Click);
			button12.Location = new System.Drawing.Point(701, 543);
			button12.Margin = new System.Windows.Forms.Padding(2);
			button12.Name = "button12";
			button12.Size = new System.Drawing.Size(98, 28);
			button12.TabIndex = 178;
			button12.Text = "ADB Command";
			button12.UseVisualStyleBackColor = true;
			button12.Click += new System.EventHandler(button12_Click);
			button13.Location = new System.Drawing.Point(884, 575);
			button13.Margin = new System.Windows.Forms.Padding(2);
			button13.Name = "button13";
			button13.Size = new System.Drawing.Size(95, 28);
			button13.TabIndex = 179;
			button13.Text = "Fix Devices";
			button13.UseVisualStyleBackColor = true;
			button13.Click += new System.EventHandler(button13_Click);
			numericUpDown1.Location = new System.Drawing.Point(913, 240);
			numericUpDown1.Margin = new System.Windows.Forms.Padding(2);
			numericUpDown1.Maximum = new decimal(new int[4] { 999999999, 0, 0, 0 });
			numericUpDown1.Name = "numericUpDown1";
			numericUpDown1.Size = new System.Drawing.Size(55, 20);
			numericUpDown1.TabIndex = 181;
			numericUpDown1.Value = new decimal(new int[4] { 4723, 0, 0, 0 });
			numericUpDown1.ValueChanged += new System.EventHandler(numericUpDown1_ValueChanged);
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(834, 244);
			label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(51, 13);
			label3.TabIndex = 180;
			label3.Text = "Start Port";
			button14.Location = new System.Drawing.Point(884, 543);
			button14.Margin = new System.Windows.Forms.Padding(2);
			button14.Name = "button14";
			button14.Size = new System.Drawing.Size(95, 28);
			button14.TabIndex = 123;
			button14.Text = "Setup Power On";
			button14.UseVisualStyleBackColor = true;
			button14.Click += new System.EventHandler(button14_Click);
			groupBox3.Controls.Add(rbtTWrp);
			groupBox3.Controls.Add(rbtNoroot);
			groupBox3.Controls.Add(rbtRoot);
			groupBox3.Location = new System.Drawing.Point(810, 385);
			groupBox3.Name = "groupBox3";
			groupBox3.Size = new System.Drawing.Size(179, 109);
			groupBox3.TabIndex = 182;
			groupBox3.TabStop = false;
			groupBox3.Text = "Chế độ điện thoại";
			rbtTWrp.AutoSize = true;
			rbtTWrp.Location = new System.Drawing.Point(17, 72);
			rbtTWrp.Name = "rbtTWrp";
			rbtTWrp.Size = new System.Drawing.Size(135, 17);
			rbtTWrp.TabIndex = 1;
			rbtTWrp.Text = "Không Root với TWRP";
			rbtTWrp.UseVisualStyleBackColor = true;
			rbtTWrp.CheckedChanged += new System.EventHandler(rbtTWrp_CheckedChanged);
			rbtNoroot.AutoSize = true;
			rbtNoroot.Checked = true;
			rbtNoroot.Location = new System.Drawing.Point(17, 49);
			rbtNoroot.Name = "rbtNoroot";
			rbtNoroot.Size = new System.Drawing.Size(132, 17);
			rbtNoroot.TabIndex = 0;
			rbtNoroot.TabStop = true;
			rbtNoroot.Text = "Điện thoại không Root";
			rbtNoroot.UseVisualStyleBackColor = true;
			rbtNoroot.CheckedChanged += new System.EventHandler(rbtNoroot_CheckedChanged);
			rbtRoot.AutoSize = true;
			rbtRoot.Location = new System.Drawing.Point(17, 26);
			rbtRoot.Name = "rbtRoot";
			rbtRoot.Size = new System.Drawing.Size(99, 17);
			rbtRoot.TabIndex = 0;
			rbtRoot.TabStop = true;
			rbtRoot.Text = "Điện thoại Root";
			rbtRoot.UseVisualStyleBackColor = true;
			rbtRoot.CheckedChanged += new System.EventHandler(rbtRoot_CheckedChanged);
			cbxLogVideo.AutoSize = true;
			cbxLogVideo.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			cbxLogVideo.Location = new System.Drawing.Point(850, 269);
			cbxLogVideo.Margin = new System.Windows.Forms.Padding(2);
			cbxLogVideo.Name = "cbxLogVideo";
			cbxLogVideo.Size = new System.Drawing.Size(89, 17);
			cbxLogVideo.TabIndex = 115;
			cbxLogVideo.Text = "Log By Video";
			cbxLogVideo.UseVisualStyleBackColor = true;
			cbxLogVideo.CheckedChanged += new System.EventHandler(cbxLogVideo_CheckedChanged);
			btnTimeZone.Location = new System.Drawing.Point(791, 609);
			btnTimeZone.Margin = new System.Windows.Forms.Padding(2);
			btnTimeZone.Name = "btnTimeZone";
			btnTimeZone.Size = new System.Drawing.Size(118, 28);
			btnTimeZone.TabIndex = 183;
			btnTimeZone.Text = "Change Language";
			btnTimeZone.UseVisualStyleBackColor = true;
			btnTimeZone.Click += new System.EventHandler(btnTimeZone_Click);
			btnshutdowna.BackColor = System.Drawing.Color.Blue;
			btnshutdowna.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnshutdowna.ForeColor = System.Drawing.Color.White;
			btnshutdowna.Location = new System.Drawing.Point(501, 574);
			btnshutdowna.Margin = new System.Windows.Forms.Padding(2);
			btnshutdowna.Name = "btnshutdowna";
			btnshutdowna.Size = new System.Drawing.Size(132, 28);
			btnshutdowna.TabIndex = 140;
			btnshutdowna.Text = "Shutdown Phone";
			btnshutdowna.UseVisualStyleBackColor = false;
			btnshutdowna.Click += new System.EventHandler(btnshutdowna_Click);
			checkBox1.AutoSize = true;
			checkBox1.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			checkBox1.Location = new System.Drawing.Point(824, 356);
			checkBox1.Margin = new System.Windows.Forms.Padding(2);
			checkBox1.Name = "checkBox1";
			checkBox1.Size = new System.Drawing.Size(118, 17);
			checkBox1.TabIndex = 184;
			checkBox1.Text = "Bật wifi khi proxy lỗi";
			checkBox1.UseVisualStyleBackColor = true;
			checkBox1.CheckedChanged += new System.EventHandler(checkBox1_CheckedChanged);
			cbxSchedule.AutoSize = true;
			cbxSchedule.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			cbxSchedule.Location = new System.Drawing.Point(671, 511);
			cbxSchedule.Margin = new System.Windows.Forms.Padding(2);
			cbxSchedule.Name = "cbxSchedule";
			cbxSchedule.Size = new System.Drawing.Size(113, 17);
			cbxSchedule.TabIndex = 115;
			cbxSchedule.Text = "Hẹn giờ chạy Tool";
			cbxSchedule.UseVisualStyleBackColor = true;
			cbxSchedule.CheckedChanged += new System.EventHandler(cbxSchedule_CheckedChanged);
			button1.Location = new System.Drawing.Point(912, 608);
			button1.Margin = new System.Windows.Forms.Padding(2);
			button1.Name = "button1";
			button1.Size = new System.Drawing.Size(78, 28);
			button1.TabIndex = 179;
			button1.Text = "Time Zone";
			button1.UseVisualStyleBackColor = true;
			button1.Click += new System.EventHandler(button1_Click_2);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(1001, 651);
			base.Controls.Add(checkBox1);
			base.Controls.Add(btnTimeZone);
			base.Controls.Add(groupBox3);
			base.Controls.Add(numericUpDown1);
			base.Controls.Add(label3);
			base.Controls.Add(button1);
			base.Controls.Add(button13);
			base.Controls.Add(button12);
			base.Controls.Add(button10);
			base.Controls.Add(button11);
			base.Controls.Add(button9);
			base.Controls.Add(button8);
			base.Controls.Add(btnCCK);
			base.Controls.Add(groupBox1);
			base.Controls.Add(button6);
			base.Controls.Add(button2);
			base.Controls.Add(button7);
			base.Controls.Add(btnportrait);
			base.Controls.Add(btnshutdown);
			base.Controls.Add(button5);
			base.Controls.Add(linkLabel3);
			base.Controls.Add(linkLabel1);
			base.Controls.Add(cbxWifiMode);
			base.Controls.Add(cbxWifi);
			base.Controls.Add(btnAirOff);
			base.Controls.Add(btnAir);
			base.Controls.Add(btnLang);
			base.Controls.Add(btnedXposed);
			base.Controls.Add(button3);
			base.Controls.Add(nudPhoneDelay);
			base.Controls.Add(label5);
			base.Controls.Add(btnshutdowna);
			base.Controls.Add(btnReboot);
			base.Controls.Add(label4);
			base.Controls.Add(numLine);
			base.Controls.Add(btnShowPhone);
			base.Controls.Add(btnClean);
			base.Controls.Add(button14);
			base.Controls.Add(btnAppium);
			base.Controls.Add(btnBri);
			base.Controls.Add(btnSetupApk);
			base.Controls.Add(btnturnoff);
			base.Controls.Add(btnTurnon);
			base.Controls.Add(cbxSchedule);
			base.Controls.Add(cbxLogVideo);
			base.Controls.Add(cbxVolumn);
			base.Controls.Add(nudTimeout);
			base.Controls.Add(label8);
			base.Controls.Add(nudBright);
			base.Controls.Add(btnSetup);
			base.Controls.Add(btnCheck);
			base.Controls.Add(dataGridViewPhone);
			base.Controls.Add(btnLoadDevices);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "frmDevices";
			base.ShowIcon = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "Devices List";
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(frmDevices_FormClosing);
			base.Load += new System.EventHandler(frmDevices_Load);
			((System.ComponentModel.ISupportInitialize)nudTimeout).EndInit();
			((System.ComponentModel.ISupportInitialize)nudBright).EndInit();
			((System.ComponentModel.ISupportInitialize)dataGridViewPhone).EndInit();
			((System.ComponentModel.ISupportInitialize)numLine).EndInit();
			((System.ComponentModel.ISupportInitialize)nudPhoneDelay).EndInit();
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
			groupBox3.ResumeLayout(false);
			groupBox3.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
