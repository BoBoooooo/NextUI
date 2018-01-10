using System;
using System.Collections.Generic;
using System.Text;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.DALMySql;
using Next.WorkFlow.IDAL;
using Next.Admin.BLL;
using Next.WorkFlow.Utility;
using System.Linq;

namespace Next.WorkFlow.BLL
{
	public class WorkFlowCommentBLL : BaseBLL<WorkFlowComment>
	{
		private IWorkFlowCommentDAL workFlowCommentDAL;
		public WorkFlowCommentBLL(): base()
		{
			base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
			baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//如果需要记录操作日志，则实现这个事件
			this.workFlowCommentDAL = (IWorkFlowCommentDAL)base.baseDal;
		}
        /// <summary>
        /// 查询管理员的所有记录
        /// </summary>
        public List<WorkFlowComment> GetManagerAll()
        {
            return workFlowCommentDAL.GetManagerAll();
        }

        /// <summary>
        /// 得到管理员类别的最大排序值
        /// </summary>
        /// <returns></returns>
        public int GetManagerMaxSort()
        {
            return workFlowCommentDAL.GetManagerMaxSort();
        }

        /// <summary>
        /// 得到一个人员的最大排序值
        /// </summary>
        /// <returns></returns>
        public int GetUserMaxSort(string userID)
        {
            return workFlowCommentDAL.GetUserMaxSort(userID);
        }

        /// <summary>
        /// 获得所有列表
        /// </summary>
        /// <param name="fromCache">是否从缓存获取</param>
        /// <returns></returns>
        private List<Tuple<string, string, int, int, List<string>>> GetAllList(bool fromCache = false)
        {
                return getAllListByDb();
        }
        /// <summary>
        /// 从数据库获取所有意见列表
        /// </summary>
        /// <returns></returns>
        private List<Tuple<string, string, int, int, List<string>>> getAllListByDb()
        {
            var comments = GetAll();
            DeptBLL borganize = new DeptBLL();
            List<Tuple<string, string, int, int, List<string>>> list = new List<Tuple<string, string, int, int, List<string>>>();
            foreach (var comment in comments)
            {
                List<string> userList = new List<string>();
                if (!comment.MemberID.IsNullOrEmpty())
                {
                    var users = borganize.GetAllUsers(comment.MemberID);
                    foreach (var user in users)
                    {
                        userList.Add(user.ID);
                    }
                }

                Tuple<string, string, int, int, List<string>> tuple = new Tuple<string, string, int, int, List<string>>(
                    comment.ID,
                    comment.Comment,
                    comment.Type,
                    comment.Sort,
                    userList
                    );
                list.Add(tuple);
            }
            return list;
        }

        /// <summary>
        /// 得到一个用户的所有意见
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<string> GetListByUserID(string userID)
        {
            var list = GetAllList();
            var list1 = list.Where(p => p.Item5.Count == 0 || p.Item5.Exists(q => q == userID)).OrderByDescending(p => p.Item3).ThenBy(p => p.Item4);
            List<string> commentsList = new List<string>();
            foreach (var li in list1.OrderBy(p => p.Item3).ThenBy(p => p.Item4))
            {
                commentsList.Add(li.Item2);
            }
            return commentsList;
        }
        /// <summary>
        /// 得到一个用户的所有意见选择项
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public string GetOptionsStringByUserID(string userID)
        {
            var list = GetListByUserID(userID);
            StringBuilder options = new StringBuilder();
            foreach (var li in list)
            {
                options.AppendFormat("<option value=\"{0}\">{0}</option>", li);
            }
            return options.ToString();
        }
    }
}
