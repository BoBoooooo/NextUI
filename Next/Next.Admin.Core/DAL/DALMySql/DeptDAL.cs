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
    public class DeptDAL : BaseDALMySql<Dept>,IDeptDAL
    {
        public static DeptDAL Instance
        {
            get
            {
                return new DeptDAL();
            }
        }
        public DeptDAL() : base("Dept", "ID")
        {
            this.sortField = "SortCode";
            this.IsDescending = false;
        }
        public List<Dept> GetDeptsByUser(string userID)
        {
            string sql = "SELECT * FROM Dept INNER JOIN Dept_User ON Dept.ID=DeptID WHERE UserID= '" + userID + "'";
            return this.GetList(sql, null);
        }

        public List<DeptNode> GetTreeByID(string mainDeptID)
        {
            List<DeptNode> arrReturn = new List<DeptNode>();
            string sql = string.Format("Select * From {0} Order By PID,Name", tableName);
            DataTable dt = SqlTable(sql);
            string sort = string.Format("{0} {1}", GetSafeFileName(sortField), IsDescending ? "DESC" : "ASC");
            DataRow[] dataRows = dt.Select(string.Format(" PID='{0}' ", mainDeptID), sort);
            for (int i = 0; i < dataRows.Length; i++)
            {
                string id = (string)dataRows[i]["ID"];
                DeptNode menuNode = GetNode(id, dt);
                arrReturn.Add(menuNode);
            }
            return arrReturn;
        }
        public DeptNode GetNode(string id, DataTable dt)
        {
            Dept deptInfo = this.FindByID(id);
            DeptNode deptNodeInfo = new DeptNode(deptInfo);

            string sort = string.Format("{0} {1}", GetSafeFileName(sortField), IsDescending ? "DESC" : "ASC");
            DataRow[] dChildRows = dt.Select(string.Format(" PID='{0}' ", id), sort);
            for (int i = 0; i < dChildRows.Length; i++)
            {
                string childId = (string)dChildRows[i]["ID"];
                DeptNode childNode = GetNode(childId, dt);
                deptNodeInfo.Children.Add(childNode);
            }
            return deptNodeInfo;
        }

        public bool EditDeptUsers(string deptID, List<string> newUserList)
        {
            string sql = string.Format("Delete from Dept_User Where DeptID='{0}'", deptID);
            base.SqlExecute(sql);

            foreach (string userID in newUserList)
            {
                AddUser(userID, deptID);
            }
            return true;
        }

        public void AddUser(string userID, string deptID)
        {
            string commandText = string.Format("Insert Into Dept_User(ID,UserID,DeptID) Values('{0}','{1}','{2}')", Guid.NewGuid().ToString(), userID, deptID);
            Database db = CreateDatabase();
            DbCommand command = db.GetSqlStringCommand(commandText);
            db.ExecuteNonQuery(command);
        }
        public void RemoveUser(string userID, string deptID)
        {
            string commandText = string.Format("Delete From Dept_User Where UserID='{0}' AND DeptID='{1}'", userID, deptID);
            Database db = CreateDatabase();
            DbCommand command = db.GetSqlStringCommand(commandText);
            db.ExecuteNonQuery(command);
        }
        public List<Dept> GetDeptsByRole(string roleID)
        {
            string sql = "Select * From Dept Inner Join Dept_Role On Dept.ID=Dept_Role.DeptID Where RoleID='" + roleID + "'";
            return this.GetList(sql, null);
        }
        /// <summary>
        /// 查询下级记录
        /// </summary>
        public List<Dept> GetChilds(string ID)
        {
            string sql = "SELECT * FROM Dept WHERE PID='" + ID + "'";
            return this.GetList(sql, null);

        }
    }
}
