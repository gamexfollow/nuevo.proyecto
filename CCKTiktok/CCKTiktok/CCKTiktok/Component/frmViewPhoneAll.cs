using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CCKTiktok.Bussiness;
using CCKTiktok.DAL;
using CCKTiktok.Helper;

namespace CCKTiktok.Component
{
	public class frmViewPhoneAll : Form
	{
		private int numberOfDevice = 0;

		public const int WM_SYSCOMMAND = 274;

		public const int SC_CLOSE = 61536;

		private bool isRunning = true;

		private Dictionary<string, string> param = new Dictionary<string, string>();

		private Dictionary<string, ScrcpyEntity> paramObject = new Dictionary<string, ScrcpyEntity>();

		private Dictionary<string, string> dicDevices = new Dictionary<string, string>();

		private List<string> startedDevices = new List<string>();

		private string scrcpyPath = "scrcpy.exe";

		private bool needReconnect = true;

		private IContainer components = null;

		private Panel LDPanel;

		public frmViewPhoneAll(int numberOfDevice)
		{
			InitializeComponent();
			this.numberOfDevice = numberOfDevice;
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

		private async void frmViewPhoneAll_Load(object sender, EventArgs e)
		{
			LoadDevices();
			LDPanel.AutoScroll = true;
			ADBHelperCCK.KillSCRCPY();
			List<string> deviceSerials = ADBHelperCCK.GetListSerialDevice();
			Rectangle resolution = Screen.PrimaryScreen.Bounds;
			_ = resolution.Width;
			_ = resolution.Height;
			List<ScrcpyEntity> lst = new List<ScrcpyEntity>();
			int iRow = 0;
			int iColumn = 0;
			int height = 1600;
			int width = 720;
			double ratio = (double)width / (double)height;
			if (deviceSerials.Count > 0)
			{
				Point screen = ADBHelperCCK.GetScreenResolution(deviceSerials[0]);
				ratio = (double)screen.Y / (double)screen.X;
			}
			int w = Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.PHONE_WIDTH));
			if (w == 0)
			{
				w = 240;
			}
			Convert.ToInt32((double)w * ratio);
			int numLine = base.Width / w;
			int remain = (base.Width - 30) % w;
			int wrena = remain / numLine;
			w += wrena;
			int h = Convert.ToInt32((double)w * ratio);
			int numOfDevicePerRow = numLine;
			int top = 0;
			int datacount = (deviceSerials.Count - 1) / numOfDevicePerRow + 1;
			LDPanel.AutoScrollMinSize = new Size(base.Width, datacount * h + 10 * datacount);
			int rowCount = ((deviceSerials.Count < numberOfDevice) ? deviceSerials.Count : numberOfDevice);
			for (int i = 1; i <= rowCount; i++)
			{
				string row = deviceSerials[i - 1];
				string deviceId = row;
				if (deviceId != null)
				{
					ScrcpyEntity item = new ScrcpyEntity();
					item.DeviceId = row.ToString();
					item.DeviceName = ((dicDevices == null || !dicDevices.ContainsKey(item.DeviceId)) ? item.DeviceId : dicDevices[item.DeviceId]);
					item.DeviceId = row.ToString();
					item.Top = top + (iRow + 1);
					item.Left = iColumn * w + 5;
					item.Width = w - 5;
					item.Height = h - 5;
					lst.Add(item);
					if (i % numOfDevicePerRow == 0 && i > 0)
					{
						iColumn = 0;
						iRow++;
						top = iRow * h;
					}
					else
					{
						iColumn++;
					}
					param.Add(deviceId, string.Format(" --serial \"{0}\" --window-title \"{1}\" --window-y {2} --window-x {3} --window-width {4} --window-height {5} --max-size 1024 --bit-rate 4M --max-fps 15 {6}", deviceId, item.DeviceName ?? deviceId, item.Top, item.Left, item.Width, item.Height, Utils.ConvertToBoolean(Utils.ReadTextFile(CaChuaConstant.BorderLess)) ? "--window-borderless" : ""));
					paramObject.Add(deviceId, item);
				}
			}
			new Task(delegate
			{
				while (isRunning)
				{
					if (needReconnect)
					{
						foreach (string serial in deviceSerials)
						{
							if (!IsScrcpyRunning(serial))
							{
								needReconnect = false;
								new Task(delegate
								{
									StartScrcpy(serial);
								}).Start();
							}
						}
					}
					Thread.Sleep(TimeSpan.FromSeconds(5.0));
				}
			}).Start();
		}

		private bool IsScrcpyRunning(string serial)
		{
			string processName = "scrcpy";
			string value = param[serial];
			Process[] processesByName = Process.GetProcessesByName(processName);
			Process[] array = processesByName;
			int num = 0;
			while (true)
			{
				if (num < array.Length)
				{
					Process process = array[num];
					string commandLine = GetCommandLine(process.Id);
					if (commandLine != null && commandLine.Contains(value))
					{
						break;
					}
					num++;
					continue;
				}
				return false;
			}
			return true;
		}

		private void StartScrcpy(string serial)
		{
			while (true)
			{
				string fileName = "scrcpy.exe";
				string arguments = param[serial];
				Process process = new Process();
				process.StartInfo.FileName = fileName;
				process.StartInfo.Arguments = arguments;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				process.EnableRaisingEvents = true;
				process.Exited += delegate
				{
					needReconnect = true;
				};
				process.Start();
				Thread.Sleep(2000);
				int num = 0;
				while (true)
				{
					if (!isRunning)
					{
						return;
					}
					string lpWindowName = paramObject[serial].DeviceName ?? serial;
					IntPtr hWnd = FindWindow(null, lpWindowName);
					if (!(hWnd != IntPtr.Zero) || !isRunning)
					{
						num++;
						if (num > 5)
						{
							break;
						}
						Thread.Sleep(2000);
						continue;
					}
					ScrcpyEntity scrcpyEntity = paramObject[serial];
					Thread.Sleep(1000);
					MoveWindow(hWnd, scrcpyEntity.Left, scrcpyEntity.Top, scrcpyEntity.Width, scrcpyEntity.Height, bRepaint: true);
					try
					{
						LDPanel.Invoke((MethodInvoker)delegate
						{
							if (isRunning)
							{
								SetParent(hWnd, LDPanel.Handle);
							}
						});
					}
					catch
					{
					}
					return;
				}
			}
		}

		private static string GetCommandLine(int processId)
		{
			string queryString = $"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {processId}";
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(queryString))
			{
				using ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
				using ManagementObjectCollection.ManagementObjectEnumerator managementObjectEnumerator = managementObjectCollection.GetEnumerator();
				if (managementObjectEnumerator.MoveNext())
				{
					ManagementBaseObject current = managementObjectEnumerator.Current;
					return current["CommandLine"]?.ToString();
				}
			}
			return null;
		}

		private void frmViewPhoneAll_FormClosing(object sender, FormClosingEventArgs e)
		{
			isRunning = false;
			ADBHelperCCK.KillSCRCPY();
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
			LDPanel = new System.Windows.Forms.Panel();
			SuspendLayout();
			LDPanel.AutoScroll = true;
			LDPanel.AutoScrollMargin = new System.Drawing.Size(320, 600);
			LDPanel.AutoScrollMinSize = new System.Drawing.Size(320, 600);
			LDPanel.AutoSize = true;
			LDPanel.BackColor = System.Drawing.Color.DarkGray;
			LDPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			LDPanel.Location = new System.Drawing.Point(0, 0);
			LDPanel.Name = "LDPanel";
			LDPanel.Size = new System.Drawing.Size(1904, 1041);
			LDPanel.TabIndex = 4;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(1904, 1041);
			base.Controls.Add(LDPanel);
			base.Name = "frmViewPhoneAll";
			Text = "frmViewPhoneAll";
			base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(frmViewPhoneAll_FormClosing);
			base.Load += new System.EventHandler(frmViewPhoneAll_Load);
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
