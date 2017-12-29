using System;
using System.Collections.Generic;
using System.Text;
using Next.Framework.Core;
using Jssjw.Organization.Entity;
using Jssjw.Organization.DALMySql;
using Jssjw.Organization.IDAL;


namespace Jssjw.Organization.BLL
{
	public class AffixBLL : BaseBLL<Affix>
	{
		private IAffixDAL affixDAL;
		public AffixBLL(): base()
		{
			base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//如果需要记录操作日志，则实现这个事件
			this.affixDAL = (IAffixDAL)base.baseDal;
		}
	}
}
