using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace Next.WorkFlow.Entity
{
	[Serializable]
	public class WorkFlowTask: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("PrevID")]
		public string  PrevID { get; set; }
		[DisplayName("PrevStepID")]
		public string  PrevStepID { get; set; }
		[DisplayName("FlowID")]
		public string  FlowID { get; set; }
		[DisplayName("StepID")]
		public string  StepID { get; set; }
		[DisplayName("StepName")]
		public string  StepName { get; set; }
		[DisplayName("InstanceID")]
		public string  InstanceID { get; set; }
		[DisplayName("GroupID")]
		public string  GroupID { get; set; }
		[DisplayName("Type")]
		public int?  Type { get; set; }
		[DisplayName("Title")]
		public string  Title { get; set; }
		[DisplayName("SenderID")]
		public string  SenderID { get; set; }
		[DisplayName("SenderName")]
		public string  SenderName { get; set; }
		[DisplayName("SenderTime")]
		public DateTime?  SenderTime { get; set; }
		[DisplayName("ReceiveID")]
		public string  ReceiveID { get; set; }
		[DisplayName("ReveiveName")]
		public string  ReveiveName { get; set; }
		[DisplayName("ReceiveTime")]
		public DateTime?  ReceiveTime { get; set; }
		[DisplayName("OpenTime")]
		public DateTime?  OpenTime { get; set; }
		[DisplayName("CompletedTime")]
		public DateTime?  CompletedTime { get; set; }
		[DisplayName("CompletedTime1")]
		public DateTime?  CompletedTime1 { get; set; }
		[DisplayName("Comment")]
		public string  Comment { get; set; }
		[DisplayName("IsSign")]
		public int?  IsSign { get; set; }
		[DisplayName("Status")]
		public int?  Status { get; set; }
		[DisplayName("Note")]
		public string  Note { get; set; }
		[DisplayName("Sort")]
		public int?  Sort { get; set; }
		[DisplayName("SubFlowGroupID")]
		public string  SubFlowGroupID { get; set; }

	}
}
