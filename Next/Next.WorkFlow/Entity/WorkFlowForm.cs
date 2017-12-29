using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace Next.WorkFlow.Entity
{
	[Serializable]
	public class WorkFlowForm: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("Name")]
		public string  Name { get; set; }
		[DisplayName("Type")]
		public string  Type { get; set; }
		[DisplayName("CreateUserID")]
		public string  CreateUserID { get; set; }
		[DisplayName("CreateUserName")]
		public string  CreateUserName { get; set; }
		[DisplayName("CreateTime")]
		public DateTime?  CreateTime { get; set; }
		[DisplayName("LastModifyTime")]
		public DateTime?  LastModifyTime { get; set; }
		[DisplayName("Html")]
		public string  Html { get; set; }
		[DisplayName("SubTableJson")]
		public string  SubTableJson { get; set; }
		[DisplayName("EventsJson")]
		public string  EventsJson { get; set; }
		[DisplayName("Attribute")]
		public string  Attribute { get; set; }
		[DisplayName("Status")]
		public int?  Status { get; set; }

	}
}
