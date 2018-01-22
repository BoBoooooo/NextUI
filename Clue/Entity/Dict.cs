using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace App.Clue.Entity
{
	[Serializable]
	public class Dict: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("Name")]
		public string  Name { get; set; }
		[DisplayName("Unit")]
		public string  Unit { get; set; }
		[DisplayName("Duty")]
		public string  Duty { get; set; }
		[DisplayName("Region")]
		public string  Region { get; set; }

	}
}
