using System;
using System.IO;
using System.Text.RegularExpressions;
using xNet;

namespace CCKTiktok.Bussiness
{
	public class VideoRender
	{
		public static void DownLoadTiktokVideo(string FileName, string VideoURL)
		{
			HttpRequest httpRequest = new HttpRequest();
			httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/37.0.2049.0 Safari/537.36";
			string input = httpRequest.Get("https://tikmate.online/").ToString();
			string text = Regex.Match(input, "(?<=name=\"token\" value=\").*?(?=\")").ToString();
			string str = "url=" + Uri.EscapeDataString(VideoURL) + "&token=" + text;
			httpRequest.Post("https://tikmate.online/abc.php", str, "application/x-www-form-urlencoded").ToString();
			string address = "https://tikmate.online/dl.php";
			httpRequest.ConnectTimeout = 99999999;
			httpRequest.KeepAlive = true;
			httpRequest.ReadWriteTimeout = 99999999;
			httpRequest.Get(address);
			byte[] bytes = httpRequest.Get(address).ToMemoryStream().ToArray();
			File.WriteAllBytes(FileName, bytes);
		}

		public static void DouyinVideoDownload(string VideoID, string FileName)
		{
			HttpRequest httpRequest = new HttpRequest();
			httpRequest.ConnectTimeout = 99999999;
			httpRequest.KeepAlive = true;
			httpRequest.ReadWriteTimeout = 99999999;
			httpRequest.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_4_5 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.1.1 Mobile/15E148 Safari/604.1";
			string input = httpRequest.Get("https://www.iesdouyin.com/web/api/v2/aweme/iteminfo/?item_ids=" + VideoID).ToString();
			input = Regex.Match(Regex.Match(input, "(?<=\"play_addr\":).*?(?=]})").ToString(), "(?<=\"url_list\":\\[\").*?(?=\")").ToString();
			byte[] bytes = httpRequest.Get(input.Replace("playwm", "play")).ToMemoryStream().ToArray();
			File.WriteAllBytes(FileName, bytes);
		}

		public static string GetLinkDouyin(string VideoURL)
		{
			HttpRequest httpRequest = new HttpRequest();
			httpRequest.ConnectTimeout = 99999999;
			httpRequest.KeepAlive = true;
			httpRequest.ReadWriteTimeout = 99999999;
			httpRequest.AllowAutoRedirect = false;
			VideoURL = httpRequest.Get(VideoURL).ToString();
			return Regex.Match(VideoURL, "(?<=href=\").*?(?=\")").ToString();
		}
	}
}
