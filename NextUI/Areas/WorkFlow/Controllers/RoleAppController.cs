using Next.WorkFlow.BLL;
using Next.WorkFlow.Entity;
using Next.Attachment.BLL;
using Next.Attachment.Entity;
using Next.Controllers;
using Next.Framework.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Next.Framework.Core.Commons;
using Next.Commons;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Next.Admin.Entity;
using Next.Admin.BLL;
using Next.WorkFlow.Utility;

namespace Next.Areas.WorkFlow.Controllers
{
    public class RoleAppController : BusinessController<RoleAppBLL, RoleApp>
    {
        //
        // GET: /Jssjw/Loan/
        public ActionResult Index()
        {
            var roles = new RoleBLL().GetAll();
            if (!Request.Form["Search"].IsNullOrEmpty())
            {
                string name = Request.Form["Name"];
                if (!name.IsNullOrEmpty())
                {
                    roles = roles.Where(p => p.Name.Contains(name)).ToList();
                }
                ViewBag.Name = name;
            }
            return View(roles.OrderBy(p => p.Name));
        }

        public ActionResult SetApp()
        {
            return View();
        }

        public ActionResult Tree()
        {
            return View();
        }

        public string Tree1()
        {
            string roleID = Request.QueryString["roleid"];
            string roleGuid;
            if (!roleID.IsGuid(out roleGuid))
            {
                return "[]";
            }

            RoleAppBLL BRoleApp = new RoleAppBLL();
            var appDt = BRoleApp.GetAllDataTableByRoleID(roleGuid);
            if (appDt.Rows.Count == 0)
            {
                return "[]";
            }

            var root = appDt.Select("PID='" + string.Empty.ToString() + "'");
            if (root.Length == 0)
            {
                return "[]";
            }

            var apps = appDt.Select("PID='" + root[0]["ID"].ToString() + "'");
            StringBuilder json = new StringBuilder("[", 1000);
            System.Data.DataRow rootDr = root[0];
            json.Append("{");
            json.AppendFormat("\"id\":\"{0}\",", rootDr["ID"]);
            json.AppendFormat("\"title\":\"{0}\",", rootDr["Title"]);
            json.AppendFormat("\"ico\":\"{0}\",", rootDr["Ico"].ToString().IsNullOrEmpty() ? "" : Url.Content("~/" + rootDr["Ico"]));
            json.AppendFormat("\"link\":\"{0}\",", rootDr["Address"]);
            json.AppendFormat("\"type\":\"{0}\",", "0");
            json.AppendFormat("\"model\":\"{0}\",", rootDr["OpenMode"]);
            json.AppendFormat("\"width\":\"{0}\",", rootDr["Width"]);
            json.AppendFormat("\"height\":\"{0}\",", rootDr["Height"]);
            json.AppendFormat("\"hasChilds\":\"{0}\",", apps.Length > 0 ? "1" : "0");
            json.AppendFormat("\"childs\":[");

            for (int i = 0; i < apps.Length; i++)
            {
                System.Data.DataRow dr = apps[i];
                var childs = appDt.Select("PID='" + dr["ID"].ToString() + "'");
                json.Append("{");
                json.AppendFormat("\"id\":\"{0}\",", dr["ID"]);
                json.AppendFormat("\"title\":\"{0}\",", dr["Title"]);
                json.AppendFormat("\"ico\":\"{0}\",", dr["Ico"].ToString().IsNullOrEmpty() ? "" : Url.Content("~/" + dr["Ico"]));
                json.AppendFormat("\"link\":\"{0}\",", dr["Address"]);
                json.AppendFormat("\"type\":\"{0}\",", "0");
                json.AppendFormat("\"model\":\"{0}\",", dr["OpenMode"]);
                json.AppendFormat("\"width\":\"{0}\",", dr["Width"]);
                json.AppendFormat("\"height\":\"{0}\",", dr["Height"]);
                json.AppendFormat("\"hasChilds\":\"{0}\",", childs.Length > 0 ? "1" : "0");
                json.AppendFormat("\"childs\":[");
                json.Append("]");
                json.Append("}");
                if (i < apps.Length - 1)
                {
                    json.Append(",");
                }
            }
            json.Append("]");
            json.Append("}");
            json.Append("]");

            return json.ToString();
        }

        public string TreeRefresh()
        {
            string id = Request["refreshid"];
            if (!id.IsGuid())
            {
                return "[]";
            }
            RoleAppBLL BRoleApp = new RoleAppBLL();
            var childs = BRoleApp.GetChild(id.ToGuid());
            System.Text.StringBuilder json = new System.Text.StringBuilder("[", childs.Count * 50);
            int count = childs.Count;
            int i = 0;
            foreach (var child in childs)
            {
                json.Append("{");
                json.AppendFormat("\"id\":\"{0}\",", child.ID);
                json.AppendFormat("\"title\":\"{0}\",", child.Title);
                json.AppendFormat("\"ico\":\"{0}\",", child.Ico.IsNullOrEmpty() ? "" : Url.Content("~/" + child.Ico));
                json.AppendFormat("\"link\":\"{0}\",", "");
                json.AppendFormat("\"type\":\"{0}\",", "0");
                json.AppendFormat("\"model\":\"{0}\",", "");
                json.AppendFormat("\"width\":\"{0}\",", "");
                json.AppendFormat("\"height\":\"{0}\",", "");
                json.AppendFormat("\"hasChilds\":\"{0}\",", BRoleApp.HasChild(child.ID) ? "1" : "0");
                json.AppendFormat("\"childs\":[");
                json.Append("]");
                json.Append("}");
                if (i++ < count - 1)
                {
                    json.Append(",");
                }
            }
            json.Append("]");
            return json.ToString();
        }


        public ActionResult EditRole()
        {
            return EditRole(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRole(FormCollection collection)
        {
            RoleBLL brole = new RoleBLL();
            Role role = null;
            string roleID = Request.QueryString["roleid"];
            string roleGID;
            string name = string.Empty;
            string useMember = string.Empty;
            string note = string.Empty;

            if (roleID.IsGuid(out roleGID))
            {
                role = brole.FindByID(roleGID);
            }

            if (!Request.Form["Copy"].IsNullOrEmpty())
            {
                string tpl = Request.Form["ToTpl"];
                if (tpl.IsGuid())
                {
                    new RoleAppBLL().CopyRoleApp(roleGID, tpl.ToGuid());
                    //RoadFlow.Platform.Log.Add("复制了模板应用", "源：" + roleID + "复制给：" + tpl, RoadFlow.Platform.Log.Types.角色应用);
                    ViewBag.Script = "alert('复制成功!');";
                }
            }

            if (!Request.Form["Save"].IsNullOrEmpty() && role != null)
            {
                User_Role busersRole = new User_Role();
                using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope())
                {
                    name = Request.Form["Name"];
                    useMember = Request.Form["UseMember"];
                    note = Request.Form["Note"];

                    role.Name = name.Trim();
                    role.Note = note.IsNullOrEmpty() ? null : note.Trim();
                    role.UserID = useMember.IsNullOrEmpty() ? null : useMember;
                    brole.Update(role);
                    busersRole.DeleteByRoleID(role.ID);
                    if (!useMember.IsNullOrEmpty())
                    {
                        busersRole.DeleteByRoleID(role.ID);
                        var users = new DeptBLL().GetAllUsers(useMember);
                        foreach (var user in users)
                        {
                            RoadFlow.Data.Model.UsersRole ur = new RoadFlow.Data.Model.UsersRole();
                            ur.IsDefault = true;
                            ur.MemberID = user.ID;
                            ur.RoleID = role.ID;
                            busersRole.Insert(ur);
                        }
                    }
                    scope.Complete();
                }
                ViewBag.Script = "alert('保存成功!');new RoadUI.Window().reloadOpener();new RoadUI.Window().close();";
            }

            if (!Request.Form["Delete"].IsNullOrEmpty())
            {
                using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope())
                {
                    brole.Delete(roleGID);
                    new RoleAppBLL().DeleteByRoleID(roleGID);
                    new UsersRoleBLL().DeleteByRoleID(roleGID);
                    scope.Complete();
                }
                //RoadFlow.Platform.Log.Add("删除的角色其及相关数据", roleID, RoadFlow.Platform.Log.Types.角色应用);
                ViewBag.Script = "new RoadUI.Window().reloadOpener();new RoadUI.Window().close();";
            }
            ViewBag.RoleOptions = brole.GetRoleOptions("", roleID);
            return View(role);
        }


        public ActionResult AddRole()
        {
            return AddRole(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRole(FormCollection collection)
        {
            string name = Request.Form["Name"];
            string note = Request.Form["Note"];
            string useMember = Request.Form["UseMember"];
            Role role = new Role();
            if (collection != null)
            {
                using (System.Transactions.TransactionScope trans = new System.Transactions.TransactionScope())
                {
                    role.ID = Guid.NewGuid().ToString();
                    role.Name = name.Trim();
                    if (!useMember.IsNullOrEmpty())
                    {
                        role.UseMember = useMember;
                        RoadFlow.Platform.UsersRole busersRole = new RoadFlow.Platform.UsersRole();
                        var users = new DeptBLL().GetAllUsers(useMember);
                        foreach (var user in users)
                        {
                            RoadFlow.Data.Model.UsersRole ur = new RoadFlow.Data.Model.UsersRole();
                            ur.IsDefault = true;
                            ur.MemberID = user.ID;
                            ur.RoleID = role.ID;
                            busersRole.Insert(ur);
                        }
                    }
                    if (!note.IsNullOrEmpty())
                    {
                        role.Note = note.Trim();
                    }
                    new RoleBLL().Insert(role);

                    //添加一个根应用
                    RoleApp roleApp = new RoleApp();
                    roleApp.ID = Guid.NewGuid().ToString();
                    roleApp.PID = string.Empty;
                    roleApp.RoleID = role.ID;
                    roleApp.Sort = 1;
                    roleApp.Title = "管理目录";
                    new RoleAppBLL().Insert(roleApp);
                    trans.Complete();
                    ViewBag.Script = "alert('添加成功!');new RoadUI.Window().reloadOpener();new RoadUI.Window().close();";
                }
            }
            return View(role);
        }

        public ActionResult Empty()
        {
            return null;
        }

        public ActionResult Body()
        {
            return Body(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Body(FormCollection collection)
        {
            AppLibraryBLL bappLibrary = new AppLibraryBLL();
            RoleAppBLL broleApp = new RoleAppBLL();
            RoleApp roleApp = null;
            string id = Request.QueryString["id"];
            string name = string.Empty;
            string type = string.Empty;
            string appid = string.Empty;
            string params1 = string.Empty;
            string ico = string.Empty;
            string appID;
            if (id.IsGuid(out appID))
            {
                roleApp = broleApp.FindByID(appID);
            }

            if (!Request.Form["Save"].IsNullOrEmpty())
            {
                name = Request.Form["Name"];
                type = Request.Form["Type"];
                appid = Request.Form["AppID"];
                params1 = Request.Form["Params"];
                ico = Request.Form["Ico"];

                string oldXML = roleApp.Serialize();
                roleApp.Title = name.Trim();
                if (appid.IsGuid())
                {
                    roleApp.AppID = appid.ToGuid();
                }
                else
                {
                    roleApp.AppID = null;
                }
                roleApp.Params = params1.IsNullOrEmpty() ? null : params1.Trim();
                if (!ico.IsNullOrEmpty())
                {
                    roleApp.Ico = ico;
                }
                else
                {
                    roleApp.Ico = null;
                }

                broleApp.Update(roleApp);
                //broleApp.ClearAllDataTableCache();
                //RoadFlow.Platform.Log.Add("修改了应用模板", "", RoadFlow.Platform.Log.Types.角色应用, oldXML, roleApp.Serialize());
                string refreshID = roleApp.PID == string.Empty ? roleApp.ID.ToString() : roleApp.PID.ToString();
                ViewBag.Script = "parent.frames[0].reLoad('" + refreshID + "');alert('保存成功!');";
            }

            if (!Request.Form["Delete"].IsNullOrEmpty())
            {
                int i = broleApp.DeleteAndAllChilds(roleApp.ID);
                //broleApp.ClearAllDataTableCache();
                //RoadFlow.Platform.Log.Add("删除了模板及其所有下级共" + i.ToString() + "项", roleApp.Serialize(), RoadFlow.Platform.Log.Types.角色应用);
                string refreshID = roleApp.PID == string.Empty ? roleApp.ID.ToString() : roleApp.PID.ToString();
                ViewBag.Script = "parent.frames[0].reLoad('" + refreshID + "');window.location='Body?id=" + refreshID + "&appid=" + Request.QueryString["appid"] + "&tabid=" + Request.QueryString["tabid"] + "';";
            }
            if (roleApp != null)// && roleApp.AppID.HasValue)
            {
                var app = new AppLibraryBLL().FindByID(roleApp.AppID);//.Value);
                if (app != null)
                {
                    type = app.Type.ToString();
                }
            }
            ViewBag.AppTypesOptions = bappLibrary.GetTypeOptions(type);
            ViewBag.AppID = roleApp.AppID.ToString();
            return View(roleApp);
        }

        public string GetApps()
        {
            string type = Request.Form["type"];
            string appid = Request.Form["value"];
            return new AppLibraryBLL().GetAppsOptions(type.ToGuid(), appid);
        }


        public ActionResult AddApp()
        {
            return AddApp(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddApp(FormCollection collection)
        {
            AppLibraryBLL bappLibrary = new AppLibraryBLL();
            RoleAppBLL broleApp = new RoleAppBLL();
            RoleApp roleApp = null;

            string id = Request.QueryString["id"];

            if (collection != null)
            {
                roleApp = broleApp.FindByID(id.ToGuid());
                if (!Request.Form["Save"].IsNullOrEmpty())
                {
                    string name = Request.Form["Name"];
                    string type = Request.Form["Type"];
                    string appid = Request.Form["AppID"];
                    string params1 = Request.Form["Params"];
                    string ico = Request.Form["Ico"];

                    RoleApp roleApp1 = new RoleApp();

                    roleApp1.ID = Guid.NewGuid().ToString();
                    roleApp1.PID = id.ToGuid();
                    roleApp1.RoleID = roleApp.RoleID;
                    roleApp1.Title = name.Trim();
                    roleApp1.PID = roleApp.ID;
                    roleApp1.Sort = broleApp.GetMaxSort(roleApp.ID);
                    roleApp1.Type = 0;
                    if (appid.IsGuid())
                    {
                        roleApp1.AppID = appid.ToGuid();
                    }
                    else
                    {
                        roleApp1.AppID = null;
                    }
                    roleApp1.Params = params1.IsNullOrEmpty() ? null : params1.Trim();
                    if (!ico.IsNullOrEmpty())
                    {
                        roleApp1.Ico = ico;
                    }

                    broleApp.Insert(roleApp1);
                    //broleApp.ClearAllDataTableCache();
                    //RoadFlow.Platform.Log.Add("添加了应用模板", roleApp1.Serialize(), RoadFlow.Platform.Log.Types.角色应用);
                    string refreshID = id;
                    ViewBag.Script = "alert('添加成功');parent.frames[0].reLoad('" + refreshID + "');";
                }

            }

            ViewBag.AppTypesOptions = bappLibrary.GetTypeOptions();
            return View();
        }

        public ActionResult Sort()
        {
            return Sort(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sort(FormCollection collection)
        {
            RoleAppBLL broleApp = new RoleAppBLL();
            List<RoleApp> roleAppList = new List<RoleApp>();
            string id = Request.QueryString["id"];
            var roleApp = broleApp.FindByID(id.ToGuid());
            roleAppList = broleApp.GetChild(roleApp.PID);

            if (collection != null)
            {
                string srots = Request.Form["sortapp"];
                if (srots.IsNullOrEmpty())
                {
                    return View(roleAppList);
                }
                string[] sortArray = srots.Split(new char[] { ',' });
                for (int i = 0; i < sortArray.Length; i++)
                {
                    string guid;
                    if (!sortArray[i].IsGuid(out guid))
                    {
                        continue;
                    }
                    broleApp.UpdateSort(guid, i + 1);
                }
                //broleApp.ClearAllDataTableCache();
                string rid = roleApp.PID.ToString();
                ViewBag.Script = "parent.frames[0].reLoad('" + rid + "');";
                roleAppList = broleApp.GetChild(roleApp.PID);
            }
            return View(roleAppList);
        }

    }
}
