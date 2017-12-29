using Next.Admin.BLL;
using Next.Admin.Entity;
using Next.Admin.Model;
using Next.Controllers;
using Next.Framework.Core;
using Next.Framework.Core.Commons;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Next.Areas.Admin.Controllers
{
    public class FunctionController : BusinessController<FunctionBLL,Function>
    {
        public FunctionController()
            : base()
        {

        }
        //
        // GET: /Function/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetFunctionTreeJsonByUser(string userID)
        {
            List<EasyTreeData> treeList = new List<EasyTreeData>();
            List<SystemType> typeList = BLLFactory<SystemTypeBLL>.Instance.GetAll();
            foreach (SystemType typeInfo in typeList)
            {
                EasyTreeData parentNode = new EasyTreeData(typeInfo.ID, typeInfo.Name, "icon-organ");
                List<FunctionNode> list = BLLFactory<FunctionBLL>.Instance.GetFunctionNodesByUser(userID, typeInfo.ID);
                AddFunctionNode(parentNode, list);
                treeList.Add(parentNode);
            }

            if (treeList.Count == 0)
            {
                treeList.Insert(0, new EasyTreeData(-1, "无"));
            }
            string json = ToJson(treeList);
            return Content(json);
        }
        public ActionResult GetFunctions(string roleID)
        {
            ActionResult result = Content("");
            if (!string.IsNullOrEmpty(roleID))
            {
                List<Function> roleList = BLLFactory<FunctionBLL>.Instance.GetFunctionsByRole(roleID);
                result = Json(roleList, JsonRequestBehavior.AllowGet);
            }
            return result;
        }
        public ActionResult GetAllTreeJson()
        {
            List<EasyTreeData> treeList = new List<EasyTreeData>();
            List<SystemType> typeList = BLLFactory<SystemTypeBLL>.Instance.GetAll();
            foreach (SystemType typeInfo in typeList)
            {
                EasyTreeData pNode = new EasyTreeData(typeInfo.ID, typeInfo.Name, "icon-computer");
                treeList.Add(pNode);

                string systemType = typeInfo.ID;
                List<FunctionNode> functionList = BLLFactory<FunctionBLL>.Instance.GetTree(systemType);
                foreach (FunctionNode info in functionList)
                {
                    EasyTreeData item = new EasyTreeData(info.ID, info.Name, "icon-key");
                    pNode.children.Add(item);
                    AddChildNode(info.Children, item);
                }
            }
            string json = ToJson(treeList);
            return Content(json);
        }
        public ActionResult BatchAddFunction(Function mainInfo, string controlString)
        {
            List<string> controlList = new List<string>();
            if (!string.IsNullOrWhiteSpace(controlString))
            {
                foreach (string item in controlString.ToLower().Split(','))
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        controlList.Add(item);
                    }
                }
            }

            bool result = false;
            using (DbTransaction trans = BLLFactory<FunctionBLL>.Instance.CreateTransaction())
            {
                try
                {
                    if (trans != null)
                    {
                        bool success = BLLFactory<FunctionBLL>.Instance.Insert(mainInfo, trans);
                        if (success)
                        {
                            Function subInfo = null;
                            int sortCodeIndex = 1;
                            if (controlList.Contains("add"))
                            {
                                subInfo = CreateSubFunction(mainInfo);
                                subInfo.SortCode = (sortCodeIndex++).ToString("D2");
                                subInfo.ControlID = string.Format("{0}/Add", mainInfo.ControlID);
                                subInfo.Name = string.Format("添加{0}", mainInfo.Name);

                                BLLFactory<FunctionBLL>.Instance.Insert(subInfo, trans);
                            }
                            if (controlList.Contains("delete"))
                            {
                                subInfo = CreateSubFunction(mainInfo);
                                subInfo.SortCode = (sortCodeIndex++).ToString("D2");
                                subInfo.ControlID = string.Format("{0}/Delete", mainInfo.ControlID);
                                subInfo.Name = string.Format("删除{0}", mainInfo.Name);

                                BLLFactory<FunctionBLL>.Instance.Insert(subInfo, trans);
                            }
                            if (controlList.Contains("edit") || controlList.Contains("modify"))
                            {
                                subInfo = CreateSubFunction(mainInfo);
                                subInfo.SortCode = (sortCodeIndex++).ToString("D2");
                                subInfo.ControlID = string.Format("{0}/Edit", mainInfo.ControlID);
                                subInfo.Name = string.Format("修改{0}", mainInfo.Name);

                                BLLFactory<FunctionBLL>.Instance.Insert(subInfo, trans);
                            }
                            if (controlList.Contains("view"))
                            {
                                subInfo = CreateSubFunction(mainInfo);
                                subInfo.SortCode = (sortCodeIndex++).ToString("D2");
                                subInfo.ControlID = string.Format("{0}/View", mainInfo.ControlID);
                                subInfo.Name = string.Format("查看{0}", mainInfo.Name);

                                BLLFactory<FunctionBLL>.Instance.Insert(subInfo, trans);
                            }
                            if (controlList.Contains("import"))
                            {
                                subInfo = CreateSubFunction(mainInfo);
                                subInfo.SortCode = (sortCodeIndex++).ToString("D2");
                                subInfo.ControlID = string.Format("{0}/Import", mainInfo.ControlID);
                                subInfo.Name = string.Format("导入{0}", mainInfo.Name);

                                BLLFactory<FunctionBLL>.Instance.Insert(subInfo, trans);
                            }
                            if (controlList.Contains("export"))
                            {
                                subInfo = CreateSubFunction(mainInfo);
                                subInfo.SortCode = (sortCodeIndex++).ToString("D2");
                                subInfo.ControlID = string.Format("{0}/Export", mainInfo.ControlID);
                                subInfo.Name = string.Format("导出{0}", mainInfo.Name);

                                BLLFactory<FunctionBLL>.Instance.Insert(subInfo, trans);
                            }
                            trans.Commit();
                            result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (trans != null)
                    {
                        trans.Rollback();
                    }
                    LogTextHelper.Error(ex);
                    throw;
                }
            }
            return Content(result);
        }
        private Function CreateSubFunction(Function mainInfo)
        {
            Function subInfo = new Function();
            subInfo.PID = mainInfo.ID;
            subInfo.SystemTypeID = mainInfo.SystemTypeID;
            return subInfo;
        }
        private void AddChildNode(List<FunctionNode> list, EasyTreeData fnode)
        {
            foreach (FunctionNode info in list)
            {
                EasyTreeData item = new EasyTreeData(info.ID, info.Name, "icon-key");
                fnode.children.Add(item);
                AddChildNode(info.Children, item);
            }
        }

        public void AddFunctionNode(EasyTreeData node, List<FunctionNode> list)
        {
            foreach (FunctionNode info in list)
            {
                EasyTreeData subNode = new EasyTreeData(info.ID, info.Name, info.Children.Count > 0 ? "icon-group-key" : "icon-key");
                node.children.Add(subNode);
                AddFunctionNode(subNode, info.Children);
            }
        }
        public override ActionResult Insert(Function info)
        {
            base.CheckAuthorized(AuthorizeKey.InsertKey);
            string filter = string.Format("ControlID='{0}' and SystemTypeID='{1}' and Name='{2}'", info.ControlID,info.SystemTypeID,info.Name);
            bool isExist = BLLFactory<FunctionBLL>.Instance.IsExistRecord(filter);
            if (isExist)
            {
                throw new ArgumentException("指定功能控制ID重复，请重新输入！");
            }

            SetCommonInfo(info);
            return base.Insert(info);
        }
        protected override bool Update(string id, Function info)
        {
            string filter = string.Format("ControlID='{0}' and SystemTypeID={1} and ID<>'{2}'", info.ControlID,info.SystemTypeID, info.ID);
            bool isExist = BLLFactory<FunctionBLL>.Instance.IsExistRecord(filter);
            if (isExist)
            {
                throw new ArgumentException("指定角色名称重复，请重新输入！");
            }
            return base.Update(id, info);
        }
        public void SetCommonInfo(Function info)
        {

        }
        public ActionResult GetRoleFunctionByUser(string userID)
        {
            List<EasyTreeData> treeList = new List<EasyTreeData>();
            bool isSuperAdmin = false;
            User userInfo = BLLFactory<UserBLL>.Instance.FindByID(userID);
            if (userInfo != null)
            {
                isSuperAdmin = BLLFactory<UserBLL>.Instance.UserInRole(userInfo.Name, Role.SuperAdminName);
            }

            List<SystemType> typeList = BLLFactory<SystemTypeBLL>.Instance.GetAll();
            foreach (SystemType typeInfo in typeList)
            {
                EasyTreeData parentNode = new EasyTreeData(typeInfo.ID, typeInfo.Name, "icon-organ");
                List<FunctionNode> allNode=new List<FunctionNode>();
                if (isSuperAdmin)
                {
                    allNode = BLLFactory<FunctionBLL>.Instance.GetTree(typeInfo.ID);
                }
                else
                {
                    allNode = BLLFactory<FunctionBLL>.Instance.GetFunctionNodesByUser(userID, typeInfo.ID);
                }
                AddFunctionNode(parentNode, allNode);
                treeList.Add(parentNode);
            }
            if (treeList.Count == 0)
            {
                treeList.Insert(0, new EasyTreeData(-1, "无"));
            }
            string json = ToJson(treeList);
            return Content(json);

        }
	}
}