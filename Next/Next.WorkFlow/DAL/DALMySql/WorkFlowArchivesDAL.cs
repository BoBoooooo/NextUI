using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.IDAL;

namespace Next.WorkFlow.DALMySql
{
	public class WorkFlowArchivesDAL: BaseDALMySql<WorkFlowArchives> , IWorkFlowArchivesDAL
	{
		public static WorkFlowArchivesDAL Instance
		{
			get
			{
				return new WorkFlowArchivesDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public WorkFlowArchivesDAL()
		: base("WorkFlowArchives", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}