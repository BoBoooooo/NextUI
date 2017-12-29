using Next.Admin.BLL;
using Next.Admin.Entity;
using Next.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Next.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Home/
        List<int> colorArray = new List<int>();
        public ActionResult Index()
        {
            /*User info = BLLFactory<UserBLL>.Instance.GetUserByName("admin");
            CurrentUser = info;
            Session["UserID"] = info.ID;
            Session["CompanyID"] = info.CompanyID;
            Session["UserInfo"] = info;*/

            if (CurrentUser != null)
            {
                ViewBag.FullName = CurrentUser.FullName;
                ViewBag.Name = CurrentUser.Name;
                StringBuilder sb = new StringBuilder();
                List<Menu> menuList = BLLFactory<MenuBLL>.Instance.GetTopMenu();
                //ViewBag.FirstMenu = menuList[0].Name;
                int i = 0;
                colorArray.Clear();
                foreach (Menu menu in menuList)
                {
                    sb.Append(GetMenuItemString(menu, i));
                    i++;
                }
                ViewBag.HeaderScript = sb.ToString();
                
            }
            return View();
        }
        bool firstMemuFlag = false;
        private string GetMenuItemString(Menu info, int i)
        {
            
            string result = "";
            
            if (HasFunction(info.FunctionID))
            {
                if(!firstMemuFlag){
                    firstMemuFlag=true;
                    ViewBag.FirstMenu =info.Name;
                }
                
                string url = info.Url;
                if (url != null)
                {

                }
                string menuId = (i == 0) ? "default" : info.ID.ToString();

                if (!string.IsNullOrEmpty(url))
                {
                    result = string.Format("<li class=\"" + randomColor() + " dropdown-modal\"><a href=\"#\" onclick=\"showSubMenu('{0}','{1}','{2}')\">{3}</a></li>", info.Url, info.Name, menuId, info.Name);
                }
                else
                {
                    result = string.Format("<li class=\"" + randomColor() + "  dropdown-modal\"><a href=\"#\" onclick=\"showSubMenu('{2}','系统管理','{0}')\">{1}</a></li>", menuId, info.Name, info.Url);
                }
            }
            return result;
        }
        private Dictionary<int, string> colorDict = new Dictionary<int, string> { { 0, "dark" }, { 1, "red" }, { 2, "blue" }, { 3, "green" }, { 4, "orange" }, { 5, "grey" }, { 6, "purple" }, { 7, "pink" }, { 8, "brown" } };

        private String randomColor()
        {
            Random random = new Random();
            int i = random.Next(9);
            while (colorArray.Contains(i))
            {
                random = new Random();
                i = random.Next(9);
            }
            Thread.Sleep(100);
            string result = colorDict[i];
            colorArray.Add(i);

            return colorDict[i];

        }

    }
}