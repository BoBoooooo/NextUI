using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using App.Clue.Entity;
using App.Clue.IDAL;

namespace App.Clue.DALMySql
{
	public class LetterDAL: BaseDALMySql<Letter> , ILetterDAL
	{
		public static LetterDAL Instance
		{
			get
			{
				return new LetterDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public LetterDAL()
		: base("Letter", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}