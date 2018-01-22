using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace App.Clue.Entity
{
	[Serializable]
	public class Log: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("UserID")]
		public string  UserID { get; set; }
		[DisplayName("OptTime")]
		public DateTime?  OptTime { get; set; }
		[DisplayName("IP")]
		public string  IP { get; set; }
		[DisplayName("Region")]
		public string  Region { get; set; }
		[DisplayName("Account")]
		public string  Account { get; set; }
		[DisplayName("OptType")]
		public string  OptType { get; set; }
		[DisplayName("Content")]
		public string  Content { get; set; }

	}
}
