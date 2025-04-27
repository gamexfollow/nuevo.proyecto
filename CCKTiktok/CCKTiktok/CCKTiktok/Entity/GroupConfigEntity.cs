using System;
using System.Collections.Generic;

namespace CCKTiktok.Entity
{
	public class GroupConfigEntity
	{
		public JoinType Type { get; set; }

		public int MemberCount { get; set; }

		public decimal NumberOfGroupWantJoin { get; set; }

		public decimal ActiveInJointGroup_Number { get; set; }

		public bool IsRemoveAfterUse { get; set; }

		public bool OnlyJoinUnapprovedGroup { get; set; }

		public bool Is_Leave_ApprovedGroup { get; set; }

		public decimal Leave_ApprovedGroup_Number { get; set; }

		public List<string> Answer { get; set; }

		public bool Is_Leave_AllGroup { get; set; }

		public bool Is_Leave_Uid { get; set; }

		public int Is_Leave_Uid_Number { get; set; }

		public decimal Leave_AllGroup_Number { get; set; }

		public string BackupDomain { get; set; }

		public GroupConfigEntity()
		{
			IsRemoveAfterUse = true;
			ActiveInJointGroup_Number = 5m;
			NumberOfGroupWantJoin = 5m;
			OnlyJoinUnapprovedGroup = true;
			Is_Leave_ApprovedGroup = true;
			Is_Leave_AllGroup = false;
			Leave_ApprovedGroup_Number = 5m;
			Leave_AllGroup_Number = 5m;
			MemberCount = 0;
			Answer = new List<string>();
			BackupDomain = "www";
			Type = JoinType.UID;
			Is_Leave_Uid = false;
		}

		public void Dispose()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
	}
}
