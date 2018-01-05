using Next.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Next.Admin.Entity;

namespace Next.Admin.IDAL
{
    public interface IDictTypeDAL : IBaseDAL<DictType>
    {
        DictType GetRoot();

        /// <summary>
        /// 查询下级记录
        /// </summary>
        List<DictType> GetChildsByID(string id);

        /// <summary>
        /// 查询下级记录
        /// </summary>
        List<DictType> GetChildsByCode(string code);

        /// <summary>
        /// 查询上级记录
        /// </summary>
        DictType GetParent(string id);

        /// <summary>
        /// 是否包含下级记录
        /// </summary>
        bool HasChilds(string id);

        /// <summary>
        /// 得到最大排序
        /// </summary>
        int GetMaxSort(string id);

        /// <summary>
        /// 更新排序
        /// </summary>
        int UpdateSort(string id, int sort);

        /// <summary>
        /// 根据代码查询一条记录
        /// </summary>
        DictType GetByCode(string code);
    }
}
