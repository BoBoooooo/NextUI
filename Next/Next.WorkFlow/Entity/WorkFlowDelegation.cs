using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace Next.WorkFlow.Entity
{
	[Serializable]
	public class WorkFlowDelegation: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("UserID")]
		public string  UserID { get; set; }
		[DisplayName("StartTime")]
		public DateTime?  StartTime { get; set; }
		[DisplayName("EndTime")]
		public DateTime?  EndTime { get; set; }
		[DisplayName("FlowID")]
		public string  FlowID { get; set; }
		[DisplayName("ToUserID")]
		public string  ToUserID { get; set; }
		[DisplayName("WriteTime")]
		public DateTime?  WriteTime { get; set; }
		[DisplayName("Note")]
		public string  Note { get; set; }

	}
}
