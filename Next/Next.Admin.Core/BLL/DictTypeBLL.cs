using System;
using System.Collections.Generic;
using System.Text;
using Next.Framework.Core;
using Next.Admin.Entity;
using Next.Admin.DALMySql;
using Next.Admin.IDAL;


namespace Next.Admin.BLL
{
	public class DictTypeBLL : BaseBLL<DictType>
	{
		private IDictTypeDAL dictTypeDAL;
		public DictTypeBLL(): base()
		{
			base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//如果需要记录操作日志，则实现这个事件
			this.dictTypeDAL = (IDictTypeDAL)base.baseDal;
		}
	}
}
