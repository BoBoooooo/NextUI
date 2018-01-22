using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace App.Clue.Entity
{
	[Serializable]
	public class Sheet1: BaseEntity
	{
		[DisplayName("code")]
		public string  code { get; set; }

	}
}
