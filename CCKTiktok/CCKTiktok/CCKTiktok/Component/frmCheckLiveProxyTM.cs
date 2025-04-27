using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CCKTiktok.Bussiness;

namespace CCKTiktok.Component
{
	public class frmCheckLiveProxyTM : Form
	{
		private IContainer components = null;

		private DataGridView dataGridView;

		private Button btnAdd;

		private Button btnCheckLive;

		private TextBox txtProxy;

		private Label label1;

		private GroupBox groupBox1;

		private Label lblDie;

		private Label lblLive;

		private Label lblTotal;

		public frmCheckLiveProxyTM()
		{
			InitializeComponent();
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtProxy.Text))
			{
				return;
			}
			string text = txtProxy.Text;
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("#");
			dataTable.Columns.Add("Key");
			dataTable.Columns.Add("Proxy");
			dataTable.Columns.Add("Status");
			dataTable.Columns.Add("Expired");
			List<string> source = text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
			source = source.Distinct().ToList();
			int num = 1;
			foreach (string item in source)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow[0] = num++.ToString();
				dataRow["Key"] = item.ToString();
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			dataGridView.DataSource = dataTable;
			dataGridView.Columns[0].Width = 100;
			dataGridView.Columns[1].Width = 300;
			File.WriteAllLines(CaChuaConstant.TM_Proxy, source);
		}

		private static bool IsProxyAlive(string address, int port)
		{
			try
			{
				using TcpClient tcpClient = new TcpClient();
				tcpClient.Connect(address, port);
				return true;
			}
			catch (Exception ex)
			{
				File.AppendAllLines("proxyerr.txt", new List<string>
				{
					ex.Message,
					ex.StackTrace,
					address,
					port.ToString()
				});
				return false;
			}
		}

		private static bool IsProxyAlive(string address, int port, string username, string password)
		{
			try
			{
				using TcpClient tcpClient = new TcpClient();
				tcpClient.Connect(address, port);
				NetworkStream stream = tcpClient.GetStream();
				string s = username + ":" + password;
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				string text = Convert.ToBase64String(bytes);
				byte[] bytes2 = Encoding.UTF8.GetBytes("GET / HTTP/1.1\r\nHost: " + address + "\r\nProxy-Authorization: Basic " + text);
				stream.Write(bytes2, 0, bytes2.Length);
				return true;
			}
			catch (Exception ex)
			{
				File.AppendAllLines("proxyerr.txt", new List<string>
				{
					ex.Message,
					ex.StackTrace,
					address,
					port.ToString()
				});
				return false;
			}
		}

		public static bool SoketConnect(string proxyAddress, int proxyPort, string proxyUsername, string proxyPassword)
		{
			new WebProxy(proxyAddress, proxyPort);
			if (!string.IsNullOrEmpty(proxyUsername) && !string.IsNullOrEmpty(proxyPassword))
			{
				return IsProxyAlive(proxyAddress, proxyPort, proxyUsername, proxyPassword);
			}
			return IsProxyAlive(proxyAddress, proxyPort);
		}

		private async void btnCheckLive_Click(object sender, EventArgs e)
		{
			btnAdd_Click(null, null);
			DataGridViewRowCollection rows = dataGridView.Rows;
			List<Task> tasks = new List<Task>();
			foreach (DataGridViewRow row in (IEnumerable)rows)
			{
				string proxy = row.Cells["Key"].Value.ToString();
				if (proxy != "")
				{
					Task t = Task.Run(delegate
					{
						TMProxy tMProxy = new TMProxy(proxy, "");
						TMProxyResult newProxy = tMProxy.GetNewProxy();
						row.Cells[2].Value = newProxy.data.https;
						row.Cells[4].Value = newProxy.data.expired_at;
						row.Cells[3].Value = newProxy.data.location_name;
						row.Selected = false;
					});
					tasks.Add(t);
				}
			}
			await Task.WhenAll(tasks);
			await Task.Run(delegate
			{
				int num = 0;
				int num2 = 0;
				foreach (DataGridViewRow item in (IEnumerable)dataGridView.Rows)
				{
					if (item.Cells["Status"].Value != null && item.Cells["Status"].Value.ToString().Equals("Live"))
					{
						num++;
					}
					else if (item.Cells["Status"].Value != null && item.Cells["Status"].Value.ToString().Equals("Die"))
					{
						num2++;
					}
				}
				lblTotal.Text = $"Total: {num + num2}";
				lblLive.Text = $"Live: {num}";
				lblLive.ForeColor = Color.Green;
				lblDie.Text = $"Die: {num2}";
				lblDie.ForeColor = Color.Red;
			});
		}

		private void frmCheckLiveProxy_Load(object sender, EventArgs e)
		{
			string tM_Proxy = CaChuaConstant.TM_Proxy;
			txtProxy.Text = Utils.ReadTextFile(tM_Proxy);
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
			dataGridView = new System.Windows.Forms.DataGridView();
			btnAdd = new System.Windows.Forms.Button();
			btnCheckLive = new System.Windows.Forms.Button();
			txtProxy = new System.Windows.Forms.TextBox();
			label1 = new System.Windows.Forms.Label();
			groupBox1 = new System.Windows.Forms.GroupBox();
			lblDie = new System.Windows.Forms.Label();
			lblLive = new System.Windows.Forms.Label();
			lblTotal = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
			groupBox1.SuspendLayout();
			SuspendLayout();
			dataGridView.AllowUserToAddRows = false;
			dataGridView.AllowUserToDeleteRows = false;
			dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridView.Location = new System.Drawing.Point(12, 303);
			dataGridView.Name = "dataGridView";
			dataGridView.ReadOnly = true;
			dataGridView.RowHeadersVisible = false;
			dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			dataGridView.Size = new System.Drawing.Size(793, 279);
			dataGridView.TabIndex = 0;
			btnAdd.Location = new System.Drawing.Point(261, 251);
			btnAdd.Name = "btnAdd";
			btnAdd.Size = new System.Drawing.Size(115, 46);
			btnAdd.TabIndex = 1;
			btnAdd.Text = "Save";
			btnAdd.UseVisualStyleBackColor = true;
			btnAdd.Click += new System.EventHandler(btnAdd_Click);
			btnCheckLive.Location = new System.Drawing.Point(419, 251);
			btnCheckLive.Name = "btnCheckLive";
			btnCheckLive.Size = new System.Drawing.Size(115, 46);
			btnCheckLive.TabIndex = 1;
			btnCheckLive.Text = "Check Live Proxy";
			btnCheckLive.UseVisualStyleBackColor = true;
			btnCheckLive.Click += new System.EventHandler(btnCheckLive_Click);
			txtProxy.Location = new System.Drawing.Point(12, 32);
			txtProxy.MaxLength = 327670000;
			txtProxy.Multiline = true;
			txtProxy.Name = "txtProxy";
			txtProxy.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			txtProxy.Size = new System.Drawing.Size(793, 208);
			txtProxy.TabIndex = 2;
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(12, 9);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(72, 13);
			label1.TabIndex = 3;
			label1.Text = "TM Proxy API";
			groupBox1.Controls.Add(lblDie);
			groupBox1.Controls.Add(lblLive);
			groupBox1.Controls.Add(lblTotal);
			groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
			groupBox1.Location = new System.Drawing.Point(0, 588);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(827, 46);
			groupBox1.TabIndex = 4;
			groupBox1.TabStop = false;
			lblDie.AutoSize = true;
			lblDie.Location = new System.Drawing.Point(394, 21);
			lblDie.Name = "lblDie";
			lblDie.Size = new System.Drawing.Size(35, 13);
			lblDie.TabIndex = 2;
			lblDie.Text = "Die: 0";
			lblLive.AutoSize = true;
			lblLive.Location = new System.Drawing.Point(202, 21);
			lblLive.Name = "lblLive";
			lblLive.Size = new System.Drawing.Size(39, 13);
			lblLive.TabIndex = 1;
			lblLive.Text = "Live: 0";
			lblTotal.AutoSize = true;
			lblTotal.Location = new System.Drawing.Point(15, 21);
			lblTotal.Name = "lblTotal";
			lblTotal.Size = new System.Drawing.Size(46, 13);
			lblTotal.TabIndex = 0;
			lblTotal.Text = "Total : 0";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(827, 634);
			base.Controls.Add(groupBox1);
			base.Controls.Add(label1);
			base.Controls.Add(txtProxy);
			base.Controls.Add(btnCheckLive);
			base.Controls.Add(btnAdd);
			base.Controls.Add(dataGridView);
			base.Name = "frmCheckLiveProxyTM";
			Text = "Proxy List";
			base.Load += new System.EventHandler(frmCheckLiveProxy_Load);
			((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
