using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Next.Admin.Entity;
using Next.Commons;
using Next.Framework.Core.Commons;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Next.Controllers
{
    public class BaseController : Controller
    {
        //
        // GET: /Base/
        public User CurrentUser = new User();
        protected AuthorizeKey AuthorizeKey = new AuthorizeKey();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            base.OnActionExecuting(filterContext);
            CurrentUser = Session["UserInfo"] as User;
            if (CurrentUser == null)
            {
                Response.Redirect("/Login/Login");
            }
            ConvertAuthorizedInfo();
            ViewBag.AuthorizeKey = AuthorizeKey;
        }

        protected virtual void ConvertAuthorizedInfo()
        {
            AuthorizeKey.CanInsert = HasFunction(AuthorizeKey.InsertKey);
            AuthorizeKey.CanUpdate = HasFunction(AuthorizeKey.UpdateKey);
            AuthorizeKey.CanDelete = HasFunction(AuthorizeKey.DeleteKey);
            AuthorizeKey.CanView = HasFunction(AuthorizeKey.ViewKey);
            AuthorizeKey.CanInsert = HasFunction(AuthorizeKey.ListKey);
            AuthorizeKey.CanExport = HasFunction(AuthorizeKey.ExportKey);

        }
        public virtual bool HasFunction(string functionID)
        {
            //return true;
            return Permission.HasFunction(functionID);
        }
        protected string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        protected virtual void CheckAuthorized(string functionID)
        {
            if (!HasFunction(functionID))
            {
                string errorMessage = "您未被授权使用该功能，请重新登录测试或联系管理员进行处理";
                throw new MyDenyAccessException(errorMessage);
            }
        }
        public ContentResult JsonDate(object date)
        {
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            return Content(JsonConvert.SerializeObject(date, Formatting.Indented, timeConverter));
        }

        public ContentResult Content(bool result)
        {
            return Content(result.ToString().ToLower());
        }
        /// <summary>
        /// 调用AsposeCell控件，生成Excel文件
        /// </summary>
        /// <param name="datatable">生成的表格数据</param>
        /// <param name="relatedPath">服务器相对路径</param>
        /// <returns></returns>
        protected virtual bool GenerateExcel(DataTable datatable, string relatedPath)
        {
            #region 把DataTable转换为Excel并输出
            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook();
            //为单元格添加样式    
            Aspose.Cells.Style style = workbook.Styles[workbook.Styles.Add()];
            //设置居中
            style.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;
            //设置背景颜色
            style.ForegroundColor = System.Drawing.Color.FromArgb(153, 204, 0);
            style.Pattern = Aspose.Cells.BackgroundType.Solid;
            style.Font.IsBold = true;

            int rowIndex = 0;
            for (int i = 0; i < datatable.Columns.Count; i++)
            {
                DataColumn col = datatable.Columns[i];
                string columnName = col.Caption ?? col.ColumnName;
                workbook.Worksheets[0].Cells[rowIndex, i].PutValue(columnName);
                workbook.Worksheets[0].Cells[rowIndex, i].SetStyle(style);
            }
            rowIndex++;

            foreach (DataRow row in datatable.Rows)
            {
                for (int i = 0; i < datatable.Columns.Count; i++)
                {
                    workbook.Worksheets[0].Cells[rowIndex, i].PutValue(row[i].ToString());
                }
                rowIndex++;
            }

            for (int k = 0; k < datatable.Columns.Count; k++)
            {
                workbook.Worksheets[0].AutoFitColumn(k, 0, 150);
            }
            workbook.Worksheets[0].FreezePanes(1, 0, 1, datatable.Columns.Count);

            //根据用户创建目录，确保生成的文件不会产生冲突
            string realPath = Server.MapPath(relatedPath);
            string parentPath = Directory.GetParent(realPath).FullName;
            DirectoryUtil.AssertDirExist(parentPath);

            workbook.Save(realPath, Aspose.Cells.FileFormatType.Excel2003);

            #endregion

            return true;
        }
	}
}