using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace Next.Admin.Entity
{
	[Serializable]
	public class LoginLog: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("UserID")]
		public string  UserID { get; set; }
		[DisplayName("LoginName")]
		public string  LoginName { get; set; }
		[DisplayName("FullName")]
		public string  FullName { get; set; }
		[DisplayName("CompanyID")]
		public string  CompanyID { get; set; }
		[DisplayName("CompanyName")]
		public string  CompanyName { get; set; }
		[DisplayName("Note")]
		public string  Note { get; set; }
		[DisplayName("IPAddress")]
		public string  IPAddress { get; set; }
		[DisplayName("MacAddress")]
		public string  MacAddress { get; set; }
		[DisplayName("LastUpdated")]
		public DateTime?  LastUpdated { get; set; }
		[DisplayName("SystemTypeID")]
		public string  SystemTypeID { get; set; }

	}
}
