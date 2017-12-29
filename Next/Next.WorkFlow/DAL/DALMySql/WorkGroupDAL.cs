using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.IDAL;

namespace Next.WorkFlow.DALMySql
{
	public class WorkGroupDAL: BaseDALMySql<WorkGroup> , IWorkGroupDAL
	{
		public static WorkGroupDAL Instance
		{
			get
			{
				return new WorkGroupDAL();
			}
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public WorkGroupDAL()
		: base("WorkGroup", "ID")
		{
			this.sortField = "ID";
			this.IsDescending = false;
		}
	}
}