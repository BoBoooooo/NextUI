using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace App.Clue.Entity
{
	[Serializable]
	public class Dept: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("DeptName")]
		public string  DeptName { get; set; }
		[DisplayName("ParentId")]
		public string  ParentId { get; set; }
		[DisplayName("Remark")]
		public string  Remark { get; set; }
		[DisplayName("Sequence")]
		public int?  Sequence { get; set; }
		[DisplayName("Region")]
		public string  Region { get; set; }
		[DisplayName("TimeStamp")]
		public DateTime?  TimeStamp { get; set; }
		[DisplayName("IsDeleted")]
		public bool?  IsDeleted { get; set; }

	}
}
