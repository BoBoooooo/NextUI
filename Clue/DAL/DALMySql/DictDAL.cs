using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using App.Clue.Entity;
using App.Clue.IDAL;

namespace App.Clue.DALMySql
{
	public class DictDAL: BaseDALMySql<Dict> , IDictDAL
	{
		public static DictDAL Instance
		{
			get
			{
				return new DictDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public DictDAL()
		: base("Dict", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}