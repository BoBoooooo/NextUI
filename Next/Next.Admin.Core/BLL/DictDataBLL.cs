using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Next.Framework.Core;
using Next.Admin.Entity;
using Next.Admin.DALMySql;
using Next.Admin.IDAL;
using System.Linq;

namespace Next.Admin.BLL
{
	public class DictDataBLL : BaseBLL<DictData>
	{
		private IDictDataDAL dictDataDAL;
		public DictDataBLL(): base()
		{
			base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//如果需要记录操作日志，则实现这个事件
			this.dictDataDAL = (IDictDataDAL)base.baseDal;
		}


	}
}
