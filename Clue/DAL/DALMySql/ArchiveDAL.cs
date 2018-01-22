using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using App.Clue.Entity;
using App.Clue.IDAL;

namespace App.Clue.DALMySql
{
	public class ArchiveDAL: BaseDALMySql<Archive> , IArchiveDAL
	{
		public static ArchiveDAL Instance
		{
			get
			{
				return new ArchiveDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public ArchiveDAL()
		: base("Archive", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}