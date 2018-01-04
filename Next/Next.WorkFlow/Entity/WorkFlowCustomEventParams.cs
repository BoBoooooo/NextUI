using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.WorkFlow.Entity
{
    /// <summary>
    /// 调用流程事件时的参数实体
    /// </summary>
    [Serializable]
    public struct WorkFlowCustomEventParams
    {

        public string FlowID { get; set; }

        public string StepID { get; set; }

        public string GroupID { get; set; }

        public string TaskID { get; set; }

        public string InstanceID { get; set; }
    }
}
