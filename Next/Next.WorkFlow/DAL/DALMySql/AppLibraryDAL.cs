using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.IDAL;
using System.Linq;
using Next.WorkFlow.Utility;
using MySql.Data.MySqlClient;

namespace Next.WorkFlow.DALMySql
{
	public class AppLibraryDAL: BaseDALMySql<AppLibrary> , IAppLibraryDAL
	{
        private DBHelper dbHelper = new DBHelper();
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

        /// <summary>
        /// 查询一个类别下所有记录
        /// </summary>
        public List<AppLibrary> GetAllByType(string types)
        {
            string sql = "SELECT * FROM AppLibrary WHERE Type IN(" + Next.WorkFlow.Utility.Tools.GetSqlInString(types) + ")";
            return GetList(sql);
        }
        /// <summary>
        /// 得到一页数据
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="query"></param>
        /// <param name="order"></param>
        /// <param name="size"></param>
        /// <param name="numbe"></param>
        /// <param name="title"></param>
        /// <param name="type"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public List<AppLibrary> GetPagerData(out string pager, string query = "", string order = "Type,Title", int size = 15, int number = 1, string title = "", string type = "", string address = "")
        {
            StringBuilder WHERE = new StringBuilder();
            List<MySqlParameter> parList = new List<MySqlParameter>();
            if (!title.IsNullOrEmpty())
            {
                WHERE.AppendFormat("AND CHARINDEX({0},Title)>0 ", title);
                //parList.Add(new MySqlParameter("@Title", SqlDbType.NVarChar) { Value = title });
            }
            if (!type.IsNullOrEmpty())
            {
                WHERE.AppendFormat("AND Type IN({0}) ", Next.WorkFlow.Utility.Tools.GetSqlInString(type));
            }
            if (!address.IsNullOrEmpty())
            {
                WHERE.AppendFormat("AND CHARINDEX({0},Address)>0 ", address);
                //parList.Add(new MySqlParameter("@Address", SqlDbType.VarChar) { Value = address });
            }
            long count;
            string sql = dbHelper.GetPaerSql("AppLibrary", "*", WHERE.ToString(), order, size, number, out count, parList.ToArray());

            pager = Next.WorkFlow.Utility.Tools.GetPagerHtml(count, size, number, query);

            return GetList(sql);
            /*MySqlDataReader dataReader = dbHelper.GetDataReader(sql, parList.ToArray());
            List<AppLibrary> List = DataReaderToList(dataReader);
            dataReader.Close();
            return List;*/
        }
	}
}