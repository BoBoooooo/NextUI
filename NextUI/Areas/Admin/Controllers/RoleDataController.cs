using Next.Admin.BLL;
using Next.Admin.Entity;
using Next.Admin.Model;
using Next.Controllers;
using Next.Framework.Core;
using Next.Framework.Core.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Next.Areas.Admin.Controllers
{
    public class RoleDataController : BusinessController<RoleDataBLL,RoleData>
    {
        public RoleDataController()
            : base()
        {

        }
        //
        // GET: /RoleData/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UpdateRoleData(string roleID, string belongCompanys, string belongDepts)
        {
            bool result = BLLFactory<RoleDataBLL>.Instance.UpdateRoleData(roleID, belongCompanys, belongDepts);
            return Content(result);
        }

        public ActionResult GetRoleDataList(string roleID)
        {
            Dictionary<string, string> dict = BLLFactory<RoleDataBLL>.Instance.GetRoleDataDict(roleID);
            List<string> list = new List<string>();
            list.AddRange(dict.Keys);

            return Json(list, JsonRequestBehavior.AllowGet);
        }
	}
}