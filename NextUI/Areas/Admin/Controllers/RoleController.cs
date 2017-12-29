using Next.Admin.BLL;
using Next.Admin.Entity;
using Next.Admin.Model;
using Next.Controllers;
using Next.Framework.Core;
using Next.Framework.Core.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Next.Areas.Admin.Controllers
{
    public class RoleController : BusinessController<RoleBLL,Role>
    {
        public RoleController()
            : base()
        {

        }
        //
        // GET: /Role/
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult EditFunctions(string roleID, string newList)
        {
            if (!string.IsNullOrEmpty(roleID))
            {
                if (!string.IsNullOrWhiteSpace(newList))
                {
                    List<string> list = new List<string>();
                    foreach (string id in newList.Split(','))
                    {
                        list.Add(id);
                    }
                    BLLFactory<RoleBLL>.Instance.EditRoleFunctions(roleID, list);
                    return Content("true");
                }

            }
            return Content("");
        }
        public ActionResult GetRoleCategorys()
        {
            List<CListItem> listItem = new List<CListItem>();
            string[] enumNames = EnumHelper.GetMemberNames<RoleCategoryEnum>();
            foreach (string item in enumNames)
            {
                listItem.Add(new CListItem(item));
            }
            return Json(listItem, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditUsers(string roleID, string newList)
        {
            if (!string.IsNullOrEmpty(roleID))
            {
                if (!string.IsNullOrWhiteSpace(newList))
                {
                    List<string> list = new List<string>();
                    foreach (string id in newList.Split(','))
                    {
                        list.Add(id);
                    }
                    BLLFactory<RoleBLL>.Instance.EditRoleUsers(roleID, list);
                    return Content("true");
                }

            }
            return Content("");
        }

        public ActionResult EditDepts(string roleID, string newList)
        {
            if (!string.IsNullOrEmpty(roleID))
            {
                if (!string.IsNullOrWhiteSpace(newList))
                {
                    List<string> list = new List<string>();
                    foreach (string id in newList.Split(','))
                    {
                        list.Add(id);
                    }
                    BLLFactory<RoleBLL>.Instance.EditRoleDepts(roleID, list);
                    return Content("true");
                }

            }
            return Content("");
        }
        public override ActionResult Insert(Role info)
        {
            base.CheckAuthorized(AuthorizeKey.InsertKey);
            string filter = string.Format("Name='{0}'", info.Name);
            bool isExist = BLLFactory<RoleBLL>.Instance.IsExistRecord(filter);
            if (isExist)
            {
                throw new ArgumentException("指定角色名称重复，请重新输入！");
            }
           
            SetCommonInfo(info);
            return base.Insert(info);
        }
        protected override bool Update(string id, Role info)
        {
            string filter = string.Format("Name='{0}' AND ID<>'{1}'", info.Name, info.ID);
            bool isExist = BLLFactory<RoleBLL>.Instance.IsExistRecord(filter);
            if (isExist)
            {
                throw new ArgumentException("指定角色名称重复，请重新输入！");
            }
            return base.Update(id, info);
        }
        public void SetCommonInfo(Role info)
        {

        }
        public ActionResult GetMyRoleTreeJson(string userID)
        {
            StringBuilder content = new StringBuilder();
            User userInfo = BLLFactory<UserBLL>.Instance.FindByID(userID);
            if (userInfo != null)
            {
                
                Dept groupInfo = GetMyTopGroup(userInfo);


                if (groupInfo != null)
                {
                    EasyTreeData topnode = new EasyTreeData( "dept"+groupInfo.ID, groupInfo.Name, GetIconcls(groupInfo.Category));
                    AddRole(groupInfo, topnode);
                    if (groupInfo.Category == "总部")
                    {
                        List<Dept> list = BLLFactory<DeptBLL>.Instance.GetAllCompany();
                        foreach (Dept info in list)
                        {
                            EasyTreeData companyNode = new EasyTreeData( "dept"+info.ID, info.Name, GetIconcls(info.Category));
                            topnode.children.Add(companyNode);
                            AddRole(info, companyNode);
                        }
                    }
                    content.Append(base.ToJson(topnode));
                }

            }
            string json = string.Format("[{0}]", content.ToString().Trim(','));

            return Content(json);
        }

        private void AddRole(Dept deptInfo, EasyTreeData treeNode)
        {
            List<Role> roleList = BLLFactory<RoleBLL>.Instance.GetRolesByCompany(deptInfo.ID);
            foreach (Role roleInfo in roleList)
            {
                EasyTreeData roleNode = new EasyTreeData("role" + roleInfo.ID, roleInfo.Name, "icon-group-key");
                treeNode.children.Add(roleNode);
            }
        }

        public ActionResult GetRolesByUser(string userID)
        {
            if (!string.IsNullOrEmpty(userID))
            {
                List<Role> roleList=BLLFactory<RoleBLL>.Instance.GetRolesByUser(userID);
                return Json(roleList,JsonRequestBehavior.AllowGet);

            }
            return Content("");
        }

        public ActionResult GetRolesByDept(string deptid)
        {
            if (!string.IsNullOrEmpty(deptid))
            {
                List<Role> roleList = BLLFactory<RoleBLL>.Instance.GetRolesByDept(deptid);
                return Json(roleList, JsonRequestBehavior.AllowGet);

            }
            return Content("");

        }
        public ActionResult GetRolesByFunction(string functionId)
        {
            if (!string.IsNullOrEmpty(functionId))
            {
                List<Role> roleList = BLLFactory<RoleBLL>.Instance.GetRolesByFunction(functionId);
                return Json(roleList, JsonRequestBehavior.AllowGet);
            }
            return Content("");
        }
        public ActionResult EditUserRelation(string roleID, string addList, string removeList)
        {
            if (!string.IsNullOrEmpty(roleID))
            {
                if (!string.IsNullOrWhiteSpace(removeList))
                {
                    foreach (string id in removeList.Split(','))
                    {
                        if (!string.IsNullOrEmpty(id))
                        {
                            BLLFactory<RoleBLL>.Instance.RemoveUser(id, roleID);
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(addList))
                {
                    foreach (string id in addList.Split(','))
                    {
                        if (!string.IsNullOrEmpty(id))
                        {
                            BLLFactory<RoleBLL>.Instance.AddUser(id, roleID);
                        }
                    }
                }
                return Content("true");
            }
            return Content("");
        }
        public ActionResult EditDeptRelation(string roleID, string addList, string removeList)
        {
            if (!string.IsNullOrEmpty(roleID))
            {
                if (!string.IsNullOrWhiteSpace(removeList))
                {
                    foreach (string id in removeList.Split(','))
                    {
                        if (!string.IsNullOrEmpty(id))
                        {
                            BLLFactory<RoleBLL>.Instance.RemoveDept(id, roleID);
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(addList))
                {
                    foreach (string id in addList.Split(','))
                    {
                        if (!string.IsNullOrEmpty(id))
                        {
                            BLLFactory<RoleBLL>.Instance.AddDept(id, roleID);
                        }
                    }
                }
                return Content("true");
            }
            return Content("");
        }
        public ActionResult EditFunctionRelation(string roleID, string addList, string removeList)
        {
            if (!string.IsNullOrEmpty(roleID))
            {
                if (!string.IsNullOrWhiteSpace(removeList))
                {
                    foreach (string id in removeList.Split(','))
                    {
                        if (!string.IsNullOrEmpty(id))
                        {
                            BLLFactory<RoleBLL>.Instance.RemoveFunction(id, roleID);
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(addList))
                {
                    foreach (string id in addList.Split(','))
                    {
                        if (!string.IsNullOrEmpty(id))
                        {
                            BLLFactory<RoleBLL>.Instance.AddFunction(id, roleID);
                        }
                    }
                }
                return Content("true");
            }
            return Content("");
        }
       
	}
}