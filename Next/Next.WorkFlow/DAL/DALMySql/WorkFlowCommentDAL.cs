using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.IDAL;

namespace Next.WorkFlow.DALMySql
{
	public class WorkFlowCommentDAL: BaseDALMySql<WorkFlowComment> , IWorkFlowCommentDAL
	{
		public static WorkFlowCommentDAL Instance
		{
			get
			{
				return new WorkFlowCommentDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public WorkFlowCommentDAL()
		: base("WorkFlowComment", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}