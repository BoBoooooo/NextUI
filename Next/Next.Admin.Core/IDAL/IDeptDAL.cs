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
    public interface IDeptDAL:IBaseDAL<Dept>
    {
        List<Dept> GetDeptsByUser(string userID);

        List<DeptNode> GetTreeByID(string mainDeptID);

        bool EditDeptUsers(string deptID, List<string> newUserList);

        void AddUser(string userID, string deptID);

        void RemoveUser(string userID, string deptID);

        List<Dept> GetDeptsByRole(string roleID);
        List<Dept> GetChilds(string ID);

    }
}
