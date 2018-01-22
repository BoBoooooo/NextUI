using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace App.Clue.Entity
{
	[Serializable]
	public class Process: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("ClueID")]
		public string  ClueID { get; set; }
		[DisplayName("ProcessType")]
		public string  ProcessType { get; set; }
		[DisplayName("ProcessLink")]
		public string  ProcessLink { get; set; }
		[DisplayName("FillInDept")]
		public string  FillInDept { get; set; }
		[DisplayName("FillInDate")]
		public string  FillInDate { get; set; }
		[DisplayName("TargetName")]
		public string  TargetName { get; set; }
		[DisplayName("TargetUnitAndPosition")]
		public string  TargetUnitAndPosition { get; set; }
		[DisplayName("Sex")]
		public string  Sex { get; set; }
		[DisplayName("Age")]
		public string  Age { get; set; }
		[DisplayName("PoliticsStatus")]
		public string  PoliticsStatus { get; set; }
		[DisplayName("Nation")]
		public string  Nation { get; set; }
		[DisplayName("ClueSource")]
		public string  ClueSource { get; set; }
		[DisplayName("MainProblem")]
		public string  MainProblem { get; set; }
		[DisplayName("UnderTakeDeptOpinion")]
		public string  UnderTakeDeptOpinion { get; set; }
		[DisplayName("UnderTakeDeptSign")]
		public string  UnderTakeDeptSign { get; set; }
		[DisplayName("UnderTakeDeptDate")]
		public string  UnderTakeDeptDate { get; set; }
		[DisplayName("ChargeLeaderOpinion")]
		public string  ChargeLeaderOpinion { get; set; }
		[DisplayName("ChargeLeaderSign")]
		public string  ChargeLeaderSign { get; set; }
		[DisplayName("ChargeLeaderDate")]
		public string  ChargeLeaderDate { get; set; }
		[DisplayName("DeputySecretaryOpinion")]
		public string  DeputySecretaryOpinion { get; set; }
		[DisplayName("DeputySecretarySign")]
		public string  DeputySecretarySign { get; set; }
		[DisplayName("DeputySecretaryDate")]
		public string  DeputySecretaryDate { get; set; }
		[DisplayName("MainLeaderOpinion")]
		public string  MainLeaderOpinion { get; set; }
		[DisplayName("MainLeaderSign")]
		public string  MainLeaderSign { get; set; }
		[DisplayName("MainLeaderDate")]
		public string  MainLeaderDate { get; set; }
		[DisplayName("Note")]
		public string  Note { get; set; }
		[DisplayName("LetterDate")]
		public string  LetterDate { get; set; }
		[DisplayName("ReplyDate")]
		public string  ReplyDate { get; set; }
		[DisplayName("UnderTakeDept")]
		public string  UnderTakeDept { get; set; }
		[DisplayName("UnderTakeStaff")]
		public string  UnderTakeStaff { get; set; }
		[DisplayName("ReplyContent")]
		public string  ReplyContent { get; set; }
		[DisplayName("TalkStaff")]
		public string  TalkStaff { get; set; }
		[DisplayName("TalkDate")]
		public string  TalkDate { get; set; }
		[DisplayName("TalkLoc")]
		public string  TalkLoc { get; set; }
		[DisplayName("RecordStaff")]
		public string  RecordStaff { get; set; }
		[DisplayName("TalkContent")]
		public string  TalkContent { get; set; }
		[DisplayName("TargetUnit")]
		public string  TargetUnit { get; set; }
		[DisplayName("TargetPosition")]
		public string  TargetPosition { get; set; }
		[DisplayName("DOB")]
		public string  DOB { get; set; }
		[DisplayName("PartyDate")]
		public string  PartyDate { get; set; }
		[DisplayName("WorkDate")]
		public string  WorkDate { get; set; }
		[DisplayName("Education")]
		public string  Education { get; set; }
		[DisplayName("NatureProblem")]
		public string  NatureProblem { get; set; }
		[DisplayName("CrimeDate")]
		public string  CrimeDate { get; set; }
		[DisplayName("CaseFormedDate")]
		public string  CaseFormedDate { get; set; }
		[DisplayName("CaseName")]
		public string  CaseName { get; set; }
		[DisplayName("TrialOpinion")]
		public string  TrialOpinion { get; set; }
		[DisplayName("TrialSign")]
		public string  TrialSign { get; set; }
		[DisplayName("TrialDate")]
		public string  TrialDate { get; set; }
		[DisplayName("TrialLeaderOpinion")]
		public string  TrialLeaderOpinion { get; set; }
		[DisplayName("TrialLeaderSign")]
		public string  TrialLeaderSign { get; set; }
		[DisplayName("TrialLeaderDate")]
		public string  TrialLeaderDate { get; set; }
		[DisplayName("TrialDeputySecretaryOpinion")]
		public string  TrialDeputySecretaryOpinion { get; set; }
		[DisplayName("TrialDeputySecretarySign")]
		public string  TrialDeputySecretarySign { get; set; }
		[DisplayName("TrialDeputySecretaryDate")]
		public string  TrialDeputySecretaryDate { get; set; }
		[DisplayName("FilingCaseDate")]
		public string  FilingCaseDate { get; set; }
		[DisplayName("ProblemList")]
		public string  ProblemList { get; set; }
		[DisplayName("SendUnit")]
		public string  SendUnit { get; set; }
		[DisplayName("SendStaff")]
		public string  SendStaff { get; set; }
		[DisplayName("SendDate")]
		public string  SendDate { get; set; }
		[DisplayName("ReceiveUnit")]
		public string  ReceiveUnit { get; set; }
		[DisplayName("ReceiveStaff")]
		public string  ReceiveStaff { get; set; }
		[DisplayName("ReceiveDate")]
		public string  ReceiveDate { get; set; }
		[DisplayName("HandleFact")]
		public string  HandleFact { get; set; }
		[DisplayName("UnhandleFact")]
		public string  UnhandleFact { get; set; }
		[DisplayName("ExtraFact")]
		public string  ExtraFact { get; set; }
		[DisplayName("TimeStamp")]
		public DateTime?  TimeStamp { get; set; }
		[DisplayName("IsDeleted")]
		public bool  IsDeleted { get; set; }

	}
}
