using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.IDAL;

namespace Next.WorkFlow.DALMySql
{
	public class WorkFlowInfoDAL: BaseDALMySql<WorkFlowInfo> , IWorkFlowInfoDAL
	{
		public static WorkFlowInfoDAL Instance
		{
			get
			{
				return new WorkFlowInfoDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public WorkFlowInfoDAL()
		: base("WorkFlowInfo", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}