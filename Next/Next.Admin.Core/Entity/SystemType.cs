using Next.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Admin.Entity
{
    [Serializable]
    public partial class SystemType : BaseEntity
    {
        public string ID { get; set; }
        public string Name { get; set; }

        public string CustomID { get; set; }
    }

}
