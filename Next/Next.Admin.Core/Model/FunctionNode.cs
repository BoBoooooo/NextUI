using Next.Admin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Admin.Model
{
    public class FunctionNode:Function
    {

        public List<FunctionNode> Children { get; set; }
        public FunctionNode()
        {
            Children = new List<FunctionNode>();
        }
        public FunctionNode(Function function)
        {
            base.ID = function.ID;
            base.ControlID = function.ControlID;
            base.Name = function.Name;
            base.SystemTypeID = function.SystemTypeID;
            base.SortCode = function.SortCode;

            Children = new List<FunctionNode>();
        }
    }
}
