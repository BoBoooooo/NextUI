using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace Next.WorkFlow.Entity
{
	[Serializable]
	public class RoleApp: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("PID")]
		public string  PID { get; set; }
		[DisplayName("RoleID")]
		public string  RoleID { get; set; }
		[DisplayName("AppID")]
		public string  AppID { get; set; }
		[DisplayName("Title")]
		public string  Title { get; set; }
		[DisplayName("Params")]
		public string  Params { get; set; }
		[DisplayName("Sort")]
		public int?  Sort { get; set; }
		[DisplayName("Ico")]
		public string  Ico { get; set; }
		[DisplayName("Type")]
		public int?  Type { get; set; }

	}
}
