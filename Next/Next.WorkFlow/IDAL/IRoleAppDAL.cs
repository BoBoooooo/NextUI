using Next.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Next.WorkFlow.Entity;

namespace Next.WorkFlow.IDAL
{
    public interface IRoleAppDAL : IBaseDAL<RoleApp>
    {
        List<RoleApp> GetAllByRoleID(string roleID);

        /// <summary>
        /// 查询个角色所有记录
        /// </summary>
        System.Data.DataTable GetAllDataTableByRoleID(string roleID);

        /// <summary>
        /// 查询所有记录
        /// </summary>
        System.Data.DataTable GetAllDataTable();

        /// <summary>
        /// 查询所有下级记录
        /// </summary>
        System.Data.DataTable GetChildsDataTable(string id);

        /// <summary>
        /// 查询下级记录
        /// </summary>
        List<RoleApp> GetChild(string id);

        /// <summary>
        /// 是否有下级记录
        /// </summary>
        bool HasChild(string id);

        /// <summary>
        /// 更新排序
        /// </summary>
        int UpdateSort(string id, int sort);

        /// <summary>
        /// 删除一个角色记录
        /// </summary>
        int DeleteByRoleID(string roleID);

        /// <summary>
        /// 得到最大排序值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int GetMaxSort(string id);

        /// <summary>
        /// 删除一个应用记录
        /// </summary>
        int DeleteByAppID(string appID);
    }
}
