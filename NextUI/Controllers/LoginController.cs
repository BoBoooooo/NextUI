using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Next.Admin;
using Next.Admin.Entity;
using Next.Framework.Core;
using Next.Admin.DALMySql;
using Next.Admin.BLL;
using Next.Commons;
using System.Management;
namespace Next.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult CheckUser(User user)
        {
            bool flag = BLLFactory<UserBLL>.Instance.VerifyUser(user);
            if (flag)
            {

                User info = BLLFactory<UserBLL>.Instance.GetUserByName(user.Name);

                Session["UserID"] = info.ID;
                Session["CompanyID"] = info.CompanyID;
                Session["UserInfo"] = info;
                Session["IP"] = GetClientIp();
                Session["MAC"] = GetMacAddress();

                List<Function> functionList = BLLFactory<FunctionBLL>.Instance.GetFunctionsByUser(info.ID, MyConstants.SystemType);
                Dictionary<string, string> functionDict = new Dictionary<string, string>();
                foreach (Function functionInfo in functionList)
                {
                    //if (!string.IsNullOrEmpty(functionInfo.ControlID) && !functionDict.ContainsKey(functionInfo.ControlID))
                    if (!string.IsNullOrEmpty(functionInfo.ID) && !functionDict.ContainsKey(functionInfo.ID))
                    {
                        functionDict.Add(functionInfo.ID, functionInfo.ControlID);
                    }
                }
                Session["Functions"] = functionDict;
                return Content("true");
            }
            else
            {
                return Content("false");
            }
        }

        /// <summary>
        /// 获取客户端IP地址
        /// </summary>
        /// <returns></returns>
        private string GetClientIp()
        {
            //可以透过代理服务器
            string userIP = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(userIP))
            {
                //没有代理服务器,如果有代理服务器获取的是代理服务器的IP
                userIP = Request.ServerVariables["REMOTE_ADDR"];
            }
            if (string.IsNullOrEmpty(userIP))
            {
                userIP = Request.UserHostAddress;
            }

            //替换本机默认的::1
            if (userIP == "::1")
            {
                userIP = "127.0.0.1";
            }

            return userIP;
        }
        /// <summary>  
        /// 获取网卡地址信息  
        /// </summary>  
        /// <returns></returns>  
        private string GetMacAddress()
        {
            try
            {
                string mac = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        mac = mo["MacAddress"].ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
                return mac;
            }
            catch
            {
                return "unknow";
            }
        }  
    }
}