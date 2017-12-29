using Next.Admin.IDAL;
using Next.Admin.Entity;
using Next.Admin.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Next.Framework.Core;

namespace Next.Admin.IDAL
{
    public interface IFunctionDAL:IBaseDAL<Function>
    {
        List<FunctionNode> GetFunctionNodes(string roleIDs, string typeID);

        List<FunctionNode> GetTree(string systemType);

        List<Function> GetFunctionsByRole(string roleID);

        List<Function> GetFunctions(string roleIDs, string typeID);
    }
}
