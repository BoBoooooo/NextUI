using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using App.Clue.Entity;
using App.Clue.IDAL;

namespace App.Clue.DALMySql
{
	public class HandleDAL: BaseDALMySql<Handle> , IHandleDAL
	{
		public static HandleDAL Instance
		{
			get
			{
				return new HandleDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public HandleDAL()
		: base("Handle", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}