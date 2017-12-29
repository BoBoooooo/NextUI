using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace Next.WorkFlow.Entity
{
	[Serializable]
	public class WorkGroup: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("Name")]
		public string  Name { get; set; }
		[DisplayName("Members")]
		public string  Members { get; set; }
		[DisplayName("Note")]
		public string  Note { get; set; }

	}
}
