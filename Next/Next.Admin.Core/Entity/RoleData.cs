using Next.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Admin.Entity
{
    [Serializable]
    public partial class RoleData : BaseEntity
    {
        public string ID { get; set; }
        public string RoleID { get; set; }

        public string BelongCompanys { get; set; }

        public string BelongDepts { get; set; }

        public string ExcludeDepts { get; set; }

    }

}
