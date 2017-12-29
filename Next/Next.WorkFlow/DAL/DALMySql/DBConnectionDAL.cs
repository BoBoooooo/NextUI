using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.IDAL;

namespace Next.WorkFlow.DALMySql
{
	public class DBConnectionDAL: BaseDALMySql<DBConnection> , IDBConnectionDAL
	{
		public static DBConnectionDAL Instance
		{
			get
			{
				return new DBConnectionDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public DBConnectionDAL()
		: base("DBConnection", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}