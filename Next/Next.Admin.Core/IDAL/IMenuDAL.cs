using Next.Admin.Entity;
using Next.Admin.Model;
using Next.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Admin.IDAL
{
    public interface IMenuDAL:IBaseDAL<Menu>
    {
        List<Menu> GetTopMenu();
        List<MenuNode> GetTreeByID(string mainMenuID);

        List<MenuNode> GetTree(string systemType = "");
    }
}
