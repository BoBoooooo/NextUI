using Next.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Next.WorkFlow.Entity;

namespace Next.WorkFlow.IDAL
{
    public interface IWorkFlowCommentDAL : IBaseDAL<WorkFlowComment>
    {
        /// <summary>
        /// 查询管理员的所有记录
        /// </summary>
        List<WorkFlowComment> GetManagerAll();

        /// <summary>
        /// 得到管理员类别的最大排序值
        /// </summary>
        /// <returns></returns>
        int GetManagerMaxSort();

        /// <summary>
        /// 得到一个人员的最大排序值
        /// </summary>
        /// <returns></returns>
        int GetUserMaxSort(string userID);
    }
}
