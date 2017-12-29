using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.IDAL;

namespace Next.WorkFlow.DALMySql
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