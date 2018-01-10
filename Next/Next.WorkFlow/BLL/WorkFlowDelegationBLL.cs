using System;
using System.Collections.Generic;
using System.Text;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.DALMySql;
using Next.WorkFlow.IDAL;
using System.Linq;
using Next.WorkFlow.Utility;

namespace Next.WorkFlow.BLL
{
	public class WorkFlowDelegationBLL : BaseBLL<WorkFlowDelegation>
	{
		private IWorkFlowDelegationDAL workFlowDelegationDAL;
		public WorkFlowDelegationBLL(): base()
		{
			base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
			baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//如果需要记录操作日志，则实现这个事件
			this.workFlowDelegationDAL = (IWorkFlowDelegationDAL)base.baseDal;
		}
        /// <summary>
        /// 查询一个用户所有记录
        /// </summary>
        public List<Next.WorkFlow.Entity.WorkFlowDelegation> GetByUserID(string userID)
        {
            return workFlowDelegationDAL.GetByUserID(userID);
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
            return workFlowDelegationDAL.GetPagerData(out pager, query, userID, startTime, endTime);
        }

        /// <summary>
        /// 得到未过期的委托
        /// </summary>
        public List<Next.WorkFlow.Entity.WorkFlowDelegation> GetNoExpiredList()
        {
            return workFlowDelegationDAL.GetNoExpiredList();
        }

        /// <summary>
        /// 刷新缓存
        /// </summary>
        /*public void RefreshCache()
        {
            var list = GetNoExpiredList();
            RoadFlow.Cache.IO.Opation.Set(cacheKey, list);
        }*/
        
        /// <summary>
        /// 从缓存得到所有有效委托
        /// </summary>
        /// <returns></returns>
        public List<Next.WorkFlow.Entity.WorkFlowDelegation> GetNoExpiredListFromCache()
        {
            var list = GetNoExpiredList();
            //RoadFlow.Cache.IO.Opation.Set(cacheKey, list);
            return list;
            /*object obj = RoadFlow.Cache.IO.Opation.Get(cacheKey);
            if (obj != null && obj is List<Next.WorkFlow.Entity.WorkFlowDelegation>)
            {
                return obj as List<Next.WorkFlow.Entity.WorkFlowDelegation>;
            }
            else
            {
                var list = GetNoExpiredList();
                RoadFlow.Cache.IO.Opation.Set(cacheKey, list);
                return list;
            }*/

        }

        /// <summary>
        /// 得到一个流程一个用户是否有委托
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="userID"></param>
        /// <returns>返回Guid.Empty表示没有委托</returns>
        public string GetFlowDelegationByUserID(string flowID, string userID)
        {
            var list = GetNoExpiredListFromCache();
            if (list.Count == 0)
            {
                return string.Empty;
            }
            string toUserID = string.Empty;
            var userList = list.Where(p => p.UserID == userID && (!p.FlowID.IsNullOrEmpty() || p.FlowID == string.Empty || p.FlowID == flowID) && p.EndTime >= Next.WorkFlow.Utility.DateTimeNew.Now);

            if (userList.Count() == 0)
            {
                toUserID = string.Empty;
            }
            else
            {
                toUserID = userList.OrderByDescending(p => p.WriteTime).First().ToUserID;
            }

            return getFlowDelegationByUserID1(flowID, toUserID, list);

        }
        private string getFlowDelegationByUserID1(string flowID, string userID, List<Next.WorkFlow.Entity.WorkFlowDelegation> list)
        {

            var userList = list.Where(p => p.UserID == userID && (!p.FlowID.IsNullOrEmpty() || p.FlowID == string.Empty || p.FlowID == flowID) && p.EndTime >= Next.WorkFlow.Utility.DateTimeNew.Now);
            if (userList.Count() == 0)
            {
                return userID;
            }
            else
            {
                userID = userList.OrderByDescending(p => p.WriteTime).First().ToUserID;
                getFlowDelegationByUserID1(flowID, userID, list);
            }

            return string.Empty;
        }


    }
}

