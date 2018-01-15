using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Next.WorkFlow.Utility;
using Next.WorkFlow.Entity;
using Next.WorkFlow.Utility;
using Next.Controllers;

namespace NextUI.Areas.WorkFlow.Controllers
{
    public class WorkFlowDelegationController : BaseController
    {
        //
        // GET: /WorkFlow/WorkFlowDelegation/
        //
        public ActionResult Index()
        {
            return Index(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FormCollection collection)
        {
            Next.WorkFlow.BLL.WorkFlowDelegationBLL bworkFlowDelegation = new Next.WorkFlow.BLL.WorkFlowDelegationBLL();
            Next.Admin.BLL.DeptBLL borganize = new Next.Admin.BLL.DeptBLL();
            Next.Admin.BLL.UserBLL busers = new Next.Admin.BLL.UserBLL();
            Next.WorkFlow.BLL.WorkFlowInfoBLL bworkFlow = new Next.WorkFlow.BLL.WorkFlowInfoBLL();
            IEnumerable<WorkFlowDelegation> workFlowDelegationList;

            string startTime = string.Empty;
            string endTime = string.Empty;
            string suserid = string.Empty;
            string query1 = string.Format("&appid={0}&tabid={1}&isoneself={2}", Request.QueryString["appid"], Request.QueryString["tabid"], Request.QueryString["isoneself"]);
            if (collection != null)
            {
                if (!Request.Form["DeleteBut"].IsNullOrEmpty())
                {
                    string ids = Request.Form["checkbox_app"];
                    foreach (string id in ids.Split(','))
                    {
                        string bid;
                        if (!id.IsGuid(out bid))
                        {
                            continue;
                        }
                        var comment = bworkFlowDelegation.FindByID(bid);
                        if (comment != null)
                        {
                            bworkFlowDelegation.Delete(bid);
                            //RoadFlow.Platform.Log.Add("删除了流程意见", comment.Serialize(), RoadFlow.Platform.Log.Types.流程相关);
                        }
                    }
                    //bworkFlowDelegation.RefreshCache();
                }
                startTime = Request.Form["S_StartTime"];
                endTime = Request.Form["S_EndTime"];
                suserid = Request.Form["S_UserID"];
            }
            else
            {
                startTime = Request.QueryString["S_StartTime"];
                endTime = Request.QueryString["S_EndTime"];
                suserid = Request.QueryString["S_UserID"];
            }
            query1 += "&S_StartTime=" + startTime + "&S_EndTime=" + endTime + "&S_UserID=" + suserid;
            string pager;
            bool isOneSelf = "1" == Request.QueryString["isoneself"];
            if (isOneSelf)
            {
                workFlowDelegationList = bworkFlowDelegation.GetPagerData(out pager, query1, new Next.Admin.BLL.UserBLL().CurrentUserID.ToString(), startTime, endTime);
            }
            else
            {
                workFlowDelegationList = bworkFlowDelegation.GetPagerData(out pager, query1, Next.Admin.BLL.UserBLL.RemovePrefix(suserid), startTime, endTime);
            }
            ViewBag.Query1 = query1;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            ViewBag.suserid = suserid;
            return View(workFlowDelegationList);
        }

        public ActionResult Edit()
        {
            return Edit(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection collection)
        {
            Next.WorkFlow.BLL.WorkFlowDelegationBLL bworkFlowDelegation = new Next.WorkFlow.BLL.WorkFlowDelegationBLL();
            WorkFlowDelegation workFlowDelegation = null;
            string id = Request.QueryString["id"];

            string UserID = string.Empty;
            string ToUserID = string.Empty;
            string StartTime = string.Empty;
            string EndTime = string.Empty;
            string FlowID = string.Empty;
            string Note = string.Empty;

            bool isOneSelf = "1" == Request.QueryString["isoneself"];

            string delegationID;
            if (id.IsGuid(out delegationID))
            {
                workFlowDelegation = bworkFlowDelegation.FindByID(delegationID);
                if (workFlowDelegation != null)
                {
                    FlowID = workFlowDelegation.FlowID.ToString();
                }
            }
            string oldXML = workFlowDelegation.Serialize();

            if (collection != null)
            {
                UserID = Request.Form["UserID"];
                ToUserID = Request.Form["ToUserID"];
                StartTime = Request.Form["StartTime"];
                EndTime = Request.Form["EndTime"];
                FlowID = Request.Form["FlowID"];
                Note = Request.Form["Note"];

                bool isAdd = !id.IsGuid();
                if (workFlowDelegation == null)
                {
                    workFlowDelegation = new WorkFlowDelegation();
                    workFlowDelegation.ID = Guid.NewGuid().ToString();
                }
                workFlowDelegation.UserID = isOneSelf ? new Next.Admin.BLL.UserBLL().CurrentUserID : Next.Admin.BLL.UserBLL.RemovePrefix(UserID).ToGuid();
                workFlowDelegation.EndTime = EndTime.ToDateTime();
                if (FlowID.IsGuid())
                {
                    workFlowDelegation.FlowID = FlowID.ToGuid();
                }
                workFlowDelegation.Note = Note.IsNullOrEmpty() ? null : Note;
                workFlowDelegation.StartTime = StartTime.ToDateTime();
                workFlowDelegation.ToUserID = Next.Admin.BLL.UserBLL.RemovePrefix(ToUserID).ToGuid();
                workFlowDelegation.WriteTime = Next.WorkFlow.Utility.DateTimeNew.Now;



                if (isAdd)
                {
                    bworkFlowDelegation.Insert(workFlowDelegation);
                    //RoadFlow.Platform.Log.Add("添加了工作委托", workFlowDelegation.Serialize(), RoadFlow.Platform.Log.Types.流程相关);
                }
                else
                {
                    bworkFlowDelegation.Update(workFlowDelegation);
                    //RoadFlow.Platform.Log.Add("修改了工作委托", "", RoadFlow.Platform.Log.Types.流程相关, oldXML, workFlowDelegation.Serialize());
                }
                //bworkFlowDelegation.RefreshCache();
                ViewBag.Script = "alert('保存成功!');new RoadUI.Window().reloadOpener();new RoadUI.Window().close();";
            }
            ViewBag.FlowOptions = new Next.WorkFlow.BLL.WorkFlowInfoBLL().GetOptions(FlowID);
            return View(workFlowDelegation == null ? new WorkFlowDelegation() { UserID = new Next.Admin.BLL.UserBLL().CurrentUserID } : workFlowDelegation);
        }

    }
}
