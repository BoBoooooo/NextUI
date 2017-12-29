using Next.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Attachment.Entity
{
    public class FileUpload : BaseEntity
    {
        public string ID { get; set; }
        public string AttachmentID { get; set; }

        public string FileName { get; set; }
        public string BasePath { get; set; }
        public string SavePath { get; set; }
        public string Category { get; set; }
        public int FileSize { get; set; }
        public string FileExtend { get; set; }
        public string Editor { get; set; }

        public DateTime AddTime { get; set; }

        public byte[] FileData { get; set; }
        public bool Deleted { get; set; }
    }
}
