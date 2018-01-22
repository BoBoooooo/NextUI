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
	public class DictDAL: BaseDALMySql<Dict> , IDictDAL
	{
		public static DictDAL Instance
		{
			get
			{
				return new DictDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public DictDAL()
		: base("Dict", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
        public List<Dict> GetChildsByID(string id)
        {
            string sql = "SELECT * FROM Dict WHERE ParentID='" + id + "' ORDER BY Sort";
            var result = GetList(sql);
            return result;
        }
        /// <summary>
        /// 查询下级记录
        /// </summary>
        public List<Dict> GetChildsByCode(string code)
        {
            string sql = string.Format("SELECT * FROM Dict WHERE PID=(SELECT ID FROM Dict WHERE Code='{0}') ORDER BY Sort", code);
            var result = GetList(sql);
            return result;
        }
        public Dict GetRoot()
        {
            string sql = "SELECT * FROM Dict WHERE ParentID='-1'";
            var result = GetList(sql);
            return result.FirstOrDefault();
        }

        /// <summary>
        /// 是否包含下级记录
        /// </summary>
        public bool HasChilds(string id)
        {
            string sql = string.Format("SELECT * FROM Dict WHERE ParentID='{0}'", id);
            var result = GetList(sql);
            bool has = result.Count > 0;
            return has;
        }

        public Dict GetByCode(string code)
        {
            string sql = string.Format("SELECT * FROM Dict WHERE Code='{0}'", code);
            var result = GetList(sql);
            return result.FirstOrDefault();
        }

        /// <summary>
        /// 查询上级记录
        /// </summary>
        public Dict GetParent(string id)
        {
            string sql = string.Format("SELECT * FROM Dict WHERE ID=(SELECT ParentID FROM Dict WHERE ID='{0}')", id);
            var result = GetList(sql);
            return result.FirstOrDefault();
        }


        /// <summary>
        /// 得到最大排序
        /// </summary>
        public int GetMaxSort(string id)
        {
            string sql = string.Format("SELECT MAX(Sort)+1 FROM Dict WHERE ParentID='{0}'", id);
            DataTable max = SqlTable(sql);
            return max.Rows[0][0].ToString().IsInt() ? max.Rows[0][0].ToString().ToInt() : 1;
        }

        /// <summary>
        /// 更新排序
        /// </summary>
        public int UpdateSort(string id, int sort)
        {
            string sql = string.Format("UPDATE Dict SET Sort={0} WHERE ID='{1}'", sort, id);
            return SqlExecute(sql);
        }

    }
}
