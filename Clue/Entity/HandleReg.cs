using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace App.Clue.Entity
{
	[Serializable]
	public class HandleReg: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("ClueID")]
		public string  ClueID { get; set; }
		[DisplayName("FillInDept")]
		public string  FillInDept { get; set; }
		[DisplayName("FillInDate")]
		public DateTime?  FillInDate { get; set; }
		[DisplayName("ClueCode")]
		public string  ClueCode { get; set; }
		[DisplayName("StaffCode")]
		public string  StaffCode { get; set; }
		[DisplayName("DeptOrAccidentClue")]
		public string  DeptOrAccidentClue { get; set; }
		[DisplayName("TargetName")]
		public string  TargetName { get; set; }
		[DisplayName("DOB")]
		public string  DOB { get; set; }
		[DisplayName("Nation")]
		public string  Nation { get; set; }
		[DisplayName("Political")]
		public string  Political { get; set; }
		[DisplayName("PartyDate")]
		public string  PartyDate { get; set; }
		[DisplayName("PostType1")]
		public string  PostType1 { get; set; }
		[DisplayName("PostType2")]
		public string  PostType2 { get; set; }
		[DisplayName("Dept")]
		public string  Dept { get; set; }
		[DisplayName("NPCMember")]
		public string  NPCMember { get; set; }
		[DisplayName("CPPCCmember")]
		public string  CPPCCmember { get; set; }
		[DisplayName("AdminObject")]
		public string  AdminObject { get; set; }
		[DisplayName("PublicServant")]
		public string  PublicServant { get; set; }
		[DisplayName("ClueSource")]
		public string  ClueSource { get; set; }
		[DisplayName("ProblemType")]
		public string  ProblemType { get; set; }
		[DisplayName("HandleUnit")]
		public string  HandleUnit { get; set; }
		[DisplayName("AcceptDate")]
		public DateTime?  AcceptDate { get; set; }
		[DisplayName("HandleDate1")]
		public string  HandleDate1 { get; set; }
		[DisplayName("HandleMethod1")]
		public string  HandleMethod1 { get; set; }
		[DisplayName("HandleDate2")]
		public string  HandleDate2 { get; set; }
		[DisplayName("HandleMethod2")]
		public string  HandleMethod2 { get; set; }
		[DisplayName("HandleDate3")]
		public string  HandleDate3 { get; set; }
		[DisplayName("HandleMethod3")]
		public string  HandleMethod3 { get; set; }
		[DisplayName("SaveMoneyLosses")]
		public string  SaveMoneyLosses { get; set; }
		[DisplayName("TakeoverMoney")]
		public string  TakeoverMoney { get; set; }
		[DisplayName("MainClue")]
		public string  MainClue { get; set; }
		[DisplayName("HandleReport")]
		public string  HandleReport { get; set; }
		[DisplayName("Notes")]
		public string  Notes { get; set; }
		[DisplayName("FillInStaff")]
		public string  FillInStaff { get; set; }
		[DisplayName("Checker")]
		public string  Checker { get; set; }
		[DisplayName("TimeStamp")]
		public DateTime?  TimeStamp { get; set; }
		[DisplayName("IsDeleted")]
		public bool  IsDeleted { get; set; }

	}
}
