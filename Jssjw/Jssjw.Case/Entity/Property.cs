using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace Jssjw.Case.Entity
{
	[Serializable]
	public class Property: BaseEntity
	{
		[DisplayName("ID")]
        public int ID { get; set; }

		[DisplayName("UserID")]
		public string  UserID { get; set; }
		[DisplayName("DeptID")]
		public string  DeptID { get; set; }
		[DisplayName("AttachmentID")]
		public string  AttachmentID { get; set; }
		[DisplayName("Dept")]
		public string  Dept { get; set; }
		[DisplayName("CaseName")]
		public string  CaseName { get; set; }
		[DisplayName("CaseCode")]
		public string  CaseCode { get; set; }
		[DisplayName("GoodsNumber")]
		public string  GoodsNumber { get; set; }
		[DisplayName("GoodsName")]
		public string  GoodsName { get; set; }
		[DisplayName("GoodsCount")]
		public string  GoodsCount { get; set; }
		[DisplayName("GoodsPrice")]
		public string  GoodsPrice { get; set; }
		[DisplayName("GoodsHolder")]
		public string  GoodsHolder { get; set; }
		[DisplayName("DetainDate")]
		public string  DetainDate { get; set; }
		[DisplayName("DetainPerson")]
		public string  DetainPerson { get; set; }
		[DisplayName("ReturnTime")]
		public string  ReturnTime { get; set; }
		[DisplayName("Keeper")]
		public string  Keeper { get; set; }
		[DisplayName("KeepSpace")]
		public string  KeepSpace { get; set; }
		[DisplayName("Process")]
		public string  Process { get; set; }
		[DisplayName("ProcessTime")]
		public string  ProcessTime { get; set; }
		[DisplayName("GoodsDetail")]
		public string  GoodsDetail { get; set; }
		[DisplayName("ImagePath")]
		public string  ImagePath { get; set; }
		[DisplayName("IsDeleted")]
		public bool  IsDeleted { get; set; }

	}
}
