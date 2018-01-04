using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Next.WorkFlow.Utility;

namespace Next.WorkFlow.Utility
{
    public class Tools
    {
        /// <summary>
        /// 包含文件
        /// </summary>
        public static string IncludeFiles
        {
            get
            {
                return
                    string.Format(@"<link href=""{0}Style/Theme/Common.css"" rel=""stylesheet"" type=""text/css"" media=""screen""/>
    <link href=""{0}Style/Theme/{1}/Style/style.css"" id=""style_style"" rel=""stylesheet"" type=""text/css"" media=""screen""/>
    <link href=""{0}Style/Theme/{1}/Style/ui.css"" id=""style_ui"" rel=""stylesheet"" type=""text/css"" media=""screen""/> 
    <script type=""text/javascript"" src=""{0}Scripts/My97DatePicker/WdatePicker.js""></script>
    <script type=""text/javascript"" src=""{0}Scripts/jquery-1.11.1.min.js""></script>
    <script type=""text/javascript"" src=""{0}Scripts/jquery.cookie.js""></script>
    <script type=""text/javascript"" src=""{0}Scripts/json.js""></script>
    <script type=""text/javascript"" src=""{0}Scripts/roadui.core.js""></script>
    <script type=""text/javascript"" src=""{0}Scripts/roadui.button.js""></script>
    <script type=""text/javascript"" src=""{0}Scripts/roadui.calendar.js""></script>
    <script type=""text/javascript"" src=""{0}Scripts/roadui.file.js""></script>
    <script type=""text/javascript"" src=""{0}Scripts/roadui.member.js""></script>
    <script type=""text/javascript"" src=""{0}Scripts/roadui.dict.js""></script>
    <script type=""text/javascript"" src=""{0}Scripts/roadui.menu.js""></script>
    <script type=""text/javascript"" src=""{0}Scripts/roadui.select.js""></script>
    <script type=""text/javascript"" src=""{0}Scripts/roadui.combox.js""></script>
    <script type=""text/javascript"" src=""{0}Scripts/roadui.tab.js""></script>
    <script type=""text/javascript"" src=""{0}Scripts/roadui.text.js""></script>
    <script type=""text/javascript"" src=""{0}Scripts/roadui.textarea.js""></script>
    <script type=""text/javascript"" src=""{0}Scripts/roadui.editor.js""></script>
    <script type=""text/javascript"" src=""{0}Scripts/roadui.tree.js""></script>
    <script type=""text/javascript"" src=""{0}Scripts/roadui.validate.js""></script>
    <script type=""text/javascript"" src=""{0}Scripts/roadui.window.js""></script>
    <script type=""text/javascript"" src=""{0}Scripts/roadui.dragsort.js""></script>
    <script type=""text/javascript"" src=""{0}Scripts/roadui.selectico.js""></script>
    <script type=""text/javascript"" src=""{0}Scripts/roadui.accordion.js""></script>
    <script type=""text/javascript"" src=""{0}Scripts/roadui.grid.js""></script>
    <script type=""text/javascript"" src=""{0}Scripts/roadui.init.js""></script>"
    , "~/Assets/WorkFlow/", "blue");
            }
        }
        public static bool CheckLogin(out string msg)
        {
            msg = "";
            /*object session = System.Web.HttpContext.Current.Session["UserID"];
            string uid;
            if (session == null || !session.ToString().IsGuid(out uid) || uid == string.Empty)
            {
                return false;
            }

            //#if DEBUG
            return true; //正式使用时请注释掉这一行
            //#endif

            string uniqueIDSessionKey = RoadFlow.Utility.Keys.SessionKeys.UserUniqueID.ToString();
            var user = new RoadFlow.Platform.OnlineUsers().Get(uid);
            if (user == null)
            {
                return false;
            }
            else if (System.Web.HttpContext.Current.Session[uniqueIDSessionKey] == null)
            {
                return false;
            }
            else if (string.Compare(System.Web.HttpContext.Current.Session[uniqueIDSessionKey].ToString(), user.UniqueID.ToString(), true) != 0)
            {
                msg = string.Format("您的帐号在{0}登录,您被迫下线!", user.IP);
                return false;
            }*/
            return true;
        }

        public static bool CheckLogin(bool redirect = true)
        {
            string msg;
            if (!CheckLogin(out msg))
            {
                if (!redirect)
                {
                    System.Web.HttpContext.Current.Response.Write("登录验证失败!");
                    System.Web.HttpContext.Current.Response.End();
                    return false;
                }
                else
                {
                    System.Web.HttpContext.Current.Response.Write("<script>top.login();</script>");
                    System.Web.HttpContext.Current.Response.End();
                    return false;
                }
            }
            return true;
        }
    }
}
