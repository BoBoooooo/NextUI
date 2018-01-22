using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using App.Clue.Entity;
using App.Clue.IDAL;

namespace App.Clue.DALMySql
{
	public class ApprovalDAL: BaseDALMySql<Approval> , IApprovalDAL
	{
		public static ApprovalDAL Instance
		{
			get
			{
				return new ApprovalDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public ApprovalDAL()
		: base("Approval", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}