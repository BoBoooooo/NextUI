using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace App.Clue.Entity
{
	[Serializable]
	public class RoleToMenu: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("roleId")]
		public string  roleId { get; set; }
		[DisplayName("menuId")]
		public int?  menuId { get; set; }
		[DisplayName("TimeStamp")]
		public DateTime?  TimeStamp { get; set; }

	}
}
