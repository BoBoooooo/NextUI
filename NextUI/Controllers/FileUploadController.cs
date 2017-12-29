using Next.Admin.BLL;
using Next.Admin.Entity;
using Next.Admin.Model;
using Next.Attachment.BLL;
using Next.Attachment.Entity;
using Next.Framework.Core;
using Next.Framework.Core.Commons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Next.Controllers
{
    public class FileUploadController : BusinessController<FileUploadBLL, FileUpload>
    {
        //
        // GET: /FileUpload/
        public ActionResult Index()
        {
            string attachmentID = Request["AttachmentID"];
            string flag = Request["Flag"];
            ViewBag.attachmentID = attachmentID;
            ViewBag.flag = flag;
            return View();
        }
        public override ActionResult FindWithPager()
        {
            base.CheckAuthorized(AuthorizeKey.ListKey);
            string where = GetPagerCondition();
            string attachmentID = Request["AttachmentID"];
            where += string.Format(" AND AttachmentID='{0}'", attachmentID);
            PagerInfo pagerInfo = GetPagerInfo();
            List<FileUpload> list = baseBLL.FindWithPager(where, pagerInfo);
            var result = new { total = pagerInfo.RecordCount, rows = list };
            return JsonDate(result);
        }

        public FileResult Download(string ID)
        {


            var file = BLLFactory<FileUploadBLL>.Instance.FindByID(ID);
            string filePathStr = "~/UploadFiles/" + file.SavePath;
            String filePath = Server.MapPath(filePathStr);
            FileStream fs = new FileStream(filePath, FileMode.Open);
            long size = fs.Length;
            byte[] buffer = new byte[size];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();
            return File(buffer, " ",/*affix.ContentType*/ file.FileName);


        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Upload(HttpPostedFileBase fileData, string guid, string folder)
        {
            if (fileData != null)
            {
                try
                {
                    /*ControllerContext.HttpContext.Request.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    ControllerContext.HttpContext.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    ControllerContext.HttpContext.Response.Charset = "UTF-8";*/

                    string filePath = Server.MapPath("/UploadFiles");
                    DirectoryUtil.AssertDirExist(filePath);

                    string fileName = Path.GetFileName(fileData.FileName);
                    string fileExtension = Path.GetExtension(fileName);
                    /*if (fileExtension == "")
                    {
                        fileExtension = " ";
                    }*/
                    string saveName = Guid.NewGuid().ToString() + fileExtension;

                    FileUpload info = new FileUpload();
                    info.FileData = ReadFileBytes(fileData);
                    if (info.FileData != null)
                    {
                        info.FileSize = info.FileData.Length;
                    }
                    info.Category = folder;
                    info.FileName = fileName;
                    info.FileExtend = fileExtension;
                    info.AttachmentID = guid;
                    info.AddTime = DateTime.Now;
                    //info.Editor = CurrentUser.Name;

                    CommonResult result = BLLFactory<FileUploadBLL>.Instance.Upload(info);
                    if (!result.Success)
                    {
                        LogTextHelper.Error("上传文件失败：" + result.ErrorMessage);

                    }
                    return Content(result.Success.ToString());

                }
                catch (Exception ex)
                {
                    LogTextHelper.Error(ex);
                    return Content("false");
                }
            }
            else
            {
                return Content("false");
            }
        }
        /*public ActionResult Delete(string id)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(id))
            {
                result = BLLFactory<FileUploadBLL>.Instance.Delete(id);
            }
            return Content(result);
        }*/
        private byte[] ReadFileBytes(HttpPostedFileBase fileData)
        {
            byte[] data;
            using (Stream inputStream = fileData.InputStream)
            {
                MemoryStream memoryStream = inputStream as MemoryStream;
                if (memoryStream == null)
                {
                    memoryStream = new MemoryStream();
                    inputStream.CopyTo(memoryStream);
                }
                data = memoryStream.ToArray();
            }
            return data;
        }
        public ActionResult GetViewAttachmentHtml(string guid)
        {
            string html = @"<li>附件暂无</li>";
            if (string.IsNullOrEmpty(guid))
            {
                return Content(html);
            }
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
                    sb.AppendFormat(@"<img border='0' width='16px' height='16px' src='/Assets/Themes/Default/file_extension/{0}.png", info.FileExtend.Trim('.'));
                    sb.AppendFormat(@"<a href='/{0}?ext={1}' target='_black'>&nbsp; {2}</a></li>", GetFilePath(info), info.FileExtend.Trim('.'), info.FileName);
                }
            }
            else
            {
                sb.Append(html);
            }
            return Content(sb.ToString());
        }
        public ActionResult GetAttachmentHtml(string guid)
        {
            string html = @"<li>附件暂无</li>";
            if (string.IsNullOrEmpty(guid))
            {
                return Content(html);
            }
            StringBuilder sb = new StringBuilder();
            int seq = 1;
            List<FileUpload> fileList = BLLFactory<FileUploadBLL>.Instance.GetByAttachGuid(guid);
            if (fileList != null && fileList.Count > 0)
            {
                foreach (FileUpload info in fileList)
                {
                    string fileName = info.FileName.Trim();
                    fileName = System.Web.HttpContext.Current.Server.UrlEncode(fileName);

                    sb.Append("<tr>");
                    sb.AppendFormat("<td style='width:20px'><img border='0' width='16px' height='16px' src='/Assets/images/delete.gif' onclick=\"deleteAttach('{0}')\"/></td>", info.ID);
                    sb.AppendFormat(@"<td><li><span>[ 附件{0} ]</span>", seq++);
                    if (info.FileExtend != null)
                    {
                        sb.AppendFormat(@"<img border='0' width='16px' height='16px' src='/Assets/Themes/Default/file_extension/{0}.png'/>", info.FileExtend.Trim('.'));
                        sb.AppendFormat(@"<a href='/{0}?ext={1}' target='_black'>&nbsp; {2}</a></li></td>", GetFilePath(info), info.FileExtend.Trim('.'), info.FileName);
                    }
                    else
                    {
                        sb.AppendFormat(@"<a href='/{0}' target='_black'>&nbsp; {1}</a></li></td>", GetFilePath(info), info.FileName);
                    }
                    sb.Append("</tr>");
                }
                string result = string.Format("<table style='border:0px, solid #A8CFEB;'>{0}</table>", sb.ToString());
                return Content(result);
            }
            else
            {
                sb.Append(html);
            }
            return Content(sb.ToString());
        }
        private string GetFilePath(FileUpload info)
        {
            string filePath = BLLFactory<FileUploadBLL>.Instance.GetFilePath(info);
            return HttpUtility.UrlPathEncode(filePath);
        }
    }
}