using Next.Attachment.Entity;
using Next.Attachment.IDAL;
using Next.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Attachment.DALMySql
{
    public class FileUploadDAL : BaseDALMySql<FileUpload>, IFileUploadDAL
    {
        public static FileUploadDAL Instance
        {
            get
            {
                return new FileUploadDAL();
            }
        }
        public FileUploadDAL()
            : base("FileUpload", "ID")
        {
            this.sortField = "ID";
            this.IsDescending = false;
        }

        public List<FileUpload> GetByAttachGuid(string attachmentGuid)
        {
            if (string.IsNullOrEmpty(attachmentGuid))
            {
                throw new ArgumentException("附件组Guid不能为空", attachmentGuid);
            }
            else
            {
                string condition = string.Format("AttachmentID='{0}' ", attachmentGuid);
                return Find(condition);
            }
        }
    }
}
