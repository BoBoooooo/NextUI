using Next.Framework.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Framework.Core.Commons
{
    public class PagerHelper
    {
        private string tableName;
        private string fieldsToReturn = "*";
        private string fieldNameToSort = string.Empty;
        private int pageSize = 10;
        private int pageIndex = 1;
        private bool isDescending = false;
        private string strWhere = string.Empty;

        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }


        public string FieldsToReturn
        {
            get { return fieldsToReturn; }
            set { fieldsToReturn = value; }
        }
        public string FieldNameToSort
        {
            get { return fieldNameToSort; }
            set { fieldNameToSort = value; }
        }
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value; }
        }
        public int PageIndex
        {
            get { return pageIndex; }
            set { pageIndex = value; }
        }
        public bool IsDescending
        {
            get { return isDescending; }
            set { isDescending = value; }
        }
        public string StrWhere
        {
            get { return strWhere; }
            set { strWhere = value; }
        }
        internal string TableOrSqlWrapper
        {
            get
            {
                bool isSql = tableName.ToLower().Contains("from");
                if (isSql)
                {
                    return string.Format("({0}) AA", tableName);
                }
                else
                {
                    return tableName;
                }
            }
        }

        public PagerHelper()
        {

        }

        public PagerHelper(string tableName,string fieldsToReturn,string fieldNameToSort,int pageSize,int pageIndex,bool isDescending,string strWhere)
        {
            this.tableName=tableName;
            this.fieldsToReturn=fieldsToReturn;
            this.fieldNameToSort=fieldNameToSort;
            this.pageSize=pageSize;
            this.pageIndex=pageIndex;
            this.isDescending=isDescending;
            this.strWhere=strWhere;

        }
        private string GetOracleSql(bool isDoCount)
        {
            string sql = "";
            if (string.IsNullOrEmpty(this.strWhere))
            {
                this.strWhere = " (1=1) ";
            }
            if (isDoCount)
            {
                sql = string.Format("Select Count(*) as Total from {0} Where {1} ", this.TableOrSqlWrapper, this.strWhere);
            }
            else
            {
                string strOrder=string.Format(" order by {0} {1} ",this.fieldNameToSort,this.isDescending?"DESC":"ASC");

                int minRow=pageSize*(pageIndex-1);
                int maxRow=pageSize*pageIndex;
                string selectSql=string.Format("Select {0} from {1} Where {2} {3} ",fieldsToReturn,this.TableOrSqlWrapper,this.strWhere,strOrder);
                sql=string.Format(@"Select b.* from (Select a.*,rownum as rowIndex from ({2}) a) b where b.rowIndex >{0} and b.rowIndex <= {1} ",minRow,maxRow,selectSql);
            }
            return sql;
        }
        private string GetSqlServerSql(bool isDoCount)
        {
            string sql = "";
            if (string.IsNullOrEmpty(this.strWhere))
            {
                this.strWhere = " (1=1) ";
            }
            if (isDoCount)
            {
                sql = string.Format("Select Count(*) as Total from {0} Where {1} ", this.TableOrSqlWrapper, this.strWhere);
            }
            else
            {
                string strTemp=string.Empty;
                string strOrder=string.Empty;
                if(this.isDescending){
                    strTemp="<(select min";
                    strOrder=string.Format(" order by [{0}] desc",this.fieldNameToSort);
                }
                else{
                    strTemp=">(select max";
                    strOrder=string.Format(" order by [{0}] desc",this.fieldNameToSort);
                }
                sql = string.Format("select top {0} {1} from {2} ", this.pageSize, this.fieldsToReturn, this.TableOrSqlWrapper);
                if (this.pageSize == 1)
                {
                    sql+=string.Format(" Where {0} ",this.strWhere);
                    sql += strOrder;
                }
                else
                {
                    sql += string.Format(" Where [{0}] {1} ([{0}]) From (select top {2} [{0}] from {3} where {5} {4} ) as tblTmp) and {5} {4}", this.fieldNameToSort, strTemp, (this.pageIndex - 1) * this.pageSize, this.TableOrSqlWrapper, strOrder, this.strWhere);
                }
            }
            return sql;
        }
        private string GetAccessSql(bool isDoCount)
        {
            return GetSqlServerSql(isDoCount);
        }
        private string GetMySqlSql(bool isDoCount)
        {
            string sql = "";
            if (string.IsNullOrEmpty(this.strWhere))
            {
                this.strWhere = " (1=1) ";
            }
            if (isDoCount)
            {
                sql = string.Format("Select Count(*) as Total from {0} Where {1} ", this.TableOrSqlWrapper, this.strWhere);
            }
            else
            {
                string strOrder=string.Format(" order by {0} {1} ",this.fieldNameToSort,this.isDescending?"DESC":"ASC");

                int minRow=pageSize*(pageIndex-1);
                //int maxRow=pageSize*pageIndex;
                //int step=
                sql = string.Format("Select {0} from {1} Where {2} {3} LIMIT {4},{5} ", fieldsToReturn, this.TableOrSqlWrapper, this.strWhere, strOrder, minRow, pageSize);
            }
            return sql;
        }
        private string GetSQLiteSql(bool isDoCount)
        {
            string sql = "";
            if (string.IsNullOrEmpty(this.strWhere))
            {
                this.strWhere = " (1=1) ";
            }
            if (isDoCount)
            {
                sql = string.Format("Select Count(*) as Total from {0} Where {1} ", this.TableOrSqlWrapper, this.strWhere);
            }
            else
            {
                string strOrder = string.Format(" order by {0} {1} ", this.fieldNameToSort, this.isDescending ? "DESC" : "ASC");

                int minRow = pageSize * (pageIndex - 1);
                int maxRow = pageSize * pageIndex;

                sql = string.Format("Select {0} from {1} Where {2} {3} LIMIT {4},{5} ", fieldsToReturn, this.TableOrSqlWrapper, this.strWhere, minRow, maxRow);
            }
            return sql;
        }

        public string GetPagingSql(DatabaseType dbType,bool isDoCount)
        {
            string sql = "";
            switch (dbType)
            {
                case DatabaseType.Access:
                    sql = GetAccessSql(isDoCount);
                    break;
                case DatabaseType.SqlServer:
                    sql = GetSqlServerSql(isDoCount);
                    break;
                case DatabaseType.Oracle:
                    sql = GetOracleSql(isDoCount);
                    break;
                case DatabaseType.MySql:
                    sql = GetMySqlSql(isDoCount);
                    break;
                case DatabaseType.SQLite:
                    sql = GetSQLiteSql(isDoCount);
                    break;


            }
            return sql;
        }

        public string GetPagingSql(bool isDoCount)
        {
            AppConfig config = new AppConfig();
            string databaseType = config.AppConfigGet("ComponentDbType");
            DatabaseType dbType = GetDataBaseType(databaseType);
            return GetPagingSql(dbType, isDoCount);
        }
        private DatabaseType GetDataBaseType(string databaseType)
        {
            DatabaseType returnValue = DatabaseType.SqlServer;
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
    }
}
