using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Jssjw.Organization.Entity;
using Jssjw.Organization.IDAL;

namespace Jssjw.Organization.DALMySql
{
	public class LoanDAL: BaseDALMySql<Loan> , ILoanDAL
	{
		public static LoanDAL Instance
		{
			get
			{
				return new LoanDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public LoanDAL()
		: base("Loan", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}