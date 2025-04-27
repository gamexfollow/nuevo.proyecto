using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using CCKTiktok.Bussiness;
using CCKTiktok.DAL;
using CCKTiktok.Entity;
using CCKTiktok.Helper;

namespace CCKTiktok.Component
{
	public class frmArticles : Form
	{
		private string selectedUid = "";

		private IContainer components = null;

		private Button btnSave;

		private Label lblLabelTitle;

		private Label label2;

		private TextBox txtFolder;

		private Label label4;

		private TextBox txtContents;

		private Label label5;

		private Button btnFolder;

		private Label label3;

		private Label label6;

		private Label label7;

		private Button txtBaiMoi;

		private DataGridView dataGridViewComment;

		private TextBox txtName;

		private Label label12;

		private TextBox txtId;

		private CheckBox cbxActive;

		private CheckBox cbxDeletePic;

		private TextBox txtProductName;

		private Label label1;

		private TextBox txtLocation;

		private Label label8;

		private TextBox txtSongName;

		private NumericUpDown nudDelay;

		private Label label10;

		private Label label11;

		private NumericUpDown nudPostcount;

		private TextBox txtShortName;

		private Label label13;

		private GroupBox groupBox1;

		private Button btnCopy;

		private TextBox txtUid;

		private Label label14;

		private Button button1;

		private RadioButton rbtByName;

		private RadioButton rbtTop;

		private GroupBox groupBox2;

		private GroupBox groupBox3;

		private RadioButton rbtFavorite;

		private ContextMenuStrip contextMenuStrip1;

		private ToolStripMenuItem mnuDelete;

		private ToolStripMenuItem mnuDeleteVideo;

		private ToolStripMenuItem mnuRemoveProdcut;

		private ToolStripMenuItem mnuDeactive;

		private ToolStripMenuItem nvuActive;

		private ToolStripMenuItem bàiHátToolStripMenuItem;

		private ToolStripMenuItem xaaTenToolStripMenuItem;

		private ToolStripMenuItem bàiHátThịnhHànhToolStripMenuItem;

		private ToolStripMenuItem activeaToolStripMenuItem;

		private ToolStripMenuItem activebToolStripMenuItem;

		private ToolStripMenuItem bàiHátYêuThíchToolStripMenuItem;

		private ToolStripMenuItem activeToolStripMenuItem1;

		private ToolStripMenuItem deActiveHoạtToolStripMenuItem1;

		private LinkLabel linkLabel1;

		private CheckBox cbxVideoVolumn;

		private Label label15;

		private Label label9;

		private NumericUpDown nudVolumnMusic;

		private LinkLabel linkLabel2;

		private RadioButton rbtNoneMusic;

		private CheckBox cbxContent;

		private NumericUpDown nudNumOfPic;

		private RadioButton rbtPic;

		private RadioButton rbtVideo;

		private Label label16;

		private CheckBox cbxKichview;

		private RadioButton cbxShop;

		private RadioButton cbxMarketPlace;

		public frmArticles()
		{
			InitializeComponent();
		}

		private void btnFolder_Click(object sender, EventArgs e)
		{
			using FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			if (txtFolder.Text != "" && Directory.Exists(txtFolder.Text))
			{
				folderBrowserDialog.SelectedPath = txtFolder.Text;
			}
			DialogResult dialogResult = folderBrowserDialog.ShowDialog();
			if (dialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
			{
				txtFolder.Text = folderBrowserDialog.SelectedPath;
			}
		}

		private void LoadExcel(string url)
		{
			//Discarded unreachable code: IL_0019, IL_007b, IL_0082, IL_009b, IL_00a0, IL_00aa, IL_00c0, IL_00ed, IL_00f5, IL_00fc, IL_0120, IL_0156, IL_017a, IL_0190, IL_01c0, IL_01f9, IL_0207, IL_0212, IL_0220
			//Error decoding local variables: Specified handle is not a TypeDefinitionHandle, TypeRefererenceHandle, or TypeSpecificationHandle.
			//IL_0020: Expected O, but got Unknown
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Expected O, but got Unknown
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Incompatible stack heights: 15 vs 0
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Expected O, but got Unknown
			//IL_00c7: Expected O, but got Unknown
			//IL_00d9: Expected O, but got Unknown
			//IL_0135: Expected O, but got Unknown
			//IL_013c: Expected I4, but got Unknown
			//IL_0143: Expected I4, but got Unknown
			//IL_01d6: Expected O, but got Unknown
			//IL_01ee: Expected O, but got Unknown
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Expected O, but got Unknown
			//IL_023b: Expected O, but got Unknown
			Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("00024500-0000-0000-C000-000000000046")));
			/*Error near IL_0014: Invalid metadata token*/;
		}

		private void frmArticles_Load(object sender, EventArgs e)
		{
			Utils.SetLang(lblLabelTitle, "fba8ae93027547188d6b7f860b1a278a");
			Utils.SetLang(lblLabelTitle, "");
			Utils.SetLang(lblLabelTitle, "");
			Utils.SetLang(lblLabelTitle, "");
			Utils.SetLang(lblLabelTitle, "");
			string aRTICLES = CaChuaConstant.ARTICLES;
			List<ArticleItem> list = new List<ArticleItem>();
			if (File.Exists(aRTICLES))
			{
				string text = Utils.ReadTextFile(aRTICLES);
				if (text != "")
				{
					list = new JavaScriptSerializer().Deserialize<List<ArticleItem>>(text);
				}
				for (int i = 0; i < list.Count; i++)
				{
					list[i].Index = i + 1;
				}
				dataGridViewComment.DataSource = list;
				nudPostcount.Value = Utils.Convert2Int(Utils.ReadTextFile(CaChuaConstant.ARTICLES_COUNT));
				if (nudPostcount.Value == 0m)
				{
					nudPostcount.Value = 1m;
				}
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			if (txtName.Text.Trim() == "")
			{
				MessageBox.Show("Bạn chưa nhập tên bài viết");
				txtName.Focus();
				return;
			}
			if (txtContents.Text.Trim() == "" && !cbxContent.Checked)
			{
				MessageBox.Show("Bạn chưa nhập nội dung bài viết");
				txtContents.Focus();
				return;
			}
			ArticleItem articleItem = new ArticleItem();
			articleItem.Id = DateTime.Now.ToFileTime();
			articleItem.Name = txtName.Text;
			articleItem.Contents = txtContents.Text;
			articleItem.PictureFolder = txtFolder.Text.Trim();
			articleItem.MusicVolumn = (int)nudVolumnMusic.Value;
			articleItem.TurnOffVideoVolumn = cbxVideoVolumn.Checked;
			articleItem.Active = cbxActive.Checked;
			articleItem.ProductShortName = txtShortName.Text.Trim();
			articleItem.Songname = txtSongName.Text.Trim();
			articleItem.DeletePhotoAfterUse = cbxDeletePic.Checked;
			articleItem.ProductName = txtProductName.Text.Trim();
			articleItem.Location = txtLocation.Text.Trim();
			articleItem.Delay = Convert.ToInt32(nudDelay.Value);
			articleItem.Uid = txtUid.Text.Trim();
			articleItem.KickView = cbxKichview.Checked;
			articleItem.ProductOnShop = cbxShop.Checked;
			articleItem.IsContentVideo = rbtVideo.Checked;
			articleItem.NumOfPic = (int)nudNumOfPic.Value;
			articleItem.FavoriteSong = rbtFavorite.Checked;
			articleItem.ContentInVideoFile = cbxContent.Checked;
			articleItem.TopSong = rbtTop.Checked;
			articleItem.NonMusic = rbtNoneMusic.Checked;
			string aRTICLES = CaChuaConstant.ARTICLES;
			string aRTICLES_COUNT = CaChuaConstant.ARTICLES_COUNT;
			File.WriteAllText(aRTICLES_COUNT, nudPostcount.Value.ToString());
			List<ArticleItem> list = new List<ArticleItem>();
			if (File.Exists(aRTICLES))
			{
				string text = Utils.ReadTextFile(aRTICLES);
				if (text != "")
				{
					list = new JavaScriptSerializer().Deserialize<List<ArticleItem>>(text);
				}
			}
			if (!(txtId.Text != ""))
			{
				list.Add(articleItem);
			}
			else
			{
				Convert.ToInt64(txtId.Text);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].Id == Convert.ToInt64(selectedUid))
					{
						list[i] = articleItem;
					}
				}
			}
			File.WriteAllText(aRTICLES, new JavaScriptSerializer().Serialize(list));
			for (int j = 0; j < list.Count; j++)
			{
				list[j].Index = j + 1;
			}
			dataGridViewComment.DataSource = list;
			txtBaiMoi_Click(null, null);
		}

		private void ClearForm()
		{
			txtContents.Text = "";
			txtFolder.Text = "";
		}

		private List<ArticleItem> GetData()
		{
			string aRTICLES = CaChuaConstant.ARTICLES;
			List<ArticleItem> result = new List<ArticleItem>();
			if (File.Exists(aRTICLES))
			{
				string text = Utils.ReadTextFile(aRTICLES);
				if (text != "")
				{
					result = new JavaScriptSerializer().Deserialize<List<ArticleItem>>(text);
				}
			}
			return result;
		}

		private void btnTest_Click(object sender, EventArgs e)
		{
		}

		private string GetRandomToken()
		{
			DataTable dataTable = new SQLiteUtils().ExecuteQuery("Select Token from Account where trangthai='Live' limit 100");
			foreach (DataRow row in dataTable.Rows)
			{
				string text = row["token"].ToString();
				if (!(text != ""))
				{
					continue;
				}
				try
				{
					string data = GetData($"https://graph.facebook.com/v11.0/me?access_token={text}");
					if (!(data != ""))
					{
						continue;
					}
					return text.ToString();
				}
				catch
				{
				}
			}
			return "";
		}

		private void btnCopy_Click(object sender, EventArgs e)
		{
			frmInputControl frmInputControl2 = new frmInputControl(isSingleLine: false, "Copy Post from ID");
			frmInputControl2.ShowDialog();
			string result = frmInputControl2.Result;
			if (!(result != ""))
			{
				return;
			}
			string[] array = result.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			string[] array2 = array;
			foreach (string arg in array2)
			{
				string randomToken = GetRandomToken();
				string url = $"https://graph.facebook.com/v11.0/{arg}?access_token={randomToken}&fields=message,full_picture,from,created_time,attachments";
				string data = GetData(url);
				if (!(data != ""))
				{
					continue;
				}
				dynamic val = new JavaScriptSerializer().DeserializeObject(data);
				txtContents.Text = val["message"].ToString();
				dynamic val2 = val["full_picture"].ToString();
				if (val2 != "")
				{
					if (!Directory.Exists(Application.StartupPath + "\\Picture\\"))
					{
						Directory.CreateDirectory(Application.StartupPath + "\\Picture\\");
					}
					if (!Directory.Exists(Application.StartupPath + "\\Picture\\Article"))
					{
						Directory.CreateDirectory(Application.StartupPath + "\\Picture\\Article");
					}
					string arg2 = Guid.NewGuid().ToString("N");
					new WebClient().DownloadFile(val2, Application.StartupPath + $"\\Picture\\Article\\{arg2}.jpg");
					txtFolder.Text = Application.StartupPath + $"\\Picture\\Article";
				}
				btnSave_Click(null, null);
			}
		}

		public string GetData(string url)
		{
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
				httpWebRequest.ContentType = "text/html; charset=utf-8";
				httpWebRequest.Method = "GET";
				httpWebRequest.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_3_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.5 Mobile/15E148 Snapchat/10.77.0.54 (like Safari/604.1)";
				httpWebRequest.CookieContainer = new CookieContainer();
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
				using HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				using Stream stream = httpWebResponse.GetResponseStream();
				using StreamReader streamReader = new StreamReader(stream);
				string result = streamReader.ReadToEnd();
				httpWebRequest.Abort();
				return result;
			}
			catch
			{
			}
			return "";
		}

		private void txtBaiMoi_Click(object sender, EventArgs e)
		{
			txtContents.Text = "";
			txtFolder.Text = "";
			txtName.Text = "";
			txtId.Text = "";
			cbxDeletePic.Checked = false;
			txtProductName.Text = "";
			txtContents.Focus();
			txtUid.Text = "";
			rbtByName.Checked = false;
			rbtTop.Checked = false;
			rbtFavorite.Checked = false;
			nudVolumnMusic.Value = 100m;
			cbxVideoVolumn.Checked = false;
			rbtNoneMusic.Checked = true;
			cbxContent.Checked = false;
			cbxKichview.Checked = false;
		}

		private void dataGridViewComment_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
		}

		private void dataGridViewComment_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (e.RowIndex != -1)
			{
				dataGridViewComment.Rows[e.RowIndex].Selected = true;
			}
			try
			{
				if (e.RowIndex != -1)
				{
					selectedUid = dataGridViewComment.Rows[e.RowIndex].Cells["id"].Value.ToString();
				}
				List<ArticleItem> data = GetData();
				foreach (ArticleItem item in data)
				{
					if (item.Id == Convert.ToInt64(selectedUid))
					{
						txtName.Text = item.Name;
						txtContents.Text = string.Join(Environment.NewLine, item.Contents);
						txtFolder.Text = item.PictureFolder;
						txtId.Text = item.Id.ToString();
						cbxActive.Checked = item.Active;
						txtLocation.Text = item.Location;
						txtSongName.Text = item.Songname;
						rbtVideo.Checked = item.IsContentVideo;
						rbtPic.Checked = !item.IsContentVideo;
						nudNumOfPic.Value = item.NumOfPic;
						cbxShop.Checked = item.ProductOnShop;
						cbxMarketPlace.Checked = !item.ProductOnShop;
						if (!(txtSongName.Text != ""))
						{
							rbtByName.Checked = true;
						}
						else
						{
							rbtByName.Checked = true;
						}
						rbtFavorite.Checked = item.FavoriteSong;
						rbtTop.Checked = item.TopSong;
						txtUid.Text = item.Uid;
						txtShortName.Text = item.ProductShortName;
						rbtFavorite.Checked = item.FavoriteSong;
						txtProductName.Text = item.ProductName;
						txtId.Text = item.Id.ToString();
						cbxDeletePic.Checked = item.DeletePhotoAfterUse;
						nudDelay.Value = item.Delay;
						cbxVideoVolumn.Checked = item.TurnOffVideoVolumn;
						nudVolumnMusic.Value = item.MusicVolumn;
						rbtNoneMusic.Checked = item.NonMusic;
						cbxContent.Checked = item.ContentInVideoFile;
						cbxKichview.Checked = item.KickView;
					}
				}
			}
			catch
			{
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Do you want to delete this post?", "CCK Message", MessageBoxButtons.YesNo) != DialogResult.Yes)
			{
				return;
			}
			DataGridViewSelectedRowCollection selectedRows = dataGridViewComment.SelectedRows;
			string aRTICLES = CaChuaConstant.ARTICLES;
			List<ArticleItem> list = new List<ArticleItem>();
			if (File.Exists(aRTICLES))
			{
				string text = Utils.ReadTextFile(aRTICLES);
				if (text != "")
				{
					list = new JavaScriptSerializer().Deserialize<List<ArticleItem>>(text);
				}
			}
			foreach (DataGridViewRow item in selectedRows)
			{
				if (item.Index < 0)
				{
					continue;
				}
				txtId.Text = item.Cells["Id"].Value.ToString();
				if (txtId.Text != "")
				{
					long num = Convert.ToInt64(txtId.Text);
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i].Id == num)
						{
							list.RemoveAt(i);
						}
					}
				}
				txtId.Text = "";
			}
			File.WriteAllText(aRTICLES, new JavaScriptSerializer().Serialize(list));
			dataGridViewComment.DataSource = list;
			txtBaiMoi_Click(null, null);
		}

		protected virtual bool IsFileLocked(FileInfo file)
		{
			try
			{
				using FileStream fileStream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
				fileStream.Close();
			}
			catch (IOException)
			{
				return true;
			}
			return false;
		}

		private void btnCopy_Click_1(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "Select file";
			openFileDialog.InitialDirectory = "c:\\";
			openFileDialog.Filter = "CSV (*.csv)|*.csv|All Files(*.*)|*.*";
			openFileDialog.FilterIndex = 1;
			openFileDialog.RestoreDirectory = true;
			if (openFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			if (IsFileLocked(new FileInfo(openFileDialog.FileName)))
			{
				ADBHelperCCK.RunCommand("taskkill /f /im excel.exe");
			}
			Cursor.Current = Cursors.WaitCursor;
			string aRTICLES = CaChuaConstant.ARTICLES;
			string[] array = File.ReadAllLines(openFileDialog.FileName);
			if (array != null && array.Length > 1)
			{
				List<ArticleItem> list = new List<ArticleItem>();
				if (File.Exists(aRTICLES))
				{
					string text = Utils.ReadTextFile(aRTICLES);
					if (text != "")
					{
						list = new JavaScriptSerializer().Deserialize<List<ArticleItem>>(text);
					}
					if (openFileDialog.FileName.ToLower().EndsWith("csv"))
					{
						string[] array2 = array[0].Split(",;\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
						for (int i = 1; i < array.Length; i++)
						{
							string[] array3 = array[i].ToString().Split(",;\t".ToCharArray());
							if (array3.Length < array2.Length)
							{
								continue;
							}
							ArticleItem articleItem = new ArticleItem();
							articleItem.Id = DateTime.Now.ToFileTime() + i * 100;
							for (int j = 0; j < array2.Length; j++)
							{
								if (array2[j] == "TenSP")
								{
									if (articleItem.Name == "")
									{
										articleItem.Name = array3[j];
									}
									articleItem.ProductName = array3[j];
								}
								else if (!(array2[j] == "Name"))
								{
									if (!(array2[j] == "DeleteVideo"))
									{
										if (!(array2[j] == "MoTa"))
										{
											if (!(array2[j] == "Music_Favorite"))
											{
												if (!(array2[j] == "Delay"))
												{
													if (array2[j] == "Account")
													{
														articleItem.Uid = array3[j];
													}
													else if (!(array2[j] == "TenSP_Short"))
													{
														if (!(array2[j] == "LinkPicture"))
														{
															if (!(array2[j] == "Music"))
															{
																if (array2[j] == "OffMusic")
																{
																	articleItem.MusicVolumn = Utils.Convert2Int(array3[j]);
																}
																else if (array2[j] == "NonMusic")
																{
																	if (array3[j].ToLower().Equals("x"))
																	{
																		articleItem.NonMusic = true;
																	}
																}
																else if (array2[j] == "OffVideo" && array3[j].ToLower().Equals("x"))
																{
																	articleItem.TurnOffVideoVolumn = true;
																}
															}
															else
															{
																articleItem.Songname = array3[j];
															}
														}
														else
														{
															articleItem.PictureFolder = array3[j];
														}
													}
													else
													{
														articleItem.ProductShortName = array3[j];
													}
												}
												else
												{
													articleItem.Delay = Utils.Convert2Int(array3[j]);
												}
											}
											else if (array3[j].ToLower().Equals("x"))
											{
												articleItem.FavoriteSong = true;
											}
										}
										else
										{
											articleItem.Contents = array3[j];
										}
									}
									else if (array3[j].Trim().ToLower().Equals("x"))
									{
										articleItem.DeletePhotoAfterUse = true;
									}
								}
								else
								{
									articleItem.Name = array3[j];
								}
							}
							list.Add(articleItem);
						}
					}
				}
				for (int k = 0; k < list.Count; k++)
				{
					list[k].Index = k + 1;
				}
				File.WriteAllText(aRTICLES, new JavaScriptSerializer().Serialize(list));
				dataGridViewComment.DataSource = list;
			}
			Cursor.Current = Cursors.Default;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			btnSave_Click(null, null);
			Close();
		}

		private void rbtByName_CheckedChanged(object sender, EventArgs e)
		{
			txtSongName.Enabled = rbtByName.Checked;
			if (txtSongName.Enabled)
			{
				txtSongName.Focus();
			}
			else
			{
				txtSongName.Text = "";
			}
		}

		private void mnuDelete_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				if (MessageBox.Show("Do you want to delete this post?", "CCK Message", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					DataGridViewSelectedRowCollection selectedRows = dataGridViewComment.SelectedRows;
					string aRTICLES = CaChuaConstant.ARTICLES;
					List<ArticleItem> list = new List<ArticleItem>();
					if (File.Exists(aRTICLES))
					{
						string text = Utils.ReadTextFile(aRTICLES);
						if (text != "")
						{
							list = new JavaScriptSerializer().Deserialize<List<ArticleItem>>(text);
						}
					}
					foreach (DataGridViewRow item in selectedRows)
					{
						if (item.Index >= 0)
						{
							txtId.Text = item.Cells["Id"].Value.ToString();
							if (txtId.Text != "")
							{
								long num = Convert.ToInt64(txtId.Text);
								for (int i = 0; i < list.Count; i++)
								{
									if (list[i].Id == num)
									{
										list.RemoveAt(i);
									}
								}
							}
							txtId.Text = "";
						}
					}
					File.WriteAllText(aRTICLES, new JavaScriptSerializer().Serialize(list));
					dataGridViewComment.DataSource = list;
					txtBaiMoi_Click(null, null);
				}
			}).Start();
		}

		private void mnuDeleteVideo_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				DataGridViewSelectedRowCollection selectedRows = dataGridViewComment.SelectedRows;
				string aRTICLES = CaChuaConstant.ARTICLES;
				List<ArticleItem> list = new List<ArticleItem>();
				if (File.Exists(aRTICLES))
				{
					string text = Utils.ReadTextFile(aRTICLES);
					if (text != "")
					{
						list = new JavaScriptSerializer().Deserialize<List<ArticleItem>>(text);
					}
				}
				foreach (DataGridViewRow item in selectedRows)
				{
					if (item.Index >= 0)
					{
						txtId.Text = item.Cells["Id"].Value.ToString();
						if (txtId.Text != "")
						{
							long num = Convert.ToInt64(txtId.Text);
							for (int i = 0; i < list.Count; i++)
							{
								if (list[i].Id == num)
								{
									list[i].DeletePhotoAfterUse = true;
								}
							}
						}
						txtId.Text = "";
					}
				}
				File.WriteAllText(aRTICLES, new JavaScriptSerializer().Serialize(list));
				dataGridViewComment.DataSource = list;
				txtBaiMoi_Click(null, null);
			}).Start();
		}

		private void mnuRemoveProdcut_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				DataGridViewSelectedRowCollection selectedRows = dataGridViewComment.SelectedRows;
				string aRTICLES = CaChuaConstant.ARTICLES;
				List<ArticleItem> list = new List<ArticleItem>();
				if (File.Exists(aRTICLES))
				{
					string text = Utils.ReadTextFile(aRTICLES);
					if (text != "")
					{
						list = new JavaScriptSerializer().Deserialize<List<ArticleItem>>(text);
					}
				}
				foreach (DataGridViewRow item in selectedRows)
				{
					if (item.Index >= 0)
					{
						txtId.Text = item.Cells["Id"].Value.ToString();
						if (txtId.Text != "")
						{
							long num = Convert.ToInt64(txtId.Text);
							for (int i = 0; i < list.Count; i++)
							{
								if (list[i].Id == num)
								{
									list[i].ProductName = "";
									list[i].ProductShortName = "";
								}
							}
						}
						txtId.Text = "";
					}
				}
				File.WriteAllText(aRTICLES, new JavaScriptSerializer().Serialize(list));
				dataGridViewComment.DataSource = list;
				txtBaiMoi_Click(null, null);
			}).Start();
		}

		private void mnuDeactive_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				DataGridViewSelectedRowCollection selectedRows = dataGridViewComment.SelectedRows;
				string aRTICLES = CaChuaConstant.ARTICLES;
				List<ArticleItem> list = new List<ArticleItem>();
				if (File.Exists(aRTICLES))
				{
					string text = Utils.ReadTextFile(aRTICLES);
					if (text != "")
					{
						list = new JavaScriptSerializer().Deserialize<List<ArticleItem>>(text);
					}
				}
				foreach (DataGridViewRow item in selectedRows)
				{
					if (item.Index >= 0)
					{
						txtId.Text = item.Cells["Id"].Value.ToString();
						if (txtId.Text != "")
						{
							long num = Convert.ToInt64(txtId.Text);
							for (int i = 0; i < list.Count; i++)
							{
								if (list[i].Id == num)
								{
									list[i].Active = false;
								}
							}
						}
						txtId.Text = "";
					}
				}
				File.WriteAllText(aRTICLES, new JavaScriptSerializer().Serialize(list));
				dataGridViewComment.DataSource = list;
				txtBaiMoi_Click(null, null);
			}).Start();
		}

		private void nvuActive_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				DataGridViewSelectedRowCollection selectedRows = dataGridViewComment.SelectedRows;
				string aRTICLES = CaChuaConstant.ARTICLES;
				List<ArticleItem> list = new List<ArticleItem>();
				if (File.Exists(aRTICLES))
				{
					string text = Utils.ReadTextFile(aRTICLES);
					if (text != "")
					{
						list = new JavaScriptSerializer().Deserialize<List<ArticleItem>>(text);
					}
				}
				foreach (DataGridViewRow item in selectedRows)
				{
					if (item.Index >= 0)
					{
						txtId.Text = item.Cells["Id"].Value.ToString();
						if (txtId.Text != "")
						{
							long num = Convert.ToInt64(txtId.Text);
							for (int i = 0; i < list.Count; i++)
							{
								if (list[i].Id == num)
								{
									list[i].Active = true;
								}
							}
						}
						txtId.Text = "";
					}
				}
				File.WriteAllText(aRTICLES, new JavaScriptSerializer().Serialize(list));
				dataGridViewComment.DataSource = list;
				txtBaiMoi_Click(null, null);
			}).Start();
		}

		private void xoasTXToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				DataGridViewSelectedRowCollection selectedRows = dataGridViewComment.SelectedRows;
				string aRTICLES = CaChuaConstant.ARTICLES;
				List<ArticleItem> list = new List<ArticleItem>();
				if (File.Exists(aRTICLES))
				{
					string text = Utils.ReadTextFile(aRTICLES);
					if (text != "")
					{
						list = new JavaScriptSerializer().Deserialize<List<ArticleItem>>(text);
					}
				}
				foreach (DataGridViewRow item in selectedRows)
				{
					if (item.Index >= 0)
					{
						txtId.Text = item.Cells["Id"].Value.ToString();
						if (txtId.Text != "")
						{
							long num = Convert.ToInt64(txtId.Text);
							for (int i = 0; i < list.Count; i++)
							{
								if (list[i].Id == num)
								{
									list[i].Songname = "";
								}
							}
						}
						txtId.Text = "";
					}
				}
				File.WriteAllText(aRTICLES, new JavaScriptSerializer().Serialize(list));
				dataGridViewComment.DataSource = list;
				txtBaiMoi_Click(null, null);
			}).Start();
		}

		private void chonToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private void activeToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private void deActiveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				DataGridViewSelectedRowCollection selectedRows = dataGridViewComment.SelectedRows;
				string aRTICLES = CaChuaConstant.ARTICLES;
				List<ArticleItem> list = new List<ArticleItem>();
				if (File.Exists(aRTICLES))
				{
					string text = Utils.ReadTextFile(aRTICLES);
					if (text != "")
					{
						list = new JavaScriptSerializer().Deserialize<List<ArticleItem>>(text);
					}
				}
				foreach (DataGridViewRow item in selectedRows)
				{
					if (item.Index >= 0)
					{
						txtId.Text = item.Cells["Id"].Value.ToString();
						if (txtId.Text != "")
						{
							long num = Convert.ToInt64(txtId.Text);
							for (int i = 0; i < list.Count; i++)
							{
								if (list[i].Id == num)
								{
									list[i].FavoriteSong = false;
								}
							}
						}
						txtId.Text = "";
					}
				}
				File.WriteAllText(aRTICLES, new JavaScriptSerializer().Serialize(list));
				dataGridViewComment.DataSource = list;
				txtBaiMoi_Click(null, null);
			}).Start();
		}

		private void kíchHoạtToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				DataGridViewSelectedRowCollection selectedRows = dataGridViewComment.SelectedRows;
				string aRTICLES = CaChuaConstant.ARTICLES;
				List<ArticleItem> list = new List<ArticleItem>();
				if (File.Exists(aRTICLES))
				{
					string text = Utils.ReadTextFile(aRTICLES);
					if (text != "")
					{
						list = new JavaScriptSerializer().Deserialize<List<ArticleItem>>(text);
					}
				}
				foreach (DataGridViewRow item in selectedRows)
				{
					if (item.Index >= 0)
					{
						txtId.Text = item.Cells["Id"].Value.ToString();
						if (txtId.Text != "")
						{
							long num = Convert.ToInt64(txtId.Text);
							for (int i = 0; i < list.Count; i++)
							{
								if (list[i].Id == num)
								{
									list[i].FavoriteSong = true;
								}
							}
						}
						txtId.Text = "";
					}
				}
				File.WriteAllText(aRTICLES, new JavaScriptSerializer().Serialize(list));
				dataGridViewComment.DataSource = list;
				txtBaiMoi_Click(null, null);
			}).Start();
		}

		private void activeaToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				DataGridViewSelectedRowCollection selectedRows = dataGridViewComment.SelectedRows;
				string aRTICLES = CaChuaConstant.ARTICLES;
				List<ArticleItem> list = new List<ArticleItem>();
				if (File.Exists(aRTICLES))
				{
					string text = Utils.ReadTextFile(aRTICLES);
					if (text != "")
					{
						list = new JavaScriptSerializer().Deserialize<List<ArticleItem>>(text);
					}
				}
				foreach (DataGridViewRow item in selectedRows)
				{
					if (item.Index >= 0)
					{
						txtId.Text = item.Cells["Id"].Value.ToString();
						if (txtId.Text != "")
						{
							long num = Convert.ToInt64(txtId.Text);
							for (int i = 0; i < list.Count; i++)
							{
								if (list[i].Id == num)
								{
									list[i].TopSong = true;
								}
							}
						}
						txtId.Text = "";
					}
				}
				File.WriteAllText(aRTICLES, new JavaScriptSerializer().Serialize(list));
				dataGridViewComment.DataSource = list;
				txtBaiMoi_Click(null, null);
			}).Start();
		}

		private void activebToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				DataGridViewSelectedRowCollection selectedRows = dataGridViewComment.SelectedRows;
				string aRTICLES = CaChuaConstant.ARTICLES;
				List<ArticleItem> list = new List<ArticleItem>();
				if (File.Exists(aRTICLES))
				{
					string text = Utils.ReadTextFile(aRTICLES);
					if (text != "")
					{
						list = new JavaScriptSerializer().Deserialize<List<ArticleItem>>(text);
					}
				}
				foreach (DataGridViewRow item in selectedRows)
				{
					if (item.Index >= 0)
					{
						txtId.Text = item.Cells["Id"].Value.ToString();
						if (txtId.Text != "")
						{
							long num = Convert.ToInt64(txtId.Text);
							for (int i = 0; i < list.Count; i++)
							{
								if (list[i].Id == num)
								{
									list[i].TopSong = false;
								}
							}
						}
						txtId.Text = "";
					}
				}
				File.WriteAllText(aRTICLES, new JavaScriptSerializer().Serialize(list));
				dataGridViewComment.DataSource = list;
				txtBaiMoi_Click(null, null);
			}).Start();
		}

		private void deActiveHoạtToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			new Task(delegate
			{
				DataGridViewSelectedRowCollection selectedRows = dataGridViewComment.SelectedRows;
				string aRTICLES = CaChuaConstant.ARTICLES;
				List<ArticleItem> list = new List<ArticleItem>();
				if (File.Exists(aRTICLES))
				{
					string text = Utils.ReadTextFile(aRTICLES);
					if (text != "")
					{
						list = new JavaScriptSerializer().Deserialize<List<ArticleItem>>(text);
					}
				}
				foreach (DataGridViewRow item in selectedRows)
				{
					if (item.Index >= 0)
					{
						txtId.Text = item.Cells["Id"].Value.ToString();
						if (txtId.Text != "")
						{
							long num = Convert.ToInt64(txtId.Text);
							for (int i = 0; i < list.Count; i++)
							{
								if (list[i].Id == num)
								{
									list[i].FavoriteSong = false;
								}
							}
						}
						txtId.Text = "";
					}
				}
				File.WriteAllText(aRTICLES, new JavaScriptSerializer().Serialize(list));
				dataGridViewComment.DataSource = list;
				txtBaiMoi_Click(null, null);
			}).Start();
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://cck.vn/Download/Update/cck_tiktok_template.rar");
		}

		private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (txtFolder.Text != "" && Directory.Exists(txtFolder.Text))
			{
				Process.Start(txtFolder.Text);
			}
		}

		private void txtFolder_TextChanged(object sender, EventArgs e)
		{
			linkLabel2.Visible = txtFolder.Text != "" && Directory.Exists(txtFolder.Text);
		}

		private void cbxContent_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxContent.Checked)
			{
				txtContents.Text = "";
			}
		}

		private void rbtVideo_CheckedChanged(object sender, EventArgs e)
		{
			nudNumOfPic.Value = 1m;
			nudNumOfPic.Enabled = false;
		}

		private void rbtPic_CheckedChanged(object sender, EventArgs e)
		{
			nudNumOfPic.Enabled = true;
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
			btnSave = new System.Windows.Forms.Button();
			lblLabelTitle = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			txtFolder = new System.Windows.Forms.TextBox();
			label4 = new System.Windows.Forms.Label();
			txtContents = new System.Windows.Forms.TextBox();
			label5 = new System.Windows.Forms.Label();
			btnFolder = new System.Windows.Forms.Button();
			label3 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			txtBaiMoi = new System.Windows.Forms.Button();
			dataGridViewComment = new System.Windows.Forms.DataGridView();
			contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(components);
			mnuDelete = new System.Windows.Forms.ToolStripMenuItem();
			mnuDeleteVideo = new System.Windows.Forms.ToolStripMenuItem();
			mnuRemoveProdcut = new System.Windows.Forms.ToolStripMenuItem();
			mnuDeactive = new System.Windows.Forms.ToolStripMenuItem();
			nvuActive = new System.Windows.Forms.ToolStripMenuItem();
			bàiHátToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			xaaTenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			bàiHátThịnhHànhToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			activeaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			activebToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			bàiHátYêuThíchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			activeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			deActiveHoạtToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			txtName = new System.Windows.Forms.TextBox();
			label12 = new System.Windows.Forms.Label();
			txtId = new System.Windows.Forms.TextBox();
			cbxActive = new System.Windows.Forms.CheckBox();
			cbxDeletePic = new System.Windows.Forms.CheckBox();
			txtProductName = new System.Windows.Forms.TextBox();
			label1 = new System.Windows.Forms.Label();
			txtLocation = new System.Windows.Forms.TextBox();
			label8 = new System.Windows.Forms.Label();
			txtSongName = new System.Windows.Forms.TextBox();
			nudDelay = new System.Windows.Forms.NumericUpDown();
			label10 = new System.Windows.Forms.Label();
			label11 = new System.Windows.Forms.Label();
			nudPostcount = new System.Windows.Forms.NumericUpDown();
			txtShortName = new System.Windows.Forms.TextBox();
			label13 = new System.Windows.Forms.Label();
			groupBox1 = new System.Windows.Forms.GroupBox();
			btnCopy = new System.Windows.Forms.Button();
			txtUid = new System.Windows.Forms.TextBox();
			label14 = new System.Windows.Forms.Label();
			button1 = new System.Windows.Forms.Button();
			rbtByName = new System.Windows.Forms.RadioButton();
			rbtTop = new System.Windows.Forms.RadioButton();
			groupBox2 = new System.Windows.Forms.GroupBox();
			rbtNoneMusic = new System.Windows.Forms.RadioButton();
			label15 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			nudVolumnMusic = new System.Windows.Forms.NumericUpDown();
			cbxVideoVolumn = new System.Windows.Forms.CheckBox();
			rbtFavorite = new System.Windows.Forms.RadioButton();
			groupBox3 = new System.Windows.Forms.GroupBox();
			cbxKichview = new System.Windows.Forms.CheckBox();
			nudNumOfPic = new System.Windows.Forms.NumericUpDown();
			rbtPic = new System.Windows.Forms.RadioButton();
			rbtVideo = new System.Windows.Forms.RadioButton();
			cbxContent = new System.Windows.Forms.CheckBox();
			linkLabel2 = new System.Windows.Forms.LinkLabel();
			label16 = new System.Windows.Forms.Label();
			linkLabel1 = new System.Windows.Forms.LinkLabel();
			cbxMarketPlace = new System.Windows.Forms.RadioButton();
			cbxShop = new System.Windows.Forms.RadioButton();
			((System.ComponentModel.ISupportInitialize)dataGridViewComment).BeginInit();
			contextMenuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)nudDelay).BeginInit();
			((System.ComponentModel.ISupportInitialize)nudPostcount).BeginInit();
			groupBox1.SuspendLayout();
			groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)nudVolumnMusic).BeginInit();
			groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)nudNumOfPic).BeginInit();
			SuspendLayout();
			btnSave.Location = new System.Drawing.Point(264, 581);
			btnSave.Margin = new System.Windows.Forms.Padding(2);
			btnSave.Name = "btnSave";
			btnSave.Size = new System.Drawing.Size(90, 35);
			btnSave.TabIndex = 4;
			btnSave.Text = "Lưu";
			btnSave.UseVisualStyleBackColor = true;
			btnSave.Click += new System.EventHandler(btnSave_Click);
			lblLabelTitle.AutoSize = true;
			lblLabelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			lblLabelTitle.Location = new System.Drawing.Point(385, 7);
			lblLabelTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			lblLabelTitle.Name = "lblLabelTitle";
			lblLabelTitle.Size = new System.Drawing.Size(134, 20);
			lblLabelTitle.TabIndex = 2;
			lblLabelTitle.Text = "Kho video lưu trữ";
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(13, 310);
			label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(134, 13);
			label2.TabIndex = 4;
			label2.Text = "Thư mục chứa video / ảnh";
			txtFolder.Location = new System.Drawing.Point(159, 307);
			txtFolder.Margin = new System.Windows.Forms.Padding(2);
			txtFolder.Name = "txtFolder";
			txtFolder.Size = new System.Drawing.Size(238, 20);
			txtFolder.TabIndex = 3;
			txtFolder.TextChanged += new System.EventHandler(txtFolder_TextChanged);
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(14, 81);
			label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(44, 13);
			label4.TabIndex = 8;
			label4.Text = "Content";
			txtContents.Location = new System.Drawing.Point(18, 108);
			txtContents.Margin = new System.Windows.Forms.Padding(2);
			txtContents.MaxLength = 3276700;
			txtContents.Multiline = true;
			txtContents.Name = "txtContents";
			txtContents.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			txtContents.Size = new System.Drawing.Size(466, 95);
			txtContents.TabIndex = 2;
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(378, 246);
			label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(102, 13);
			label5.TabIndex = 9;
			label5.Text = "Support spin {A|B|C}";
			btnFolder.Location = new System.Drawing.Point(406, 305);
			btnFolder.Margin = new System.Windows.Forms.Padding(2);
			btnFolder.Name = "btnFolder";
			btnFolder.Size = new System.Drawing.Size(74, 22);
			btnFolder.TabIndex = 4;
			btnFolder.Text = "Chose folder";
			btnFolder.UseVisualStyleBackColor = true;
			btnFolder.Click += new System.EventHandler(btnFolder_Click);
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(188, 81);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(86, 13);
			label3.TabIndex = 21;
			label3.Text = "[r]: Random Icon";
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(303, 81);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(70, 13);
			label6.TabIndex = 22;
			label6.Text = "[n]: New Line";
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(392, 81);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(91, 13);
			label7.TabIndex = 23;
			label7.Text = "[d]: Random Date";
			txtBaiMoi.Location = new System.Drawing.Point(442, 581);
			txtBaiMoi.Margin = new System.Windows.Forms.Padding(2);
			txtBaiMoi.Name = "txtBaiMoi";
			txtBaiMoi.Size = new System.Drawing.Size(90, 35);
			txtBaiMoi.TabIndex = 24;
			txtBaiMoi.Text = "Viết bài mới";
			txtBaiMoi.UseVisualStyleBackColor = true;
			txtBaiMoi.Click += new System.EventHandler(txtBaiMoi_Click);
			dataGridViewComment.AllowUserToAddRows = false;
			dataGridViewComment.AllowUserToDeleteRows = false;
			dataGridViewComment.AllowUserToResizeRows = false;
			dataGridViewComment.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridViewComment.ContextMenuStrip = contextMenuStrip1;
			dataGridViewComment.Location = new System.Drawing.Point(27, 388);
			dataGridViewComment.Name = "dataGridViewComment";
			dataGridViewComment.ReadOnly = true;
			dataGridViewComment.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			dataGridViewComment.Size = new System.Drawing.Size(963, 180);
			dataGridViewComment.TabIndex = 37;
			dataGridViewComment.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(dataGridViewComment_CellContentClick);
			dataGridViewComment.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(dataGridViewComment_CellMouseDown);
			contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[6] { mnuDelete, mnuDeleteVideo, mnuRemoveProdcut, mnuDeactive, nvuActive, bàiHátToolStripMenuItem });
			contextMenuStrip1.Name = "contextMenuStrip1";
			contextMenuStrip1.Size = new System.Drawing.Size(243, 136);
			mnuDelete.Name = "mnuDelete";
			mnuDelete.Size = new System.Drawing.Size(242, 22);
			mnuDelete.Text = "Xóa bài viết";
			mnuDelete.Click += new System.EventHandler(mnuDelete_Click);
			mnuDeleteVideo.Name = "mnuDeleteVideo";
			mnuDeleteVideo.Size = new System.Drawing.Size(242, 22);
			mnuDeleteVideo.Text = "Bật xóa video sau khi đăng";
			mnuDeleteVideo.Click += new System.EventHandler(mnuDeleteVideo_Click);
			mnuRemoveProdcut.Name = "mnuRemoveProdcut";
			mnuRemoveProdcut.Size = new System.Drawing.Size(242, 22);
			mnuRemoveProdcut.Text = "Xóa tên sản phẩm trong bài viết";
			mnuRemoveProdcut.Click += new System.EventHandler(mnuRemoveProdcut_Click);
			mnuDeactive.Name = "mnuDeactive";
			mnuDeactive.Size = new System.Drawing.Size(242, 22);
			mnuDeactive.Text = "Hủy kích hoạt bài viết";
			mnuDeactive.Click += new System.EventHandler(mnuDeactive_Click);
			nvuActive.Name = "nvuActive";
			nvuActive.Size = new System.Drawing.Size(242, 22);
			nvuActive.Text = "Kích hoạt bài viết";
			nvuActive.Click += new System.EventHandler(nvuActive_Click);
			bàiHátToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[3] { xaaTenToolStripMenuItem, bàiHátThịnhHànhToolStripMenuItem, bàiHátYêuThíchToolStripMenuItem });
			bàiHátToolStripMenuItem.Name = "bàiHátToolStripMenuItem";
			bàiHátToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
			bàiHátToolStripMenuItem.Text = "Bài hát";
			xaaTenToolStripMenuItem.Name = "xaaTenToolStripMenuItem";
			xaaTenToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			xaaTenToolStripMenuItem.Text = "Xóa tên";
			xaaTenToolStripMenuItem.Click += new System.EventHandler(xoasTXToolStripMenuItem_Click);
			bàiHátThịnhHànhToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[2] { activeaToolStripMenuItem, activebToolStripMenuItem });
			bàiHátThịnhHànhToolStripMenuItem.Name = "bàiHátThịnhHànhToolStripMenuItem";
			bàiHátThịnhHànhToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			bàiHátThịnhHànhToolStripMenuItem.Text = "Bài hát thịnh hành";
			activeaToolStripMenuItem.Name = "activeaToolStripMenuItem";
			activeaToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
			activeaToolStripMenuItem.Text = "Kích hoạt";
			activeaToolStripMenuItem.Click += new System.EventHandler(activeaToolStripMenuItem_Click);
			activebToolStripMenuItem.Name = "activebToolStripMenuItem";
			activebToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
			activebToolStripMenuItem.Text = "Hủy kích hoạt";
			activebToolStripMenuItem.Click += new System.EventHandler(activebToolStripMenuItem_Click);
			bàiHátYêuThíchToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[2] { activeToolStripMenuItem1, deActiveHoạtToolStripMenuItem1 });
			bàiHátYêuThíchToolStripMenuItem.Name = "bàiHátYêuThíchToolStripMenuItem";
			bàiHátYêuThíchToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			bàiHátYêuThíchToolStripMenuItem.Text = "Bài hát yêu thích";
			activeToolStripMenuItem1.Name = "activeToolStripMenuItem1";
			activeToolStripMenuItem1.Size = new System.Drawing.Size(148, 22);
			activeToolStripMenuItem1.Text = "Kích hoạt";
			activeToolStripMenuItem1.Click += new System.EventHandler(kíchHoạtToolStripMenuItem1_Click);
			deActiveHoạtToolStripMenuItem1.Name = "deActiveHoạtToolStripMenuItem1";
			deActiveHoạtToolStripMenuItem1.Size = new System.Drawing.Size(148, 22);
			deActiveHoạtToolStripMenuItem1.Text = "Hủy kích hoạt";
			deActiveHoạtToolStripMenuItem1.Click += new System.EventHandler(deActiveHoạtToolStripMenuItem1_Click);
			txtName.Location = new System.Drawing.Point(16, 50);
			txtName.Margin = new System.Windows.Forms.Padding(2);
			txtName.Name = "txtName";
			txtName.Size = new System.Drawing.Size(152, 20);
			txtName.TabIndex = 38;
			label12.AutoSize = true;
			label12.Location = new System.Drawing.Point(15, 25);
			label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label12.Name = "label12";
			label12.Size = new System.Drawing.Size(35, 13);
			label12.TabIndex = 39;
			label12.Text = "Name";
			txtId.Location = new System.Drawing.Point(895, 590);
			txtId.Margin = new System.Windows.Forms.Padding(2);
			txtId.Name = "txtId";
			txtId.Size = new System.Drawing.Size(95, 20);
			txtId.TabIndex = 3;
			txtId.Visible = false;
			cbxActive.AutoSize = true;
			cbxActive.Checked = true;
			cbxActive.CheckState = System.Windows.Forms.CheckState.Checked;
			cbxActive.Location = new System.Drawing.Point(17, 247);
			cbxActive.Margin = new System.Windows.Forms.Padding(2);
			cbxActive.Name = "cbxActive";
			cbxActive.Size = new System.Drawing.Size(73, 17);
			cbxActive.TabIndex = 40;
			cbxActive.Text = "Kích hoạt";
			cbxActive.UseVisualStyleBackColor = true;
			cbxDeletePic.AutoSize = true;
			cbxDeletePic.Checked = true;
			cbxDeletePic.CheckState = System.Windows.Forms.CheckState.Checked;
			cbxDeletePic.Location = new System.Drawing.Point(96, 247);
			cbxDeletePic.Margin = new System.Windows.Forms.Padding(2);
			cbxDeletePic.Name = "cbxDeletePic";
			cbxDeletePic.Size = new System.Drawing.Size(139, 17);
			cbxDeletePic.TabIndex = 44;
			cbxDeletePic.Text = "Xóa video sau khi đăng";
			cbxDeletePic.UseVisualStyleBackColor = true;
			txtProductName.Location = new System.Drawing.Point(14, 45);
			txtProductName.Margin = new System.Windows.Forms.Padding(2);
			txtProductName.MaxLength = 3276700;
			txtProductName.Multiline = true;
			txtProductName.Name = "txtProductName";
			txtProductName.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			txtProductName.Size = new System.Drawing.Size(429, 69);
			txtProductName.TabIndex = 45;
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(12, 133);
			label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(110, 13);
			label1.TabIndex = 46;
			label1.Text = "Tên sản phẩm viết tắt";
			txtLocation.Location = new System.Drawing.Point(329, 130);
			txtLocation.Margin = new System.Windows.Forms.Padding(2);
			txtLocation.Name = "txtLocation";
			txtLocation.Size = new System.Drawing.Size(115, 20);
			txtLocation.TabIndex = 47;
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(278, 133);
			label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(48, 13);
			label8.TabIndex = 48;
			label8.Text = "Location";
			txtSongName.Location = new System.Drawing.Point(17, 56);
			txtSongName.Margin = new System.Windows.Forms.Padding(2);
			txtSongName.MaxLength = 3276700;
			txtSongName.Multiline = true;
			txtSongName.Name = "txtSongName";
			txtSongName.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			txtSongName.Size = new System.Drawing.Size(432, 65);
			txtSongName.TabIndex = 45;
			nudDelay.Location = new System.Drawing.Point(305, 244);
			nudDelay.Maximum = new decimal(new int[4] { 10000, 0, 0, 0 });
			nudDelay.Minimum = new decimal(new int[4] { 10, 0, 0, 0 });
			nudDelay.Name = "nudDelay";
			nudDelay.Size = new System.Drawing.Size(52, 20);
			nudDelay.TabIndex = 49;
			nudDelay.Value = new decimal(new int[4] { 10, 0, 0, 0 });
			label10.AutoSize = true;
			label10.Location = new System.Drawing.Point(255, 248);
			label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label10.Name = "label10";
			label10.Size = new System.Drawing.Size(34, 13);
			label10.TabIndex = 8;
			label10.Text = "Delay";
			label11.AutoSize = true;
			label11.Location = new System.Drawing.Point(392, 26);
			label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label11.Name = "label11";
			label11.Size = new System.Drawing.Size(65, 13);
			label11.TabIndex = 8;
			label11.Text = "Số bài đăng";
			nudPostcount.Location = new System.Drawing.Point(395, 49);
			nudPostcount.Maximum = new decimal(new int[4] { 100000000, 0, 0, 0 });
			nudPostcount.Name = "nudPostcount";
			nudPostcount.Size = new System.Drawing.Size(89, 20);
			nudPostcount.TabIndex = 49;
			nudPostcount.Value = new decimal(new int[4] { 1, 0, 0, 0 });
			txtShortName.Location = new System.Drawing.Point(125, 130);
			txtShortName.Margin = new System.Windows.Forms.Padding(2);
			txtShortName.Name = "txtShortName";
			txtShortName.Size = new System.Drawing.Size(137, 20);
			txtShortName.TabIndex = 50;
			label13.AutoSize = true;
			label13.Location = new System.Drawing.Point(11, 21);
			label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label13.Name = "label13";
			label13.Size = new System.Drawing.Size(184, 13);
			label13.TabIndex = 51;
			label13.Text = "Tến sản phẩm dùng cho Tiktok Shop";
			groupBox1.Controls.Add(label13);
			groupBox1.Controls.Add(label1);
			groupBox1.Controls.Add(txtLocation);
			groupBox1.Controls.Add(label8);
			groupBox1.Controls.Add(txtShortName);
			groupBox1.Controls.Add(cbxShop);
			groupBox1.Controls.Add(txtProductName);
			groupBox1.Controls.Add(cbxMarketPlace);
			groupBox1.Location = new System.Drawing.Point(533, 30);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(467, 166);
			groupBox1.TabIndex = 52;
			groupBox1.TabStop = false;
			groupBox1.Text = "Cấu hình shop";
			btnCopy.Location = new System.Drawing.Point(536, 581);
			btnCopy.Margin = new System.Windows.Forms.Padding(2);
			btnCopy.Name = "btnCopy";
			btnCopy.Size = new System.Drawing.Size(118, 35);
			btnCopy.TabIndex = 53;
			btnCopy.Text = "Import Excel / CSV";
			btnCopy.UseVisualStyleBackColor = true;
			btnCopy.Click += new System.EventHandler(btnCopy_Click_1);
			txtUid.Location = new System.Drawing.Point(191, 49);
			txtUid.Margin = new System.Windows.Forms.Padding(2);
			txtUid.Name = "txtUid";
			txtUid.Size = new System.Drawing.Size(182, 20);
			txtUid.TabIndex = 54;
			label14.AutoSize = true;
			label14.Location = new System.Drawing.Point(192, 26);
			label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label14.Name = "label14";
			label14.Size = new System.Drawing.Size(61, 13);
			label14.TabIndex = 55;
			label14.Text = "Account ID";
			button1.Location = new System.Drawing.Point(358, 581);
			button1.Margin = new System.Windows.Forms.Padding(2);
			button1.Name = "button1";
			button1.Size = new System.Drawing.Size(80, 35);
			button1.TabIndex = 4;
			button1.Text = "Lưu và đóng";
			button1.UseVisualStyleBackColor = true;
			button1.Click += new System.EventHandler(button1_Click);
			rbtByName.AutoSize = true;
			rbtByName.Enabled = false;
			rbtByName.Location = new System.Drawing.Point(14, 26);
			rbtByName.Name = "rbtByName";
			rbtByName.Size = new System.Drawing.Size(163, 17);
			rbtByName.TabIndex = 56;
			rbtByName.Text = "Tên bài hát (Nhạc đính kèm)";
			rbtByName.UseVisualStyleBackColor = true;
			rbtByName.CheckedChanged += new System.EventHandler(rbtByName_CheckedChanged);
			rbtTop.AutoSize = true;
			rbtTop.Enabled = false;
			rbtTop.Location = new System.Drawing.Point(260, 26);
			rbtTop.Name = "rbtTop";
			rbtTop.Size = new System.Drawing.Size(79, 17);
			rbtTop.TabIndex = 57;
			rbtTop.Text = "Thịnh hành";
			rbtTop.UseVisualStyleBackColor = true;
			groupBox2.Controls.Add(rbtNoneMusic);
			groupBox2.Controls.Add(label15);
			groupBox2.Controls.Add(label9);
			groupBox2.Controls.Add(nudVolumnMusic);
			groupBox2.Controls.Add(cbxVideoVolumn);
			groupBox2.Controls.Add(rbtFavorite);
			groupBox2.Controls.Add(txtSongName);
			groupBox2.Controls.Add(rbtTop);
			groupBox2.Controls.Add(rbtByName);
			groupBox2.Location = new System.Drawing.Point(533, 210);
			groupBox2.Name = "groupBox2";
			groupBox2.Size = new System.Drawing.Size(467, 162);
			groupBox2.TabIndex = 58;
			groupBox2.TabStop = false;
			groupBox2.Text = "Cấu hình bài hát đính kèm video";
			rbtNoneMusic.AutoSize = true;
			rbtNoneMusic.Checked = true;
			rbtNoneMusic.Location = new System.Drawing.Point(353, 26);
			rbtNoneMusic.Name = "rbtNoneMusic";
			rbtNoneMusic.Size = new System.Drawing.Size(97, 17);
			rbtNoneMusic.TabIndex = 64;
			rbtNoneMusic.TabStop = true;
			rbtNoneMusic.Text = "Không sử dụng";
			rbtNoneMusic.UseVisualStyleBackColor = true;
			label15.AutoSize = true;
			label15.Location = new System.Drawing.Point(210, 133);
			label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label15.Name = "label15";
			label15.Size = new System.Drawing.Size(15, 13);
			label15.TabIndex = 63;
			label15.Text = "%";
			label9.AutoSize = true;
			label9.Location = new System.Drawing.Point(14, 134);
			label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(134, 13);
			label9.TabIndex = 62;
			label9.Text = "Âm thanh bài hát vừa thêm";
			nudVolumnMusic.Increment = new decimal(new int[4] { 5, 0, 0, 0 });
			nudVolumnMusic.Location = new System.Drawing.Point(153, 130);
			nudVolumnMusic.Name = "nudVolumnMusic";
			nudVolumnMusic.Size = new System.Drawing.Size(52, 20);
			nudVolumnMusic.TabIndex = 61;
			nudVolumnMusic.Value = new decimal(new int[4] { 10, 0, 0, 0 });
			cbxVideoVolumn.AutoSize = true;
			cbxVideoVolumn.Location = new System.Drawing.Point(310, 133);
			cbxVideoVolumn.Margin = new System.Windows.Forms.Padding(2);
			cbxVideoVolumn.Name = "cbxVideoVolumn";
			cbxVideoVolumn.Size = new System.Drawing.Size(139, 17);
			cbxVideoVolumn.TabIndex = 60;
			cbxVideoVolumn.Text = "Tắt âm thanh gốc video";
			cbxVideoVolumn.UseVisualStyleBackColor = true;
			rbtFavorite.AutoSize = true;
			rbtFavorite.Enabled = false;
			rbtFavorite.Location = new System.Drawing.Point(187, 26);
			rbtFavorite.Name = "rbtFavorite";
			rbtFavorite.Size = new System.Drawing.Size(67, 17);
			rbtFavorite.TabIndex = 58;
			rbtFavorite.Text = "Ưa thích";
			rbtFavorite.UseVisualStyleBackColor = true;
			groupBox3.Controls.Add(cbxKichview);
			groupBox3.Controls.Add(nudNumOfPic);
			groupBox3.Controls.Add(rbtPic);
			groupBox3.Controls.Add(rbtVideo);
			groupBox3.Controls.Add(cbxContent);
			groupBox3.Controls.Add(linkLabel2);
			groupBox3.Controls.Add(label14);
			groupBox3.Controls.Add(txtUid);
			groupBox3.Controls.Add(nudPostcount);
			groupBox3.Controls.Add(nudDelay);
			groupBox3.Controls.Add(cbxDeletePic);
			groupBox3.Controls.Add(cbxActive);
			groupBox3.Controls.Add(label12);
			groupBox3.Controls.Add(txtName);
			groupBox3.Controls.Add(label7);
			groupBox3.Controls.Add(label6);
			groupBox3.Controls.Add(label3);
			groupBox3.Controls.Add(btnFolder);
			groupBox3.Controls.Add(label16);
			groupBox3.Controls.Add(label11);
			groupBox3.Controls.Add(label5);
			groupBox3.Controls.Add(label10);
			groupBox3.Controls.Add(label4);
			groupBox3.Controls.Add(txtContents);
			groupBox3.Controls.Add(label2);
			groupBox3.Controls.Add(txtFolder);
			groupBox3.Location = new System.Drawing.Point(14, 30);
			groupBox3.Name = "groupBox3";
			groupBox3.Size = new System.Drawing.Size(505, 342);
			groupBox3.TabIndex = 59;
			groupBox3.TabStop = false;
			groupBox3.Text = "Cấu hình nội dung và hình ảnh";
			cbxKichview.AutoSize = true;
			cbxKichview.Location = new System.Drawing.Point(18, 273);
			cbxKichview.Margin = new System.Windows.Forms.Padding(2);
			cbxKichview.Name = "cbxKichview";
			cbxKichview.Size = new System.Drawing.Size(193, 17);
			cbxKichview.TabIndex = 66;
			cbxKichview.Text = "Bật nút khai báo nội dung bài đăng";
			cbxKichview.UseVisualStyleBackColor = true;
			nudNumOfPic.Location = new System.Drawing.Point(439, 212);
			nudNumOfPic.Maximum = new decimal(new int[4] { 9, 0, 0, 0 });
			nudNumOfPic.Name = "nudNumOfPic";
			nudNumOfPic.Size = new System.Drawing.Size(45, 20);
			nudNumOfPic.TabIndex = 65;
			nudNumOfPic.Value = new decimal(new int[4] { 1, 0, 0, 0 });
			rbtPic.AutoSize = true;
			rbtPic.Location = new System.Drawing.Point(274, 214);
			rbtPic.Name = "rbtPic";
			rbtPic.Size = new System.Drawing.Size(88, 17);
			rbtPic.TabIndex = 64;
			rbtPic.Text = "Bài dạng ảnh";
			rbtPic.UseVisualStyleBackColor = true;
			rbtPic.CheckedChanged += new System.EventHandler(rbtPic_CheckedChanged);
			rbtVideo.AutoSize = true;
			rbtVideo.Checked = true;
			rbtVideo.Location = new System.Drawing.Point(166, 214);
			rbtVideo.Name = "rbtVideo";
			rbtVideo.Size = new System.Drawing.Size(97, 17);
			rbtVideo.TabIndex = 63;
			rbtVideo.TabStop = true;
			rbtVideo.Text = "Bài dạng Video";
			rbtVideo.UseVisualStyleBackColor = true;
			rbtVideo.CheckedChanged += new System.EventHandler(rbtVideo_CheckedChanged);
			cbxContent.AutoSize = true;
			cbxContent.Location = new System.Drawing.Point(17, 215);
			cbxContent.Margin = new System.Windows.Forms.Padding(2);
			cbxContent.Name = "cbxContent";
			cbxContent.Size = new System.Drawing.Size(144, 17);
			cbxContent.TabIndex = 62;
			cbxContent.Text = "Nội dung là tên file Video";
			cbxContent.UseVisualStyleBackColor = true;
			cbxContent.CheckedChanged += new System.EventHandler(cbxContent_CheckedChanged);
			linkLabel2.AutoSize = true;
			linkLabel2.Location = new System.Drawing.Point(403, 278);
			linkLabel2.Name = "linkLabel2";
			linkLabel2.Size = new System.Drawing.Size(63, 13);
			linkLabel2.TabIndex = 61;
			linkLabel2.TabStop = true;
			linkLabel2.Text = "Mở thư mục";
			linkLabel2.Visible = false;
			linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel2_LinkClicked);
			label16.AutoSize = true;
			label16.Location = new System.Drawing.Point(366, 216);
			label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			label16.Name = "label16";
			label16.Size = new System.Drawing.Size(70, 13);
			label16.TabIndex = 8;
			label16.Text = "Số lưởng ảnh";
			linkLabel1.AutoSize = true;
			linkLabel1.Location = new System.Drawing.Point(673, 593);
			linkLabel1.Name = "linkLabel1";
			linkLabel1.Size = new System.Drawing.Size(65, 13);
			linkLabel1.TabIndex = 60;
			linkLabel1.TabStop = true;
			linkLabel1.Text = "Tải csv mẫu";
			linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel1_LinkClicked);
			cbxMarketPlace.AutoSize = true;
			cbxMarketPlace.Location = new System.Drawing.Point(357, 17);
			cbxMarketPlace.Name = "cbxMarketPlace";
			cbxMarketPlace.Size = new System.Drawing.Size(88, 17);
			cbxMarketPlace.TabIndex = 57;
			cbxMarketPlace.Text = "Market Place";
			cbxMarketPlace.UseVisualStyleBackColor = true;
			cbxShop.AutoSize = true;
			cbxShop.Checked = true;
			cbxShop.Location = new System.Drawing.Point(299, 17);
			cbxShop.Name = "cbxShop";
			cbxShop.Size = new System.Drawing.Size(50, 17);
			cbxShop.TabIndex = 58;
			cbxShop.TabStop = true;
			cbxShop.Text = "Shop";
			cbxShop.UseVisualStyleBackColor = true;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(1019, 628);
			base.Controls.Add(linkLabel1);
			base.Controls.Add(groupBox2);
			base.Controls.Add(btnCopy);
			base.Controls.Add(dataGridViewComment);
			base.Controls.Add(txtBaiMoi);
			base.Controls.Add(txtId);
			base.Controls.Add(lblLabelTitle);
			base.Controls.Add(button1);
			base.Controls.Add(btnSave);
			base.Controls.Add(groupBox1);
			base.Controls.Add(groupBox3);
			base.Margin = new System.Windows.Forms.Padding(2);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "frmArticles";
			Text = "Article";
			base.Load += new System.EventHandler(frmArticles_Load);
			((System.ComponentModel.ISupportInitialize)dataGridViewComment).EndInit();
			contextMenuStrip1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)nudDelay).EndInit();
			((System.ComponentModel.ISupportInitialize)nudPostcount).EndInit();
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)nudVolumnMusic).EndInit();
			groupBox3.ResumeLayout(false);
			groupBox3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)nudNumOfPic).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
