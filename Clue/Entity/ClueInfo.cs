using System;
using System.ComponentModel;
using Next.Framework.Core;

namespace App.Clue.Entity
{
	[Serializable]
	public class ClueInfo: BaseEntity
	{
		[DisplayName("ID")]
		public string  ID { get; set; }
		[DisplayName("createUser")]

        public string DeptID { get; set; }
        [DisplayName("UserID")]
        public string UserID { get; set; }
        [DisplayName("AttachmentID")]
        public string AttachmentID { get; set; }
        [DisplayName("Dept")]
		public string  createUser { get; set; }
		[DisplayName("createDate")]
		public DateTime?  createDate { get; set; }
		[DisplayName("source")]
		public string  source { get; set; }
		[DisplayName("sync")]
		public string  sync { get; set; }
		[DisplayName("handleDept")]
		public string  handleDept { get; set; }
		[DisplayName("handleStaff")]
		public string  handleStaff { get; set; }
		[DisplayName("affix")]
		public string  affix { get; set; }
		[DisplayName("status")]
		public string  status { get; set; }
		[DisplayName("handleType")]
		public string  handleType { get; set; }
		[DisplayName("clueCode")]
		public string  clueCode { get; set; }
		[DisplayName("barCode")]
		public string  barCode { get; set; }
		[DisplayName("clueMode")]
		public string  clueMode { get; set; }
		[DisplayName("clueDate")]
		public string  clueDate { get; set; }
		[DisplayName("clueType")]
		public string  clueType { get; set; }
		[DisplayName("clueSource")]
		public string  clueSource { get; set; }
		[DisplayName("clueNum")]
		public string  clueNum { get; set; }
		[DisplayName("urgency")]
		public string  urgency { get; set; }
		[DisplayName("objType")]
		public string  objType { get; set; }
		[DisplayName("objName")]
		public string  objName { get; set; }
		[DisplayName("objAddress")]
		public string  objAddress { get; set; }
		[DisplayName("objAdminLvl")]
		public string  objAdminLvl { get; set; }
		[DisplayName("objComMode")]
		public string  objComMode { get; set; }
		[DisplayName("objArea")]
		public string  objArea { get; set; }
		[DisplayName("objPost")]
		public string  objPost { get; set; }
		[DisplayName("objControler")]
		public string  objControler { get; set; }
		[DisplayName("objIsManager")]
		public string  objIsManager { get; set; }
		[DisplayName("objIsLeader")]
		public string  objIsLeader { get; set; }
		[DisplayName("objCardNum")]
		public string  objCardNum { get; set; }
		[DisplayName("objSex")]
		public string  objSex { get; set; }
		[DisplayName("objBirthday")]
		public string  objBirthday { get; set; }
		[DisplayName("objNation")]
		public string  objNation { get; set; }
		[DisplayName("reflectIsSign")]
		public string  reflectIsSign { get; set; }
		[DisplayName("reflectName")]
		public string  reflectName { get; set; }
		[DisplayName("reflectAddress")]
		public string  reflectAddress { get; set; }
		[DisplayName("reflectAdminLvl")]
		public string  reflectAdminLvl { get; set; }
		[DisplayName("reflectLinkPhone")]
		public string  reflectLinkPhone { get; set; }
		[DisplayName("reflectArea")]
		public string  reflectArea { get; set; }
		[DisplayName("reflectPost")]
		public string  reflectPost { get; set; }
		[DisplayName("reflectEmail")]
		public string  reflectEmail { get; set; }
		[DisplayName("reflectZipCode")]
		public string  reflectZipCode { get; set; }
		[DisplayName("reflectTxaddress")]
		public string  reflectTxaddress { get; set; }
		[DisplayName("reflectNation")]
		public string  reflectNation { get; set; }
		[DisplayName("reflectSex")]
		public string  reflectSex { get; set; }
		[DisplayName("reflectPolitics")]
		public string  reflectPolitics { get; set; }
		[DisplayName("reflectVocation")]
		public string  reflectVocation { get; set; }
		[DisplayName("reflectCardNumr")]
		public string  reflectCardNumr { get; set; }
		[DisplayName("mainMode")]
		public string  mainMode { get; set; }
		[DisplayName("belongAddress")]
		public string  belongAddress { get; set; }
		[DisplayName("otherMode")]
		public string  otherMode { get; set; }
		[DisplayName("keyWord")]
		public string  keyWord { get; set; }
		[DisplayName("des")]
		public string  des { get; set; }
		[DisplayName("remark")]
		public string  remark { get; set; }
		[DisplayName("replyNote")]
		public string  replyNote { get; set; }
		[DisplayName("rollOut")]
		public string  rollOut { get; set; }
		[DisplayName("TimeStamp")]
		public DateTime?  TimeStamp { get; set; }
		[DisplayName("IsDeleted")]
		public bool  IsDeleted { get; set; }

	}
}
