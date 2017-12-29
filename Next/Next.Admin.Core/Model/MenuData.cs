using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Admin.Model
{
    public class MenuData
    {
        public string menuid { get; set; }
        public string menuname { get; set; }
        public string icon { get; set; }

        public string url { get; set; }

        public List<MenuData> menus { get; set; }

        public MenuData()
        {
            this.menus = new List<MenuData>();
            this.icon = "icon-view";
        }

        public MenuData(string menuid, string menuname, string icon = "icon-view", string url = null)
            : this()
        {
            this.menuid = menuid;
            this.menuname = menuname;
            this.icon = icon;
            this.url = url;
            
        }

    }
}
