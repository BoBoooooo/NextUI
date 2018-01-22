using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace App.Clue.Entity
{
	[Serializable]
	public class CodeList: BaseEntity
	{
		[DisplayName("ClueCode5")]
		public string  ClueCode5 { get; set; }
		[DisplayName("ClueCode6")]
		public string  ClueCode6 { get; set; }
		[DisplayName("ClueCode7")]
		public string  ClueCode7 { get; set; }
		[DisplayName("ClueCode8")]
		public string  ClueCode8 { get; set; }

	}
}
