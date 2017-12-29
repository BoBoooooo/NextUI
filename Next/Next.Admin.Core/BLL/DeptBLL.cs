using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Next.Framework.Core;
using Next.Admin.IDAL;
using Next.Admin.Entity;
using Next.Admin.Model;


namespace Next.Admin.BLL
{
    public class DeptBLL :BaseBLL<Dept>
    {
        private IDeptDAL deptDAL;
        public DeptBLL()
            : base()
        {
            base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//如果需要记录操作日志，则实现这个事件
            this.deptDAL = (IDeptDAL)base.baseDal;
        }

        
        public List<Dept> GetDeptsByUser(string userID)
        {
            return this.deptDAL.GetDeptsByUser(userID);
        }
        public List<DeptNode> GetTreeByID(string mainDeptID)
        {
            return deptDAL.GetTreeByID(mainDeptID);
        }
        public Dept GetTopGroup()
        {
            string condition = string.Format(" PID='-1'");
            return FindSingle(condition);
        }

        public List<Dept> GetAllCompany()
        {
            string condition = string.Format("Category='单位'");
            return Find(condition);
        }

        public List<DeptNode> GetGroupCompanyTree()
        {
            List<DeptNode> list = new List<DeptNode>();

            Dept groupDept = GetTopGroup();
            if (groupDept != null)
            {
                DeptNode groupNodeInfo = new DeptNode(groupDept);

                List<Dept> complayList = GetAllCompany();
                foreach (Dept info in complayList)
                {
                    groupNodeInfo.Children.Add(new DeptNode(info));
                }
                list.Add(groupNodeInfo);
            }
            return list;
        }

        public bool EditDeptUsers(string deptID, List<string> newUserList)
        {
            return deptDAL.EditDeptUsers(deptID, newUserList);
        }

        public void RemoveUser(string userID, string deptID)
        {
            if (this.DeptInRole(deptID, Role.SuperAdminName))
            {
                List<SimpleUser> adminSimpleUsers = BLLFactory<RoleBLL>.Instance.GetAdminSimpleUsers();
                if (adminSimpleUsers.Count == 1)
                {
                    SimpleUser info = (SimpleUser)adminSimpleUsers[0];
                    if (userID == info.ID)
                    {
                        throw new Exception("管理员角色至少需要包含一个用户！");
                    }
                }
            }
            deptDAL.RemoveUser(userID, deptID);
        }
        public void AddUser(string userID, string deptID)
        {
            /*if (this.DeptInRole(deptID, Role.SuperAdminName))
            {
                BLLFactory<UserBLL>.Instance.CancelExpire(userID);
            }*/
            this.deptDAL.AddUser(userID, deptID);
        }
        public List<Dept> GetDeptsByRole(string roleID)
        {
            return this.deptDAL.GetDeptsByRole(roleID);
        }
        public bool DeptInRole(string deptID, string roleName)
        {
            bool result = false;
            List<Role> roleByDept = BLLFactory<RoleBLL>.Instance.GetRolesByDept(deptID);
            foreach (Role info in roleByDept)
            {
                if (info.Name == roleName)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 查询下级记录
        /// </summary>
        public List<Dept> GetChilds(string ID)
        {
            return deptDAL.GetChilds(ID);
        }

        /// <summary>
        /// 得到一组机构字符串下所有人员
        /// </summary>
        /// <param name="idString"></param>
        /// <returns></returns>
        public List<User> GetAllUsers(string idString)
        {
            if (string.IsNullOrEmpty(idString))
            {
                return new List<User>();
            }
            string[] idArray = idString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<User> userList = new List<User>();
            UserBLL busers = new UserBLL();

            foreach (string id in idArray)
            {
                 userList.AddRange(busers.GetUsersByDept(id));
            }
            userList.RemoveAll(p => p == null);
            return userList.Distinct(new UsersEqualityComparer()).ToList();
        }

        public class UsersEqualityComparer : IEqualityComparer<User>
        {
            public bool Equals(User user1, User user2)
            {
                return user1 == null || user2 == null || user1.ID == user2.ID;
            }
            public int GetHashCode(User user)
            {
                return user.ToString().GetHashCode();
            }
        }
    }
}
