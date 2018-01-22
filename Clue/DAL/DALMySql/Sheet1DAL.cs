using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using App.Clue.Entity;
using App.Clue.IDAL;

namespace App.Clue.DALMySql
{
	public class Sheet1DAL: BaseDALMySql<Sheet1> , ISheet1DAL
	{
		public static Sheet1DAL Instance
		{
			get
			{
				return new Sheet1DAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public Sheet1DAL()
		: base("Sheet1", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}