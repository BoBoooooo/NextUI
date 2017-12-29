using Next.Admin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Admin.Model
{
    public class MenuNode:Menu
    {

        public List<MenuNode> Children { get; set; }
        public MenuNode()
        {
            Children = new List<MenuNode>();
        }
        public MenuNode(Menu menu)
        {
            base.ID = menu.ID;
            base.PID = menu.PID;
            base.Name = menu.Name;
            base.FunctionID = menu.FunctionID;
            base.Url = menu.Url;
            base.WebIcon = menu.WebIcon;
            base.SortCode = menu.SortCode;
            base.Deleted = menu.Deleted;
            Children = new List<MenuNode>();
        }
    }
}
