using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace Next.WorkFlow.Entity
{
	[Serializable]
	public class DBConnection: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("UserID")]
		public string  UserID { get; set; }
		[DisplayName("DeptID")]
		public string  DeptID { get; set; }
		[DisplayName("AttachmentID")]
		public string  AttachmentID { get; set; }
		[DisplayName("Name")]
		public string  Name { get; set; }
		[DisplayName("Type")]
		public string  Type { get; set; }
		[DisplayName("ConnectionString")]
		public string  ConnectionString { get; set; }
		[DisplayName("Note")]
		public string  Note { get; set; }
		[DisplayName("IsDeleted")]
		public bool  IsDeleted { get; set; }

	}
}
