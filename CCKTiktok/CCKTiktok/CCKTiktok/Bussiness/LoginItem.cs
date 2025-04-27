using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace CCKTiktok.Bussiness
{
	public class LoginItem
	{
		public string Session { get; set; }

		public string Code { get; set; }

		public string HDDCode { get; set; }

		public string Phone { get; set; }

		public string Pass { get; set; }

		public List<Permission> Role { get; set; }

		public bool IsActive { get; set; }

		public LoginItem()
		{
			IsActive = false;
			Role = new List<Permission>();
			Pass = "";
			Phone = "";
			Code = "";
			HDDCode = "";
			Session = "";
		}

		public bool IsPermission(int permissionId)
		{
			foreach (Permission item in Role)
			{
				if (item.PermissionId == permissionId && item.IsActive)
				{
					return true;
				}
			}
			return false;
		}

		public override string ToString()
		{
			return new JavaScriptSerializer().Serialize(this);
		}
	}
}
