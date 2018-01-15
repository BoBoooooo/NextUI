using Next.Admin.BLL;
using Next.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Next.WorkFlow.Utility;
using Next.Admin.Entity;
using Next.WorkFlow.BLL;
using Next.WorkFlow.Entity;

namespace NextUI.Areas.WorkFlow.Controllers
{
    public class DictController  : BusinessController<DictBLL, Dict>
    {
        //
        // GET: /WorkFlow/Dict/
        public ActionResult Index()
        {
            return View();
        }

        public string Tree1()
        {
            Next.WorkFlow.BLL.DictBLL BDict = new Next.WorkFlow.BLL.DictBLL();

            string rootid = Request.QueryString["root"];
            bool ischild = "1" == Request.QueryString["ischild"];//是否要加载下级节点
            string rootID = Guid.Empty.ToString();
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

            var root = rootID != Guid.Empty.ToString() ? BDict.FindByID(rootID) : BDict.GetRoot();
            var rootHasChild = BDict.HasChilds(root.ID);
            System.Text.StringBuilder json = new System.Text.StringBuilder("[", 1000);
            json.Append("{");
            json.AppendFormat("\"id\":\"{0}\",", root.ID);
            json.AppendFormat("\"parentID\":\"{0}\",", root.ParentID);
            json.AppendFormat("\"title\":\"{0}\",", root.Title);
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
                json.AppendFormat("\"parentID\":\"{0}\",", child.ParentID);
                json.AppendFormat("\"title\":\"{0}\",", child.Title);
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
            Next.WorkFlow.BLL.DictBLL BDict = new Next.WorkFlow.BLL.DictBLL();
            var childs = BDict.GetChildsByID(gid).OrderBy(p => p.Sort);
            int i = 0;
            int count = childs.Count();
            foreach (var child in childs)
            {
                var hasChilds = BDict.HasChilds(child.ID);
                json.Append("{");
                json.AppendFormat("\"id\":\"{0}\",", child.ID);
                json.AppendFormat("\"parentID\":\"{0}\",", child.ParentID);
                json.AppendFormat("\"title\":\"{0}\",", child.Title);
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


        public ActionResult Tree()
        {
            return View();
        }

        public ActionResult Body()
        {
            return Body(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Body(FormCollection collection)
        {
            Next.WorkFlow.BLL.DictBLL bdict = new Next.WorkFlow.BLL.DictBLL();
            Dict dict = null;
            string id = Request.QueryString["id"];
            if (id.IsGuid())
            {
                dict = bdict.FindByID(id.ToGuid());
            }
            if (dict == null)
            {
                dict = bdict.GetRoot();
            }

            if (collection != null)
            {
                string refreshID = dict.ParentID == Guid.Empty.ToString() ? dict.ID.ToString() : dict.ParentID.ToString();
                //删除
                if (!Request.Form["Delete"].IsNullOrEmpty())
                {
                    int i = bdict.DeleteAndAllChilds(dict.ID);
                    //bdict.RefreshCache();
                    //RoadFlow.Platform.Log.Add("删除了数据字典及其下级共" + i.ToString() + "项", dict.Serialize(), RoadFlow.Platform.Log.Types.数据字典);
                    ViewBag.Script = "alert('删除成功!');parent.frames[0].reLoad('" + refreshID + "');window.location='Body?id=" + dict.ParentID.ToString() + "&appid=" + Request.QueryString["appid"] + "';";
                    return View(dict);
                }

                string title = Request.Form["Title"];
                string code = Request.Form["Code"];
                string values = Request.Form["Values"];
                string note = Request.Form["Note"];
                string other = Request.Form["Other"];
                string oldXML = dict.Serialize();

                dict.Code = code.IsNullOrEmpty() ? null : code.Trim();
                dict.Note = note.IsNullOrEmpty() ? null : note.Trim();
                dict.Other = other.IsNullOrEmpty() ? null : other.Trim();
                dict.Title = title.Trim();
                dict.Value = values.IsNullOrEmpty() ? null : values.Trim();

                bdict.Update(dict);
                //bdict.RefreshCache();
                //RoadFlow.Platform.Log.Add("修改了数据字典项", "", RoadFlow.Platform.Log.Types.数据字典, oldXML, dict.Serialize());
                ViewBag.Script = "alert('保存成功!');parent.frames[0].reLoad('" + refreshID + "');";
            }

            return View(dict);
        }

        public string CheckCode()
        {
            string code = Request.Form["value"];
            string id = Request["id"];
            return new Next.WorkFlow.BLL.DictBLL().HasCode(code, id) ? "唯一代码重复" : "1";
        }

        public ActionResult Add()
        {
            return add1(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(FormCollection collection)
        {
            return add1(collection);
        }

        public ActionResult add1(FormCollection collection)
        {
            Dict dict = new Dict();
            Next.WorkFlow.BLL.DictBLL bdict = new Next.WorkFlow.BLL.DictBLL();
            string id = Request.QueryString["id"];
            if (!id.IsGuid())
            {
                var dictRoot = bdict.GetRoot();
                id = dictRoot != null ? dictRoot.ID.ToString() : "";
            }
            if (!id.IsGuid())
            {
                throw new Exception("未找到父级");
            }

            if (collection != null)
            {
                string title = Request.Form["Title"];
                string code = Request.Form["Code"];
                string values = Request.Form["Values"];
                string note = Request.Form["Note"];
                string other = Request.Form["Other"];

                dict.ID = Guid.NewGuid().ToString();
                dict.Code = code.IsNullOrEmpty() ? null : code.Trim();
                dict.Note = note.IsNullOrEmpty() ? null : note.Trim();
                dict.Other = other.IsNullOrEmpty() ? null : other.Trim();
                dict.ParentID = id.ToGuid();
                dict.Sort = bdict.GetMaxSort(id.ToGuid());
                dict.Title = title.Trim();
                dict.Value = values.IsNullOrEmpty() ? null : values.Trim();

                bdict.Insert(dict);
                //bdict.RefreshCache();
                //RoadFlow.Platform.Log.Add("添加了数据字典项", dict.Serialize(), RoadFlow.Platform.Log.Types.数据字典);
                ViewBag.Script = "alert('添加成功!');parent.frames[0].reLoad('" + id + "');";
            }

            return View(dict);
        }

        public ActionResult Sort()
        {
            return Sort(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sort(FormCollection collection)
        {
            Next.WorkFlow.BLL.DictBLL BDict = new Next.WorkFlow.BLL.DictBLL();
            string id = Request.QueryString["id"];
            string refreshID = "";
            string dictid;
            List<Dict> dicts = new List<Dict>();
            if (id.IsGuid(out dictid))
            {
                var dict = BDict.FindByID(dictid);
                if (dict != null)
                {
                    dicts = BDict.GetChildsByID(dict.ParentID);
                    refreshID = dict.ParentID.ToString();
                }
            }

            if (collection != null)
            {
                string sortdict = Request.Form["sort"];
                if (sortdict.IsNullOrEmpty())
                {
                    return View(dicts);
                }
                string[] sortArray = sortdict.Split(',');

                int i = 1;
                foreach (string id1 in sortArray)
                {
                    string gid;
                    if (id1.IsGuid(out gid))
                    {
                        BDict.UpdateSort(gid, i++);
                    }
                }
                //BDict.RefreshCache();

                //RoadFlow.Platform.Log.Add("保存了数据字典排序", "保存了ID为：" + id + "的同级排序", RoadFlow.Platform.Log.Types.数据字典);
                ViewBag.Script = "parent.frames[0].reLoad('" + refreshID + "');";
                dicts = BDict.GetChildsByID(refreshID.ToGuid());
            }

            return View(dicts);
        }

    }
}
