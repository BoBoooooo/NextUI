using Next.Framework.Core.Commons;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Framework.Core
{
    public interface IBaseDAL<T> where T:BaseEntity
    {
        event OperationLogEventHandler OnOperationLog;
        T FindSingle(string condition, DbTransaction trans = null);

        T FindSingle(string condition, string orderBy, DbTransaction trans = null);

        T FindSingle(string condition, string orderBy, IDbDataParameter[] paramList, DbTransaction trans = null);

        T FindByID(object key, DbTransaction trans = null);

        List<T> Find(string condition, DbTransaction trans = null);

        List<T> FindWithPager(string condition, PagerInfo info, DbTransaction trans = null);

        DataTable GetFieldTypeList();

        List<T> GetAll(DbTransaction trans = null);
        List<T> GetAll(string orderBy,DbTransaction trans = null);
        List<T> GetAll(PagerInfo info,DbTransaction trans = null);
        List<T> GetAll(PagerInfo info,string fieldToSort,bool desc,DbTransaction trans = null);

        bool IsExistRecord(string condition, DbTransaction trans = null);

        bool Insert(T obj, DbTransaction trans = null);

        bool Delete(object key, DbTransaction trans = null);
        /// <summary>
        /// 根据指定条件,从数据库中删除指定对象
        /// </summary>
        /// <param name="condition">删除记录的条件语句</param>
        /// <param name="trans">事务对象</param>
        /// <param name="paramList">Sql参数列表</param>
        /// <returns>执行成功返回<c>true</c>，否则为<c>false</c>。</returns>
        bool DeleteByCondition(string condition, DbTransaction trans = null, IDbDataParameter[] paramList = null);
        bool Update(T obj, object primaryKeyValue, DbTransaction trans = null);

        List<T> GetList(string sql, IDbDataParameter[] paramList = null, DbTransaction trans = null);

        DbTransaction CreateTransaction();
     
    }
}
