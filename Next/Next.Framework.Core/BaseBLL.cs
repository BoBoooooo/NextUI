using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Next.Framework.Commons;
using System.Data.Common;
using Next.Framework.Core.Commons;
using System.Data;

namespace Next.Framework.Core
{
    public class BaseBLL<T> where T:BaseEntity, new()
    {
        private string dalName = "";

        protected string bllFullName;

        protected string dalAssemblyName;

        protected string bllPrefix = "BLL.";

        protected IBaseDAL<T> baseDal = null;

        public BaseBLL() { }

        protected void Init(string bllFullName, string dalAssemblyName = null, string bllPrefix="BLL.",string dbConfigName="mysql")
        {

            
            if (string.IsNullOrEmpty(bllFullName))
            {
                throw new ArgumentNullException("子类未设置bllFullName业务类全名!");
            }
            if (string.IsNullOrEmpty(dalAssemblyName))
            {
                dalAssemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            }
            this.bllFullName = bllFullName;
            this.dalAssemblyName = dalAssemblyName;
            this.bllPrefix = bllPrefix;

            AppConfig config = new AppConfig();
            string dbType = config.AppConfigGet("ComponentDbType");
            if (string.IsNullOrEmpty(dbType))
            {
                dbType = "mysql";
            }
            dbType = dbType.ToLower();
            string DALPrefix = "";
            if (dbType == "mysql")
            {
                DALPrefix = "DALMySql.";
            }
            if (dbType == "sqlserver")
            {
                DALPrefix = "DALSQL.";
            }
            this.dalName = bllFullName.Replace(bllPrefix, DALPrefix);
            this.dalName = this.dalName.Replace("BLL", "DAL");
            baseDal = Reflect<IBaseDAL<T>>.Create(this.dalName, dalAssemblyName);
            baseDal.SetDbConfigName(dbConfigName); //设置数据库配置项名称
        }

        protected void CheckDAL()
        {
            if (baseDal == null)
            {
                throw new ArgumentNullException("baseDal", "未能成功创建对应的DAL对象，请在BLL业务类构造函数中调用base.Init(**,**)方法，如base.Init(this.GetType().FullName,System.Reflection.Assembly.GetExecutingAssmbly().GetName().Name);");
            }
        }
        public virtual T FindByID(object key,DbTransaction trans=null )
        {
            CheckDAL();
            return baseDal.FindByID(key, trans);
        }

        public virtual T FindSingle(string condition, DbTransaction trans = null)
        {
            CheckDAL();
            return baseDal.FindSingle(condition, trans);
        }

        public virtual List<T> Find(string condition, DbTransaction trans = null)
        {
            CheckDAL();
            return baseDal.Find(condition,trans);
        }

        public virtual List<T> FindWithPager(string condition, PagerInfo info, DbTransaction trans = null)
        {
            CheckDAL();
            return baseDal.FindWithPager(condition, info, trans);
        }
        public virtual DataTable GetFieldTypeList()
        {
            CheckDAL();
            return baseDal.GetFieldTypeList();
        }

        public virtual List<T> GetAll(DbTransaction trans = null)
        {
            CheckDAL();
            return baseDal.GetAll(trans);
        }
        public virtual List<T> GetList(string sql)
        {
            CheckDAL();
            return baseDal.GetList(sql);
        }
        public virtual bool IsExistRecord(string condition, DbTransaction trans = null)
        {
            CheckDAL();
            return baseDal.IsExistRecord(condition, trans);
        }

        public virtual bool Insert(T obj, DbTransaction trans = null)
        {
            CheckDAL();
            return baseDal.Insert(obj, trans);
        }

        public virtual bool Delete(object key, DbTransaction trans = null)
        {
            CheckDAL();
            return baseDal.Delete(key, trans);
        }

        public virtual bool Update(T obj, object primaryKeyValue, DbTransaction trans = null)
        {
            CheckDAL();
            return baseDal.Update(obj, primaryKeyValue, trans);
        }

        public virtual bool Update(T obj, DbTransaction trans = null)
        {
            CheckDAL();
            string ID = baseDal.GetPrimaryKeyValue(obj);
            return baseDal.Update(obj, ID, trans);
        }

        public virtual DbTransaction CreateTransaction()
        {
            CheckDAL();
            return baseDal.CreateTransaction();
        }
    }
}
