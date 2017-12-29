using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Framework.Core.Commons
{
    public class CListItem
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public CListItem(string text, string value)
        {
            this.Text = text;
            this.Value = value;
        }
        public CListItem(string text)
        {
            this.Text = text;
            this.Value = text;
        }
        public override string ToString()
        {
            return Text.ToString();
        }
    }
}
