using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.IDAL;
using Next.WorkFlow.Utility;
namespace Next.WorkFlow.DALMySql
{
	public class WorkFlowButtonsDAL: BaseDALMySql<WorkFlowButtons> , IWorkFlowButtonsDAL
	{
		public static WorkFlowButtonsDAL Instance
		{
			get
			{
				return new WorkFlowButtonsDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public WorkFlowButtonsDAL()
		: base("WorkFlowButtons", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}

        /// <summary>
        /// 查询最大排序
        /// </summary>
        public int GetMaxSort()
        {
            string sql = "SELECT IfNULL(MAX(Sort),0)+1 FROM WorkFlowButtons";
            DataTable max = SqlTable(sql);

            return max.Rows[0][0].ToString().IsInt() ? max.Rows[0][0].ToString().ToInt() : 1;
        }
	}
}