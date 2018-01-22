﻿using System;
using System.Collections.Generic;
using System.Text;
using Next.Framework.Core;
using App.Clue.Entity;
using App.Clue.DALMySql;
using App.Clue.IDAL;


namespace App.Clue.BLL
{
	public class NodeBLL : BaseBLL<Node>
	{
		private INodeDAL nodeDAL;
		public NodeBLL(): base()
		{
			base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
			baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//如果需要记录操作日志，则实现这个事件
			this.nodeDAL = (INodeDAL)base.baseDal;
		}
	}
}
