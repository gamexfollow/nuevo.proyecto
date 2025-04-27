using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using CCKTiktok.Bussiness;
using CCKTiktok.DAL;
using CCKTiktok.Helper;

namespace CCKTiktok.Component
{
	public class frmPhoneView : Form
	{
		public const int WM_SYSCOMMAND = 274;

		public const int SC_CLOSE = 61536;

		private bool isRunning = true;

		private List<IntPtr> opener = new List<IntPtr>();

		private static Dictionary<string, string> dicDevices = new Dictionary<string, string>();

		private IContainer components = null;

		private TabControl tabControl1;

		private TabPage tabPage1;

		private Panel LDPanel;

		private TabPage tabPage2;

		private CheckedListBox cbxCategory;

		private Button btnLoadDevices;

		private Button btnGroupAdd;

		private Label label1;

		private TextBox txtName;

		private Button button2;

		private ListBox lstGroup;

		private Button button3;

		private ListBox lstPhoneInGroup;

		private Button btnDelete;

		private Label lblMsg;

		private Label label3;

		private Label label2;

		private NumericUpDown nudHeight;

		private NumericUpDown nudWith;

		private LinkLabel linkLabel1;

		private CheckBox cbxCheckall;

		public frmPhoneView()
		{
			InitializeComponent();
		}

		[DllImport("user32.dll")]
		private static extern bool DestroyWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll")]
		public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

		[DllImport("user32.dll")]
		public static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);

		private void frmPhoneView_Load(object sender, EventArgs e)
		{
			if (File.Exists(CaChuaConstant.Group_Data))
			{
				List<GroupInfo> list = new List<GroupInfo>();
				JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer
				{
					MaxJsonLength = int.MaxValue
				};
				list = javaScriptSerializer.Deserialize<List<GroupInfo>>(Utils.ReadTextFile(CaChuaConstant.Group_Data));
				GroupInfo groupInfo = new GroupInfo();
				groupInfo.Uid = DateTime.Now.ToFileTime().ToString();
				groupInfo.Name = txtName.Text.ToString();
				lstGroup.DataSource = new BindingSource(list, null);
				lstGroup.DisplayMember = "Name";
				lstGroup.ValueMember = "Uid";
				nudHeight.Value = Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.PHONE_HEIGHT));
				if (nudHeight.Value == 0m)
				{
					nudHeight.Value = 400m;
				}
				nudWith.Value = Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.PHONE_WIDTH));
				if (nudWith.Value == 0m)
				{
					nudWith.Value = 220m;
				}
			}
			btnLoadDevices_Click(null, null);
			if (lstGroup.Items.Count > 0)
			{
				lstGroup.SelectedIndex = 0;
				lstGroup_SelectedIndexChanged(null, null);
			}
			ViewScrcpy();
		}

		private void ViewScrcpy()
		{
			ManagementEventWatcher managementEventWatcher = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace WHERE ProcessName='scrcpy'"));
			managementEventWatcher.EventArrived += delegate
			{
				if (IsPhoneConnected())
				{
					StartScrcpy();
				}
				else
				{
					Console.WriteLine("Phone disconnected, waiting for reconnection...");
				}
			};
			managementEventWatcher.Start();
			StartScrcpy();
			new Task(delegate
			{
				while (isRunning)
				{
					if (!IsPhoneConnected())
					{
						Console.WriteLine("Phone disconnected, waiting for reconnection...");
					}
					Thread.Sleep(1000);
				}
			}).Start();
		}

		private static void StartScrcpy()
		{
			Console.WriteLine("Scrcpy started.");
		}

		private static bool IsPhoneConnected()
		{
			return true;
		}

		private void BindData(List<string> lst)
		{
			Thread thread = null;
			List<ViewPhoneItem> lstViewPhone = new List<ViewPhoneItem>();
			List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
			IEnumerable<string> source = from str1 in lst
				join str2 in listSerialDevice on str1 equals str2
				select str1;
			lst = source.ToList();
			int num = Screen.PrimaryScreen.Bounds.Width - 10;
			int widthMin = Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.PHONE_WIDTH));
			decimal ratio = nudHeight.Value / nudWith.Value;
			int col = num / widthMin;
			int count = lst.Count;
			int num2 = count / col + 1;
			int current = 0;
			LDPanel.AutoScrollMinSize = new Size(0, num2 * (int)((decimal)widthMin * ratio));
			for (int i = 0; i < count; i++)
			{
				string s = lst[i];
				thread = new Thread((ThreadStart)delegate
				{
					int num3 = current;
					current = num3 + 1;
					int num4 = num3;
					int left = num4 % col * widthMin;
					int top = num4 / col * (int)((decimal)widthMin * ratio);
					string text = s;
					ViewPhoneItem viewPhoneItem2 = new ViewPhoneItem
					{
						Name = text,
						Left = left,
						Top = top,
						Width = widthMin,
						Height = (int)((decimal)widthMin * ratio)
					};
					lstViewPhone.Add(viewPhoneItem2);
					ADBHelperCCK.DisplayPhone(s, text, viewPhoneItem2.Left, viewPhoneItem2.Top, viewPhoneItem2.Width, viewPhoneItem2.Height);
					Thread.Sleep(2000);
					while (isRunning)
					{
						IntPtr hWnd2 = FindWindow(null, text);
						if (hWnd2 != IntPtr.Zero && !opener.Contains(hWnd2) && isRunning)
						{
							opener.Add(hWnd2);
							MoveWindow(hWnd2, viewPhoneItem2.Left, viewPhoneItem2.Top, viewPhoneItem2.Width, viewPhoneItem2.Height, bRepaint: true);
							LDPanel.Invoke((MethodInvoker)delegate
							{
								if (isRunning)
								{
									SetParent(hWnd2, LDPanel.Handle);
								}
							});
							break;
						}
						ADBHelperCCK.DisplayPhone(s, text, viewPhoneItem2.Left, viewPhoneItem2.Top, viewPhoneItem2.Width, viewPhoneItem2.Height);
						Thread.Sleep(5000);
						Thread.Sleep(2000);
					}
				});
				thread.Start();
				Thread.Sleep(500);
			}
			bool flag = true;
			while (isRunning)
			{
				List<string> list = new List<string>();
				Process[] processesByName = Process.GetProcessesByName("scrcpy");
				List<Process> list2 = new List<Process>();
				Process[] array = processesByName;
				foreach (Process process in array)
				{
					_ = process.MainWindowTitle;
					string commandLine = GetCommandLine(process);
					if (commandLine != null)
					{
						Regex regex = new Regex("serial \"(.*?)\"");
						Match match = regex.Match(commandLine);
						string value = match.Groups[1].Value;
						if (!list.Contains(value))
						{
							list.Add(value);
						}
						else
						{
							list2.Add(process);
						}
					}
				}
				foreach (Process item in list2)
				{
					try
					{
						item.Kill();
					}
					catch
					{
					}
				}
				List<string> list3 = lst.Except(list).ToList();
				if (list3.Count > 0 || flag)
				{
					flag = false;
					foreach (string a in list3)
					{
						new Task(delegate
						{
							ViewPhoneItem viewPhoneItem = lstViewPhone.Find((ViewPhoneItem o) => o.Name == a);
							if (viewPhoneItem != null)
							{
								ADBHelperCCK.DisplayPhone(viewPhoneItem.Name, viewPhoneItem.Name, viewPhoneItem.Left, viewPhoneItem.Top, viewPhoneItem.Width, viewPhoneItem.Height);
								Thread.Sleep(2000);
								IntPtr hWnd = FindWindow(null, viewPhoneItem.Name);
								if (hWnd != IntPtr.Zero && !opener.Contains(hWnd) && isRunning)
								{
									opener.Add(hWnd);
									MoveWindow(hWnd, viewPhoneItem.Left, viewPhoneItem.Top, viewPhoneItem.Width, viewPhoneItem.Height, bRepaint: true);
									LDPanel.Invoke((MethodInvoker)delegate
									{
										if (isRunning)
										{
											SetParent(hWnd, LDPanel.Handle);
										}
									});
								}
							}
						}).Start();
					}
				}
				else
				{
					Thread.Sleep(15000);
				}
			}
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

		private void btnLoadDevices_Click(object sender, EventArgs e)
		{
			LoadDevices();
			List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("IP");
			dataTable.Columns.Add("Name");
			foreach (string item in listSerialDevice)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["IP"] = item;
				string text = (string)(dataRow["Name"] = (dicDevices.ContainsKey(item) ? dicDevices[item] : item));
				dataTable.Rows.Add(dataRow);
			}
			dataTable.AcceptChanges();
			dataTable.DefaultView.Sort = "Name";
			dataTable = dataTable.DefaultView.ToTable();
			cbxCategory.DataSource = new BindingSource(dataTable, null);
			cbxCategory.DisplayMember = "Name";
			cbxCategory.ValueMember = "IP";
		}

		protected static void BindObjectList(DataGridView dataGridView, DataTable table, bool fixmode)
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
					}
					dataGridView.DataSource = table;
					if (fixmode)
					{
						dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
					}
					table = null;
				});
			}
			catch
			{
			}
		}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage1"])
			{
				btnLoadDevices_Click(null, null);
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (lstGroup.SelectedIndex == -1)
			{
				if (lstGroup.Items.Count != 0)
				{
					MessageBox.Show("Please select group");
					txtName.Focus();
					return;
				}
				txtName.Text = "All Phone";
				btnGroupAdd_Click(null, null);
				lstGroup.SelectedIndex = 0;
				txtName.Text = "";
			}
			lstGroup.SelectedValue.ToString();
			List<string> list = new List<string>();
			for (int i = 0; i < cbxCategory.Items.Count; i++)
			{
				if (cbxCategory.GetItemChecked(i))
				{
					list.Add(((DataRowView)cbxCategory.Items[i])["IP"].ToString());
				}
			}
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			string key = lstGroup.SelectedValue.ToString();
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer
			{
				MaxJsonLength = int.MaxValue
			};
			if (File.Exists(CaChuaConstant.Group_Data_mapping))
			{
				dictionary = javaScriptSerializer.Deserialize<Dictionary<string, List<string>>>(Utils.ReadTextFile(CaChuaConstant.Group_Data_mapping));
			}
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, list);
			}
			else
			{
				dictionary[key] = list;
			}
			File.WriteAllText(CaChuaConstant.Group_Data_mapping, javaScriptSerializer.Serialize(dictionary));
			cbxCategory.ClearSelected();
			lstGroup_SelectedIndexChanged(null, null);
		}

		private void button3_Click(object sender, EventArgs e)
		{
			ADBHelperCCK.ExecuteCMD("taskkill /f /im scrcpy.exe");
			List<string> lst = new List<string>();
			try
			{
				foreach (IntPtr item in opener)
				{
					SendMessage((int)item, 274u, 61536, 0);
				}
			}
			catch
			{
			}
			ListBox.ObjectCollection items = lstPhoneInGroup.Items;
			foreach (DataRowView item2 in items)
			{
				lst.Add(item2["IP"].ToString());
			}
			tabControl1.SelectedIndex = 1;
			new Task(delegate
			{
				BindData(lst);
			}).Start();
		}

		private void cbxCategory_SelectedIndexChanged(object sender, EventArgs e)
		{
			int num = 0;
			for (int i = 0; i < cbxCategory.Items.Count; i++)
			{
				if (cbxCategory.GetItemChecked(i))
				{
					num++;
				}
			}
			lblMsg.Text = $"Selected: {num} phones";
		}

		private void btnGroupAdd_Click(object sender, EventArgs e)
		{
			if (txtName.Text != "")
			{
				List<GroupInfo> list = new List<GroupInfo>();
				JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer
				{
					MaxJsonLength = int.MaxValue
				};
				if (File.Exists(CaChuaConstant.Group_Data))
				{
					list = javaScriptSerializer.Deserialize<List<GroupInfo>>(Utils.ReadTextFile(CaChuaConstant.Group_Data));
				}
				GroupInfo groupInfo = new GroupInfo();
				groupInfo.Uid = DateTime.Now.ToFileTime().ToString();
				groupInfo.Name = txtName.Text.ToString();
				txtName.Text = "";
				list.Add(groupInfo);
				lstGroup.DataSource = new BindingSource(list, null);
				lstGroup.DisplayMember = "Name";
				lstGroup.ValueMember = "Uid";
				File.WriteAllText(CaChuaConstant.Group_Data, javaScriptSerializer.Serialize(list));
			}
		}

		private static string GetCommandLine(Process process)
		{
			using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id);
			using ManagementObjectCollection source = managementObjectSearcher.Get();
			return source.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString();
		}

		private void lstGroup_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				lstPhoneInGroup.Items.Clear();
			}
			catch
			{
			}
			txtName.Text = "";
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			string key = lstGroup.SelectedValue.ToString();
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer
			{
				MaxJsonLength = int.MaxValue
			};
			if (File.Exists(CaChuaConstant.Group_Data_mapping))
			{
				dictionary = javaScriptSerializer.Deserialize<Dictionary<string, List<string>>>(Utils.ReadTextFile(CaChuaConstant.Group_Data_mapping));
			}
			if (dictionary.ContainsKey(key))
			{
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("IP");
				dataTable.Columns.Add("Name");
				foreach (string item in dictionary[key])
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["IP"] = item;
					string text = (string)(dataRow["Name"] = (dicDevices.ContainsKey(item) ? dicDevices[item] : item));
					dataTable.Rows.Add(dataRow);
				}
				dataTable.AcceptChanges();
				lstPhoneInGroup.DataSource = new BindingSource(dataTable, null);
				lstPhoneInGroup.DisplayMember = "Name";
				lstPhoneInGroup.ValueMember = "IP";
			}
			button3.Text = $"Show {lstPhoneInGroup.Items.Count} Phones";
		}

		private void nudWith_ValueChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.PHONE_WIDTH, nudWith.Value.ToString());
		}

		private void nudHeight_ValueChanged(object sender, EventArgs e)
		{
			File.WriteAllText(CaChuaConstant.PHONE_HEIGHT, nudHeight.Value.ToString());
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://www.youtube.com/watch?v=-ufrKrXo_48");
		}

		private void tabPage2_Click(object sender, EventArgs e)
		{
		}

		private void frmPhoneView_FormClosing(object sender, FormClosingEventArgs e)
		{
			isRunning = false;
		}

		private void btnUpdate_Click(object sender, EventArgs e)
		{
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (lstGroup.SelectedItem.ToString() != "")
			{
				List<GroupInfo> list = new List<GroupInfo>();
				JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer
				{
					MaxJsonLength = int.MaxValue
				};
				if (File.Exists(CaChuaConstant.Group_Data))
				{
					list = javaScriptSerializer.Deserialize<List<GroupInfo>>(Utils.ReadTextFile(CaChuaConstant.Group_Data));
				}
				GroupInfo groupInfo = list.Find((GroupInfo o) => o.Name == ((GroupInfo)lstGroup.SelectedItem).Name.ToString());
				if (groupInfo != null)
				{
					list.Remove(groupInfo);
				}
				lstGroup.DataSource = new BindingSource(list, null);
				lstGroup.DisplayMember = "Name";
				lstGroup.ValueMember = "Uid";
				File.WriteAllText(CaChuaConstant.Group_Data, javaScriptSerializer.Serialize(list));
			}
		}

		private void groupBox1_DragOver(object sender, DragEventArgs e)
		{
		}

		private void cbxCheckall_CheckedChanged(object sender, EventArgs e)
		{
			for (int i = 0; i < cbxCategory.Items.Count; i++)
			{
				cbxCategory.SetItemChecked(i, cbxCheckall.Checked);
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
			tabControl1 = new System.Windows.Forms.TabControl();
			tabPage2 = new System.Windows.Forms.TabPage();
			linkLabel1 = new System.Windows.Forms.LinkLabel();
			label3 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			nudHeight = new System.Windows.Forms.NumericUpDown();
			nudWith = new System.Windows.Forms.NumericUpDown();
			lblMsg = new System.Windows.Forms.Label();
			btnDelete = new System.Windows.Forms.Button();
			button3 = new System.Windows.Forms.Button();
			lstPhoneInGroup = new System.Windows.Forms.ListBox();
			lstGroup = new System.Windows.Forms.ListBox();
			button2 = new System.Windows.Forms.Button();
			btnGroupAdd = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			txtName = new System.Windows.Forms.TextBox();
			btnLoadDevices = new System.Windows.Forms.Button();
			cbxCategory = new System.Windows.Forms.CheckedListBox();
			tabPage1 = new System.Windows.Forms.TabPage();
			LDPanel = new System.Windows.Forms.Panel();
			cbxCheckall = new System.Windows.Forms.CheckBox();
			tabControl1.SuspendLayout();
			tabPage2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)nudHeight).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudWith).BeginInit();
			tabPage1.SuspendLayout();
			SuspendLayout();
			tabControl1.Controls.Add(tabPage2);
			tabControl1.Controls.Add(tabPage1);
			tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			tabControl1.Location = new System.Drawing.Point(0, 0);
			tabControl1.Name = "tabControl1";
			tabControl1.SelectedIndex = 0;
			tabControl1.Size = new System.Drawing.Size(1022, 687);
			tabControl1.TabIndex = 3;
			tabControl1.SelectedIndexChanged += new System.EventHandler(tabControl1_SelectedIndexChanged);
			tabPage2.Controls.Add(cbxCheckall);
			tabPage2.Controls.Add(linkLabel1);
			tabPage2.Controls.Add(label3);
			tabPage2.Controls.Add(label2);
			tabPage2.Controls.Add(nudHeight);
			tabPage2.Controls.Add(nudWith);
			tabPage2.Controls.Add(lblMsg);
			tabPage2.Controls.Add(btnDelete);
			tabPage2.Controls.Add(button3);
			tabPage2.Controls.Add(lstPhoneInGroup);
			tabPage2.Controls.Add(lstGroup);
			tabPage2.Controls.Add(button2);
			tabPage2.Controls.Add(btnGroupAdd);
			tabPage2.Controls.Add(label1);
			tabPage2.Controls.Add(txtName);
			tabPage2.Controls.Add(btnLoadDevices);
			tabPage2.Controls.Add(cbxCategory);
			tabPage2.Location = new System.Drawing.Point(4, 22);
			tabPage2.Name = "tabPage2";
			tabPage2.Padding = new System.Windows.Forms.Padding(3);
			tabPage2.Size = new System.Drawing.Size(1014, 661);
			tabPage2.TabIndex = 1;
			tabPage2.Text = "Configuration";
			tabPage2.UseVisualStyleBackColor = true;
			tabPage2.Click += new System.EventHandler(tabPage2_Click);
			linkLabel1.AutoSize = true;
			linkLabel1.Location = new System.Drawing.Point(21, 15);
			linkLabel1.Name = "linkLabel1";
			linkLabel1.Size = new System.Drawing.Size(155, 13);
			linkLabel1.TabIndex = 127;
			linkLabel1.TabStop = true;
			linkLabel1.Text = "Watch Video / Xem hướng dẫn";
			linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel1_LinkClicked);
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(29, 551);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(72, 13);
			label3.TabIndex = 126;
			label3.Text = "Phone Height";
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(29, 520);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(69, 13);
			label2.TabIndex = 125;
			label2.Text = "Phone Width";
			nudHeight.Location = new System.Drawing.Point(104, 549);
			nudHeight.Maximum = new decimal(new int[4] { 2000, 0, 0, 0 });
			nudHeight.Name = "nudHeight";
			nudHeight.Size = new System.Drawing.Size(51, 20);
			nudHeight.TabIndex = 124;
			nudHeight.Value = new decimal(new int[4] { 330, 0, 0, 0 });
			nudHeight.ValueChanged += new System.EventHandler(nudHeight_ValueChanged);
			nudWith.Location = new System.Drawing.Point(104, 517);
			nudWith.Maximum = new decimal(new int[4] { 1000, 0, 0, 0 });
			nudWith.Name = "nudWith";
			nudWith.Size = new System.Drawing.Size(51, 20);
			nudWith.TabIndex = 123;
			nudWith.Value = new decimal(new int[4] { 220, 0, 0, 0 });
			nudWith.ValueChanged += new System.EventHandler(nudWith_ValueChanged);
			lblMsg.AutoSize = true;
			lblMsg.Location = new System.Drawing.Point(21, 494);
			lblMsg.Name = "lblMsg";
			lblMsg.Size = new System.Drawing.Size(49, 13);
			lblMsg.TabIndex = 122;
			lblMsg.Text = "Selected";
			btnDelete.Location = new System.Drawing.Point(127, 280);
			btnDelete.Name = "btnDelete";
			btnDelete.Size = new System.Drawing.Size(103, 23);
			btnDelete.TabIndex = 121;
			btnDelete.Text = "Delete";
			btnDelete.UseVisualStyleBackColor = true;
			btnDelete.Click += new System.EventHandler(btnDelete_Click);
			button3.BackColor = System.Drawing.Color.Red;
			button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 254);
			button3.ForeColor = System.Drawing.Color.White;
			button3.Location = new System.Drawing.Point(24, 585);
			button3.Name = "button3";
			button3.Size = new System.Drawing.Size(206, 35);
			button3.TabIndex = 119;
			button3.Text = "Show Phone";
			button3.UseVisualStyleBackColor = false;
			button3.Click += new System.EventHandler(button3_Click);
			lstPhoneInGroup.FormattingEnabled = true;
			lstPhoneInGroup.Location = new System.Drawing.Point(24, 421);
			lstPhoneInGroup.Name = "lstPhoneInGroup";
			lstPhoneInGroup.Size = new System.Drawing.Size(206, 56);
			lstPhoneInGroup.TabIndex = 118;
			lstGroup.FormattingEnabled = true;
			lstGroup.Location = new System.Drawing.Point(24, 309);
			lstGroup.Name = "lstGroup";
			lstGroup.Size = new System.Drawing.Size(206, 69);
			lstGroup.TabIndex = 117;
			lstGroup.SelectedIndexChanged += new System.EventHandler(lstGroup_SelectedIndexChanged);
			button2.Location = new System.Drawing.Point(24, 385);
			button2.Name = "button2";
			button2.Size = new System.Drawing.Size(207, 30);
			button2.TabIndex = 116;
			button2.Text = "Add Phone to Group";
			button2.UseVisualStyleBackColor = true;
			button2.Click += new System.EventHandler(button2_Click);
			btnGroupAdd.Location = new System.Drawing.Point(23, 280);
			btnGroupAdd.Name = "btnGroupAdd";
			btnGroupAdd.Size = new System.Drawing.Size(98, 23);
			btnGroupAdd.TabIndex = 115;
			btnGroupAdd.Text = "Add";
			btnGroupAdd.UseVisualStyleBackColor = true;
			btnGroupAdd.Click += new System.EventHandler(btnGroupAdd_Click);
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(21, 232);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(67, 13);
			label1.TabIndex = 114;
			label1.Text = "Group Name";
			txtName.Location = new System.Drawing.Point(23, 254);
			txtName.Name = "txtName";
			txtName.Size = new System.Drawing.Size(207, 20);
			txtName.TabIndex = 112;
			btnLoadDevices.BackColor = System.Drawing.SystemColors.AppWorkspace;
			btnLoadDevices.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnLoadDevices.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			btnLoadDevices.ForeColor = System.Drawing.Color.White;
			btnLoadDevices.Location = new System.Drawing.Point(117, 36);
			btnLoadDevices.Margin = new System.Windows.Forms.Padding(2);
			btnLoadDevices.Name = "btnLoadDevices";
			btnLoadDevices.Size = new System.Drawing.Size(113, 25);
			btnLoadDevices.TabIndex = 111;
			btnLoadDevices.Text = "Load Devices";
			btnLoadDevices.UseVisualStyleBackColor = false;
			btnLoadDevices.Click += new System.EventHandler(btnLoadDevices_Click);
			cbxCategory.CheckOnClick = true;
			cbxCategory.FormattingEnabled = true;
			cbxCategory.Location = new System.Drawing.Point(21, 66);
			cbxCategory.Margin = new System.Windows.Forms.Padding(2);
			cbxCategory.Name = "cbxCategory";
			cbxCategory.Size = new System.Drawing.Size(209, 154);
			cbxCategory.TabIndex = 13;
			cbxCategory.SelectedIndexChanged += new System.EventHandler(cbxCategory_SelectedIndexChanged);
			tabPage1.Controls.Add(LDPanel);
			tabPage1.Location = new System.Drawing.Point(4, 22);
			tabPage1.Name = "tabPage1";
			tabPage1.Padding = new System.Windows.Forms.Padding(3);
			tabPage1.Size = new System.Drawing.Size(1014, 661);
			tabPage1.TabIndex = 0;
			tabPage1.Text = "Phone List";
			tabPage1.UseVisualStyleBackColor = true;
			LDPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			LDPanel.Location = new System.Drawing.Point(3, 3);
			LDPanel.Name = "LDPanel";
			LDPanel.Size = new System.Drawing.Size(1008, 655);
			LDPanel.TabIndex = 3;
			cbxCheckall.AutoSize = true;
			cbxCheckall.Location = new System.Drawing.Point(25, 41);
			cbxCheckall.Name = "cbxCheckall";
			cbxCheckall.Size = new System.Drawing.Size(70, 17);
			cbxCheckall.TabIndex = 128;
			cbxCheckall.Text = "Check all";
			cbxCheckall.UseVisualStyleBackColor = true;
			cbxCheckall.CheckedChanged += new System.EventHandler(cbxCheckall_CheckedChanged);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(1022, 687);
			base.Controls.Add(tabControl1);
			base.Name = "frmPhoneView";
			base.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			Text = "frmPhoneView";
			base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(frmPhoneView_FormClosing);
			base.Load += new System.EventHandler(frmPhoneView_Load);
			tabControl1.ResumeLayout(false);
			tabPage2.ResumeLayout(false);
			tabPage2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)nudHeight).EndInit();
			((System.ComponentModel.ISupportInitialize)nudWith).EndInit();
			tabPage1.ResumeLayout(false);
			ResumeLayout(false);
		}
	}
}
