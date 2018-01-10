using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.IDAL;
using MySql.Data.MySqlClient;
using Next.WorkFlow.Utility;

namespace Next.WorkFlow.DALMySql
{
	public class WorkFlowDelegationDAL: BaseDALMySql<WorkFlowDelegation> , IWorkFlowDelegationDAL
	{
		public static WorkFlowDelegationDAL Instance
		{
			get
			{
				return new WorkFlowDelegationDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public WorkFlowDelegationDAL()
		: base("WorkFlowDelegation", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
        private DBHelper dbHelper = new DBHelper();
        /// <summary>
        /// 查询一个用户所有记录
        /// </summary>
        public List<Next.WorkFlow.Entity.WorkFlowDelegation> GetByUserID(string userID)
        {
            string sql = string.Format("SELECT * FROM WorkFlowDelegation WHERE UserID={'0}'",userID);
            return GetList(sql);
        }

        /// <summary>
        /// 得到一页数据
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="query"></param>
        /// <param name="userID"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<Next.WorkFlow.Entity.WorkFlowDelegation> GetPagerData(out string pager, string query = "", string userID = "", string startTime = "", string endTime = "")
        {
            StringBuilder WHERE = new StringBuilder();
            List<MySqlParameter> parList = new List<MySqlParameter>();

            if (userID.IsGuid())
            {
                WHERE.Append("AND UserID=@UserID ");
                parList.Add(new MySqlParameter("@UserID", SqlDbType.UniqueIdentifier) { Value = userID.ToGuid() });
            }
            if (startTime.IsDateTime())
            {
                WHERE.Append("AND StartTime>=@StartTime ");
                parList.Add(new MySqlParameter("@StartTime", SqlDbType.DateTime) { Value = startTime.ToDateTime().ToString("yyyy-MM-dd").ToDateTime() });
            }
            if (endTime.IsDateTime())
            {
                WHERE.Append("AND EndTime<=@EndTime ");
                parList.Add(new MySqlParameter("@EndTime", SqlDbType.DateTime) { Value = endTime.ToDateTime().AddDays(1).ToString("yyyy-MM-dd").ToDateTime() });
            }
            long count;
            int pageSize = Next.WorkFlow.Utility.Tools.GetPageSize();
            int pageNumber = Next.WorkFlow.Utility.Tools.GetPageNumber();
            string sql = dbHelper.GetPaerSql("WorkFlowDelegation", "*", WHERE.ToString(), "WriteTime Desc", pageSize, pageNumber, out count, parList.ToArray());

            pager = Next.WorkFlow.Utility.Tools.GetPagerHtml(count, pageSize, pageNumber, query);
            List<Next.WorkFlow.Entity.WorkFlowDelegation> List = GetList(sql);
            return List;
        }

        /// <summary>
        /// 得到未过期的委托
        /// </summary>
        public List<Next.WorkFlow.Entity.WorkFlowDelegation> GetNoExpiredList()
        {
            string sql = string.Format("SELECT * FROM WorkFlowDelegation WHERE EndTime>='{0}'",Next.WorkFlow.Utility.DateTimeNew.Now);
            return GetList(sql);
        }
    }
}