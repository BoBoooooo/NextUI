using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.IDAL;

namespace Next.WorkFlow.DALMySql
{
	public class WorkFlowDelegationDAL: BaseDALMySql<WorkFlowDelegation> , IWorkFlowDelegationDAL
	{
		public static WorkFlowDelegationDAL Instance
		{
			get
			{
				return new WorkFlowDelegationDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public WorkFlowDelegationDAL()
		: base("WorkFlowDelegation", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}