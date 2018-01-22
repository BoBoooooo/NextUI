using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using App.Clue.Entity;
using App.Clue.IDAL;

namespace App.Clue.DALMySql
{
	public class AffixFlagDAL: BaseDALMySql<AffixFlag> , IAffixFlagDAL
	{
		public static AffixFlagDAL Instance
		{
			get
			{
				return new AffixFlagDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public AffixFlagDAL()
		: base("AffixFlag", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}