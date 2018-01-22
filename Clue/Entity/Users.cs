using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace App.Clue.Entity
{
	[Serializable]
	public class Users: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("DeptID")]
		public string  DeptID { get; set; }
		[DisplayName("RealName")]
		public string  RealName { get; set; }
		[DisplayName("UserName")]
		public string  UserName { get; set; }
		[DisplayName("Password")]
		public string  Password { get; set; }
		[DisplayName("CreatTime")]
		public DateTime  CreatTime { get; set; }
		[DisplayName("RoleId")]
		public int?  RoleId { get; set; }
		[DisplayName("IsSys")]
		public string  IsSys { get; set; }
		[DisplayName("LoginNum")]
		public int?  LoginNum { get; set; }
		[DisplayName("LoginTime")]
		public DateTime?  LoginTime { get; set; }
		[DisplayName("RoleKey")]
		public string  RoleKey { get; set; }
		[DisplayName("IP")]
		public string  IP { get; set; }
		[DisplayName("Region")]
		public string  Region { get; set; }
		[DisplayName("TimeStamp")]
		public DateTime?  TimeStamp { get; set; }
		[DisplayName("IsDeleted")]
		public bool?  IsDeleted { get; set; }

	}
}
