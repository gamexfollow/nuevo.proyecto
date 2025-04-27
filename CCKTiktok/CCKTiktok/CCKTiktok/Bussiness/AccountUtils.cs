using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using CCKTiktok.Helper;

namespace CCKTiktok.Bussiness
{
	internal class AccountUtils
	{
		public static void Backup(string devicesID, string uid)
		{
			ADBHelperCCK.ExecuteCMD(devicesID, " shell \"rm /data/data/cache/*");
			ADBHelperCCK.ExecuteCMD(devicesID, "shell \"rm /data/data/*.tar\"");
			List<string> list = new List<string>();
			string pACKAGE_NAME = CaChuaConstant.PACKAGE_NAME;
			foreach (string item in list)
			{
				ADBHelperCCK.ExecuteCMD(devicesID, "shell \"rm -r /data/data/" + pACKAGE_NAME + "/" + item + "\"");
			}
			ADBHelperCCK.ExecuteCMD(devicesID, "shell \"cd /data/data/; tar -cvf " + pACKAGE_NAME + ".tar " + pACKAGE_NAME + "/\"");
			string path = Application.StartupPath + "\\Authentication\\" + uid;
			if (Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			ADBHelperCCK.ExecuteCMD(devicesID, " pull /data/data/" + pACKAGE_NAME + ".tar \"" + Application.StartupPath + "\\Authentication\\" + uid + "\"");
		}

		public static void Restore(string devicesID, string uid)
		{
			string pACKAGE_NAME = CaChuaConstant.PACKAGE_NAME;
			string text = Application.StartupPath + "\\Authentication\\" + uid;
			ADBHelperCCK.ExecuteCMD(devicesID, "shell \"pm clear " + pACKAGE_NAME + "\"");
			ADBHelperCCK.ExecuteCMD(devicesID, " shell rm /data/data/*.tar");
			ADBHelperCCK.ExecuteCMD(devicesID, " push \"" + text + "\\" + pACKAGE_NAME + ".tar\" /data/data/");
			ADBHelperCCK.ExecuteCMD(devicesID, " shell \"cd /data/data/; tar -xvf " + pACKAGE_NAME + ".tar; exit\"");
			ADBHelperCCK.ExecuteCMD(devicesID, " shell \"am force-stop " + pACKAGE_NAME + "\"");
		}
	}
}
