using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using App.Clue.Entity;
using App.Clue.IDAL;

namespace App.Clue.DALMySql
{
	public class HandleRegDAL: BaseDALMySql<HandleReg> , IHandleRegDAL
	{
		public static HandleRegDAL Instance
		{
			get
			{
				return new HandleRegDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public HandleRegDAL()
		: base("HandleReg", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}