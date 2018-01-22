using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace App.Clue.Entity
{
	[Serializable]
	public class Fast: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("ClueLinkID")]
		public string  ClueLinkID { get; set; }
		[DisplayName("TailLinkID")]
		public string  TailLinkID { get; set; }
		[DisplayName("VisitID")]
		public string  VisitID { get; set; }

	}
}
