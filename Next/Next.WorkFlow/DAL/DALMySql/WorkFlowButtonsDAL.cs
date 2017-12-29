using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.IDAL;

namespace Next.WorkFlow.DALMySql
{
	public class WorkFlowButtonsDAL: BaseDALMySql<WorkFlowButtons> , IWorkFlowButtonsDAL
	{
		public static WorkFlowButtonsDAL Instance
		{
			get
			{
				return new WorkFlowButtonsDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public WorkFlowButtonsDAL()
		: base("WorkFlowButtons", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}