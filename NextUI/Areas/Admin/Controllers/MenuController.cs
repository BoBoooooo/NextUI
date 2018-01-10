using Next.Admin.BLL;
using Next.Admin.Entity;
using Next.Admin.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Next.Framework.Core;
using System.Text.RegularExpressions;
using Next.Controllers;

namespace Next.Areas.Admin.Controllers
{
    public class MenuController : BusinessController <MenuBLL,Menu>
    {
        public MenuController()
            : base()
        {

        }
        //
        // GET: /Menu/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetMenuData()
        {
            String title = Request["title"];
            bool defaultFlag = false;
            Dictionary<string, List<MenuData>> dict = new Dictionary<string, List<MenuData>>();
            List<Menu> list = BLLFactory<MenuBLL>.Instance.GetTopMenu();
            int i = 0;
            foreach (Menu info in list)
            {
                if (!HasFunction(info.FunctionID))
                {
                    continue;
                }
                List<MenuData> treeList=new List<MenuData>();
                List<MenuNode> nodeList=BLLFactory<MenuBLL>.Instance.GetTreeByID(info.ID);
                foreach(MenuNode node in nodeList){
                    if(!HasFunction(node.FunctionID))
                    {
                        continue;                    
                    }
                    MenuData menuData=new MenuData(node.ID,node.Name,string.IsNullOrEmpty(node.WebIcon)?"icon-computer":node.WebIcon);
                    foreach (MenuNode subNode in node.Children)
                    {
                        if (!HasFunction(subNode.FunctionID))
                        {
                            continue;
                        }
                        string icon = "icon-computer";// string.IsNullOrEmpty(subNode.WebIcon) ? "icon-computer" : subNode.WebIcon;
                        if (subNode.Url.IndexOf('?') > 0)
                        {
                            subNode.Url += "&appid=" + subNode.ID + "&tabid=tab_" + subNode.ID;
                        }
                        else
                        {
                            subNode.Url += "?appid=" + subNode.ID + "&tabid=tab_" + subNode.ID;
                        }
                        MenuData secondMenuData = new MenuData(subNode.ID, subNode.Name, icon, subNode.Url);
                        menuData.menus.Add(secondMenuData);
                        if (subNode.Children != null)
                        {
                            foreach (MenuNode triNode in subNode.Children)
                            {
                                if (!HasFunction(triNode.FunctionID))
                                {
                                    continue;
                                }
                                icon = "icon-computer";// string.IsNullOrEmpty(subNode.WebIcon) ? "icon-computer" : subNode.WebIcon;
                                secondMenuData.menus.Add(new MenuData(triNode.ID, triNode.Name, icon, triNode.Url));
                            }
                        }

                    }
                    treeList.Add(menuData);
                }
                
                string dictName;
                //string dictName = (i++ == 0) ? "default" : info.ID.ToString();
                //if (!defaultFlag)//info.Name == title)
                if (info.Name == title)
                {
                    dictName = "default";
                    defaultFlag = true;
                }
                else
                {
                    dictName = info.Name;
                }
                //string dictName = info.Name;
                dict.Add(dictName, treeList);
            }
            string content=ToJson(dict);
            content=RemoveJsonNulls(content);
            return Content(content.Trim(','));

        }

        public ActionResult GetMenuTreeJson()
        {
            List<EasyTreeData> treeList = new List<EasyTreeData>();
            List<SystemType> typeList = BLLFactory<SystemTypeBLL>.Instance.GetAll();
            foreach (SystemType typeInfo in typeList)
            {
                EasyTreeData pNode = new EasyTreeData(typeInfo.ID, typeInfo.Name, "icon-computer");
                treeList.Add(pNode);
                string systemType = typeInfo.ID;
                List<MenuNode> menuList = BLLFactory<MenuBLL>.Instance.GetTree(systemType);
                string abc = ToJson(menuList);
                foreach (MenuNode info in menuList)
                {
                    EasyTreeData item = new EasyTreeData(info.ID, info.Name, "icon-view");
                    pNode.children.Add(item);
                    AddChildNode(info.Children, item);
                }
            }

            string content = ToJson(treeList);
            return Content(content.Trim(','));
        }
        private void AddChildNode(List<MenuNode> list, EasyTreeData fnode)
        {
            foreach (MenuNode info in list)
            {
                EasyTreeData item = new EasyTreeData(info.ID, info.Name, "icon-view");
                fnode.children.Add(item);
                AddChildNode(info.Children,item);
            }
        }
        private string RemoveJsonNulls(string str)
        {
            string JsonNullRegEx = "[\"][a-zA-Z0-0_]*[\"]:null[ ]*[,]?";
            string JsonNullArrayRegEx = "\\[( *null *,? *)*]";
            if (!string.IsNullOrEmpty(str))
            {
                Regex regex = new Regex(JsonNullRegEx);
                string data = regex.Replace(str, string.Empty);
                regex = new Regex(JsonNullArrayRegEx);
                return regex.Replace(data, "[]");
            }
            return null;
        }
	}
}