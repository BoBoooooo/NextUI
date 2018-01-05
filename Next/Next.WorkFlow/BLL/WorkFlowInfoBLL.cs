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
        /// 查询所有ID和名称
        /// </summary>
        public Dictionary<string, string> GetAllIDAndName()
        {
            return workFlowInfoDAL.GetAllIDAndName();
        }
        /// <summary>
        /// 得到类型选择项
        /// </summary>
        /// <returns></returns>
        public string GetTypeOptions(string value = "")
        {
            return new DictTypeBLL().GetOptionsByCode("FlowTypes", DictTypeBLL.OptionValueField.ID, value);
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

            if (string.IsNullOrEmpty(id))
            {
                return "请先新建或打开流程!";
            }
            else if (string.IsNullOrEmpty(name))
            {
                return "流程名称不能为空!";
            }
            else
            {
                flowID=id;
                WorkFlowInfoBLL bwf = new WorkFlowInfoBLL();
                WorkFlowInfo wf = bwf.FindByID(flowID);
                bool isAdd = false;
                if (wf == null)
                {
                    wf = new WorkFlowInfo();
                    isAdd = true;
                    wf.ID = flowID;
                    wf.CreateDate = Next.WorkFlow.Utility.DateTimeNew.Now;
                    wf.CreateUserID = (string) HttpContext.Current.Session["UserID"];// RoadFlow.Platform.Users.CurrentUserID;
                    wf.Status = 1;
                }
                wf.DesignJSON = jsonString;
                wf.InstanceManager = jsonData["instanceManager"].ToString();
                wf.Manager = jsonData["manager"].ToString();
                wf.Name = name.Trim();
                wf.Type = string.IsNullOrEmpty(type) ? type : new DictTypeBLL().GetIDByCode("FlowTypes");
                try
                {
                    if (isAdd)
                    {
                        bwf.Insert(wf);
                    }
                    else
                    {
                        bwf.Update(wf,wf.ID);
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
        public string InstallFlow(string jsonString)
        {
            string saveInfo = SaveFlow(jsonString);
            if ("1" != saveInfo)
            {
                return saveInfo;
            }
            string errMsg;

            WorkFlowInstalled wfInstalled = GetWorkFlowRunModel(jsonString, out errMsg);
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
                        wf.InstallUserID = wfInstalled.InstallUser;
                        wf.RunJSON = wfInstalled.RunJSON;
                        wf.Status = 2;
                        workFlowInfoDAL.Update(wf,wf.ID);

                        wfInstalled.Status = 2;


                        AppLibraryBLL bappLibrary = new AppLibraryBLL();
                        AppLibrary app = bappLibrary.GetByCode(wfInstalled.ID.ToString());
                        bool isAdd = false;
                        if (app == null)
                        {
                            isAdd = true;
                            app = new AppLibrary();
                            app.ID = Guid.NewGuid().ToString();
                        }
                        app.Address = "WorkFlowRun/Index" ;
                        app.Code = wfInstalled.ID.ToString();
                        app.Note = "流程应用";
                        app.OpenMode = 0;
                        app.Params = "flowid=" + wfInstalled.ID.ToString();
                        app.Title = wfInstalled.Name;
                        app.Type = wfInstalled.Type.IsNullOrEmpty() ? wfInstalled.Type : new DictTypeBLL().GetIDByCode("FlowTypes");
                        if (isAdd)
                        {
                            bappLibrary.Insert(app);
                        }
                        else
                        {
                            bappLibrary.Update(app,app.ID);
                        }
                        /*bappLibrary.ClearCache();
                        new RoadFlow.Platform.RoleApp().ClearAllDataTableCache();
                        
                        RoadFlow.Cache.IO.Opation.Set(getCacheKey(wfInstalled.ID), wfInstalled);*/
                        scope.Complete();
                        return "1";
                    }
                }
            }
        }
        /// <summary>
        /// 得到一个流程运行时实体
        /// </summary>
        /// <param name="jsonString">流程设计json字符串</param>
        /// <returns>流程已安装实体类(如果返回为空则表示验证失败,流程设计不完整)</returns>
        public WorkFlowInstalled GetWorkFlowRunModel(string jsonString, out string errMsg)
        {
            errMsg = "";
            WorkFlowInstalled wfInstalled = new WorkFlowInstalled();
            var json = LitJson.JsonMapper.ToObject(jsonString);


            string id = json["id"].ToString();
            if (string.IsNullOrEmpty(id))
            {
                errMsg = "流程ID错误";
                return null;
            }
            else
            {
                wfInstalled.ID = id;
            }

            string name = json["name"].ToString();
            if (string.IsNullOrEmpty(name))
            {
                errMsg = "流程名称为空";
                return null;
            }
            else
            {
                wfInstalled.Name = name.Trim();
            }

            string type = json["type"].ToString();
            wfInstalled.Type = string.IsNullOrEmpty(type) ? new DictTypeBLL().GetIDByCode("FlowTypes") : type.Trim();


            string manager = json["manager"].ToString();
            if (string.IsNullOrEmpty(manager))
            {
                errMsg = "流程管理者为空";
                return null;
            }
            else
            {
                wfInstalled.Manager = manager;
            }

            string instanceManager = json["instanceManager"].ToString();
            if (string.IsNullOrEmpty(instanceManager))
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
            wfInstalled.DebugUsers = BLLFactory<DeptBLL>.Instance.GetAllUsers(json["debugUsers"].ToString());
            wfInstalled.Note = json["note"].ToString();

            List<Next.WorkFlow.Entity.WorkFlowInstalledSub.DataBases> dataBases = new List<Next.WorkFlow.Entity.WorkFlowInstalledSub.DataBases>();
            var dbs = json["databases"];
            if (dbs.IsArray)
            {
                foreach (LitJson.JsonData db in dbs)
                {
                    dataBases.Add(new Next.WorkFlow.Entity.WorkFlowInstalledSub.DataBases()
                    {
                        LinkID = db["link"].ToString(),
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
                    LinkID = titleField["link"].ToString(),
                    LinkName = "",
                    Table = titleField["table"].ToString()
                };
            }
            


            List<Next.WorkFlow.Entity.WorkFlowInstalledSub.Step> stepsList = new List<Next.WorkFlow.Entity.WorkFlowInstalledSub.Step>();
            LitJson.JsonData steps = json["steps"];
            if (steps.IsArray)
            {
                foreach (LitJson.JsonData step in steps)
                {

                    LitJson.JsonData behavior = step["behavior"];
                    Next.WorkFlow.Entity.WorkFlowInstalledSub.StepSet.Behavior behavior1 = new Next.WorkFlow.Entity.WorkFlowInstalledSub.StepSet.Behavior();
                    if (behavior.IsObject)
                    {
                        behavior1.BackModel = behavior["backModel"].ToString().ToInt();
                        behavior1.BackStepID = behavior["backStep"].ToString();
                        behavior1.BackType = behavior["backType"].ToString().ToInt();
                        behavior1.DefaultHandler = behavior["defaultHandler"].ToString();
                        behavior1.FlowType = behavior["flowType"].ToString().ToInt();
                        behavior1.HandlerStepID = behavior["handlerStep"].ToString();
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
                    

                    LitJson.JsonData buttons = step["buttons"];
                    List<Next.WorkFlow.Entity.WorkFlowInstalledSub.StepSet.Button> buttionList = new List<Next.WorkFlow.Entity.WorkFlowInstalledSub.StepSet.Button>();
                    if (buttons.IsArray)
                    {
                        foreach (LitJson.JsonData button in buttons)
                        {
                            string butID = button["id"].ToString();
                            if (butID.IsNullOrEmpty())
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
                    

                    LitJson.JsonData forms = step["forms"];
                    List<Next.WorkFlow.Entity.WorkFlowInstalledSub.StepSet.Form> formList = new List<Next.WorkFlow.Entity.WorkFlowInstalledSub.StepSet.Form>();
                    if (forms.IsArray)
                    {
                        foreach (LitJson.JsonData form in forms)
                        {
                            formList.Add(new Next.WorkFlow.Entity.WorkFlowInstalledSub.StepSet.Form()
                            {
                                ID = form["id"].ToString(),
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
                        ID = step["id"].ToString(),
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
                    

                }
            }
            wfInstalled.Steps = stepsList;
            if (stepsList.Count == 0)
            {
                errMsg = "流程至少需要一个步骤";
                return null;
            }
            



            List<Next.WorkFlow.Entity.WorkFlowInstalledSub.Line> linesList = new List<Next.WorkFlow.Entity.WorkFlowInstalledSub.Line>();
            LitJson.JsonData lines = json.ContainsKey("lines") ? json["lines"] : new LitJson.JsonData();
            if (lines.IsArray)
            {
                foreach (LitJson.JsonData line in lines)
                {
                    linesList.Add(new Next.WorkFlow.Entity.WorkFlowInstalledSub.Line()
                    {
                        ID = line["id"].ToString(),
                        FromID = line["from"].ToString(),
                        ToID = line["to"].ToString(),
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
                wfInstalled.InstallTime = DateTime.Now;
                wfInstalled.InstallUser = (string)HttpContext.Current.Session["UserID"];
                wfInstalled.RunJSON = jsonString;
                wfInstalled.Status = wf.Status;
            }
            

            return wfInstalled;
        }

        /// <summary>
        /// 得到下级ID字符串
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetAllChildsIDString(string id, bool isSelf = true)
        {
            return new DictTypeBLL().GetAllChildsIDString(id, true);
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
            DBConnectionBLL bdbconn = new DBConnectionBLL();
            DBConnection dbconn = bdbconn.FindByID(connID);
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
                        parList.Add(new System.Data.SqlClient.SqlParameter("@fieldvalue", fieldValue));
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

	}
}
