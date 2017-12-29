using System;
using System.Collections.Generic;
using System.Text;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.DALMySql;
using Next.WorkFlow.IDAL;


namespace Next.WorkFlow.BLL
{
	public class WorkFlowCommentBLL : BaseBLL<WorkFlowComment>
	{
		private IWorkFlowCommentDAL workFlowCommentDAL;
		public WorkFlowCommentBLL(): base()
		{
			base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
			baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//如果需要记录操作日志，则实现这个事件
			this.workFlowCommentDAL = (IWorkFlowCommentDAL)base.baseDal;
		}
	}
}
