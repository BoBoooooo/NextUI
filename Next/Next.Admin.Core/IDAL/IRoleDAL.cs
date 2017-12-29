using Next.Admin.Entity;
using Next.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Admin.IDAL
{
    public interface IRoleDAL:IBaseDAL<Role>
    {
        List<Role> GetRolesByUser(string userID);

        List<Role> GetRolesByDept(string deptID);
        List<Role> GetRolesByFunction(string functionID);
        bool EditRoleUsers(string roleID, List<string> newUserList);
        bool EditRoleFunctions(string roleID, List<string> newFunctionList);
        bool EditRoleDepts(string roleID, List<string> newDeptList);

        void AddFunction(string functionID, string roleID);
        void AddDept(string deptID, string roleID);
        void AddUser(string userID, string roleID);
        void RemoveFunction(string functionID, string roleID);
        void RemoveDept(string deptID, string roleID);
        void RemoveUser(string userID, string roleID);
    }
}
