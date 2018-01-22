using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using App.Clue.Entity;
using App.Clue.IDAL;

namespace App.Clue.DALMySql
{
	public class ClueLinkDAL: BaseDALMySql<ClueLink> , IClueLinkDAL
	{
		public static ClueLinkDAL Instance
		{
			get
			{
				return new ClueLinkDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public ClueLinkDAL()
		: base("ClueLink", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}