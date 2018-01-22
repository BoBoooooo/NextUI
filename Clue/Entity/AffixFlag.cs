using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace App.Clue.Entity
{
	[Serializable]
	public class AffixFlag: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("IsCreated")]
		public bool  IsCreated { get; set; }
		[DisplayName("IsSync")]
		public bool  IsSync { get; set; }
		[DisplayName("IsBackuped")]
		public bool  IsBackuped { get; set; }

	}
}
