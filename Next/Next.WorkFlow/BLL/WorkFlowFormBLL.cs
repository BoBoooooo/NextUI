using System;
using System.Collections.Generic;
using System.Text;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.DALMySql;
using Next.WorkFlow.IDAL;
using System.Web.UI.WebControls;


namespace Next.WorkFlow.BLL
{
	public class WorkFlowFormBLL : BaseBLL<WorkFlowForm>
	{
		private IWorkFlowFormDAL workFlowFormDAL;
		public WorkFlowFormBLL(): base()
		{
			base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
			baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//如果需要记录操作日志，则实现这个事件
			this.workFlowFormDAL = (IWorkFlowFormDAL)base.baseDal;
		}


        /// <summary>
        /// 得到流程值类型选择项字符串
        /// </summary>
        /// <returns></returns>
        public string GetValueTypeOptions(string value)
        {
            ListItem[] items = new ListItem[]{ 
                new ListItem("字符串","0"){ Selected="0"==value},
                new ListItem("整数","1"){ Selected="1"==value},
                new ListItem("实数","2"){ Selected="2"==value},
                new ListItem("正整数","3"){ Selected="3"==value},
                new ListItem("正实数","4"){ Selected="4"==value},
                new ListItem("负整数","5"){ Selected="5"==value},
                new ListItem("负实数","6"){ Selected="6"==value},
                new ListItem("手机号码","7"){ Selected="7"==value}
            };
            return RoadFlow.Utility.Tools.GetOptionsString(items);
        }
        /// <summary>
        /// 得到默认值下拉选项字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetDefaultValueSelect(string value)
        {
            StringBuilder options = new StringBuilder(1000);
            options.Append("<option value=\"\"></option>");
            options.Append("<optgroup label=\"组织机构相关选项\"></optgroup>");
            options.AppendFormat("<option value=\"u_@RoadFlow.Platform.Users.CurrentUserID.ToString()\" {0}>当前步骤用户ID</option>", "10" == value ? "selected=\"selected\"" : "");
            options.AppendFormat("<option value=\"@(RoadFlow.Platform.Users.CurrentUserName)\" {0}>当前步骤用户姓名</option>", "11" == value ? "selected=\"selected\"" : "");
            options.AppendFormat("<option value=\"@(RoadFlow.Platform.Users.CurrentDeptID)\" {0}>当前步骤用户部门ID</option>", "12" == value ? "selected=\"selected\"" : "");
            options.AppendFormat("<option value=\"@(RoadFlow.Platform.Users.CurrentDeptName)\" {0}>当前步骤用户部门名称</option>", "13" == value ? "selected=\"selected\"" : "");
            options.AppendFormat("<option value=\"u_@(new RoadFlow.Platform.WorkFlowTask().GetFirstSnderID(FlowID.ToGuid(), GroupID.ToGuid(), true))\" {0}>流程发起者ID</option>", "14" == value ? "selected=\"selected\"" : "");
            options.AppendFormat("<option value=\"@(new RoadFlow.Platform.Users().GetName(new RoadFlow.Platform.WorkFlowTask().GetFirstSnderID(FlowID.ToGuid(), GroupID.ToGuid(), true)))\" {0}>流程发起者姓名</option>", "15" == value ? "selected=\"selected\"" : "");
            options.AppendFormat("<option value=\"@(new RoadFlow.Platform.WorkFlowTask().GetFirstSnderDeptID(FlowID.ToGuid(), GroupID.ToGuid()))\" {0}>流程发起者部门ID</option>", "16" == value ? "selected=\"selected\"" : "");
            options.AppendFormat("<option value=\"@(new RoadFlow.Platform.Users().GetName(new RoadFlow.Platform.WorkFlowTask().GetFirstSnderDeptID(FlowID.ToGuid(), GroupID.ToGuid())))\" {0}>流程发起者部门名称</option>", "17" == value ? "selected=\"selected\"" : "");
            options.Append("<optgroup label=\"日期时间相关选项\"></optgroup>");
            options.AppendFormat("<option value=\"@(RoadFlow.Utility.DateTimeNew.ShortDate)\" {0}>短日期格式(2014-4-15)</option>", "20" == value ? "selected=\"selected\"" : "");
            options.AppendFormat("<option value=\"@(RoadFlow.Utility.DateTimeNew.LongDate)\" {0}>长日期格式(2014年4月15日)</option>", "21" == value ? "selected=\"selected\"" : "");
            options.AppendFormat("<option value=\"@(RoadFlow.Utility.DateTimeNew.ShortTime)\" {0}>短时间格式(23:59)</option>", "22" == value ? "selected=\"selected\"" : "");
            options.AppendFormat("<option value=\"@(RoadFlow.Utility.DateTimeNew.LongTime)\" {0}>长时间格式(23时59分)</option>", "23" == value ? "selected=\"selected\"" : "");
            options.AppendFormat("<option value=\"@(RoadFlow.Utility.DateTimeNew.ShortDateTime)\" {0}>短日期时间格式(2014-4-15 22:31)</option>", "24" == value ? "selected=\"selected\"" : "");
            options.AppendFormat("<option value=\"@(RoadFlow.Utility.DateTimeNew.LongDateTime)\" {0}>长日期时间格式(2014年4月15日 22时31分)</option>", "25" == value ? "selected=\"selected\"" : "");
            options.Append("<optgroup label=\"流程实例相关选项\"></optgroup>");
            options.AppendFormat("<option value=\"@Html.Raw(BWorkFlow.GetFlowName(FlowID.ToGuid()))\" {0}>当前流程名称</option>", "30" == value ? "selected=\"selected\"" : "");
            options.AppendFormat("<option value=\"@Html.Raw(BWorkFlow.GetStepName(StepID.ToGuid(), FlowID.ToGuid(), true))\" {0}>当前步骤名称</option>", "31" == value ? "selected=\"selected\"" : "");
            return options.ToString();
        }

        /// <summary>
        /// 得到默认值下拉选项字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetDefaultValueSelectByAspx(string value)
        {
            StringBuilder options = new StringBuilder(1000);
            options.Append("<option value=\"\"></option>");
            options.Append("<optgroup label=\"组织机构相关选项\"></optgroup>");
            options.AppendFormat("<option value=\"u_<%=RoadFlow.Platform.Users.CurrentUserID.ToString()%>\" {0}>当前步骤用户ID</option>", "10" == value ? "selected=\"selected\"" : "");
            options.AppendFormat("<option value=\"<%=RoadFlow.Platform.Users.CurrentUserName%>\" {0}>当前步骤用户姓名</option>", "11" == value ? "selected=\"selected\"" : "");
            options.AppendFormat("<option value=\"<%=RoadFlow.Platform.Users.CurrentDeptID%>\" {0}>当前步骤用户部门ID</option>", "12" == value ? "selected=\"selected\"" : "");
            options.AppendFormat("<option value=\"<%=RoadFlow.Platform.Users.CurrentDeptName%>\" {0}>当前步骤用户部门名称</option>", "13" == value ? "selected=\"selected\"" : "");
            options.AppendFormat("<option value=\"u_<%=new RoadFlow.Platform.WorkFlowTask().GetFirstSnderID(FlowID.ToGuid(), GroupID.ToGuid(), true)%>\" {0}>流程发起者ID</option>", "14" == value ? "selected=\"selected\"" : "");
            options.AppendFormat("<option value=\"<%=new RoadFlow.Platform.Users().GetName(new RoadFlow.Platform.WorkFlowTask().GetFirstSnderID(FlowID.ToGuid(), GroupID.ToGuid(), true))%>\" {0}>流程发起者姓名</option>", "15" == value ? "selected=\"selected\"" : "");
            options.AppendFormat("<option value=\"<%=new RoadFlow.Platform.WorkFlowTask().GetFirstSnderDeptID(FlowID.ToGuid(), GroupID.ToGuid())%>\" {0}>流程发起者部门ID</option>", "16" == value ? "selected=\"selected\"" : "");
            options.AppendFormat("<option value=\"<%=new RoadFlow.Platform.Users().GetName(new RoadFlow.Platform.WorkFlowTask().GetFirstSnderDeptID(FlowID.ToGuid(), GroupID.ToGuid()))%>\" {0}>流程发起者部门名称</option>", "17" == value ? "selected=\"selected\"" : "");
            options.Append("<optgroup label=\"日期时间相关选项\"></optgroup>");
            options.AppendFormat("<option value=\"<%=RoadFlow.Utility.DateTimeNew.ShortDate%>\" {0}>短日期格式(2014-4-15)</option>", "20" == value ? "selected=\"selected\"" : "");
            options.AppendFormat("<option value=\"<%=RoadFlow.Utility.DateTimeNew.LongDate%>\" {0}>长日期格式(2014年4月15日)</option>", "21" == value ? "selected=\"selected\"" : "");
            options.AppendFormat("<option value=\"<%=RoadFlow.Utility.DateTimeNew.ShortTime%>\" {0}>短时间格式(23:59)</option>", "22" == value ? "selected=\"selected\"" : "");
            options.AppendFormat("<option value=\"<%=RoadFlow.Utility.DateTimeNew.LongTime%>\" {0}>长时间格式(23时59分)</option>", "23" == value ? "selected=\"selected\"" : "");
            options.AppendFormat("<option value=\"<%=RoadFlow.Utility.DateTimeNew.ShortDateTime%>\" {0}>短日期时间格式(2014-4-15 22:31)</option>", "24" == value ? "selected=\"selected\"" : "");
            options.AppendFormat("<option value=\"<%=RoadFlow.Utility.DateTimeNew.LongDateTime%>\" {0}>长日期时间格式(2014年4月15日 22时31分)</option>", "25" == value ? "selected=\"selected\"" : "");
            options.Append("<optgroup label=\"流程实例相关选项\"></optgroup>");
            options.AppendFormat("<option value=\"<%=BWorkFlow.GetFlowName(FlowID.ToGuid())%>\" {0}>当前流程名称</option>", "30" == value ? "selected=\"selected\"" : "");
            options.AppendFormat("<option value=\"<%=BWorkFlow.GetStepName(StepID.ToGuid(), FlowID.ToGuid(), true)%>\" {0}>当前步骤名称</option>", "31" == value ? "selected=\"selected\"" : "");
            return options.ToString();
        }
	}
}
