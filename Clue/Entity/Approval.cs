using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace App.Clue.Entity
{
	[Serializable]
	public class Approval: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("TaskID")]
		public string  TaskID { get; set; }
		[DisplayName("UserID")]
		public string  UserID { get; set; }
		[DisplayName("DeptID")]
		public string  DeptID { get; set; }
		[DisplayName("AttachmentID")]
		public string  AttachmentID { get; set; }
		[DisplayName("FillInDate")]
		public DateTime?  FillInDate { get; set; }
		[DisplayName("ClueMethod")]
		public string  ClueMethod { get; set; }
		[DisplayName("ClueSource")]
		public string  ClueSource { get; set; }
		[DisplayName("LetterNumber")]
		public string  LetterNumber { get; set; }
		[DisplayName("TargetName")]
		public string  TargetName { get; set; }
		[DisplayName("TargetAddressAndUnit")]
		public string  TargetAddressAndUnit { get; set; }
		[DisplayName("TargetDuty")]
		public string  TargetDuty { get; set; }
		[DisplayName("TargetRank")]
		public string  TargetRank { get; set; }
		[DisplayName("ReporterName")]
		public string  ReporterName { get; set; }
		[DisplayName("ReporterAddressAndUnit")]
		public string  ReporterAddressAndUnit { get; set; }
		[DisplayName("ReporterDuty")]
		public string  ReporterDuty { get; set; }
		[DisplayName("ReporterPhone")]
		public string  ReporterPhone { get; set; }
		[DisplayName("Content")]
		public string  Content { get; set; }
		[DisplayName("UnderTaker")]
		public string  UnderTaker { get; set; }
		[DisplayName("DeviseOpinion")]
		public string  DeviseOpinion { get; set; }
		[DisplayName("DeptOpinion")]
		public string  DeptOpinion { get; set; }
		[DisplayName("LeaderOpinion")]
		public string  LeaderOpinion { get; set; }
		[DisplayName("ClueCode")]
		public string  ClueCode { get; set; }
		[DisplayName("IsDeleted")]
		public bool  IsDeleted { get; set; }

	}
}
