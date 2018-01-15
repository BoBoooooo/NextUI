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
using Next.Admin.Model;
using Next.WorkFlow.Utility;

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
        /*
        public string Tree1()
        {
            DictTypeBLL BDict = new DictTypeBLL();

            string rootid = Request.QueryString["root"];
            bool ischild = "1" == Request.QueryString["ischild"];//是否要加载下级节点
            string rootID = string.Empty;
            if (!rootid.IsNullOrEmpty())
            {
                if (!rootid.IsGuid(out rootID))
                {
                    var dict = BDict.GetByCode(rootid);
                    if (dict != null)
                    {
                        rootID = dict.ID;
                    }
                }
            }

            var root = rootID != string.Empty ? BDict.FindByID(rootID) : BDict.GetRoot();
            var rootHasChild = BDict.HasChilds(root.ID);
            System.Text.StringBuilder json = new System.Text.StringBuilder("[", 1000);
            json.Append("{");
            json.AppendFormat("\"id\":\"{0}\",", root.ID);
            json.AppendFormat("\"parentID\":\"{0}\",", root.PID);
            json.AppendFormat("\"title\":\"{0}\",", root.Name);
            json.AppendFormat("\"type\":\"{0}\",", rootHasChild ? "0" : "2"); //类型：0根 1父 2子
            json.AppendFormat("\"ico\":\"{0}\",", Url.Content("~/Assets/WorkFlow/images/ico/role.gif"));
            json.AppendFormat("\"hasChilds\":\"{0}\",", rootHasChild ? "1" : "0");
            json.Append("\"childs\":[");

            var childs = BDict.GetChildsByID(root.ID);
            int i = 0;
            int count = childs.Count;
            foreach (var child in childs)
            {
                var hasChild = ischild && BDict.HasChilds(child.ID);
                json.Append("{");
                json.AppendFormat("\"id\":\"{0}\",", child.ID);
                json.AppendFormat("\"parentID\":\"{0}\",", child.PID);
                json.AppendFormat("\"title\":\"{0}\",", child.Name);
                json.AppendFormat("\"type\":\"{0}\",", hasChild ? "1" : "2");
                json.AppendFormat("\"ico\":\"{0}\",", "");
                json.AppendFormat("\"hasChilds\":\"{0}\",", hasChild ? "1" : "0");
                json.Append("\"childs\":[");
                json.Append("]");
                json.Append("}");
                if (i++ < count - 1)
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
            string id = Request.QueryString["refreshid"];
            string gid;
            if (!id.IsGuid(out gid))
            {
                Response.Write("[]");
            }
            System.Text.StringBuilder json = new System.Text.StringBuilder("[", 1000);
            DictTypeBLL BDict = new DictTypeBLL();
            var childs = BDict.GetChildsByID(gid).OrderBy(p => p.Seq);
            int i = 0;
            int count = childs.Count();
            foreach (var child in childs)
            {
                var hasChilds = BDict.HasChilds(child.ID);
                json.Append("{");
                json.AppendFormat("\"id\":\"{0}\",", child.ID);
                json.AppendFormat("\"parentID\":\"{0}\",", child.PID);
                json.AppendFormat("\"title\":\"{0}\",", child.Name);
                json.AppendFormat("\"type\":\"{0}\",", hasChilds ? "1" : "2");
                json.AppendFormat("\"ico\":\"{0}\",", "");
                json.AppendFormat("\"hasChilds\":\"{0}\",", hasChilds ? "1" : "0");
                json.Append("\"childs\":[");
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
        */
    }
}