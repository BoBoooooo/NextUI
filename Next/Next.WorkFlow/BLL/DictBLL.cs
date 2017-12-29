using System;
using System.Collections.Generic;
using System.Text;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.DALMySql;
using Next.WorkFlow.IDAL;


namespace Next.WorkFlow.BLL
{
	public class DictBLL : BaseBLL<Dict>
	{
		private IDictDAL dictDAL;
		public DictBLL(): base()
		{
			base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
			baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//如果需要记录操作日志，则实现这个事件
			this.dictDAL = (IDictDAL)base.baseDal;
		}
        /// <summary>
        /// 根据代码得到选项
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetOptionsByCode(string code, OptionValueField valueField = OptionValueField.Value, string value = "")
        {
            return GetOptionsByID(GetIDByCode(code), valueField, value);
        }
        /// <summary>
        /// 下拉选项时以哪个字段作为值字段
        /// </summary>
        public enum OptionValueField
        {
            ID,
            Title,
            Code,
            Value,
            Other,
            Note
        }

        /// <summary>
        /// 根据ID得到选项
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetOptionsByID(string id, OptionValueField valueField = OptionValueField.Value, string value = "")
        {
            var childs = GetAllChilds(id, true);
            StringBuilder options = new StringBuilder(childs.Count * 100);
            StringBuilder space = new StringBuilder();
            foreach (var child in childs)
            {
                space.Clear();
                int parentCount = getParentCount(childs, child);

                for (int i = 0; i < parentCount - 1; i++)
                {
                    space.Append("&nbsp;&nbsp;");
                }

                if (parentCount > 0)
                {
                    space.Append("┝");
                }
                string value1 = getOptionsValue(valueField, child);
                options.AppendFormat("<option value=\"{0}\"{1}>{2}{3}</option>" + parentCount, value1, value1 == value ? " selected=\"selected\"" : "", space.ToString(), child.Title);
            }
            return options.ToString();
        }


        private string getOptionsValue(OptionValueField valueField, Dict dict)
        {
            string value = string.Empty;
            switch (valueField)
            {
                case OptionValueField.Code:
                    value = dict.Code;
                    break;
                case OptionValueField.ID:
                    value = dict.ID.ToString();
                    break;
                case OptionValueField.Note:
                    value = dict.Note;
                    break;
                case OptionValueField.Other:
                    value = dict.Other;
                    break;
                case OptionValueField.Title:
                    value = dict.Title;
                    break;
                case OptionValueField.Value:
                    value = dict.Value;
                    break;
            }
            return value;
        }
        /// <summary>
        /// 得到一个字典项的上级节点数
        /// </summary>
        /// <param name="dictList"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        private int getParentCount(List<Dict> dictList, Dict dict)
        {
            int parent = 0;
            Dict parentDict = dictList.Find(p => p.ID == dict.ParentID);
            while (parentDict != null)
            {
                parentDict = dictList.Find(p => p.ID == parentDict.ParentID);
                parent++;
            }
            return parent;
        }
        /// <summary>
        /// 根据代码得到ID
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetIDByCode(string code)
        {
            var dict = GetByCode(code, true);
            if (dict == null)
            {
                return "";
            }
            return dict.ID== null ? "" : dict.ID;
        }

        /// <summary>
        /// 根据代码查询一条记录
        /// </summary>
        /// <param name="code"></param>
        /// <param name="fromCache">是否使用缓存</param>
        /// <returns></returns>
        public Dict GetByCode(string code, bool fromCache = false)
        {
            return dictDAL.FindByID(code.Trim());
            //return code.IsNullOrEmpty() ? null :
                //fromCache ? GetAll(true).Find(p => string.Compare(p.Code, code, true) == 0) : dataDictionary.GetByCode(code.Trim());
        }

        /// <summary>
        /// 得到所有下级
        /// </summary>
        /// <param name="code"></param>
        /// <param name="fromCache">是否使用缓存</param>
        /// <returns></returns>
        public List<Dict> GetAllChilds(string code, bool fromCache)
        {
            if (string.IsNullOrEmpty(code)) return new List<Dict>();
            var dict = GetByCode(code, fromCache);
            if (dict == null) return new List<Dict>();
            return GetAllChilds(dict.ID, fromCache);
        }
	}
}
