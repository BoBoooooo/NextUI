using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace App.Clue.Entity
{
	[Serializable]
	public class Menu: BaseEntity
	{
		[DisplayName("menuid")]
		public int  menuid { get; set; }
		[DisplayName("menuname")]
		public string  menuname { get; set; }
		[DisplayName("icon")]
		public string  icon { get; set; }
		[DisplayName("parentId")]
		public string  parentId { get; set; }
		[DisplayName("url")]
		public string  url { get; set; }
		[DisplayName("remark")]
		public string  remark { get; set; }
		[DisplayName("orderBy")]
		public int?  orderBy { get; set; }
		[DisplayName("isUse")]
		public string  isUse { get; set; }
		[DisplayName("TimeStamp")]
		public DateTime?  TimeStamp { get; set; }

	}
}
