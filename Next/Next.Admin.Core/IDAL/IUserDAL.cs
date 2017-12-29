using Next.Admin.Entity;
using Next.Admin.Model;
using Next.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Admin.IDAL
{
    public interface IUserDAL:IBaseDAL<User>
    {
        List<User> GetUsersByRole(string roleID);
        List<User> GetUsersByDept(string deptID);

        List<SimpleUser> GetSimpleUsersByRole(string roleID);

        List<SimpleUser> GetSimpleUsersByDept(string roleID);


    }
}
