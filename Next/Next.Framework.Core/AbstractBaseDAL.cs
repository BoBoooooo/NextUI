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
using System.Diagnostics;

namespace Next.Framework.Core
{

    public delegate bool OperationLogEventHandler(string userId, string tableName, string operationType, string note, DbTransaction trans = null);
    public abstract partial class AbstractBaseDAL<T> where T : BaseEntity, new()
    {
        protected string dbConfigName = "";
        protected string parameterPrefix = "@";
        protected string safeFieldFormat = "[{0}]";
        protected string tableName;
        protected string primaryKey;
        protected string sortField;

        protected string selectedFields = "*";
        public event OperationLogEventHandler OnOperationLog;//定义一个操作记录的事件处理
        public string SortField { get; set; }

        public bool IsDescending { get; set; }

        public AbstractBaseDAL()
        {
        }
        public AbstractBaseDAL(string tableName, string primaryKey)
            : this()
        {
            this.tableName = tableName;
            this.primaryKey = primaryKey;
            this.sortField = primaryKey;
            
        }
        protected virtual Database CreateDatabase()
        {
            Database db = null;
            if (string.IsNullOrEmpty(dbConfigName))
            {
                db = DatabaseFactory.CreateDatabase();
            }
            else
            {
                db = DatabaseFactory.CreateDatabase(dbConfigName);
            }
            return db;
        }
        public virtual List<T> GetList(string sql, IDbDataParameter[] paramList = null, DbTransaction trans = null)
        {
            T entity = null;
            List<T> list = new List<T>();
            Database db = CreateDatabase();
            DbCommand command = db.GetSqlStringCommand(sql);

            if (paramList != null)
            {
                command.Parameters.AddRange(paramList);
            }
            if (trans != null)
            {
                using (IDataReader dr = db.ExecuteReader(command, trans))
                {
                    while (dr.Read())
                    {
                        entity = DataReaderToEntity(dr);
                        list.Add(entity);
                    }
                }
            }
            else
            {
                using (IDataReader dr = db.ExecuteReader(command))
                {
                    while (dr.Read())
                    {
                        entity = DataReaderToEntity(dr);
                        list.Add(entity);
                    }
                }
            }
            return list;
        }



        public virtual int SqlExecute(string sql, DbTransaction trans = null)
        {
            Database db = CreateDatabase();
            DbCommand command = db.GetSqlStringCommand(sql);
            if (trans != null)
            {
                return db.ExecuteNonQuery(command, trans);
            }
            else
            {
                return db.ExecuteNonQuery(command);
            }
        }
        public virtual DataTable SqlTable(string sql, DbTransaction trans = null)
        {
            return SqlTable(sql, null, trans);
        }
        public virtual DataTable SqlTable(string sql, DbParameter[] parameters, DbTransaction trans = null)
        {
            Database db = CreateDatabase();
            DbCommand command = db.GetSqlStringCommand(sql);
            if (parameters != null)
            {
                foreach (DbParameter param in parameters)
                {
                    db.AddInParameter(command, param.ParameterName, param.DbType, param.Value);
                }
            }
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
                dt.TableName = "tableName";
            }
            return dt;
        }
        public virtual T FindByID(object key, DbTransaction trans = null)
        {
            return PrivateFindByID(key, trans);
        }

        private T PrivateFindByID(object key, DbTransaction trans = null)
        {
            string sql = string.Format("Select {0} From {1} Where ({2}={3}ID)",selectedFields, tableName, primaryKey, parameterPrefix);
            Database db = CreateDatabase();
            DbCommand command = db.GetSqlStringCommand(sql);
            db.AddInParameter(command, "ID", TypeToDbType(key.GetType()), key);
            T entity = GetEntity(db,command, trans);
            return entity;

        }
        /// <summary>
        /// 获取字段中文别名（用于界面显示）的字典集合
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, string> GetColumnNameAlias()
        {
            return new Dictionary<string, string>();
        }
        #region 用户操作记录的实现
        /// <summary>
        /// 插入操作的日志记录
        /// </summary>
        /// <param name="obj">数据对象</param>
        /// <param name="trans">事务对象</param>
        protected virtual void OperationLogOfInsert(T obj, DbTransaction trans = null)
        {
            if (OnOperationLog != null)
            {
                string operationType = "增加";
                string userId = obj.CurrentLoginUserId;

                Hashtable recordField = GetHashByEntity(obj);
                Dictionary<string, string> dictColumnNameAlias = GetColumnNameAlias();

                StringBuilder sb = new StringBuilder();
                foreach (string field in recordField.Keys)
                {
                    string columnAlias = field;
                    bool result = dictColumnNameAlias.TryGetValue(field, out columnAlias);
                    if (result && !string.IsNullOrEmpty(columnAlias))
                    {
                        columnAlias = string.Format("({0})", columnAlias);//增加一个括号显示
                    }

                    sb.AppendLine(string.Format("{0}{1}:{2}", field, columnAlias, recordField[field]));
                    sb.AppendLine();
                }
                sb.AppendLine();
                string note = sb.ToString();

                OnOperationLog(userId, this.tableName, operationType, note, trans);
            }
        }

        /// <summary>
        /// 修改操作的日志记录
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <param name="obj">数据对象</param>
        /// <param name="trans">事务对象</param>
        protected virtual void OperationLogOfUpdate(T obj, object id, DbTransaction trans = null)
        {
            if (OnOperationLog != null)
            {
                string operationType = "修改";
                string userId = obj.CurrentLoginUserId;

                Hashtable recordField = GetHashByEntity(obj);
                Dictionary<string, string> dictColumnNameAlias = GetColumnNameAlias();

                T objInDb = FindByID(id, trans);
                if (objInDb != null)
                {
                    Hashtable dbrecordField = GetHashByEntity(objInDb);//把数据库里的实体对象数据转换为哈希表

                    StringBuilder sb = new StringBuilder();
                    foreach (string field in recordField.Keys)
                    {
                        string newValue = recordField[field].ToString();
                        string oldValue = dbrecordField[field].ToString();
                        if (newValue != oldValue)//只记录变化的内容
                        {
                            string columnAlias = "";
                            bool result = dictColumnNameAlias.TryGetValue(field, out columnAlias);
                            if (result && !string.IsNullOrEmpty(columnAlias))
                            {
                                columnAlias = string.Format("({0})", columnAlias);//增加一个括号显示
                            }

                            sb.AppendLine(string.Format("{0}{1}:", field, columnAlias));
                            sb.AppendLine(string.Format("\t {0} -> {1}", dbrecordField[field], recordField[field]));
                            sb.AppendLine();
                        }
                    }
                    sb.AppendLine();
                    string note = sb.ToString();
                    Debug.WriteLine("OperationLogOfUpdate");
                    OnOperationLog(userId, this.tableName, operationType, note, trans);
                }
            }
        }

        /// <summary>
        /// 删除操作的日志记录
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <param name="userId">用户ID</param>
        /// <param name="trans">事务对象</param>
        protected virtual void OperationLogOfDelete(object id, string userId, DbTransaction trans = null)
        {
            if (OnOperationLog != null)
            {
                string operationType = "删除";

                Dictionary<string, string> dictColumnNameAlias = GetColumnNameAlias();

                T objInDb = FindByID(id, trans);
                if (objInDb != null)
                {
                    Hashtable dbrecordField = GetHashByEntity(objInDb);//把数据库里的实体对象数据转换为哈希表

                    StringBuilder sb = new StringBuilder();
                    foreach (string field in dbrecordField.Keys)
                    {
                        string columnAlias = "";
                        bool result = dictColumnNameAlias.TryGetValue(field, out columnAlias);
                        if (result && !string.IsNullOrEmpty(columnAlias))
                        {
                            columnAlias = string.Format("({0})", columnAlias);//增加一个括号显示
                        }

                        sb.AppendLine(string.Format("{0}{1}:", field, columnAlias));
                        sb.AppendLine(string.Format("\t {0}", dbrecordField[field]));
                        sb.AppendLine();
                    }
                    sb.AppendLine();
                    string note = sb.ToString();

                    OnOperationLog(userId, this.tableName, operationType, note, trans);
                }
            }
        }
        #endregion
        public virtual DbType TypeToDbType(Type t)
        {
            DbType dbt;
            try
            {
                if (t.Name.ToLower() == "byte[]")
                {
                    dbt = DbType.Binary;
                }
                else
                {
                    dbt = (DbType)Enum.Parse(typeof(DbType), t.Name);
                }
            }
            catch
            {
                dbt = DbType.String;
            }
            return dbt;
        }
        public virtual T FindSingle(string condition, DbTransaction trans = null)
        {
            return FindSingle(condition, null, null, trans);
        }
        public virtual T FindSingle(string condition, string orderBy, DbTransaction trans = null)
        {
            return FindSingle(condition, orderBy, null, trans);
        }
        public virtual T FindSingle(string condition, string orderBy, IDbDataParameter[] paramList, DbTransaction trans = null)
        {
            if (HasInjectionData(condition))
            {
                LogTextHelper.Error(string.Format("检测出SQL注入的恶意数据, {0}", condition));
                throw new Exception("检测出SQL注入的恶意数据");
            }
            if (HasInjectionData(orderBy))
            {
                LogTextHelper.Error(string.Format("检测出SQL注入的恶意数据, {0}", orderBy));
                throw new Exception("检测出SQL注入的恶意数据");
            }
            string sql = string.Format("Select {0} From {1} ", selectedFields, GetSafeFileName(tableName));
            if (!string.IsNullOrEmpty(condition))
            {
                sql += string.Format("Where {0}", condition);
            }
            if (!string.IsNullOrEmpty(orderBy))
            {
                sql += " " + orderBy;
            }
            else
            {
                sql += string.Format(" Order by {0} {1}", GetSafeFileName(sortField), IsDescending ? "DESC" : "ASC");
            }

            Database db = CreateDatabase();
            DbCommand command = db.GetSqlStringCommand(sql);
            if (paramList != null)
            {
                command.Parameters.AddRange(paramList);
            }
            T entity = GetEntity(db, command, trans);
            return entity;
        }

        protected T GetEntity(Database db, DbCommand command, DbTransaction trans = null)
        {
            T entity = null;
            if (trans != null)
            {
                using (IDataReader dr = db.ExecuteReader(command, trans))
                {
                    if (dr.Read())
                    {
                        entity = DataReaderToEntity(dr);
                    }
                }
            }
            else
            {
                using (IDataReader dr = db.ExecuteReader(command))
                {
                    if (dr.Read())
                    {
                        entity = DataReaderToEntity(dr);
                    }
                }
            }
            return entity;
        }
        protected virtual Hashtable GetHashByEntity(T obj)
        {
            Hashtable ht = new Hashtable();
            PropertyInfo[] pis = obj.GetType().GetProperties();
            for (int i = 0; i < pis.Length; i++)
            {
                {
                    object objValue = pis[i].GetValue(obj, null);
                    objValue = (objValue == null) ? DBNull.Value : objValue;
                    if (!ht.ContainsKey(pis[i].Name) && pis[i].Name != "CurrentLoginUserId")
                    {
                        ht.Add(pis[i].Name, objValue);
                        //EntityTypeHash.Add(pis[i].Name, pis[i].GetType());
                    }
                }
            }
            return ht;
        }
        private Hashtable EntityTypeHash = new Hashtable();
        protected virtual T DataReaderToEntity(IDataReader dr)
        {
            T obj = new T();
            PropertyInfo[] pis = obj.GetType().GetProperties();

            foreach (PropertyInfo pi in pis)
            {
                try
                {
                    if (pi.Name != "CurrentLoginUserId")
                    {
                        string columnName = pi.Name;
                        if (dr[columnName].ToString() != "")
                        {
                            string type = pi.PropertyType.Name.ToString();
                            if (type == "Boolean")
                            {
                                string str = dr[pi.Name].ToString();
                                try
                                {
                                    int i = Convert.ToInt32(str);
                                    pi.SetValue(obj, i > 0);
                                }
                                catch (Exception e)
                                {
                                    throw new Exception("布尔类型值赋值错误！" + e.ToString());
                                }

                            }
                            else
                            {
                                pi.SetValue(obj, dr[pi.Name] ?? "", null);
                            }

                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("读取数据到实体发生错误" + e.ToString());
                }
            }
            return obj;
        }
        protected string GetSafeFileName(string fieldName)
        {
            return string.Format(safeFieldFormat, fieldName);
        }

        public virtual List<T> Find(string condition, DbTransaction trans = null)
        {
            return Find(condition, null, null, trans);
        }

        public virtual List<T> Find(string condition, string orderBy, DbTransaction trans = null)
        {
            return Find(condition, orderBy, null, trans);
        }
        public virtual List<T> Find(string condition, string orderBy, IDbDataParameter[] paramList, DbTransaction trans = null)
        {
            if (HasInjectionData(condition))
            {
                LogTextHelper.Error(string.Format("检测出SQL注入的恶意数据, {0}", condition));
                throw new Exception("检测出SQL注入的恶意数据");
            }
            string sql = string.Format("Select {0} From {1}", selectedFields, tableName);
            if(!string.IsNullOrEmpty(condition)){
                sql+=string.Format(" Where {0} ",condition);
            }
            if(!string.IsNullOrEmpty(orderBy)){
                sql+=" "+orderBy;
            }
            else{
                sql+=string.Format(" Order by {0} {1}",GetSafeFileName(sortField),IsDescending?"DESC":"ASC");
            }
            List<T> list=GetList(sql,paramList,trans);
            return list;
        }
        public virtual List<T> FindWithPager(string condition, PagerInfo info, DbTransaction trans = null)
        {
            return FindWithPager(condition, info, this.sortField, this.IsDescending, trans);
        }
        public virtual List<T> FindWithPager(string condition, PagerInfo info, string fieldToSort, DbTransaction trans = null)
        {
            return FindWithPager(condition, info, fieldToSort, this.IsDescending, trans);
        }
        public virtual List<T> FindWithPager(string condition, PagerInfo info, string fieldToSort, bool desc, DbTransaction trans = null)
        {
            if (HasInjectionData(condition))
            {
                LogTextHelper.Error(string.Format("检测出SQL注入的恶意数据, {0}", condition));
                throw new Exception("检测出SQL注入的恶意数据");
            }
            PagerHelper helper = new PagerHelper(tableName, this.selectedFields, fieldToSort, info.PageSize, info.CurrentPageIndex, desc, condition);
            string countSql = helper.GetPagingSql(true);
            string strCount = SqlValueList(countSql);
            info.RecordCount = Convert.ToInt32(strCount);
            string dataSql = helper.GetPagingSql(false);
            List<T> list = GetList(dataSql, null, trans);
            return list;
        }

        public virtual string SqlValueList(string sql, DbTransaction trans = null)
        {
            StringBuilder result = new StringBuilder();
            Database db = CreateDatabase();
            DbCommand command = db.GetSqlStringCommand(sql);

            if (trans != null)
            {
                using (IDataReader dr = db.ExecuteReader(command, trans))
                {
                    if (dr.Read())
                    {
                        result.AppendFormat("{0},", dr[0].ToString());
                    }
                }
            }
            else
            {
                using (IDataReader dr = db.ExecuteReader(command))
                {
                    if (dr.Read())
                    {
                        result.AppendFormat("{0},", dr[0].ToString());
                    }
                }
            }
            string strResult = result.ToString().Trim(',');
            return strResult;
        }
        public virtual List<T> GetAll(DbTransaction trans = null)
        {
            return GetAll("", trans);
        }
        public virtual List<T> GetAll(string orderBy, DbTransaction trans = null)
        {
            if (HasInjectionData(orderBy))
            {
                LogTextHelper.Error(string.Format("检测出SQL注入的恶意数据, {0}", orderBy));
                throw new Exception("检测出SQL注入的恶意数据");
            }
            string sql = string.Format("Select {0} From {1} ", selectedFields, tableName);
            if (!string.IsNullOrEmpty(orderBy))
            {
                sql += orderBy;
            }
            else
            {
                sql+=string.Format(" Order by {0} {1} ",GetSafeFileName(sortField),IsDescending?"DESC":"ASC");
            }
            List<T> list=GetList(sql,null);
            return list;
        }

        public virtual bool IsExistRecord(string condition, DbTransaction trans = null)
        {
            if (HasInjectionData(condition))
            {
                LogTextHelper.Error(string.Format("检测出SQL注入的恶意数据, {0}", condition));
                throw new Exception("检测出SQL注入的恶意数据");
            }
            string sql = string.Format("Select Count(*) from {0} WHERE {1}", tableName, condition);
            Database db = CreateDatabase();
            DbCommand command = db.GetSqlStringCommand(sql);
            int result = GetExecuteScalarValue(db, command, trans);
            return result > 0;
        }

        public virtual bool Insert(T obj, DbTransaction trans = null)
        {

            PropertyInfo[] pis = obj.GetType().GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                var temp = pi.GetValue(obj);
                if (pi.Name.ToString() == "ID"&&pi.PropertyType.FullName=="System.String"&&pi.GetValue(obj)==null)
                {
                    pi.SetValue(obj, Guid.NewGuid().ToString());
                }
            }
            ArgumentValidation.CheckForNullReference(obj, "传入的对象obj为空");
            OperationLogOfInsert(obj, trans);
            Hashtable hash = GetHashByEntity(obj);
            return Insert(hash, trans);
        }

        public virtual string GetPrimaryKeyValue(T obj, DbTransaction trans = null)
        {
            string ID = null; ;
            PropertyInfo[] pis = obj.GetType().GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                var temp = pi.GetValue(obj);
                if (pi.Name.ToString() == "ID")
                {
                    ID=(string)(pi.GetValue(obj));
                }
            }
            return ID;
        }
        public virtual bool Insert(Hashtable recordField, DbTransaction trans)
        {
            return this.Insert(recordField, tableName, trans);
        }
        public virtual bool Insert(Hashtable recordField, string targetTable, DbTransaction trans)
        {
            bool result = false;
            if (recordField == null || recordField.Count < 1)
            {
                return result;
            }

            string fields = "";
            string vals = "";
            foreach (string field in recordField.Keys)
            {
                fields += string.Format("{0},", GetSafeFileName(field));
                vals += string.Format("{0}{1},", parameterPrefix, field);
            }
            fields = fields.Trim(',');
            vals = vals.Trim(',');
            string sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", targetTable, fields, vals);

            Database db = CreateDatabase();
            DbCommand command = db.GetSqlStringCommand(sql);

            foreach (string field in recordField.Keys)
            {
                object val = recordField[field];
                val = val ?? DBNull.Value;
                if (val is DateTime)
                {
                    if(Convert.ToDateTime(val)<=Convert.ToDateTime("1753-1-1")){
                        val=DBNull.Value;
                    }
                }
                db.AddInParameter(command, field, TypeToDbType(val.GetType()), val);
            }
            if (trans != null)
            {
                result = db.ExecuteNonQuery(command, trans) > 0;
            }
            else
            {
                result = db.ExecuteNonQuery(command) > 0;
            }
            return result;
        }
        public virtual bool Update(T obj, object primaryKeyValue, DbTransaction trans = null)
        {
            ArgumentValidation.CheckForNullReference(obj, "传入的对象obj为空");
            Debug.WriteLine("Update(T obj, object primaryKeyValue, DbTransaction trans = null)");
            OperationLogOfUpdate(obj, primaryKeyValue, trans);
            Hashtable hash = GetHashByEntity(obj);
            return Update(primaryKeyValue, hash, trans);
        }

        public virtual bool Update(object id, Hashtable recordField, DbTransaction trans)
        {
            
            return this.PrivateUpdate(id, recordField, tableName, trans);
        }
        public virtual bool PrivateUpdate(object id, Hashtable recordField, string targetTable, DbTransaction trans)
        {
            try
            {
                if (recordField == null || recordField.Count < 1)
                {
                    return false;
                }
                string setValue = "";
                foreach (string field in recordField.Keys)
                {
                    setValue += string.Format("{0}={1}{2},", GetSafeFileName(field), parameterPrefix, field);
                }
                string sql = string.Format("UPDATE {0} SET {1} WHERE {2}={3}{2}", targetTable, setValue.Substring(0, setValue.Length - 1), primaryKey, parameterPrefix);
                Database db = CreateDatabase();
                DbCommand command = db.GetSqlStringCommand(sql);
                bool foundID = false;
                foreach (string field in recordField.Keys)
                {
                    object val = recordField[field];
                    val = val ?? DBNull.Value;
                    if (val is DateTime)
                    {
                        if (Convert.ToDateTime(val) <= Convert.ToDateTime("1753-1-1"))
                        {
                            val = DBNull.Value;
                        }
                        db.AddInParameter(command, field, DbType.DateTime, val);
                    }
                    else
                    {
                        db.AddInParameter(command, field, TypeToDbType(val.GetType()), val);
                    }
                    if (field.Equals(primaryKey, StringComparison.OrdinalIgnoreCase))
                    {
                        foundID = true;
                    }
                    
                }
                if (!foundID)
                {
                    db.AddInParameter(command, primaryKey, TypeToDbType(id.GetType()), id);
                }
                bool result = false;
                if (trans != null)
                {
                    result = db.ExecuteNonQuery(command, trans) > 0;
                }
                else
                {
                    result = db.ExecuteNonQuery(command) > 0;
                }
                return result;
            }
            catch (Exception ex)
            {
                LogTextHelper.WriteLine(ex.ToString());
                throw;
            }
        }

        public virtual bool Delete(object key, DbTransaction trans = null)
        {
            OperationLogOfDelete(key, null, trans);

            string condition = string.Format("{0}={1}{0}", primaryKey, parameterPrefix);
            string sql = string.Format("DELETE FROM {0} WHERE {1} ", tableName, condition);
            Database db = CreateDatabase();
            DbCommand command = db.GetSqlStringCommand(sql);

            db.AddInParameter(command, primaryKey, TypeToDbType(key.GetType()), key);
            bool result = false;
            if (trans != null)
            {
                result = db.ExecuteNonQuery(command, trans) > 0;
            }
            else
            {
                result = db.ExecuteNonQuery(command) > 0;
            }
            return result;
            
        }
        		/// <summary>
        /// 根据指定条件,从数据库中删除指定对象
        /// </summary>
        /// <param name="condition">删除记录的条件语句</param>
        /// <param name="trans">事务对象</param>
        /// <param name="paramList">Sql参数列表</param>
        /// <param name="trans">事务对象</param>
        /// <returns>执行成功返回<c>true</c>，否则为<c>false</c>。</returns>
        public virtual bool DeleteByCondition(string condition, DbTransaction trans=null, IDbDataParameter[] paramList=null)
        {
            if (HasInjectionData(condition))
            {
                LogTextHelper.Error(string.Format("检测出SQL注入的恶意数据, {0}", condition));
                throw new Exception("检测出SQL注入的恶意数据");
            }

            string sql = string.Format("DELETE FROM {0} WHERE {1} ", tableName, condition);

            Database db = CreateDatabase();
            DbCommand command = db.GetSqlStringCommand(sql);
            if(paramList != null)
            {
				command.Parameters.AddRange(paramList);
			}

            bool result = false;
            if (trans != null)
            {
                result = db.ExecuteNonQuery(command, trans) > 0;
            }
            else
            {
                result = db.ExecuteNonQuery(command) > 0;
            }

            return result;
        }
               		
        public virtual List<T> GetAll(PagerInfo info, DbTransaction trans = null)
        {
            return FindWithPager("", info, this.sortField, this.IsDescending, trans);
        }
        public virtual List<T> GetAll(PagerInfo info, string fieldToSort, bool desc, DbTransaction trans = null)
        {
            return FindWithPager("", info, fieldToSort,desc, trans);
        }
        public virtual DataTable GetFieldTypeList()
        {
            DataTable dt = DataTableHelper.CreateTable("ColumnName,DataType");
            DataTable schemaTable = GetReaderSchema(tableName);
            if (schemaTable != null)
            {
                foreach (DataRow dr in schemaTable.Rows)
                {
                    string columnName = dr["ColumnName"].ToString().ToUpper();
                    string netType = dr["DataType"].ToString().ToLower();
                    DataRow row = dt.NewRow();
                    row["ColumnName"] = columnName;
                    row["DataType"] = netType;
                    dt.Rows.Add(row);
                }
            }
            if (dt != null)
            {
                dt.TableName = "tableName";
            }
            return dt;
        }

        private DataTable GetReaderSchema(string tableName)
        {
            DataTable schemaTable = null;
            string sql = string.Format("Select * From {0} ", tableName);
            Database db = CreateDatabase();
            DbCommand command = db.GetSqlStringCommand(sql);
            try
            {
                using (IDataReader reader = db.ExecuteReader(command))
                {
                    schemaTable = reader.GetSchemaTable();
                }
            }
            catch (Exception ex)
            {
                LogTextHelper.Error(ex);
            }
            return schemaTable;
        }

        public virtual bool HasInjectionData(string inputData)
        {
            if (string.IsNullOrEmpty(inputData))
                return false;

            if (Regex.IsMatch(inputData.ToLower(), GetRegexString()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected int GetExecuteScalarValue(Database db, DbCommand command, DbTransaction trans = null)
        {
            int result = 0;
            object objResult = null;
            if (trans != null)
            {
                objResult = db.ExecuteScalar(command, trans);
            }
            else
            {
                objResult = db.ExecuteScalar(command);
            }

            if (objResult != null && objResult != DBNull.Value)
            {
                result = Convert.ToInt32(objResult);
            }
            return result;
        }

        public virtual DbTransaction CreateTransaction()
        {
            Database db = CreateDatabase();
            DbConnection connection = db.CreateConnection();
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }
            return connection.BeginTransaction();
        }
        private string GetRegexString()
        {
            string[] strBadChar ={
            "insert\\s",
            "delete\\s",
            "update\\s",
            "drop\\s",
            "truncate\\s",
            "exec\\s",
            "count\\s",
            "declare\\s",
            "asc\\(",
            "mid\\(",
            "char\\(",
            "net user",
            "xp_cmdshell",
            "/add\\s",
            "exec master.dbo.xp_cmdshell",
            "net localgroup administrators"
                                };
            string str_Regex = ".*(";
            for (int i = 0; i < strBadChar.Length - 1; i++)
            {
                str_Regex += strBadChar[i] + "|";
            }
            str_Regex += strBadChar[strBadChar.Length - 1] + ").*";
            return str_Regex;
        }


    }
}
