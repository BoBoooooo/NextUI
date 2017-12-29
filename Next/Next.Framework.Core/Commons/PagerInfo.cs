using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Framework.Core.Commons
{
    public delegate void PageInfoChanged(PagerInfo info);
    public class PagerInfo
    {
        public event PageInfoChanged OnPageInfoChanged;
        private int currentPageIndex;
        private int pageSize;
        private int recordCount;
        public int CurrentPageIndex
        {

            get { return currentPageIndex; }
            set { currentPageIndex = value;
            if (OnPageInfoChanged != null)
            {
                OnPageInfoChanged(this);
            }
            }
        }
        public int PageSize
        {
            get { return pageSize; }
            set
            {
                pageSize = value;
                if (OnPageInfoChanged != null)
                {
                    OnPageInfoChanged(this);
                }
            }
        }

        public int RecordCount
        {
            get { return recordCount; }
            set
            {
                recordCount = value;
                if (OnPageInfoChanged != null)
                {
                    OnPageInfoChanged(this);
                }
            }
        }
    }
}
