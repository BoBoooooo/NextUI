using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using App.Clue.Entity;
using App.Clue.IDAL;

namespace App.Clue.DALMySql
{
	public class LogDAL: BaseDALMySql<Log> , ILogDAL
	{
		public static LogDAL Instance
		{
			get
			{
				return new LogDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public LogDAL()
		: base("Log", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}