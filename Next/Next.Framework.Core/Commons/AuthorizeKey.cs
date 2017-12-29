using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Framework.Core.Commons
{
    public class AuthorizeKey
    {
        public string InsertKey { get; set; }
        public string UpdateKey { get; set; }
        public string DeleteKey { get; set; }

        public string ListKey { get; set; }
        public string ViewKey { get; set; }
        public string ExportKey { get; set; }

        public bool CanInsert { get; set; }

        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        public bool CanList { get; set; }
        public bool CanView { get; set; }
        public bool CanExport { get; set; }

        public AuthorizeKey() { }
        public AuthorizeKey(string insert, string update, string delete, string view)
        {
            this.InsertKey = insert;
            this.UpdateKey = update;
            this.DeleteKey = delete;
            this.ViewKey = view;
        }

    }
}
