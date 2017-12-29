using Next.Admin.BLL;
using Next.Admin.Entity;
using Next.Admin.Model;
using Next.Commons;
using Next.Framework.Commons;
using Next.Framework.Core;
using Next.Framework.Core.Commons;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Next.Controllers
{
    public class BusinessController<B, T> : BaseController
        where B : class
        where T : BaseEntity,new()
    {

        protected string bllAssemblyName = null;
        protected BaseBLL<T> baseBLL = null;

        public BusinessController()
        {
            this.bllAssemblyName = System.Reflection.Assembly.GetAssembly(typeof(B)).GetName().Name;
            this.baseBLL = Reflect<BaseBLL<T>>.Create(typeof(B).FullName, bllAssemblyName);
            if (baseBLL == null) {
                throw new ArgumentNullException("baseBLL", "未能成功创建对应的BaseBLL<T>业务访问层的对象");
            }
        }
        //
        // GET: /Business/
        protected Dept GetMyTopGroup(User userInfo)
        {
            Dept groupInfo = null;
            if (BLLFactory<UserBLL>.Instance.UserInRole(userInfo.Name, Role.SuperAdminName))
            {
                groupInfo = BLLFactory<DeptBLL>.Instance.GetTopGroup();
            }
            else
            {
                groupInfo = BLLFactory<DeptBLL>.Instance.FindByID(userInfo.CompanyID);
            }
            return groupInfo;
        }
        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult Insert(T info)
        {
            base.CheckAuthorized(AuthorizeKey.InsertKey);

            bool result = false;


            if (info != null)
            {

                result = baseBLL.Insert(info);
            }
            return Content(result);
        }
        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult Update(string id, FormCollection formValues)
        {
            base.CheckAuthorized(AuthorizeKey.UpdateKey);

            T obj = baseBLL.FindByID(id);
            if (obj != null)
            {
                foreach (string key in formValues.Keys)
                {
                    string value = formValues[key];
                    string temp=key.Substring(key.IndexOf('.')+1);
                    System.Reflection.PropertyInfo propertyInfo;
                    if (key.IndexOf('.') != -1)
                    {
                        propertyInfo = obj.GetType().GetProperty(key.Substring(key.IndexOf('.')+1));
                    }
                    else
                    {
                        propertyInfo = obj.GetType().GetProperty(key);
                    }
                    if (propertyInfo != null)
                    {
                        try
                        {
                            propertyInfo.SetValue(obj, Convert.ChangeType(value, propertyInfo.PropertyType), null);
                        }
                        catch (Exception e)
                        {
                            //throw new Exception("字段类型转换出错。" + e.ToString());
                        }
                    }
                }
            }
            
            bool result = Update(id, obj);
            return Content(result);
        }

        protected virtual bool Update(string id, T info)
        {
            return baseBLL.Update(info, id);
        }

        public virtual ActionResult Delete(string id)
        {
            base.CheckAuthorized(AuthorizeKey.DeleteKey);
            bool result = false;
            if (!string.IsNullOrEmpty(id))
            {
                result = baseBLL.Delete(id);
            }
            return Content(result);
        }
        public virtual ActionResult DeleteByIds(string ids)
        {
            base.CheckAuthorized(AuthorizeKey.DeleteKey);
            bool result = false;
            if (!string.IsNullOrEmpty(ids))
            {
                string[] idArray = ids.Split(new char[] { ',' });
                foreach (string strId in idArray)
                {
                    if (!string.IsNullOrEmpty(strId))
                    {
                        baseBLL.Delete(strId);
                    }
                }
                result = true;
            }
            return Content(result);
        }
        protected string GetIconcls(string category)
        {

            string result = "icon-house";
            if (category == DeptCategoryEnum.单位.ToString())
            {
                result = "icon-organ";
            }
            if (category == DeptCategoryEnum.部门.ToString())
            {
                result = "icon-group";
            }
            if (category == DeptCategoryEnum.工作组.ToString())
            {
                result = "icon-group";
            }
            return result;
        }

        public virtual ActionResult FindWithPager()
        {
            base.CheckAuthorized(AuthorizeKey.ListKey);
            string where = GetPagerCondition();
            PagerInfo pagerInfo = GetPagerInfo();
            List<T> list = baseBLL.FindWithPager(where, pagerInfo);
            var result = new { total = pagerInfo.RecordCount, rows = list };
            return JsonDate(result);
        }

        public virtual ActionResult FindByID(string id)
        {
            base.CheckAuthorized(AuthorizeKey.ViewKey);

            ActionResult result = Content("");
            T info = baseBLL.FindByID(id);
            if (info != null)
            {
                result = JsonDate(info);
            }
            return result;
        }
        protected virtual PagerInfo GetPagerInfo(){
            int pageIndex = Request["page"] == null ? 1 : int.Parse(Request["page"]);
            int pageSize = Request["rows"] == null ? 1 : int.Parse(Request["rows"]);

            PagerInfo pagerInfo=new PagerInfo();
            pagerInfo.CurrentPageIndex = pageIndex;
            pagerInfo.PageSize = pageSize;
            return pagerInfo;
        }
        public virtual string GetPagerCondition()
        {
            string where = "";
            string CustomedCondition = Request["CustomedCondition"] ?? "";
            if (!string.IsNullOrWhiteSpace(CustomedCondition))
            {
                where = CustomedCondition;
            }
            else
            {
                SearchCondition condition = new SearchCondition();
                DataTable dt = baseBLL.GetFieldTypeList();
                foreach (DataRow dr in dt.Rows)
                {
                    string columnName = dr["ColumnName"].ToString();
                    string dataType = dr["DataType"].ToString();
                    string columnValue = Request[columnName] ?? "";
                    if (columnName.ToUpper().Equals("URL"))
                    {
                        continue;
                    }
                    if (IsDateTime(dataType))
                    {
                        condition.AddDateCondition(columnName, columnValue);
                    }
                    else if(IsNumericType(dataType))
                    {
                        bool boolValue = false;
                        if (bool.TryParse(columnValue, out boolValue))
                        {
                            condition.AddCondition(columnName,boolValue ? 1 : 0, SqlOperator.Equal);
                        }
                        else
                        {
                            condition.AddNumberCondition(columnName, columnValue);
                        }
                    }
                    else
                    {

                        if (ValidateUtil.IsNumeric(columnValue))
                        {
                            condition.AddCondition(columnName, columnValue, SqlOperator.Equal);
                        }
                        else
                        {
                            condition.AddCondition(columnName, columnValue,SqlOperator.Like);
                        }
                    }
                    
                }
                where = condition.BuildConditionSql().Replace("Where", "");
            }
            return where;
        }
        protected bool IsDateTime(string dataType)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(dataType))
            {
                dataType = dataType.ToLower();
                if (dataType.ToLower().Contains("datatime"))
                {
                    result = true;
                }

            }
            return result;
        }

        protected bool IsNumericType(string dataType)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(dataType))
            {
                dataType = dataType.ToLower();
                if(dataType.Contains("int")||dataType.Contains("decimal")||dataType.Contains("double")||
                    dataType.Contains("single")||dataType.Contains("byte")||dataType.Contains("short")||
                    dataType.Contains("float"))
                {
                    result = true;
                }
            }
            return result;
        }

	}
}