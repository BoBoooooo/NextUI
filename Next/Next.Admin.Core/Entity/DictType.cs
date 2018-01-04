using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace Next.Admin.Entity
{
	[Serializable]
	public class DictType: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("PID")]
		public string  PID { get; set; }
		[DisplayName("Name")]
		public string  Name { get; set; }
        [DisplayName("Value")]
        public string Value { get; set; }
        [DisplayName("Code")]
        public string Code { get; set; }
        [DisplayName("Other")]
        public string Other { get; set; }
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
