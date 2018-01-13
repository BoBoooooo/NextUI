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
using Next.WorkFlow.Utility;

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

        /// <summary>
        /// 去除ID前缀
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string RemovePrefix(string id)
        {
            return id.IsNullOrEmpty() ? "" : id;// id.Replace(PREFIX, "");
        }

        /// <summary>
        /// 得到当前登录用户
        /// </summary>
        public User CurrentUser
        {
            get
            {
                object obj = System.Web.HttpContext.Current.Session["UserInfo"];
                return obj as User;
            }
        }
        /// <summary>
        /// 得到当前登录用户ID
        /// </summary>
        public string CurrentUserID
        {
            get
            {
                object session = System.Web.HttpContext.Current.Session["UserID"];
                return session == null ? string.Empty : session.ToString();
            }
        }
        /// <summary>
        /// 当前用户部门ID
        /// </summary>
        public string CurrentDeptID
        {
            get
            {
                var dept = new UserBLL().FindByID(CurrentUserID);
                return dept == null ? string.Empty : dept.DeptID;
            }
        }

        /// <summary>
        /// 当前用户名称
        /// </summary>
        public string CurrentUserName
        {
            get
            {
                var user = new UserBLL().FindByID(CurrentUserID);
                return user == null ? string.Empty : user.FullName;
            }
        }

                /// <summary>
        /// 判断一个人员是否是部门主管
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool IsLeader(string userID)
        {
            //string leader = GetLeader(userID);
            return true;// leader.Contains(userID.ToString(), StringComparison.CurrentCultureIgnoreCase);
        }
        /// <summary>
        /// 判断一个人员是否是部门分管领导
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool IsChargeLeader(string userID)
        {
            //string leader = GetChargeLeader(userID);
            return true;// leader.Contains(userID.ToString(), StringComparison.CurrentCultureIgnoreCase);
        }
        /// <summary>
        /// 判断一个人员是否在一个组织机构字符串里
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="memberString"></param>
        /// <returns></returns>
        public bool IsContains(string userID, string memberString)
        {
            if (memberString.IsNullOrEmpty())
            {
                return false;
            }
            var user = new DeptBLL().GetAllUsers(memberString).Find(p => p.ID == userID);
            return user != null;
        }
        /// <summary>
        /// 得到一个人员的主管
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public string GetLeader(string userID)
        {
            /*var mainStation = GetMainStation(userID);
            if (mainStation == null)
            {
                return "";
            }
            DeptBLL borg = new DeptBLL();
            var station = borg.FindByID(mainStation);
            if (station == null)
            {
                return "";
            }
            if (!station.Leader.IsNullOrEmpty())
            {
                return station.Leader;
            }
            var parents = borg.GetAllParent(station.Number);
            foreach (var parent in parents)
            {
                if (!parent.Leader.IsNullOrEmpty())
                {
                    return parent.Leader;
                }
            }*/
            return "";
        }

        /// <summary>
        /// 得到一个人员的分管领导
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public string GetChargeLeader(string userID)
        {

            
            /*var mainStation = GetMainStation(userID);
            if (mainStation == null)
            {
                return "";
            }
            DeptBLL borg = new DeptBLL();
            var station = borg.FindByID(mainStation);
            if (station == null)
            {
                return "";
            }
            if (!station.ChargeLeader.IsNullOrEmpty())
            {
                return station.ChargeLeader;
            }
            var parents = borg.GetAllParent(station.Number);
            foreach (var parent in parents)
            {
                if (!parent.ChargeLeader.IsNullOrEmpty())
                {
                    return parent.ChargeLeader;
                }
            }*/
            return "";
        }
        /// <summary>
        /// 得到一个用户的主要岗位
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public string GetMainStation(string userID)
        {
            //var ur = new UsersRelation().GetMainByUserID(userID);
            //return ur == null ? Guid.Empty : ur.OrganizeID;
            return "";
        }
    }
    
}
