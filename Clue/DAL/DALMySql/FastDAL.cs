using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using App.Clue.Entity;
using App.Clue.IDAL;

namespace App.Clue.DALMySql
{
	public class FastDAL: BaseDALMySql<Fast> , IFastDAL
	{
		public static FastDAL Instance
		{
			get
			{
				return new FastDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public FastDAL()
		: base("Fast", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}