using Next.Admin.BLL;
using Next.Admin.Entity;
using Next.Admin.Model;
using Next.Controllers;
using Next.Framework.Core;
using Next.Framework.Core.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Next.Areas.Admin.Controllers
{
    public class DeptController : BusinessController<DeptBLL,Dept>
    {
        public DeptController()
            : base()
        {

        }
        //
        // GET: /Dept/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDeptsByUser(string userID)
        {
            if (!string.IsNullOrEmpty(userID))
            {
                List<Dept> deptList = BLLFactory<DeptBLL>.Instance.GetDeptsByUser(userID);
                return Json(deptList, JsonRequestBehavior.AllowGet);
            }
            return Content("");
        }
        public ActionResult GetDeptsByRole(string roleID)
        {
            if (!string.IsNullOrEmpty(roleID))
            {
                List<Dept> deptList = BLLFactory<DeptBLL>.Instance.GetDeptsByRole(roleID);
                return Json(deptList, JsonRequestBehavior.AllowGet);
            }
            return Content("");
        }
        public ActionResult GetDeptCategorys()
        {
            List<CListItem> listItem = new List<CListItem>();
            string[] enumNames = EnumHelper.GetMemberNames<DeptCategoryEnum>();
            foreach (string item in enumNames)
            {
                listItem.Add(new CListItem(item));
            }
            return Json(listItem, JsonRequestBehavior.AllowGet);
        }

        public override ActionResult Insert(Dept info)
        {
            base.CheckAuthorized(AuthorizeKey.InsertKey);
            string filter = string.Format("Name='{0}' AND PID='{1}'", info.Name,info.PID);
            bool isExist = BLLFactory<DeptBLL>.Instance.IsExistRecord(filter);
            if (isExist)
            {
                throw new ArgumentException("指定机构名称重复，请重新输入！");
            }
            
            SetCommonInfo(info);
            return base.Insert(info);
        }

        protected override bool Update(string id, Dept info)
        {
            string filter = string.Format("Name='{0}' AND ID<>'{1}' AND PID='{1}'", info.Name, info.ID, info.PID);
            bool isExist = BLLFactory<DeptBLL>.Instance.IsExistRecord(filter);
            if (isExist)
            {
                throw new ArgumentException("指定机构名称重复，请重新输入！");
            }
            return base.Update(id, info);
        }

        public void SetCommonInfo(Dept info)
        {

        }

        public ActionResult EditDeptUsers(string deptID, string newList)
        {
            if (!string.IsNullOrEmpty(deptID))
            {
                if (!string.IsNullOrWhiteSpace(newList))
                {
                    List<string> list = new List<string>();
                    foreach (string id in newList.Split(','))
                    {
                        list.Add(id);
                    }
                    BLLFactory<DeptBLL>.Instance.EditDeptUsers(deptID, list);
                    return Content("true");
                }

            }
            return Content("");
        }

        public ActionResult EditUserRelation(string deptID, string addList, string removeList)
        {
            if (!string.IsNullOrEmpty(deptID))
            {
                if (!string.IsNullOrWhiteSpace(removeList))
                {
                    foreach (string id in removeList.Split(','))
                    {
                        if (!string.IsNullOrEmpty(id))
                        {
                            BLLFactory<DeptBLL>.Instance.RemoveUser(id, deptID);
                        }  
                    }
                }
                if (!string.IsNullOrWhiteSpace(addList))
                {
                    foreach (string id in addList.Split(','))
                    {
                        if (!string.IsNullOrEmpty(id))
                        {
                            BLLFactory<DeptBLL>.Instance.AddUser(id, deptID);
                        }
                    }
                }
                
            }
            return Content("true");
        }
	}
}