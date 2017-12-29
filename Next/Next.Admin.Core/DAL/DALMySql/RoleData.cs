using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Next.Admin.IDAL;
using Next.Framework.Core;
using Next.Admin.Entity;
using System.Data;
using Next.Framework.Core.Commons;
using System.Collections;


namespace Next.Admin.DALMySql
{
    public class RoleDataDAL : BaseDALMySql<RoleData>,IRoleDataDAL
    {
        public static RoleDataDAL Instance
        {
            get
            {
                return new RoleDataDAL();
            }
        }
        public RoleDataDAL()
            : base("RoleData", "ID")
        {
            //this.sortField = "SortCode";
            this.IsDescending = false;
        }


    }
}
