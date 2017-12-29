using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace Next.WorkFlow.Entity
{
	[Serializable]
	public class WorkFlowArchives: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("FlowID")]
		public string  FlowID { get; set; }
		[DisplayName("StepID")]
		public string  StepID { get; set; }
		[DisplayName("FlowName")]
		public string  FlowName { get; set; }
		[DisplayName("StepName")]
		public string  StepName { get; set; }
		[DisplayName("TaskID")]
		public string  TaskID { get; set; }
		[DisplayName("GroupID")]
		public string  GroupID { get; set; }
		[DisplayName("InstanceID")]
		public string  InstanceID { get; set; }
		[DisplayName("Title")]
		public string  Title { get; set; }
		[DisplayName("Contents")]
		public string  Contents { get; set; }
		[DisplayName("Comments")]
		public string  Comments { get; set; }
		[DisplayName("WriteTime")]
		public DateTime?  WriteTime { get; set; }

	}
}
