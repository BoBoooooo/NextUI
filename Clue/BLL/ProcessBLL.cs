﻿using System;
using System.Collections.Generic;
using System.Text;
using Next.Framework.Core;
using App.Clue.Entity;
using App.Clue.DALMySql;
using App.Clue.IDAL;


namespace App.Clue.BLL
{
	public class ProcessBLL : BaseBLL<Process>
	{
		private IProcessDAL processDAL;
		public ProcessBLL(): base()
		{
			base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
			baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//如果需要记录操作日志，则实现这个事件
			this.processDAL = (IProcessDAL)base.baseDal;
		}
	}
}
