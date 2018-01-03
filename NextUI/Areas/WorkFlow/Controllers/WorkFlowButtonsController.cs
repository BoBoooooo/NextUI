using Next.Controllers;
using Next.WorkFlow.BLL;
using Next.WorkFlow.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Next.WorkFlow.Utility;
namespace NextUI.Areas.WorkFlow.Controllers
{
    public class WorkFlowButtonsController : BaseController
    {
        //
        // GET: /WorkFlow/WorkFlowButtons/
        //


        public ActionResult Index()
        {
            return Index(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FormCollection collection)
        {
            string name = string.Empty;
            ViewBag.Query1 = string.Format("&appid={0}&tabid={1}", Request.QueryString["appid"], Request.QueryString["tabid"]);
            WorkFlowButtonsBLL bworkFlowButtons = new WorkFlowButtonsBLL();
            IEnumerable<WorkFlowButtons> workFlowButtonsList;

            if (collection != null)
            {
                if (!Request.Form["DeleteBut"].IsNullOrEmpty())
                {
                    string ids = Request.Form["checkbox_app"];
                    foreach (string id in ids.Split(','))
                    {
                        string bid;
                        if (id.IsNullOrEmpty())
                        {
                            continue;
                        }
                        bid=id;
                        var but = bworkFlowButtons.FindByID(bid);
                        if (but != null)
                        {
                            bworkFlowButtons.Delete(bid);
                            //RoadFlow.Platform.Log.Add("删除了流程按钮", but.Serialize(), RoadFlow.Platform.Log.Types.流程相关);
                        }
                    }
                    //bworkFlowButtons.ClearCache();

                }
                workFlowButtonsList = bworkFlowButtons.GetAll();

                if (!Request.Form["Search"].IsNullOrEmpty())
                {
                    name = Request.Form["Name"];
                    if (!name.IsNullOrEmpty())
                    {
                        workFlowButtonsList = workFlowButtonsList.Where(p => p.Title.IndexOf(name) >= 0);
                    }
                }
            }
            else
            {
                workFlowButtonsList = bworkFlowButtons.GetAll();
            }
            ViewBag.Name = name;
            return View(workFlowButtonsList);
        }


        public ActionResult Edit()
        {
            return Edit(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection collection)
        {
            WorkFlowButtonsBLL bworkFlowButtons = new WorkFlowButtonsBLL();
            WorkFlowButtons workFlowButton = null;
            string id = Request.QueryString["id"];

            string title = string.Empty;
            string ico = string.Empty;
            string script = string.Empty;
            string note = string.Empty;

            string buttionID;
            if (id.IsNullOrEmpty(out buttionID))
            {
                workFlowButton = bworkFlowButtons.FindByID(buttionID);
            }
            string oldXML = workFlowButton.Serialize();
            if (collection != null)
            {
                title = Request.Form["Title"];
                ico = Request.Form["Ico"];
                script = Request.Form["Script"];
                note = Request.Form["Note"];

                bool isAdd = !id.IsGuid();
                if (workFlowButton == null)
                {
                    workFlowButton = new WorkFlowButtons();
                    workFlowButton.ID = Guid.NewGuid().ToString();
                    workFlowButton.Sort = bworkFlowButtons.GetMaxSort();
                }

                workFlowButton.Ico = ico.IsNullOrEmpty() ? null : ico.Trim();
                workFlowButton.Note = note.IsNullOrEmpty() ? null : note.Trim();
                workFlowButton.Script = script.IsNullOrEmpty() ? null : script;
                workFlowButton.Title = title.Trim();

                if (isAdd)
                {
                    bworkFlowButtons.Add(workFlowButton);
                    RoadFlow.Platform.Log.Add("添加了流程按钮", workFlowButton.Serialize(), RoadFlow.Platform.Log.Types.流程相关);
                }
                else
                {
                    bworkFlowButtons.Update(workFlowButton);
                    RoadFlow.Platform.Log.Add("修改了流程按钮", "", RoadFlow.Platform.Log.Types.流程相关, oldXML, workFlowButton.Serialize());
                }
                bworkFlowButtons.ClearCache();
                ViewBag.Script = "new RoadUI.Window().reloadOpener();alert('保存成功!');new RoadUI.Window().close();";
            }
            return View(workFlowButton == null ? new RoadFlow.Data.Model.WorkFlowButtons() : workFlowButton);
        }