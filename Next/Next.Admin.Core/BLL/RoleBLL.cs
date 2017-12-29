using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Next.Framework.Core;
using Next.Admin.IDAL;
using Next.Admin.Entity;
using Next.Admin.Model;
using System.Data.Common;


namespace Next.Admin.BLL
{
    public class RoleBLL : BaseBLL<Role>
    {
        private IRoleDAL roleDAL;
        public RoleBLL()
            : base()
        {
            base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//如果需要记录操作日志，则实现这个事件
            this.roleDAL = (IRoleDAL)base.baseDal;
        }

        public List<Role> GetRolesByUser(string userID)
        {
            List<Role> rolesByUser = this.roleDAL.GetRolesByUser(userID);
            List<string> list = new List<string>();
            foreach (Role info in rolesByUser)
            {
                list.Add(info.ID);
            }

            foreach (Dept deptInfo in BLLFactory<DeptBLL>.Instance.GetDeptsByUser(userID))
            {
                foreach (Role roleInfo in this.roleDAL.GetRolesByDept(deptInfo.ID))
                {
                    if (!list.Contains(roleInfo.ID))
                    {
                        rolesByUser.Add(roleInfo);
                        list.Add(roleInfo.ID);
                    }
                }
            }
            User userInfo = BLLFactory<UserBLL>.Instance.FindByID(userID);
            if (userInfo != null)
            {
                foreach (Role roleInfo in this.roleDAL.GetRolesByDept(userInfo.DeptID))
                {
                    if (!list.Contains(roleInfo.ID))
                    {
                        rolesByUser.Add(roleInfo);
                        list.Add(roleInfo.ID);
                    }
                }
            }
            return rolesByUser;

        }
        public List<Role> GetRolesByDept(string deptID)
        {
            return this.roleDAL.GetRolesByDept(deptID);
        }
        public List<Role> GetRolesByFunction(string functionID)
        {
            return this.roleDAL.GetRolesByFunction(functionID);
        }
        public List<Role> GetRolesByCompany(string companyID)
        {
            string condition = string.Format("CompanyID='{0}' and Deleted=0", companyID);
            return Find(condition);
        }
        public bool EditRoleUsers(string roleID, List<string> newUserList)
        {
            return roleDAL.EditRoleUsers(roleID, newUserList);
        }
        public bool EditRoleDepts(string roleID, List<string> newDeptList)
        {
            return roleDAL.EditRoleDepts(roleID, newDeptList);
        }

        public bool EditRoleFunctions(string roleID, List<string> newFunctionList)
        {
            return roleDAL.EditRoleFunctions(roleID, newFunctionList);
        }
        internal List<SimpleUser> GetAdminSimpleUsers()
        {
            this.FillAdminID();

            List<SimpleUser> simpleUsersByRole = BLLFactory<UserBLL>.Instance.GetSimpleUsersByRole(m_AdminID);
            int count = simpleUsersByRole.Count;
            if (count <= 1)
            {
                foreach (Dept info in BLLFactory<DeptBLL>.Instance.GetDeptsByRole(m_AdminID))
                {
                    List<SimpleUser> simpleUserByDept = BLLFactory<UserBLL>.Instance.GetSimpleUsersByDept(info.ID);
                    if (simpleUserByDept.Count > 0)
                    {
                        simpleUsersByRole.Add(simpleUserByDept[0]);
                        count++;
                        if (simpleUserByDept.Count > 1)
                        {
                            simpleUsersByRole.Add(simpleUserByDept[1]);
                            count++;
                        }
                        if (count > 1)
                        {
                            return simpleUsersByRole;
                        }
                    }
                }
                
            }
            return simpleUsersByRole;
        }

        public void RemoveUser(string userID, string roleID)
        {
            this.CanRemoveFromAdmin(roleID);
            this.roleDAL.RemoveUser(userID, roleID);
        }

        private void CanRemoveFromAdmin(string roleID)
        {
            this.FillAdminID();
            if ((roleID == m_AdminID) && (this.GetAdminSimpleUsers().Count <= 1))
            {
                throw new Exception("管理员角色至少需要包含一个用户！");
            }
        }

        public void AddUser(string userID,string roleID){
            this.FillAdminID();
            if(roleID==m_AdminID){
                //BLLFactory<UserBLL>.Instance.CancelExpire(userID);
            }
            roleDAL.AddUser(userID,roleID);
        }

        public void AddDept(string deptID,string roleID){
            roleDAL.AddDept(deptID, roleID);
        }

        public void AddFunction(string functionID,string roleID){
            this.roleDAL.AddFunction(functionID,roleID);
        }
        public void RemoveDept(string deptID, string roleID)
        {
            this.FillAdminID();
            if (roleID == m_AdminID)
            {
                List<SimpleUser> simpleUsersByRole = BLLFactory<UserBLL>.Instance.GetSimpleUsersByRole(m_AdminID);
                if (simpleUsersByRole.Count < 1)
                {
                    simpleUsersByRole.Clear();
                    List<User> userByDept = BLLFactory<UserBLL>.Instance.GetUsersByDept(deptID);
                    if (userByDept.Count > 0)
                    {
                        userByDept.Clear();
                        bool flag = false;
                        List<Dept> deptsByRole = BLLFactory<DeptBLL>.Instance.GetDeptsByRole(m_AdminID);
                        foreach (Dept info in deptsByRole)
                        {
                            if((info.ID!=deptID)&&(BLLFactory<UserBLL>.Instance.GetSimpleUsersByDept(info.ID).Count>0))
                            {
                                flag=true;
                                break;
                            }
                        }
                        deptsByRole.Clear();
                        if(!flag){
                            throw new Exception("管理员角色至少需要包含一个用户");
                        }
                    }
                }
            }
            roleDAL.RemoveDept(deptID, roleID);
        }

        public void RemoveFunction(string functionID, string roleID)
        {
            this.roleDAL.RemoveFunction(functionID, roleID);
        }
        private void FillAdminID(DbTransaction trans = null)
        {
            if (m_AdminID == "-1")
            {
                string condition = string.Format("Name='{0}'", Role.SuperAdminName);
                Role roleByName = FindSingle(condition, trans);
                if (roleByName != null)
                {
                    m_AdminID = roleByName.ID;
                }
            }
        }
        private static string m_AdminID = "-1";
    }
}
