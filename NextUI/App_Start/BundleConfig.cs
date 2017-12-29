using System.Web;
using System.Web.Optimization;

namespace Next
{
    public class BundleConfig
    {
        // 有关 Bundling 的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new StyleBundle("~/Assets/style").Include("~/Assets/EasyUI/themes/bootstrap/easyui.css", "~/Assets/EasyUI/themes/icon.css", "~/Assets/themes/Default/style.css", "~/Assets/themes/Default/default.css"));

            bundles.Add(new ScriptBundle("~/Assets/script").Include("~/Assets/EasyUI/jquery.min.js", "~/Assets/EasyUI/jquery.easyui.min.js", "~/Assets/EasyUI/locale/easyui-lang-zh_CN.js"));


            //bundles.Add(new StyleBundle("~/Assets/style").Include("~/Assets/Jquery-Easyui/themes/bootstrap/easyui.css", "~/Assets/Jquery-Easyui/themes/icon.css", "~/Assets/themes/Default/style.css", "~/Assets/themes/Default/default.css"));

            //bundles.Add(new ScriptBundle("~/Assets/script").Include("~/Assets/Jquery-Easyui/jquery.min.js", "~/Assets/Jquery-Easyui/jquery.easyui.min.js", "~/Assets/Jquery-Easyui/locale/easyui-lang-zh_CN.js"));


            bundles.Add(new StyleBundle("~/Assets/InsdepStyle").Include("~/Assets/Insdep/themes/insdep/easyui.css", "~/Assets/Insdep/themes/insdep/easyui_animation.css", "~/Assets/Insdep/themes/insdep/easyui_plus.css", "~/Assets/Insdep/themes/insdep/insdep_theme_default.css", "~/Assets/Insdep/themes/insdep/icon.css", "~/Assets/EasyUI/themes/icon.css", "~/Assets/Next/Style/star0.css"));

            bundles.Add(new ScriptBundle("~/Assets/InsdepScript").Include("~/Assets/Insdep/jquery.min.js", "~/Assets/Insdep/jquery.easyui-1.5.1.min.js", "~/Assets/Insdep/themes/insdep/insdep-extend.min.js", "~/Assets/Insdep/locale/easyui-lang-zh_CN.js"));

            BundleTable.Bundles.Add(new ScriptBundle("~/Assets/RoadUIScript")
            .Include("~/assets/workflow/Scripts/My97DatePicker/WdatePicker.js")
            .Include("~/Assets/WorkFlow/Scripts/jquery-1.11.1.js")
            .Include("~/Assets/WorkFlow/Scripts/jquery.cookie.js")
            .Include("~/Assets/WorkFlow/Scripts/json.js")
            .Include("~/Assets/WorkFlow/Scripts/roadui.core.js")
            .Include("~/Assets/WorkFlow/Scripts/roadui.button.js")
            .Include("~/Assets/WorkFlow/Scripts/roadui.calendar.js")
            .Include("~/Assets/WorkFlow/Scripts/roadui.file.js")
            .Include("~/Assets/WorkFlow/Scripts/roadui.member.js")
            .Include("~/Assets/WorkFlow/Scripts/roadui.dict.js")
            .Include("~/Assets/WorkFlow/Scripts/roadui.menu.js")
            .Include("~/Assets/WorkFlow/Scripts/roadui.select.js")
            .Include("~/Assets/WorkFlow/Scripts/roadui.combox.js")
            .Include("~/Assets/WorkFlow/Scripts/roadui.tab.js")
            .Include("~/Assets/WorkFlow/Scripts/roadui.text.js")
            .Include("~/Assets/WorkFlow/Scripts/roadui.textarea.js")
            .Include("~/Assets/WorkFlow/Scripts/roadui.editor.js")
            .Include("~/Assets/WorkFlow/Scripts/roadui.tree.js")
            .Include("~/Assets/WorkFlow/Scripts/roadui.validate.js")
            .Include("~/Assets/WorkFlow/Scripts/roadui.window.js")
            .Include("~/Assets/WorkFlow/Scripts/roadui.dragsort.js")
            .Include("~/Assets/WorkFlow/Scripts/roadui.selectico.js")
            .Include("~/Assets/WorkFlow/Scripts/roadui.accordion.js")
            .Include("~/Assets/WorkFlow/Scripts/roadui.grid.js")
            .Include("~/Assets/WorkFlow/Scripts/roadui.init.js")
    );

            bundles.Add(new StyleBundle("~/Assets/RoadUIStyle").Include("~/assets/workflow/Style/Theme/Common.css", "~/assets/workflow/Style/Theme/blue/Style/style.css", "~/assets/workflow/Style/Theme/blue/Style/ui.css"));

        }
    }
}