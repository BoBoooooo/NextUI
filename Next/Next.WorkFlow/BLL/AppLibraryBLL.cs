using System;
using System.Collections.Generic;
using System.Text;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.DALMySql;
using Next.WorkFlow.IDAL;
using Next.Admin.BLL;
using Next.Admin.Entity;
using Next.WorkFlow.Utility;
using System.Linq;

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
        /// 得到一页数据
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="query"></param>
        /// <param name="order"></param>
        /// <param name="size"></param>
        /// <param name="numbe"></param>
        /// <param name="title"></param>
        /// <param name="type"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public List<AppLibrary> GetPagerData(out string pager, string query = "", string title = "", string type = "", string address = "")
        {
            return appLibraryDAL.GetPagerData(out pager, query, "Type,Title", Next.WorkFlow.Utility.Tools.GetPageSize(),
                Next.WorkFlow.Utility.Tools.GetPageNumber(), title, type, address);
        }
        /// <summary>
        /// 根据代码查询一条记录
        /// </summary>
        public AppLibrary GetByCode(string code)
        {

            return string.IsNullOrEmpty(code) ? null : appLibraryDAL.GetByCode(code.Trim());
        }


        /// <summary>
        /// 得到类型选择项
        /// </summary>
        /// <returns></returns>
        public string GetTypeOptions(string value = "")
        {
            return new DictBLL().GetOptionsByCode("AppLibraryTypes", DictBLL.OptionValueField.ID, value);
        }

        /// <summary>
        /// 得到一个类型选择项
        /// </summary>
        /// <param name="type">程序类型</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetAppsOptions(string type, string value = "")
        {
            if (type.IsEmptyGuid()) return "";
            var apps = GetAllByType(type);
            StringBuilder options = new StringBuilder();
            foreach (var app in apps)
            {
                options.AppendFormat("<option value=\"{0}\" {1}>{2}</option>", app.ID,
                    string.Compare(app.ID.ToString(), value, true) == 0 ? "selected=\"selected\"" : "",
                    app.Title
                    );
            }
            return options.ToString();
        }
        /// <summary>
        /// 查询一个类别下所有记录
        /// </summary>
        public List<AppLibrary> GetAllByType(string type)
        {
            if (type.IsEmptyGuid())
            {
                return new List<AppLibrary>();
            }
            return appLibraryDAL.GetAllByType(GetAllChildsIDString(type)).OrderBy(p => p.Title).ToList();
        }

        /// <summary>
        /// 得到下级ID字符串
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetAllChildsIDString(string id, bool isSelf = true)
        {
            return new DictBLL().GetAllChildsIDString(id, true);
        }
	}
}
