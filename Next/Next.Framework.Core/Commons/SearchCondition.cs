using Next.Framework.Commons;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Framework.Core.Commons
{
    public class SearchCondition
    {
        private Hashtable conditionTable = new Hashtable();
        public Hashtable ConditionTable
        {
            get { return this.conditionTable; }
        }

        public SearchCondition AddCondition(string fieldName, object fieldValue, SqlOperator sqlOperator)
        {
            this.conditionTable.Add(System.Guid.NewGuid(), new SearchInfo(fieldName, fieldValue, sqlOperator));
            return this;
        }
        public SearchCondition AddCondition(string fieldName, object fieldValue, SqlOperator sqlOperator, bool excludeIfEmpty)
        {

            this.conditionTable.Add(System.Guid.NewGuid(), new SearchInfo(fieldName, fieldValue, sqlOperator, excludeIfEmpty));
            return this;
        }
        public SearchCondition AddCondition(string fieldName, object fieldValue, SqlOperator sqlOperator, bool excludeIfEmpty, string groupName)
        {

            this.conditionTable.Add(System.Guid.NewGuid(), new SearchInfo(fieldName, fieldValue, sqlOperator, excludeIfEmpty, groupName));
            return this;
        }
        public string BuildConditionSql()
        {
            AppConfig config = new AppConfig();
            string databaseType = config.AppConfigGet("ComponentDbType");
            DatabaseType dbType = GetDataBaseType(databaseType);
            return BuildConditionSql(dbType);
        }
        private DatabaseType GetDataBaseType(string databaseType)
        {
            DatabaseType returnValue = DatabaseType.MySql;
            foreach (DatabaseType dbType in Enum.GetValues(typeof(DatabaseType)))
            {
                if (dbType.ToString().Equals(databaseType, StringComparison.OrdinalIgnoreCase))
                {
                    returnValue = dbType;
                    break;
                }
            }
            return returnValue;
        }

        public string BuildConditionSql(DatabaseType dbType)
        {
            string sql = " Where (1=1) ";
            string fieldName = string.Empty;
            SearchInfo searchInfo = null;

            StringBuilder sb = new StringBuilder();
            sql += BuildGroupCondition(dbType);

            foreach (DictionaryEntry de in this.conditionTable)
            {
                searchInfo = (SearchInfo)de.Value;

                if (searchInfo.ExcludeIfEmpty && (searchInfo.FieldValue == null || string.IsNullOrEmpty(searchInfo.FieldValue.ToString())))
                {
                    continue;
                }
                TypeCode typeCode = Type.GetTypeCode(searchInfo.FieldValue.GetType());
                if (string.IsNullOrEmpty(searchInfo.GroupName))
                {
                    if (searchInfo.SqlOperator == SqlOperator.Like)
                    {
                        sb.AppendFormat(" AND {0} {1} '{2}' ", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator), string.Format("%{0}%", searchInfo.FieldValue));
                    }
                    else if (searchInfo.SqlOperator == SqlOperator.NotLike)
                    {
                        sb.AppendFormat(" AND {0} {1} '{2}' ", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator), string.Format("%{0}%", searchInfo.FieldValue));
                    }
                    else if (searchInfo.SqlOperator == SqlOperator.LikeStartAt)
                    {
                        sb.AppendFormat(" AND {0} {1} '{2}' ", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator), string.Format("{0}%", searchInfo.FieldValue));
                    }
                    else if (searchInfo.SqlOperator == SqlOperator.In)
                    {
                        sb.AppendFormat(" AND {0} {1} '{2}' ", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator), string.Format("({0})", searchInfo.FieldValue));
                    }

                    else
                    {
                        if (dbType == DatabaseType.Oracle)
                        {
                            if (typeCode == TypeCode.DateTime)
                            {
                                DateTime dt = Convert.ToDateTime(searchInfo.FieldValue);
                                if (dt.Hour > 0 || dt.Minute > 0)
                                {
                                    sb.AppendFormat(" AND {0} {1} to_date('{2}','YYYY-MM-dd HH:mi')", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator), dt.ToString("yyyy-MM-dd HH:mm"));
                                }
                                else
                                {
                                    sb.AppendFormat(" AND {0} {1} to_date('{2}','YYYY-MM-dd')", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator), dt.ToString("yyyy-MM-dd"));
                                }
                            }
                            else if (!searchInfo.ExcludeIfEmpty)
                            {
                                if (searchInfo.SqlOperator == SqlOperator.Equal)
                                {
                                    sb.AppendFormat(" AND {0} is null or {0}='')", searchInfo.FieldName);
                                }
                                else if (searchInfo.SqlOperator == SqlOperator.NotEqual)
                                {
                                    sb.AppendFormat(" AND {0} is not null )", searchInfo.FieldName);
                                }
                            }
                            else
                            {
                                sb.AppendFormat(" AND {0} {1} '{2}'", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator), searchInfo.FieldValue);
                            }
                        }
                        else if (dbType == DatabaseType.Access)
                        {
                            if (searchInfo.SqlOperator == SqlOperator.Equal && typeCode == TypeCode.String && string.IsNullOrEmpty(searchInfo.FieldValue.ToString()))
                            {
                                sb.AppendFormat(" AND ({0} {1} '{2}' OR {0} IS NULL", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator), searchInfo.FieldValue);
                            }
                            else
                            {
                                if (typeCode == TypeCode.DateTime)
                                {
                                    sb.AppendFormat(" AND {0} {1} #{2}#", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator), searchInfo.FieldValue);
                                }
                                else if (typeCode == TypeCode.Byte || typeCode == TypeCode.Decimal || typeCode == TypeCode.Double ||
                                    typeCode == TypeCode.Int16 || typeCode == TypeCode.Int32 || typeCode == TypeCode.Int64 ||
                                    typeCode == TypeCode.SByte || typeCode == TypeCode.Single || typeCode == TypeCode.UInt16 ||
                                    typeCode == TypeCode.UInt32 || typeCode == TypeCode.UInt64)
                                {
                                    sb.AppendFormat(" AND {0} {1} {2}", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator), searchInfo.FieldValue);
                                }
                                else
                                {
                                    sb.AppendFormat(" AND {0} {1} '{2}'", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator), searchInfo.FieldValue);
                                }
                            }

                        }
                        else if (dbType == DatabaseType.SQLite)
                        {
                            if (typeCode == TypeCode.DateTime)
                            {
                                DateTime dt = Convert.ToDateTime(searchInfo.FieldValue);
                                sb.AppendFormat(" AND {0} {1} datetime('{2}')", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator), dt.ToString("yyyy-MM-dd HH:mm:ss"));

                            }
                            else
                            {
                                sb.AppendFormat(" AND {0} {1} '{2}' ", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator), searchInfo.FieldValue);
                            }
                        }
                        else
                        {

                            sb.AppendFormat(" AND {0} {1} '{2}' ", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator), searchInfo.FieldValue);

                        }
                    }
                }
            }
            sql += sb.ToString();

            return sql;
        }



        public string BuildGroupCondition(DatabaseType dbType)
        {
            Hashtable ht = GetGroupNames();
            SearchInfo searchInfo = null;
            StringBuilder sb = new StringBuilder();
            string sql = string.Empty;
            string tempSql = string.Empty;

            foreach (string groupName in ht.Keys)
            {
                sb = new StringBuilder();
                tempSql = " AND ({0}) ";
                foreach (DictionaryEntry de in this.conditionTable)
                {
                    searchInfo = (SearchInfo)de.Value;

                    if (searchInfo.ExcludeIfEmpty && (searchInfo.FieldValue == null || string.IsNullOrEmpty(searchInfo.FieldValue.ToString())))
                    {
                        continue;
                    }

                    TypeCode typeCode = Type.GetTypeCode(searchInfo.FieldValue.GetType());
                    if (groupName.Equals(searchInfo.GroupName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (searchInfo.SqlOperator == SqlOperator.Like)
                        {
                            sb.AppendFormat(" OR {0} {1} '{2}' ", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator), string.Format("%{0}%", searchInfo.FieldValue));
                        }
                        else if (searchInfo.SqlOperator == SqlOperator.NotLike)
                        {
                            sb.AppendFormat(" OR {0} {1} '{2}' ", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator), string.Format("%{0}%", searchInfo.FieldValue));
                        }
                        else if (searchInfo.SqlOperator == SqlOperator.LikeStartAt)
                        {
                            sb.AppendFormat(" OR {0} {1} '{2}' ", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator), string.Format("{0}%", searchInfo.FieldValue));
                        }
                    }
                    else
                    {
                        if (dbType == DatabaseType.Oracle)
                        {
                            if (typeCode == TypeCode.DateTime)
                            {
                                DateTime dt = Convert.ToDateTime(searchInfo.FieldValue);
                                if (dt.Hour > 0 || dt.Minute > 0)
                                {
                                    sb.AppendFormat(" OR {0} {1} to_date('{2}','YYYY-MM-dd HH:mi')", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator), dt.ToString("yyyy-MM-dd HH:mm"));
                                }
                                else
                                {
                                    sb.AppendFormat(" OR {0} {1} to_date('{2}','YYYY-MM-dd')", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator), dt.ToString("yyyy-MM-dd"));
                                }
                            }
                            else if (!searchInfo.ExcludeIfEmpty)
                            {
                                if (searchInfo.SqlOperator == SqlOperator.Equal)
                                {
                                    sb.AppendFormat(" OR {0} is null or {0}='')", searchInfo.FieldName);
                                }
                                else if (searchInfo.SqlOperator == SqlOperator.NotEqual)
                                {
                                    sb.AppendFormat(" OR {0} is not null )", searchInfo.FieldName);
                                }
                            }
                            else
                            {
                                sb.AppendFormat(" OR {0} {1} '{2}'", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator));
                            }
                        }
                        else if (dbType == DatabaseType.Access)
                        {
                            if (typeCode == TypeCode.DateTime)
                            {
                                sb.AppendFormat(" OR {0} {1} #{2}#", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator), searchInfo.FieldValue);
                            }
                            else if (typeCode == TypeCode.Byte || typeCode == TypeCode.Decimal || typeCode == TypeCode.Double ||
                                typeCode == TypeCode.Int16 || typeCode == TypeCode.Int32 || typeCode == TypeCode.Int64 ||
                                typeCode == TypeCode.SByte || typeCode == TypeCode.Single || typeCode == TypeCode.UInt16 ||
                                typeCode == TypeCode.UInt32 || typeCode == TypeCode.UInt64)
                            {
                                sb.AppendFormat(" OR {0} {1} {2}", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator), searchInfo.FieldValue);
                            }
                            else
                            {
                                sb.AppendFormat(" OR {0} {1} '{2}'", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator), searchInfo.FieldValue);
                            }
                        }
                        else if (dbType == DatabaseType.SQLite)
                        {
                            if (typeCode == TypeCode.DateTime)
                            {
                                DateTime dt = Convert.ToDateTime(searchInfo.FieldValue);
                                sb.AppendFormat(" OR {0} {1} datetime('{2}')", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator), dt.ToString("yyyy-MM-dd HH:mm:ss"));

                            }
                            else
                            {
                                sb.AppendFormat(" OR {0} {1} '{2}' ", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator), searchInfo.FieldValue);
                            }
                        }
                        else
                        {
                            if (searchInfo.SqlOperator == SqlOperator.Like)
                            {
                                sb.AppendFormat(" OR {0} {1} '{2}' ", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator), string.Format("%{0}%", searchInfo.FieldValue));
                            }
                            else
                            {
                                sb.AppendFormat(" OR {0} {1} '{2}' ", searchInfo.FieldName, this.ConvertSqlOperator(searchInfo.SqlOperator), searchInfo.FieldValue);
                            }
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                tempSql = string.Format(tempSql, sb.ToString().Substring(3));
                sql += tempSql;
            }
            return sql;
        }

        private Hashtable GetGroupNames()
        {
            Hashtable htGroupNames = new Hashtable();
            SearchInfo searchInfo = null;
            foreach (DictionaryEntry de in this.conditionTable)
            {
                searchInfo = (SearchInfo)de.Value;
                if (!string.IsNullOrEmpty(searchInfo.GroupName) && !htGroupNames.Contains(searchInfo.GroupName))
                {
                    htGroupNames.Add(searchInfo.GroupName, searchInfo.GroupName);
                }
            }
            return htGroupNames;
        }

        private string ConvertSqlOperator(SqlOperator sqlOperator)
        {
            string stringOperator = " = ";
            switch (sqlOperator)
            {
                case SqlOperator.Equal:
                    stringOperator = " = ";
                    break;
                case SqlOperator.LessThan:
                    stringOperator = " < ";
                    break;
                case SqlOperator.LessThanOrEqual:
                    stringOperator = " <= ";
                    break;
                case SqlOperator.Like:
                    stringOperator = " Like ";
                    break;
                case SqlOperator.NotLike:
                    stringOperator = " Not Like ";
                    break;
                case SqlOperator.LikeStartAt:
                    stringOperator = " Like ";
                    break;
                case SqlOperator.MoreThan:
                    stringOperator = " > ";
                    break;
                case SqlOperator.MoreThanOrEqual:
                    stringOperator = " >= ";
                    break;
                case SqlOperator.NotEqual:
                    stringOperator = " <> ";
                    break;
                case SqlOperator.In:
                    stringOperator = " in ";
                    break;
                default:
                    break;
            }
            return stringOperator;
        }
    }
}
