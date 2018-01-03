using System;
using System.Collections.Generic;
using System.Text;
using Next.Framework.Core;
using Next.Admin.Entity;
using Next.Admin.DALMySql;
using Next.Admin.IDAL;
using System.Linq;

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


        /// <summary>
        /// 下拉选项时以哪个字段作为值字段
        /// </summary>
        public enum OptionValueField
        {
            ID,
            Name,
            Value,
            Remark
        }

        /// <summary>
        /// 根据代码得到选项
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetOptionsByName(string name, string value = "")
        {
            DictType dictType = BLLFactory<DictTypeBLL>.Instance.FindSingle("Name='" + name + "'");
            List<DictType> childs = BLLFactory<DictTypeBLL>.Instance.Find("PID='" + dictType.ID + "'");
            StringBuilder options = new StringBuilder(childs.Count * 100);
            StringBuilder space = new StringBuilder();
            foreach (var child in childs)
            {
                space.Clear();
                options.AppendFormat("<option value=\"{0}\" {1} >{2}</option>", child.Name, child.Name == value ? " selected=\"selected\"" : "", child.Name);
            }
            return options.ToString();
        }

        /// <summary>
        /// 根据代码得到ID
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetIDByName(string name)
        {
            DictType dictType = BLLFactory<DictTypeBLL>.Instance.FindSingle("Name='" + name + "'");
            //List<DictData> result = BLLFactory<DictDataBLL>.Instance.Find("PID='" + dictType.ID + "'");
            return dictType.ID;
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
            var childs = GetAllChilds(id);
            foreach (var child in childs)
            {
                sb.Append(child.ID);
                sb.Append(",");
            }
            return sb.ToString().TrimEnd(',');
        }

        /// <summary>
        /// 得到所有下级
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fromCache">是否使用缓存</param>
        /// <returns></returns>
        public List<DictType> GetAllChilds(string id)
        {
            List<DictType> list = new List<DictType>();
            addChilds(list, id);
            return list;
        }

        private void addChilds(List<DictType> list, string id)
        {
            var childs = GetChilds(id);
            foreach (var child in childs)
            {
                list.Add(child);
                addChilds(list, child.ID);
            }
        }
        /// <summary>
        /// 查询下级记录
        /// </summary>
        public List<DictType> GetChilds(string id)
        {
            return dictTypeDAL.GetChilds(id);
        }

        /// <summary>
        /// 查询根记录
        /// </summary>
        public DictType GetRoot()
        {
            return dictTypeDAL.GetRoot();
        }
        /// <summary>
        /// 是否包含下级记录
        /// </summary>
        public bool HasChilds(string id)
        {
            return dictTypeDAL.HasChilds(id);
        }
	}
}
