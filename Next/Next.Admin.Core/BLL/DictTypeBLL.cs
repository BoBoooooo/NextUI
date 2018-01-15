using System;
using System.Collections.Generic;
using System.Text;
using Next.Framework.Core;
using Next.Admin.Entity;
using Next.Admin.DALMySql;
using Next.Admin.IDAL;
using System.Linq;
using Next.WorkFlow.Utility;


namespace Next.Admin.BLL
{
    public class DictTypeBLL : BaseBLL<DictType>
    {
        private IDictTypeDAL dictTypeDAL;
        public DictTypeBLL()
            : base()
        {
            base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//如果需要记录操作日志，则实现这个事件
            this.dictTypeDAL = (IDictTypeDAL)base.baseDal;
        }

        /*
        /// <summary>
        /// 查询根记录
        /// </summary>
        public DictType GetRoot()
        {
            return dictTypeDAL.GetRoot();
        }

        /// <summary>
        /// 查询下级记录
        /// </summary>
        public List<DictType> GetChildsByID(string id, bool fromCache = false)
        {
            return fromCache ? getChildsByIDFromCache(id) : dictTypeDAL.GetChildsByID(id);
        }

        /// <summary>
        /// 查询下级记录p.Seq
        /// </summary>
        public List<DictType> GetChildsByCode(string code, bool fromCache = false)
        {
            return code.IsNullOrEmpty() ? new List<DictType>() :
                fromCache ? getChildsByCodeFromCache(code) :
                dictTypeDAL.GetChildsByCode(code.Trim());
        }

        private List<DictType> getChildsByCodeFromCache(string code)
        {
            var list = GetAll();
            var dict = list.Find(p => string.Compare(p.Code, code, true) == 0);
            return dict == null ? new List<DictType>() : list.FindAll(p => p.PID == dict.ID).OrderBy(p => p.Seq).ToList();
        }

        private List<DictType> getChildsByIDFromCache(string id)
        {
            var list = GetAll();
            return list.FindAll(p => p.PID == id).OrderBy(p => p.Seq).ToList();
        }

        /// <summary>
        /// 得到所有下级
        /// </summary>
        /// <param name="code"></param>
        /// <param name="fromCache">是否使用缓存</param>
        /// <returns></returns>
        /*public List<DictType> GetAllChilds(string code, bool fromCache)
        {
            if (code.IsNullOrEmpty()) return new List<DictType>();
            var dict = GetByCode(code, fromCache);
            if (dict == null) return new List<DictType>();
            return GetAllChilds(dict.ID, fromCache);
        }

        /// <summary>
        /// 得到所有下级
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fromCache">是否使用缓存</param>
        /// <returns></returns>
        public List<DictType> GetAllChilds(string id, bool fromCache = false)
        {
            List<DictType> list = new List<DictType>();
            addChilds(list, id, fromCache);
            return list;
        }

        private void addChilds(List<DictType> list, string id, bool fromCache = false)
        {
            var childs = fromCache ? getChildsByIDFromCache(id) : GetChildsByID(id);
            foreach (var child in childs)
            {
                list.Add(child);
                addChilds(list, child.ID, fromCache);
            }
        }

        /// <summary>
        /// 得到一个项的所有下级项ID字符串
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isSelf">是否包含自己</param>
        /// <returns></returns>
        public string GetAllChildsIDString(string id, bool isSelf = true)
        {
            StringBuilder sb = new StringBuilder();
            if (isSelf)
            {
                sb.Append(id);
                sb.Append(",");
            }
            var childs = GetAllChilds(id, true);
            foreach (var child in childs)
            {
                sb.Append(child.ID);
                sb.Append(",");
            }
            return sb.ToString().TrimEnd(',');
        }

        /// <summary>
        /// 查询上级记录
        /// </summary>
        public DictType GetParent(string id)
        {
            return dictTypeDAL.GetParent(id);
        }

        /// <summary>
        /// 是否包含下级记录
        /// </summary>
        public bool HasChilds(string id)
        {
            return dictTypeDAL.HasChilds(id);
        }

        /// <summary>
        /// 得到最大排序
        /// </summary>
        public int GetMaxSort(string id)
        {
            return dictTypeDAL.GetMaxSort(id);
        }

        /// <summary>
        /// 更新排序
        /// </summary>
        public int UpdateSort(string id, int sort)
        {
            return dictTypeDAL.UpdateSort(id, sort);
        }

        /// <summary>
        /// 根据代码查询一条记录
        /// </summary>
        /// <param name="code"></param>
        /// <param name="fromCache">是否使用缓存</param>
        /// <returns></returns>
        public DictType GetByCode(string code, bool fromCache = false)
        {
            return code.IsNullOrEmpty() ? null :
                fromCache ? GetAll().Find(p => string.Compare(p.Code, code, true) == 0) : dictTypeDAL.GetByCode(code.Trim());
        }

        /// <summary>
        /// 下拉选项时以哪个字段作为值字段
        /// </summary>
        public enum OptionValueField
        {
            ID,
            Name,
            Code,
            Value,
            Other,
            Remark
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
                options.AppendFormat("<option value=\"{0}\"{1}>{2}{3}</option>" + parentCount, value1, value1 == value ? " selected=\"selected\"" : "", space.ToString(), child.Name);
            }
            return options.ToString();
        }


        /// <summary>
        /// 得到一个字典项的上级节点数
        /// </summary>
        /// <param name="dictList"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        private int getParentCount(List<DictType> dictList, DictType dict)
        {
            int parent = 0;
            DictType parentDict = dictList.Find(p => p.ID == dict.PID);
            while (parentDict != null)
            {
                parentDict = dictList.Find(p => p.ID == parentDict.PID);
                parent++;
            }
            return parent;
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
        /// 根据ID得到单选项
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name">名称</param>
        /// <param name="valueField"></param>
        /// <param name="value"></param>
        /// <param name="attr">其它属性</param>
        /// <returns></returns>
        public string GetRadiosByID(string id, string name, OptionValueField valueField = OptionValueField.Value, string value = "", string attr = "")
        {
            var childs = GetChildsByID(id, true);
            return getRadios(childs, name, valueField, value, attr);
        }

        /// <summary>
        /// 根据代码得到单选项
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name">名称</param>
        /// <param name="valueField"></param>
        /// <param name="value"></param>
        /// <param name="attr">其它属性</param>
        /// <returns></returns>
        public string GetRadiosByCode(string code, string name, OptionValueField valueField = OptionValueField.Value, string value = "", string attr = "")
        {
            if (code.IsNullOrEmpty()) return "";
            var childs = GetChildsByCode(code.Trim(), true);
            return getRadios(childs, name, valueField, value, attr);
        }

        private string getRadios(List<DictType> childs, string name, OptionValueField valueField = OptionValueField.Value, string value = "", string attr = "")
        {
            StringBuilder options = new StringBuilder(childs.Count * 100);
            foreach (var child in childs)
            {
                string value1 = getOptionsValue(valueField, child);
                options.Append("<input type=\"radio\" style=\"vertical-align:middle;\" ");
                options.AppendFormat("id=\"{0}_{1}\" ", name, child.ID.ToString());
                options.AppendFormat("name=\"{0}\" ", name);
                options.AppendFormat("value=\"{0}\" ", value1);
                options.Append(string.Compare(value, value1, true) == 0 ? "checked=\"checked\" " : "");
                options.Append(attr);
                options.Append("/>");
                options.AppendFormat("<label style=\"vertical-align:middle;margin-right:3px;\" for=\"{0}_{1}\">{2}</label>", name, child.ID.ToString(), child.Name);
            }
            return options.ToString();
        }

        /// <summary>
        /// 根据ID得到多选项
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name">名称</param>
        /// <param name="valueField"></param>
        /// <param name="value"></param>
        /// <param name="attr">其它属性</param>
        /// <returns></returns>
        public string GetCheckboxsByID(string id, string name, OptionValueField valueField = OptionValueField.Value, string value = "", string attr = "")
        {
            var childs = GetChildsByID(id, true);
            return getCheckboxs(childs, name, valueField, value, attr);
        }

        /// <summary>
        /// 根据代码得到多选项
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name">名称</param>
        /// <param name="valueField"></param>
        /// <param name="value"></param>
        /// <param name="attr">其它属性</param>
        /// <returns></returns>
        public string GetCheckboxsByCode(string code, string name, OptionValueField valueField = OptionValueField.Value, string value = "", string attr = "")
        {
            if (code.IsNullOrEmpty()) return "";
            var childs = GetChildsByCode(code.Trim(), true);
            return getCheckboxs(childs, name, valueField, value, attr);
        }

        private string getCheckboxs(List<DictType> childs, string name, OptionValueField valueField = OptionValueField.Value, string value = "", string attr = "")
        {
            StringBuilder options = new StringBuilder(childs.Count * 100);
            foreach (var child in childs)
            {
                string value1 = getOptionsValue(valueField, child);
                options.Append("<input type=\"checkbox\" style=\"vertical-align:middle;\" ");
                options.AppendFormat("id=\"{0}_{1}\" ", name, child.ID.ToString());
                options.AppendFormat("name=\"{0}\" ", name);
                options.AppendFormat("value=\"{0}\" ", value1);
                options.Append(value.Contains(value1) ? "checked=\"checked\"" : "");
                options.Append(attr);
                options.Append("/>");
                options.AppendFormat("<label style=\"vertical-align:middle;margin-right:3px;\" for=\"{0}_{1}\">{2}</label>", name, child.ID.ToString(), child.Name);
            }
            return options.ToString();
        }

        private string getOptionsValue(OptionValueField valueField, DictType dict)
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
                case OptionValueField.Remark:
                    value = dict.Remark;
                    break;
                case OptionValueField.Other:
                    value = dict.Other;
                    break;
                case OptionValueField.Name:
                    value = dict.Name;
                    break;
                case OptionValueField.Value:
                    value = dict.Value;
                    break;
            }
            return value;
        }



        /// <summary>
        /// 检查代码是否存在
        /// </summary>
        /// <param name="code"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool HasCode(string code, string id = "")
        {
            if (code.IsNullOrEmpty())
            {
                return false;
            }
            var dict = GetByCode(code.Trim());
            string gid;
            if (dict == null)
            {
                return false;
            }
            else
            {
                if (id.IsGuid(out gid) && dict.ID == gid)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// 删除一个字典及其所有下级
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteAndAllChilds(string id)
        {
            int i = 0;
            var childs = GetAllChilds(id);
            foreach (var child in childs)
            {
                Delete(child.ID);
                i++;
            }
            Delete(id);
            i++;
            return i;
        }

        /// <summary>
        /// 得到标题
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetTitleByID(string id)
        {
            var dict = FindByID(id);
            return dict == null ? "" : dict.Name;
        }
        /// <summary>
        /// 得到标题
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetTitleByCode(string code)
        {
            if (code.IsNullOrEmpty()) return "";
            var dict = GetByCode(code.Trim(), true);
            return dict == null ? "" : dict.Name;
        }
        /// <summary>
        /// 得到标题
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetTitle(string code, string value)
        {
            if (code.IsNullOrEmpty()) return "";
            var childs = getChildsByCodeFromCache(code.Trim());
            var child = childs.Find(p => p.Value == value);
            return child == null ? "" : child.Name;
        }

        /// <summary>
        /// 根据代码得到ID
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetIDByCode(string code)
        {
            var dict = GetByCode(code, true);
            return dict == null ? string.Empty : dict.ID;
        }

        /// <summary>
        /// 得到combox表格html
        /// </summary>
        /// <param name="id"></param>
        /// <param name="valueField"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string GetComboxTableHtmlByID(string id, OptionValueField valueField, string defaultValue)
        {
            if (!id.IsGuid())
            {
                return "";
            }
            var dicts = GetChildsByID(id);
            StringBuilder html = new StringBuilder(2000);
            html.Append("<table><thead><tr><th>标题</th><th>备注</th><th>其它</th></tr></thead><tbody>");
            foreach (var dict in dicts)
            {
                html.Append("<tr>");
                html.AppendFormat("<td value=\"{0}\"{1}>", dict.ID, dict.ID.ToString().Equals(defaultValue, StringComparison.CurrentCultureIgnoreCase) ? " selected=\"selected\"" : "");
                html.Append(dict.Name);
                html.Append("</td>");
                html.Append("<td>");
                html.Append(dict.Remark);
                html.Append("</td>");
                html.Append("<td>");
                html.Append(dict.Other);
                html.Append("</td>");
                html.Append("</tr>");
            }
            html.Append("</tbody>");
            html.Append("</table>");
            return html.ToString();
        }*/
    }
}