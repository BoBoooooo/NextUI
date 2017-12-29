using Next.Framework.Core;
using Next.Framework.Core.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Admin.Entity
{
    [Serializable]
    public class User:BaseEntity
    {

        public const int IdentityLen = 50;

        public string ID { get; set; }
        public string PID { get; set; }
        public string DeptID { get; set; }

        public string DeptName { get; set; }
        public String Name { get; set; }
        public String Password { get; set; }
        public String FullName { get; set; }

        public string CompanyID { get; set; }

        public string CompanyName { get; set; }
        public String SortCode { get; set; }

        public DateTime EditTime { get; set; }

        public bool Deleted { get; set; }

    }

    public class UserExtension : User
    {
        public UserExtension(User user){
            ObjectCopy.CopyTo(user, this);
        }
    }
}
