using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace Jssjw.Case.Entity
{
	[Serializable]
	public class CaseIndex: BaseEntity
	{
		[DisplayName("ID")]
        public int ID { get; set; }
		[DisplayName("DeptID")]
		public string  DeptID { get; set; }
		[DisplayName("UserID")]
		public string  UserID { get; set; }
		[DisplayName("AttachmentID")]
		public string  AttachmentID { get; set; }
		[DisplayName("Dept")]
		public string  Dept { get; set; }
		[DisplayName("CaseName")]
		public string  CaseName { get; set; }
		[DisplayName("CaseCode")]
		public string  CaseCode { get; set; }
		[DisplayName("CaseTarget")]
		public string  CaseTarget { get; set; }
		[DisplayName("CaseCharge")]
		public string  CaseCharge { get; set; }
		[DisplayName("CaseAgent")]
		public string  CaseAgent { get; set; }
		[DisplayName("CaseDuration")]
		public string  CaseDuration { get; set; }
		[DisplayName("CaseContentCount")]
		public string  CaseContentCount { get; set; }
		[DisplayName("IsDeleted")]
		public bool  IsDeleted { get; set; }

	}
}
