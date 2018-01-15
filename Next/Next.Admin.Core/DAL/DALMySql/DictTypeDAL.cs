using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Next.Admin.Entity;
using Next.Admin.IDAL;
using System.Linq;
using Next.WorkFlow.Utility;
namespace Next.Admin.DALMySql
{
    public class DictTypeDAL : BaseDALMySql<DictType>, IDictTypeDAL
    {
        public static DictTypeDAL Instance
        {
            get
            {
                return new DictTypeDAL();
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public DictTypeDAL()
            : base("DictType", "ID")
        {
            this.sortField = "ID";
            this.IsDescending = false;
        }
        /*
        public List<DictType> GetChildsByID(string id)
        {
            string sql = "SELECT * FROM DictType WHERE PID='" + id + "' ORDER BY Seq";
            var result = GetList(sql);
            return result;
        }
        /// <summary>
        /// 查询下级记录
        /// </summary>
        public List<DictType> GetChildsByCode(string code)
        {
            string sql = string.Format("SELECT * FROM DictType WHERE PID=(SELECT ID FROM DictType WHERE Code='{0}') ORDER BY Seq",code);
            var result = GetList(sql);
            return result;
        }
        public DictType GetRoot()
        {
            string sql = "SELECT * FROM DictType WHERE PID='-1'";
            var result = GetList(sql);
            return result.FirstOrDefault();
        }

        /// <summary>
        /// 是否包含下级记录
        /// </summary>
        public bool HasChilds(string id)
        {
            string sql = string.Format("SELECT * FROM DictType WHERE PID='{0}'", id);
            var result = GetList(sql);
            bool has = result.Count > 0;
            return has;
        }

        public DictType GetByCode(string code)
        {
            string sql = string.Format("SELECT * FROM DictType WHERE Code='{0}'", code);
            var result = GetList(sql);
            return result.FirstOrDefault();
        }

        /// <summary>
        /// 查询上级记录
        /// </summary>
        public DictType GetParent(string id)
        {
            string sql = string.Format("SELECT * FROM DictType WHERE ID=(SELECT PID FROM DictType WHERE ID='{0}')", id);
            var result = GetList(sql);
            return result.FirstOrDefault();
        }


        /// <summary>
        /// 得到最大排序
        /// </summary>
        public int GetMaxSort(string id)
        {
            string sql = string.Format("SELECT MAX(Sort)+1 FROM DictType WHERE PID='{0}'", id);
            DataTable max = SqlTable(sql);
            return max.Rows[0][0].ToString().IsInt() ? max.Rows[0][0].ToString().ToInt() : 1;
        }

        /// <summary>
        /// 更新排序
        /// </summary>
        public int UpdateSort(string id, int sort)
        {
            string sql = string.Format("UPDATE Dictionary SET Seq={0} WHERE ID='{1}'", sort, id);
            return SqlExecute(sql);
        }
        */
    }
}
