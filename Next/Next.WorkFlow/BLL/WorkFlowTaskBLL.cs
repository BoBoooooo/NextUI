using System;
using System.Collections.Generic;
using System.Text;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.DALMySql;
using Next.WorkFlow.IDAL;
using Next.WorkFlow.Utility;
using Next.Admin.BLL;
using System.Linq;

namespace Next.WorkFlow.BLL
{
	public class WorkFlowTaskBLL : BaseBLL<WorkFlowTask> 
	{
        private WorkFlowInfoBLL bWorkFlow = new WorkFlowInfoBLL();
        private WorkFlowInstalled wfInstalled;
        private Next.WorkFlow.Entity.WorkFlowExecute.Result result;
        private List<WorkFlowTask> nextTasks = new List<WorkFlowTask>();
		private IWorkFlowTaskDAL workFlowTaskDAL;
		public WorkFlowTaskBLL(): base()
		{
			base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
			baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//如果需要记录操作日志，则实现这个事件
			this.workFlowTaskDAL = (IWorkFlowTaskDAL)base.baseDal;
		}

        /// <summary>
        /// 去除重复的接收人，在退回任务时去重，避免一个人收到多条任务。
        /// </summary>
        /// <param name="task1"></param>
        /// <param name="task2"></param>
        /// <returns></returns>
        public bool Equals(WorkFlowTask task1, WorkFlowTask task2)
        {
            return task1.ReceiveID == task2.ReceiveID;
        }

        public int GetHashCode(WorkFlowTask task)
        {
            return task.ToString().GetHashCode();
        }

        /// <summary>
        /// 更新打开时间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="openTime"></param>
        /// <param name="isStatus">是否将状态更新为1</param>
        public void UpdateOpenTime(string id, DateTime openTime, bool isStatus = false)
        {
            workFlowTaskDAL.UpdateOpenTime(id, openTime, isStatus);
        }

        /// <summary>
        /// 得到一个流程实例的发起者
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="groupID"></param>
        /// <param name="isDefault">如果为空是否返回当前登录用户ID</param>
        /// <returns></returns>
        public string GetFirstSnderID(string flowID, string groupID, bool isDefault = false)
        {
            string senderID = workFlowTaskDAL.GetFirstSnderID(flowID, groupID);
            return senderID.IsEmptyGuid() && isDefault ? new UserBLL().CurrentUserID : senderID;
        }

        /// <summary>
        /// 得到一个流程实例的发起者部门
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public string GetFirstSnderDeptID(string flowID, string groupID)
        {
            if (flowID.IsEmptyGuid() || groupID.IsEmptyGuid())
            {
                return new UserBLL().CurrentDeptID;
            }
            var senderID = workFlowTaskDAL.GetFirstSnderID(flowID, groupID);
            var dept = new UserBLL().FindByID(senderID);
            return dept == null ? string.Empty : dept.ID;
        }


        /// <summary>
        /// 得到一个流程实例一个步骤的处理者
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public List<string> GetStepSnderID(string flowID, string stepID, string groupID)
        {
            return workFlowTaskDAL.GetStepSnderID(flowID, stepID, groupID);
        }

        /// <summary>
        /// 得到一个流程实例一个步骤的处理者字符串
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="stepID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public string GetStepSnderIDString(string flowID, string stepID, string groupID)
        {
            var list = GetStepSnderID(flowID, stepID, groupID);
            StringBuilder sb = new StringBuilder(list.Count * 43);
            foreach (var li in list)
            {
                sb.Append(Next.Admin.BLL.UserBLL.PREFIX);
                sb.Append(li);
                sb.Append(",");
            }
            return sb.ToString().TrimEnd(',');
        }

        /// <summary>
        /// 得到一个流程实例前一步骤的处理者
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public List<string> GetPrevSnderID(string flowID, string stepID, string groupID)
        {
            return workFlowTaskDAL.GetPrevSnderID(flowID, stepID, groupID);
        }

        /// <summary>
        /// 得到一个流程实例前一步骤的处理者
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public String GetPrevSnderIDString(string flowID, string stepID, string groupID)
        {
            var list = workFlowTaskDAL.GetPrevSnderID(flowID, stepID, groupID);
            StringBuilder sb = new StringBuilder(list.Count * 43);
            foreach (var li in list)
            {
                sb.Append(Next.Admin.BLL.UserBLL.PREFIX);
                sb.Append(li);
                sb.Append(",");
            }
            return sb.ToString().TrimEnd(',');
        }

        /// <summary>
        /// 将json字符串转换为执行实体
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        private Next.WorkFlow.Entity.WorkFlowExecute.Execute GetExecuteModel(string jsonString)
        {
            Next.WorkFlow.Entity.WorkFlowExecute.Execute execute = new Next.WorkFlow.Entity.WorkFlowExecute.Execute();
            DeptBLL borganize = new DeptBLL();

            LitJson.JsonData jsondata = LitJson.JsonMapper.ToObject(jsonString);
            if (jsondata == null) return execute;

            execute.Comment = jsondata["comment"].ToString();
            string op = jsondata["type"].ToString().ToLower();
            switch (op)
            {
                case "submit":
                    execute.ExecuteType = Next.WorkFlow.Entity.WorkFlowExecute.EnumType.ExecuteType.Submit;
                    break;
                case "save":
                    execute.ExecuteType = Next.WorkFlow.Entity.WorkFlowExecute.EnumType.ExecuteType.Save;
                    break;
                case "back":
                    execute.ExecuteType = Next.WorkFlow.Entity.WorkFlowExecute.EnumType.ExecuteType.Back;
                    break;
            }
            execute.FlowID = jsondata["flowid"].ToString().ToGuid();
            execute.GroupID = jsondata["groupid"].ToString().ToGuid();
            execute.InstanceID = jsondata["instanceid"].ToString();
            execute.IsSign = jsondata["issign"].ToString().ToInt() == 1;
            execute.StepID = jsondata["stepid"].ToString().ToGuid();
            execute.TaskID = jsondata["taskid"].ToString().ToGuid();

            var stepsjson = jsondata["steps"];
            Dictionary<string, List<Next.Admin.Entity.User>> steps = new Dictionary<string, List<Next.Admin.Entity.User>>();
            if (stepsjson.IsArray)
            {
                foreach (LitJson.JsonData step in stepsjson)
                {
                    var id = step["id"].ToString().ToGuid();
                    var member = step["member"].ToString();
                    if (id == string.Empty || member.IsNullOrEmpty())
                    {
                        continue;
                    }
                    steps.Add(id, borganize.GetAllUsers(member));
                }
            }
            execute.Steps = steps;
            return execute;
        }

        /// <summary>
        /// 处理流程
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public Next.WorkFlow.Entity.WorkFlowExecute.Result Execute(string jsonString)
        {
            return Execute(GetExecuteModel(jsonString));
        }


        /// <summary>
        /// 处理流程
        /// </summary>
        /// <param name="executeModel">处理实体</param>
        /// <returns></returns>
        public Next.WorkFlow.Entity.WorkFlowExecute.Result Execute(Next.WorkFlow.Entity.WorkFlowExecute.Execute executeModel)
        {
            result = new Next.WorkFlow.Entity.WorkFlowExecute.Result();
            nextTasks = new List<WorkFlowTask>();
            if (executeModel.FlowID == string.Empty)
            {
                result.DebugMessages = "流程ID错误";
                result.IsSuccess = false;
                result.Messages = "执行参数错误";
                return result;
            }


            wfInstalled = bWorkFlow.GetWorkFlowRunModel(executeModel.FlowID);
            if (wfInstalled == null)
            {
                result.DebugMessages = "未找到流程运行时实体";
                result.IsSuccess = false;
                result.Messages = "流程运行时为空";
                return result;
            }

            lock (executeModel.GroupID.ToString())
            {
                switch (executeModel.ExecuteType)
                {
                    case Next.WorkFlow.Entity.WorkFlowExecute.EnumType.ExecuteType.Back:
                        executeBack(executeModel);
                        break;
                    //case Next.WorkFlow.Entity.WorkFlowExecute.EnumType.ExecuteType.Completed:
                    //    executeComplete(executeModel);
                    //    break;
                    case Next.WorkFlow.Entity.WorkFlowExecute.EnumType.ExecuteType.Save:
                        executeSave(executeModel);
                        break;
                    case Next.WorkFlow.Entity.WorkFlowExecute.EnumType.ExecuteType.Submit:
                    case Next.WorkFlow.Entity.WorkFlowExecute.EnumType.ExecuteType.Completed:
                        executeSubmit(executeModel);
                        break;
                    case Next.WorkFlow.Entity.WorkFlowExecute.EnumType.ExecuteType.Redirect:
                        executeRedirect(executeModel);
                        break;
                    default:
                        result.DebugMessages = "流程处理类型为空";
                        result.IsSuccess = false;
                        result.Messages = "流程处理类型为空";
                        return result;
                }

                result.NextTasks = nextTasks;
                return result;
            }
        }

        private void executeSubmit(Next.WorkFlow.Entity.WorkFlowExecute.Execute executeModel)
        {
            using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope())
            {
                //如果是第一步提交并且没有实例则先创建实例
                WorkFlowTask currentTask = null;
                bool isFirst = executeModel.StepID == wfInstalled.FirstStepID && executeModel.TaskID == Guid.Empty.ToString() && executeModel.GroupID == Guid.Empty.ToString();
                if (isFirst)
                {
                    currentTask = createFirstTask(executeModel);
                }
                else
                {
                    currentTask = FindByID(executeModel.TaskID);
                }
                if (currentTask == null)
                {
                    result.DebugMessages = "未能创建或找到当前任务";
                    result.IsSuccess = false;
                    result.Messages = "未能创建或找到当前任务";
                    return;
                }
                else if (currentTask.Status.In(2, 3, 4))
                {
                    result.DebugMessages = "当前任务已处理";
                    result.IsSuccess = false;
                    result.Messages = "当前任务已处理";
                    return;
                }

                var currentSteps = wfInstalled.Steps.Where(p => p.ID == executeModel.StepID);
                var currentStep = currentSteps.Count() > 0 ? currentSteps.First() : null;
                if (currentStep == null)
                {
                    result.DebugMessages = "未找到当前步骤";
                    result.IsSuccess = false;
                    result.Messages = "未找到当前步骤";
                    return;
                }

                //如果当前步骤是子流程步骤，并且策略是 子流程完成后才能提交 则要判断子流程是否已完成
                if (currentStep.Type == "subflow"
                    && currentStep.SubFlowID.IsGuid()
                    && currentStep.Behavior.SubFlowStrategy == 0
                    && currentTask.SubFlowGroupID.IsGuid()
                    && !currentTask.SubFlowGroupID.IsEmptyGuid()
                    && !GetInstanceIsCompleted(currentStep.SubFlowID.ToGuid(), currentTask.SubFlowGroupID))
                {
                    result.DebugMessages = "当前步骤的子流程实例未完成,子流程：" + currentStep.SubFlowID + ",实例组：" + currentTask.SubFlowGroupID.ToString();
                    result.IsSuccess = false;
                    result.Messages = "当前步骤的子流程未完成,不能提交!";
                    return;
                }

                int status = 0;
                #region 处理策略判断
                if (currentTask.StepID != wfInstalled.FirstStepID)//第一步不判断策略
                {
                    switch (currentStep.Behavior.HanlderModel)
                    {
                        case 0://所有人必须处理
                            var taskList = GetTaskListByStepID(currentTask.ID, currentTask.StepID);
                            if (taskList.Count > 1)
                            {
                                var noCompleted = taskList.Where(p => p.Status != 2);
                                if (noCompleted.Count() - 1 > 0)
                                {
                                    status = -1;
                                }
                            }
                            Completed(currentTask.ID, executeModel.Comment, executeModel.IsSign);
                            break;
                        case 1://一人同意即可
                            var taskList1 = GetTaskListByStepID(currentTask.ID, currentTask.StepID);
                            foreach (var task in taskList1)
                            {
                                if (task.ID != currentTask.ID)
                                {
                                    if (task.Status.In(0, 1))
                                    {
                                        Completed(task.ID, "", false, 4);
                                    }
                                }
                                else
                                {
                                    Completed(task.ID, executeModel.Comment, executeModel.IsSign);
                                }
                            }
                            break;
                        case 2://依据人数比例
                            var taskList2 = GetTaskListByStepID(currentTask.ID, currentTask.StepID);
                            if (taskList2.Count > 1)
                            {
                                decimal percentage = currentStep.Behavior.Percentage <= 0 ? 100 : currentStep.Behavior.Percentage;//比例
                                if ((((decimal)(taskList2.Where(p => p.Status == 2).Count() + 1) / (decimal)taskList2.Count) * 100).Round() < percentage)
                                {
                                    status = -1;
                                }
                                else
                                {
                                    foreach (var task in taskList2)
                                    {
                                        if (task.ID != currentTask.ID && task.Status.In(0, 1))
                                        {
                                            Completed(task.ID, "", false, 4);
                                        }
                                    }
                                }
                            }
                            Completed(currentTask.ID, executeModel.Comment, executeModel.IsSign);
                            break;
                        case 3://独立处理
                            Completed(currentTask.ID, executeModel.Comment, executeModel.IsSign);
                            break;
                    }
                }
                else
                {
                    Completed(currentTask.ID, executeModel.Comment, executeModel.IsSign);
                }
                #endregion

                //如果条件不满足则不创建后续任务，直到最后一个条件满足时才创建后续任务。等待中的任务 状态为：5 已不用
                if (status == -1)
                {
                    result.DebugMessages += "已发送,其他人未处理,不创建后续任务";
                    result.IsSuccess = true;
                    result.Messages += "已发送,等待他人处理!";
                    result.NextTasks = nextTasks;
                    scope.Complete();
                    return;
                }

                if (executeModel.ExecuteType == Next.WorkFlow.Entity.WorkFlowExecute.EnumType.ExecuteType.Completed || executeModel.Steps == null || executeModel.Steps.Count == 0)
                {
                    executeComplete(executeModel, false);
                    scope.Complete();
                    return;
                }

                foreach (var step in executeModel.Steps)
                {
                    foreach (var user in step.Value)
                    {
                        if (HasNoCompletedTasks(executeModel.FlowID, step.Key, currentTask.GroupID, user.ID))
                        {
                            continue;
                        }

                        var nextSteps = wfInstalled.Steps.Where(p => p.ID == step.Key);
                        if (nextSteps.Count() == 0)
                        {
                            continue;
                        }
                        var nextStep = nextSteps.First();

                        bool isPassing = 0 == nextStep.Behavior.Countersignature;

                        #region 如果下一步骤为会签，则要检查当前步骤的平级步骤是否已处理
                        if (0 != nextStep.Behavior.Countersignature)
                        {
                            var prevSteps = bWorkFlow.GetPrevSteps(executeModel.FlowID, nextStep.ID);
                            switch (nextStep.Behavior.Countersignature)
                            {
                                case 1://所有步骤同意
                                    isPassing = true;
                                    foreach (var prevStep in prevSteps)
                                    {
                                        if (!IsPassing(prevStep, executeModel.FlowID, executeModel.GroupID, currentTask.PrevID))
                                        {
                                            isPassing = false;
                                            break;
                                        }
                                    }
                                    break;
                                case 2://一个步骤同意即可
                                    isPassing = false;
                                    foreach (var prevStep in prevSteps)
                                    {
                                        if (IsPassing(prevStep, executeModel.FlowID, executeModel.GroupID, currentTask.PrevID))
                                        {
                                            isPassing = true;
                                            break;
                                        }
                                    }
                                    break;
                                case 3://依据比例
                                    int passCount = 0;
                                    foreach (var prevStep in prevSteps)
                                    {
                                        if (IsPassing(prevStep, executeModel.FlowID, executeModel.GroupID, currentTask.PrevID))
                                        {
                                            passCount++;
                                        }
                                    }
                                    isPassing = (((decimal)passCount / (decimal)prevSteps.Count) * 100).Round() >= (nextStep.Behavior.CountersignaturePercentage <= 0 ? 100 : nextStep.Behavior.CountersignaturePercentage);
                                    break;
                            }
                            if (isPassing)
                            {
                                var tjTasks = GetTaskList(currentTask.ID, false);
                                foreach (var tjTask in tjTasks)
                                {
                                    if (tjTask.ID == currentTask.ID || tjTask.Status.In(2, 3, 4, 5))
                                    {
                                        continue;
                                    }
                                    Completed(tjTask.ID, "", false, 4);
                                }
                            }
                        }
                        #endregion

                        if (isPassing)
                        {
                            WorkFlowTask task = new WorkFlowTask();
                            if (nextStep.WorkTime > 0)
                            {
                                task.CompletedTime = Next.WorkFlow.Utility.DateTimeNew.Now.AddHours((double)nextStep.WorkTime);
                            }

                            task.FlowID = executeModel.FlowID;
                            task.GroupID = currentTask != null ? currentTask.GroupID : executeModel.GroupID;
                            task.ID = Guid.NewGuid().ToString();
                            task.Type = 0;
                            task.InstanceID = executeModel.InstanceID;
                            if (!executeModel.Note.IsNullOrEmpty())
                            {
                                task.Note = executeModel.Note;
                            }
                            task.PrevID = currentTask.ID;
                            task.PrevStepID = currentTask.StepID;
                            task.ReceiveID = user.ID;
                            task.ReceiveName = user.Name;
                            task.ReceiveTime = Next.WorkFlow.Utility.DateTimeNew.Now;
                            task.SenderID = executeModel.Sender.ID;
                            task.SenderName = executeModel.Sender.Name;
                            task.SenderTime = task.ReceiveTime;
                            task.Status = status;
                            task.StepID = step.Key;
                            task.StepName = nextSteps.First().Name;
                            task.Sort = currentTask.Sort + 1;
                            task.Title = executeModel.Title.IsNullOrEmpty() ? currentTask.Title : executeModel.Title;

                            #region 如果当前步骤是子流程步骤，则要发起子流程实例
                            if (nextStep.Type == "subflow" && nextStep.SubFlowID.IsGuid())
                            {
                                Next.WorkFlow.Entity.WorkFlowExecute.Execute subflowExecuteModel = new Next.WorkFlow.Entity.WorkFlowExecute.Execute();
                                if (!nextStep.Event.SubFlowActivationBefore.IsNullOrEmpty())
                                {
                                    object obj = ExecuteFlowCustomEvent(nextStep.Event.SubFlowActivationBefore.Trim(),
                                        new Next.WorkFlow.Entity.WorkFlowCustomEventParams()
                                        {
                                            FlowID = executeModel.FlowID,
                                            GroupID = currentTask.GroupID,
                                            InstanceID = currentTask.InstanceID,
                                            StepID = executeModel.StepID,
                                            TaskID = currentTask.ID
                                        });
                                    if (obj is Next.WorkFlow.Entity.WorkFlowExecute.Execute)
                                    {
                                        subflowExecuteModel = obj as Next.WorkFlow.Entity.WorkFlowExecute.Execute;
                                    }
                                }
                                subflowExecuteModel.ExecuteType = Next.WorkFlow.Entity.WorkFlowExecute.EnumType.ExecuteType.Save;
                                subflowExecuteModel.FlowID = nextStep.SubFlowID.ToGuid();
                                subflowExecuteModel.Sender = user;
                                if (subflowExecuteModel.Title.IsNullOrEmpty())
                                {
                                    subflowExecuteModel.Title = bWorkFlow.GetFlowName(subflowExecuteModel.FlowID);
                                }
                                if (subflowExecuteModel.InstanceID.IsNullOrEmpty())
                                {
                                    subflowExecuteModel.InstanceID = "";
                                }
                                var subflowTask = createFirstTask(subflowExecuteModel, true);
                                task.SubFlowGroupID = subflowTask.GroupID;
                            }
                            #endregion

                            Insert(task);
                            nextTasks.Add(task);
                        }
                    }
                }

                scope.Complete();

                if (nextTasks.Count > 0)
                {
                    List<string> nextStepName = new List<string>();
                    foreach (var nstep in nextTasks)
                    {
                        nextStepName.Add(nstep.StepName);
                    }
                    result.DebugMessages += string.Format("已发送到：{0}", nextStepName.Distinct().ToList().ToString(","));
                    result.IsSuccess = true;
                    result.Messages += string.Format("已发送到：{0}", nextStepName.Distinct().ToList().ToString(","));
                    result.NextTasks = nextTasks;
                }
                else
                {
                    result.DebugMessages += string.Format("已发送,等待其它步骤处理");
                    result.IsSuccess = true;
                    result.Messages += string.Format("已发送,等待其它步骤处理");
                    result.NextTasks = nextTasks;
                }
            }
        }

        private void executeSave(Next.WorkFlow.Entity.WorkFlowExecute.Execute executeModel)
        {
            //如果是第一步提交并且没有实例则先创建实例
            WorkFlowTask currentTask = null;
            bool isFirst = executeModel.StepID == wfInstalled.FirstStepID && executeModel.TaskID == Guid.Empty.ToString() && executeModel.GroupID == Guid.Empty.ToString();
            if (isFirst)
            {
                currentTask = createFirstTask(executeModel);
            }
            else
            {
                currentTask = FindByID(executeModel.TaskID);
            }
            if (currentTask == null)
            {
                result.DebugMessages = "未能创建或找到当前任务";
                result.IsSuccess = false;
                result.Messages = "未能创建或找到当前任务";
                return;
            }
            else if (currentTask.Status.In(2, 3, 4))
            {
                result.DebugMessages = "当前任务已处理";
                result.IsSuccess = false;
                result.Messages = "当前任务已处理";
                return;
            }
            else
            {
                currentTask.InstanceID = executeModel.InstanceID;
                nextTasks.Add(currentTask);
                if (isFirst)
                {
                    currentTask.Title = executeModel.Title.IsNullOrEmpty() ? "未命名任务" : executeModel.Title;
                    Update(currentTask);
                }
                else
                {
                    if (!executeModel.Title.IsNullOrEmpty())
                    {
                        currentTask.Title = executeModel.Title;
                        Update(currentTask);
                    }
                }
            }

            result.DebugMessages = "保存成功";
            result.IsSuccess = true;
            result.Messages = "保存成功";
        }

        private void executeBack(Next.WorkFlow.Entity.WorkFlowExecute.Execute executeModel)
        {
            var currentTask = FindByID(executeModel.TaskID);
            if (currentTask == null)
            {
                result.DebugMessages = "未能找到当前任务";
                result.IsSuccess = false;
                result.Messages = "未能找到当前任务";
                return;
            }
            else if (currentTask.Status.In(2, 3, 4))
            {
                result.DebugMessages = "当前任务已处理";
                result.IsSuccess = false;
                result.Messages = "当前任务已处理";
                return;
            }

            var currentSteps = wfInstalled.Steps.Where(p => p.ID == currentTask.StepID);
            var currentStep = currentSteps.Count() > 0 ? currentSteps.First() : null;

            if (currentStep == null)
            {
                result.DebugMessages = "未能找到当前步骤";
                result.IsSuccess = false;
                result.Messages = "未能找到当前步骤";
                return;
            }
            if (currentTask.StepID == wfInstalled.FirstStepID)
            {
                result.DebugMessages = "当前任务是流程第一步,不能退回";
                result.IsSuccess = false;
                result.Messages = "当前任务是流程第一步,不能退回";
                return;
            }
            if (executeModel.Steps.Count == 0)
            {
                result.DebugMessages = "没有选择要退回的步骤";
                result.IsSuccess = false;
                result.Messages = "没有选择要退回的步骤";
                return;
            }
            using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope())
            {
                List<WorkFlowTask> backTasks = new List<WorkFlowTask>();
                int status = 0;
                switch (currentStep.Behavior.BackModel)
                {
                    case 0://不能退回
                        result.DebugMessages = "当前步骤设置为不能退回";
                        result.IsSuccess = false;
                        result.Messages = "当前步骤设置为不能退回";
                        return;
                    #region 根据策略退回
                    case 1:
                        switch (currentStep.Behavior.HanlderModel)
                        {
                            case 0://所有人必须同意,如果一人不同意则全部退回
                                var taskList1 = GetTaskListByStepID(currentTask.ID, currentTask.StepID);
                                foreach (var task in taskList1)
                                {
                                    if (task.ID != currentTask.ID)
                                    {
                                        if (task.Status.In(0, 1))
                                        {
                                            Completed(task.ID, "", false, 5);
                                        }
                                    }
                                    else
                                    {
                                        Completed(task.ID, executeModel.Comment, executeModel.IsSign, 3);
                                    }
                                }
                                break;
                            case 1://一人同意即可
                                var taskList = GetTaskListByStepID(currentTask.ID, currentTask.StepID);
                                if (taskList.Count > 1)
                                {
                                    var noCompleted = taskList.Where(p => p.Status != 3);
                                    if (noCompleted.Count() - 1 > 0)
                                    {
                                        status = -1;
                                    }
                                }
                                Completed(currentTask.ID, executeModel.Comment, executeModel.IsSign, 3);
                                break;
                            case 2://依据人数比例
                                var taskList2 = GetTaskListByStepID(currentTask.ID, currentTask.StepID);
                                if (taskList2.Count > 1)
                                {
                                    decimal percentage = currentStep.Behavior.Percentage <= 0 ? 100 : currentStep.Behavior.Percentage;//比例
                                    if ((((decimal)(taskList2.Where(p => p.Status == 3).Count() + 1) / (decimal)taskList2.Count) * 100).Round() < percentage)
                                    {
                                        status = -1;
                                    }
                                    else
                                    {
                                        foreach (var task in taskList2)
                                        {
                                            if (task.ID != currentTask.ID && task.Status.In(0, 1))
                                            {
                                                Completed(task.ID, "", false, 5);
                                            }
                                        }
                                    }
                                }
                                Completed(currentTask.ID, executeModel.Comment, executeModel.IsSign, 3);
                                break;
                            case 3://独立处理
                                Completed(currentTask.ID, executeModel.Comment, executeModel.IsSign, 3);
                                break;
                        }
                        backTasks.Add(currentTask);
                        break;
                    #endregion
                }

                if (status == -1)
                {
                    result.DebugMessages += "已退回,等待他人处理";
                    result.IsSuccess = true;
                    result.Messages += "已退回,等待他人处理!";
                    result.NextTasks = nextTasks;
                    scope.Complete();
                    return;
                }

                foreach (var backTask in backTasks)
                {
                    if (backTask.Status.In(2, 3))//已完成的任务不能退回
                    {
                        continue;
                    }
                    if (backTask.ID == currentTask.ID)
                    {
                        Completed(backTask.ID, executeModel.Comment, executeModel.IsSign, 3);
                    }
                    else
                    {
                        Completed(backTask.ID, "", false, 3, "他人已退回");
                    }
                }

                List<WorkFlowTask> tasks = new List<WorkFlowTask>();
                foreach (var step in executeModel.Steps)
                {
                    tasks.AddRange(GetTaskList(executeModel.FlowID, step.Key, executeModel.GroupID));
                }

                #region 处理会签形式的退回
                //当前步骤是否是会签步骤
                var countersignatureStep = bWorkFlow.GetNextSteps(executeModel.FlowID, executeModel.StepID).Find(p => p.Behavior.Countersignature != 0);
                bool IsCountersignature = countersignatureStep != null;
                bool isBack = true;
                if (IsCountersignature)
                {
                    var steps = bWorkFlow.GetPrevSteps(executeModel.FlowID, countersignatureStep.ID);
                    switch (countersignatureStep.Behavior.Countersignature)
                    {
                        case 1://所有步骤处理，如果一个步骤退回则退回
                            isBack = false;
                            foreach (var step in steps)
                            {
                                if (IsBack(step, executeModel.FlowID, currentTask.GroupID, currentTask.PrevID))
                                {
                                    isBack = true;
                                    break;
                                }
                            }
                            break;
                        case 2://一个步骤退回,如果有一个步骤同意，则不退回
                            isBack = true;
                            foreach (var step in steps)
                            {
                                if (!IsBack(step, executeModel.FlowID, currentTask.GroupID, currentTask.PrevID))
                                {
                                    isBack = false;
                                    break;
                                }
                            }
                            break;
                        case 3://依据比例退回
                            int backCount = 0;
                            foreach (var step in steps)
                            {
                                if (IsBack(step, executeModel.FlowID, currentTask.GroupID, currentTask.PrevID))
                                {
                                    backCount++;
                                }
                            }
                            isBack = (((decimal)backCount / (decimal)steps.Count) * 100).Round() >= (countersignatureStep.Behavior.CountersignaturePercentage <= 0 ? 100 : countersignatureStep.Behavior.CountersignaturePercentage);
                            break;
                    }

                    if (isBack)
                    {
                        var tjTasks = GetTaskList(currentTask.ID, false);
                        foreach (var tjTask in tjTasks)
                        {
                            if (tjTask.ID == currentTask.ID || tjTask.Status.In(2, 3, 4, 5))
                            {
                                continue;
                            }
                            Completed(tjTask.ID, "", false, 5);
                        }
                    }
                }
                #endregion

                //如果退回步骤是子流程步骤，则要作废子流程实例
                if (currentStep.Type == "subflow" && currentStep.SubFlowID.IsGuid() && currentTask.SubFlowGroupID.IsGuid())
                {
                    DeleteInstance(currentStep.SubFlowID.ToGuid(), currentTask.SubFlowGroupID, true);
                }

                if (isBack)
                {
                    foreach (var task in tasks)//.Distinct(this))
                    {
                        if (task != null)
                        {
                            WorkFlowTask newTask = task;
                            newTask.ID = Guid.NewGuid().ToString();
                            newTask.PrevID = currentTask.ID;
                            newTask.Note = "退回任务";
                            newTask.ReceiveTime = Next.WorkFlow.Utility.DateTimeNew.Now;
                            newTask.SenderID = currentTask.ReceiveID;
                            newTask.SenderName = currentTask.ReceiveName;
                            newTask.SenderTime = Next.WorkFlow.Utility.DateTimeNew.Now;
                            newTask.Sort = currentTask.Sort + 1;
                            newTask.Status = 0;
                            newTask.Comment = "";
                            newTask.OpenTime = null;
                            //newTask.PrevStepID = currentTask.StepID;
                            if (currentStep.WorkTime > 0)
                            {
                                newTask.CompletedTime = Next.WorkFlow.Utility.DateTimeNew.Now.AddHours((double)currentStep.WorkTime);
                            }
                            else
                            {
                                newTask.CompletedTime = null;
                            }
                            newTask.CompletedTime1 = null;
                            Insert(newTask);
                            nextTasks.Add(newTask);
                        }
                    }
                }

                scope.Complete();
            }

            if (nextTasks.Count > 0)
            {
                List<string> nextStepName = new List<string>();
                foreach (var nstep in nextTasks)
                {
                    nextStepName.Add(nstep.StepName);
                }
                string msg = string.Format("已退回到：{0}", nextStepName.Distinct().ToList().ToString(","));
                result.DebugMessages += msg;
                result.IsSuccess = true;
                result.Messages += msg;
                result.NextTasks = nextTasks;
            }
            else
            {
                result.DebugMessages += "已退回,等待其它步骤处理";
                result.IsSuccess = true;
                result.Messages += "已退回,等待其它步骤处理";
                result.NextTasks = nextTasks;
            }
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="executeModel"></param>
        /// <param name="isCompleteTask">是否需要调用Completed方法完成当前任务</param>
        private void executeComplete(Next.WorkFlow.Entity.WorkFlowExecute.Execute executeModel, bool isCompleteTask = true)
        {
            if (executeModel.TaskID == string.Empty || executeModel.FlowID == string.Empty)
            {
                result.DebugMessages = "完成流程参数错误";
                result.IsSuccess = false;
                result.Messages = "完成流程参数错误";
                return;
            }
            var task = FindByID(executeModel.TaskID);
            if (task == null)
            {
                result.DebugMessages = "未找到当前任务";
                result.IsSuccess = false;
                result.Messages = "未找到当前任务";
                return;
            }
            else if (isCompleteTask && task.Status.In(2, 3, 4))
            {
                result.DebugMessages = "当前任务已处理";
                result.IsSuccess = false;
                result.Messages = "当前任务已处理";
                return;
            }
            if (isCompleteTask)
            {
                Completed(task.ID, executeModel.Comment, executeModel.IsSign);
            }

            #region 更新业务表标识字段的值为1
            if (wfInstalled.TitleField != null && wfInstalled.TitleField.LinkID != string.Empty && !wfInstalled.TitleField.Table.IsNullOrEmpty()
                && !wfInstalled.TitleField.Field.IsNullOrEmpty() && wfInstalled.DataBases.Count() > 0)
            {
                var firstDB = wfInstalled.DataBases.First();
                new DBConnectionBLL().UpdateFieldValue(
                    wfInstalled.TitleField.LinkID,
                    wfInstalled.TitleField.Table,
                    wfInstalled.TitleField.Field,
                    "1",
                   string.Format("{0}='{1}'", firstDB.PrimaryKey, task.InstanceID));
            }
            #endregion

            #region 执行子流程完成后事件
            var parentTasks = GetBySubFlowGroupID(task.GroupID);
            if (parentTasks.Count > 0)
            {
                var parentTask = parentTasks.First();
                var flowRunModel = bWorkFlow.GetWorkFlowRunModel(parentTask.FlowID);
                if (flowRunModel != null)
                {
                    var steps = flowRunModel.Steps.Where(p => p.ID == parentTask.StepID);
                    if (steps.Count() > 0 && !steps.First().Event.SubFlowCompletedBefore.IsNullOrEmpty())
                    {
                        ExecuteFlowCustomEvent(steps.First().Event.SubFlowCompletedBefore.Trim(), new Next.WorkFlow.Entity.WorkFlowCustomEventParams()
                        {
                            FlowID = parentTask.FlowID,
                            GroupID = parentTask.GroupID,
                            InstanceID = parentTask.InstanceID,
                            StepID = parentTask.StepID,
                            TaskID = parentTask.ID
                        });
                    }
                }
            }
            #endregion

            result.DebugMessages += "已完成";
            result.IsSuccess = true;
            result.Messages += "已完成";
        }

        private void executeRedirect(Next.WorkFlow.Entity.WorkFlowExecute.Execute executeModel)
        {
            WorkFlowTask currentTask = null;
            bool isFirst = executeModel.StepID == wfInstalled.FirstStepID && executeModel.TaskID == string.Empty && executeModel.GroupID == string.Empty;
            if (isFirst)
            {
                currentTask = createFirstTask(executeModel);
            }
            else
            {
                currentTask = FindByID(executeModel.TaskID);
            }
            if (currentTask == null)
            {
                result.DebugMessages = "未能创建或找到当前任务";
                result.IsSuccess = false;
                result.Messages = "未能创建或找到当前任务";
                return;
            }
            else if (currentTask.Status.In(2, 3, 4))
            {
                result.DebugMessages = "当前任务已处理";
                result.IsSuccess = false;
                result.Messages = "当前任务已处理";
                return;
            }
            else if (currentTask.Status == 5)
            {
                result.DebugMessages = "当前任务正在等待他人处理";
                result.IsSuccess = false;
                result.Messages = "当前任务正在等待他人处理";
                return;
            }
            if (executeModel.Steps.First().Value.Count == 0)
            {
                result.DebugMessages = "未设置转交人员";
                result.IsSuccess = false;
                result.Messages = "未设置转交人员";
                return;
            }
            else if (executeModel.Steps.First().Value.Count > 1)
            {
                result.DebugMessages = "当前任务只能转交给一个人员";
                result.IsSuccess = false;
                result.Messages = "当前任务只能转交给一个人员";
                return;
            }
            string receiveName = currentTask.ReceiveName;
            currentTask.ReceiveID = executeModel.Steps.First().Value.First().ID;
            currentTask.ReceiveName = executeModel.Steps.First().Value.First().Name;
            currentTask.OpenTime = null;
            currentTask.Status = 0;
            currentTask.Note = string.Format("该任务由{0}转交", receiveName);
            Update(currentTask);
            nextTasks.Add(currentTask);
            result.DebugMessages = "转交成功";
            result.IsSuccess = true;
            result.Messages = string.Concat("已转交给：", currentTask.ReceiveName);
            return;
        }

        /// <summary>
        /// 判断一个步骤是否通过
        /// </summary>
        /// <param name="step"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        private bool IsPassing(Next.WorkFlow.Entity.WorkFlowInstalledSub.Step step, string flowID, string groupID, string taskID)
        {
            var tasks = GetTaskList(flowID, step.ID, groupID).FindAll(p => p.PrevID == taskID);
            if (tasks.Count == 0)
            {
                return false;
            }
            bool isPassing = true;
            switch (step.Behavior.HanlderModel)
            {
                case 0://所有人必须处理
                case 3://独立处理
                    isPassing = tasks.Where(p => p.Status != 2).Count() == 0;
                    break;
                case 1://一人同意即可
                    isPassing = tasks.Where(p => p.Status == 2).Count() > 0;
                    break;
                case 2://依据人数比例
                    isPassing = (((decimal)(tasks.Where(p => p.Status == 2).Count() + 1) / (decimal)tasks.Count) * 100).Round() >= (step.Behavior.Percentage <= 0 ? 100 : step.Behavior.Percentage);
                    break;
            }
            return isPassing;
        }

        /// <summary>
        /// 判断一个步骤是否退回
        /// </summary>
        /// <param name="step"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        private bool IsBack(Next.WorkFlow.Entity.WorkFlowInstalledSub.Step step, string flowID, string groupID, string taskID)
        {
            var tasks = GetTaskList(flowID, step.ID, groupID).FindAll(p => p.PrevID == taskID);
            if (tasks.Count == 0)
            {
                return false;
            }
            bool isBack = true;
            switch (step.Behavior.HanlderModel)
            {
                case 0://所有人必须处理
                case 3://独立处理
                    isBack = tasks.Where(p => p.Status.In(3, 5)).Count() > 0;
                    break;
                case 1://一人同意即可
                    isBack = tasks.Where(p => p.Status.In(2, 4)).Count() == 0;
                    break;
                case 2://依据人数比例
                    isBack = (((decimal)(tasks.Where(p => p.Status.In(3, 5)).Count() + 1) / (decimal)tasks.Count) * 100).Round() >= (step.Behavior.Percentage <= 0 ? 100 : step.Behavior.Percentage);
                    break;
            }
            return isBack;
        }

        /// <summary>
        /// 创建第一个任务
        /// </summary>
        /// <param name="executeModel"></param>
        /// <param name="isSubFlow">是否是创建子流程任务</param>
        /// <returns></returns>
        private WorkFlowTask createFirstTask(Next.WorkFlow.Entity.WorkFlowExecute.Execute executeModel, bool isSubFlow = false)
        {
            if (wfInstalled == null || isSubFlow)
            {
                wfInstalled = bWorkFlow.GetWorkFlowRunModel(executeModel.FlowID);
            }

            var nextSteps = wfInstalled.Steps.Where(p => p.ID == wfInstalled.FirstStepID);
            if (nextSteps.Count() == 0)
            {
                return null;
            }
            WorkFlowTask task = new WorkFlowTask();
            if (nextSteps.First().WorkTime > 0)
            {
                task.CompletedTime = Next.WorkFlow.Utility.DateTimeNew.Now.AddHours((double)nextSteps.First().WorkTime);
            }
            task.FlowID = executeModel.FlowID;
            task.GroupID = Guid.NewGuid().ToString();
            task.ID = Guid.NewGuid().ToString();
            task.Type = 0;
            task.InstanceID = executeModel.InstanceID;
            if (!executeModel.Note.IsNullOrEmpty())
            {
                task.Note = executeModel.Note;
            }
            task.PrevID = string.Empty;
            task.PrevStepID = string.Empty;
            task.ReceiveID = executeModel.Sender.ID;
            task.ReceiveName = executeModel.Sender.Name;
            task.ReceiveTime = Next.WorkFlow.Utility.DateTimeNew.Now;
            task.SenderID = executeModel.Sender.ID;
            task.SenderName = executeModel.Sender.Name;
            task.SenderTime = task.ReceiveTime;
            task.Status = 0;
            task.StepID = wfInstalled.FirstStepID;
            task.StepName = nextSteps.First().Name;
            task.Sort = 1;
            task.Title = executeModel.Title.IsNullOrEmpty() ? "未命名任务" : executeModel.Title;
            Insert(task);
            if (isSubFlow)
            {
                wfInstalled = null;
            }
            return task;
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
            return workFlowTaskDAL.GetTasks(userID, out pager, query, title, flowid, Next.Admin.BLL.UserBLL.RemovePrefix(sender), date1, date2, type);
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
        /// <param name="isCompleted">是否完成 0:全部 1:未完成 2:已完成</param>
        /// <returns></returns>
        public List<WorkFlowTask> GetInstances(string[] flowID, string[] senderID, string[] receiveID, out string pager, string query = "", string title = "", string flowid = "", string date1 = "", string date2 = "", int status = 0)
        {
            return workFlowTaskDAL.GetInstances(flowID, senderID, receiveID, out pager, query, title, flowid, date1, date2, status);
        }

        /// <summary>
        /// 执行自定义方法
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public object ExecuteFlowCustomEvent(string eventName, object eventParams, string dllName = "")
        {
            if (dllName.IsNullOrEmpty())
            {
                dllName = eventName.Substring(0, eventName.IndexOf('.'));
            }
            System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(dllName);
            string typeName = System.IO.Path.GetFileNameWithoutExtension(eventName);
            string methodName = eventName.Substring(typeName.Length + 1);
            Type type = assembly.GetType(typeName, true);

            object obj = System.Activator.CreateInstance(type, false);
            var method = type.GetMethod(methodName);

            if (method != null)
            {
                return method.Invoke(obj, new object[] { eventParams });
            }
            else
            {
                throw new MissingMethodException(typeName, methodName);
            }
        }

        /// <summary>
        /// 删除流程实例
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public int DeleteInstance(string flowID, string groupID, bool hasInstanceData = false)
        {
            if (hasInstanceData)
            {
                var tasks = GetTaskList(flowID, groupID);
                if (tasks.Count > 0 && !tasks.First().InstanceID.IsNullOrEmpty())
                {
                    var wfRunModel = bWorkFlow.GetWorkFlowRunModel(flowID);
                    if (wfRunModel != null && wfRunModel.DataBases.Count() > 0)
                    {
                        var dataBase = wfRunModel.DataBases.First();
                        new DBConnectionBLL().DeleteData(dataBase.LinkID, dataBase.Table, dataBase.PrimaryKey, tasks.First().InstanceID);
                    }
                }
            }
            return workFlowTaskDAL.Delete(flowID, groupID);
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
            return workFlowTaskDAL.Completed(taskID, comment, isSign, status, note);
        }

        /// <summary>
        /// 得到一个流程实例一个步骤的任务
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public List<WorkFlowTask> GetTaskList(string flowID, string stepID, string groupID)
        {
            return workFlowTaskDAL.GetTaskList(flowID, stepID, groupID);
        }

        /// <summary>
        /// 得到一个实例的任务
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public List<WorkFlowTask> GetTaskList(string flowID, string groupID)
        {
            return workFlowTaskDAL.GetTaskList(flowID, groupID);
        }

        /// <summary>
        /// 得到和一个任务同级的任务
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <param name="isStepID">是否区分步骤ID，多步骤会签区分的是上一步骤ID</param>
        /// <returns></returns>
        public List<WorkFlowTask> GetTaskList(string taskID, bool isStepID = true)
        {
            return workFlowTaskDAL.GetTaskList(taskID, isStepID);
        }

        /// <summary>
        /// 得到和一个任务同级的任务(同一步骤内)
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <param name="stepID">步骤ID</param>
        /// <returns></returns>
        public List<WorkFlowTask> GetTaskListByStepID(string taskID, string stepID)
        {
            return workFlowTaskDAL.GetTaskList(taskID).FindAll(p => p.StepID == stepID);
        }

        /// <summary>
        /// 得到一个任务的前一任务
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public List<WorkFlowTask> GetPrevTaskList(string taskID)
        {
            return workFlowTaskDAL.GetPrevTaskList(taskID);
        }

        /// <summary>
        /// 得到一个任务的后续任务
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public List<WorkFlowTask> GetNextTaskList(string taskID)
        {
            return workFlowTaskDAL.GetNextTaskList(taskID);
        }

        /// <summary>
        /// 得到一个任务可以退回的步骤
        /// </summary>
        /// <param name="taskID">当前任务ID</param>
        /// <param name="backType">退回类型</param>
        /// <param name="stepID"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetBackSteps(string taskID, int backType, string stepID, Next.WorkFlow.Entity.WorkFlowInstalled wfInstalled)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            var steps = wfInstalled.Steps.Where(p => p.ID == stepID);
            if (steps.Count() == 0)
            {
                return dict;
            }
            var step = steps.First();
            switch (backType)
            {
                case 0://退回前一步
                    var task = FindByID(taskID);
                    if (task != null)
                    {
                        if (step.Behavior.Countersignature != 0)//如果是会签步骤，则要退回到前面所有步骤
                        {
                            var backSteps = bWorkFlow.GetPrevSteps(task.FlowID, step.ID);
                            foreach (var backStep in backSteps)
                            {
                                dict.Add(backStep.ID, backStep.Name);
                            }
                        }
                        else
                        {
                            dict.Add(task.PrevStepID, bWorkFlow.GetStepName(task.PrevStepID, wfInstalled));
                        }
                    }
                    break;
                case 1://退回第一步
                    dict.Add(wfInstalled.FirstStepID, bWorkFlow.GetStepName(wfInstalled.FirstStepID, wfInstalled));
                    break;
                case 2://退回某一步
                    if (step.Behavior.BackType == 2 && step.Behavior.BackStepID != string.Empty)
                    {
                        dict.Add(step.Behavior.BackStepID, bWorkFlow.GetStepName(step.Behavior.BackStepID, wfInstalled));
                    }
                    else
                    {
                        var task0 = FindByID(taskID);
                        if (task0 != null)
                        {
                            var taskList = GetTaskList(task0.FlowID, task0.GroupID).Where(p => p.Status.In(2, 3, 4)).OrderBy(p => p.Sort);
                            foreach (var task1 in taskList)
                            {
                                if (!dict.Keys.Contains(task1.StepID) && task1.StepID != stepID)
                                {
                                    dict.Add(task1.StepID, bWorkFlow.GetStepName(task1.StepID, wfInstalled));
                                }
                            }
                        }
                    }
                    break;
            }
            return dict;
        }

        /// <summary>
        /// 更新一个任务后续任务状态
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="comment"></param>
        /// <param name="isSign"></param>
        /// <returns></returns>
        public int UpdateNextTaskStatus(string taskID, int status)
        {
            int i = 0;
            var taskList = GetTaskList(taskID);

            foreach (var task in taskList)
            {
                i += workFlowTaskDAL.UpdateNextTaskStatus(task.ID, status);
            }

            return i;
        }

        /// <summary>
        /// 查询一个流程是否有任务数据
        /// </summary>
        /// <param name="flowID"></param>
        /// <returns></returns>
        public bool HasTasks(string flowID)
        {
            return workFlowTaskDAL.HasTasks(flowID);
        }

        /// <summary>
        /// 查询一个用户在一个步骤是否有未完成任务
        /// </summary>
        /// <param name="flowID"></param>
        /// <returns></returns>
        public bool HasNoCompletedTasks(string flowID, string stepID, string groupID, string userID)
        {
            return workFlowTaskDAL.HasNoCompletedTasks(flowID, stepID, groupID, userID);
        }

        /// <summary>
        /// 得到状态显示标题
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public string GetStatusTitle(int status)
        {
            string title = string.Empty;
            switch (status)
            {
                case 0:
                    title = "待处理";
                    break;
                case 1:
                    title = "已打开";
                    break;
                case 2:
                    title = "已完成";
                    break;
                case 3:
                    title = "已退回";
                    break;
                case 4:
                    title = "他人已处理";
                    break;
                case 5:
                    title = "他人已退回";
                    break;
                default:
                    title = "其它";
                    break;
            }

            return title;
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
            return workFlowTaskDAL.GetUserTaskList(flowID, stepID, groupID, userID);
        }

        /// <summary>
        /// 判断一个任务是否可以收回
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public bool HasWithdraw(string taskID)
        {
            var taskList = GetNextTaskList(taskID);
            if (taskList.Count == 0) return false;
            foreach (var task in taskList)
            {
                if (task.Status.In(1, 2, 3, 4))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 收回任务
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public bool WithdrawTask(string taskID)
        {
            var taskList1 = GetTaskList(taskID);
            using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope())
            {
                foreach (var task in taskList1)
                {
                    var taskList2 = GetNextTaskList(task.ID);
                    foreach (var task2 in taskList2)
                    {
                        if (task2.Status == 0 || task2.Status == 1 || task2.Status == 5)
                        {
                            Delete(task2.ID);
                        }
                    }

                    if (task.ID == taskID || task.Status == 4)
                    {
                        Completed(task.ID, "", false, 1, "");
                    }
                }
                scope.Complete();
                return true;
            }
        }

        /// <summary>
        /// 指派任务
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <param name="user">要指派的人员</param>
        /// <returns></returns>
        public string DesignateTask(string taskID, Next.Admin.Entity.User user)
        {
            var task = FindByID(taskID);
            if (task == null)
            {
                return "未找到任务";
            }
            else if (task.Status.In(2, 3, 4))
            {
                return "该任务已处理";
            }
            string receiveName = task.ReceiveName;
            task.ReceiveID = user.ID;
            task.ReceiveName = user.Name;
            task.OpenTime = null;
            task.Status = 0;
            task.Note = string.Format("该任务由{0}指派", receiveName);
            Update(task);

            return "指派成功";
        }

        /// <summary>
        /// 管理员强制退回任务
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public string BackTask(string taskID)
        {
            var task = FindByID(taskID);
            if (task == null)
            {
                return "未找到任务";
            }
            else if (task.Status.In(2, 3, 4))
            {
                return "该任务已处理";
            }
            if (wfInstalled == null)
            {
                wfInstalled = bWorkFlow.GetWorkFlowRunModel(task.FlowID);
            }
            Next.WorkFlow.Entity.WorkFlowExecute.Execute executeModel = new Next.WorkFlow.Entity.WorkFlowExecute.Execute();
            executeModel.ExecuteType = Next.WorkFlow.Entity.WorkFlowExecute.EnumType.ExecuteType.Back;
            executeModel.FlowID = task.FlowID;
            executeModel.GroupID = task.GroupID;
            executeModel.InstanceID = task.InstanceID;
            executeModel.Note = "管理员退回";
            executeModel.Sender = new UserBLL().FindByID(task.ReceiveID);
            executeModel.StepID = task.StepID;
            executeModel.TaskID = task.ID;
            executeModel.Title = task.Title;
            var steps = wfInstalled.Steps.Where(p => p.ID == task.StepID);
            if (steps.Count() == 0)
            {
                return "未找到步骤";
            }
            else if (steps.First().Behavior.BackType == 2 && steps.First().Behavior.BackStepID == string.Empty)
            {
                return "未设置退回步骤";
            }
            Dictionary<string, List<Next.Admin.Entity.User>> execSteps = new Dictionary<string, List<Next.Admin.Entity.User>>();
            var backsteps = GetBackSteps(taskID, steps.First().Behavior.BackType, task.StepID, wfInstalled);
            foreach (var back in backsteps)
            {
                execSteps.Add(back.Key, new List<Next.Admin.Entity.User>());
            }
            executeModel.Steps = execSteps;
            var result = Execute(executeModel);
            return result.Messages;
        }

        /// <summary>
        /// 排序流程任务列表
        /// </summary>
        /// <param name="tasks"></param>
        public List<WorkFlowTask> Sort(List<WorkFlowTask> tasks)
        {
            return tasks.OrderBy(p => p.Sort).ToList();
        }

        /// <summary>
        /// 得到一个任务的状态
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public int GetTaskStatus(string taskID)
        {
            return workFlowTaskDAL.GetTaskStatus(taskID);
        }

        /// <summary>
        /// 判断一个任务是否可以处理
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public bool IsExecute(string taskID)
        {
            return GetTaskStatus(taskID) <= 1;
        }

        /// <summary>
        /// 判断sql流转条件是否满足
        /// </summary>
        /// <param name="linkID"></param>
        /// <param name="table"></param>
        /// <param name="tablepk"></param>
        /// <param name="instabceID">实例ID</param>
        /// <param name="where"></param>
        /// <returns></returns>
        public bool TestLineSql(string linkID, string table, string tablepk, string instabceID, string where)
        {
            if (instabceID.IsNullOrEmpty())
            {
                return false;
            }
            DBConnectionBLL dbconn = new DBConnectionBLL();
            var conn = dbconn.FindByID(linkID);
            if (conn == null)
            {
                return false;
            }
            string sql = "SELECT * FROM " + table + " WHERE " + tablepk + "='" + instabceID + "' AND (" + where + ")".ReplaceSelectSql();
            if (!dbconn.TestSql(conn, sql))
            {
                return false;
            }
            System.Data.DataTable dt = dbconn.GetDataTable(conn, sql);
            return dt.Rows.Count > 0;
        }

        /// <summary>
        /// 判断实例是否已完成
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool GetInstanceIsCompleted(string flowID, string groupID)
        {
            var tasks = GetTaskList(flowID, groupID);
            return tasks.Find(p => p.Status.In(0, 1)) == null;
        }

        /// <summary>
        /// 根据SubFlowID得到一个任务
        /// </summary>
        /// <param name="subflowGroupID"></param>
        /// <returns></returns>
        public List<WorkFlowTask> GetBySubFlowGroupID(string subflowGroupID)
        {
            return workFlowTaskDAL.GetBySubFlowGroupID(subflowGroupID);
        }
    }
}