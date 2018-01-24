using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace App.Clue.Entity
{
	[Serializable]
	public class ClueInfo: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("FlowID")]
		public string  FlowID { get; set; }
		[DisplayName("TaskID")]
		public string  TaskID { get; set; }
		[DisplayName("UserID")]
		public string  UserID { get; set; }
		[DisplayName("DeptID")]
		public string  DeptID { get; set; }
		[DisplayName("AttachmentID")]
		public string  AttachmentID { get; set; }
		[DisplayName("ClueCode")]
		public string  ClueCode { get; set; }
		[DisplayName("BarCode")]
		public string  BarCode { get; set; }
		[DisplayName("ClueMethod")]
		public string  ClueMethod { get; set; }
		[DisplayName("ClueDate")]
		public DateTime?  ClueDate { get; set; }
		[DisplayName("ClueType")]
		public string  ClueType { get; set; }
		[DisplayName("ClueSource")]
		public string  ClueSource { get; set; }
		[DisplayName("CluePersonCount")]
		public int?  CluePersonCount { get; set; }
		[DisplayName("IsEmergency")]
		public string  IsEmergency { get; set; }
		[DisplayName("TargetType")]
		public string  TargetType { get; set; }
		[DisplayName("TargetName")]
		public string  TargetName { get; set; }
		[DisplayName("TargetUnitAndAddress")]
		public string  TargetUnitAndAddress { get; set; }
		[DisplayName("TargetRank")]
		public string  TargetRank { get; set; }
		[DisplayName("TargetUnitNature")]
		public string  TargetUnitNature { get; set; }
		[DisplayName("TargetArea")]
		public string  TargetArea { get; set; }
		[DisplayName("TargetDuty")]
		public string  TargetDuty { get; set; }
		[DisplayName("TargetMonitor")]
		public string  TargetMonitor { get; set; }
		[DisplayName("TargetIsLeader")]
		public string  TargetIsLeader { get; set; }
		[DisplayName("TargetIsCurrentLevelOfficer")]
		public string  TargetIsCurrentLevelOfficer { get; set; }
		[DisplayName("TargetIDCard")]
		public string  TargetIDCard { get; set; }
		[DisplayName("TargetSex")]
		public string  TargetSex { get; set; }
		[DisplayName("TargetDOB")]
		public DateTime?  TargetDOB { get; set; }
		[DisplayName("TargetNation")]
		public string  TargetNation { get; set; }
		[DisplayName("ReporterIsSign")]
		public string  ReporterIsSign { get; set; }
		[DisplayName("ReporterName")]
		public string  ReporterName { get; set; }
		[DisplayName("ReporterUnitAndAddress")]
		public string  ReporterUnitAndAddress { get; set; }
		[DisplayName("ReporterRank")]
		public string  ReporterRank { get; set; }
		[DisplayName("ReporterPhone")]
		public string  ReporterPhone { get; set; }
		[DisplayName("ReporterArea")]
		public string  ReporterArea { get; set; }
		[DisplayName("ReporterDuty")]
		public string  ReporterDuty { get; set; }
		[DisplayName("ReporterEmail")]
		public string  ReporterEmail { get; set; }
		[DisplayName("ReporterPostCode")]
		public string  ReporterPostCode { get; set; }
		[DisplayName("ReporterAddress")]
		public string  ReporterAddress { get; set; }
		[DisplayName("ReporterNation")]
		public string  ReporterNation { get; set; }
		[DisplayName("ReporterSex")]
		public string  ReporterSex { get; set; }
		[DisplayName("ReporterPoliticsStatus")]
		public string  ReporterPoliticsStatus { get; set; }
		[DisplayName("ReporterJob")]
		public string  ReporterJob { get; set; }
		[DisplayName("ReporterIDCard")]
		public string  ReporterIDCard { get; set; }
		[DisplayName("ContentType")]
		public string  ContentType { get; set; }
		[DisplayName("ContentArea")]
		public string  ContentArea { get; set; }
		[DisplayName("Keyword")]
		public string  Keyword { get; set; }
		[DisplayName("Content")]
		public string  Content { get; set; }
		[DisplayName("Note")]
		public string  Note { get; set; }
		[DisplayName("Reply")]
		public string  Reply { get; set; }
		[DisplayName("IsDeleted")]
		public bool  IsDeleted { get; set; }

	}
}
