using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace Jssjw.Organization.Entity
{
	[Serializable]
	public class Affix: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("MasterID")]
		public string  MasterID { get; set; }
		[DisplayName("Prev")]
		public string  Prev { get; set; }
		[DisplayName("Next")]
		public string  Next { get; set; }
		[DisplayName("Type")]
		public string  Type { get; set; }
		[DisplayName("FileName")]
		public string  FileName { get; set; }
		[DisplayName("SaveName")]
		public string  SaveName { get; set; }
		[DisplayName("FileExtension")]
		public string  FileExtension { get; set; }
		[DisplayName("ContentType")]
		public string  ContentType { get; set; }
		[DisplayName("Content")]
		public string Content { get; set; }
		[DisplayName("IsDeleted")]
		public bool  IsDeleted { get; set; }
		[DisplayName("Timestamp")]
		public DateTime?  Timestamp { get; set; }
		[DisplayName("TableType")]
		public string  TableType { get; set; }

	}
}
