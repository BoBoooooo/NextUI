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
	public class WorkFlowCommentDAL: BaseDALMySql<WorkFlowComment> , IWorkFlowCommentDAL
	{
		public static WorkFlowCommentDAL Instance
		{
			get
			{
				return new WorkFlowCommentDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public WorkFlowCommentDAL()
		: base("WorkFlowComment", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}

        /// <summary>
        /// 查询管理员的所有记录
        /// </summary>
        public List<WorkFlowComment> GetManagerAll()
        {
            string sql = "SELECT * FROM WorkFlowComment WHERE Type=0";
            return GetList(sql);
        }
        /// <summary>
        /// 得到管理员类别的最大排序值
        /// </summary>
        /// <returns></returns>
        public int GetManagerMaxSort()
        {
            string sql = "SELECT ISNULL(MAX(Sort)+1,1) FROM WorkFlowComment WHERE Type=0";
            return SqlTable(sql).Rows[0][0].ToString().ToInt();
        }

        /// <summary>
        /// 得到一个人员的最大排序值
        /// </summary>
        /// <returns></returns>
        public int GetUserMaxSort(string userID)
        {
            string sql = string.Format("SELECT ISNULL(MAX(Sort)+1,1) FROM WorkFlowComment WHERE MemberID='{0}",userID);
            return SqlTable(sql).Rows[0][0].ToString().ToInt();
        }
    }
}