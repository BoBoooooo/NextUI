using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace App.Clue.Entity
{
	[Serializable]
	public class Approval: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("ClueID")]
		public string  ClueID { get; set; }
		[DisplayName("fillInDate")]
		public DateTime?  fillInDate { get; set; }
		[DisplayName("clueMode")]
		public string  clueMode { get; set; }
		[DisplayName("clueSource")]
		public string  clueSource { get; set; }
		[DisplayName("letterNum")]
		public string  letterNum { get; set; }
		[DisplayName("objName")]
		public string  objName { get; set; }
		[DisplayName("objAddress")]
		public string  objAddress { get; set; }
		[DisplayName("objAdminLvl")]
		public string  objAdminLvl { get; set; }
		[DisplayName("objPost")]
		public string  objPost { get; set; }
		[DisplayName("reflectName")]
		public string  reflectName { get; set; }
		[DisplayName("reflectAddress")]
		public string  reflectAddress { get; set; }
		[DisplayName("reflectPost")]
		public string  reflectPost { get; set; }
		[DisplayName("reflectLinkPhone")]
		public string  reflectLinkPhone { get; set; }
		[DisplayName("des")]
		public string  des { get; set; }
		[DisplayName("clueCode")]
		public string  clueCode { get; set; }
		[DisplayName("deviseMethod")]
		public string  deviseMethod { get; set; }
		[DisplayName("undertaker")]
		public string  undertaker { get; set; }
		[DisplayName("deptOpinion")]
		public string  deptOpinion { get; set; }
		[DisplayName("deviseOpinion")]
		public string  deviseOpinion { get; set; }
		[DisplayName("leaderInstruction")]
		public string  leaderInstruction { get; set; }
		[DisplayName("TimeStamp")]
		public DateTime?  TimeStamp { get; set; }
		[DisplayName("IsDeleted")]
		public bool  IsDeleted { get; set; }

	}
}
