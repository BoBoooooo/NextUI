using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace App.Clue.Entity
{
	[Serializable]
	public class Letter: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("UserID")]
		public string  UserID { get; set; }
		[DisplayName("DeptID")]
		public string  DeptID { get; set; }
		[DisplayName("RealName")]
		public string  RealName { get; set; }
		[DisplayName("Region")]
		public string  Region { get; set; }
		[DisplayName("LetterCode")]
		public string  LetterCode { get; set; }
		[DisplayName("LetterName")]
		public string  LetterName { get; set; }
		[DisplayName("LetterRegion")]
		public string  LetterRegion { get; set; }

	}
}
