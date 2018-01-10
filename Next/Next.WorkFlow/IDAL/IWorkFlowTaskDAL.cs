using Next.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Next.WorkFlow.Entity;

namespace Next.WorkFlow.IDAL
{
    public interface IWorkFlowTaskDAL : IBaseDAL<WorkFlowTask>
    {
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
        List<WorkFlowTask> GetTasks(string userID, out string pager, string query = "", string title = "", string flowid = "", string sender = "", string date1 = "", string date2 = "", int type = 0);

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
        List<WorkFlowTask> GetInstances(string[] flowID, string[] senderID, string[] receiveID, out string pager, string query = "", string title = "", string flowid = "", string date1 = "", string date2 = "", int status = 0);

        /// <summary>
        /// 更新打开时间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="openTime"></param>
        /// <param name="isStatus">是否将状态更新为1</param>
        void UpdateOpenTime(string id, DateTime openTime, bool isStatus = false);

        /// <summary>
        /// 得到一个流程实例的发起者
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        string GetFirstSnderID(string flowID, string groupID);

        /// <summary>
        /// 得到一个流程实例一个步骤的处理者
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        List<string> GetStepSnderID(string flowID, string stepID, string groupID);

        /// <summary>
        /// 得到一个流程实例前一步骤的处理者
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        List<string> GetPrevSnderID(string flowID, string stepID, string groupID);

        /// <summary>
        /// 完成一个任务
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="comment"></param>
        /// <param name="isSign"></param>
        /// <returns></returns>
        int Completed(string taskID, string comment = "", bool isSign = false, int status = 2, string note = "");

        /// <summary>
        /// 得到一个流程实例一个步骤的任务
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        List<WorkFlowTask> GetTaskList(string flowID, string stepID, string groupID);

        /// <summary>
        /// 得到一个实例的任务
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        List<WorkFlowTask> GetTaskList(string flowID, string groupID);

        /// <summary>
        /// 得到和一个任务同级的任务
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <param name="isStepID">是否区分步骤ID，多步骤会签区分的是上一步骤ID</param>
        /// <returns></returns>
        List<WorkFlowTask> GetTaskList(string taskID, bool isStepID = true);

        /// <summary>
        /// 得到一个任务的前一任务
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        List<WorkFlowTask> GetPrevTaskList(string taskID);

        /// <summary>
        /// 得到一个任务的后续任务
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        List<WorkFlowTask> GetNextTaskList(string taskID);

        /// <summary>
        /// 得到一个流程实例一个步骤一个人员的任务
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="stepID"></param>
        /// <param name="groupID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        List<WorkFlowTask> GetUserTaskList(string flowID, string stepID, string groupID, string userID);

        /// <summary>
        /// 更新一个任务后后续任务状态
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="comment"></param>
        /// <param name="isSign"></param>
        /// <returns></returns>
        int UpdateNextTaskStatus(string taskID, int status);

        /// <summary>
        /// 查询一个流程是否有任务数据
        /// </summary>
        /// <param name="flowID"></param>
        /// <returns></returns>
        bool HasTasks(string flowID);

        /// <summary>
        /// 查询一个用户在一个步骤是否有未完成任务
        /// </summary>
        /// <param name="flowID"></param>
        /// <returns></returns>
        bool HasNoCompletedTasks(string flowID, string stepID, string groupID, string userID);

        /// <summary>
        /// 得到一个任务的状态
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        int GetTaskStatus(string taskID);

        /// <summary>
        /// 根据SubFlowID得到一个任务
        /// </summary>
        /// <param name="subflowGroupID"></param>
        /// <returns></returns>
        List<WorkFlowTask> GetBySubFlowGroupID(string subflowGroupID);

        int Delete(string flowID, string groupID);
    }
}
