using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace Next.WorkFlow.Entity
{
	[Serializable]
	public class Dict: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("ParentID")]
		public string  ParentID { get; set; }
		[DisplayName("Title")]
		public string  Title { get; set; }
		[DisplayName("Code")]
		public string  Code { get; set; }
		[DisplayName("Value")]
		public string  Value { get; set; }
		[DisplayName("Note")]
		public string  Note { get; set; }
		[DisplayName("Other")]
		public string  Other { get; set; }
		[DisplayName("Sort")]
		public int?  Sort { get; set; }

	}
}
