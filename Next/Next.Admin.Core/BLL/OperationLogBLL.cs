using System;
using System.Collections.Generic;
using System.Text;
using Next.Framework.Core;
using Next.Admin.Entity;
using Next.Admin.DALMySql;
using Next.Admin.IDAL;
using System.Data.Common;
using System.Web;
using System.Diagnostics;
namespace Next.Admin.BLL
{
    public class OperationLogBLL : BaseBLL<OperationLog>
    {
        private IOperationLogDAL operationLogDAL;
        public OperationLogBLL()
            : base()
        {
            base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            this.operationLogDAL = (IOperationLogDAL)base.baseDal;
        }
        static bool flag = false;
        public static bool OnOperationLog(string userId, string tableName, string operationType, string note, DbTransaction trans = null)
        {
            if (flag == false)
            {
                //虽然实现了这个事件，但是我们还需要判断该表是否在配置表里面，如果不在，则不记录操作日志。
                flag = true;
                Debug.WriteLine("Hello World!");
                bool insert = operationType == "增加";
                bool update = operationType == "修改";
                bool delete = operationType == "删除";
                if (insert || update || delete)
                {
                    OperationLog info = new OperationLog();
                    info.TableName = tableName;
                    info.OperationType = operationType;
                    info.Note = note;
                    info.CreateTime = DateTime.Now;
                    userId = (string)HttpContext.Current.Session["UserID"];
                    if (!string.IsNullOrEmpty(userId))
                    {
                        User userInfo = BLLFactory<UserBLL>.Instance.FindByID(userId, trans);
                        if (userInfo != null)
                        {
                            info.UserID = userId;
                            info.LoginName = userInfo.Name;
                            info.FullName = userInfo.FullName;
                            info.CompanyID = userInfo.CompanyID;
                            info.CompanyName = userInfo.CompanyName;
                            info.MacAddress = (string)HttpContext.Current.Session["IP"]; ;//userInfo.CurrentMacAddress;
                            info.IPAddress = (string)HttpContext.Current.Session["MAC"]; ;//userInfo.CurrentLoginIP;
                        }
                    }
                    return BLLFactory<OperationLogBLL>.Instance.Insert(info, trans);
                }
            }
            else
            {
                flag = false;
            }
            return false;

        }
    }
}
