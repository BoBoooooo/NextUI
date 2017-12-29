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
    public class FunctionBLL :BaseBLL<Function>
    {
        private IFunctionDAL functionDAL;
        public FunctionBLL()
            : base()
        {
            base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//如果需要记录操作日志，则实现这个事件
            this.functionDAL = (IFunctionDAL)base.baseDal;
        }
        public List<Function> GetFunctionsByUser(string userID,string typeID)
        {
            List<Role> rolesByUser = BLLFactory<RoleBLL>.Instance.GetRolesByUser(userID);
            string roleIDs = ",";
            foreach (Role info in rolesByUser)
            {
                roleIDs = roleIDs + info.ID + "','";
            }
            roleIDs = roleIDs.Trim(',');
            List<Function> functions = new List<Function>();
            if (!string.IsNullOrEmpty(roleIDs))
            {
                functions = this.GetFunctions(roleIDs,typeID);
            }
            return functions;
        }

        public List<Function> GetFunctions(string roleIDs,string typeID)
        {
            if (roleIDs == string.Empty)
            {
                roleIDs = "-1";
            }
            return this.functionDAL.GetFunctions(roleIDs,typeID);
        }
        public List<FunctionNode> GetFunctionNodesByUser(string userID, string typeID)
        {
            List<Role> rolesByUser = BLLFactory<RoleBLL>.Instance.GetRolesByUser(userID);
            string roleIDs = ",";
            foreach (Role info in rolesByUser)
            {
                roleIDs = roleIDs + info.ID + ",";
            }
            roleIDs = roleIDs.Trim(',');
            List<FunctionNode> functions = new List<FunctionNode>();
            if (!string.IsNullOrEmpty(roleIDs))
            {
                functions = this.GetFunctionNodes(roleIDs, typeID);
            }
            return functions;
        }

        public List<FunctionNode> GetFunctionNodes(string roleIDs, string typeID)
        {
            if (roleIDs == string.Empty)
            {
                roleIDs = "-1";
            }
            return this.functionDAL.GetFunctionNodes(roleIDs, typeID);
        }

        public List<FunctionNode> GetTree(string systemType)
        {
            return functionDAL.GetTree(systemType);
        }
        public List<Function> GetFunctionsByRole(string roleID)
        {
            return this.functionDAL.GetFunctionsByRole(roleID);
        }

    }
}
