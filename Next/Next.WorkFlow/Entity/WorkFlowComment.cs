using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace Next.WorkFlow.Entity
{
	[Serializable]
	public class WorkFlowComment: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("MemberID")]
		public string  MemberID { get; set; }
		[DisplayName("Comment")]
		public string  Comment { get; set; }
		[DisplayName("Type")]
		public int  Type { get; set; }
		[DisplayName("Sort")]
		public int  Sort { get; set; }

	}
}
