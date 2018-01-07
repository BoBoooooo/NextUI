using Next.Admin.BLL;
using Next.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Next.WorkFlow.Utility;
using Next.Admin.Entity;

namespace NextUI.Areas.WorkFlow.Controllers
{
    public class DictController  : BusinessController<DictTypeBLL, DictType>
    {
        //
        // GET: /WorkFlow/Dict/
        public ActionResult Index()
        {
            return View();
        }
        public string Tree1()
        {
            DictTypeBLL BDict = new DictTypeBLL();

            string rootid = Request.QueryString["root"];
            bool ischild = "1" == Request.QueryString["ischild"];//是否要加载下级节点
            string rootID = null;
            if (!rootid.IsNullOrEmpty())
            {
                    rootID = rootid;
                    /*var dict = BDict.GetByCode(rootid);
                    if (dict != null)
                    {
                        rootID = dict.ID;
                    }*/
            }

            var root = !rootID.IsNullOrEmpty() ? BDict.FindByID(rootID) : BDict.GetRoot();
            var rootHasChild = BDict.HasChilds(root.ID);
            System.Text.StringBuilder json = new System.Text.StringBuilder("[", 1000);
            json.Append("{");
            json.AppendFormat("\"id\":\"{0}\",", root.ID);
            json.AppendFormat("\"parentID\":\"{0}\",", root.PID);
            json.AppendFormat("\"title\":\"{0}\",", root.Name);
            json.AppendFormat("\"type\":\"{0}\",", rootHasChild ? "0" : "2"); //类型：0根 1父 2子
            json.AppendFormat("\"ico\":\"{0}\",", Url.Content("~/assets/workflow/images/ico/role.gif"));
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
            if (!id.IsNullOrEmpty())
            {
                gid = id;
                Response.Write("[]");
            }
            else
            {
                gid = id;
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
	}
}