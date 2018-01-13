using System;
using System.Collections.Generic;
using System.Text;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.DALMySql;
using Next.WorkFlow.IDAL;
using Next.WorkFlow.Utility;
using Next.Admin.BLL;
using Next.Admin.Entity;
using System.Web;
using System.Linq;
using System.Drawing;
using MySql.Data.MySqlClient;
using System.Data;

namespace Next.WorkFlow.BLL
{
	public class WorkFlowInfoBLL : BaseBLL<WorkFlowInfo>
	{
		private IWorkFlowInfoDAL workFlowInfoDAL;
		public WorkFlowInfoBLL(): base()
		{
			base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
			baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//如果需要记录操作日志，则实现这个事件
			this.workFlowInfoDAL = (IWorkFlowInfoDAL)base.baseDal;
		}

        /// <summary>
        /// 查询所有类型
        /// </summary>
        public List<string> GetAllTypes()
        {
            return workFlowInfoDAL.GetAllTypes();
        }

        /// <summary>
        /// 得到所有类型的下拉选择项
        /// </summary>
        /// <returns></returns>
        public string GetAllTypesOptions(string value = "")
        {
            var types = GetAllTypes();
            StringBuilder options = new StringBuilder();
            foreach (var type in types)
            {
                options.AppendFormat("<option value=\"{0}\" {1}>{0}</option>", type, type == value ? "selected=\"selected\"" : "");
            }
            return options.ToString();
        }
        /// <summary>
        /// 得到流程状态显示
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public string GetStatusTitle(int status)
        {
            string title = string.Empty;
            switch (status)
            {
                case 1:
                    title = "设计中";
                    break;
                case 2:
                    title = "已安装";
                    break;
                case 3:
                    title = "已卸载";
                    break;
                case 4:
                    title = "已删除";
                    break;
                case 5:
                    title = "等待他人处理";
                    break;
            }
            return title;
        }

        /// <summary>
        /// 保存一个流程
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns>返回1 表示成功 其它为具体错误信息</returns>
        public string SaveFlow(string jsonString)
        {
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);
            string id = jsonData["id"].ToString();
            string name = jsonData["name"].ToString();
            string type = jsonData["type"].ToString();
            string flowID;
            if (!id.IsGuid(out flowID))
            {
                return "请先新建或打开流程!";
            }
            else if (name.IsNullOrEmpty())
            {
                return "流程名称不能为空!";
            }
            else
            {
                Next.WorkFlow.BLL.WorkFlowInfoBLL bwf = new Next.WorkFlow.BLL.WorkFlowInfoBLL();
                WorkFlowInfo wf = bwf.FindByID(flowID);
                bool isAdd = false;
                if (wf == null)
                {
                    wf = new WorkFlowInfo();
                    isAdd = true;
                    wf.ID = flowID;
                    wf.CreateDate = Next.WorkFlow.Utility.DateTimeNew.Now;
                    wf.CreateUserID = new Next.Admin.BLL.UserBLL().CurrentUserID;
                    wf.Status = 1;
                }
                wf.DesignJSON = jsonString;
                wf.InstanceManager = jsonData["instanceManager"].ToString();
                wf.Manager = jsonData["manager"].ToString();
                wf.Name = name.Trim();
                wf.Type = type.IsGuid() ? type.ToGuid() : new Next.Admin.BLL.DictTypeBLL().GetIDByCode("FlowTypes");
                try
                {
                    if (isAdd)
                    {
                        bwf.Insert(wf);
                    }
                    else
                    {
                        bwf.Update(wf);
                    }
                    return "1";
                }
                catch (Exception err)
                {
                    return err.Message;
                }
            }
        }

        /// <summary>
        /// 安装一个流程
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="isMvc">是否是mvc程序，用于区分应用程序库连接</param>
        /// <returns>返回1 表示成功 其它为具体错误信息</returns>
        public string InstallFlow(string jsonString, bool isMvc = true)
        {
            string saveInfo = SaveFlow(jsonString);
            if ("1" != saveInfo)
            {
                return saveInfo;
            }
            string errMsg;

            Next.WorkFlow.Entity.WorkFlowInstalled wfInstalled = GetWorkFlowRunModel(jsonString, out errMsg);
            if (wfInstalled == null)
            {
                return errMsg;
            }
            else
            {
                WorkFlowInfo wf = workFlowInfoDAL.FindByID(wfInstalled.ID);
                if (wf == null)
                {
                    return "流程实体为空";
                }
                else
                {
                    using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope())
                    {
                        wf.InstallDate = wfInstalled.InstallTime;
                        wf.InstallUserID = wfInstalled.InstallUser.ToGuid();
                        wf.RunJSON = wfInstalled.RunJSON;
                        wf.Status = 2;
                        workFlowInfoDAL.Update(wf);

                        wfInstalled.Status = 2;

                        #region 添加到应用程序库
                        Next.WorkFlow.BLL.AppLibraryBLL bappLibrary = new AppLibraryBLL();
                        AppLibrary app = bappLibrary.GetByCode(wfInstalled.ID.ToString());
                        bool isAdd = false;
                        if (app == null)
                        {
                            isAdd = true;
                            app = new AppLibrary();
                            app.ID = Guid.NewGuid().ToString();
                        }
                        app.Address = isMvc ? "WorkFlow/WorkFlowRun/Index" : "Platform/WorkFlowRun/Default.aspx";
                        app.Code = wfInstalled.ID.ToString();
                        app.Note = "流程应用";
                        app.OpenMode = 0;
                        app.Params = "flowid=" + wfInstalled.ID.ToString();
                        app.Title = wfInstalled.Name;
                        app.Type = wfInstalled.Type.IsGuid() ? wfInstalled.Type.ToGuid() : new Next.Admin.BLL.DictTypeBLL().GetIDByCode("FlowTypes");
                        if (isAdd)
                        {
                            bappLibrary.Insert(app);
                        }
                        else
                        {
                            bappLibrary.Update(app);
                        }
                        //bappLibrary.ClearCache();
                        //new RoadFlow.Platform.RoleApp().ClearAllDataTableCache();
                        //#endregion
                        //RoadFlow.Cache.IO.Opation.Set(getCacheKey(wfInstalled.ID), wfInstalled);
                        scope.Complete();
                        return "1";
                    }
                }
            }
        }

        /// <summary>
        /// 流程另存为
        /// </summary>
        /// <param name="flowID">流程ID</param>
        /// <param name="newName">新流程名称</param>
        /// <returns>返回另存后的流程实体</returns>
        public WorkFlowInfo SaveAs(string flowID, string newName)
        {
            WorkFlowInfo wf = workFlowInfoDAL.FindByID(flowID);
            if (wf == null || newName.IsNullOrEmpty())
            {
                return wf;
            }
            else
            {
                wf.ID = Guid.NewGuid().ToString();
                wf.Name = newName.Trim();
                wf.CreateDate = Next.WorkFlow.Utility.DateTimeNew.Now;
                wf.CreateUserID = new Next.Admin.BLL.UserBLL().CurrentUserID;
                wf.InstallDate = null;
                wf.InstallUserID = null;
                wf.RunJSON = null;
                wf.Status = 1;

                if (!wf.DesignJSON.IsNullOrEmpty())
                {
                    LitJson.JsonData json = LitJson.JsonMapper.ToObject(wf.DesignJSON);
                    json["id"] = wf.ID.ToString();
                    json["name"] = wf.Name;

                    LitJson.JsonData steps = json["steps"];
                    LitJson.JsonData lines = json["lines"];
                    foreach (LitJson.JsonData step in steps)
                    {
                        string oldStepid = step["id"].ToString();
                        string stepid = Guid.NewGuid().ToString();
                        step["id"] = stepid;
                        foreach (LitJson.JsonData line in lines)
                        {
                            if (line["from"].ToString() == oldStepid)
                            {
                                line["from"] = stepid;
                            }
                            if (line["to"].ToString() == oldStepid)
                            {
                                line["to"] = stepid;
                            }
                        }

                    }
                    foreach (LitJson.JsonData line in lines)
                    {
                        line["id"] = Guid.NewGuid().ToString();
                    }
                    wf.DesignJSON = json.ToJson();
                }

                workFlowInfoDAL.Insert(wf);
            }

            return wf;
        }

        /// <summary>
        /// 得到一个流程的缓存键
        /// </summary>
        /// <param name="flowID"></param>
        /// <returns></returns>
        /*private string getCacheKey(string flowID)
        {
            return string.Concat(RoadFlow.Utility.Keys.CacheKeys.WorkFlowInstalled_.ToString(), flowID.ToString("N"));
        }*/

        /// <summary>
        /// 得到流程运行时实体
        /// </summary>
        /// <param name="flowID">流程ID</param>
        /// <returns></returns>
        /*public Next.WorkFlow.Entity.WorkFlowInstalled GetWorkFlowRunModel(string flowID, bool cache = true)
        {
            string fid;
            return flowID.IsGuid(out fid) ? GetWorkFlowRunModel(fid, cache) : null;
        }*/

        /// <summary>
        /// 得到流程运行时实体
        /// </summary>
        /// <param name="flowID">流程ID</param>
        /// <returns></returns>
        public Next.WorkFlow.Entity.WorkFlowInstalled GetWorkFlowRunModel(string flowID, bool cache = true)
        {
            return getWorkFlowRunFromDesign(flowID);
            /*if (!cache)
            {
                return getWorkFlowRunFromDesign(flowID);
            }
            else
            {
                string key = getCacheKey(flowID);
                object obj = RoadFlow.Cache.IO.Opation.Get(key);
                if (obj == null)
                {
                    var wfi = getWorkFlowRunFromDesign(flowID);
                    RoadFlow.Cache.IO.Opation.Set(key, wfi);
                    return wfi;
                }
                else
                {
                    return obj as Next.WorkFlow.Entity.WorkFlowInstalled;
                }
            }*/

        }

        private Next.WorkFlow.Entity.WorkFlowInstalled getWorkFlowRunFromDesign(string flowID)
        {
            var wf = FindByID(flowID);
            if (wf == null || wf.RunJSON.IsNullOrEmpty())
            {
                return null;
            }
            string msg;
            var wfi = GetWorkFlowRunModel(wf.RunJSON, out msg);
            return wfi;
        }

        /// <summary>
        /// 清除一个流程的运行时实体缓存
        /// </summary>
        /// <param name="flowID"></param>
        /*public void ClearWorkFlowCache(string flowID)
        {
            string key = getCacheKey(flowID);
            RoadFlow.Cache.IO.Opation.Remove(key);
        }

        /// <summary>
        /// 刷新一个流程运行时实体
        /// </summary>
        /// <param name="flowID"></param>
        public void RefreshWrokFlowCache(string flowID)
        {
            string key = getCacheKey(flowID);
            RoadFlow.Cache.IO.Opation.Set(key, GetWorkFlowRunModel(flowID, false));
        }*/

        /// <summary>
        /// 得到一个流程运行时实体
        /// </summary>
        /// <param name="jsonString">流程设计json字符串</param>
        /// <returns>流程已安装实体类(如果返回为空则表示验证失败,流程设计不完整)</returns>
        public Next.WorkFlow.Entity.WorkFlowInstalled GetWorkFlowRunModel(string jsonString, out string errMsg)
        {
            errMsg = "";
            Next.WorkFlow.Entity.WorkFlowInstalled wfInstalled = new Next.WorkFlow.Entity.WorkFlowInstalled();
            var json = LitJson.JsonMapper.ToObject(jsonString);

            #region 载入基本信息
            string id = json["id"].ToString();
            if (!id.IsGuid())
            {
                errMsg = "流程ID错误";
                return null;
            }
            else
            {
                wfInstalled.ID = id.ToGuid();
            }

            string name = json["name"].ToString();
            if (name.IsNullOrEmpty())
            {
                errMsg = "流程名称为空";
                return null;
            }
            else
            {
                wfInstalled.Name = name.Trim();
            }

            string type = json["type"].ToString();
            wfInstalled.Type = type.IsNullOrEmpty() ? new Next.Admin.BLL.DictTypeBLL().GetIDByCode("FlowTypes").ToString() : type.Trim();


            string manager = json["manager"].ToString();
            if (manager.IsNullOrEmpty())
            {
                errMsg = "流程管理者为空";
                return null;
            }
            else
            {
                wfInstalled.Manager = manager;
            }

            string instanceManager = json["instanceManager"].ToString();
            if (instanceManager.IsNullOrEmpty())
            {
                errMsg = "流程实例管理者为空";
                return null;
            }
            else
            {
                wfInstalled.Manager = instanceManager;
            }

            wfInstalled.RemoveCompleted = json["removeCompleted"].ToString().ToInt();
            wfInstalled.Debug = json["debug"].ToString().ToInt();
            wfInstalled.DebugUsers = new Next.Admin.BLL.DeptBLL().GetAllUsers(json["debugUsers"].ToString());
            wfInstalled.Note = json["note"].ToString();

            List<Next.WorkFlow.Entity.WorkFlowInstalledSub.DataBases> dataBases = new List<Next.WorkFlow.Entity.WorkFlowInstalledSub.DataBases>();
            var dbs = json["databases"];
            if (dbs.IsArray)
            {
                foreach (LitJson.JsonData db in dbs)
                {
                    dataBases.Add(new Next.WorkFlow.Entity.WorkFlowInstalledSub.DataBases()
                    {
                        LinkID = db["link"].ToString().ToGuid(),
                        LinkName = db["linkName"].ToString(),
                        Table = db["table"].ToString(),
                        PrimaryKey = db["primaryKey"].ToString()
                    });
                }
            }
            wfInstalled.DataBases = dataBases;

            var titleField = json["titleField"];
            if (titleField.IsObject)
            {
                wfInstalled.TitleField = new Next.WorkFlow.Entity.WorkFlowInstalledSub.TitleField()
                {
                    Field = titleField["field"].ToString(),
                    LinkID = titleField["link"].ToString().ToGuid(),
                    LinkName = "",
                    Table = titleField["table"].ToString()
                };
            }
            #endregion

            #region 载入步骤信息
            List<Next.WorkFlow.Entity.WorkFlowInstalledSub.Step> stepsList = new List<Next.WorkFlow.Entity.WorkFlowInstalledSub.Step>();
            LitJson.JsonData steps = json["steps"];
            if (steps.IsArray)
            {
                foreach (LitJson.JsonData step in steps)
                {
                    #region 行为
                    LitJson.JsonData behavior = step["behavior"];
                    Next.WorkFlow.Entity.WorkFlowInstalledSub.StepSet.Behavior behavior1 = new Next.WorkFlow.Entity.WorkFlowInstalledSub.StepSet.Behavior();
                    if (behavior.IsObject)
                    {
                        behavior1.BackModel = behavior["backModel"].ToString().ToInt();
                        behavior1.BackStepID = behavior["backStep"].ToString().ToGuid();
                        behavior1.BackType = behavior["backType"].ToString().ToInt();
                        behavior1.DefaultHandler = behavior["defaultHandler"].ToString();
                        behavior1.FlowType = behavior["flowType"].ToString().ToInt();
                        behavior1.HandlerStepID = behavior["handlerStep"].ToString().ToGuid();
                        behavior1.HandlerType = behavior["handlerType"].ToString().ToInt();
                        behavior1.HanlderModel = behavior["hanlderModel"].ToString().ToInt(3);
                        behavior1.Percentage = behavior["percentage"].ToString().IsDecimal() ? behavior["percentage"].ToString().ToDecimal() : decimal.MinusOne;
                        behavior1.RunSelect = behavior["runSelect"].ToString().ToInt();
                        behavior1.SelectRange = behavior["selectRange"].ToString();
                        behavior1.ValueField = behavior["valueField"].ToString();
                        behavior1.Countersignature = behavior.ContainsKey("countersignature") ? behavior["countersignature"].ToString().ToInt() : 0;
                        behavior1.CountersignaturePercentage = behavior.ContainsKey("countersignaturePercentage") ? behavior["countersignaturePercentage"].ToString().ToDecimal() : decimal.MinusOne;
                        behavior1.SubFlowStrategy = behavior.ContainsKey("subflowstrategy") ? behavior["subflowstrategy"].ToString().ToInt() : int.MinValue;
                    }
                    #endregion
                    #region 按钮
                    LitJson.JsonData buttons = step["buttons"];
                    List<Next.WorkFlow.Entity.WorkFlowInstalledSub.StepSet.Button> buttionList = new List<Next.WorkFlow.Entity.WorkFlowInstalledSub.StepSet.Button>();
                    if (buttons.IsArray)
                    {
                        foreach (LitJson.JsonData button in buttons)
                        {
                            string butID = button["id"].ToString();
                            if (!butID.IsGuid())
                            {
                                continue;
                            }
                            var buttonModel = new WorkFlowButtonsBLL().FindByID(butID);
                            if (buttonModel == null)
                            {
                                continue;
                            }
                            buttionList.Add(new Next.WorkFlow.Entity.WorkFlowInstalledSub.StepSet.Button()
                            {
                                ID = butID,
                                Note = buttonModel.Note.IsNullOrEmpty() ? "" : buttonModel.Note.Replace("\"", "'"),
                                Sort = button["sort"].ToString().ToInt()
                            });
                        }
                    }
                    if (buttionList.Count == 0)
                    {
                        errMsg = string.Format("步骤[{0}]未设置按钮", step["name"].ToString());
                        return null;
                    }
                    #endregion
                    #region 事件
                    LitJson.JsonData event1 = step["event"];
                    Next.WorkFlow.Entity.WorkFlowInstalledSub.StepSet.Event event2 = new Next.WorkFlow.Entity.WorkFlowInstalledSub.StepSet.Event();
                    if (event1.IsObject)
                    {
                        event2.BackAfter = event1["backAfter"].ToString();
                        event2.BackBefore = event1["backBefore"].ToString();
                        event2.SubmitAfter = event1["submitAfter"].ToString();
                        event2.SubmitBefore = event1["submitBefore"].ToString();
                        event2.SubFlowActivationBefore = event1.ContainsKey("subflowActivationBefore") ? event1["subflowActivationBefore"].ToString() : "";
                        event2.SubFlowCompletedBefore = event1.ContainsKey("subflowCompletedBefore") ? event1["subflowCompletedBefore"].ToString() : "";
                    }
                    #endregion
                    #region 表单
                    LitJson.JsonData forms = step["forms"];
                    List<Next.WorkFlow.Entity.WorkFlowInstalledSub.StepSet.Form> formList = new List<Next.WorkFlow.Entity.WorkFlowInstalledSub.StepSet.Form>();
                    if (forms.IsArray)
                    {
                        foreach (LitJson.JsonData form in forms)
                        {
                            formList.Add(new Next.WorkFlow.Entity.WorkFlowInstalledSub.StepSet.Form()
                            {
                                ID = form["id"].ToString().ToGuid(),
                                Name = form["name"].ToString(),
                                Sort = form["srot"].ToString().ToInt()
                            });
                        }
                    }
                    if (formList.Count == 0)
                    {
                        errMsg = string.Format("步骤[{0}]未设置表单", step["name"].ToString());
                        return null;
                    }
                    #endregion
                    #region 字段状态
                    LitJson.JsonData fieldStatus = step["fieldStatus"];
                    List<Next.WorkFlow.Entity.WorkFlowInstalledSub.StepSet.FieldStatus> fieldStatusList = new List<Next.WorkFlow.Entity.WorkFlowInstalledSub.StepSet.FieldStatus>();
                    if (fieldStatus.IsArray)
                    {
                        foreach (LitJson.JsonData field in fieldStatus)
                        {
                            fieldStatusList.Add(new Next.WorkFlow.Entity.WorkFlowInstalledSub.StepSet.FieldStatus()
                            {
                                Check = field["check"].ToString().ToInt(),
                                Field = field["field"].ToString(),
                                Status1 = field["status"].ToString().ToInt()
                            });
                        }
                    }
                    #endregion
                    #region 坐标/基本信息
                    LitJson.JsonData position = step["position"];
                    decimal x = 0, y = 0;
                    if (position.IsObject)
                    {
                        x = position["x"].ToString().ToDecimal();
                        y = position["y"].ToString().ToDecimal();
                    }

                    stepsList.Add(new Next.WorkFlow.Entity.WorkFlowInstalledSub.Step()
                    {
                        Archives = step["archives"].ToString().ToInt(),
                        ArchivesParams = step["archivesParams"].ToString(),
                        Behavior = behavior1,
                        Buttons = buttionList,
                        Event = event2,
                        ExpiredPrompt = step["expiredPrompt"].ToString().ToInt(),
                        Forms = formList,
                        FieldStatus = fieldStatusList,
                        ID = step["id"].ToString().ToGuid(),
                        Type = step.ContainsKey("type") ? step["type"].ToString() : "normal",
                        LimitTime = step["limitTime"].ToString().ToDecimal(),
                        Name = step["name"].ToString(),
                        Note = step["note"].ToString(),
                        OpinionDisplay = step["opinionDisplay"].ToString().ToInt(),
                        OtherTime = step["otherTime"].ToString().ToDecimal(),
                        SignatureType = step["signatureType"].ToString().ToInt(),
                        WorkTime = step["workTime"].ToString().ToDecimal(),
                        SubFlowID = step.ContainsKey("subflow") ? step["subflow"].ToString() : "",
                        Position_x = x,
                        Position_y = y
                    });
                    #endregion

                }
            }
            wfInstalled.Steps = stepsList;
            if (stepsList.Count == 0)
            {
                errMsg = "流程至少需要一个步骤";
                return null;
            }
            #endregion

            #region 载入连线信息

            List<Next.WorkFlow.Entity.WorkFlowInstalledSub.Line> linesList = new List<Next.WorkFlow.Entity.WorkFlowInstalledSub.Line>();
            LitJson.JsonData lines = json.ContainsKey("lines") ? json["lines"] : new LitJson.JsonData();
            if (lines.IsArray)
            {
                foreach (LitJson.JsonData line in lines)
                {
                    linesList.Add(new Next.WorkFlow.Entity.WorkFlowInstalledSub.Line()
                    {
                        ID = line["id"].ToString().ToGuid(),
                        FromID = line["from"].ToString().ToGuid(),
                        ToID = line["to"].ToString().ToGuid(),
                        CustomMethod = line["customMethod"].ToString(),
                        SqlWhere = line["sql"].ToString(),
                        NoAccordMsg = line.ContainsKey("noaccordMsg") ? line["noaccordMsg"].ToString() : "",
                        Organize_SenderIn = line.ContainsKey("organize_senderin") ? line["organize_senderin"].ToString() : "",
                        Organize_SenderNotIn = line.ContainsKey("organize_sendernotin") ? line["organize_sendernotin"].ToString() : "",
                        Organize_SponsorIn = line.ContainsKey("organize_sponsorin") ? line["organize_sponsorin"].ToString() : "",
                        Organize_SponsorNotIn = line.ContainsKey("organize_sponsornotin") ? line["organize_sponsornotin"].ToString() : "",
                        Organize_SenderLeader = line.ContainsKey("organize_senderleader") ? line["organize_senderleader"].ToString() : "",
                        Organize_SenderChargeLeader = line.ContainsKey("organize_senderchargeleader") ? line["organize_senderchargeleader"].ToString() : "",
                        Organize_SponsorLeader = line.ContainsKey("organize_sponsorleader") ? line["organize_sponsorleader"].ToString() : "",
                        Organize_SponsorChargeLeader = line.ContainsKey("organize_sponsorchargeleader") ? line["organize_sponsorchargeleader"].ToString() : "",
                        Organize_NotSenderLeader = line.ContainsKey("organize_notsenderleader") ? line["organize_notsenderleader"].ToString() : "",
                        Organize_NotSenderChargeLeader = line.ContainsKey("organize_notsenderchargeleader") ? line["organize_notsenderchargeleader"].ToString() : "",
                        Organize_NotSponsorLeader = line.ContainsKey("organize_notsponsorleader") ? line["organize_notsponsorleader"].ToString() : "",
                        Organize_NotSponsorChargeLeader = line.ContainsKey("organize_notsponsorchargeleader") ? line["organize_notsponsorchargeleader"].ToString() : ""
                    });
                }
            }

            wfInstalled.Lines = linesList;

            #endregion

            #region 载入其它信息
            //得到第一步
            List<string> firstStepIDList = new List<string>();
            foreach (var step in wfInstalled.Steps)
            {
                if (wfInstalled.Lines.Where(p => p.ToID == step.ID).Count() == 0)
                {
                    firstStepIDList.Add(step.ID);
                    break;
                }
            }
            if (firstStepIDList.Count == 0)
            {
                errMsg = "流程没有开始步骤";
                return null;
            }
            else if (firstStepIDList.Count > 1)
            {
                errMsg = "流程有多个开始步骤";
                return null;
            }

            string lastStepID = string.Empty;
            foreach (var step in wfInstalled.Steps)
            {
                if (wfInstalled.Lines.Where(p => p.FromID == step.ID).Count() == 0)
                {
                    lastStepID = step.ID;
                    break;
                }
            }
            if (lastStepID == string.Empty)
            {
                errMsg = "流程没有结束步骤";
                return null;
            }

            var wf = workFlowInfoDAL.FindByID(wfInstalled.ID);
            if (wf != null)
            {
                wfInstalled.CreateTime = wf.CreateDate;
                wfInstalled.CreateUser = wf.CreateUserID.ToString();
                wfInstalled.DesignJSON = wf.DesignJSON;
                wfInstalled.FirstStepID = firstStepIDList.First();
                wfInstalled.InstallTime = Next.WorkFlow.Utility.DateTimeNew.Now;
                wfInstalled.InstallUser = new Next.Admin.BLL.UserBLL().CurrentUserID.ToString();
                wfInstalled.RunJSON = jsonString;
                wfInstalled.Status = wf.Status;
            }
            #endregion

            return wfInstalled;
        }

        /// <summary>
        /// 得到一个流程步骤的前面所有步骤集合
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="stepID"></param>
        /// <returns></returns>
        public List<Next.WorkFlow.Entity.WorkFlowInstalledSub.Step> GetAllPrevSteps(string flowID, string stepID)
        {
            List<Next.WorkFlow.Entity.WorkFlowInstalledSub.Step> stepList = new List<Next.WorkFlow.Entity.WorkFlowInstalledSub.Step>();
            var wfInstalled = GetWorkFlowRunModel(flowID);
            if (wfInstalled == null)
            {
                return stepList;
            }
            addPrevSteps(stepList, wfInstalled, stepID);
            return stepList.Distinct().ToList();
        }

        private void addPrevSteps(List<Next.WorkFlow.Entity.WorkFlowInstalledSub.Step> list, Next.WorkFlow.Entity.WorkFlowInstalled wfInstalled, string stepID)
        {
            if (wfInstalled == null) return;
            var lines = wfInstalled.Lines.Where(p => p.ToID == stepID);
            foreach (var line in lines)
            {
                var step = wfInstalled.Steps.Where(p => p.ID == line.FromID);
                if (step.Count() > 0)
                {
                    list.Add(step.First());
                    addPrevSteps(list, wfInstalled, step.First().ID);
                }
            }
        }

        /// <summary>
        /// 得到一个流程步骤的前面步骤集合
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="stepID"></param>
        /// <returns></returns>
        public List<Next.WorkFlow.Entity.WorkFlowInstalledSub.Step> GetPrevSteps(string flowID, string stepID)
        {
            List<Next.WorkFlow.Entity.WorkFlowInstalledSub.Step> stepList = new List<Next.WorkFlow.Entity.WorkFlowInstalledSub.Step>();
            var wfInstalled = GetWorkFlowRunModel(flowID);
            if (wfInstalled == null)
            {
                return stepList;
            }
            var lines = wfInstalled.Lines.Where(p => p.ToID == stepID);
            foreach (var line in lines)
            {
                var step = wfInstalled.Steps.Where(p => p.ID == line.FromID);
                if (step.Count() > 0)
                {
                    stepList.Add(step.First());
                }
            }
            return stepList;
        }


        /// <summary>
        /// 得到一个流程当前步骤的后续步骤集合
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="stepID"></param>
        /// <returns></returns>
        public List<Next.WorkFlow.Entity.WorkFlowInstalledSub.Step> GetNextSteps(string flowID, string stepID)
        {
            List<Next.WorkFlow.Entity.WorkFlowInstalledSub.Step> stepList = new List<Next.WorkFlow.Entity.WorkFlowInstalledSub.Step>();
            var wfInstalled = GetWorkFlowRunModel(flowID);
            if (wfInstalled == null)
            {
                return stepList;
            }
            var lines = wfInstalled.Lines.Where(p => p.FromID == stepID);
            foreach (var line in lines)
            {
                var step = wfInstalled.Steps.Where(p => p.ID == line.ToID);
                if (step.Count() > 0)
                {
                    stepList.Add(step.First());
                }
            }
            return stepList;
        }


        /// <summary>
        /// 根据步骤ID得到步骤名称
        /// </summary>
        /// <param name="stepID"></param>
        /// <param name="flowID"></param>
        /// <param name="flowName"></param>
        /// <param name="defaultFirstStepName">如果步骤为空是否返回第一步的名称</param>
        /// <returns></returns>
        public string GetStepName(string stepID, string flowID, out string flowName, bool defaultFirstStepName = false)
        {
            flowName = "";
            var wfInstalled = GetWorkFlowRunModel(flowID);
            if (wfInstalled == null) return "";
            if (stepID == string.Empty && defaultFirstStepName)
            {
                stepID = wfInstalled.FirstStepID;
            }
            flowName = wfInstalled.Name;
            var steps = wfInstalled.Steps.Where(p => p.ID == stepID);
            return steps.Count() > 0 ? steps.First().Name : "";
        }

        /// <summary>
        /// 根据步骤ID得到步骤名称
        /// </summary>
        /// <param name="stepID"></param>
        /// <param name="flowID"></param>
        /// <param name="defautFirstStepName">如果步骤ID为空是否默认为第一步</param>
        /// <returns></returns>
        public string GetStepName(string stepID, string flowID, bool defautFirstStepName = false)
        {
            string temp;
            return GetStepName(stepID, flowID, out temp, defautFirstStepName);
        }
        /// <summary>
        /// 根据步骤ID得到步骤名称
        /// </summary>
        /// <param name="stepID"></param>
        /// <param name="flowID"></param>
        /// <param name="defautFirstStepName">如果步骤ID为空是否默认为第一步</param>
        /// <returns></returns>
        public string GetStepName(string stepID, Next.WorkFlow.Entity.WorkFlowInstalled wfinstalled, bool defautFirstStepName = false)
        {
            if (wfinstalled == null) return "";
            if (stepID == string.Empty && defautFirstStepName)
            {
                stepID = wfinstalled.FirstStepID;
            }
            var steps = wfinstalled.Steps.Where(p => p.ID == stepID);
            return steps.Count() > 0 ? steps.First().Name : "";
        }

        /// <summary>
        /// 得到流程名称
        /// </summary>
        /// <param name="flowID"></param>
        /// <returns></returns>
        public string GetFlowName(string flowID)
        {
            var flow = GetWorkFlowRunModel(flowID);
            return flow != null ? flow.Name : "";
        }

        /// <summary>
        /// 查询所有ID和名称
        /// </summary>
        public Dictionary<string, string> GetAllIDAndName()
        {
            return workFlowInfoDAL.GetAllIDAndName();
        }

        /// <summary>
        /// 得到所有流程选择项
        /// </summary>
        /// <returns></returns>
        public string GetOptions(string value = "")
        {
            var dicts = GetAllIDAndName();
            StringBuilder options = new StringBuilder();
            foreach (var dict in dicts.OrderBy(p => p.Value))
            {
                options.AppendFormat("<option value=\"{0}\" {1}>{2}</option>", dict.Key,
                    ("," + value + ",").Contains("," + dict.Key.ToString() + ",") ? "selected=\"selected\"" : "", dict.Value);
            }
            return options.ToString();
        }

        /// <summary>
        /// 得到一个人员可管理实例的所有流程选择项
        /// </summary>
        /// <returns></returns>
        public string GetOptions(Dictionary<string, string> flows, string typeid, string value = "")
        {
            var dicts = flows;
            StringBuilder options = new StringBuilder();
            foreach (var dict in dicts)
            {
                options.AppendFormat("<option value=\"{0}\" {1}>{2}</option>", dict.Key,
                    dict.Key.ToString() == value ? "selected=\"selected\"" : "", dict.Value);
            }
            return options.ToString();
        }

        /// <summary>
        /// 得到一个人员可管理实例的流程ID和名称列表
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="typeID">分类ID</param>
        /// <returns></returns>
        public Dictionary<string, string> GetInstanceManageFlowIDList(string userID, string typeID = "")
        {
            var flows = this.GetAll();
            DeptBLL borg = new DeptBLL();
            Dictionary<string, string> flowids = new Dictionary<string, string>();
            foreach (var flow in flows)
            {
                if (typeID.IsGuid() && !GetAllChildsIDString(typeID.ToGuid()).Contains(flow.Type.ToString(), StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }
                if (borg.GetAllUsers(flow.InstanceManager).Exists(p => p.ID == userID))
                {
                    flowids.Add(flow.ID, flow.Name);
                }
            }
            return flowids;
        }

        /// <summary>
        /// 生成印章图片
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public Bitmap CreateSignImage(string UserName)
        {
            if (UserName.IsNullOrEmpty())
            {
                return null;
            }
            System.Random rand = new Random(UserName.GetHashCode());
            Size ImageSize = Size.Empty;
            Font myFont = new Font("隶书", 16);

            // 计算图片大小 
            using (Bitmap bmp1 = new Bitmap(5, 5))
            {
                using (Graphics g = Graphics.FromImage(bmp1))
                {
                    SizeF size = g.MeasureString(UserName, myFont, 10000);
                    ImageSize.Width = (int)size.Width + 4;
                    ImageSize.Height = (int)size.Height;
                }
            }

            // 创建图片 
            Bitmap bmp = new Bitmap(ImageSize.Width, ImageSize.Height);

            // 绘制文本 
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                using (StringFormat f = new StringFormat())
                {
                    f.Alignment = StringAlignment.Center;
                    f.LineAlignment = StringAlignment.Center;
                    f.FormatFlags = StringFormatFlags.NoWrap;
                    g.DrawString(
                        UserName,
                        myFont,
                        Brushes.Red,
                        new RectangleF(
                        0,
                        0,
                        ImageSize.Width,
                        ImageSize.Height),
                        f);
                }
            }

            // 随机制造噪点 (用户名绑定)
            Color c = Color.Red;
            int x, y;
            int num = ImageSize.Width * ImageSize.Height * 8 / 100;
            for (int iCount = 0; iCount < num; iCount++)
            {
                x = rand.Next(0, 4);
                y = rand.Next(ImageSize.Height);
                bmp.SetPixel(x, y, c);

                x = rand.Next(ImageSize.Width - 4, ImageSize.Width);
                y = rand.Next(ImageSize.Height);
                bmp.SetPixel(x, y, c);

            }

            int num1 = ImageSize.Width * ImageSize.Height * 20 / 100;
            for (int iCount = 0; iCount < num1; iCount++)
            {
                x = rand.Next(ImageSize.Width);
                y = rand.Next(0, 4);
                bmp.SetPixel(x, y, c);

                x = rand.Next(ImageSize.Width);
                y = rand.Next(ImageSize.Height - 4, ImageSize.Height);
                bmp.SetPixel(x, y, c);
            }

            int num2 = ImageSize.Width * ImageSize.Height / 150;
            for (int iCount = 0; iCount < num2; iCount++)
            {
                x = rand.Next(ImageSize.Width);
                y = rand.Next(ImageSize.Height);
                bmp.SetPixel(x, y, c);
            }

            myFont.Dispose();

            return bmp;
        }

        /// <summary>
        /// 得到流程运行时自动标题
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="stepID"></param>
        /// <returns></returns>
        public string GetAutoTitle(string flowID, string stepID)
        {
            string flowName;
            string stepName = GetStepName(stepID.ToGuid(), flowID.ToGuid(), out flowName, true);
            return string.Format("<div class='flowautotitle'>{0} - {1}</div>", flowName, stepName);
        }

        /// <summary>
        /// 得到默认任务标题
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="stepID"></param>
        /// <returns></returns>
        public string GetAutoTaskTitle(string flowID, string stepID, string groupID = "")
        {
            var wfrun = GetWorkFlowRunModel(flowID);
            if (wfrun == null) return "";
            string flowName = wfrun.Name;
            string senderName = "";
            string gid;
            if (groupID.IsGuid(out gid) || gid == string.Empty)
            {
                var fqz = new WorkFlowTaskBLL().GetFirstSnderID(flowID.ToGuid(), gid);
                senderName = new UserBLL().FindByID(fqz).FullName;
            }
            if (senderName.IsNullOrEmpty())
            {
                senderName = new UserBLL().CurrentUserName;
            }

            return string.Concat(flowName, "(", senderName, ")");
        }

        /// <summary>
        /// 保存表单数据
        /// </summary>
        public string SaveFromData(string instanceid, WorkFlowCustomEventParams eventParams)
        {
            //保存自定义表单内容
            string form_CustomSaveMethod = System.Web.HttpContext.Current.Request.Form["Form_CustomSaveMethod"];
            if (!form_CustomSaveMethod.IsNullOrEmpty())
            {
                return new WorkFlowTaskBLL().ExecuteFlowCustomEvent(form_CustomSaveMethod, eventParams).ToString();
            }

            if ("1" != System.Web.HttpContext.Current.Request.Form["Form_AutoSaveData"])
            {
                return instanceid;
            }
            Next.WorkFlow.BLL.DBConnectionBLL bdbconn = new Next.WorkFlow.BLL.DBConnectionBLL();
            string dbconnid = System.Web.HttpContext.Current.Request.Form["Form_DBConnID"];
            string dbtable = System.Web.HttpContext.Current.Request.Form["Form_DBTable"];
            string dbtablepk = System.Web.HttpContext.Current.Request.Form["Form_DBTablePk"];
            string dbtabletitle = System.Web.HttpContext.Current.Request.Form["Form_DBTableTitle"];
            if (!dbconnid.IsGuid())
            {
                return instanceid;
            }

            DBConnection dbconn = bdbconn.FindByID(dbconnid.ToGuid());
            if (dbconn == null)
            {
                return instanceid;
            }

            using (System.Data.IDbConnection conn = bdbconn.GetConnection(dbconn))
            {
                if (conn == null)
                {
                    return instanceid;
                }
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    System.Web.HttpContext.Current.Response.Write("连接数据库出错：" + ex.Message);
                    //RoadFlow.Platform.Log.Insert(ex);
                }

                string sql = string.Empty;
                List<System.Data.IDataParameter> parList = new List<System.Data.IDataParameter>();
                if (instanceid.IsNullOrEmpty())
                {
                    sql = string.Format("SELECT * FROM {0} WHERE 1=0", dbtable);
                }
                else
                {
                    switch (dbconn.Type)
                    {
                        case "MySql":
                            sql = string.Format("SELECT * FROM {0} WHERE {1}='{2}'", dbtable, dbtablepk, instanceid);
                            
                            break;

                    }
                }
                var sConnectionString = "Database='Next';Data Source='localhost';Port='3306';User Id='root';Password='';charset='utf8';pooling=true";
                MySqlConnection Conn = new MySqlConnection(sConnectionString);

                //打开Conn

                Conn.Open();

                System.Data.IDbDataAdapter dataAdapter = new MySqlDataAdapter(sql, Conn);//bdbconn.GetDataAdapter(conn, dbconn.Type, sql, parList.ToArray());
                //System.Data.DataSet ds = new System.Data.DataSet();
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);
                //ds = workFlowInfoDAL.SqlTable(sql).DataSet;
                System.Data.DataTable schemaDt = bdbconn.GetTableSchema(conn, dbtable, dbconn.Type);
                System.Data.DataTable dt = ds.Tables[0];
                bool isNew = dt.Rows.Count == 0;
                if (isNew)
                {
                    dt.Rows.Add(dt.NewRow());
                }

                //设置主键值(应用于参数中有instanceid而对应业务表中没有数据时保存会出错)
                if (!instanceid.IsNullOrEmpty())
                {
                    dt.Rows[0][dbtablepk] = instanceid;
                }

                #region 保存主表数据
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string colnumName = dt.Columns[i].ColumnName;
                    if (string.Compare(colnumName, dbtablepk, true) == 0)
                    {
                        continue;
                    }
                    string name = string.Concat(dbtable, ".", dt.Columns[i].ColumnName);

                    string value = System.Web.HttpContext.Current.Request.Form[name];
                    if (value == null && !isNew)
                    {
                        continue;
                    }
                    var colnum = dt.Columns[i];
                    string colnumDataType = colnum.DataType.FullName;
                    object defaultValue;

                    System.Data.DataRow[] schemaDrs = schemaDt.Select(string.Format("Field='{0}'", colnumName));
                    bool hasDefault = false;//列是否有默认值
                    bool hasNull = false;//列是否可以为空
                    bool isSet = getColnumIsValue(colnumDataType, value, out defaultValue);

                    switch (dbconn.Type)
                    {
                        case "MySql":
                            hasDefault = schemaDrs.Length > 0 && schemaDrs[0]["Default"].ToString() != string.Empty;
                            hasNull = schemaDrs.Length > 0 && schemaDrs[0]["NULL"].ToString() != "0";
                            break;


                    }

                    if (isSet)
                    {
                        dt.Rows[0][colnumName] = value;
                    }
                    else
                    {
                        if (!hasDefault)
                        {
                            if (hasNull)
                            {
                                dt.Rows[0][colnumName] = DBNull.Value;
                            }
                            else
                            {
                                dt.Rows[0][colnumName] = defaultValue;
                            }
                        }
                    }
                }

                #endregion

                #region 设置主键值
                bool isIdentity = false;
                if (isNew)
                {
                    if (instanceid.IsNullOrEmpty())
                    {
                        var pkColnum = dt.Columns[dbtablepk];
                        System.Data.DataRow[] schemaDrs = schemaDt.Select(string.Format("Field='{0}'", dbtablepk));
                        if (schemaDrs.Length > 0)
                        {
                            isIdentity = false;
                            bool isDefault = false;
                            bool isGuid = false;
                            switch (dbconn.Type)
                            {
                                case "MySql":
                                    isIdentity = schemaDrs[0]["Key"].ToString() == "1";
                                    isDefault = schemaDrs[0]["Default"].ToString() != "0";
                                    isGuid = pkColnum.DataType.FullName == "System.String";
                                    break;

                            }
                            if (!isIdentity && isGuid)
                            {
                                instanceid = Guid.NewGuid().ToString();
                                dt.Rows[0][dbtablepk] = instanceid;
                            }
                        }
                    }
                    else
                    {
                        //dt.Rows[0][dbtablepk] = instanceid;
                    }
                }
                #endregion

                #region 执行保存
                switch (dbconn.Type)
                {
                    case "MySql":
                        MySql.Data.MySqlClient.MySqlCommandBuilder scb = new MySql.Data.MySqlClient.MySqlCommandBuilder((MySql.Data.MySqlClient.MySqlDataAdapter)dataAdapter);
                        break;

                }
                //workFlowInfoDAL.Update
                dataAdapter.Update(ds);
                #endregion

                #region 如果是新增，又是自增列则查询刚插入的自增列值
                if (isNew && isIdentity)
                {
                    switch (dbconn.Type)
                    {
                        case "MySql":
                            string identitysql = "SELECT @@IDENTITY";
                            using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(identitysql, (MySql.Data.MySqlClient.MySqlConnection)conn))
                            {
                                object obj = cmd.ExecuteScalar();
                                if (obj != null)
                                {
                                    instanceid = obj.ToString();
                                    dt.Rows[0][dbtablepk] = instanceid;
                                }
                            }
                            break;

                    }
                }
                #endregion

                #region 保存从表数据
                string flowSubTableIDString = System.Web.HttpContext.Current.Request.Form["flowsubtable_id"] ?? "";
                string[] flowSubTableIDArray = flowSubTableIDString.Split(',');
                foreach (string flowSubTableID in flowSubTableIDArray)
                {
                    string secondtable = System.Web.HttpContext.Current.Request.Form["flowsubtable_" + flowSubTableID + "_secondtable"];
                    string primarytablefiled = System.Web.HttpContext.Current.Request.Form["flowsubtable_" + flowSubTableID + "_primarytablefiled"];
                    string secondtableprimarykey = System.Web.HttpContext.Current.Request.Form["flowsubtable_" + flowSubTableID + "_secondtableprimarykey"];
                    string secondtablerelationfield = System.Web.HttpContext.Current.Request.Form["flowsubtable_" + flowSubTableID + "_secondtablerelationfield"];
                    if (secondtable.IsNullOrEmpty() || primarytablefiled.IsNullOrEmpty() || secondtableprimarykey.IsNullOrEmpty() || secondtablerelationfield.IsNullOrEmpty())
                    {
                        continue;
                    }
                    string primyarTableFeldValue = dt.Rows[0][primarytablefiled].ToString();
                    if (primyarTableFeldValue.IsNullOrEmpty())
                    {
                        continue;
                    }
                    string subSql = string.Empty;
                    List<System.Data.IDataParameter> parList1 = new List<System.Data.IDataParameter>();
                    switch (dbconn.Type)
                    {
                        case "MySql":
                            subSql = string.Format("SELECT * FROM {0} WHERE {1}=@pk", secondtable, secondtablerelationfield);
                            parList1.Add(new MySql.Data.MySqlClient.MySqlParameter("@pk", primyarTableFeldValue));
                            break;

                    }
                    string[] colGuidArray = (System.Web.HttpContext.Current.Request.Form["hidden_guid_" + flowSubTableID] ?? "").Split(',');
                    System.Data.IDbDataAdapter dataAdapter1 = bdbconn.GetDataAdapter(conn, dbconn.Type, subSql, parList1.ToArray());
                    System.Data.DataSet ds1 = new System.Data.DataSet();
                    dataAdapter1.Fill(ds1);
                    System.Data.DataTable schemaDt1 = bdbconn.GetTableSchema(conn, secondtable, dbconn.Type);
                    System.Data.DataTable dt1 = ds1.Tables[0];
                    bool isInitNew = dt1.Rows.Count == 0;
                    foreach (string colGuid in colGuidArray)
                    {
                        bool isNew1 = true;
                        System.Data.DataRow dr1 = null;
                        foreach (System.Data.DataRow dr in dt1.Rows)
                        {
                            if (string.Compare(dr[secondtableprimarykey].ToString(), colGuid, 0) == 0)
                            {
                                dr1 = dr;
                                isNew1 = false;
                                break;
                            }
                        }

                        if (isNew1)
                        {
                            dr1 = dt1.NewRow();
                            dr1[secondtablerelationfield] = primyarTableFeldValue;
                            dt1.Rows.Add(dr1);
                            isNew1 = true;
                        }

                        #region 循环保存列数据
                        for (int i = 0; i < dt1.Columns.Count; i++)
                        {
                            string colnumName1 = dt1.Columns[i].ColumnName;
                            if (string.Compare(colnumName1, secondtableprimarykey, true) == 0
                                 || string.Compare(colnumName1, secondtablerelationfield, 0) == 0)
                            {
                                continue;
                            }

                            string value1 = System.Web.HttpContext.Current.Request.Form[flowSubTableID + "_" + colGuid + "_" + secondtable + "_" + colnumName1];
                            if (value1 == null && !isNew1)
                            {
                                continue;
                            }
                            var colnum1 = dt1.Columns[i];
                            string colnumDataType1 = colnum1.DataType.FullName;
                            object defaultValue1 = string.Empty;

                            System.Data.DataRow[] schemaDrs1 = schemaDt1.Select(string.Format("Field='{0}'", colnumName1));
                            bool hasDefault1 = schemaDrs1.Length > 0 && schemaDrs1[0]["Default"].ToString() != "0";//列是否有默认值
                            bool hasNull1 = schemaDrs1.Length > 0 && schemaDrs1[0]["Null"].ToString() != "0";//列是否可以为空
                            bool isSet1 = getColnumIsValue(colnumDataType1, value1, out defaultValue1);
                            if (isSet1)
                            {
                                dr1[colnumName1] = value1;
                            }
                            else
                            {
                                if (!hasDefault1)
                                {
                                    if (hasNull1)
                                    {
                                        dr1[colnumName1] = DBNull.Value;
                                    }
                                    else
                                    {
                                        dr1[colnumName1] = defaultValue1;
                                    }
                                }
                            }
                        }

                        #endregion
                    }
                    #region 删除多余行
                    if (!isInitNew)
                    {
                        for (int i = 0; i < dt1.Rows.Count; i++)
                        {
                            bool isIn = false;
                            foreach (string colGuid in colGuidArray)
                            {
                                if (dt1.Rows[i][secondtableprimarykey].ToString().IsNullOrEmpty() || string.Compare(dt1.Rows[i][secondtableprimarykey].ToString(), colGuid, 0) == 0)
                                {
                                    isIn = true;
                                    break;
                                }
                            }
                            if (!isIn)
                            {
                                dt1.Rows[i].Delete();
                            }
                        }
                    }
                    #endregion


                    #region 执行保存
                    switch (dbconn.Type)
                    {
                        case "MySql":
                            MySql.Data.MySqlClient.MySqlCommandBuilder scb1 = new MySql.Data.MySqlClient.MySqlCommandBuilder((MySql.Data.MySqlClient.MySqlDataAdapter)dataAdapter1);
                            break;

                    }
                    dataAdapter1.Update(ds1);
                    #endregion

                }

                #endregion

                return instanceid;
            }
        }

        /// <summary>
        /// 判断列是否有值
        /// </summary>
        /// <param name="colnumDataType"></param>
        /// <param name="value"></param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        private bool getColnumIsValue(string colnumDataType, string value, out object defaultValue)
        {
            bool isSet = false;
            defaultValue = null;
            switch (colnumDataType)
            {
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                    isSet = value.IsInt();
                    defaultValue = int.MinValue;
                    break;
                case "System.String":
                    isSet = value != null;
                    defaultValue = "";
                    break;
                case "System.Guid":
                    isSet = value.IsGuid();
                    defaultValue = string.Empty;
                    break;
                case "System.Decimal":
                    isSet = value.IsDecimal();
                    defaultValue = decimal.MinValue;
                    break;
                case "System.Double":
                case "System.Single":
                    isSet = value.IsDouble();
                    defaultValue = double.MinValue;
                    break;
                case "System.DateTime":
                    isSet = value.IsDateTime();
                    defaultValue = DateTime.MinValue;
                    break;
                case "System.Object":
                    isSet = value != null;
                    defaultValue = "";
                    break;
                case "System.Boolean":
                    isSet = value != null && (value.ToString().ToLower() == "false"
                        || value.ToString().ToLower() == "true");
                    defaultValue = 0;
                    break;
            }
            return isSet;
        }

        /// <summary>
        /// 得到实例数据
        /// </summary>
        /// <param name="connid"></param>
        /// <param name="table"></param>
        /// <param name="pk"></param>
        /// <param name="instanceid"></param>
        /// <returns></returns>
        public LitJson.JsonData GetFormData(string connid, string table, string pk, string instanceid, string filedStatus = "")
        {
            LitJson.JsonData jsonData = new LitJson.JsonData();
            if (instanceid.IsNullOrEmpty())
            {
                return jsonData;
            }
            Next.WorkFlow.BLL.DBConnectionBLL bdbconn = new Next.WorkFlow.BLL.DBConnectionBLL();
            DBConnection dbconn = bdbconn.FindByID(connid.ToGuid());
            if (dbconn == null)
            {
                return "";
            }

            using (System.Data.IDbConnection conn = bdbconn.GetConnection(dbconn))
            {
                if (conn == null)
                {
                    return "";
                }
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    System.Web.HttpContext.Current.Response.Write("连接数据库出错：" + ex.Message);
                    //RoadFlow.Platform.Log.Add(ex);
                }
                string sql = string.Empty;
                List<System.Data.IDataParameter> parList = new List<System.Data.IDataParameter>();
                switch (dbconn.Type)
                {
                    case "MySql":
                        sql = string.Format("SELECT * FROM {0} WHERE {1}='{2}'", table, pk, instanceid);
                        //parList.Add(new MySql.Data.MySqlClient.MySqlParameter("@pk", instanceid));
                        break;

                }

                //System.Data.IDbDataAdapter dataAdapter = bdbconn.GetDataAdapter(conn, dbconn.Type, sql, parList.ToArray());
                System.Data.DataSet ds = new System.Data.DataSet();
                //dataAdapter.Fill(ds);
                ds=workFlowInfoDAL.SqlTable(sql).DataSet;
                /*if (dataAdapter.SelectCommand != null)
                {
                    dataAdapter.SelectCommand.Dispose();
                }*/

                System.Data.DataTable dt = ds.Tables[0];
                LitJson.JsonData json = null;
                if (!filedStatus.IsNullOrEmpty())
                {
                    json = LitJson.JsonMapper.ToObject(filedStatus);
                }
                if (dt.Rows.Count > 0)
                {
                    System.Data.DataRow dr = dt.Rows[0];
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        bool isShow = true;
                        string fileName = table + "_" + dt.Columns[i].ColumnName;
                        if (json != null && json.ContainsKey(fileName))
                        {
                            string status = json[fileName].ToString();
                            if (!status.IsNullOrEmpty())
                            {
                                string[] statusArray = status.Split('_');
                                if (statusArray.Length == 2 && "2" == statusArray[0])
                                {
                                    isShow = false;
                                }
                            }
                        }
                        //string value = dr[dt.Columns[i].ColumnName].ToString();
                        jsonData[fileName] = isShow ? dr[dt.Columns[i].ColumnName].ToString() : "";
                    }
                }

            }
            return jsonData;
        }

        /// <summary>
        /// 得到实例数据
        /// </summary>
        /// <param name="connid"></param>
        /// <param name="table"></param>
        /// <param name="pk"></param>
        /// <param name="instanceid"></param>
        /// <returns>json字符串</returns>
        public string GetFormDataJsonString(string connid, string table, string pk, string instanceid)
        {
            LitJson.JsonData jsonData = GetFormData(connid, table, pk, instanceid);
            return GetFormDataJsonString(jsonData);
        }


        /// <summary>
        /// 得到实例数据
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns>json字符串</returns>
        public string GetFormDataJsonString(LitJson.JsonData jsonData)
        {
            string json = jsonData.ToJson();
            return json.IsNullOrEmpty() ? "{}" : json;
        }

        /// <summary>
        /// 得到从表数据
        /// </summary>
        /// <param name="connID">连接ID</param>
        /// <param name="secondTable">从表名称</param>
        /// <param name="relationField">关联字段</param>
        /// <param name="fieldValue">关联字段值</param>
        /// <param name="sortField">排序字段</param>
        /// <returns></returns>
        public LitJson.JsonData GetSubTableData(string connID, string secondTable, string relationField, string fieldValue, string sortField = "")
        {
            LitJson.JsonData jsonData = new LitJson.JsonData();
            if (fieldValue.IsNullOrEmpty())
            {
                return jsonData;
            }
            Next.WorkFlow.BLL.DBConnectionBLL bdbconn = new Next.WorkFlow.BLL.DBConnectionBLL();
            DBConnection dbconn = bdbconn.FindByID(connID.ToGuid());
            if (dbconn == null)
            {
                return "";
            }

            using (System.Data.IDbConnection conn = bdbconn.GetConnection(dbconn))
            {
                if (conn == null)
                {
                    return "";
                }
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    System.Web.HttpContext.Current.Response.Write("连接数据库出错：" + ex.Message);
                    //RoadFlow.Platform.Log.Add(ex);
                }
                string sql = string.Empty;
                List<System.Data.IDataParameter> parList = new List<System.Data.IDataParameter>();
                switch (dbconn.Type)
                {
                    case "MySql":
                        sql = string.Format("SELECT * FROM {0} WHERE {1}=@fieldvalue {2}", secondTable, relationField,
                                 (sortField.IsNullOrEmpty() ? "" : string.Concat("ORDER BY ", sortField)));
                        parList.Add(new MySql.Data.MySqlClient.MySqlParameter("@fieldvalue", fieldValue));
                        break;

                }

                System.Data.IDbDataAdapter dataAdapter = bdbconn.GetDataAdapter(conn, dbconn.Type, sql, parList.ToArray());
                System.Data.DataSet ds = new System.Data.DataSet();
                dataAdapter.Fill(ds);
                if (dataAdapter.SelectCommand != null)
                {
                    dataAdapter.SelectCommand.Dispose();
                }
                System.Data.DataTable dt = ds.Tables[0];
                //jsonData.SetJsonType(LitJson.JsonType.Array);
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    LitJson.JsonData data = new LitJson.JsonData();
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        data[secondTable + "_" + dt.Columns[i].ColumnName] = dr[dt.Columns[i].ColumnName].ToString();
                    }
                    jsonData.Add(data);
                }
            }
            return jsonData;
        }

        /// <summary>
        /// 得到实例某个字段的值
        /// </summary>
        /// <param name="jsonData"></param>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public string GetFromFieldData(LitJson.JsonData jsonData, string table, string field)
        {
            string value = string.Empty;
            if (jsonData == null || table.IsNullOrEmpty() || field.IsNullOrEmpty())
            {
                return value;
            }
            var key = string.Concat(table, "_", field);
            if (!jsonData.ContainsKey(key))
            {
                return value;
            }
            value = jsonData[key].ToString();
            return value;
        }

        /// <summary>
        ///  得到一个流程一个步骤的字段状态设置
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="stepID"></param>
        /// <returns></returns>
        public string GetFieldStatus(string flowID, string stepID)
        {
            string fid, sid;
            if (!flowID.IsGuid(out fid))
            {
                return "{}";
            }
            var wfinstance = GetWorkFlowRunModel(fid);
            if (wfinstance == null)
            {
                return "{}";
            }
            if (!stepID.IsGuid(out sid))
            {
                sid = wfinstance.FirstStepID;
            }
            var steps = wfinstance.Steps.Where(p => p.ID == sid);
            if (steps.Count() == 0)
            {
                return "{}";
            }
            var step = steps.First();
            var fieldStatus = step.FieldStatus;
            StringBuilder sb = new StringBuilder("{");
            int count = fieldStatus.Count();
            int i = 0;
            foreach (var fs in fieldStatus)
            {
                var fields = fs.Field.Split('.');
                if (fields.Length != 3)
                {
                    continue;
                }
                var fieldName = fields[1] + "_" + fields[2];
                sb.AppendFormat("\"{0}\":\"{1}\"", fieldName, string.Concat(fs.Status1, "_", fs.Check));
                if (i++ < count - 1)
                {
                    sb.Append(",");
                }
            }
            return sb.ToString() + "}";
        }

        /// <summary>
        /// 得到下级ID字符串
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetAllChildsIDString(string id, bool isSelf = true)
        {
            return new Next.Admin.BLL.DictTypeBLL().GetAllChildsIDString(id, true);
        }

        /// <summary>
        /// 得到类型选择项
        /// </summary>
        /// <returns></returns>
        public string GetTypeOptions(string value = "")
        {
            return new Next.Admin.BLL.DictTypeBLL().GetOptionsByCode("FlowTypes", DictTypeBLL.OptionValueField.ID, value);
        }

        /// <summary>
        /// 查询所有记录
        /// </summary>
        public List<WorkFlowInfo> GetByTypes(string typeString)
        {
            return workFlowInfoDAL.GetByTypes(typeString);
        }

        /// <summary>
        /// 得到一个类型下的流程ID
        /// </summary>
        /// <param name="typeID"></param>
        /// <returns></returns>
        public string GetFlowIDFromType(string typeID)
        {
            var flows = GetByTypes(GetAllChildsIDString(typeID));
            StringBuilder sb = new StringBuilder();
            foreach (var flow in flows)
            {
                sb.Append(flow.ID);
                sb.Append(",");
            }
            return sb.ToString().TrimEnd(',');
        }


    }

}


                        #endregion