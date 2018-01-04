using System;
using System.Collections.Generic;
using System.Text;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.DALMySql;
using Next.WorkFlow.IDAL;
using Next.WorkFlow.Utility;

namespace Next.WorkFlow.BLL
{
	public class WorkFlowTaskBLL : BaseBLL<WorkFlowTask>
	{
		private IWorkFlowTaskDAL workFlowTaskDAL;
		public WorkFlowTaskBLL(): base()
		{
			base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
			baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//如果需要记录操作日志，则实现这个事件
			this.workFlowTaskDAL = (IWorkFlowTaskDAL)base.baseDal;
		}

        /// <summary>
        /// 执行自定义方法
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public object ExecuteFlowCustomEvent(string eventName, object eventParams, string dllName = "")
        {
            if (dllName.IsNullOrEmpty())
            {
                dllName = eventName.Substring(0, eventName.IndexOf('.'));
            }
            System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(dllName);
            string typeName = System.IO.Path.GetFileNameWithoutExtension(eventName);
            string methodName = eventName.Substring(typeName.Length + 1);
            Type type = assembly.GetType(typeName, true);

            object obj = System.Activator.CreateInstance(type, false);
            var method = type.GetMethod(methodName);

            if (method != null)
            {
                return method.Invoke(obj, new object[] { eventParams });
            }
            else
            {
                throw new MissingMethodException(typeName, methodName);
            }
        }
	}
}
