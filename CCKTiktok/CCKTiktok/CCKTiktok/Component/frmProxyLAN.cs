using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using CCKTiktok.Bussiness;
using CCKTiktok.Entity;
using CCKTiktok.Helper;

namespace CCKTiktok.Component
{
	public class frmProxyLAN : Form
	{
		private IContainer components = null;

		private Button btnMap;

		private TextBox txtProxy;

		private TextBox txtDevices;

		private Label label1;

		private Button btnDeviceList;

		private DataGridView dataGridViewProxy;

		private Label label2;

		private TextBox txtProxyServer;

		private Label lblCount;

		private RadioButton rbtNewVersion;

		private RadioButton rbtOldVersion;

		private CheckBox rbtRotate;

		private Label label3;

		private CheckBox cbxMulti;

		private Button btnCheck;

		public frmProxyLAN()
		{
			InitializeComponent();
		}

		private void btnMap_Click(object sender, EventArgs e)
		{
			if (txtProxyServer.Text.Trim() == "")
			{
				MessageBox.Show("Proxy Server is not Empty");
				txtProxyServer.Focus();
			}
			else if (!(txtProxy.Text.Trim() == ""))
			{
				string[] array = txtProxy.Text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				string[] array2 = txtDevices.Text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				List<DeviceProxy> list = new List<DeviceProxy>();
				for (int i = 0; i < array2.Length; i++)
				{
					DeviceProxy deviceProxy = new DeviceProxy();
					deviceProxy.Stt = i + 1;
					deviceProxy.DeviceId = array2[i];
					deviceProxy.Proxy = new ProxyInfo((array.Length > i) ? array[i] : "");
					deviceProxy.ProxyServer = txtProxyServer.Text.Trim();
					deviceProxy.IsRotate = rbtRotate.Checked;
					deviceProxy.MultipleDevice = cbxMulti.Checked;
					list.Add(deviceProxy);
				}
				string contents = new JavaScriptSerializer().Serialize(list);
				File.WriteAllText(CaChuaConstant.XPROXY_LAN, contents);
				File.WriteAllText(CaChuaConstant.XPROXY_LAN_API, new JavaScriptSerializer().Serialize((!rbtOldVersion.Checked) ? Xproxy_API.New : Xproxy_API.Old));
				dataGridViewProxy.DataSource = list;
				Close();
			}
			else
			{
				MessageBox.Show("Proxy is not Empty");
				txtProxyServer.Focus();
			}
		}

		private string DataTable2Json(DataTable dt)
		{
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
			foreach (DataRow row in dt.Rows)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				foreach (DataColumn column in dt.Columns)
				{
					dictionary.Add(column.ColumnName, row[column]);
				}
				list.Add(dictionary);
			}
			return javaScriptSerializer.Serialize(list);
		}

		private void frmProxyLAN_Load(object sender, EventArgs e)
		{
			if (!File.Exists(CaChuaConstant.XPROXY_LAN))
			{
				return;
			}
			try
			{
				btnDeviceList_Click(null, null);
				string[] source = txtDevices.Text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				List<DeviceProxy> list = new JavaScriptSerializer().Deserialize<List<DeviceProxy>>(Utils.ReadTextFile(CaChuaConstant.XPROXY_LAN));
				List<string> list2 = new List<string>();
				if (list.Count > 0)
				{
					txtProxyServer.Text = list[0].ProxyServer;
					foreach (DeviceProxy item in list)
					{
						if (source.Contains(item.DeviceId))
						{
							list2.Add(item.Proxy.Ip + ":" + item.Proxy.Port);
							rbtRotate.Checked = item.IsRotate;
						}
					}
				}
				dataGridViewProxy.DataSource = list;
				txtProxy.Text = string.Join(Environment.NewLine, list2);
				if (File.Exists(CaChuaConstant.XPROXY_LAN_API))
				{
					Xproxy_API xproxy_API = new JavaScriptSerializer().Deserialize<Xproxy_API>(Utils.ReadTextFile(CaChuaConstant.XPROXY_LAN_API));
					rbtOldVersion.Checked = xproxy_API == Xproxy_API.Old;
					rbtNewVersion.Checked = xproxy_API == Xproxy_API.New;
				}
			}
			catch
			{
			}
		}

		private void btnDeviceList_Click(object sender, EventArgs e)
		{
			List<string> listSerialDevice = ADBHelperCCK.GetListSerialDevice();
			txtDevices.Text = string.Join(Environment.NewLine, listSerialDevice);
			lblCount.Text = listSerialDevice.Count.ToString();
		}

		private void txtDevices_TextChanged(object sender, EventArgs e)
		{
		}

		private void btnGetProxy_Click(object sender, EventArgs e)
		{
			string text = txtProxyServer.Text;
			if (!text.StartsWith("http"))
			{
				text = "http://" + text + "/proxy_list";
			}
			string input = new WebClient().DownloadString(text);
			dynamic val = new JavaScriptSerializer().DeserializeObject(input);
			foreach (dynamic item in val)
			{
				foreach (dynamic item2 in item)
				{
					if (item2.ContainsKey("ip"))
					{
						_ = item2["ip"];
					}
				}
			}
		}

		private void txtProxyServer_TextChanged(object sender, EventArgs e)
		{
			string text = txtProxyServer.Text.Trim();
			text = text.Replace("http://", "");
			text = text.Replace("https://", "");
			text = text.Replace("/", "");
			if (text.Length > 8 && text.Contains(":"))
			{
				Regex regex = new Regex("([0-9]+)\\.([0-9]+)\\.([0-9]+)\\.([0-9]+):([0-9]+)");
				if (regex.Match(text).Success)
				{
					txtProxyServer.Text = regex.Match(text).Groups[0].Value;
				}
			}
			else
			{
				Regex regex = new Regex("([0-9]+)\\.([0-9]+)\\.([0-9]+)\\.([0-9]+)");
				if (regex.Match(text).Success)
				{
					txtProxyServer.Text = regex.Match(text).Groups[0].Value;
				}
				else
				{
					txtProxyServer.Text = text;
				}
			}
		}

		private void txtProxy_TextChanged(object sender, EventArgs e)
		{
			label3.Text = txtProxy.Text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length.ToString();
		}

		private void rbtNewVersion_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void rbtOldVersion_CheckedChanged(object sender, EventArgs e)
		{
		}

		public static bool SoketConnect(string proxyAddress, int proxyPort, string proxyUsername, string proxyPassword)
		{
			WebProxy webProxy = new WebProxy(proxyAddress, proxyPort);
			if (!string.IsNullOrEmpty(proxyUsername) && !string.IsNullOrEmpty(proxyPassword))
			{
				webProxy.Credentials = new NetworkCredential(proxyUsername, proxyPassword);
			}
			WebClient webClient = new WebClient();
			webClient.Proxy = webProxy;
			try
			{
				string address = "https://cck.vn";
				webClient.DownloadString(address);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private void btnCheck_Click(object sender, EventArgs e)
		{
			DataGridViewRowCollection rows = dataGridViewProxy.Rows;
			for (int i = 0; i < rows.Count; i++)
			{
				DataGridViewRow row = rows[i];
				string text = row.Cells["Proxy"].Value.ToString();
				string[] myP = text.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				if (myP.Length > 1)
				{
					Task.Run(delegate
					{
						bool flag = SoketConnect(myP[0], int.Parse(myP[1]), (myP.Length > 2) ? myP[2] : "", (myP.Length > 3) ? myP[3] : "");
						row.DefaultCellStyle.ForeColor = (flag ? Color.Green : Color.Red);
						row.Selected = false;
					});
				}
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
			btnMap = new System.Windows.Forms.Button();
			txtProxy = new System.Windows.Forms.TextBox();
			txtDevices = new System.Windows.Forms.TextBox();
			label1 = new System.Windows.Forms.Label();
			btnDeviceList = new System.Windows.Forms.Button();
			dataGridViewProxy = new System.Windows.Forms.DataGridView();
			label2 = new System.Windows.Forms.Label();
			txtProxyServer = new System.Windows.Forms.TextBox();
			lblCount = new System.Windows.Forms.Label();
			rbtNewVersion = new System.Windows.Forms.RadioButton();
			rbtOldVersion = new System.Windows.Forms.RadioButton();
			rbtRotate = new System.Windows.Forms.CheckBox();
			label3 = new System.Windows.Forms.Label();
			cbxMulti = new System.Windows.Forms.CheckBox();
			btnCheck = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)dataGridViewProxy).BeginInit();
			SuspendLayout();
			btnMap.Location = new System.Drawing.Point(40, 301);
			btnMap.Name = "btnMap";
			btnMap.Size = new System.Drawing.Size(256, 24);
			btnMap.TabIndex = 6;
			btnMap.Text = "Save (Link Proxy to the device)";
			btnMap.UseVisualStyleBackColor = true;
			btnMap.Click += new System.EventHandler(btnMap_Click);
			txtProxy.Location = new System.Drawing.Point(40, 83);
			txtProxy.MaxLength = 3276700;
			txtProxy.Multiline = true;
			txtProxy.Name = "txtProxy";
			txtProxy.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			txtProxy.Size = new System.Drawing.Size(557, 210);
			txtProxy.TabIndex = 9;
			txtProxy.TextChanged += new System.EventHandler(txtProxy_TextChanged);
			txtDevices.BackColor = System.Drawing.Color.White;
			txtDevices.Location = new System.Drawing.Point(603, 83);
			txtDevices.MaxLength = 3276700;
			txtDevices.Multiline = true;
			txtDevices.Name = "txtDevices";
			txtDevices.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			txtDevices.Size = new System.Drawing.Size(256, 210);
			txtDevices.TabIndex = 10;
			txtDevices.TextChanged += new System.EventHandler(txtDevices_TextChanged);
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(39, 54);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(52, 13);
			label1.TabIndex = 11;
			label1.Text = "Proxy List";
			btnDeviceList.Location = new System.Drawing.Point(603, 51);
			btnDeviceList.Name = "btnDeviceList";
			btnDeviceList.Size = new System.Drawing.Size(117, 23);
			btnDeviceList.TabIndex = 13;
			btnDeviceList.Text = "Get Devices List";
			btnDeviceList.UseVisualStyleBackColor = true;
			btnDeviceList.Click += new System.EventHandler(btnDeviceList_Click);
			dataGridViewProxy.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			dataGridViewProxy.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridViewProxy.Location = new System.Drawing.Point(40, 334);
			dataGridViewProxy.Name = "dataGridViewProxy";
			dataGridViewProxy.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			dataGridViewProxy.Size = new System.Drawing.Size(819, 179);
			dataGridViewProxy.TabIndex = 14;
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(39, 27);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(67, 13);
			label2.TabIndex = 15;
			label2.Text = "Proxy Server";
			txtProxyServer.Location = new System.Drawing.Point(137, 25);
			txtProxyServer.Name = "txtProxyServer";
			txtProxyServer.Size = new System.Drawing.Size(722, 20);
			txtProxyServer.TabIndex = 16;
			txtProxyServer.TextChanged += new System.EventHandler(txtProxyServer_TextChanged);
			lblCount.AutoSize = true;
			lblCount.Location = new System.Drawing.Point(726, 56);
			lblCount.Name = "lblCount";
			lblCount.Size = new System.Drawing.Size(13, 13);
			lblCount.TabIndex = 17;
			lblCount.Text = "0";
			rbtNewVersion.AutoSize = true;
			rbtNewVersion.Location = new System.Drawing.Point(764, 308);
			rbtNewVersion.Name = "rbtNewVersion";
			rbtNewVersion.Size = new System.Drawing.Size(95, 17);
			rbtNewVersion.TabIndex = 18;
			rbtNewVersion.Text = "Reset - API V1";
			rbtNewVersion.UseVisualStyleBackColor = true;
			rbtNewVersion.CheckedChanged += new System.EventHandler(rbtNewVersion_CheckedChanged);
			rbtOldVersion.AutoSize = true;
			rbtOldVersion.Checked = true;
			rbtOldVersion.Location = new System.Drawing.Point(612, 307);
			rbtOldVersion.Name = "rbtOldVersion";
			rbtOldVersion.Size = new System.Drawing.Size(95, 17);
			rbtOldVersion.TabIndex = 19;
			rbtOldVersion.TabStop = true;
			rbtOldVersion.Text = "Reset - API V0";
			rbtOldVersion.UseVisualStyleBackColor = true;
			rbtOldVersion.CheckedChanged += new System.EventHandler(rbtOldVersion_CheckedChanged);
			rbtRotate.AutoSize = true;
			rbtRotate.Checked = true;
			rbtRotate.CheckState = System.Windows.Forms.CheckState.Checked;
			rbtRotate.Location = new System.Drawing.Point(514, 306);
			rbtRotate.Name = "rbtRotate";
			rbtRotate.Size = new System.Drawing.Size(83, 17);
			rbtRotate.TabIndex = 20;
			rbtRotate.Text = "Auto Rotate";
			rbtRotate.UseVisualStyleBackColor = true;
			rbtRotate.Visible = false;
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(119, 56);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(13, 13);
			label3.TabIndex = 21;
			label3.Text = "0";
			cbxMulti.AutoSize = true;
			cbxMulti.Location = new System.Drawing.Point(311, 306);
			cbxMulti.Name = "cbxMulti";
			cbxMulti.Size = new System.Drawing.Size(134, 17);
			cbxMulti.TabIndex = 22;
			cbxMulti.Text = "1 key - multiple phones";
			cbxMulti.UseVisualStyleBackColor = true;
			btnCheck.Location = new System.Drawing.Point(294, 536);
			btnCheck.Name = "btnCheck";
			btnCheck.Size = new System.Drawing.Size(256, 24);
			btnCheck.TabIndex = 23;
			btnCheck.Text = "Check Live Proxy";
			btnCheck.UseVisualStyleBackColor = true;
			btnCheck.Click += new System.EventHandler(btnCheck_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(902, 572);
			base.Controls.Add(btnCheck);
			base.Controls.Add(cbxMulti);
			base.Controls.Add(label3);
			base.Controls.Add(rbtRotate);
			base.Controls.Add(rbtOldVersion);
			base.Controls.Add(rbtNewVersion);
			base.Controls.Add(lblCount);
			base.Controls.Add(txtProxyServer);
			base.Controls.Add(label2);
			base.Controls.Add(dataGridViewProxy);
			base.Controls.Add(btnDeviceList);
			base.Controls.Add(label1);
			base.Controls.Add(txtDevices);
			base.Controls.Add(txtProxy);
			base.Controls.Add(btnMap);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "frmProxyLAN";
			Text = "XProxy LAN";
			base.Load += new System.EventHandler(frmProxyLAN_Load);
			((System.ComponentModel.ISupportInitialize)dataGridViewProxy).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
