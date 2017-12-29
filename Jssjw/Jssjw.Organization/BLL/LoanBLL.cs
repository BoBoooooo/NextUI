using System;
using System.Collections.Generic;
using System.Text;
using Next.Framework.Core;
using Jssjw.Organization.Entity;
using Jssjw.Organization.DALMySql;
using Jssjw.Organization.IDAL;


namespace Jssjw.Organization.BLL
{
	public class LoanBLL : BaseBLL<Loan>
	{
		private ILoanDAL loanDAL;
		public LoanBLL(): base()
		{
			base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//�����Ҫ��¼������־����ʵ������¼�
			this.loanDAL = (ILoanDAL)base.baseDal;
		}
	}
}
