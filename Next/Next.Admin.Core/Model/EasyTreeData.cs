using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Admin.Model
{
    public class EasyTreeData
    {
        public string id { get; set; }
        public string text { get; set; }
        public string state { get; set; }

        public string iconCls { get; set; }

        public bool Checked { get; set; }

        public List<EasyTreeData> children { get; set; }

        public EasyTreeData()
        {
            this.children = new List<EasyTreeData>();
            this.state = "open";
        }
        public EasyTreeData(string id, string text, string iconCls = "", string state = "open")
        {
            this.children = new List<EasyTreeData>();
            this.id = id;
            this.text = text;
            this.state = state;
            this.iconCls = iconCls;
        }
        public EasyTreeData(int id, string text, string iconCls = "", string state = "open"):this()
        {
            this.children = new List<EasyTreeData>();
            this.id = id.ToString();
            this.text = text;
            this.state = state;
            this.iconCls = iconCls;
        }
    }
}
