using Next.Framework.Core;
using System;
using System.Collections.Generic;

namespace Next.Admin.Entity
{
    [Serializable]
    public partial class Dept : BaseEntity
    {
        public string ID { get; set; }
        public string PID { get; set; }
        public string Name { get; set; }

        public string Category { get; set; }
        public string SortCode { get; set; }
        public bool Deleted { get; set; }
    }
}
