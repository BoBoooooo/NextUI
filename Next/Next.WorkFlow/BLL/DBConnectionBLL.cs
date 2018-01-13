using System;
using System.Collections.Generic;
using System.Text;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.DALMySql;
using Next.WorkFlow.IDAL;
using MySql.Data.MySqlClient;
using System.Linq;
using Next.WorkFlow.Utility;
using System.Data;
namespace Next.WorkFlow.BLL
{
	public class DBConnectionBLL : BaseBLL<DBConnection>
	{
		private IDBConnectionDAL dBConnectionDAL;
		public DBConnectionBLL(): base()
		{
			base.Init(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
			baseDal.OnOperationLog += new OperationLogEventHandler(Next.Admin.BLL.OperationLogBLL.OnOperationLog);//如果需要记录操作日志，则实现这个事件
			this.dBConnectionDAL = (IDBConnectionDAL)base.baseDal;
		}
        public List<string> GetTables(string id)
        {
            var allConns = GetAll();
            var conn = allConns.Find(p => p.ID == id);
            if (conn == null) return new List<string>();
            List<string> tables = new List<string>();
            switch (conn.Type)
            {
                case "MySql":
                    tables = getTables_MySql(conn);
                    break;
            }
            return tables;
        }

        /// <summary>
        /// 得到一个连接所有表
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        private List<string> getTables_MySql(DBConnection conn)
        {
            using (MySqlConnection sqlConn = new MySqlConnection(conn.ConnectionString))
            {
                try
                {
                    sqlConn.Open();
                }
                catch (MySqlException err)
                {
                    //Log.Add(err);
                    return new List<string>();
                }
                List<string> tables = new List<string>();
                string sql = string.Format("show full tables from {0} where table_type!='VIEW'",conn.Name);//"SELECT name FROM sysobjects WHERE xtype='U' ORDER BY name";
                using (MySqlCommand sqlCmd = new MySqlCommand(sql, sqlConn))
                {
                    MySqlDataReader dr = sqlCmd.ExecuteReader();
                    while (dr.Read())
                    {
                        tables.Add(dr.GetString(0));
                    }
                    dr.Close();
                    return tables;
                }
            }
        }
        /// <summary>
        /// 得到所有字段
        /// </summary>
        /// <param name="id">连接ID</param>
        /// <param name="table">表名</param>
        /// <returns></returns>
        public Dictionary<string, string> GetFields(string id, string table)
        {
            if (string.IsNullOrEmpty(table)) return new Dictionary<string, string>();
            var allConns = GetAll();
            var conn = allConns.Find(p => p.ID == id);
            if (conn == null) return new Dictionary<string, string>();
            Dictionary<string, string> fields = new Dictionary<string, string>();
            switch (conn.Type)
            {
                case "MySql":
                    fields = getFields_MySql(conn, table);
                    break;
            }
            return fields;
        }
        /// <summary>
        /// 得到一个连接一个表所有字段
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        private Dictionary<string, string> getFields_MySql(DBConnection conn, string table)
        {
            using (MySqlConnection sqlConn = new MySqlConnection(conn.ConnectionString))
            {
                try
                {
                    sqlConn.Open();
                }
                catch (MySqlException err)
                {
                    //Log.Add(err);
                    return new Dictionary<string, string>();
                }
                Dictionary<string, string> fields = new Dictionary<string, string>();
                string sql = string.Format(@"show full fields from {0}", table);
                using (MySqlCommand sqlCmd = new MySqlCommand(sql, sqlConn))
                {
                    MySqlDataReader dr = sqlCmd.ExecuteReader();
                    while (dr.Read())
                    {
                        fields.Add(dr.GetString(0), dr.IsDBNull(1) ? "" : dr.GetString(1));
                    }
                    dr.Close();
                    return fields;
                }
            }
        }
        /// <summary>
        /// 测试一个sql是否合法
        /// </summary>
        /// <param name="dbconn"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public bool TestSql(DBConnection dbconn, string sql)
        {
            if (dbconn == null)
            {
                return false;
            }
            switch (dbconn.Type)
            {
                case "MySql":
                    using (MySqlConnection conn = new MySqlConnection(dbconn.ConnectionString))
                    {
                        try
                        {
                            conn.Open();
                        }
                        catch
                        {
                            return false;
                        }
                        using (MySqlCommand cmd = new MySqlCommand(sql/*.ReplaceSelectSql()*/, conn))
                        {
                            try
                            {
                                cmd.ExecuteNonQuery();
                                return true;
                            }
                            catch
                            {
                                return false;
                            }
                        }
                    }
            }
            return false;
        }
        /// <summary>
        /// 得到所有数据连接的下拉选择
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetAllOptions(string value = "")
        {
            var conns = GetAll();
            StringBuilder options = new StringBuilder();
            foreach (var conn in conns.OrderBy(p => p.Name))
            {
                options.AppendFormat("<option value=\"{0}\" {1}>{2}</option>", conn.ID,
                    string.Compare(conn.ID.ToString(), value, true) == 0 ? "selected=\"selected\"" : "", conn.Name);
            }
            return options.ToString();
        }

        /// <summary>
        /// 根据连接实体得到连接
        /// </summary>
        /// <param name="linkID"></param>
        /// <returns></returns>
        public System.Data.IDbConnection GetConnection(DBConnection dbconn)
        {
            if (dbconn == null || dbconn.Type.IsNullOrEmpty() || dbconn.ConnectionString.IsNullOrEmpty())
            {
                return null;
            }
            System.Data.IDbConnection conn = null;
            switch (dbconn.Type)
            {
                case "MySql":
                    conn = new MySqlConnection(dbconn.ConnectionString);
                    break;

            }

            return conn;

        }

        /// <summary>
        /// 根据连接实体得到数据适配器
        /// </summary>
        /// <param name="linkID"></param>
        /// <returns></returns>
        public System.Data.IDbDataAdapter GetDataAdapter(IDbConnection conn, string connType, string cmdText, IDataParameter[] parArray)
        {
            IDbDataAdapter dataAdapter = null;
            switch (connType)
            {
                case "MySql":
                    using (MySqlCommand cmd = new MySqlCommand(cmdText, (MySqlConnection)conn))
                    {
                        if (parArray != null && parArray.Length > 0)
                        {
                            cmd.Parameters.AddRange(parArray);
                        }
                        dataAdapter = new MySqlDataAdapter(cmd);
                    }
                    break;
            }
            return dataAdapter;
        }

        /// <summary>
        /// 根据连接实体得到数据表
        /// </summary>
        /// <param name="linkID"></param>
        /// <returns></returns>
        public DataTable GetDataTable(DBConnection dbconn, string sql, IDataParameter[] parameterArray = null)
        {
            if (dbconn == null || dbconn.Type.IsNullOrEmpty() || dbconn.ConnectionString.IsNullOrEmpty())
            {
                return null;
            }
            DataTable dt = new DataTable();
            switch (dbconn.Type)
            {
                case "MySql":
                    using (MySqlConnection conn = new MySqlConnection(dbconn.ConnectionString))
                    {
                        try
                        {
                            conn.Open();
                            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                            {
                                if (parameterArray != null && parameterArray.Length > 0)
                                {
                                    cmd.Parameters.AddRange((MySqlParameter[])parameterArray);
                                }
                                using (MySqlDataAdapter dap = new MySqlDataAdapter(cmd))
                                {
                                    dap.Fill(dt);
                                }
                            }
                        }
                        catch (MySqlException ex)
                        {
                            //Platform.Log.Add(ex);
                        }
                    }
                    break;


            }

            return dt;
        }

        /// <summary>
        /// 删除一个连接表的数据
        /// </summary>
        /// <param name="connID"></param>
        /// <param name="table"></param>
        /// <param name="pkFiled"></param>
        /// <param name="pkValue"></param>
        public int DeleteData(string connID, string table, string pkFiled, string pkValue)
        {
            int count = 0;
            var conn = FindByID(connID);
            if (conn == null)
            {
                return count;
            }
            switch (conn.Type)
            {
                case "MySql":
                    using (var dbconn = GetConnection(conn))
                    {
                        try
                        {
                            dbconn.Open();
                        }
                        catch (MySqlException ex)
                        {
                            //Platform.Log.Add(ex);
                        }
                        string sql = string.Format("DELETE FROM {0} WHERE {1}=@{1}", table, pkFiled);
                        MySqlParameter par = new MySqlParameter("@" + pkFiled, pkValue);
                        using (MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)dbconn))
                        {
                            cmd.Parameters.Add(par);
                            try
                            {
                                count = cmd.ExecuteNonQuery();
                            }
                            catch (MySqlException ex)
                            {
                                //Platform.Log.Add(ex);
                            }
                        }
                    }
                    break;

            }
            return count;
        }
        /// <summary>
        /// 更新一个连接一个表一个字段的值
        /// </summary>
        /// <param name="connID"></param>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public void UpdateFieldValue(string connID, string table, string field, string value, string where)
        {
            var conn = FindByID(connID);
            if (conn == null)
            {
                return;
            }
            switch (conn.Type)
            {
                case "MySql":
                    using (var dbconn = GetConnection(conn))
                    {
                        try
                        {
                            dbconn.Open();
                        }
                        catch (MySqlException ex)
                        {
                            //Platform.Log.Add(ex);
                        }
                        string sql = string.Format("UPDATE {0} SET {1}=@value WHERE {2}", table, field, where);
                        MySqlParameter par = new MySqlParameter("@value", value);
                        using (MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)dbconn))
                        {
                            cmd.Parameters.Add(par);
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (MySqlException ex)
                            {
                                //Platform.Log.Add(ex);
                            }
                        }
                    }
                    break;

            }
        }

        /// <summary>
        /// 得到一个表的结构
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tableName"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public System.Data.DataTable GetTableSchema(System.Data.IDbConnection conn, string tableName, string dbType)
        {
            DataTable dt = new DataTable();
            switch (dbType)
            {
                case "MySql":
                    /*string sql = string.Format(@"select a.name as f_name,b.name as t_name,[length],a.isnullable as is_null, a.cdefault as cdefault,COLUMNPROPERTY( OBJECT_ID('{0}'),a.name,'IsIdentity') as isidentity from 
                    sys.syscolumns a inner join sys.types b on b.user_type_id=a.xtype 
                    where object_id('{0}')=id order by a.colid", tableName);
                    MySqlDataAdapter dap = new MySqlDataAdapter(sql, (MySqlConnection)conn);
                    dap.Fill(dt);*/
                    string sql = string.Format("show full fields from {0}", tableName);
                    dt=dBConnectionDAL.SqlTable(sql);
                    //MySqlDataAdapter dap = new MySqlDataAdapter(sql, (MySqlConnection)conn);
                    //dap.Fill(dt);
                    break;
            }
            return dt;
        }

        /// <summary>
        /// 得到一个连接一个表一个字段的值
        /// </summary>
        /// <param name="link_table_field"></param>
        /// <returns></returns>
        public string GetFieldValue(string link_table_field, string pkField, string pkFieldValue)
        {
            if (link_table_field.IsNullOrEmpty())
            {
                return "";
            }
            string[] array = link_table_field.Split('.');
            if (array.Length != 3)
            {
                return "";
            }
            string link = array[0];
            string table = array[1];
            string field = array[2];
            var allConns = GetAll();
            string linkid;
            if (!link.IsGuid(out linkid))
            {
                return "";
            }
            var conn = allConns.Find(p => p.ID == linkid);
            if (conn == null)
            {
                return "";
            }
            string value = string.Empty;
            switch (conn.Type)
            {
                case "MySql":
                    value = getFieldValue_MySql(conn, table, field, pkField, pkFieldValue);
                    break;
            }
            return value;
        }

        /// <summary>
        /// 得到一个连接一个表一个字段的值
        /// </summary>
        /// <param name="linkID">连接ID</param>
        /// <param name="table">表</param>
        /// <param name="field">字段</param>
        /// <param name="pkField">主键字段</param>
        /// <param name="pkFieldValue">主键值</param>
        /// <returns></returns>
        private string getFieldValue_MySql(DBConnection conn, string table, string field, string pkField, string pkFieldValue)
        {
            string v = "";
            using (MySqlConnection sqlConn = new MySqlConnection(conn.ConnectionString))
            {
                try
                {
                    sqlConn.Open();
                }
                catch (MySqlException err)
                {
                    //Log.Add(err);
                    return "";
                }
                string sql = string.Format("SELECT {0} FROM {1} WHERE {2} = '{3}'", field, table, pkField, pkFieldValue);
                using (MySqlDataAdapter dap = new MySqlDataAdapter(sql, sqlConn))
                {
                    try
                    {
                        DataTable dt = new DataTable();
                        dap.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            v = dt.Rows[0][0].ToString();
                        }
                    }
                    catch (MySqlException err)
                    {
                        //Log.Add(err);
                    }
                    return v;
                }
            }
        }
    }
}

