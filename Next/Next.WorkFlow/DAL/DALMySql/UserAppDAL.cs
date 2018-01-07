using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.IDAL;

namespace Next.WorkFlow.DALMySql
{
	public class UserAppDAL: BaseDALMySql<UserApp> , IUserAppDAL
	{
		public static UserAppDAL Instance
		{
			get
			{
				return new UserAppDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public UserAppDAL()
		: base("UserApp", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}