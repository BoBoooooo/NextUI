using Next.Admin.BLL;
using Next.Controllers;
using Next.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NextUI.Areas.WorkFlow.Controllers
{
    public class SelectMemberController : BaseController
    {
        //
        // GET: /WorkFlow/SelectMember/
        public ActionResult Index()
        {

            ViewBag.userName = BLLFactory<UserBLL>.Instance.FindByID(Request.QueryString["values"]);
            return View();
        }
        public string GetNames()
        {
            string values = Request.QueryString["values"];
            return BLLFactory<UserBLL>.Instance.FindByID(values).Name;
        }
	}
}