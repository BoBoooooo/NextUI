using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Next.Admin.Entity;
using Next.Admin.IDAL;

namespace Next.Admin.DALMySql
{
	public class LoginLogDAL: BaseDALMySql<LoginLog> , ILoginLogDAL
	{
		public static LoginLogDAL Instance
		{
			get
			{
				return new LoginLogDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public LoginLogDAL()
		: base("LoginLog", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
        /// <summary>
        /// 获取上一次（非刚刚登录）的登录日志
        /// </summary>
        /// <param name="userId">登录用户ID</param>
        /// <returns></returns>
        public LoginLog GetLastLoginInfo(string userId)
        {
            string sql = string.Format("Select Top 2 * from {0} where UserID='{1}' order by LastUpdated desc", tableName, userId);
            List<LoginLog> list = GetList(sql, null);
            if (list.Count == 2)
            {
                return list[1];
            }
            else
            {
                return null;
            }
        }
	}
}