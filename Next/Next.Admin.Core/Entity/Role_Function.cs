using Next.Framework.Core;
using System;
using System.Collections.Generic;

namespace Next.Admin.Entity
{

    [Serializable]
    public partial class Role_Function : BaseEntity
    {
        public string ID { get; set; }
        public string RoleID { get; set; }
        public string FunctionID { get; set; }
        public bool Deleted { get; set; }
    }
}
