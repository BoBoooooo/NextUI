using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.IDAL;

namespace Next.WorkFlow.DALMySql
{
	public class WorkFlowInfoDAL: BaseDALMySql<WorkFlowInfo> , IWorkFlowInfoDAL
	{
		public static WorkFlowInfoDAL Instance
		{
			get
			{
				return new WorkFlowInfoDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public WorkFlowInfoDAL()
		: base("WorkFlowInfo", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}

        /// <summary>
        /// 查询所有类型
        /// </summary>
        public List<string> GetAllTypes()
        {
            string sql = "SELECT Type FROM WorkFlowInfo GROUP BY Type";
            List<string> list = new List<string>();
            var dt = SqlTable(sql);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                list.Add(dt.Rows[i][0].ToString());
            }     
            return list;
        }

        /// <summary>
        /// 查询所有ID和名称
        /// </summary>
        public Dictionary<string, string> GetAllIDAndName()
        {
            string sql = "SELECT ID,Name FROM WorkFlowInfo WHERE Status<4 ORDER BY Name";
            Dictionary<string, string> dict = new Dictionary<string, string>();
            var dt = SqlTable(sql);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dict.Add(dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString());
            }     
            return dict;
        }

        /// <summary>
        /// 查询所有记录
        /// </summary>
        public List<WorkFlowInfo> GetByTypes(string typeString)
        {
            string sql = "SELECT * FROM WorkFlowInfo where Type IN(" + Next.WorkFlow.Utility.Tools.GetSqlInString(typeString) + ")";
            return GetList(sql);
        }
    }
}