﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using App.Clue.Entity;
using App.Clue.IDAL;

namespace App.Clue.DALMySql
{
	public class UsersDAL: BaseDALMySql<Users> , IUsersDAL
	{
		public static UsersDAL Instance
		{
			get
			{
				return new UsersDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public UsersDAL()
		: base("Users", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}