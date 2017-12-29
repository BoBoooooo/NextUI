using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Next.Admin.Entity;
using Next.Admin.IDAL;

namespace Next.Admin.DALMySql
{
	public class OperationLogDAL: BaseDALMySql<OperationLog> , IOperationLogDAL
	{
		public static OperationLogDAL Instance
		{
			get
			{
				return new OperationLogDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public OperationLogDAL()
            : base("OperationLog", "ID")
		{
            this.sortField = "CreateTime";
			this.IsDescending = false;
		}
	}
}