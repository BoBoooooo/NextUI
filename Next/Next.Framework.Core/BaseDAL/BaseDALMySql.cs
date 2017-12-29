using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Framework.Core
{
    public abstract class BaseDALMySql<T>:AbstractBaseDAL<T>,IBaseDAL<T> where T:BaseEntity,new()
    {
        public BaseDALMySql()
        {

        }

        public BaseDALMySql(string tableName,string primaryKey):base(tableName,primaryKey)
        {
            this.parameterPrefix = "?";
            this.safeFieldFormat = "{0}";
        }
        public override DataTable SqlTable(string sql, DbTransaction trans = null)
        {
            return base.SqlTable(sql.ToUpper(), trans);
        }
        /// <summary>
        /// 获取前面记录指定数量的记录
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <param name="count">指定数量</param>
        /// <param name="orderBy">排序条件，例如order by id</param>
        /// <param name="trans">事务对象</param>
        /// <returns></returns>
        public override DataTable GetTopResult(string sql, int count, string orderBy, DbTransaction trans = null)
        {
            string resultSql = string.Format("select * from ({1} {2})  LIMIT {0} ", count, sql, orderBy);
            return SqlTable(resultSql, trans);
        }


    }
}
