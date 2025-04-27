using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CCKTiktok.Bussiness;
using CCKTiktok.DAL;
using CCKTiktok.Helper;
using OpenQA.Selenium;

namespace CCKTiktok.Component
{
	public class frmUnlockHotmail : Form
	{
		private DataTable tbl = new DataTable();

		private List<DataGridViewRow> SelectedRow = new List<DataGridViewRow>();

		private IContainer components = null;

		private DataGridView dataGridView1;

		private Button btnUnlock;

		private Button btnTesst;

		private GroupBox groupBox1;

		private Button button1;

		private TextBox txtChangePass;

		private CheckBox cbxChagnePasss;

		private LinkLabel linkLabel1;

		private Button btnImport;

		private Label label1;

		private ComboBox cbxThread;

		private Button button2;

		private Label label2;

		private Button button3;

		private CheckBox cbxRandom;

		private CheckBox cbxAddRecover;

		private Button button4;

		private Button button5;

		private Label lblTotal;

		private Label lblLive;

		private Label lblDie;

		private LinkLabel linkLabel2;

		private Label lblCheck;

		private StatusStrip statusStrip1;

		private ToolStripProgressBar toolStripProgressBar1;

		private ToolStripStatusLabel toolStripStatusLabel;

		public frmUnlockHotmail()
		{
			InitializeComponent();
			Control.CheckForIllegalCrossThreadCalls = false;
		}

		public frmUnlockHotmail(DataTable tbl)
		{
			this.tbl = tbl;
			InitializeComponent();
		}

		private void frmUnlockHotmail_Load(object sender, EventArgs e)
		{
			dataGridView1.DataSource = tbl;
			cbxThread.SelectedIndex = 3;
			SQLiteUtils sQLiteUtils = new SQLiteUtils();
			DataTable dataTable = sQLiteUtils.ExecuteQuery("Select * from Account limit 1");
			if (dataTable != null && !dataTable.Columns.Contains("EmailRecovery"))
			{
				sQLiteUtils.ExecuteQuery("ALTER TABLE Account ADD COLUMN EmailRecovery TEXT default null");
			}
			dataTable = null;
			lblTotal.Text = $"Total: {((tbl != null) ? tbl.Rows.Count : 0)}";
		}

		private void btnUnlock_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				List<DataGridViewRow> SelectedRow = dataGridView1.SelectedRows.Cast<DataGridViewRow>().Reverse().ToList();
				Utils.CCKLog("aaa", "aaaa");
				int maxThread = Math.Min(Utils.Convert2Int(cbxThread.SelectedItem.ToString()), SelectedRow.Count);
				if (maxThread == 0)
				{
					maxThread = 1;
				}
				ADBHelperCCK.RunCommand("taskkill /f /im chrome.exe");
				toolStripProgressBar1.Maximum = SelectedRow.Count;
				toolStripProgressBar1.Value = 0;
				int index = 0;
				int total = SelectedRow.Count;
				Invoke((MethodInvoker)delegate
				{
					toolStripStatusLabel.Text = $"Unlocking ... {0}/{total}";
				});
				for (int i = 0; i < maxThread; i++)
				{
					new Task(delegate
					{
						try
						{
							UnlockHotmail unlockHotmail = new UnlockHotmail();
							while (SelectedRow.Count > 0)
							{
								try
								{
									DataGridViewRow dataGridViewRow = SelectedRow[0];
									SelectedRow.RemoveAt(0);
									unlockHotmail.DoUnlock(cbxAddRecover.Checked, dataGridViewRow, cbxChagnePasss.Checked);
									Invoke((MethodInvoker)delegate
									{
										toolStripStatusLabel.Text = $"Unlocking ... {index++}/{total}";
									});
									if (toolStripProgressBar1.Value < toolStripProgressBar1.Maximum)
									{
										toolStripProgressBar1.Value++;
									}
									if (index % 10 == 0 && index < total - maxThread)
									{
										dataGridView1.FirstDisplayedScrollingRowIndex = dataGridViewRow.Index;
									}
								}
								catch (Exception ex)
								{
									Utils.CCKLog(ex.Message, "DoUnlock Hotmail 1");
								}
								finally
								{
									if (index == total)
									{
										Invoke((MethodInvoker)delegate
										{
											toolStripStatusLabel.Text = $"Done ... {index}/{total}";
										});
									}
								}
							}
						}
						catch (Exception ex2)
						{
							Utils.CCKLog(ex2.Message, "DoUnlock Hotmail 2");
						}
					}).Start();
					Thread.Sleep(1000);
				}
			}).Start();
		}

		private void btnTesst_Click(object sender, EventArgs e)
		{
			new List<string>();
			new Task(delegate
			{
				lblCheck.Text = "";
				List<DataGridViewRow> rows = dataGridView1.SelectedRows.Cast<DataGridViewRow>().Reverse().ToList();
				dataGridView1.ClearSelection();
				DongVanFB d = new DongVanFB("");
				int index = 0;
				int total = rows.Count;
				toolStripProgressBar1.Maximum = total;
				toolStripProgressBar1.Value = 0;
				int threadCount = 20;
				for (int i = 0; i < threadCount; i++)
				{
					new Thread((ThreadStart)delegate
					{
						SQLiteUtils sQLiteUtils = new SQLiteUtils();
						sQLiteUtils.OpenConnection();
						while (rows.Count > index)
						{
							DataGridViewRow dataGridViewRow = rows[index];
							index++;
							try
							{
								string status = "";
								if (index % threadCount == 0 && index < rows.Count - threadCount)
								{
									dataGridView1.FirstDisplayedScrollingRowIndex = dataGridViewRow.Index;
								}
								try
								{
									dataGridViewRow.Cells["Note"].Value = "Starting...";
								}
								catch
								{
								}
								d.GetEmail(dataGridViewRow.Cells["Email"].Value.ToString(), dataGridViewRow.Cells["Pass"].Value.ToString(), out status, loginOnly: true);
								dataGridViewRow.Cells["Note"].Value = status;
								sQLiteUtils.BatchUpdateLog(dataGridViewRow.Cells[1].Value.ToString(), status.Equals("Success") ? "Email Live" : "Email Die");
								Invoke((MethodInvoker)delegate
								{
									toolStripStatusLabel.Text = $"Checking ... {index}/{total}";
								});
								if (toolStripProgressBar1.Value < toolStripProgressBar1.Maximum)
								{
									toolStripProgressBar1.Value++;
								}
							}
							catch (Exception)
							{
							}
							finally
							{
								if (index == rows.Count)
								{
									Invoke((MethodInvoker)delegate
									{
										toolStripStatusLabel.Text = $"Done ... {index}/{total}";
									});
								}
							}
						}
						sQLiteUtils.CloseConnection();
					}).Start();
					Thread.Sleep(500);
				}
			}).Start();
		}

		private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
		}

		private void button1_Click_1(object sender, EventArgs e)
		{
			_ = dataGridView1.SelectedRows;
			List<string> list = new List<string>();
			for (int i = 0; i < dataGridView1.SelectedRows.Count; i++)
			{
				DataGridViewRow dataGridViewRow = dataGridView1.SelectedRows[i];
				if (dataGridViewRow.Cells["Id"].Value != null)
				{
					list.Add(dataGridViewRow.Cells["Id"].Value.ToString());
				}
			}
			Clipboard.SetText(string.Join(Environment.NewLine, list));
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://www.youtube.com/watch?v=sJVfGXYzRH8&t=61s");
		}

		private void btnImport_Click(object sender, EventArgs e)
		{
			frmInputControl frmInputControl2 = new frmInputControl(isSingleLine: false, "Input list of hotmail", "Hotmail");
			frmInputControl2.ShowDialog();
			if (frmInputControl2.Result != "")
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
				string[] array = frmInputControl2.Result.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				string[] array2 = array;
				foreach (string text in array2)
				{
					string[] array3 = text.Split('|');
					DataRow dataRow = dataTable.NewRow();
					dataRow["Stt"] = ++num;
					dataRow["id"] = array3[0].Trim();
					dataRow["Email"] = array3[0].Trim();
					dataRow["Pass"] = array3[1].Trim();
					dataTable.Rows.Add(dataRow);
					dataTable.AcceptChanges();
				}
				tbl = dataTable;
				frmUnlockHotmail_Load(null, null);
			}
		}

		private void groupBox1_Enter(object sender, EventArgs e)
		{
		}

		private void button2_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				int num = 1;
				string[] directories = Directory.GetDirectories(SQLiteUtils.data_folder + "\\ChromeProfile\\");
				string[] array = directories;
				foreach (string d in array)
				{
					try
					{
						button2.Text = $"Deleted {num++}/{directories.Length}";
						Application.DoEvents();
						Utils.ClearFolder(d);
					}
					catch
					{
					}
				}
			}).Start();
		}

		private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Right)
			{
				return;
			}
			ContextMenu i = new ContextMenu();
			MenuItem menuItem = new MenuItem("Login");
			menuItem.Name = "viewinchrome";
			i.MenuItems.Add(menuItem);
			foreach (MenuItem menuItem2 in i.MenuItems)
			{
				menuItem2.Click += FaceBookCreator_Click;
			}
			Invoke((Action)delegate
			{
				i.Show(dataGridView1, new Point(e.X, e.Y));
			});
		}

		private void SrcollTrueView(FBChrome chrome, IWebElement elm)
		{
			((IJavaScriptExecutor)chrome.m_driver).ExecuteScript("arguments[0].scrollIntoView(true);", new object[1] { elm });
			Thread.Sleep(500);
		}

		private void FaceBookCreator_Click(object sender, EventArgs e)
		{
			string name = ((MenuItem)sender).Name;
			new List<string>();
			string text = name;
			string text2 = text;
			if (!(text2 == "viewinchrome"))
			{
				return;
			}
			DataGridViewSelectedRowCollection selectedRows = dataGridView1.SelectedRows;
			if (selectedRows == null || selectedRows.Count <= 0)
			{
				return;
			}
			string uid = selectedRows[0].Cells["Email"].Value.ToString();
			string text3 = selectedRows[0].Cells["Pass"].Value.ToString();
			FBChrome fBChrome = new FBChrome();
			fBChrome.Init("https://login.live.com/", uid);
			if (!fBChrome.m_driver.PageSource.Contains("loginfmt"))
			{
				return;
			}
			IWebElement webElement = fBChrome.m_driver.FindElementByName("loginfmt");
			if (webElement == null)
			{
				return;
			}
			SrcollTrueView(fBChrome, webElement);
			webElement.SendKeys(uid);
			IWebElement webElement2 = fBChrome.m_driver.FindElementByXPath("//*[@value=\"Next\"]");
			if (webElement2 == null)
			{
				return;
			}
			SrcollTrueView(fBChrome, webElement2);
			webElement2.Click();
			int num = 0;
			while (num < 60)
			{
				num++;
				Thread.Sleep(1000);
				ReadOnlyCollection<IWebElement> readOnlyCollection = fBChrome.m_driver.FindElementsByName("passwd");
				if (readOnlyCollection != null && readOnlyCollection.Count > 0)
				{
					SrcollTrueView(fBChrome, readOnlyCollection[0]);
					readOnlyCollection[0].SendKeys(text3);
					break;
				}
			}
			webElement2 = fBChrome.m_driver.FindElementByXPath("//*[@value=\"Sign in\"]");
			if (webElement2 != null)
			{
				SrcollTrueView(fBChrome, webElement2);
				webElement2.Click();
				Thread.Sleep(5000);
			}
		}

		private void cbxRandom_CheckedChanged(object sender, EventArgs e)
		{
			if (!cbxRandom.Checked)
			{
				txtChangePass.Enabled = true;
				return;
			}
			txtChangePass.Text = "";
			txtChangePass.Enabled = false;
		}

		private void button4_Click(object sender, EventArgs e)
		{
			frmVerifyEmail frmVerifyEmail2 = new frmVerifyEmail();
			frmVerifyEmail2.StartPosition = FormStartPosition.CenterScreen;
			frmVerifyEmail2.ShowDialog();
		}

		private void button5_Click(object sender, EventArgs e)
		{
			int count = dataGridView1.Rows.Count;
			for (int num = count - 1; num >= 0; num--)
			{
				DataGridViewRow dataGridViewRow = dataGridView1.Rows[num];
				if (dataGridViewRow.Cells["Status"].Value.ToString().Equals("Success"))
				{
					dataGridView1.Rows.Remove(dataGridViewRow);
				}
			}
		}

		private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			dataGridView1.SelectAll();
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
			dataGridView1 = new System.Windows.Forms.DataGridView();
			btnUnlock = new System.Windows.Forms.Button();
			btnTesst = new System.Windows.Forms.Button();
			groupBox1 = new System.Windows.Forms.GroupBox();
			cbxRandom = new System.Windows.Forms.CheckBox();
			cbxAddRecover = new System.Windows.Forms.CheckBox();
			label2 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			cbxThread = new System.Windows.Forms.ComboBox();
			linkLabel1 = new System.Windows.Forms.LinkLabel();
			txtChangePass = new System.Windows.Forms.TextBox();
			cbxChagnePasss = new System.Windows.Forms.CheckBox();
			button3 = new System.Windows.Forms.Button();
			button2 = new System.Windows.Forms.Button();
			btnImport = new System.Windows.Forms.Button();
			button1 = new System.Windows.Forms.Button();
			button4 = new System.Windows.Forms.Button();
			button5 = new System.Windows.Forms.Button();
			lblTotal = new System.Windows.Forms.Label();
			lblLive = new System.Windows.Forms.Label();
			lblDie = new System.Windows.Forms.Label();
			linkLabel2 = new System.Windows.Forms.LinkLabel();
			lblCheck = new System.Windows.Forms.Label();
			statusStrip1 = new System.Windows.Forms.StatusStrip();
			toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
			toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
			groupBox1.SuspendLayout();
			statusStrip1.SuspendLayout();
			SuspendLayout();
			dataGridView1.AllowUserToAddRows = false;
			dataGridView1.AllowUserToDeleteRows = false;
			dataGridView1.AllowUserToResizeRows = false;
			dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			dataGridView1.BackgroundColor = System.Drawing.Color.White;
			dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridView1.Location = new System.Drawing.Point(12, 125);
			dataGridView1.Margin = new System.Windows.Forms.Padding(3, 83, 3, 3);
			dataGridView1.Name = "dataGridView1";
			dataGridView1.ReadOnly = true;
			dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			dataGridView1.Size = new System.Drawing.Size(984, 502);
			dataGridView1.TabIndex = 0;
			dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(dataGridView1_CellContentClick);
			dataGridView1.MouseClick += new System.Windows.Forms.MouseEventHandler(dataGridView1_MouseClick);
			btnUnlock.BackColor = System.Drawing.SystemColors.ControlDark;
			btnUnlock.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnUnlock.ForeColor = System.Drawing.Color.White;
			btnUnlock.Location = new System.Drawing.Point(9, 643);
			btnUnlock.Name = "btnUnlock";
			btnUnlock.Size = new System.Drawing.Size(142, 36);
			btnUnlock.TabIndex = 2;
			btnUnlock.Text = "Start Unlock";
			btnUnlock.UseVisualStyleBackColor = false;
			btnUnlock.Click += new System.EventHandler(btnUnlock_Click);
			btnTesst.BackColor = System.Drawing.SystemColors.ControlDark;
			btnTesst.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnTesst.ForeColor = System.Drawing.Color.White;
			btnTesst.Location = new System.Drawing.Point(158, 643);
			btnTesst.Name = "btnTesst";
			btnTesst.Size = new System.Drawing.Size(118, 36);
			btnTesst.TabIndex = 3;
			btnTesst.Text = "Check Login Hotmail";
			btnTesst.UseVisualStyleBackColor = false;
			btnTesst.Click += new System.EventHandler(btnTesst_Click);
			groupBox1.Controls.Add(cbxRandom);
			groupBox1.Controls.Add(cbxAddRecover);
			groupBox1.Controls.Add(label2);
			groupBox1.Controls.Add(label1);
			groupBox1.Controls.Add(cbxThread);
			groupBox1.Controls.Add(linkLabel2);
			groupBox1.Controls.Add(linkLabel1);
			groupBox1.Controls.Add(txtChangePass);
			groupBox1.Controls.Add(cbxChagnePasss);
			groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
			groupBox1.Location = new System.Drawing.Point(0, 0);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(1008, 116);
			groupBox1.TabIndex = 4;
			groupBox1.TabStop = false;
			groupBox1.Text = "Config";
			groupBox1.Enter += new System.EventHandler(groupBox1_Enter);
			cbxRandom.AutoSize = true;
			cbxRandom.Location = new System.Drawing.Point(254, 40);
			cbxRandom.Name = "cbxRandom";
			cbxRandom.Size = new System.Drawing.Size(115, 17);
			cbxRandom.TabIndex = 14;
			cbxRandom.Text = "Random Password";
			cbxRandom.UseVisualStyleBackColor = true;
			cbxRandom.CheckedChanged += new System.EventHandler(cbxRandom_CheckedChanged);
			cbxAddRecover.AutoSize = true;
			cbxAddRecover.Checked = true;
			cbxAddRecover.CheckState = System.Windows.Forms.CheckState.Checked;
			cbxAddRecover.Location = new System.Drawing.Point(395, 38);
			cbxAddRecover.Name = "cbxAddRecover";
			cbxAddRecover.Size = new System.Drawing.Size(195, 17);
			cbxAddRecover.TabIndex = 13;
			cbxAddRecover.Text = "Add Recovery Email (Getnada.com)";
			cbxAddRecover.UseVisualStyleBackColor = true;
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(392, 11);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(0, 13);
			label2.TabIndex = 12;
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(821, 41);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(94, 13);
			label1.TabIndex = 10;
			label1.Text = "Number of threads";
			cbxThread.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cbxThread.FormattingEnabled = true;
			cbxThread.Items.AddRange(new object[30]
			{
				"1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
				"11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
				"21", "22", "23", "24", "25", "26", "27", "28", "29", "30"
			});
			cbxThread.Location = new System.Drawing.Point(921, 38);
			cbxThread.Name = "cbxThread";
			cbxThread.Size = new System.Drawing.Size(71, 21);
			cbxThread.TabIndex = 9;
			linkLabel1.AutoSize = true;
			linkLabel1.Location = new System.Drawing.Point(885, 79);
			linkLabel1.Name = "linkLabel1";
			linkLabel1.Size = new System.Drawing.Size(111, 13);
			linkLabel1.TabIndex = 7;
			linkLabel1.TabStop = true;
			linkLabel1.Text = "Xem video hướng dẫn";
			linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel1_LinkClicked);
			txtChangePass.Location = new System.Drawing.Point(135, 38);
			txtChangePass.Name = "txtChangePass";
			txtChangePass.Size = new System.Drawing.Size(112, 20);
			txtChangePass.TabIndex = 6;
			cbxChagnePasss.AutoSize = true;
			cbxChagnePasss.Location = new System.Drawing.Point(20, 41);
			cbxChagnePasss.Name = "cbxChagnePasss";
			cbxChagnePasss.Size = new System.Drawing.Size(112, 17);
			cbxChagnePasss.TabIndex = 5;
			cbxChagnePasss.Text = "Change Password";
			cbxChagnePasss.UseVisualStyleBackColor = true;
			button3.BackColor = System.Drawing.SystemColors.ControlDark;
			button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			button3.ForeColor = System.Drawing.Color.White;
			button3.Location = new System.Drawing.Point(282, 643);
			button3.Name = "button3";
			button3.Size = new System.Drawing.Size(79, 36);
			button3.TabIndex = 16;
			button3.Text = "Export Mail";
			button3.UseVisualStyleBackColor = false;
			button2.BackColor = System.Drawing.SystemColors.ControlDark;
			button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			button2.ForeColor = System.Drawing.Color.White;
			button2.Location = new System.Drawing.Point(596, 643);
			button2.Name = "button2";
			button2.Size = new System.Drawing.Size(146, 36);
			button2.TabIndex = 11;
			button2.Text = "Clear Chrome Profile";
			button2.UseVisualStyleBackColor = false;
			button2.Click += new System.EventHandler(button2_Click);
			btnImport.BackColor = System.Drawing.SystemColors.ControlDark;
			btnImport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			btnImport.ForeColor = System.Drawing.Color.White;
			btnImport.Location = new System.Drawing.Point(367, 643);
			btnImport.Name = "btnImport";
			btnImport.Size = new System.Drawing.Size(86, 36);
			btnImport.TabIndex = 8;
			btnImport.Text = "Import Hotmail";
			btnImport.UseVisualStyleBackColor = false;
			btnImport.Click += new System.EventHandler(btnImport_Click);
			button1.BackColor = System.Drawing.SystemColors.ControlDark;
			button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			button1.ForeColor = System.Drawing.Color.White;
			button1.Location = new System.Drawing.Point(462, 643);
			button1.Name = "button1";
			button1.Size = new System.Drawing.Size(128, 36);
			button1.TabIndex = 4;
			button1.Text = "Copy selected IUD";
			button1.UseVisualStyleBackColor = false;
			button1.Click += new System.EventHandler(button1_Click_1);
			button4.BackColor = System.Drawing.SystemColors.ControlDark;
			button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			button4.ForeColor = System.Drawing.Color.White;
			button4.Location = new System.Drawing.Point(745, 643);
			button4.Name = "button4";
			button4.Size = new System.Drawing.Size(74, 36);
			button4.TabIndex = 4;
			button4.Text = "Config";
			button4.UseVisualStyleBackColor = false;
			button4.Click += new System.EventHandler(button4_Click);
			button5.BackColor = System.Drawing.SystemColors.ControlDark;
			button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			button5.ForeColor = System.Drawing.Color.White;
			button5.Location = new System.Drawing.Point(825, 643);
			button5.Name = "button5";
			button5.Size = new System.Drawing.Size(167, 36);
			button5.TabIndex = 17;
			button5.Text = "Remove Success Email";
			button5.UseVisualStyleBackColor = false;
			button5.Click += new System.EventHandler(button5_Click);
			lblTotal.AutoSize = true;
			lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 254);
			lblTotal.Location = new System.Drawing.Point(12, 707);
			lblTotal.Name = "lblTotal";
			lblTotal.Size = new System.Drawing.Size(36, 13);
			lblTotal.TabIndex = 18;
			lblTotal.Text = "Total";
			lblLive.AutoSize = true;
			lblLive.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 254);
			lblLive.Location = new System.Drawing.Point(151, 707);
			lblLive.Name = "lblLive";
			lblLive.Size = new System.Drawing.Size(14, 13);
			lblLive.TabIndex = 19;
			lblLive.Text = "0";
			lblDie.AutoSize = true;
			lblDie.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 254);
			lblDie.Location = new System.Drawing.Point(251, 707);
			lblDie.Name = "lblDie";
			lblDie.Size = new System.Drawing.Size(14, 13);
			lblDie.TabIndex = 20;
			lblDie.Text = "0";
			linkLabel2.AutoSize = true;
			linkLabel2.Location = new System.Drawing.Point(17, 88);
			linkLabel2.Name = "linkLabel2";
			linkLabel2.Size = new System.Drawing.Size(79, 13);
			linkLabel2.TabIndex = 7;
			linkLabel2.TabStop = true;
			linkLabel2.Text = "Select All Email";
			linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel2_LinkClicked);
			lblCheck.AutoSize = true;
			lblCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 254);
			lblCheck.Location = new System.Drawing.Point(901, 704);
			lblCheck.Name = "lblCheck";
			lblCheck.Size = new System.Drawing.Size(14, 13);
			lblCheck.TabIndex = 21;
			lblCheck.Text = "0";
			statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { toolStripProgressBar1, toolStripStatusLabel });
			statusStrip1.Location = new System.Drawing.Point(0, 707);
			statusStrip1.Name = "statusStrip1";
			statusStrip1.Size = new System.Drawing.Size(1008, 22);
			statusStrip1.TabIndex = 22;
			statusStrip1.Text = "statusStrip1";
			toolStripProgressBar1.Name = "toolStripProgressBar1";
			toolStripProgressBar1.Size = new System.Drawing.Size(600, 16);
			toolStripStatusLabel.Name = "toolStripStatusLabel";
			toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(1008, 729);
			base.Controls.Add(statusStrip1);
			base.Controls.Add(lblCheck);
			base.Controls.Add(lblDie);
			base.Controls.Add(lblLive);
			base.Controls.Add(lblTotal);
			base.Controls.Add(button5);
			base.Controls.Add(button3);
			base.Controls.Add(groupBox1);
			base.Controls.Add(dataGridView1);
			base.Controls.Add(button2);
			base.Controls.Add(button4);
			base.Controls.Add(button1);
			base.Controls.Add(btnUnlock);
			base.Controls.Add(btnTesst);
			base.Controls.Add(btnImport);
			base.MaximizeBox = false;
			MaximumSize = new System.Drawing.Size(1024, 768);
			base.MinimizeBox = false;
			MinimumSize = new System.Drawing.Size(1024, 768);
			base.Name = "frmUnlockHotmail";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "Unlock Hotmail";
			base.Load += new System.EventHandler(frmUnlockHotmail_Load);
			((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			statusStrip1.ResumeLayout(false);
			statusStrip1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
