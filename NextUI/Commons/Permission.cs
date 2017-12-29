using Next.Admin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Next.Commons
{
    public class Permission
    {
        public static bool HasFunction(string functionID)
        {
            bool hasFunction = false;
            User currentUser = HttpContext.Current.Session["UserInfo"] as User;
            if (currentUser != null && currentUser.Name == "admin")
            {
                hasFunction = true;
            }
            else
            {
                if (functionID==null)
                {
                    hasFunction = true;
                }
                else
                {
                    Dictionary<string, string> functionDict = HttpContext.Current.Session["Functions"] as Dictionary<string, string>;
                    if (functionDict != null && functionDict.ContainsKey(functionID))
                    {
                        hasFunction = true;
                    }
                }
            }
            return hasFunction;
        }
    }
}