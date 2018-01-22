using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace App.Clue.Entity
{
	[Serializable]
	public class Archive: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("ClueID")]
		public string  ClueID { get; set; }
		[DisplayName("Status")]
		public string  Status { get; set; }

	}
}
