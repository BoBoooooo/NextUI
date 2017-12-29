using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace Next.Admin.Entity
{
	[Serializable]
	public class DictData: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("DictTypeID")]
		public string  DictTypeID { get; set; }
		[DisplayName("Name")]
		public string  Name { get; set; }
		[DisplayName("Value")]
		public string  Value { get; set; }
		[DisplayName("Remark")]
		public string  Remark { get; set; }
		[DisplayName("Seq")]
		public string  Seq { get; set; }
		[DisplayName("Editor")]
		public string  Editor { get; set; }
		[DisplayName("LastUpdated")]
		public DateTime?  LastUpdated { get; set; }

	}
}
