using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Jssjw.Office.Entity;
using Jssjw.Office.IDAL;

namespace Jssjw.Office.DALMySql
{
	public class JournalDAL: BaseDALMySql<Journal> , IJournalDAL
	{
		public static JournalDAL Instance
		{
			get
			{
				return new JournalDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public JournalDAL()
		: base("Journal", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}