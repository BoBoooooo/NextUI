using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace App.Clue.Entity
{
	[Serializable]
	public class Handle: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("ClueID")]
		public string  ClueID { get; set; }
		[DisplayName("HandleType")]
		public string  HandleType { get; set; }
		[DisplayName("HandleLink")]
		public string  HandleLink { get; set; }
		[DisplayName("TargetName")]
		public string  TargetName { get; set; }
		[DisplayName("Sex")]
		public string  Sex { get; set; }
		[DisplayName("DOB")]
		public string  DOB { get; set; }
		[DisplayName("PartyDate")]
		public string  PartyDate { get; set; }
		[DisplayName("WorkDate")]
		public string  WorkDate { get; set; }
		[DisplayName("Culture")]
		public string  Culture { get; set; }
		[DisplayName("Dept")]
		public string  Dept { get; set; }
		[DisplayName("Post")]
		public string  Post { get; set; }
		[DisplayName("ViolateDate")]
		public string  ViolateDate { get; set; }
		[DisplayName("ClueSource")]
		public string  ClueSource { get; set; }
		[DisplayName("CrimeDate")]
		public string  CrimeDate { get; set; }
		[DisplayName("QuestionNature")]
		public string  QuestionNature { get; set; }
		[DisplayName("CrimeMoney")]
		public string  CrimeMoney { get; set; }
		[DisplayName("UndertakeDate")]
		public string  UndertakeDate { get; set; }
		[DisplayName("MainFact")]
		public string  MainFact { get; set; }
		[DisplayName("InvestigateOpinion")]
		public string  InvestigateOpinion { get; set; }
		[DisplayName("InvestigateOpinionDate")]
		public string  InvestigateOpinionDate { get; set; }
		[DisplayName("UndertakeDeptOpinion")]
		public string  UndertakeDeptOpinion { get; set; }
		[DisplayName("UndertakeDeptOpinionDate")]
		public string  UndertakeDeptOpinionDate { get; set; }
		[DisplayName("MainLeaderOpinion")]
		public string  MainLeaderOpinion { get; set; }
		[DisplayName("MainLeaderOpinionDate")]
		public string  MainLeaderOpinionDate { get; set; }
		[DisplayName("Notes")]
		public string  Notes { get; set; }
		[DisplayName("SubmitDate")]
		public string  SubmitDate { get; set; }
		[DisplayName("Age")]
		public string  Age { get; set; }
		[DisplayName("Nation")]
		public string  Nation { get; set; }
		[DisplayName("UndertakeDeptOpinionSign")]
		public string  UndertakeDeptOpinionSign { get; set; }
		[DisplayName("DeptLeaderOpinion")]
		public string  DeptLeaderOpinion { get; set; }
		[DisplayName("DeptLeaderOpinionSign")]
		public string  DeptLeaderOpinionSign { get; set; }
		[DisplayName("DeptLeaderOpinionDate")]
		public string  DeptLeaderOpinionDate { get; set; }
		[DisplayName("MainLeaderOpinionSign")]
		public string  MainLeaderOpinionSign { get; set; }
		[DisplayName("FillInDept")]
		public string  FillInDept { get; set; }
		[DisplayName("FillInPerson")]
		public string  FillInPerson { get; set; }
		[DisplayName("ClueCode")]
		public string  ClueCode { get; set; }
		[DisplayName("SaveDate")]
		public string  SaveDate { get; set; }
		[DisplayName("RefNum")]
		public string  RefNum { get; set; }
		[DisplayName("DealDept")]
		public string  DealDept { get; set; }
		[DisplayName("DealOpinion")]
		public string  DealOpinion { get; set; }
		[DisplayName("UndertakerOpinion")]
		public string  UndertakerOpinion { get; set; }
		[DisplayName("UndertakeDept")]
		public string  UndertakeDept { get; set; }
		[DisplayName("Undertaker")]
		public string  Undertaker { get; set; }
		[DisplayName("FinishDate")]
		public string  FinishDate { get; set; }
		[DisplayName("TimeStamp")]
		public DateTime?  TimeStamp { get; set; }
		[DisplayName("BasicInfo")]
		public string  BasicInfo { get; set; }
		[DisplayName("ClueSourceAndMainProblem")]
		public string  ClueSourceAndMainProblem { get; set; }
		[DisplayName("HandleMethod")]
		public string  HandleMethod { get; set; }
		[DisplayName("HandleReasonAndProgress")]
		public string  HandleReasonAndProgress { get; set; }
		[DisplayName("Reply")]
		public string  Reply { get; set; }
		[DisplayName("IsDeleted")]
		public bool  IsDeleted { get; set; }

	}
}
