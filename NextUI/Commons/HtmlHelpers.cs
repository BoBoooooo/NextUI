using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace Next.Commons
{
    public static class HtmlHelpers
    {
        public static bool HasFunction(this HtmlHelper helper, string functionID)
        {
            return Permission.HasFunction(functionID);
        }

        public static HtmlString HtmlOutput(this HtmlHelper helper, string flag, object value, MvcHtmlString target)
        {

                if (flag == "View")
                {
                    if (value == null)
                    {
                        value = "";
                    }
                    return new HtmlString(value.ToString());
                }
                if (flag == "Add")
                {
                    return new HtmlString(target.ToHtmlString());
                }
                if (flag == "Edit")
                {
                    return new HtmlString(target.ToHtmlString());
                }
            throw new Exception();
            
        }
        public static HtmlString HtmlOutput(this HtmlHelper helper, string flag, string value, MvcHtmlString defaultValue, MvcHtmlString target)
        {
            if (flag == "View")
            {
                return new HtmlString(value);
            }
            if (flag == "Add")
            {
                return new HtmlString(defaultValue.ToHtmlString());
            }
            if (flag == "Edit")
            {
                return new HtmlString(target.ToHtmlString());
            }
            throw new Exception();
            return null;
        }
    }
}