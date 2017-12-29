using Jssjw.Case.BLL;
using Jssjw.Case.Entity;
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
namespace Next.Areas.Jssjw.Controllers
{
    public class PropertyController : BusinessController<PropertyBLL, Property>
    {
        //
        // GET: /Jssjw/Loan/
        public ActionResult Index()
        {
            Session["CaseName"] = null;
            return View();
        }

        public ActionResult Prev(string type)
        {
            List<Property> Sequence = (List<Property>)Session["Sequence"];
            int ID = System.Convert.ToInt32(Session["CurrentID"].ToString());
            var r = from i in Sequence where i.ID == ID select i;

            int index = Sequence.IndexOf(r.FirstOrDefault());
            index = index <= 0 ? index : --index;
            Session["CurrentID"] = Sequence[index].ID;
            //Edit(Sequence[index].ID, "View");
            ViewBag.flag = type;
            ViewBag.item = Sequence[index];
            getIndex(Sequence[index].CaseName, Sequence[index].CaseCode);
            
            return View("Edit");
        }
        public ActionResult Next(string type)
        {
            List<Property> Sequence = (List<Property>)Session["Sequence"];
            int ID = System.Convert.ToInt32(Session["CurrentID"].ToString());
            var r = from i in Sequence where i.ID == ID select i;
            var check=r.FirstOrDefault();
            int index = Sequence.IndexOf(r.FirstOrDefault());
            index = index >= Sequence.Count - 1 ? index : ++index;
            Session["CurrentID"] = Sequence[index].ID;
            ViewBag.flag = type;
            ViewBag.item = Sequence[index];
            getIndex(Sequence[index].CaseName, Sequence[index].CaseCode);
            //Edit(Sequence[index].ID, "View");
            return View("Edit");
        }

        public ActionResult Edit(string ID, string type)
        {
            Property item = null;
            if (ID == null) return null;
            Session["CurrentID"] = ID;
            if (type != "Add")
            {

                item = BLLFactory<PropertyBLL>.Instance.FindByID(ID);
                if (item.AttachmentID == null)
                {
                    item.AttachmentID = Guid.NewGuid().ToString();
                }
                getIndex(item.CaseName, item.CaseCode);
                //ViewBag.item = item;
            }
            else
            {
                item = new Property();
                item.ID = GetMaxID()+1;
                item.UserID = CurrentUser.ID;
                item.DeptID = CurrentUser.DeptID;
                item.AttachmentID = Guid.NewGuid().ToString();
                getIndex(null, null);
            }
            ViewBag.flag = type;
            ViewBag.item = item;
            
            return View();
        }

        private string AddUsersRightToWhereCondition(string where)
        {
            List<Role> roleInfo = BLLFactory<RoleBLL>.Instance.GetRolesByUser(CurrentUser.ID);


            foreach (var info in roleInfo)
            {
                RoleData roleDataInfo = BLLFactory<RoleDataBLL>.Instance.FindByRoleID(info.ID);
                if (roleDataInfo != null)
                {
                    List<Function> FunctionList = BLLFactory<FunctionBLL>.Instance.GetFunctionsByRole(roleDataInfo.RoleID);
                    foreach (var func in FunctionList)
                    {
                        var name = typeof(Property);
                        if (func.ControlID == name.Name)
                        {
                            if (roleDataInfo != null)
                            {
                                if (roleDataInfo.BelongDepts != null)
                                {
                                    where += string.Format(" and (DeptID='{0}' ", CurrentUser.DeptID);
                                    string[] depts = roleDataInfo.BelongDepts.Split(',');
                                    foreach (string item in depts)
                                    {
                                        where += string.Format(" OR DeptID='{0}' ", item);
                                    }
                                    where += string.Format(")");
                                }
                                else
                                {
                                    where += string.Format(" and UserID='{0}' ", CurrentUser.ID);
                                }
                                return where;
                            }
                        }
                    }
                }

            }
            where += string.Format(" and UserID='{0}' ", CurrentUser.ID);
            return where;
        }
        public override ActionResult FindWithPager()
        {
            base.CheckAuthorized(AuthorizeKey.ListKey);
            string where = GetPagerCondition();
            if (Session["CaseName"] != null)
            {
                where += " and CaseName='" + Session["CaseName"] + "'";

            }
            where = AddUsersRightToWhereCondition(where);
            PagerInfo pagerInfo = GetPagerInfo();
            List<Property> list = baseBLL.FindWithPager(where, pagerInfo);
            List<Property> tipList = BLLFactory<PropertyBLL>.Instance.Find(where);
            Session["Sequence"] = tipList.ToList<Property>();
            var result = new { total = pagerInfo.RecordCount, rows = list };
            return JsonDate(result);
        }
        private Dictionary<string, string> realNameTable = null;
        private string getRealNameByColumnName(string id)
        {
            if (realNameTable == null)
            {
                StreamReader sr = System.IO.File.OpenText(Server.MapPath("/json/Jssjw/Property/ColumnName.json"));
                StringBuilder jsonArrayText_tmp = new StringBuilder();
                string input;
                while ((input = sr.ReadLine()) != null)
                {
                    jsonArrayText_tmp.Append(input);
                }
                sr.Close();
                string jsonArrayText = jsonArrayText_tmp.Replace(" ", "").ToString();
                JArray ja = (JArray)JsonConvert.DeserializeObject(jsonArrayText);
                realNameTable = new Dictionary<string, string>();
                for (int i = 0; i < ja.Count(); i++)
                {
                    JObject o = (JObject)ja[i];
                    realNameTable.Add((string)o["id"], (string)o["text"]);
                    //Debug.WriteLine(o["a"]);
                    //Debug.WriteLine(ja[1]["a"]);
                }

            }
            return realNameTable[id];
        }
        public void getIndex(string currentCaseName, string currentCaseCode)
        {
            
            var temp=BLLFactory<CaseIndexBLL>.Instance.GetAll();
            List<SelectListItem> caseName = new List<SelectListItem>();
            List<SelectListItem> caseCode = new List<SelectListItem>();
            foreach (var c in temp)
            {
                caseName.Add(new SelectListItem { Text = c.CaseName, Value = c.CaseName, Selected = (c.CaseName == currentCaseName ? true : false) });
                caseCode.Add(new SelectListItem { Text = c.CaseCode, Value = c.CaseCode, Selected = (c.CaseCode == currentCaseCode ? true : false) });
            }
            ViewBag.caseName = caseName;
            ViewBag.caseCode = caseCode;
        }

        /// <summary>
        /// 根据查询条件导出列表数据
        /// </summary>
        /// <returns></returns>
        public ActionResult Export()
        {

            string where = GetPagerCondition();
            string CustomedCondition = Request["CustomedCondition"] ?? "";
            List<Property> list = new List<Property>();

            if (!string.IsNullOrWhiteSpace(CustomedCondition))
            {
                //如果为自定义的json参数列表，那么可以使用字典反序列化获取参数，然后处理
                //Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(CustomedCondition);

                //如果是条件的自定义，可以使用Find查找
                list = baseBLL.Find(CustomedCondition);
            }
            else
            {
                list = baseBLL.Find(where);
            }
            //string type = "review";
            string columns = Request["exportData"];
            string[] colArray = columns.Split(',');

            var result = list;
            var dt = CollectionHelper.ConvertTo(result);
            DataTable dataTable = new DataTable();
            //DataRow titleRow = new DataRow();
            for (int i = 0; i < colArray.Length; i++)
            {
                dataTable.Columns.Add(getRealNameByColumnName(colArray[i]));
            }
            //把DataGridView当前页的数据保存在Excel中
            DataRow dr;

            foreach (DataRow row in dt.Rows)
            {
                dr = dataTable.NewRow();
                foreach (var d in colArray)
                {
                    dr[getRealNameByColumnName(d)] = row[d];
                }
                dataTable.Rows.Add(dr);
            }

            string filename = DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            //Excel_Book.SaveCopyAs(Server.MapPath("/xls/") + filename);
            //workbook.Save(Server.MapPath("/xls/") + filename);
            string filePath = string.Format("/GenerateFiles/{0}/" + filename, CurrentUser.Name);
            GenerateExcel(dataTable, filePath);
            return File(Server.MapPath(filePath), "application/msexcel", Url.Encode(filename));
            //#endregion

        }

        public int GetMaxID()
        {
            string sql = "select *  FROM Property,(SELECT MAX(ID) AS max from Property) M1 WHERE ID=M1.max";
            var result = BLLFactory<PropertyBLL>.Instance.GetList(sql);
            Property max = result.FirstOrDefault();
            return max.ID;
        }

        public JsonResult GetLastData()
        {
            string sql = "select *  FROM Property,(SELECT MAX(ID) AS max from Property) M1 WHERE ID=M1.max";
            var result = BLLFactory<PropertyBLL>.Instance.GetList(sql);
            Property max = result.FirstOrDefault();
            return Json(max, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ImageUpLoad()
        {
            //定义错误消息
            string msg = "";
            //接受上传文件
            HttpPostedFileBase hp = Request.Files["selectPic"];
            if (hp == null)
            {
                msg = "请选择文件.";
            }
            //获取上传目录 转换为物理路径

            string uploadPath = Server.MapPath("/UploadImages/");
            //获取文件名
            string fileName = DateTime.Now.Ticks.ToString() + System.IO.Path.GetExtension(hp.FileName);
            //获取文件大小
            long contentLength = hp.ContentLength;
            //文件不能大于1M
            //保存文件的物理路径
            string saveFile = uploadPath + fileName;
            try
            {
                //保存文件
                hp.SaveAs(saveFile);
                msg = "/UploadImages/" + fileName;
            }
            catch
            {
            }
            return Content(msg);
        }

        public ActionResult Summary()
        {
            Session["CaseIndexMode"] = "Summary";
            ViewBag.CaseCount = BLLFactory<CaseIndexBLL>.Instance.GetAll().Count();
            var t = BLLFactory<PropertyBLL>.Instance.GetAll().ToList();
            int count = 0;
            foreach (var c in t)
            {
                if (c != null)
                    count += int.Parse(c.GoodsCount);
            }
            ViewBag.GoodsCount = count;
            return View();
        }
        public ActionResult PropertyQuery()
        {
            if (Request["CaseName"] != null)
            {
                Session["CaseName"] = Request["CaseName"];
            }
            else
            {
                Session["CaseName"] = null;
            }
            Session["PropertyMode"] = "Query";

            return View("Index");
        }
    }
}