using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace App.Clue.Entity
{
	[Serializable]
	public class Node: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("IP")]
		public string  IP { get; set; }
		[DisplayName("Region")]
		public string  Region { get; set; }
		[DisplayName("ReceiveTime")]
		public DateTime?  ReceiveTime { get; set; }
		[DisplayName("Status")]
		public string  Status { get; set; }
		[DisplayName("ParentIP")]
		public string  ParentIP { get; set; }
		[DisplayName("RootID")]
		public string  RootID { get; set; }
		[DisplayName("IsSync")]
		public bool?  IsSync { get; set; }
		[DisplayName("UserID")]
		public string  UserID { get; set; }
		[DisplayName("DeptID")]
		public string  DeptID { get; set; }
		[DisplayName("TimeStamp")]
		public DateTime?  TimeStamp { get; set; }

	}
}
