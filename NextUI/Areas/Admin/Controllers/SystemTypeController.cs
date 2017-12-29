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
    public class SystemTypeController : BusinessController<SystemTypeBLL, SystemType>
    {
        public SystemTypeController()
            : base()
        {

        }
        //
        // GET: /SystemType/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetTreeJson()
        {
            List<EasyTreeData> treeList = new List<EasyTreeData>();
            List<SystemType> list = baseBLL.GetAll();
            foreach (SystemType info in list)
            {
                treeList.Add(new EasyTreeData(info.ID, info.Name, "icon-computer"));

            }
            string content = ToJson(treeList);
            return Content(content.Trim(','));
        }

        public ActionResult GetDictJson()
        {
            List<SystemType> list = baseBLL.GetAll();
            List<CListItem> itemList = new List<CListItem>();
            foreach (SystemType info in list)
            {
                itemList.Add(new CListItem(info.Name, info.ID));
            }
            return Json(itemList, JsonRequestBehavior.AllowGet);
        }
	}
}