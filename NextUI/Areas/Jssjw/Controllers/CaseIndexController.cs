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
    public class CaseIndexController : BusinessController<CaseIndexBLL, CaseIndex>
    {
        //
        // GET: /Jssjw/Loan/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(string ID, string type)
        {
            CaseIndex item = null;
            if (ID == null) return null;
            if (type != "Add")
            {
                item = BLLFactory<CaseIndexBLL>.Instance.FindByID(ID);
                if (item.AttachmentID == null)
                {
                    item.AttachmentID = Guid.NewGuid().ToString();
                }

                //ViewBag.item = item;
            }
            else
            {
                item = new CaseIndex();
                item.ID = GetMaxID() + 1;
                item.UserID = CurrentUser.ID;
                item.DeptID = CurrentUser.DeptID;
                item.Dept = CurrentUser.DeptName;
                item.AttachmentID = Guid.NewGuid().ToString();
            }
            ViewBag.flag = type;
            ViewBag.item = item;
            return View();
        }
        public int GetMaxID()
        {
            string sql = "select *  FROM CaseIndex,(SELECT MAX(ID) AS max from CaseIndex) M1 WHERE ID=M1.max";
            var result = BLLFactory<CaseIndexBLL>.Instance.GetList(sql);
            CaseIndex max = result.FirstOrDefault();
            return max.ID;
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
                        var name = typeof(CaseIndex);
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
            where = AddUsersRightToWhereCondition(where);
            PagerInfo pagerInfo = GetPagerInfo();
            List<CaseIndex> list = baseBLL.FindWithPager(where, pagerInfo);
            var result = new { total = pagerInfo.RecordCount, rows = list };
            return JsonDate(result);
        }


        public  ActionResult FindSummaryWithPager()
        {
            base.CheckAuthorized(AuthorizeKey.ListKey);
            string where = GetPagerCondition();
            where = AddUsersRightToWhereCondition(where);
            PagerInfo pagerInfo = GetPagerInfo();
            List<CaseIndex> list = baseBLL.FindWithPager(where, pagerInfo);
            for (var i = 0; i < list.Count(); i++)
            {
                var name = list.ElementAt(i).CaseName;
                //var t = (from c in db.Property where c.CaseName == name select c.GoodsCount).ToList();
                var t = BLLFactory<PropertyBLL>.Instance.Find("CaseName='" + name + "'");
                int count = 0;
                int item = 0;
                foreach (var c in t)
                {
                    if (c != null)
                        if (int.TryParse(c.GoodsCount, out item))
                        {
                            count += int.Parse(c.GoodsCount);
                        }
                        else
                        {
                            count++;
                        }

                }
                list.ElementAt(i).CaseContentCount = count.ToString();
            }
            var result = new { total = pagerInfo.RecordCount, rows = list };
            return JsonDate(result);
        }
        private Dictionary<string, string> realNameTable = null;
        private string getRealNameByColumnName(string id)
        {
            if (realNameTable == null)
            {
                StreamReader sr = System.IO.File.OpenText(Server.MapPath("/json/Jssjw/CaseIndex/ColumnName.json"));
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
        /// <summary>
        /// 根据查询条件导出列表数据
        /// </summary>
        /// <returns></returns>
        public ActionResult Export()
        {

            string where = GetPagerCondition();
            string CustomedCondition = Request["CustomedCondition"] ?? "";
            List<CaseIndex> list = new List<CaseIndex>();

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


    }
}