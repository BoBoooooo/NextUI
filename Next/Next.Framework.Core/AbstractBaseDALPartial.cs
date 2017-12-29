using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Reflection;
using System.Collections;
using System.Text.RegularExpressions;
using Next.Framework.Core.Commons;

namespace Next.Framework.Core
{
    abstract partial class AbstractBaseDAL<T> where T : BaseEntity, new()
    {

        /// <summary>
        /// 获取指定字符串中的子项的值
        /// </summary>
        /// <param name="connectionString">字符串值</param>
        /// <param name="subKeyName">以分号(;)为分隔符的子项名称</param>
        /// <returns>对应子项名称的值（即是=号后面的值）</returns>
        protected string GetSubValue(string connectionString, string subKeyName)
        {
            string[] item = connectionString.Split(new char[] { ';' });
            for (int i = 0; i < item.Length; i++)
            {
                string itemValue = item[i].ToLower();
                if (itemValue.IndexOf(subKeyName, StringComparison.OrdinalIgnoreCase) >= 0) //如果含有指定的关键字
                {
                    int startIndex = item[i].IndexOf("="); //等号开始的位置
                    return item[i].Substring(startIndex + 1).Trim(); //获取等号后面的值即为Value
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// 操根据条件返回DataTable记录辅助类
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="trans">事务对象</param>
        /// <returns></returns>
        protected DataTable GetDataTableBySql(string sql, DbTransaction trans = null)
        {
            Database db = CreateDatabase();
            DbCommand command = db.GetSqlStringCommand(sql);

            DataTable dt = null;
            if (trans != null)
            {
                dt = db.ExecuteDataSet(command, trans).Tables[0];
            }
            else
            {
                dt = db.ExecuteDataSet(command).Tables[0];
            }

            if (dt != null)
            {
                dt.TableName = "tableName";//增加一个表名称，防止WCF方式因为TableName为空出错
            }
            return dt;
        }
        /// <summary>
        /// 根据查询条件，返回记录到DataTable集合中
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <param name="trans">事务对象</param>
        /// <returns></returns>
        public virtual DataTable FindToDataTable(string condition, DbTransaction trans = null)
        {
            if (HasInjectionData(condition))
            {
                LogTextHelper.Error(string.Format("检测出SQL注入的恶意数据, {0}", condition));
                throw new Exception("检测出SQL注入的恶意数据");
            }

            //串连条件语句为一个完整的Sql语句
            string sql = string.Format("Select {0} From {1} ", selectedFields, tableName);
            if (!string.IsNullOrEmpty(condition))
            {
                sql += string.Format("Where {0}", condition);
            }
            sql += string.Format(" Order by {0} {1}", GetSafeFileName(sortField), IsDescending ? "DESC" : "ASC");

            return GetDataTableBySql(sql, trans);
        }

        /// <summary>
        /// 根据查询条件，返回记录到DataTable集合中
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <param name="info">分页条件</param>
        /// <param name="trans">事务对象</param>
        /// <returns></returns>
        public virtual DataTable FindToDataTable(string condition, PagerInfo info, DbTransaction trans = null)
        {
            return FindToDataTable(condition, info, this.sortField, this.IsDescending, trans);
        }

        /// <summary>
        /// 根据条件查询数据库,并返回DataTable集合(用于分页数据显示)
        /// </summary>
        /// <param name="condition">查询的条件</param>
        /// <param name="info">分页实体</param>
        /// <param name="fieldToSort">排序字段</param>
        /// <param name="desc">是否降序</param>
        /// <param name="trans">事务对象</param>
        /// <returns>指定DataTable的集合</returns>
        public virtual DataTable FindToDataTable(string condition, PagerInfo info, string fieldToSort, bool desc, DbTransaction trans = null)
        {
            if (HasInjectionData(condition))
            {
                LogTextHelper.Error(string.Format("检测出SQL注入的恶意数据, {0}", condition));
                throw new Exception("检测出SQL注入的恶意数据");
            }

            PagerHelper helper = new PagerHelper(tableName, this.selectedFields, fieldToSort,
                info.PageSize, info.CurrentPageIndex, desc, condition);

            string countSql = helper.GetPagingSql(true);
            string strCount = SqlValueList(countSql, trans);
            info.RecordCount = Convert.ToInt32(strCount);

            string dataSql = helper.GetPagingSql(false);
            return GetDataTableBySql(dataSql, trans);
        }

        /// <summary>
        /// 查找记录表中最旧的一条记录
        /// </summary>
        /// <param name="trans">事务对象</param>
        /// <returns></returns>
        public virtual T FindFirst(DbTransaction trans = null)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 查找记录表中最新的一条记录
        /// </summary>
        /// <param name="trans">事务对象</param>
        /// <returns></returns>
        public virtual T FindLast(DbTransaction trans = null)
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// 获取前面记录指定数量的记录
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <param name="count">指定数量</param>
        /// <param name="orderBy">排序条件，例如order by id</param>
        /// <param name="trans">事务对象</param>
        /// <returns></returns>
        public abstract DataTable GetTopResult(string sql, int count, string orderBy, DbTransaction trans = null);
        /// <summary>
        /// 添加记录
        /// </summary>
        /// <param name="recordField">Hashtable:键[key]为字段名;值[value]为字段对应的值</param>
        /// <param name="trans">事务对象,如果使用事务,传入事务对象,否则为Null不使用事务</param>
        public virtual int Insert2(Hashtable recordField, DbTransaction trans)
        {
            return this.Insert2(recordField, tableName, trans);
        }

        /// <summary>
        /// 添加记录
        /// </summary>
        /// <param name="recordField">Hashtable:键[key]为字段名;值[value]为字段对应的值</param>
        /// <param name="targetTable">需要操作的目标表名称</param>
        /// <param name="trans">事务对象,如果使用事务,传入事务对象,否则为Null不使用事务</param>
        public virtual int Insert2(Hashtable recordField, string targetTable, DbTransaction trans)
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// 测试数据库是否正常连接
        /// </summary>
        public virtual bool TestConnection(string connectionString)
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// 获取数据库的全部表名称
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetTableNames()
        {
            return new List<string>();
        }
    }
}
