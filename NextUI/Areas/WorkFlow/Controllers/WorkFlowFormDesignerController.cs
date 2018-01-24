using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Next.WorkFlow.Utility;
using Next.WorkFlow.BLL;
using Next.WorkFlow.Entity;
using System.IO;
using Next.Admin.BLL;
using System.Web;
using Next.Admin.Entity;
using Next.Controllers;

namespace NextUI.Areas.WorkFlow.Controllers
{
    public class WorkFlowFormDesignerController : BaseController
    {
        //
        // GET: /WorkFlow/WorkFlowFormDesigner/
        public ActionResult Index()
        {
            return View();
        }

        public string GetHtml()
        {
            string id = Request["id"];
            string gid;
            if (!id.IsGuid(out gid))
            {
                return "";
            }

            var wff = new WorkFlowFormBLL().FindByID(gid);
            if (wff == null)
            {
                return "";
            }
            else
            {
                return wff.Html;
            }
        }

        public string GetAttribute()
        {
            string id = Request["id"];
            string gid;
            if (!id.IsGuid(out gid))
            {
                return "";
            }

            var wff = new WorkFlowFormBLL().FindByID(gid);
            if (wff == null)
            {
                return "";
            }
            else
            {
                return wff.Attribute;
            }
        }

        public string Getsubtable()
        {
            string id = Request["id"];
            string gid;
            if (!id.IsGuid(out gid))
            {
                return "";
            }

            var wff = new WorkFlowFormBLL().FindByID(gid);
            if (wff == null)
            {
                return "";
            }
            else
            {
                return wff.SubTableJson;
            }
        }

        public string GetEvents()
        {
            string id = Request["id"];
            string gid;
            if (!id.IsGuid(out gid))
            {
                return "";
            }

            var wff = new WorkFlowFormBLL().FindByID(gid);
            if (wff == null)
            {
                return "";
            }
            else
            {
                return wff.EventsJson;
            }
        }

        public string TestSql()
        {
            string sql = Request["sql"];
            string dbconn = Request["dbconn"];

            if (sql.IsNullOrEmpty() || !dbconn.IsGuid())
            {
                return "SQL语句为空或未设置数据连接";
            }

            DBConnectionBLL bdbconn = new DBConnectionBLL();
            var dbconn1 = bdbconn.FindByID(dbconn.ToGuid());
            if (bdbconn.TestSql(dbconn1, sql))
            {
                return "SQL语句测试正确";
            }
            else
            {
                return "SQL语句测试错误";
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public string Save()
        {
            string html = Request["html"];
            string name = Request["name"];
            string att = Request["att"];
            string id = Request["id"];
            string type = Request["type"];
            string subtable = Request["subtable"];
            string formEvents = Request["formEvents"];

            if (name.IsNullOrEmpty())
            {
                return "表单名称不能为空!";
            }

            string formID;
            if (!id.IsGuid(out formID))
            {
                return "表单ID无效!";
            }

            WorkFlowFormBLL WFF = new WorkFlowFormBLL();
            WorkFlowForm wff = WFF.FindByID(formID);
            bool isAdd = false;
            string oldXML = string.Empty;
            if (wff == null)
            {
                wff = new WorkFlowForm();
                wff.ID = formID;
                wff.CreateUserID = ((User)Session["UserInfo"]).ID; ;
                wff.CreateUserName = ((User)Session["UserInfo"]).Name;
                wff.CreateTime = DateTimeNew.Now;
                wff.Status = 0;
                isAdd = true;
            }
            else
            {
                oldXML = wff.Serialize();
            }

            wff.Attribute = att;
            wff.Html = html;
            wff.LastModifyTime = Next.WorkFlow.Utility.DateTimeNew.Now;
            wff.Name = name;
            wff.Type = type;
            wff.SubTableJson = subtable;
            wff.EventsJson = formEvents;

            if (isAdd)
            {
                WFF.Insert(wff);
                //RoadFlow.Platform.Log.Add("添加了流程表单", wff.Serialize(), RoadFlow.Platform.Log.Types.流程相关);
            }
            else
            {
                WFF.Update(wff);
                //RoadFlow.Platform.Log.Add("修改了流程表单", "", RoadFlow.Platform.Log.Types.流程相关, oldXML, wff.Serialize());
            }
            return "保存成功!";
        }

        public string GetSubTableData()
        {
            string secondtable = Request["secondtable"];
            string primarytablefiled = Request["primarytablefiled"];
            string secondtableprimarykey = Request["secondtableprimarykey"];
            string primarytablefiledvalue = Request["primarytablefiledvalue"];
            string secondtablerelationfield = Request["secondtablerelationfield"];
            string dbconnid = Request["dbconnid"];
            LitJson.JsonData data = new WorkFlowInfoBLL().GetSubTableData(dbconnid, secondtable, secondtablerelationfield, primarytablefiledvalue, secondtableprimarykey);
            return data.ToJson();
        }

        [HttpPost]
        [ValidateInput(false)]
        public string Publish()
        {
            string html = Request["html"];
            string name = Request["name"];
            string att = Request["att"];
            string id = Request["id"];

            string gid;
            if (!id.IsGuid(out gid) || name.IsNullOrEmpty() || att.IsNullOrEmpty() || html.IsNullOrEmpty())
            {
                return "参数错误!";
            }
            WorkFlowFormBLL WFF = new WorkFlowFormBLL();

            WorkFlowForm wff = WFF.FindByID(gid);
            if (wff == null)
            {
                return "未找到表单!";
            }

            string fileName = id + ".cshtml";

            System.Text.StringBuilder serverScript = new System.Text.StringBuilder("@using Next.WorkFlow.Utility\r\n");

            serverScript.Append("@{\r\n");
            var attrJSON = LitJson.JsonMapper.ToObject(att);
            serverScript.Append("\tstring FlowID = Request.QueryString[\"flowid\"];\r\n");
            serverScript.Append("\tstring StepID = Request.QueryString[\"stepid\"];\r\n");
            serverScript.Append("\tstring GroupID = Request.QueryString[\"groupid\"];\r\n");
            serverScript.Append("\tstring TaskID = Request.QueryString[\"taskid\"];\r\n");
            serverScript.Append("\tstring InstanceID = Request.QueryString[\"instanceid\"];\r\n");
            serverScript.Append("\tstring DisplayModel = Request.QueryString[\"display\"] ?? \"0\";\r\n");
            serverScript.AppendFormat("\tstring DBConnID = \"{0}\";\r\n", attrJSON["dbconn"].ToString());
            serverScript.AppendFormat("\tstring DBTable = \"{0}\";\r\n", attrJSON["dbtable"].ToString());
            serverScript.AppendFormat("\tstring DBTablePK = \"{0}\";\r\n", attrJSON["dbtablepk"].ToString());
            serverScript.AppendFormat("\tstring DBTableTitle = \"{0}\";\r\n", attrJSON["dbtabletitle"].ToString());
            serverScript.Append("if(InstanceID.IsNullOrEmpty()){InstanceID = Request.QueryString[\"instanceid1\"];}");

            serverScript.Append("\tNext.WorkFlow.BLL.DictBLL BDictionary = new Next.WorkFlow.BLL.DictBLL();\r\n");
            serverScript.Append("\tNext.WorkFlow.BLL.WorkFlowInfoBLL BWorkFlow = new Next.WorkFlow.BLL.WorkFlowInfoBLL();\r\n");
            serverScript.Append("\tNext.WorkFlow.BLL.WorkFlowTaskBLL BWorkFlowTask = new Next.WorkFlow.BLL.WorkFlowTaskBLL();\r\n");
            serverScript.Append("\tstring fieldStatus = BWorkFlow.GetFieldStatus(FlowID, StepID);\r\n");
            serverScript.Append("\tLitJson.JsonData initData = BWorkFlow.GetFormData(DBConnID, DBTable, DBTablePK, InstanceID, fieldStatus);\r\n");
            serverScript.Append("\tstring TaskTitle = BWorkFlow.GetFromFieldData(initData, DBTable, DBTableTitle);\r\n");

            serverScript.Append("}\r\n");
            serverScript.Append("<link href=\"~/Assets/WorkFlow/Scripts/FlowRun/Forms/flowform.css\" rel=\"stylesheet\" type=\"text/css\" />\r\n");
            serverScript.Append("<script src=\"~/Assets/WorkFlow/Scripts/FlowRun/Forms/common.js\" type=\"text/javascript\" ></script>\r\n");

            if (attrJSON.ContainsKey("hasEditor") && "1" == attrJSON["hasEditor"].ToString())
            {
                serverScript.Append("<script src=\"~/Assets/WorkFlow/Scripts/Ueditor/ueditor.config.js\" type=\"text/javascript\" ></script>\r\n");
                serverScript.Append("<script src=\"~/Assets/WorkFlow/Scripts/Ueditor/ueditor.all.min.js\" type=\"text/javascript\" ></script>\r\n");
                serverScript.Append("<script src=\"~/Assets/WorkFlow/Scripts/Ueditor/lang/zh-cn/zh-cn.js\" type=\"text/javascript\" ></script>\r\n");
                serverScript.Append("<input type=\"hidden\" id=\"Form_HasUEditor\" name=\"Form_HasUEditor\" value=\"1\" />\r\n");
            }
            string validatePropType = attrJSON.ContainsKey("validatealerttype") ? attrJSON["validatealerttype"].ToString() : "2";
            serverScript.Append("<input type=\"hidden\" id=\"Form_ValidateAlertType\" name=\"Form_ValidateAlertType\" value=\"" + validatePropType + "\" />\r\n");
            if (attrJSON.ContainsKey("autotitle") && attrJSON["autotitle"].ToString().ToLower() == "true")
            {
                serverScript.AppendFormat("<input type=\"hidden\" id=\"{0}\" name=\"{0}\" value=\"{1}\" />\r\n",
                    string.Concat(attrJSON["dbtable"].ToString(), ".", attrJSON["dbtabletitle"].ToString()),
                    "@(TaskTitle.IsNullOrEmpty() ? BWorkFlow.GetAutoTaskTitle(FlowID, StepID, Request.QueryString[\"groupid\"]) : TaskTitle)"
                    );
            }
            serverScript.AppendFormat("<input type=\"hidden\" id=\"Form_TitleField\" name=\"Form_TitleField\" value=\"{0}\" />\r\n", string.Concat(attrJSON["dbtable"].ToString(), ".", attrJSON["dbtabletitle"].ToString()));
            //serverScript.AppendFormat("<input type=\"hidden\" id=\"Form_Name\" name=\"Form_Name\" value=\"{0}\" />\r\n", attrJSON["name"].ToString());
            serverScript.AppendFormat("<input type=\"hidden\" id=\"Form_DBConnID\" name=\"Form_DBConnID\" value=\"{0}\" />\r\n", attrJSON["dbconn"].ToString());
            serverScript.AppendFormat("<input type=\"hidden\" id=\"Form_DBTable\" name=\"Form_DBTable\" value=\"{0}\" />\r\n", attrJSON["dbtable"].ToString());
            serverScript.AppendFormat("<input type=\"hidden\" id=\"Form_DBTablePk\" name=\"Form_DBTablePk\" value=\"{0}\" />\r\n", attrJSON["dbtablepk"].ToString());
            serverScript.AppendFormat("<input type=\"hidden\" id=\"Form_DBTableTitle\" name=\"Form_DBTableTitle\" value=\"{0}\" />\r\n", attrJSON["dbtabletitle"].ToString());
            serverScript.AppendFormat("<input type=\"hidden\" id=\"Form_AutoSaveData\" name=\"Form_AutoSaveData\" value=\"{0}\" />\r\n", "1");
            serverScript.Append("<script type=\"text/javascript\">\r\n");
            serverScript.Append("\tvar initData = @Html.Raw(BWorkFlow.GetFormDataJsonString(initData));\r\n");
            serverScript.Append("\tvar fieldStatus = \"1\"==\"@Request.QueryString[\"isreadonly\"]\" ? {} : @Html.Raw(fieldStatus);\r\n");
            serverScript.Append("\tvar displayModel = '@DisplayModel';\r\n");
            serverScript.Append("\t$(window).load(function (){\r\n");
            serverScript.AppendFormat("\t\tformrun.initData(initData, \"{0}\", fieldStatus, displayModel);\r\n", attrJSON["dbtable"].ToString());
            serverScript.Append("\t});\r\n");
            serverScript.Append("</script>\r\n");


            string file = Server.MapPath("~/Areas/Workflow/Views/WorkFlowFormDesigner/Forms/" + fileName);
            System.IO.Stream stream = System.IO.File.Open(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            stream.SetLength(0);

            StreamWriter sw = new StreamWriter(stream, System.Text.Encoding.UTF8);
            sw.Write(serverScript.ToString());
            string temp = Server.HtmlDecode(html);
            string content = HtmlCodeFormat.Format(temp);
            sw.Write(content);

            sw.Close();
            stream.Close();


            string attr = wff.Attribute;
            string appType = LitJson.JsonMapper.ToObject(attr)["apptype"].ToString();
            AppLibraryBLL App = new AppLibraryBLL();
            var app = App.GetByCode(id);
            bool isAdd = false;
            if (app == null)
            {
                app = new AppLibrary();
                app.ID = Guid.NewGuid().ToString();
                app.Code = id;
                isAdd = true;
            }
            app.Address = "/Areas/WorkFlow/Views/WorkFlowFormDesigner/Forms/" + fileName;
            app.Note = "流程表单";
            app.OpenMode = 0;
            app.Params = "";
            app.Title = name.Trim();
            app.Type = appType.IsGuid() ? appType : new DictBLL().GetIDByCode("FormTypes");
            if (isAdd)
            {
                App.Insert(app);
            }
            else
            {
                App.Update(app);
            }

            //RoadFlow.Platform.Log.Add("发布了流程表单", app.Serialize() + "内容：" + html, RoadFlow.Platform.Log.Types.流程相关);
            wff.Status = 1;
            WFF.Update(wff);
            return "发布成功!";
        }
    }
}
