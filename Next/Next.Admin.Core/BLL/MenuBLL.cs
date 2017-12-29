using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Next.Admin.IDAL;
using Next.Admin.Entity;
using Next.Framework.Core;
using Next.Admin.Model;

namespace Next.Admin.BLL
{
    public class MenuBLL :BaseBLL<Menu>
    {
        private IMenuDAL menuDAL;
        public MenuBLL():base()
        {
            base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//如果需要记录操作日志，则实现这个事件
            this.menuDAL = (IMenuDAL)base.baseDal;
        }

        public List<Menu> GetTopMenu()
        {
            return menuDAL.GetTopMenu();
        }
        public List<MenuNode> GetTreeByID(string mainMenuID)
        {
            return menuDAL.GetTreeByID(mainMenuID);
        }

        public List<MenuNode> GetTree(string systemType)
        {
            return menuDAL.GetTree(systemType);
        }
    
    }
}
