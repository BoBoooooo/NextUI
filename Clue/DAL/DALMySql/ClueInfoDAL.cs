using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using App.Clue.Entity;
using App.Clue.IDAL;

namespace App.Clue.DALMySql
{
	public class ClueInfoDAL: BaseDALMySql<ClueInfo> , IClueInfoDAL
	{
		public static ClueInfoDAL Instance
		{
			get
			{
				return new ClueInfoDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public ClueInfoDAL()
		: base("ClueInfo", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}