using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Jssjw.Case.Entity;
using Jssjw.Case.IDAL;

namespace Jssjw.Case.DALMySql
{
	public class PropertyDAL: BaseDALMySql<Property> , IPropertyDAL
	{
		public static PropertyDAL Instance
		{
			get
			{
				return new PropertyDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public PropertyDAL()
		: base("Property", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}