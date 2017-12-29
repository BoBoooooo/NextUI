using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Jssjw.Case.Entity;
using Jssjw.Case.IDAL;

namespace Jssjw.Case.DALMySql
{
	public class CaseIndexDAL: BaseDALMySql<CaseIndex> , ICaseIndexDAL
	{
		public static CaseIndexDAL Instance
		{
			get
			{
				return new CaseIndexDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public CaseIndexDAL()
		: base("CaseIndex", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}