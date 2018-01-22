using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using App.Clue.Entity;
using App.Clue.IDAL;

namespace App.Clue.DALMySql
{
	public class MenuDAL: BaseDALMySql<Menu> , IMenuDAL
	{
		public static MenuDAL Instance
		{
			get
			{
				return new MenuDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public MenuDAL()
		: base("Menu", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}