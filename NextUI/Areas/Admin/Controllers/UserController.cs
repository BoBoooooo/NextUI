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
    public class UserController : BusinessController<UserBLL, User>
    {
        public UserController()
            : base()
        {

        }

        public ActionResult ChangePassword()
        {
            ViewBag.Name = CurrentUser.Name;
            return View();
        }
        public ActionResult ResetPassword(string id)
        {
            string result = "口令初始化失败";
            if(string.IsNullOrEmpty(id)){
                result="用户ID不能为空";
            }
            else{
                User info=BLLFactory<UserBLL>.Instance.FindByID(id);
                if(info!=null){
                    string defaultPassword="12345678";
                    bool tempBool=BLLFactory<UserBLL>.Instance.ModifyPassword(info.Name,defaultPassword);
                    if(tempBool){
                        result="OK";
                    }else{
                        result="口令初始化失败";
                    }
                }
            }
            return Content(result);
        }
        public ActionResult ModifyPassword(string name, string oldpass, string newpass)
        {
            string result = "";
            User info = new User();
            info.Name = name;
            info.Password = oldpass;
            bool identity = BLLFactory<UserBLL>.Instance.VerifyUser(info);
            if (!identity)
            {
                result = "原口令错误";
            }
            else
            {
                bool tempBool = BLLFactory<UserBLL>.Instance.ModifyPassword(name, newpass);
                if (tempBool)
                {
                    result = "OK";
                }
                else
                {
                    result = "口令初始化失败";
                }
            }
            return Content(result);
        }
        //
        // GET: /User/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetMyDeptTreeJson(string userID)
        {
            StringBuilder content = new StringBuilder();
            User userInfo = BLLFactory<UserBLL>.Instance.FindByID(userID);
            if (userInfo != null)
            {
                Dept groupInfo = GetMyTopGroup(userInfo);
                if (groupInfo != null)
                {
                    List<DeptNode> list = BLLFactory<DeptBLL>.Instance.GetTreeByID(groupInfo.ID);
                    EasyTreeData treeData = new EasyTreeData(groupInfo.ID, groupInfo.Name, GetIconcls(groupInfo.Category));
                    GetTreeDataWithDeptNode(list, treeData);
                    content.Append(base.ToJson(treeData));
                }

            }
            string json = string.Format("[{0}]", content.ToString().Trim(','));

            return Content(json);
        }
        private void GetTreeDataWithDeptNode(List<DeptNode> list, EasyTreeData parent)
        {
            List<EasyTreeData> result = new List<EasyTreeData>();
            foreach (DeptNode dept in list)
            {
                EasyTreeData treeData = new EasyTreeData(dept.ID, dept.Name, GetIconcls(dept.Category));
                GetTreeDataWithDeptNode(dept.Children, treeData);
                result.Add(treeData);
            }
            parent.children.AddRange(result);
        }

        public override ActionResult FindWithPager()
        {
            string roleID = Request["RoleID"] ?? "";
            if (!string.IsNullOrEmpty(roleID))
            {
                base.CheckAuthorized(AuthorizeKey.ListKey);
                List<User> list = BLLFactory<UserBLL>.Instance.GetUsersByRole(roleID);
                List<UserExtension> extList = new List<UserExtension>();
                foreach (User info in list)
                {
                    UserExtension item = new UserExtension(info);
                    item.DeptName = BLLFactory<DeptBLL>.Instance.FindByID(item.DeptID).Name;
                    item.CompanyName = BLLFactory<DeptBLL>.Instance.FindByID(item.CompanyID).Name;
                    extList.Add(item);
                }

                var result = new { total = list.Count, rows = extList };
                return JsonDate(result);

            }
            else
            {
                base.CheckAuthorized(AuthorizeKey.ListKey);
                string where = GetPagerCondition();
                PagerInfo pagerInfo = GetPagerInfo();
                List<User> list = baseBLL.FindWithPager(where, pagerInfo);


                var result = new { total = pagerInfo.RecordCount, rows = list };

                return JsonDate(result);
            }
        }
        public ActionResult GetMyCompanyTreeJson(string userID)
        {
            List<EasyTreeData> treeList = new List<EasyTreeData>();

            User userInfo = BLLFactory<UserBLL>.Instance.FindByID(userID);
            if (userInfo != null)
            {
                List<DeptNode> list = new List<DeptNode>();
                if (BLLFactory<UserBLL>.Instance.UserInRole(userInfo.Name, Role.SuperAdminName))
                {
                    list = BLLFactory<DeptBLL>.Instance.GetGroupCompanyTree();
                }
                else
                {
                    Dept myCompanyInfo = BLLFactory<DeptBLL>.Instance.FindByID(userInfo.CompanyID);
                    if (myCompanyInfo != null)
                    {
                        list.Add(new DeptNode(myCompanyInfo));
                    }
                }
                if (list.Count > 0)
                {
                    DeptNode info = list[0];
                    EasyTreeData node = new EasyTreeData(info.ID, info.Name, GetIconcls(info.Category));
                    GetTreeDataWithDeptNode(info.Children, node);
                    treeList.Add(node);
                }
            }
            string json = ToJson(treeList);
            return Content(json);
        }
        public ActionResult GetDeptTreeJson(string parentID)
        {
            List<EasyTreeData> treeList = new List<EasyTreeData>();
            treeList.Insert(0, new EasyTreeData(-1, "无"));

            if (!string.IsNullOrEmpty(parentID))
            {
                Dept groupInfo = BLLFactory<DeptBLL>.Instance.FindByID(parentID);
                if (groupInfo != null)
                {
                    List<DeptNode> list = BLLFactory<DeptBLL>.Instance.GetTreeByID(groupInfo.ID);
                    EasyTreeData treeData = new EasyTreeData(groupInfo.ID, groupInfo.Name, "icon-group");
                    GetTreeDataWithDeptNode(list, treeData);
                    treeList.Add(treeData);
                }

            }

            string json = ToJson(treeList);
            return Content(json);

        }
        public ActionResult GetUsersByDept(string deptID)
        {
            ActionResult result = Content("");
            if (!string.IsNullOrEmpty(deptID))
            {
                List<User> userList = BLLFactory<UserBLL>.Instance.GetUsersByDept(deptID);
                result = JsonDate(userList);
            }
            return result;
        }
        public ActionResult GetUserTreeJson(string deptID)
        {
            List<EasyTreeData> treeList = new List<EasyTreeData>();
            treeList.Insert(0, new EasyTreeData(-1, "无"));

            List<User> list = BLLFactory<UserBLL>.Instance.FindByDept(deptID);
            foreach (User info in list)
            {
                treeList.Add(new EasyTreeData(info.ID, info.FullName, "icon-user"));
            }

            string json = ToJson(treeList);
            return Content(json);

        }

        public override ActionResult Insert(User info)
        {
            base.CheckAuthorized(AuthorizeKey.InsertKey);

            string filter = string.Format("Name='{0}'", info.Name);
            bool isExist = BLLFactory<UserBLL>.Instance.IsExistRecord(filter);
            if (isExist)
            {
                throw new ArgumentException("指定用户名重复，请重新输入");
            }
            SetCommonInfo(info);
            return base.Insert(info);

        }

        protected override bool Update(string id, User info)
        {
            base.CheckAuthorized(AuthorizeKey.InsertKey);

            string filter = string.Format("Name='{0}' And ID<>'{1}' ", info.Name, info.ID);
            bool isExist = BLLFactory<UserBLL>.Instance.IsExistRecord(filter);
            if (isExist)
            {
                throw new ArgumentException("指定用户名重复，请重新输入");
            }
            SetCommonInfo(info);
            return base.Update(id, info);
        }
        public override string GetPagerCondition()
        {
            string condition = "";
            string deptId = Request["DeptID"] ?? "";

            if (!string.IsNullOrEmpty(deptId))
            {
                condition = string.Format("DeptID='{0}' or CompanyID='{0}'", deptId);
            }
            else
            {
                condition = base.GetPagerCondition();
            }
            return condition;
        }

        private void SetCommonInfo(User info)
        {

            info.DeptName = BLLFactory<DeptBLL>.Instance.FindByID(info.DeptID).Name;
            info.CompanyName = BLLFactory<DeptBLL>.Instance.FindByID(info.CompanyID).Name;
        }
        public ActionResult GetUsersByRole(string roleID)
        {
            ActionResult result = Content("");
            if (!string.IsNullOrEmpty(roleID))
            {
                List<User> roleList = BLLFactory<UserBLL>.Instance.GetUsersByRole(roleID);
                result = Json(roleList, JsonRequestBehavior.AllowGet);
            }
            return result;
        }
        public ActionResult FindUserExtensionByID(string userID)
        {
            ActionResult result = Content("");
            User info = baseBLL.FindByID(userID);
            if (info != null)
            {
                UserExtension item = new UserExtension(info);
                item.DeptName = BLLFactory<DeptBLL>.Instance.FindByID(item.DeptID).Name;
                item.CompanyName = BLLFactory<DeptBLL>.Instance.FindByID(item.CompanyID).Name;
                result = JsonDate(item);
            }
            return result;
        }
    }
}