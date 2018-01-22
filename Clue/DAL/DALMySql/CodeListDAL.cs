using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using App.Clue.Entity;
using App.Clue.IDAL;

namespace App.Clue.DALMySql
{
	public class CodeListDAL: BaseDALMySql<CodeList> , ICodeListDAL
	{
		public static CodeListDAL Instance
		{
			get
			{
				return new CodeListDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public CodeListDAL()
		: base("CodeList", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}