using Next.Framework.Core;
using System;
using System.Collections.Generic;

namespace Next.Admin.Entity
{

    [Serializable]
    public partial class Dept_Role : BaseEntity
    {
        public string ID { get; set; }
        public string DeptID { get; set; }
        public string RoleID { get; set; }
        public bool Deleted { get; set; }
    }
}
