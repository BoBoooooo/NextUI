using Next.Admin.BLL;
using Next.Admin.Entity;
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
using Next.Admin.Model;
namespace Next.Areas.Admin.Controllers
{
    public class DictTypeController : BusinessController<DictTypeBLL, DictType>
    {
        //
        // GET: /Jssjw/Loan/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDictJson()
        {
            List<DictType> list = baseBLL.GetAll();
            list = CollectionHelper<DictType>.Fill("-1", 0, list, "PID", "ID", "Name");

            List<CListItem> itemList = new List<CListItem>();
            foreach (DictType info in list)
            {
                itemList.Add(new CListItem(info.Name, info.ID));
            }
            return Json(itemList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取树形展示数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTreeJson()
        {
            List<TreeData> treeList = new List<TreeData>();
            List<DictType> typeList = BLLFactory<DictTypeBLL>.Instance.Find("PID='-1' ");
            foreach (DictType info in typeList)
            {
                TreeData node = new TreeData(info.ID, info.PID, info.Name);
                GetTreeJson(info.ID, node);

                treeList.Add(node);
            }
            string json = ToJson(treeList);
            return Content(json);
        }

        /// <summary>
        /// 递归获取树形信息
        /// </summary>
        /// <returns></returns>
        private void GetTreeJson(string PID, TreeData treeNode)
        {
            string condition = string.Format("PID='{0}' ", PID);
            List<DictType> nodeList = BLLFactory<DictTypeBLL>.Instance.Find(condition);
            StringBuilder content = new StringBuilder();

            foreach (DictType model in nodeList)
            {
                TreeData node = new TreeData(model.ID, model.PID, model.Name);
                treeNode.children.Add(node);

                GetTreeJson(model.ID, node);
            }
        }



    }
}