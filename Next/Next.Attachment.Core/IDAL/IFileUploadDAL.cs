using Next.Attachment.Entity;
using Next.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Attachment.IDAL
{
    public interface IFileUploadDAL : IBaseDAL<FileUpload>
    {

        List<FileUpload> GetByAttachGuid(string attachmentGuid);
    }
}
