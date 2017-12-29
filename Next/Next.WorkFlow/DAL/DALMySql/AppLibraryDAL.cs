using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.IDAL;
using System.Linq;


namespace Next.WorkFlow.DALMySql
{
	public class AppLibraryDAL: BaseDALMySql<AppLibrary> , IAppLibraryDAL
	{
		public static AppLibraryDAL Instance
		{
			get
			{
				return new AppLibraryDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public AppLibraryDAL()
		: base("AppLibrary", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}

        /// <summary>
        /// 根据代码查询一条记录
        /// </summary>
        public AppLibrary GetByCode(string code)
        {
            string sql = "SELECT * FROM AppLibrary WHERE Code='" + code+"'";

            return GetList(sql).FirstOrDefault();
        }
	}
}