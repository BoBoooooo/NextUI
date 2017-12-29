using Next.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Next.Admin.Entity;

namespace Next.Admin.IDAL
{
    public interface ILoginLogDAL : IBaseDAL<LoginLog>
    {
        /// <summary>
        /// 获取上一次（非刚刚登录）的登录日志
        /// </summary>
        /// <param name="userId">登录用户ID</param>
        /// <returns></returns>
        LoginLog GetLastLoginInfo(string userId);
    }
}
