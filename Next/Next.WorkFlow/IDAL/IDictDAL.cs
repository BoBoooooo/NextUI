using Next.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Next.WorkFlow.Entity;

namespace Next.WorkFlow.IDAL
{
    public interface IDictDAL : IBaseDAL<Dict>
    {
        Dict GetRoot();

        /// <summary>
        /// 查询下级记录
        /// </summary>
        List<Dict> GetChildsByID(string id);

        /// <summary>
        /// 查询下级记录
        /// </summary>
        List<Dict> GetChildsByCode(string code);

        /// <summary>
        /// 查询上级记录
        /// </summary>
        Dict GetParent(string id);

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
        Dict GetByCode(string code);
    }
}
