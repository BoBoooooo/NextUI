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
        public string GetOptionsByName(string name,string value="")
        {
            DictType dictType = BLLFactory<DictTypeBLL>.Instance.FindSingle("Name='" + name + "'");
            List<DictData> childs = BLLFactory<DictDataBLL>.Instance.Find("DictTypeID='" + dictType.ID + "'");
            StringBuilder options = new StringBuilder(childs.Count * 100);
            StringBuilder space = new StringBuilder();
            foreach (var child in childs)
            {
                space.Clear();
                options.AppendFormat("<option value=\"{0}\" {1} >{2}</option>", child.Value, child.Value == value ? " selected=\"selected\"" : "", child.Name);
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
            List<DictData> result = BLLFactory<DictDataBLL>.Instance.Find("DictTypeID='" + dictType.ID + "'");
            return result.FirstOrDefault().ID;
        }

	}
}
