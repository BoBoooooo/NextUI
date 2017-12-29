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


namespace Next.Admin.DALMySql
{
    public class FunctionDAL : BaseDALMySql<Function>,IFunctionDAL
    {
        public static FunctionDAL Instance
        {
            get
            {
                return new FunctionDAL();
            }
        }
        public FunctionDAL() : base("Function", "ID")
        {
            //this.sortField = "SortCode";
            this.IsDescending = false;
        }
        public List<FunctionNode> GetFunctionNodes(string roleIDs, string typeID)
        {
            string sql = @"SELECT * FROM Function INNER JOIN Role_Function ON Function.ID=Role_Function.FunctionID WHERE RoleID IN ('" + roleIDs + "')";
            if (typeID.Length > 0)
            {
                sql = sql + string.Format(" AND SystemTypeID='{0}'", typeID);
            }
            List<FunctionNode> arrReturn = new List<FunctionNode>();
            DataTable dt = base.SqlTable(sql);
            string sortCode = string.Format(" {0} {1} ", GetSafeFileName(sortField), IsDescending ? "DESC" : "ASC");
            DataRow[] dataRows = dt.Select(string.Format(" PID='{0}' ", -1), sortCode);

            for (int i = 0; i < dataRows.Length; i++)
            {
                string id = dataRows[i]["ID"].ToString();
                FunctionNode menuNodeInfo = GetNode(id, dt);
                arrReturn.Add(menuNodeInfo);
            }
            return arrReturn;
        }

        public List<Function> GetFunctions(string roleIDs, string typeID)
        {
            string sql = @"SELECT * FROM Function INNER JOIN Role_Function ON Function.ID=Role_Function.FunctionID WHERE RoleID IN ('" + roleIDs + "')";
            if (typeID.Length > 0)
            {
                sql = sql + string.Format(" AND SystemTypeID='{0}'", typeID);
            }
            return this.GetList(sql,null);
        }

        private FunctionNode GetNode(string id, DataTable dt)
        {
            Function menuInfo = this.FindByID(id);
            FunctionNode menuNodeInfo = new FunctionNode(menuInfo);

            string sortCode = string.Format(" {0} {1} ", GetSafeFileName(sortField), IsDescending ? "DESC" : "ASC");
            DataRow[] dChildRows = dt.Select(string.Format(" PID='{0}' ", id), sortCode);

            for (int i = 0; i < dChildRows.Length; i++)
            {
                string childId = dChildRows[i]["ID"].ToString();
                FunctionNode childNodeInfo = GetNode(childId, dt);
                menuNodeInfo.Children.Add(childNodeInfo);
            }
            return menuNodeInfo;
        }

        public List<FunctionNode> GetTree(string systemType)
        {
            string condition = !string.IsNullOrEmpty(systemType) ? string.Format("Where SystemTypeID='{0}'", systemType) : "";
            List<FunctionNode> arrReturn = new List<FunctionNode>();
            string sql = string.Format("Select * From {0} {1} Order By PID,Name ", tableName, condition);

            DataTable dt = base.SqlTable(sql);
            string sortCode = string.Format("{0} {1}", GetSafeFileName(sortField), IsDescending ? "DESC" : "ASC");

            DataRow[] dataRows = dt.Select(string.Format(" PID='{0}' ", -1), sortCode);

            for (int i = 0; i < dataRows.Length; i++)
            {
                string id = dataRows[i]["ID"].ToString();
                FunctionNode menuNodeInfo = GetNode(id, dt);
                arrReturn.Add(menuNodeInfo);
            }
            return arrReturn;
        }

        public List<Function> GetFunctionsByRole(string roleID)
        {
            string sql = "Select * From Function Left Join Role_Function On Function.ID=Role_Function.FunctionID Where RoleID='" + roleID + "'";
            return this.GetList(sql, null);
        }
    }
}
