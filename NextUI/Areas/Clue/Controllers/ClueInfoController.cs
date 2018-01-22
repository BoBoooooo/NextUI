using App.Clue.BLL;
using App.Clue.Entity;
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
namespace NextUI.Areas.Clue.Controllers
{
    public class ClueInfoController : BusinessController<ClueInfoBLL, ClueInfo>
    {
        //
        // GET: /Jssjw/Loan/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(string ID, string type)
        {
            ClueInfo item = null;
            if (ID == null) return null;
            if (type != "Add")
            {
                item = BLLFactory<ClueInfoBLL>.Instance.FindByID(ID);
                if (item.AttachmentID == null)
                {
                    item.AttachmentID = Guid.NewGuid().ToString();
                }

                //ViewBag.item = item;
            }
            else
            {
                item = new ClueInfo();
                item.ID = Guid.NewGuid().ToString();
                item.UserID = CurrentUser.ID;
                item.DeptID = CurrentUser.DeptID;
                item.AttachmentID = Guid.NewGuid().ToString();
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
                        var name = typeof(ClueInfo);
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
            List<ClueInfo> list = baseBLL.FindWithPager(where, pagerInfo);
            var result = new { total = pagerInfo.RecordCount, rows = list };
            return JsonDate(result);
        }
        private Dictionary<string, string> realNameTable = null;
        private string getRealNameByColumnName(string id)
        {
            if (realNameTable == null)
            {
                StreamReader sr = System.IO.File.OpenText(Server.MapPath("/json/App/ClueInfo/ColumnName.json"));
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
            List<ClueInfo> list = new List<ClueInfo>();

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