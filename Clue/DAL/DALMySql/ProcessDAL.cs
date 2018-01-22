using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using App.Clue.Entity;
using App.Clue.IDAL;

namespace App.Clue.DALMySql
{
	public class ProcessDAL: BaseDALMySql<Process> , IProcessDAL
	{
		public static ProcessDAL Instance
		{
			get
			{
				return new ProcessDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public ProcessDAL()
		: base("Process", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}