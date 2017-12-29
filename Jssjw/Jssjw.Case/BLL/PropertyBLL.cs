using System;
using System.Collections.Generic;
using System.Text;
using Next.Framework.Core;
using Jssjw.Case.Entity;
using Jssjw.Case.DALMySql;
using Jssjw.Case.IDAL;


namespace Jssjw.Case.BLL
{
	public class PropertyBLL : BaseBLL<Property>
	{
		private IPropertyDAL propertyDAL;
		public PropertyBLL(): base()
		{
			base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//如果需要记录操作日志，则实现这个事件
			this.propertyDAL = (IPropertyDAL)base.baseDal;
		}
	}
}
