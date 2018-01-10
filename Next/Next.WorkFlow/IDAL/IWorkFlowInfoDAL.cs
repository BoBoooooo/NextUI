using Next.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Next.WorkFlow.Entity;

namespace Next.WorkFlow.IDAL
{
    public interface IWorkFlowInfoDAL : IBaseDAL<WorkFlowInfo>
    {


                /// <summary>
        /// 查询所有类型
        /// </summary>
        List<string> GetAllTypes();

        /// <summary>
        /// 查询所有ID和名称
        /// </summary>
        Dictionary<string, string> GetAllIDAndName();

        /// <summary>
        /// 查询所有记录
        /// </summary>
        List<WorkFlowInfo> GetByTypes(string typeString);
    }
}

