using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace Jssjw.Organization.Entity
{
	[Serializable]
	public class Loan: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
        [DisplayName("UserID")]
		public string  UserID { get; set; }

        [DisplayName("DeptID")]
        public string DeptID { get; set; }
        [DisplayName("AttachmentID")]
        public string AttachmentID { get; set; }
		[DisplayName("BorrowUnit")]
		public string  BorrowUnit { get; set; }
		[DisplayName("BorrowUnitID")]
		public string  BorrowUnitID { get; set; }
		[DisplayName("Type")]
		public string  Type { get; set; }
		[DisplayName("BorrowRoomID")]
		public string  BorrowRoomID { get; set; }
		[DisplayName("BorrowRoom")]
		public string  BorrowRoom { get; set; }
		[DisplayName("Usings")]
		public string  Usings { get; set; }
		[DisplayName("Gender")]
		public string  Gender { get; set; }
		[DisplayName("PersonName")]
		public string  PersonName { get; set; }
		[DisplayName("Area")]
		public string  Area { get; set; }
		[DisplayName("Skill")]
		public string  Skill { get; set; }
		[DisplayName("Unit")]
		public string  Unit { get; set; }
		[DisplayName("WorkDuty")]
		public string  WorkDuty { get; set; }
		[DisplayName("WorkLevel")]
		public string  WorkLevel { get; set; }
		[DisplayName("BorrowTime")]
		public string  BorrowTime { get; set; }
		[DisplayName("StartTime")]
		public string  StartTime { get; set; }
		[DisplayName("EndTime")]
		public string  EndTime { get; set; }
		[DisplayName("DoNumber")]
		public string  DoNumber { get; set; }
		[DisplayName("IsDistributed")]
		public string  IsDistributed { get; set; }
		[DisplayName("CardNumber")]
		public string  CardNumber { get; set; }
		[DisplayName("HandlePerson")]
		public string  HandlePerson { get; set; }
		[DisplayName("IsRecycled")]
		public string  IsRecycled { get; set; }
		[DisplayName("State")]
		public string  State { get; set; }
		[DisplayName("IsDeleted")]
		public bool  IsDeleted { get; set; }
		[DisplayName("Remark")]
		public string  Remark { get; set; }
		[DisplayName("Timestamp")]
		public DateTime?  Timestamp { get; set; }
		[DisplayName("Orders")]
		public string  Orders { get; set; }
		[DisplayName("IsIdentified")]
		public string  IsIdentified { get; set; }

	}
}
