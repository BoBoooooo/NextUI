using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Framework.Core.Commons
{
    public sealed class SmartDataReader
    {
        private DateTime defaultDate;
        private IDataReader reader;

        public SmartDataReader(IDataReader reader)
        {
            this.defaultDate = Convert.ToDateTime("01/01/1900 00:00:00");
            this.reader = reader;
        }
        public int GetInt32(String column, int defaultIfNull)
        {
            int data=(reader.IsDBNull(reader.GetOrdinal(column)))?(int)defaultIfNull:int.Parse(reader[column].ToString());
            return data;
        }
        public bool GetBoolean(string column)
        {
            return GetBoolean(column, false);
        }
        public bool GetBoolean(string column, bool defaultIfNull)
        {
            string str = reader[column].ToString();
            try
            {
                int i = Convert.ToInt32(str);
                return i > 0;
            }
            catch
            {
                //throw new Exception("非法的Bool值，请检查数据库字段类型定义"+e.ToString());
            }
            bool data = (reader.IsDBNull(reader.GetOrdinal(column))) ? defaultIfNull : bool.Parse(str);
            return data;
        }
        public string GetString(string column)
        {
            return GetString(column, "");
        }
        public string GetString(String column, string defaultIfNull)
        {
            string data = (reader.IsDBNull(reader.GetOrdinal(column))) ? defaultIfNull : reader[column].ToString();
            return data;
        }
        public Guid GetGuid(string column)
        {
            return GetGuid(column, null);
        }
        public Guid GetGuid(string column, string defaultIfNull)
        {
            string data=(reader.IsDBNull(reader.GetOrdinal(column))) ? defaultIfNull : reader[column].ToString();
            Guid guid = Guid.Empty;
            if (data != null)
            {
                guid = new Guid(data);
            }
            return guid;
        }
        public DateTime GetDateTime(string column)
        {
            return GetDateTime(column, defaultDate);
        }

        public DateTime GetDateTime(String column, DateTime defaultIfNull)
        {
            DateTime data = (reader.IsDBNull(reader.GetOrdinal(column))) ? defaultIfNull : Convert.ToDateTime(reader[column].ToString());
            return data;
        }
    }
}
