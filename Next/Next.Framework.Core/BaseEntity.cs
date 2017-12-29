using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Framework.Core
{
    public class BaseEntity
    {

        //public string CurrentLoginUserID { get; set; }
        private string m_CurrentLoginUserId;

        /// <summary>
        /// 当前登录用户ID。该字段不保存到数据表中，只用于记录用户的操作日志。
        /// </summary>
        public string CurrentLoginUserId
        {
            get { return m_CurrentLoginUserId; }
            set { m_CurrentLoginUserId = value; }
        }
    }
}
