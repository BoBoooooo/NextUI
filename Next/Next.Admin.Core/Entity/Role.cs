using System;
using System.Collections.Generic;
using Next.Framework.Core;

namespace Next.Admin.Entity
{

    [Serializable]
    public partial class Role : BaseEntity
    {
        public const string SuperAdminName = "��������Ա";
        public const string CompanyAdminName = "ϵͳ����Ա";
        public string ID { get; set; }
        public string PID { get; set; }
        public string Name { get; set; }
        public string SortCode { get; set; }

        public string CompanyID { get; set; }

        public string Category { get; set; }
        public bool Deleted { get; set; }
    }
}
