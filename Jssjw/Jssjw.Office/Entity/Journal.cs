using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace Jssjw.Office.Entity
{
	[Serializable]
	public class Journal: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("UserID")]
		public string  UserID { get; set; }
        [DisplayName("FullName")]
        public string FullName { get; set; }
		[DisplayName("DeptID")]
		public string  DeptID { get; set; }
        [DisplayName("DeptName")]
        public string DeptName { get; set; }
		[DisplayName("RecordDate")]
		public DateTime?  RecordDate { get; set; }
		[DisplayName("Content")]
		public string  Content { get; set; }

        [DisplayName("TextContent")]
        public string TextContent { get; set; }

        [DisplayName("AttachmentID")]
        public string AttachmentID { get; set; }

		[DisplayName("Deleted")]

		public bool  Deleted { get; set; }

	}
}
