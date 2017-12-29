using System;
using System.Collections.Generic;
using Next.Framework.Core;

namespace Next.Admin.Entity
{

    [Serializable]
    public partial class Dept_User : BaseEntity
    {
        public string ID { get; set; }
        public string DeptID { get; set; }
        public string UserID { get; set; }
        public bool Deleted { get; set; }
    }
}
