using System;
using System.ComponentModel;
using Next.Framework.Core;
using System.Collections.Generic;
using Next.Admin.Entity;

namespace Next.WorkFlow.Entity
{
	[Serializable]
	public class WorkFlowTask: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("PrevID")]
		public string  PrevID { get; set; }
		[DisplayName("PrevStepID")]
		public string  PrevStepID { get; set; }
		[DisplayName("FlowID")]
		public string  FlowID { get; set; }
		[DisplayName("StepID")]
		public string  StepID { get; set; }
		[DisplayName("StepName")]
		public string  StepName { get; set; }
		[DisplayName("InstanceID")]
		public string  InstanceID { get; set; }
		[DisplayName("GroupID")]
		public string  GroupID { get; set; }
		[DisplayName("Type")]
		public int?  Type { get; set; }
		[DisplayName("Title")]
		public string  Title { get; set; }
		[DisplayName("SenderID")]
		public string  SenderID { get; set; }
		[DisplayName("SenderName")]
		public string  SenderName { get; set; }
		[DisplayName("SenderTime")]
		public DateTime?  SenderTime { get; set; }
		[DisplayName("ReceiveID")]
		public string  ReceiveID { get; set; }
		[DisplayName("ReveiveName")]
		public string  ReceiveName { get; set; }
		[DisplayName("ReceiveTime")]
		public DateTime  ReceiveTime { get; set; }
		[DisplayName("OpenTime")]
		public DateTime?  OpenTime { get; set; }
		[DisplayName("CompletedTime")]
		public DateTime?  CompletedTime { get; set; }
		[DisplayName("CompletedTime1")]
		public DateTime?  CompletedTime1 { get; set; }
		[DisplayName("Comment")]
		public string  Comment { get; set; }
		[DisplayName("IsSign")]
		public int?  IsSign { get; set; }
		[DisplayName("Status")]
		public int  Status { get; set; }
		[DisplayName("Note")]
		public string  Note { get; set; }
		[DisplayName("Sort")]
		public int?  Sort { get; set; }
		[DisplayName("SubFlowGroupID")]
		public string  SubFlowGroupID { get; set; }

	}
}

namespace Next.WorkFlow.Entity.WorkFlowExecute
{
    /// <summary>
    /// 任务相关的枚举类型
    /// </summary>
    public class EnumType
    {
        /// <summary>
        /// 处理类型
        /// </summary>
        public enum ExecuteType
        {
            /// <summary>
            /// 提交
            /// </summary>
            Submit,
            /// <summary>
            /// 保存
            /// </summary>
            Save,
            /// <summary>
            /// 退回
            /// </summary>
            Back,
            /// <summary>
            /// 完成
            /// </summary>
            Completed,
            /// <summary>
            /// 转交
            /// </summary>
            Redirect
        }
    }

    /// <summary>
    /// 任务处理模型
    /// </summary>
    [Serializable]
    public class Execute
    {
        public Execute()
        {
            Steps = new Dictionary<string, List<User>>();
        }
        /// <summary>
        /// 流程ID
        /// </summary>
        public string FlowID { get; set; }
        /// <summary>
        /// 步骤ID
        /// </summary>
        public string StepID { get; set; }
        /// <summary>
        /// 任务ID
        /// </summary>
        public string TaskID { get; set; }
        /// <summary>
        /// 实例ID
        /// </summary>
        public string InstanceID { get; set; }
        /// <summary>
        /// 分组ID
        /// </summary>
        public string GroupID { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public EnumType.ExecuteType ExecuteType { get; set; }
        /// <summary>
        /// 发送人员
        /// </summary>
        public Next.Admin.Entity.User Sender { get; set; }
        /// <summary>
        /// 接收的步骤和人员
        /// </summary>
        public Dictionary<string, List<Next.Admin.Entity.User>> Steps { get; set; }
        /// <summary>
        /// 处理意见
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// 是否签章
        /// </summary>
        public bool IsSign { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }
    }

    /// <summary>
    /// 任务处理结果
    /// </summary>
    [Serializable]
    public class Result
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string Messages { get; set; }
        /// <summary>
        /// 调试信息
        /// </summary>
        public string DebugMessages { get; set; }
        /// <summary>
        /// 其它信息
        /// </summary>
        public object[] Other { get; set; }
        /// <summary>
        /// 后续任务
        /// </summary>
        public IEnumerable<WorkFlowTask> NextTasks { get; set; }
    }

}
