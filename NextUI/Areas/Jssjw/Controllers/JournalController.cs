using Jssjw.Office.BLL;
using Jssjw.Office.Entity;
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
using System.Text.RegularExpressions;

namespace Next.Areas.Jssjw.Controllers
{
    public class JournalController : BusinessController<JournalBLL, Journal>
    {
        //
        // GET: /Jssjw/Journal/
        public ActionResult Index()
        {
            return View();
        }

        public override ActionResult Insert(Journal info)
        {
            info.TextContent = TextNoHTML(info.Content);
            return base.Insert(info);
        }
        protected override bool Update(string id, Journal info)
        {
            info.TextContent = TextNoHTML(info.Content);
            return base.Update(id, info);
        }
        /// <summary>
        /// 将html文本转化为 文本内容方法TextNoHTML
        /// </summary>
        /// <param name="Htmlstring">HTML文本值</param>
        /// <returns></returns>
        public string TextNoHTML(string Htmlstring)
        {
            //删除脚本   
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML   
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([/r/n])[/s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "/", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "/xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "/xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "/xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "/xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(/d+);", "", RegexOptions.IgnoreCase);
            //替换掉 < 和 > 标记
            Htmlstring = Htmlstring.Replace("<", "");
            Htmlstring = Htmlstring.Replace(">", "");
            Htmlstring = Htmlstring.Replace("\r\n", "");
            Htmlstring = Htmlstring.Replace("\r", "");
            Htmlstring = Htmlstring.Replace("\n", "");
            //返回去掉html标记的字符串
            return Htmlstring;
        }
        public ActionResult Edit(string ID, string type)
        {
            Journal item = null;
            if (ID == null) return null;
            if (type != "Add")
            {
                item = BLLFactory<JournalBLL>.Instance.FindByID(ID);
                if (item.AttachmentID == null)
                {
                    item.AttachmentID = Guid.NewGuid().ToString();
                }

                ViewBag.item = item;
            }
            else
            {
                item = new Journal();
                item.ID = Guid.NewGuid().ToString();
                item.UserID = CurrentUser.ID;
                item.FullName = CurrentUser.FullName;
                item.DeptID = CurrentUser.DeptID;
                item.DeptName = CurrentUser.DeptName;
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
                        var name = typeof(Journal);
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
            List<Journal> list = baseBLL.FindWithPager(where, pagerInfo);
            var result = new { total = pagerInfo.RecordCount, rows = list };
            return JsonDate(result);
        }
        private Dictionary<string, string> realNameTable = null;
        private string getRealNameByColumnName(string id)
        {
            if (realNameTable == null)
            {
                StreamReader sr = System.IO.File.OpenText(Server.MapPath("/json/Jssjw/Office/ColumnName.json"));
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
            where = AddUsersRightToWhereCondition(where);
            string CustomedCondition = Request["CustomedCondition"] ?? "";
            List<Journal> list = new List<Journal>();

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
        /// <summary>
        /// 获取附件的HTML代码
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        private string GetAttachmentHtml(string guid)
        {
            string html = @"<li>附件暂无</li>";

            if (string.IsNullOrEmpty(guid))
                return html;

            StringBuilder sb = new StringBuilder();
            int seq = 1;
            List<FileUpload> fileList = BLLFactory<FileUploadBLL>.Instance.GetByAttachGuid(guid);
            if (fileList != null && fileList.Count > 0)
            {
                foreach (FileUpload info in fileList)
                {
                    string fileName = info.FileName.Trim();
                    fileName = System.Web.HttpContext.Current.Server.UrlEncode(fileName);

                    sb.AppendFormat(@"<li><span>[ 附件{0} ]</span>", seq++);
                    sb.AppendFormat(@"<img border='0' width='16px' height='16px' src='/Assets/Themes/Default/file_extension/{0}.png' />", info.FileExtend.Trim('.'));
                    sb.AppendFormat(@"<a href='/{0}?ext={1}' target='_blank'>&nbsp;{2}</a></li>", GetFilePath(info), info.FileExtend.Trim('.'), info.FileName);
                }
            }
            else
            {
                sb.Append(html);
            }

            return sb.ToString();
        }

        private string GetFilePath(FileUpload info)
        {
            string filePath = BLLFactory<FileUploadBLL>.Instance.GetFilePath(info);
            return HttpUtility.UrlPathEncode(filePath);
        }

    }
}