using Next.Attachment.DALMySql;
using Next.Attachment.Entity;
using Next.Attachment.IDAL;
using Next.Framework.Commons;
using Next.Framework.Core;
using Next.Framework.Core.Commons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Attachment.BLL
{
    public class FileUploadBLL : BaseBLL<FileUpload>
    {
        private IFileUploadDAL fileUploadDAL;
        public FileUploadBLL()
            : base()
        {
            base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//如果需要记录操作日志，则实现这个事件
            this.fileUploadDAL = (IFileUploadDAL)base.baseDal;
        }

        public List<FileUpload> GetByAttachGuid(string attachmentGuid)
        {
            IFileUploadDAL dal = baseDal as IFileUploadDAL;
            return dal.GetByAttachGuid(attachmentGuid);
        }

        public string GetFilePath(FileUpload info)
        {
            string fileName = info.FileName;
            string category = info.Category;
            if (string.IsNullOrEmpty(category))
            {
                category = "Photo";
            }
            string uploadFolder = Path.Combine(info.BasePath, category);
            string realFolderPath = uploadFolder;

            if (!Path.IsPathRooted(uploadFolder))
            {
                realFolderPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, uploadFolder);

            }
            if(!Directory.Exists(realFolderPath)){
                Directory.CreateDirectory(realFolderPath);
            }
            string filePath=Path.Combine(uploadFolder,fileName);
            return filePath;

        }

        public CommonResult Upload(FileUpload info)
        {
            CommonResult result = new CommonResult();
            try
            {
                string relativeSavePath = "";

                if (string.IsNullOrEmpty(info.BasePath))
                {
                    AppConfig config = new AppConfig();
                    string AttachmentBasePath = config.AppConfigGet("AttachmentBasePath");
                    if (string.IsNullOrEmpty(AttachmentBasePath))
                    {
                        info.BasePath = "UploadFiles";
                    }
                    else
                    {
                        info.BasePath = AttachmentBasePath;
                    }
                    relativeSavePath = UploadFile(info);
                }
                else
                {
                    relativeSavePath = info.FileName;
                }

                if (!string.IsNullOrEmpty(relativeSavePath))
                {
                    info.SavePath = relativeSavePath.Trim('\\');
                    info.AddTime = DateTime.Now;

                    try
                    {
                        bool success = base.Insert(info);
                        if (success)
                        {
                            result.Success = success;
                        }
                        else
                        {
                            result.ErrorMessage = "数据写入数据库出错。";
                        }
                    }
                    catch (Exception ex)
                    {
                        result.ErrorMessage = ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public override bool Delete(object key, System.Data.Common.DbTransaction trans = null)
        {
            FileUpload info = FindByID(key, trans);
            if (info != null && !string.IsNullOrEmpty(info.SavePath))
            {
                string serverRealPath = Path.Combine(info.BasePath, info.SavePath.Trim('\\'));
                if (!Path.IsPathRooted(serverRealPath))
                {
                    serverRealPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, serverRealPath);
                    if (File.Exists(serverRealPath))
                    {
                        try
                        {
                            string deletedPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, Path.Combine(info.BasePath, "DeletedFiles"));
                            DirectoryUtil.AssertDirExist(deletedPath);
                            string newFilePath = Path.Combine(deletedPath, info.FileName);
                            newFilePath = GetRightFileName(newFilePath, 1);

                            File.Move(serverRealPath, newFilePath);
                        }
                        catch (Exception ex)
                        {
                            LogTextHelper.Error(ex);
                        }
                    }

                }
            }
            return base.Delete(key, trans);
        }

        private string UploadFile(FileUpload info)
        {
            string filePath = GetFilePath(info);
            string relativeSavePath = filePath.Replace(info.BasePath, "");
            string serverRealPath = filePath;

            if (!Path.IsPathRooted(filePath))
            {
                serverRealPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, filePath);
            }
            serverRealPath = GetRightFileName(serverRealPath, 1);
            relativeSavePath = relativeSavePath.Substring(0, relativeSavePath.LastIndexOf(info.FileName)) + FileUtil.GetFileName(serverRealPath);
            info.FileName = FileUtil.GetFileName(serverRealPath);

            FileUtil.CreateFile(serverRealPath, info.FileData);

            bool success = FileUtil.IsExistFile(serverRealPath);
            if (success)
            {
                return relativeSavePath;
            }
            else
            {
                return string.Empty;
            }
        }

        private string GetRightFileName(string originalFilePath, int i)
        {
            bool fileExist = FileUtil.IsExistFile(originalFilePath);
            if (fileExist)
            {
                string onlyFileName = FileUtil.GetFileName(originalFilePath, true);
                int idx = originalFilePath.LastIndexOf(onlyFileName);
                string firstPath = originalFilePath.Substring(0, idx);
                string onlyExt = FileUtil.GetExtension(originalFilePath);
                string newFileName = string.Format("{0}{1}{2}{3}", firstPath, onlyFileName, i, onlyExt);
                if (FileUtil.IsExistFile(newFileName))
                {
                    i++;
                    return GetRightFileName(originalFilePath, i);
                }
                else
                {
                    return newFileName;
                }
            }
            else
            {
                return originalFilePath;
            }
        }
    }
}
