using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Next.Admin.Entity;
using Next.Admin.IDAL;

namespace Next.Admin.DALMySql
{
	public class DictTypeDAL: BaseDALMySql<DictType> , IDictTypeDAL
	{
		public static DictTypeDAL Instance
		{
			get
			{
				return new DictTypeDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public DictTypeDAL()
		: base("DictType", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}