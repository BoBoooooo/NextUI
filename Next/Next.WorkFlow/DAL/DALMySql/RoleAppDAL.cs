using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.IDAL;
using Next.WorkFlow.Utility;
using System.Linq;

namespace Next.WorkFlow.DALMySql
{
	public class RoleAppDAL: BaseDALMySql<RoleApp> , IRoleAppDAL
	{
		public static RoleAppDAL Instance
		{
			get
			{
				return new RoleAppDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public RoleAppDAL()
		: base("RoleApp", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
        /// <summary>
        /// 查询一个角色所有记录
        /// </summary>
        public List<RoleApp> GetAllByRoleID(string roleID)
        {
            string sql = string.Format("SELECT * FROM RoleApp WHERE RoleID='{0}'",roleID);
            var resultList = GetList(sql);
            return resultList;
        }

        /// <summary>
        /// 查询一个角色所有记录
        /// </summary>
        public System.Data.DataTable GetAllDataTableByRoleID(string roleID)
        {
            string sql = string.Format("SELECT a.*,b.Address,b.OpenMode,b.Width,b.Height,b.Params AS Param1,b.Manager,b.UseMember FROM RoleApp a LEFT JOIN AppLibrary b ON a.AppID=b.ID WHERE a.RoleID=@RoleID ORDER BY a.Sort");
            DataTable result = SqlTable(sql);
            return result;
        }

        /// <summary>
        /// 查询所有记录
        /// </summary>
        public System.Data.DataTable GetAllDataTable()
        {
            string sql = string.Format("SELECT a.*,b.Address,b.OpenMode,b.Width,b.Height,b.Params AS Params1,b.Manager,b.UseMember FROM RoleApp a LEFT JOIN AppLibrary b ON a.AppID=b.ID ORDER BY a.Sort");
            DataTable result = SqlTable(sql);
            return result;
        }


        /// <summary>
        /// 查询所有下级记录
        /// </summary>
        public System.Data.DataTable GetChildsDataTable(string id)
        {
            string sql = string.Format("SELECT a.*,b.Address,b.OpenMode,b.Width,b.Height,b.Params AS Params1,b.Manager,b.UseMember FROM RoleApp a LEFT JOIN AppLibrary b ON a.AppID=b.ID WHERE a.PID=@PID ORDER BY a.Sort");
            DataTable result = SqlTable(sql);
            return result;
        }

        /// <summary>
        /// 查询下级记录
        /// </summary>
        public List<RoleApp> GetChild(string id)
        {
            string sql = string.Format("SELECT * FROM RoleApp WHERE PID='{0}' ORDER BY Sort",id);
            var result = GetList(sql);
            return result;
        }

        /// <summary>
        /// 是否有下级记录
        /// </summary>
        public bool HasChild(string id)
        {
            string sql = string.Format("SELECT TOP 1 ID FROM RoleApp WHERE PID=='{0}'", id);
            var result = GetList(sql);
            return result.Count>0;
        }

        /// <summary>
        /// 更新排序
        /// </summary>
        public int UpdateSort(string id, int sort)
        {
            string sql = string.Format("UPDATE RoleApp SET Sort={0} WHERE ID='{1}'",sort,id);
            return SqlExecute(sql);
        }

        /// <summary>
        /// 删除一个角色记录
        /// </summary>
        public int DeleteByRoleID(string roleID)
        {
            string sql = string.Format("DELETE FROM RoleApp WHERE RoleID='{0}'", roleID);
            return SqlExecute(sql); ;
        }
        /// <summary>
        /// 删除一个应用记录
        /// </summary>
        public int DeleteByAppID(string appID)
        {
            string sql = string.Format("DELETE FROM RoleApp WHERE AppID='{0}'", appID);
            return SqlExecute(sql);
        }
        /// <summary>
        /// 得到最大排序值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetMaxSort(string id)
        {
            string sql = string.Format("SELECT MAX(Sort)+1 FROM RoleApp WHERE PID='{0}'", id);
            DataTable max = SqlTable(sql);
            return max.Rows[0][0].ToString().IsInt() ? max.Rows[0][0].ToString().ToInt() : 1;
        }
    }
}