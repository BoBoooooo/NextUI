using Next.Admin.BLL;
using Next.Admin.Entity;
using Next.Controllers;
using Next.Framework.Core;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NextUI.Areas.WorkFlow.Controllers
{
    public class MembersController : BaseController
    {
        //
        // GET: /WorkFlow/Members/
        public ActionResult Index()
        {
            return View();
        }
        public string Tree1()
        {
            string rootid = Request.QueryString["rootid"];
            string showtype = Request.QueryString["showtype"];
            DeptBLL deptBll = BLLFactory<DeptBLL>.Instance;
            System.Text.StringBuilder json = new System.Text.StringBuilder("[", 1000);

            string rootID;
            Dept root;
            if (!string.IsNullOrEmpty(rootid))
            {
                root = deptBll.FindByID(rootid);
            }
            else
            {
                root = deptBll.GetTopGroup();
            }

            List<User> users = new List<User>();

            UserBLL busers = BLLFactory<UserBLL>.Instance;
            users = busers.Find("DeptID='" + root.ID + "'"); //busers.GetUsersByDept(root.ID);

            json.Append("{");
            json.AppendFormat("\"id\":\"{0}\",", root.ID);
            json.AppendFormat("\"parentID\":\"{0}\",", root.PID);
            json.AppendFormat("\"title\":\"{0}\",", root.Name);
            json.AppendFormat("\"ico\":\"{0}\",", Url.Content("~/Assets/WorkFlow/images/ico/icon_site.gif"));
            json.AppendFormat("\"link\":\"{0}\",", "");
            json.AppendFormat("\"type\":\"{0}\",", 2);
            json.AppendFormat("\"hasChilds\":\"{0}\",", 1);
            json.Append("\"childs\":[");

            var orgs = deptBll.GetChilds(root.ID);
            int count = orgs.Count;

            int i = 0;
            foreach (var org in orgs)
            {
                json.Append("{");
                json.AppendFormat("\"id\":\"{0}\",", org.ID);
                json.AppendFormat("\"parentID\":\"{0}\",", org.PID);
                json.AppendFormat("\"title\":\"{0}\",", org.Name);
                json.AppendFormat("\"ico\":\"{0}\",", "");
                json.AppendFormat("\"link\":\"{0}\",", "");
                json.AppendFormat("\"type\":\"{0}\",", 2);
                json.AppendFormat("\"hasChilds\":\"{0}\",", 1);
                json.Append("\"childs\":[");
                json.Append("]");
                json.Append("}");
                if (i++ < count - 1 || users.Count > 0)
                {
                    json.Append(",");
                }
            }

            if (users.Count > 0)
            {
                //var userRelations = new RoadFlow.Platform.UsersRelation().GetAllByOrganizeID(root.ID);
                //var userList
                int count1 = users.Count;
                int j = 0;
                foreach (var user in users)
                {
                    //var ur = userRelations.Find(p => p.UserID == user.ID);
                    json.Append("{");
                    json.AppendFormat("\"id\":\"{0}\",", user.ID);
                    json.AppendFormat("\"parentID\":\"{0}\",", root.ID);
                    json.AppendFormat("\"title\":\"{0}\",", user.FullName);//, ur != null && ur.IsMain == 0 ? "<span style='color:#999;'>[兼职]</span>" : "");
                    json.AppendFormat("\"ico\":\"{0}\",", "");
                    json.AppendFormat("\"link\":\"{0}\",", "");
                    json.AppendFormat("\"type\":\"{0}\",", "4");
                    json.AppendFormat("\"hasChilds\":\"{0}\",", "0");
                    json.Append("\"childs\":[");
                    json.Append("]");
                    json.Append("}");
                    if (j++ < count1 - 1)
                    {
                        json.Append(",");
                    }
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
            string showtype = Request.QueryString["showtype"];

            System.Text.StringBuilder json = new System.Text.StringBuilder("[", 1000);

            string orgID;
            if (string.IsNullOrEmpty(id))
            {
                json.Append("]");
                Response.Write(json.ToString());
            }else{
                orgID = id;
            }
            
            DeptBLL deptBll = BLLFactory<DeptBLL>.Instance;

            var childOrgs = deptBll.GetChilds(id);

            int count = childOrgs.Count;
            int i = 0;
            foreach (var org in childOrgs)
            {
                json.Append("{");
                json.AppendFormat("\"id\":\"{0}\",", org.ID);
                json.AppendFormat("\"parentID\":\"{0}\",", id);
                json.AppendFormat("\"title\":\"{0}\",", org.Name);
                json.AppendFormat("\"ico\":\"{0}\",", "");
                json.AppendFormat("\"link\":\"{0}\",", "");
                json.AppendFormat("\"type\":\"{0}\",", 2);
                json.AppendFormat("\"hasChilds\":\"{0}\",", 1);
                json.Append("\"childs\":[");
                json.Append("]");
                json.Append("}");
                if (i++ < count - 1)
                {
                    json.Append(",");
                }
            }

            UserBLL busers = BLLFactory<UserBLL>.Instance;
            var users = busers.Find("DeptID='"+id+"'");//.GetUsersByDept(id);
            int count1 = users.Count;
            if (count1 > 0 && count > 0)
            {
                json.Append(",");
            }
            int j = 0;
            foreach (var user in users)
            {
                json.Append("{");
                json.AppendFormat("\"id\":\"{0}\",", user.ID);
                json.AppendFormat("\"parentID\":\"{0}\",", id);
                json.AppendFormat("\"title\":\"{0}\",", user.FullName);//, ur != null && ur.IsMain == 0 ? "<span style='color:#999;'>[兼职]</span>" : "");
                json.AppendFormat("\"ico\":\"{0}\",", Url.Content("~/Assets/WorkFlow/images/ico/contact_grey.png"));
                json.AppendFormat("\"link\":\"{0}\",", "");
                json.AppendFormat("\"type\":\"{0}\",", "4");
                json.AppendFormat("\"hasChilds\":\"{0}\",", "0");
                json.Append("\"childs\":[");
                json.Append("]");
                json.Append("}");
                if (j++ < count1 - 1)
                {
                    json.Append(",");
                }
            }
            json.Append("]");
            return json.ToString();
        }
	}
}