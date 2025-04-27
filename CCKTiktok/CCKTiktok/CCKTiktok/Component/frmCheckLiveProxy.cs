using System;
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
	public class frmCheckLiveProxy : Form
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

		private Label label2;

		private GroupBox groupBox2;

		private RadioButton rbtCollege;

		private RadioButton rbtClientNoroot;

		private GroupBox groupBox3;

		private RadioButton rbtSock5;

		private RadioButton rbtProxyClient;

		private Label lblMessage;

		private RadioButton rbtRemovePass;

		public frmCheckLiveProxy()
		{
			InitializeComponent();
			Control.CheckForIllegalCrossThreadCalls = false;
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
			dataTable.Columns.Add("Proxy");
			dataTable.Columns.Add("Status");
			List<string> source = text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
			source = source.Distinct().ToList();
			int num = 1;
			foreach (string item in source)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow[0] = num++.ToString();
				dataRow["Proxy"] = item.ToString();
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			dataGridView.DataSource = dataTable;
			dataGridView.Columns[0].Width = 100;
			dataGridView.Columns[1].Width = 300;
			File.WriteAllLines(CaChuaConstant.STATIC_PROXY, source);
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
			btnCheckLive.Enabled = false;
			btnAdd_Click(null, null);
			DataGridViewRowCollection rows = dataGridView.Rows;
			Queue<DataGridViewRow> queue = new Queue<DataGridViewRow>();
			for (int j = 0; j < rows.Count; j++)
			{
				queue.Enqueue(rows[j]);
			}
			if (rows.Count == 0)
			{
				return;
			}
			int max = ((rows.Count < 20) ? rows.Count : 20);
			Task[] tasks = new Task[max];
			int iTotal = 0;
			int iLive = 0;
			int iDie = 0;
			PrintMessage(iLive, iDie, Color.Red, "Checking...");
			for (int i = 0; i < max; i++)
			{
				tasks[i] = Task.Run(delegate
				{
					while (queue.Count > 0)
					{
						DataGridViewRow row = queue.Dequeue();
						if (row == null || row.Cells[1].Value == null)
						{
							break;
						}
						string text = row.Cells[1].Value.ToString().Split('|')[0];
						string[] array = text.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
						if (array.Length > 1)
						{
							iTotal++;
							bool ret = false;
							try
							{
								ret = SoketConnect(array[0], int.Parse(array[1]), (array.Length > 2) ? array[2] : "", (array.Length > 3) ? array[3] : "");
							}
							catch
							{
							}
							if (dataGridView.InvokeRequired)
							{
								dataGridView.Invoke((Action)delegate
								{
									dataGridView.Rows[row.Index].DefaultCellStyle.ForeColor = (ret ? Color.Green : Color.Red);
									dataGridView.Rows[row.Index].Cells[2].Value = (ret ? "Live" : "Die");
									if (ret)
									{
										int num = iLive;
										iLive = num + 1;
									}
									else
									{
										int num = iDie;
										iDie = num + 1;
									}
								});
							}
							else
							{
								dataGridView.Rows[row.Index].DefaultCellStyle.ForeColor = (ret ? Color.Green : Color.Red);
								dataGridView.Rows[row.Index].Cells[2].Value = (ret ? "Live" : "Die");
								if (ret)
								{
									iLive++;
								}
								else
								{
									iDie++;
								}
							}
							PrintMessage(iLive, iDie, Color.Green, "Checking...");
						}
					}
				});
			}
			await Task.WhenAll(tasks);
			PrintMessage(iLive, iDie, Color.Green, "Done!");
			btnCheckLive.Enabled = true;
		}

		private void PrintMessage(int iLive, int iDie, Color color, string msg)
		{
			Invoke((MethodInvoker)delegate
			{
				lblTotal.Text = $"Total: {iLive + iDie}";
				lblLive.Text = $"Live: {iLive}";
				lblLive.ForeColor = Color.Green;
				lblDie.Text = $"Die: {iDie}";
				lblDie.ForeColor = Color.Red;
				lblMessage.Text = msg;
				lblMessage.ForeColor = color;
			});
		}

		private void frmCheckLiveProxy_Load(object sender, EventArgs e)
		{
			string sTATIC_PROXY = CaChuaConstant.STATIC_PROXY;
			txtProxy.Text = Utils.ReadTextFile(sTATIC_PROXY);
			string text = Utils.ReadTextFile(CaChuaConstant.PROXY_APP);
			if (!(text == "ip_port"))
			{
				if (!(text == "college"))
				{
					rbtClientNoroot.Checked = true;
				}
				else
				{
					rbtCollege.Checked = true;
				}
			}
			else
			{
				rbtRemovePass.Checked = true;
			}
			bool flag = Utils.ReadTextFile(CaChuaConstant.PROXY_TYPE) == "sock5";
			rbtSock5.Checked = flag;
			rbtProxyClient.Checked = !flag;
		}

		private void rbtCollege_CheckedChanged(object sender, EventArgs e)
		{
			if (rbtCollege.Checked)
			{
				File.WriteAllText(CaChuaConstant.PROXY_APP, "college");
				rbtSock5.Enabled = false;
				rbtSock5.Checked = false;
				rbtProxyClient.Checked = true;
			}
		}

		private void rbtClientNoroot_CheckedChanged(object sender, EventArgs e)
		{
			if (rbtClientNoroot.Checked)
			{
				rbtSock5.Enabled = true;
				rbtSock5.Checked = false;
				File.WriteAllText(CaChuaConstant.PROXY_APP, "clientnoroot");
			}
		}

		private void rbtSock5_CheckedChanged(object sender, EventArgs e)
		{
			if (rbtSock5.Checked)
			{
				File.WriteAllText(CaChuaConstant.PROXY_TYPE, "sock5");
			}
		}

		private void rbtProxyClient_CheckedChanged(object sender, EventArgs e)
		{
			if (rbtProxyClient.Checked)
			{
				File.WriteAllText(CaChuaConstant.PROXY_TYPE, "http");
			}
		}

		private void rbtRemovePass_CheckedChanged(object sender, EventArgs e)
		{
			if (rbtRemovePass.Checked)
			{
				File.WriteAllText(CaChuaConstant.PROXY_APP, "ip_port");
				rbtSock5.Enabled = false;
				rbtSock5.Checked = false;
				rbtProxyClient.Checked = false;
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
			dataGridView = new System.Windows.Forms.DataGridView();
			btnAdd = new System.Windows.Forms.Button();
			btnCheckLive = new System.Windows.Forms.Button();
			txtProxy = new System.Windows.Forms.TextBox();
			label1 = new System.Windows.Forms.Label();
			groupBox1 = new System.Windows.Forms.GroupBox();
			lblMessage = new System.Windows.Forms.Label();
			lblDie = new System.Windows.Forms.Label();
			lblLive = new System.Windows.Forms.Label();
			lblTotal = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			groupBox2 = new System.Windows.Forms.GroupBox();
			rbtCollege = new System.Windows.Forms.RadioButton();
			rbtClientNoroot = new System.Windows.Forms.RadioButton();
			groupBox3 = new System.Windows.Forms.GroupBox();
			rbtSock5 = new System.Windows.Forms.RadioButton();
			rbtProxyClient = new System.Windows.Forms.RadioButton();
			rbtRemovePass = new System.Windows.Forms.RadioButton();
			((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
			groupBox1.SuspendLayout();
			groupBox2.SuspendLayout();
			groupBox3.SuspendLayout();
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
			btnAdd.Location = new System.Drawing.Point(12, 251);
			btnAdd.Name = "btnAdd";
			btnAdd.Size = new System.Drawing.Size(115, 46);
			btnAdd.TabIndex = 1;
			btnAdd.Text = "Save";
			btnAdd.UseVisualStyleBackColor = true;
			btnAdd.Click += new System.EventHandler(btnAdd_Click);
			btnCheckLive.Location = new System.Drawing.Point(146, 251);
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
			label1.Size = new System.Drawing.Size(136, 13);
			label1.TabIndex = 3;
			label1.Text = "IP:Port  / IP:Port:User:Pass";
			groupBox1.Controls.Add(lblMessage);
			groupBox1.Controls.Add(lblDie);
			groupBox1.Controls.Add(lblLive);
			groupBox1.Controls.Add(lblTotal);
			groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
			groupBox1.Location = new System.Drawing.Point(0, 588);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(827, 46);
			groupBox1.TabIndex = 4;
			groupBox1.TabStop = false;
			lblMessage.AutoSize = true;
			lblMessage.Location = new System.Drawing.Point(522, 20);
			lblMessage.Name = "lblMessage";
			lblMessage.Size = new System.Drawing.Size(37, 13);
			lblMessage.TabIndex = 2;
			lblMessage.Text = "Status";
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
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(624, 9);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(181, 13);
			label2.TabIndex = 5;
			label2.Text = "IP:Port:User:Pass | ResetLink | Delay";
			groupBox2.Controls.Add(rbtCollege);
			groupBox2.Controls.Add(rbtRemovePass);
			groupBox2.Controls.Add(rbtClientNoroot);
			groupBox2.Location = new System.Drawing.Point(289, 246);
			groupBox2.Name = "groupBox2";
			groupBox2.Size = new System.Drawing.Size(362, 51);
			groupBox2.TabIndex = 171;
			groupBox2.TabStop = false;
			groupBox2.Text = "Proxy App";
			rbtCollege.AutoSize = true;
			rbtCollege.Checked = true;
			rbtCollege.Location = new System.Drawing.Point(16, 19);
			rbtCollege.Name = "rbtCollege";
			rbtCollege.Size = new System.Drawing.Size(89, 17);
			rbtCollege.TabIndex = 151;
			rbtCollege.TabStop = true;
			rbtCollege.Text = "College Proxy";
			rbtCollege.UseVisualStyleBackColor = true;
			rbtCollege.CheckedChanged += new System.EventHandler(rbtCollege_CheckedChanged);
			rbtClientNoroot.AutoSize = true;
			rbtClientNoroot.Location = new System.Drawing.Point(111, 20);
			rbtClientNoroot.Name = "rbtClientNoroot";
			rbtClientNoroot.Size = new System.Drawing.Size(94, 17);
			rbtClientNoroot.TabIndex = 152;
			rbtClientNoroot.Text = "Client No Root";
			rbtClientNoroot.UseVisualStyleBackColor = true;
			rbtClientNoroot.CheckedChanged += new System.EventHandler(rbtClientNoroot_CheckedChanged);
			groupBox3.Controls.Add(rbtSock5);
			groupBox3.Controls.Add(rbtProxyClient);
			groupBox3.Location = new System.Drawing.Point(657, 246);
			groupBox3.Name = "groupBox3";
			groupBox3.Size = new System.Drawing.Size(148, 51);
			groupBox3.TabIndex = 172;
			groupBox3.TabStop = false;
			groupBox3.Text = "Proxy Type";
			rbtSock5.AutoSize = true;
			rbtSock5.Location = new System.Drawing.Point(6, 22);
			rbtSock5.Name = "rbtSock5";
			rbtSock5.Size = new System.Drawing.Size(67, 17);
			rbtSock5.TabIndex = 151;
			rbtSock5.Text = "SOCKS5";
			rbtSock5.UseVisualStyleBackColor = true;
			rbtSock5.CheckedChanged += new System.EventHandler(rbtSock5_CheckedChanged);
			rbtProxyClient.AutoSize = true;
			rbtProxyClient.Checked = true;
			rbtProxyClient.Location = new System.Drawing.Point(79, 22);
			rbtProxyClient.Name = "rbtProxyClient";
			rbtProxyClient.Size = new System.Drawing.Size(54, 17);
			rbtProxyClient.TabIndex = 152;
			rbtProxyClient.TabStop = true;
			rbtProxyClient.Text = "HTTP";
			rbtProxyClient.UseVisualStyleBackColor = true;
			rbtProxyClient.CheckedChanged += new System.EventHandler(rbtProxyClient_CheckedChanged);
			rbtRemovePass.AutoSize = true;
			rbtRemovePass.Location = new System.Drawing.Point(211, 20);
			rbtRemovePass.Name = "rbtRemovePass";
			rbtRemovePass.Size = new System.Drawing.Size(119, 17);
			rbtRemovePass.TabIndex = 152;
			rbtRemovePass.Text = "Không sử dụng App";
			rbtRemovePass.UseVisualStyleBackColor = true;
			rbtRemovePass.Visible = false;
			rbtRemovePass.CheckedChanged += new System.EventHandler(rbtRemovePass_CheckedChanged);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(827, 634);
			base.Controls.Add(groupBox2);
			base.Controls.Add(groupBox3);
			base.Controls.Add(label2);
			base.Controls.Add(groupBox1);
			base.Controls.Add(label1);
			base.Controls.Add(txtProxy);
			base.Controls.Add(btnCheckLive);
			base.Controls.Add(btnAdd);
			base.Controls.Add(dataGridView);
			base.Name = "frmCheckLiveProxy";
			Text = "Proxy List";
			base.Load += new System.EventHandler(frmCheckLiveProxy_Load);
			((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			groupBox3.ResumeLayout(false);
			groupBox3.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
