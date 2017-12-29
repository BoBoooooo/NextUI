using System;
using System.Collections.Generic;
using Next.Framework.Core;

namespace Next.Admin.Entity
{
    [Serializable]
    public partial class User_Role : BaseEntity
    {
        public string ID { get; set; }
        public string UserID { get; set; }
        public string RoleID { get; set; }
        public bool Deleted { get; set; }
    }
}
