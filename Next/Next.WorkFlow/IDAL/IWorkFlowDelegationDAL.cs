using Next.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Next.WorkFlow.Entity;

namespace Next.WorkFlow.IDAL
{
    public interface IWorkFlowDelegationDAL : IBaseDAL<WorkFlowDelegation>
    {
                /// <summary>
        /// 查询一个用户所有记录
        /// </summary>
        List<Next.WorkFlow.Entity.WorkFlowDelegation> GetByUserID(string userID);

        /// <summary>
        /// 得到一页数据
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="query"></param>
        /// <param name="userID"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        List<Next.WorkFlow.Entity.WorkFlowDelegation> GetPagerData(out string pager, string query = "", string userID = "", string startTime = "", string endTime = "");

        /// <summary>
        /// 得到未过期的委托
        /// </summary>
        List<Next.WorkFlow.Entity.WorkFlowDelegation> GetNoExpiredList();
    }
}

