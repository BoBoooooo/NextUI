using System;
using System.Collections.Generic;
using System.Text;
using Next.Framework.Core;
using Jssjw.Office.Entity;
using Jssjw.Office.DALMySql;
using Jssjw.Office.IDAL;


namespace Jssjw.Office.BLL
{
	public class JournalBLL : BaseBLL<Journal>
	{
		private IJournalDAL journalDAL;
		public JournalBLL(): base()
		{
			base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//如果需要记录操作日志，则实现这个事件
			this.journalDAL = (IJournalDAL)base.baseDal;
		}
	}
}
