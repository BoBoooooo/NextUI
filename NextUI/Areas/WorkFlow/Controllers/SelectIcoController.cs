using Next.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NextUI.Areas.WorkFlow.Controllers
{
    public class SelectIcoController : BaseController
    {
        //
        // GET: /WorkFlow/SelectIco/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult File()
        {
            return View();
        }

        public ActionResult Folder()
        {
            return View();
        }
    }
}