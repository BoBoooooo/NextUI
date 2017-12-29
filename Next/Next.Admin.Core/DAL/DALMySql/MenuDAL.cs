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
    public class MenuDAL : BaseDALMySql<Menu>,IMenuDAL
    {
        public static MenuDAL Instance
        {
            get
            {
                return new MenuDAL();
            }
        }
        public MenuDAL(): base("Menu","ID")
        {
            this.sortField = "SortCode";
            this.IsDescending = false;
        }

        public List<Menu> GetTopMenu()
        {
            //string condition=!string.IsNullOrEmpty
            string sql = string.Format("Select * From {0} Where PID='-1' Order By SortCode", tableName);
            return GetList(sql, null);

        }
        public List<MenuNode> GetTreeByID(string mainMenuID)
        {
            List<MenuNode> arrReturn = new List<MenuNode>();
            string sql = string.Format("Select * From {0} Where Deleted=0 Order By PID,SortCode", tableName);
            DataTable dt = SqlTable(sql);
            string sort=string.Format("{0} {1}",GetSafeFileName(sortField),IsDescending? "DESC":"ASC");
            DataRow[] dataRows= dt.Select(string.Format(" PID='{0}' ",mainMenuID),sort);
            for(int i=0;i<dataRows.Length;i++){
                string id = (string)dataRows[i]["ID"];
                MenuNode menuNode=GetNode(id,dt);
                arrReturn.Add(menuNode);
            }
            return arrReturn;
        }

        private MenuNode GetNode(string id, DataTable dt)
        {
            Menu menuInfo = this.FindByID(id);
            MenuNode menuNode = new MenuNode(menuInfo);
            string sort=string.Format("{0} {1}",GetSafeFileName(sortField),IsDescending?"DESC":"ASC");
            DataRow[] dChildRows=dt.Select(string.Format(" PID='{0}'",id),sort);

            for(int i=0;i<dChildRows.Length;i++){
                string childID = (string)dChildRows[i]["ID"];
                MenuNode childNode=GetNode(childID,dt);
                menuNode.Children.Add(childNode);
            }
            return menuNode;

        }

        public List<MenuNode> GetTree(string systemType)
        {
            string condition = !string.IsNullOrEmpty(systemType) ? string.Format(" SystemTypeID='{0}'", systemType) : "";
            List<MenuNode> arrReturn=new List<MenuNode>();
            string sql=string.Format("Select * From {0} Where {1} Order By PID ",tableName,condition);

            DataTable dt=base.SqlTable(sql);
            string sort=string.Format("{0} {1}",GetSafeFileName(sortField),IsDescending?"DESC":"ASC");

            DataRow[] dataRows= dt.Select(string.Format(" PID='{0}' ",-1),sort);
            for(int i=0;i<dataRows.Length;i++){
                string id = (string)dataRows[i]["ID"];
                MenuNode menuNode=GetNode(id,dt);
                arrReturn.Add(menuNode);
            }
            return arrReturn;
        }
    }
}
