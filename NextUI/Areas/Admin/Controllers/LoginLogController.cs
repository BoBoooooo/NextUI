using Next.Admin.BLL;
using Next.Admin.Entity;
using Next.Attachment.BLL;
using Next.Attachment.Entity;
using Next.Controllers;
using Next.Framework.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Next.Framework.Core.Commons;
using Next.Commons;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Next.Admin.Entity;
using Next.Admin.BLL;
using Next.Admin.Model;
namespace Next.Areas.Admin.Controllers
{
    public class LoginLogController : BusinessController<LoginLogBLL, LoginLog>
    {
        //
        // GET: /Jssjw/Loan/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetTreeJson()
        {
            List<EasyTreeData> treeList = new List<EasyTreeData>();

            //添加一个未分类和全部客户的组别
            EasyTreeData topNode = new EasyTreeData("-1", "所有记录", "icon-house");
            treeList.Add(topNode);

            EasyTreeData companyNode = new EasyTreeData("-2", "所属公司", "");
            treeList.Add(companyNode);

            List<Dept> companyList = new List<Dept>();
            if (BLLFactory<UserBLL>.Instance.UserInRole(CurrentUser.Name, Role.SuperAdminName))
            {
                companyList = BLLFactory<DeptBLL>.Instance.GetAllCompany();
            }
            else
            {
                Dept myCompanyInfo = BLLFactory<DeptBLL>.Instance.FindByID(CurrentUser.CompanyID);
                if (myCompanyInfo != null)
                {
                    companyList.Add(myCompanyInfo);
                }
            }

            string belongCompany = "-1,";
            foreach (Dept info in companyList)
            {
                belongCompany += string.Format("'{0}',", info.ID);

                //添加公司节点
                EasyTreeData subNode = new EasyTreeData(info.ID, info.Name, "icon-organ");
                companyNode.children.Add(subNode);

                /*//下面在添加系统类型节点
                List<SystemType> typeList = BLLFactory<SystemTypeBLL>.Instance.GetAll();
                foreach (SystemType typeInfo in typeList)
                {
                    EasyTreeData typeNode = new EasyTreeData(typeInfo.ID, typeInfo.Name, "icon-computer");
                    typeNode.id = string.Format("CompanyID='{0}' AND SystemTypeID='{1}' ", info.ID, typeInfo.ID);
                    subNode.children.Add(typeNode);
                }

                EasyTreeData securityNode = new EasyTreeData("Security", "权限管理系统", "icon-key");
                securityNode.id = string.Format("CompanyID='{0}' AND SystemTypeID='{1}' ", info.ID, "Security");
                subNode.children.Add(securityNode);*/
            }
            //修改全部为所属公司的ID
            belongCompany = belongCompany.Trim(',');
            topNode.id = string.Format("CompanyID in ({0})", belongCompany);

            string content = ToJson(treeList);
            return Content(content.Trim(','));
        }


    }
}