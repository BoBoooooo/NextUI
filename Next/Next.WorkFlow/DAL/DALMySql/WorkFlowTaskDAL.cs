using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.IDAL;

namespace Next.WorkFlow.DALMySql
{
	public class WorkFlowTaskDAL: BaseDALMySql<WorkFlowTask> , IWorkFlowTaskDAL
	{
		public static WorkFlowTaskDAL Instance
		{
			get
			{
				return new WorkFlowTaskDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public WorkFlowTaskDAL()
		: base("WorkFlowTask", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}