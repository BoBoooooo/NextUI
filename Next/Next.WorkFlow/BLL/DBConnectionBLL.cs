using System;
using System.Collections.Generic;
using System.Text;
using Next.Framework.Core;
using Next.WorkFlow.Entity;
using Next.WorkFlow.DALMySql;
using Next.WorkFlow.IDAL;
using MySql.Data.MySqlClient;
using System.Linq;

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
	}
}
