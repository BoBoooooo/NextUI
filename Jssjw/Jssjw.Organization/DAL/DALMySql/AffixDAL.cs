using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Jssjw.Organization.Entity;
using Jssjw.Organization.IDAL;

namespace Jssjw.Organization.DALMySql
{
	public class AffixDAL: BaseDALMySql<Affix> , IAffixDAL
	{
		public static AffixDAL Instance
		{
			get
			{
				return new AffixDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public AffixDAL()
		: base("Affix", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}