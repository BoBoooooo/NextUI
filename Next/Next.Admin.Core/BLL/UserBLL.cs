using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Next.Framework.Core;
using Next.Admin.IDAL;
using Next.Admin.Entity;
using Next.Admin.Model;
using System.Web;


namespace Next.Admin.BLL
{
    public class UserBLL :BaseBLL<User>
    {
        public const string PREFIX = "";
        private IUserDAL userDAL;
        public UserBLL():base()
        {
            base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//如果需要记录操作日志，则实现这个事件
            this.userDAL = (IUserDAL)base.baseDal;
        }

        public bool VerifyUser(User user)
        {
            User userInfo = this.GetUserByName(user.Name);
            if (userInfo != null)
            {
                if (userInfo.Password == user.Password)
                {
                    BLLFactory<LoginLogBLL>.Instance.AddLoginLog(userInfo, "Next框架", (string)HttpContext.Current.Session["IP"], (string)HttpContext.Current.Session["MAC"], "用户登录");
                    return true;
                }
            }
            return false;
        }
        public bool ModifyPassword(string userName, string userPassword)
        {
            bool result = false;
            User info = this.GetUserByName(userName);
            if (info != null)
            {
                info.Password = userPassword;
                result = userDAL.Update(info,info.ID);
            }
            if (result)
            {
                //记录用户修改密码日志
                BLLFactory<LoginLogBLL>.Instance.AddLoginLog(info, "Next框架", (string)HttpContext.Current.Session["IP"], (string)HttpContext.Current.Session["MAC"], "用户修改密码");
            }
            return result;
        }
        public User GetUserByName(string userName)
        {
            User info = null;
            if (!string.IsNullOrEmpty(userName))
            {
                string condition = string.Format("Name='{0}'", userName);
                info = this.userDAL.FindSingle(condition);
            }
            return info;
        }

        public bool UserInRole(string userName, string roleName)
        {
            User userInfo = this.GetUserByName(userName);
            foreach (Role info in BLLFactory<RoleBLL>.Instance.GetRolesByUser(userInfo.ID))
            {
                if(info.Name==roleName){
                    return true;
                }
            }
            return false;
        }
        public List<User> GetUsersByRole(string roleID)
        {
            return this.userDAL.GetUsersByRole(roleID);
        }

        public List<User> FindByDept(string deptID)
        {
            string condition = string.Format("DeptID='{0}'", deptID);
            return base.Find(condition);
        }
        public List<User> GetUsersByDept(string deptID)
        {
            return this.userDAL.GetUsersByDept(deptID);
        }

        public List<SimpleUser> GetSimpleUsersByRole(string roleID)
        {
            return this.userDAL.GetSimpleUsersByRole(roleID);
        }
        public List<SimpleUser> GetSimpleUsersByDept(string deptID)
        {
            return this.userDAL.GetSimpleUsersByDept(deptID);
        }

    }
}
