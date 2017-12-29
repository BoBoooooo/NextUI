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
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;


namespace Next.Admin.DALMySql
{
    public class RoleDAL : BaseDALMySql<Role>,IRoleDAL
    {
        public static RoleDAL Instance
        {
            get
            {
                return new RoleDAL();
            }
        }
        public RoleDAL() : base("Role", "ID")
        {
            this.sortField = "SortCode";
            this.IsDescending = false;
        }
        public List<Role> GetRolesByUser(string userID)
        {
            string sql = "SELECT * FROM Role INNER JOIN User_Role ON Role.ID=User_Role.RoleID WHERE UserID='" + userID+"'";
            return this.GetList(sql, null);
        }
        
        public List<Role> GetRolesByDept(string deptID)
        {
            string sql = "SELECT * FROM Role INNER JOIN Dept_Role ON Role.ID=RoleID WHERE DeptID='" + deptID + "'";
            return this.GetList(sql, null);
        }
        public bool EditRoleUsers(string roleID, List<string> newUserList)
        {
            string sql = string.Format("Delete from User_Role Where RoleID='{0}'", roleID);
            base.SqlExecute(sql);

            foreach (string userID in newUserList)
            {
                AddUser(userID, roleID);
            }
            return true;
        }
        public bool EditRoleFunctions(string roleID, List<string> newFunctionList)
        {
            string sql = string.Format("Delete from Role_Function Where RoleID='{0}'", roleID);
            base.SqlExecute(sql);

            foreach (string functionID in newFunctionList)
            {
                AddFunction(functionID, roleID);
            }
            return true;
        }
        public bool EditRoleDepts(string roleID, List<string> newDeptList)
        {
            string sql = string.Format("Delete from Dept_Role Where RoleID='{0}'", roleID);
            base.SqlExecute(sql);

            foreach (string userID in newDeptList)
            {
                AddUser(userID, roleID);
            }
            return true;
        }

        public void AddFunction(string functionID, string roleID)
        {
            string commandText = string.Format("Insert Into Role_Function(ID,FunctionID,RoleID) Values('{0}','{1}','{2}')", Guid.NewGuid().ToString(), functionID, roleID);
            Database db = CreateDatabase();
            DbCommand command = db.GetSqlStringCommand(commandText);
            db.ExecuteNonQuery(command);
        }
        public void AddDept(string deptID, string roleID)
        {
            string commandText = string.Format("Insert Into Dept_Role(ID,UserID,RoleID) Values('{0}','{1}','{2}')", Guid.NewGuid().ToString(), deptID, roleID);
            Database db = CreateDatabase();
            DbCommand command = db.GetSqlStringCommand(commandText);
            db.ExecuteNonQuery(command);
        }


        public void AddUser(string userID, string roleID)
        {
            string commandText = string.Format("Insert Into User_Role(ID,UserID,RoleID) Values('{0}','{1}','{2}')", Guid.NewGuid().ToString(), userID, roleID);
            Database db = CreateDatabase();
            DbCommand command = db.GetSqlStringCommand(commandText);
            db.ExecuteNonQuery(command);
        }
        public void RemoveFunction(string functionID, string roleID)
        {
            string commandText = string.Format("Delete From Role_Function Where FunctionID='{0}' And RoleID={1}",  functionID, roleID);
            Database db = CreateDatabase();
            DbCommand command = db.GetSqlStringCommand(commandText);
            db.ExecuteNonQuery(command);
        }
        public void RemoveDept(string deptID, string roleID)
        {
            string commandText = string.Format("Delete From Dept_Role Where DeptID='{0}' And RoleID={1}", deptID, roleID);
            Database db = CreateDatabase();
            DbCommand command = db.GetSqlStringCommand(commandText);
            db.ExecuteNonQuery(command);
        }
        public void RemoveUser(string userID, string roleID)
        {
            string commandText = string.Format("Delete From User_Role Where UserID='{0}' And RoleID='{1}'", userID, roleID);
            Database db = CreateDatabase();
            DbCommand command = db.GetSqlStringCommand(commandText);
            db.ExecuteNonQuery(command);
        }

        public List<Role> GetRolesByFunction(string functionID)
        {
            string sql = string.Format(@"SELECT * FROM Role 
            INNER JOIN Role_Function On Role.ID=Role_Function.RoleID WHERE FunctionID ='{0}' ", functionID);
            return this.GetList(sql, null);
        }
    }
}
