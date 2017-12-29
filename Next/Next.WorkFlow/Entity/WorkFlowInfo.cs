using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace Next.WorkFlow.Entity
{
	[Serializable]
	public class WorkFlowInfo: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("Name")]
		public string  Name { get; set; }
		[DisplayName("Type")]
		public string  Type { get; set; }
		[DisplayName("Manager")]
		public string  Manager { get; set; }
		[DisplayName("InstanceManager")]
		public string  InstanceManager { get; set; }
		[DisplayName("CreateDate")]
		public DateTime  CreateDate { get; set; }
		[DisplayName("CreateUserID")]
		public string  CreateUserID { get; set; }
		[DisplayName("DesignJSON")]
		public string  DesignJSON { get; set; }
		[DisplayName("InstallDate")]
		public DateTime?  InstallDate { get; set; }
		[DisplayName("InstallUserID")]
		public string  InstallUserID { get; set; }
		[DisplayName("RunJSON")]
		public string  RunJSON { get; set; }
		[DisplayName("Status")]
		public int  Status { get; set; }

	}
}
