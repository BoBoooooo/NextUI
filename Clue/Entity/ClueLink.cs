using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace App.Clue.Entity
{
	[Serializable]
	public class ClueLink: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("Prev")]
		public string  Prev { get; set; }
		[DisplayName("Next")]
		public string  Next { get; set; }
		[DisplayName("ClueInfoID")]
		public string  ClueInfoID { get; set; }
		[DisplayName("ApprovalID")]
		public string  ApprovalID { get; set; }
		[DisplayName("HandleID")]
		public string  HandleID { get; set; }
		[DisplayName("HandleRegID")]
		public string  HandleRegID { get; set; }
		[DisplayName("HandleDept")]
		public string  HandleDept { get; set; }
		[DisplayName("HandleStaff")]
		public string  HandleStaff { get; set; }
		[DisplayName("Status")]
		public string  Status { get; set; }
		[DisplayName("TimeStamp")]
		public DateTime?  TimeStamp { get; set; }
		[DisplayName("IsDeleted")]
		public bool  IsDeleted { get; set; }

	}
}
