using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.IDAL;

namespace Next.WorkFlow.DALMySql
{
	public class WorkFlowFormDAL: BaseDALMySql<WorkFlowForm> , IWorkFlowFormDAL
	{
		public static WorkFlowFormDAL Instance
		{
			get
			{
				return new WorkFlowFormDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public WorkFlowFormDAL()
		: base("WorkFlowForm", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}

        /// <summary>
        /// 查询一个分类所有记录
        /// </summary>
        public List<WorkFlowForm> GetAllByType(string types)
        {
            string sql = "SELECT * FROM WorkFlowForm where Type IN(" + Utility.Tools.GetSqlInString(types) + ")";
            var resultList = GetList(sql);
            return resultList;
        }
	}
}