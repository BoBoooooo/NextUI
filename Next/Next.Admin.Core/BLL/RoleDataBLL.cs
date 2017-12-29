using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Next.Framework.Core;
using Next.Admin.IDAL;
using Next.Admin.Entity;


namespace Next.Admin.BLL
{
    public class RoleDataBLL :BaseBLL<RoleData>
    {
        private IRoleDataDAL roleDataDAL;
        public RoleDataBLL()
            : base()
        {
            base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//如果需要记录操作日志，则实现这个事件
            this.roleDataDAL = (IRoleDataDAL)base.baseDal;
        }

        public bool UpdateRoleData(string roleID, string belongCompanys, string belongDepts)
        {
            bool result = false;
            RoleData info = FindByRoleID(roleID);
            if (info != null)
            {
                info.BelongCompanys = belongCompanys;
                info.BelongDepts = belongDepts;
                result = baseDal.Update(info, info.ID);
            }
            else
            {
                info = new RoleData();
                info.RoleID = roleID;
                info.BelongCompanys = belongCompanys;
                info.BelongDepts = belongDepts;
                result = baseDal.Insert(info);
            }
            return result;
        }

        public Dictionary<string, string> GetRoleDataDict(string roleID)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            RoleData roleDataInfo = FindByRoleID(roleID);
            if (roleDataInfo != null)
            {
                if (!string.IsNullOrEmpty(roleDataInfo.BelongCompanys))
                {
                    string[] companyArray = roleDataInfo.BelongCompanys.Split(',');
                    if (companyArray != null)
                    {
                        foreach (string company in companyArray)
                        {
                            string id = company;
                            if (!dict.ContainsKey(id))
                            {
                                dict.Add(id, id);
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(roleDataInfo.BelongDepts))
                {
                    string[] deptArray = roleDataInfo.BelongDepts.Split(',');
                    if (deptArray != null)
                    {
                        foreach (string dept in deptArray)
                        {
                            string id = dept;
                            if (!dict.ContainsKey(id))
                            {
                                dict.Add(id, id);
                            }
                        }
                    }
                }
                
            }
            return dict;
        }


        public RoleData FindByRoleID(string roleID)
        {
            string condition = string.Format("RoleID='{0}'", roleID);
            return baseDal.FindSingle(condition);
        }
    }
}
