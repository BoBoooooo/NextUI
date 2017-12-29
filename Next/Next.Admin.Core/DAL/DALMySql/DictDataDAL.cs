using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Next.Admin.Entity;
using Next.Admin.IDAL;

namespace Next.Admin.DALMySql
{
	public class DictDataDAL: BaseDALMySql<DictData> , IDictDataDAL
	{
		public static DictDataDAL Instance
		{
			get
			{
				return new DictDataDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public DictDataDAL()
		: base("DictData", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}


	}
}