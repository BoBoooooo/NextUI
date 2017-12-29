using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace NextUI.Controllers
{
    public class UploadController : ApiController
    {
        public static string beforeFilename = "";
        // 上次上传的文件ID
        public static string beforeID = "";
        // 上次上传的文件后缀名
        public static string beforeExName = "";
        // 上次上传的masterID
        public static string beforeMasterID = "";

        public HttpResponseMessage Upload()
        {
            HttpPostedFile file = System.Web.HttpContext.Current.Request.Files[0];
            HttpContent content = Request.Content;

            string filePath = System.Web.HttpContext.Current.Server.MapPath("~/Attachment");

            if (!Directory.Exists(filePath))
            {
                //文件夹不存在则创建
                if (!Directory.CreateDirectory(filePath).Exists)
                {
                    //创建不成功则返回失败信息
                    return new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
                }
            }
            // 文件名
            string filename = file.FileName.Substring(file.FileName.LastIndexOf('\\') + 1);
            // 判断是否为断点续传文件
            if (filename.Equals(beforeFilename))
            {
                if (!this.SaveAs(filePath + "/" + beforeID + beforeExName, file))
                {
                    return new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
                }
            }
            else
            {
                // 生成唯一的文件名 GUID
                string ID = System.Guid.NewGuid().ToString();
                // 后缀名
                string exName = file.FileName.Substring(file.FileName.LastIndexOf('.'));
                // 本次文件名
                beforeFilename = filename;
                // 本次文件ID
                beforeID = ID;
                // 本次后缀名
                beforeExName = exName;

                if (!this.SaveAs(filePath + "/" + ID + exName, file))
                {
                    return new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
                }
            }

            System.Web.HttpContext.Current.Response.ContentType = "text/plain";
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var result = new { name = file.FileName };

            System.Web.HttpContext.Current.Response.Write(serializer.Serialize(result));
            System.Web.HttpContext.Current.Response.StatusCode = 200;

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        private bool SaveAs(string saveFilePath, HttpPostedFile file)
        {
            long lStartPos = 0;
            int startPosition = 0;
            int endPosition = 0;
            var contentRange = System.Web.HttpContext.Current.Request.Headers["Content-Range"];
            //bytes
            if (!string.IsNullOrEmpty(contentRange))
            {
                contentRange = contentRange.Replace("bytes", "").Trim();
                contentRange = contentRange.Substring(0, contentRange.IndexOf("/"));
                string[] ranges = contentRange.Split('-');
                startPosition = int.Parse(ranges[0]);
                endPosition = int.Parse(ranges[1]);
            }
            System.IO.FileStream fs;
            if (System.IO.File.Exists(saveFilePath))
            {
                fs = System.IO.File.OpenWrite(saveFilePath);
                lStartPos = fs.Length;
            }
            else
            {
                fs = new System.IO.FileStream(saveFilePath, System.IO.FileMode.Create);
                lStartPos = 0;
            }
            if (lStartPos > endPosition)
            {
                fs.Close();
                return false;
            }
            else if (lStartPos < startPosition)
            {
                lStartPos = startPosition;
            }
            else if (lStartPos > startPosition && lStartPos < endPosition)
            {
                lStartPos = startPosition;
            }
            fs.Seek(lStartPos, System.IO.SeekOrigin.Current);
            byte[] nbytes = new byte[512];
            int nReadSize = 0;
            nReadSize = file.InputStream.Read(nbytes, 0, 512);
            try
            {
                while (nReadSize > 0)
                {
                    fs.Write(nbytes, 0, nReadSize);
                    nReadSize = file.InputStream.Read(nbytes, 0, 512);
                }
                fs.Close();
                return true;
            }
            catch (Exception ex)
            {
                fs.Close();
                return false;
            }

        }
    }
}
