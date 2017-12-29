using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Next.Admin.IDAL;
using Next.Framework.Core;
using Next.Admin.Entity;
using System.Data;
using Next.Framework.Core.Commons;
using System.Collections;
using Next.Admin.Model;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;


namespace Next.Admin.DALMySql
{
    public class UserDAL : BaseDALMySql<User>,IUserDAL
    {
        public static UserDAL Instance
        {
            get
            {
                return new UserDAL();
            }
        }
        public UserDAL(): base("User","ID")
        {
            this.sortField = "SortCode";
            this.IsDescending = false;
        }
        public List<User> GetUsersByRole(string roleID)
        {
            string sql = "SELECT * FROM `User` INNER JOIN User_Role On `User`.ID=User_Role.UserID WHERE RoleID='" + roleID + "'";
            return this.GetList(sql, null);
        }

        public List<User> GetUsersByDept(string deptID)
        {
            string sql = "SELECT * FROM `User` INNER JOIN Dept_User ON User.ID=Dept_User.UserID WHERE Dept_User.DeptID='" + deptID + "'";
            return this.GetList(sql, null);
        }
        public List<SimpleUser> GetSimpleUsersByRole(string roleID)
        {
            string sql = "SELECT ID,Name,Password,FullName From `User` INNER JOIN User_Role ON `User`.ID=UserID Where RoleID='" + roleID + "'";
            return this.FillSimpleUsers(sql);
        }
        public List<SimpleUser> GetSimpleUsersByDept(string deptID)
        {
            string sql = "SELECT ID,Name,Password,FullName From `User` INNER JOIN Dept_User ON `User`.ID=UserID Where DeptID='" + deptID + "'";
            return this.FillSimpleUsers(sql);
        }
        private List<SimpleUser> FillSimpleUsers(string sql)
        {
            Database db = CreateDatabase();
            DbCommand command = db.GetSqlStringCommand(sql);

            List<SimpleUser> list = new List<SimpleUser>();
            using (IDataReader reader = db.ExecuteReader(command))
            {
                SmartDataReader dr = new SmartDataReader(reader);
                while (reader.Read())
                {
                    SimpleUser info = new SimpleUser();
                    info.ID = dr.GetString("ID");
                    info.Name = dr.GetString("Name");
                    info.Password = dr.GetString("Password");
                    info.FullName = dr.GetString("FullName");
                    list.Add(info);
                }
            }
            return list;
        }
    }
}
