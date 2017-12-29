using Next.Admin.Entity;
using Next.Framework.Core.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Admin.Model
{
    public class DeptNode:Dept
    {
        public List<DeptNode> Children { get; set; }

        public DeptNode(Dept info)
        {
            Children = new List<DeptNode>();
            ObjectCopy.CopyTo<Dept, DeptNode>(info, this);
        }
        public DeptNode()
        {
            Children = new List<DeptNode>();
        }
    }
}
