using System;
using System.Collections.Generic;
using System.Text;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.DALMySql;
using Next.WorkFlow.IDAL;
using Next.Admin.BLL;


namespace Next.WorkFlow.BLL
{
	public class AppLibraryBLL : BaseBLL<AppLibrary>
	{
		private IAppLibraryDAL appLibraryDAL;
		public AppLibraryBLL(): base()
		{
			base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
			baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//如果需要记录操作日志，则实现这个事件
			this.appLibraryDAL = (IAppLibraryDAL)base.baseDal;
		}

        /// <summary>
        /// 根据代码查询一条记录
        /// </summary>
        public AppLibrary GetByCode(string code)
        {

            return null;// string.IsNullOrEmpty(code) ? null : appLibraryDAL.GetByCode(code.Trim());
        }


        /// <summary>
        /// 得到类型选择项
        /// </summary>
        /// <returns></returns>
        public string GetTypeOptions(string value = "")
        {
            return new DictTypeBLL().GetOptionsByName("应用程序库分类", value);
        }
	}
}
