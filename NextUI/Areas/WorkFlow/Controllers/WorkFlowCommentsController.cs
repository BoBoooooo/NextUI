using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Next.WorkFlow.Utility;
using Next.WorkFlow.Entity;
using Next.Controllers;
namespace NextUI.Areas.WorkFlow.Controllers
{
    public class WorkFlowCommentsController : BaseController
    {
        //
        // GET: /WorkFlow/WorkFlowComments/
        public ActionResult Index()
        {
            return Index(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FormCollection collection)
        {
            Next.WorkFlow.BLL.WorkFlowCommentBLL bworkFlowComment = new Next.WorkFlow.BLL.WorkFlowCommentBLL();
            Next.Admin.BLL.DeptBLL borganize = new Next.Admin.BLL.DeptBLL();
            IEnumerable<WorkFlowComment> workFlowCommentList;

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
                        var comment = bworkFlowComment.FindByID(bid);
                        if (comment != null)
                        {
                            bworkFlowComment.Delete(bid);
                            //Next.WorkFlow.BLL.Log.Add("删除了流程意见", comment.Serialize(), Next.WorkFlow.BLL.Log.Types.流程相关);
                        }
                    }
                    //bworkFlowComment.RefreshCache();
                }

            }

            workFlowCommentList = bworkFlowComment.GetAll();

            bool isOneSelf = "1" == Request.QueryString["isoneself"];
            if (isOneSelf)
            {
                workFlowCommentList = workFlowCommentList.Where(p => p.MemberID == Next.Admin.BLL.UserBLL.PREFIX + new Next.Admin.BLL.UserBLL().CurrentUserID.ToString());
            }
            return View(workFlowCommentList);
        }


        public ActionResult Edit()
        {
            return Edit(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection collection)
        {
            Next.WorkFlow.BLL.WorkFlowCommentBLL bworkFlowComment = new Next.WorkFlow.BLL.WorkFlowCommentBLL();
            WorkFlowComment workFlowComment = null;
            string id = Request.QueryString["id"];

            string member = string.Empty;
            string comment = string.Empty;
            string sort = string.Empty;

            bool isOneSelf = "1" == Request.QueryString["isoneself"];

            string commentID;
            if (id.IsGuid(out commentID))
            {
                workFlowComment = bworkFlowComment.FindByID(commentID);
                member = workFlowComment.MemberID;
                comment = workFlowComment.Comment;
                sort = workFlowComment.Sort.ToString();
            }
            string oldXML = workFlowComment.Serialize();
            if (collection != null)
            {
                member = isOneSelf ? Next.Admin.BLL.UserBLL.PREFIX + new Next.Admin.BLL.UserBLL().CurrentUserID.ToString() : Request.Form["Member"];
                comment = Request.Form["Comment"];
                sort = Request.Form["Sort"];

                bool isAdd = !id.IsGuid();
                if (workFlowComment == null)
                {
                    workFlowComment = new WorkFlowComment();
                    workFlowComment.ID = Guid.NewGuid().ToString();
                    workFlowComment.Type = isOneSelf ? 1 : 0;
                }

                workFlowComment.MemberID = member.IsNullOrEmpty() ? "" : member.Trim();
                workFlowComment.Comment = comment.IsNullOrEmpty() ? "" : comment.Trim();
                workFlowComment.Sort = sort.IsInt() ? sort.ToInt() : bworkFlowComment.GetManagerMaxSort();


                if (isAdd)
                {
                    bworkFlowComment.Insert(workFlowComment);
                    //Next.WorkFlow.BLL.Log.Add("添加了流程意见", workFlowComment.Serialize(), Next.WorkFlow.BLL.Log.Types.流程相关);
                }
                else
                {
                    bworkFlowComment.Update(workFlowComment);
                    //Next.WorkFlow.BLL.Log.Add("修改了流程意见", "", Next.WorkFlow.BLL.Log.Types.流程相关, oldXML, workFlowComment.Serialize());
                }
                //bworkFlowComment.RefreshCache();
                ViewBag.Script = "new RoadUI.Window().reloadOpener();alert('保存成功!');";
            }
            return View(workFlowComment == null ? new WorkFlowComment() : workFlowComment);
        }

    }
}
