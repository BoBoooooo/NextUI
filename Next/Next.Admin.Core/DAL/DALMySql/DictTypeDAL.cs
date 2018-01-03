using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Next.Admin.Entity;
using Next.Admin.IDAL;
using System.Linq;

namespace Next.Admin.DALMySql
{
	public class DictTypeDAL: BaseDALMySql<DictType> , IDictTypeDAL
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

        public List<DictType> GetChilds(string id)
        {
            string sql = "SELECT * FROM DictType WHERE PID='"+id+"'";
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
            string sql = string.Format("SELECT * FROM DictType WHERE PID='{0}'",id);
            var result = GetList(sql);
            bool has = result.Count>0;
            return has;
        }


	}
}