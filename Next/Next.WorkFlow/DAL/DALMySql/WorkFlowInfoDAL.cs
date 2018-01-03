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
        /// 查询所有ID和名称
        /// </summary>
        public Dictionary<string, string> GetAllIDAndName()
        {
            string sql = "SELECT * FROM WorkFlowInfo WHERE Status<4 ORDER BY Name";
            Dictionary<string, string> dict = new Dictionary<string, string>();
            var resultList = GetList(sql);
            foreach (var item in resultList)
            {
                dict.Add(item.ID, item.Name);
            }
            return dict;
        }
	}
}