using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.IDAL;
using Next.WorkFlow.Utility;
using MySql.Data.MySqlClient;

namespace Next.WorkFlow.DALMySql
{
	public class WorkFlowTaskDAL: BaseDALMySql<WorkFlowTask> , IWorkFlowTaskDAL
	{
		public static WorkFlowTaskDAL Instance
		{
			get
			{
				return new WorkFlowTaskDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public WorkFlowTaskDAL()
		: base("WorkFlowTask", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
        /// <summary>
        /// 更新打开时间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="openTime"></param>
        /// <param name="isStatus">是否将状态更新为1</param>
        public void UpdateOpenTime(string id, DateTime openTime, bool isStatus = false)
        {
            string sql = string.Format("UPDATE WorkFlowTask SET OpenTime='{0}' " + (isStatus ? ", Status=1" : "") + " WHERE ID='{1}' AND OpenTime IS NULL",openTime,id);
            SqlExecute(sql);
        }


        /// <summary>
        /// 查询待办任务
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="pager"></param>
        /// <param name="query"></param>
        /// <param name="title"></param>
        /// <param name="flowid"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="type">0待办 1已完成</param>
        /// <returns></returns>
        public List<WorkFlowTask> GetTasks(string userID, out string pager, string query = "", string title = "", string flowid = "", string sender = "", string date1 = "", string date2 = "", int type = 0)
        {
            List<MySqlParameter> parList = new List<MySqlParameter>();
            StringBuilder sql = new StringBuilder();
            if (type == 0)
            {
                sql = new StringBuilder("SELECT *,ROW_NUMBER() OVER (ORDER BY ReceiveTime DESC) AS PagerAutoRowNumber FROM WorkFlowTask WHERE ReceiveID='"+userID+"' AND Status IN(0,1)");
            }else{
                sql = new StringBuilder("SELECT *,ROW_NUMBER() OVER (ORDER BY CompletedTime1 DESC) AS PagerAutoRowNumber FROM WorkFlowTask WHERE ReceiveID='" + userID + "' AND Status IN(2,3)");
            }

            if (!title.IsNullOrEmpty())
            {
                sql.Append(" AND CHARINDEX('"+title+"',Title)>0");
            }
            if (flowid.IsGuid())
            {
                sql.Append(" AND FlowID='"+flowid+"'");
            }
            else if (!flowid.IsNullOrEmpty() && flowid.IndexOf(',') >= 0)
            {
                sql.Append(" AND FlowID IN(" + Next.WorkFlow.Utility.Tools.GetSqlInString(flowid) + ")");
            }
            if (sender.IsGuid())
            {
                sql.Append(" AND SenderID=@SenderID");
                parList.Add(new MySqlParameter("@SenderID", SqlDbType.UniqueIdentifier) { Value = sender.ToGuid() });
            }
            if (date1.IsDateTime())
            {
                sql.Append(" AND ReceiveTime>=@ReceiveTime");
                parList.Add(new MySqlParameter("@ReceiveTime", SqlDbType.DateTime) { Value = date1.ToDateTime().Date });
            }
            if (date2.IsDateTime())
            {
                sql.Append(" AND ReceiveTime<=@ReceiveTime1");
                parList.Add(new MySqlParameter("@ReceiveTime1", SqlDbType.DateTime) { Value = date2.ToDateTime().AddDays(1).Date });
            }

            long count;
            int size = Next.WorkFlow.Utility.Tools.GetPageSize();
            int number = Next.WorkFlow.Utility.Tools.GetPageNumber();
            string sql1 = dbHelper.GetPaerSql(sql.ToString(), size, number, out count, parList.ToArray());
            pager = Next.WorkFlow.Utility.Tools.GetPagerHtml(count, size, number, query);


            SqlDataReader dataReader = dbHelper.GetDataReader(sql1, parList.ToArray());
            List<WorkFlowTask> List = DataReaderToList(dataReader);
            dataReader.Close();
            return List;
        }

        /// <summary>
        /// 得到流程实例列表
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="senderID"></param>
        /// <param name="receiveID"></param>
        /// <param name="pager"></param>
        /// <param name="query"></param>
        /// <param name="title"></param>
        /// <param name="flowid"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="status">是否完成 0:全部 1:未完成 2:已完成</param>
        /// <returns></returns>
        public List<WorkFlowTask> GetInstances(string[] flowID, string[] senderID, string[] receiveID, out string pager, string query = "", string title = "", string flowid = "", string date1 = "", string date2 = "", int status = 0)
        {
            List<MySqlParameter> parList = new List<MySqlParameter>();
            StringBuilder sql = new StringBuilder(@"SELECT a.*,ROW_NUMBER() OVER(ORDER BY a.SenderTime DESC) AS PagerAutoRowNumber FROM WorkFlowTask a
                WHERE a.ID=(SELECT TOP 1 ID FROM WorkFlowTask WHERE FlowID=a.FlowID AND GroupID=a.GroupID ORDER BY Sort DESC)");

            if (status != 0)
            {
                if (status == 1)
                {
                    sql.Append(" AND a.Status IN(0,1,5)");
                }
                else if (status == 2)
                {
                    sql.Append(" AND a.Status IN(2,3,4)");
                }
            }

            if (flowID != null && flowID.Length > 0)
            {
                sql.Append(string.Format(" AND a.FlowID IN({0})", Next.WorkFlow.Utility.Tools.GetSqlInString(flowID)));
            }
            if (senderID != null && senderID.Length > 0)
            {
                if (senderID.Length == 1)
                {
                    sql.Append(" AND a.SenderID=@SenderID");
                    parList.Add(new MySqlParameter("@SenderID", SqlDbType.UniqueIdentifier) { Value = senderID[0] });
                }
                else
                {
                    sql.Append(string.Format(" AND a.SenderID IN({0})", Next.WorkFlow.Utility.Tools.GetSqlInString(senderID)));
                }
            }
            if (receiveID != null && receiveID.Length > 0)
            {
                if (senderID.Length == 1)
                {
                    sql.Append(" AND a.ReceiveID=@ReceiveID");
                    parList.Add(new MySqlParameter("@ReceiveID", SqlDbType.UniqueIdentifier) { Value = receiveID[0] });
                }
                else
                {
                    sql.Append(string.Format(" AND a.ReceiveID IN({0})", Next.WorkFlow.Utility.Tools.GetSqlInString(receiveID)));
                }
            }
            if (!title.IsNullOrEmpty())
            {
                sql.Append(" AND CHARINDEX(@Title,a.Title)>0");
                parList.Add(new MySqlParameter("@Title", SqlDbType.NVarChar, 2000) { Value = title });
            }
            if (flowid.IsGuid())
            {
                sql.Append(" AND a.FlowID=@FlowID");
                parList.Add(new MySqlParameter("@FlowID", SqlDbType.UniqueIdentifier) { Value = flowid.ToGuid() });
            }
            else if (!flowid.IsNullOrEmpty() && flowid.IndexOf(',') >= 0)
            {
                sql.Append(" AND a.FlowID IN(" + Next.WorkFlow.Utility.Tools.GetSqlInString(flowid) + ")");
            }
            if (date1.IsDateTime())
            {
                sql.Append(" AND a.SenderTime>=@SenderTime");
                parList.Add(new MySqlParameter("@SenderTime", SqlDbType.DateTime) { Value = date1.ToDateTime().Date });
            }
            if (date2.IsDateTime())
            {
                sql.Append(" AND a.SenderTime<=@SenderTime1");
                parList.Add(new MySqlParameter("@SenderTime1", SqlDbType.DateTime) { Value = date1.ToDateTime().AddDays(1).Date });
            }

            long count;
            int size = Next.WorkFlow.Utility.Tools.GetPageSize();
            int number = Next.WorkFlow.Utility.Tools.GetPageNumber();
            string sql1 = dbHelper.GetPaerSql(sql.ToString(), size, number, out count, parList.ToArray());
            pager = Next.WorkFlow.Utility.Tools.GetPagerHtml(count, size, number, query);

            SqlDataReader dataReader = dbHelper.GetDataReader(sql1, parList.ToArray());
            List<WorkFlowTask> List = DataReaderToList(dataReader);
            dataReader.Close();
            return List;
        }

        /// <summary>
        /// 得到一个流程实例的发起者
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public string GetFirstSnderID(string flowID, string groupID)
        {
            string sql = string.Format("SELECT SenderID FROM WorkFlowTask WHERE FlowID='{0}' AND GroupID='{1}' AND PrevID='{2}'",flowID,groupID,string.Empty);

            return SqlTable(sql).Rows[0][0].ToString();
        }

        /// <summary>
        /// 得到一个流程实例一个步骤的处理者
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public List<string> GetStepSnderID(string flowID, string stepID, string groupID)
        {
            string sql = string.Format("SELECT ReceiveID FROM WorkFlowTask WHERE FlowID='{0}' AND StepID='{1}' AND GroupID='{2}'", flowID, stepID, groupID);
            List<string> senderList = new List<string>();
            DataTable resultList = SqlTable(sql);
            foreach (DataRow dr in resultList.Rows)
            {
                    senderList.Add(dr[0].ToString());
            }
            return senderList;
        }
        /// <summary>
        /// 得到一个流程实例前一步骤的处理者
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public List<string> GetPrevSnderID(string flowID, string stepID, string groupID)
        {
            string sql = string.Format("SELECT ReceiveID FROM WorkFlowTask WHERE ID=(SELECT PrevID FROM WorkFlowTask WHERE FlowID='{0}' AND StepID='{1}' AND GroupID='{2}')", flowID, stepID, groupID);
            List<string> senderList = new List<string>();
            DataTable resultList = SqlTable(sql);
            foreach (DataRow dr in resultList.Rows)
            {
                    senderList.Add(dr[0].ToString());
            }
            return senderList;
        }

        /// <summary>
        /// 完成一个任务
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="comment"></param>
        /// <param name="isSign"></param>
        /// <returns></returns>
        public int Completed(string taskID, string comment = "", bool isSign = false, int status = 2, string note = "")
        {
            string sql=string.Empty;
            if(note.IsNullOrEmpty()){
                sql = string.Format("UPDATE WorkFlowTask SET Comment='{0}',CompletedTime1='{1}',IsSign={2},Status={3}  WHERE ID={4}",comment,Next.WorkFlow.Utility.DateTimeNew.Now, isSign?1:0 ,status,taskID);
            }else{
                sql = string.Format("UPDATE WorkFlowTask SET Comment='{0}',CompletedTime1='{1}',IsSign={2},Status={3} ,Note={4}  WHERE ID={5}",comment,Next.WorkFlow.Utility.DateTimeNew.Now, isSign?1:0 ,status,note,taskID);
            }
            return SqlExecute(sql);
        }

        /// <summary>
        /// 更新一个任务后后续任务状态
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="comment"></param>
        /// <param name="isSign"></param>
        /// <returns></returns>
        public int UpdateNextTaskStatus(string taskID, int status)
        {
            string sql = string.Format("UPDATE WorkFlowTask SET Status={0} WHERE PrevID='{1}'",status,taskID);
            return SqlExecute(sql);
        }


        /// <summary>
        /// 得到一个流程实例一个步骤的任务
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public List<WorkFlowTask> GetTaskList(string flowID, string stepID, string groupID)
        {
            string sql = string.Format("SELECT * FROM WorkFlowTask WHERE FlowID='{0}' AND StepID='{1}' AND GroupID='{2}'", flowID, stepID, groupID);
            return GetList(sql);
        }

        /// <summary>
        /// 得到一个流程实例一个步骤一个人员的任务
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="stepID"></param>
        /// <param name="groupID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<WorkFlowTask> GetUserTaskList(string flowID, string stepID, string groupID, string userID)
        {
            string sql = string.Format("SELECT * FROM WorkFlowTask WHERE FlowID='{0}' AND StepID='{1}' AND GroupID='{2}' AND ReceiveID='{3}'",flowID, stepID, groupID, userID);
            return GetList(sql);
        }


        /// <summary>
        /// 得到一个实例的任务
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public List<WorkFlowTask> GetTaskList(string flowID, string groupID)
        {
            string sql = string.Empty;
            if (flowID == null || flowID.IsEmptyGuid())
            {
                sql = string.Format("SELECT * FROM WorkFlowTask WHERE GroupID='{0}')", groupID);
            }
            else
            {
                sql = string.Format("SELECT * FROM WorkFlowTask WHERE FlowID='{0}' AND GroupID='{1}'",flowID,groupID);
            }
            return GetList(sql);
        }

        /// <summary>
        /// 得到和一个任务同级的任务
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <param name="isStepID">是否区分步骤ID，多步骤会签区分的是上一步骤ID</param>
        /// <returns></returns>
        public List<WorkFlowTask> GetTaskList(string taskID, bool isStepID = true)
        {
            var task = FindByID(taskID);
            if (task == null)
            {
                return new List<WorkFlowTask>() { };
            }
            string sql = null; 
            if (isStepID)
            {
                sql = string.Format("SELECT * FROM WorkFlowTask WHERE PrevID={'0} AND {1}", task.PrevID, task.StepID);//isStepID ? "StepID=@StepID" : "PrevStepID=@StepID");
            }
            else
            {
                sql = string.Format("SELECT * FROM WorkFlowTask WHERE PrevID={'0} AND {1}", task.PrevID, task.PrevStepID);
            }
            return GetList(sql);
        }

        /// <summary>
        /// 得到一个任务的前一任务
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public List<WorkFlowTask> GetPrevTaskList(string taskID)
        {
            string sql = string.Format("SELECT * FROM WorkFlowTask WHERE ID=(SELECT PrevID FROM WorkFlowTask WHERE ID='{0}')",taskID);
            return GetList(sql);
        }

        /// <summary>
        /// 得到一个任务的后续任务
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public List<WorkFlowTask> GetNextTaskList(string taskID)
        {
            string sql = string.Format("SELECT * FROM WorkFlowTask WHERE PrevID='{0}')", taskID);
            return GetList(sql);
        }


        /// <summary>
        /// 查询一个流程是否有任务数据
        /// </summary>
        /// <param name="flowID"></param>
        /// <returns></returns>
        public bool HasTasks(string flowID)
        {
            string sql = string.Format("SELECT TOP 1 ID FROM WorkFlowTask WHERE FlowID='{0}')", flowID);
            bool has = GetList(sql).Count > 0;
            return has;
        }

        /// <summary>
        /// 查询一个用户在一个步骤是否有未完成任务
        /// </summary>
        /// <param name="flowID"></param>
        /// <returns></returns>
        public bool HasNoCompletedTasks(string flowID, string stepID, string groupID, string userID)
        {
            string sql = string.Format("SELECT TOP 1 ID FROM WorkFlowTask WHERE FlowID='{0}' AND StepID='{1}' AND GroupID='{2}' AND ReceiveID='{3}' AND Status IN(0,1)", flowID, stepID, groupID, userID);
            bool has = GetList(sql).Count>0;
            return has;
        }

        /// <summary>
        /// 得到一个任务的状态
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public int GetTaskStatus(string taskID)
        {
            string sql = string.Format("SELECT Status FROM WorkFlowTask WHERE ID='{0}')", taskID);
            string status = SqlTable(sql).Rows[0][0].ToString();
            int s;
            return status.IsInt(out s) ? s : -1;
        }

        /// <summary>
        /// 根据SubFlowID得到一个任务
        /// </summary>
        /// <param name="subflowGroupID"></param>
        /// <returns></returns>
        public List<WorkFlowTask> GetBySubFlowGroupID(string subflowGroupID)
        {
            string sql = string.Format("SELECT * FROM WorkFlowTask WHERE SubFlowGroupID='{0}')", subflowGroupID);
            return GetList(sql);
        }
    }
}