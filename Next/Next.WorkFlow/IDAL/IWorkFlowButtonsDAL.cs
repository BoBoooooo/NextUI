using Next.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Next.WorkFlow.Entity;

namespace Next.WorkFlow.IDAL
{
    public interface IWorkFlowButtonsDAL : IBaseDAL<WorkFlowButtons>
    {
        int GetMaxSort();
    }
}
