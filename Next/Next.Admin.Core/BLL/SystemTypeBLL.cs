using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Next.Framework.Core;
using Next.Admin.IDAL;
using Next.Admin.Entity;


namespace Next.Admin.BLL
{
    public class SystemTypeBLL :BaseBLL<SystemType>
    {
        private ISystemTypeDAL systemTypeDAL;
        public SystemTypeBLL()
            : base()
        {
            base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//如果需要记录操作日志，则实现这个事件
            this.systemTypeDAL = (ISystemTypeDAL)base.baseDal;
        }
    }
}
