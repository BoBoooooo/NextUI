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
            baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//�����Ҫ��¼������־����ʵ������¼�
			this.journalDAL = (IJournalDAL)base.baseDal;
		}
	}
}
