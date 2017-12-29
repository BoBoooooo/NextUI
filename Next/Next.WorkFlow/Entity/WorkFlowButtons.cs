using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace Next.WorkFlow.Entity
{
	[Serializable]
	public class WorkFlowButtons: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("Title")]
		public string  Title { get; set; }
		[DisplayName("Ico")]
		public string  Ico { get; set; }
		[DisplayName("Script")]
		public string  Script { get; set; }
		[DisplayName("Note")]
		public string  Note { get; set; }
		[DisplayName("Sort")]
		public int?  Sort { get; set; }

	}
}
