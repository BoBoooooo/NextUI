using System;
using System.Collections.Generic;
using Next.Framework.Core;

namespace Next.Admin.Entity
{

    [Serializable]
    public partial class Menu : BaseEntity
    {
        public string ID { get; set; }
        public string PID { get; set; }
        public string Name { get; set; }

        public string Url { get; set; }
        public string WebIcon { get; set; }

        public string SortCode { get; set; }
        public string FunctionID { get; set; }

        public string SystemTypeID { get; set; }
        public bool Deleted { get; set; }
    }
}
