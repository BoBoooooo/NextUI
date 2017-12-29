using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace Next.WorkFlow.Entity
{
	[Serializable]
	public class AppLibrary: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("Title")]
		public string  Title { get; set; }
		[DisplayName("Address")]
		public string  Address { get; set; }
		[DisplayName("Type")]
		public string  Type { get; set; }
		[DisplayName("OpenMode")]
		public int?  OpenMode { get; set; }
		[DisplayName("Width")]
		public int?  Width { get; set; }
		[DisplayName("Height")]
		public int?  Height { get; set; }
		[DisplayName("Params")]
		public string  Params { get; set; }
		[DisplayName("Manager")]
		public string  Manager { get; set; }
		[DisplayName("Note")]
		public string  Note { get; set; }
		[DisplayName("Code")]
		public string  Code { get; set; }
		[DisplayName("UseMember")]
		public string  UseMember { get; set; }

	}
}
