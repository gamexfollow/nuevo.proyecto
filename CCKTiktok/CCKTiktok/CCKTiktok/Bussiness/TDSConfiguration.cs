using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace CCKTiktok.Bussiness
{
	public class TDSConfiguration
	{
		public TDSEntity CreateAccount(TDSEntity entity)
		{
			string input = "";
			string text = "application/x-www-form-urlencoded";
			string requestUriString = "https://traodoisub.com/api/reg.php";
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString);
			httpWebRequest.Method = "POST";
			httpWebRequest.Accept = "*/*";
			httpWebRequest.UserAgent = "d78a31d048997dc589e19577c0273498";
			if (!string.IsNullOrWhiteSpace(text))
			{
				httpWebRequest.ContentType = text;
			}
			httpWebRequest.CookieContainer = new CookieContainer();
			entity.password = DataEncrypterDecrypter.GetMD5(entity.user_name);
			byte[] bytes = Encoding.UTF8.GetBytes($"user_name={entity.user_name}&password={entity.password}");
			httpWebRequest.ContentLength = bytes.Length;
			Stream requestStream = httpWebRequest.GetRequestStream();
			requestStream.Write(bytes, 0, bytes.Length);
			requestStream.Close();
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			requestStream = httpWebResponse.GetResponseStream();
			if (requestStream != null)
			{
				StreamReader streamReader = new StreamReader(requestStream);
				input = streamReader.ReadToEnd();
				streamReader.Close();
				requestStream.Close();
				httpWebResponse.Close();
			}
			dynamic val = new JavaScriptSerializer().DeserializeObject(input);
			if (val.ContainsKey("success"))
			{
				entity.token = val["access_token"].ToString();
				entity.IsSuccess = false;
			}
			else if (val.ContainsKey("error"))
			{
				entity.IsSuccess = false;
				entity.Message = val["error"].ToString();
			}
			return entity;
		}

		public DataTable GetAll()
		{
			return new DataTable();
		}
	}
}
