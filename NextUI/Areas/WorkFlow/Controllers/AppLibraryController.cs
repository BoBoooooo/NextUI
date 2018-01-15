using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Next.WorkFlow.Utility;
using Next.WorkFlow.Entity;
using Next.Controllers;
using Next.Framework.Core;
using Next.WorkFlow.BLL;
using Next.Admin.Entity;
using Next.Admin.BLL;

namespace NextUI.Areas.WorkFlow.Controllers
{
    public class AppLibraryController : BaseController
    {
        //
        // GET: /WorkFlow/AppLibrary/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Tree()
        {
            return View();
        }
        public string Install(string id)
        {
            string result = "failure";
            AppLibrary appLibrary = BLLFactory<AppLibraryBLL>.Instance.FindByID(id);
            var exist = BLLFactory<MenuBLL>.Instance.FindByID(id);
            var menu=new Menu();
            menu.ID=appLibrary.ID;
            menu.PID = "00000000-0000-0000-0000-000000000200";
            menu.Name = appLibrary.Title;
            menu.WebIcon = "icon-blank";
            menu.Url = "/"+appLibrary.Address + "?" + appLibrary.Params;
            menu.SystemTypeID = "00000000-0000-0000-0000-000000000100";
            menu.Deleted = false;
            if(exist!=null){
                BLLFactory<MenuBLL>.Instance.Update(menu);
            }else{
                BLLFactory<MenuBLL>.Instance.Insert(menu);
            }
            result = "success";
            //menu.ID=
            //var menu=BLLFactory<MenuBLL>.Instance
            return result;
        }
        public ActionResult List()
        {
            string title1 = Request.QueryString["title1"];
            string address = Request.QueryString["address"];
            return query(title1, address);
        }

        [HttpPost]
        public RedirectToRouteResult Delete()
        {
            Next.WorkFlow.BLL.AppLibraryBLL bappLibrary = new Next.WorkFlow.BLL.AppLibraryBLL();
            string deleteID = Request.Form["checkbox_app"];
            System.Text.StringBuilder delxml = new System.Text.StringBuilder();
            foreach (string id in deleteID.Split(','))
            {
                string gid;
                if (id.IsGuid(out gid))
                {
                    var app = bappLibrary.FindByID(gid);
                    if (app != null)
                    {
                        delxml.Append(app.Serialize());
                        bappLibrary.Delete(gid);
                    }
                }
            }
            //RoadFlow.Platform.Log.Add("删除了一批应用程序库", delxml.ToString(), RoadFlow.Platform.Log.Types.角色应用);
            return RedirectToAction("List", Next.WorkFlow.Utility.Tools.GetRouteValueDictionary());
        }

        [HttpPost]
        public ActionResult List(FormCollection collection)
        {
            string title1 = collection["title1"];
            string address = collection["address"];
            return query(title1, address);
        }

        private ActionResult query(string title1, string address)
        {
            string pager;
            string appid = Request.QueryString["appid"];
            string tabid = Request.QueryString["tabid"];
            string typeid = Request.QueryString["typeid"];
            Next.WorkFlow.BLL.DictBLL bdict = new Next.WorkFlow.BLL.DictBLL();
            Next.WorkFlow.BLL.AppLibraryBLL bapp = new Next.WorkFlow.BLL.AppLibraryBLL();
            string typeidstring = typeid.IsGuid() ? bapp.GetAllChildsIDString(typeid.ToGuid()) : "";
            string query = string.Format("&appid={0}&tabid={1}&title1={2}&typeid={3}&address={4}",
                        Request.QueryString["appid"],
                        Request.QueryString["tabid"],
                        title1.UrlEncode(), typeid, address.UrlEncode()
                        );
            string query1 = string.Format("{0}&pagesize={1}&pagenumber={2}", query, Request.QueryString["pagesize"], Request.QueryString["pagenumber"]);
            List<AppLibrary> appList = bapp.GetPagerData(out pager, query, title1, typeidstring, address);
            ViewBag.Pager = pager;
            ViewBag.AppID = appid;
            ViewBag.TabID = tabid;
            ViewBag.TypeID = typeid;
            ViewBag.Title1 = title1;
            ViewBag.Address = address;
            ViewBag.Query1 = query1;
            return View(appList);
        }

        public ActionResult Edit()
        {
            return Edit(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection collection)
        {
            string editID = Request.QueryString["id"];
            string type = Request.QueryString["typeid"];

            Next.WorkFlow.BLL.AppLibraryBLL bappLibrary = new Next.WorkFlow.BLL.AppLibraryBLL();
            AppLibrary appLibrary = null;
            if (editID.IsGuid())
            {
                appLibrary = bappLibrary.FindByID(editID.ToGuid());
            }
            bool isAdd = !editID.IsGuid();
            string oldXML = string.Empty;
            if (appLibrary == null)
            {
                appLibrary = new Next.WorkFlow.Entity.AppLibrary();
                appLibrary.ID = Guid.NewGuid().ToString();
                ViewBag.TypeOptions = new Next.WorkFlow.BLL.AppLibraryBLL().GetTypeOptions(type);
                ViewBag.OpenOptions = new Next.WorkFlow.BLL.DictBLL().GetOptionsByCode("appopenmodel", value: "");
            }
            else
            {
                oldXML = appLibrary.Serialize();
                ViewBag.TypeOptions = new Next.WorkFlow.BLL.AppLibraryBLL().GetTypeOptions(appLibrary.Type.ToString());
                ViewBag.OpenOptions = new Next.WorkFlow.BLL.DictBLL().GetOptionsByCode("appopenmodel", value: appLibrary.OpenMode.ToString());
            }

            if (collection != null)
            {
                string title = collection["title"];
                string address = collection["address"];
                string openModel = collection["openModel"];
                string width = collection["width"];
                string height = collection["height"];
                string params1 = collection["Params"];
                string note = collection["note"];
                string useMember = collection["UseMember"];
                type = collection["type"];

                appLibrary.Address = address.Trim();
                appLibrary.Height = height.ToIntOrNull();
                appLibrary.Note = note;
                appLibrary.OpenMode = openModel.ToInt();
                appLibrary.Params = params1;
                appLibrary.Title = title;
                appLibrary.Type = type.ToGuid();
                appLibrary.Width = width.ToIntOrNull();

                if (!useMember.IsNullOrEmpty())
                {
                    appLibrary.UseMember = useMember;
                }
                else
                {
                    appLibrary.UseMember = null;
                }

                if (isAdd)
                {
                    bappLibrary.Insert(appLibrary);
                    //RoadFlow.Platform.Log.Add("添加了应用程序库", appLibrary.Serialize(), RoadFlow.Platform.Log.Types.角色应用);
                    ViewBag.Script = "alert('添加成功!');new RoadUI.Window().reloadOpener();new RoadUI.Window().close();";
                }
                else
                {
                    bappLibrary.Update(appLibrary);
                    //RoadFlow.Platform.Log.Add("修改了应用程序库", "", RoadFlow.Platform.Log.Types.角色应用, oldXML, appLibrary.Serialize());
                    ViewBag.Script = "alert('修改成功!');new RoadUI.Window().reloadOpener();new RoadUI.Window().close();";
                }
                //bappLibrary.UpdateUseMemberCache(appLibrary.ID);
                //bappLibrary.ClearCache();
                //new RoadFlow.Platform.RoleApp().ClearAllDataTableCache();
            }
            return View(appLibrary);
        }
    }
}
