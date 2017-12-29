using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Admin.Model
{
    public class SimpleUser
    {
        public virtual string ID { get; set; }
        public virtual string Name { get; set; }

        public virtual string Password { get; set; }

        public virtual string FullName { get; set; }
    }
}
