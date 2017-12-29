using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Framework.Core.Commons
{
    public class SearchInfo
    {
        public SearchInfo()
        {

        }
        public SearchInfo(string fieldName,object fieldValue,SqlOperator sqlOperator)
            :this(fieldName,fieldValue,sqlOperator,true)
        { }

        public SearchInfo(string fieldName, object fieldValue,SqlOperator sqlOperator,bool excludeIfEmpty)
            :this(fieldName,fieldValue,sqlOperator,excludeIfEmpty,null)
        { }

        public SearchInfo(string fieldName, object fieldValue, SqlOperator sqlOperator, bool excludeIfEmpty, string groupName)
        {
            this.FieldName = fieldName;
            this.FieldValue = fieldValue;
            this.SqlOperator = sqlOperator;
            this.ExcludeIfEmpty = excludeIfEmpty;
            this.GroupName = groupName;
        }
        public string GroupName { get; set; }
        public string FieldName { get; set; }
        public object FieldValue { get; set; }
        public SqlOperator SqlOperator { get; set; }
        public bool ExcludeIfEmpty { get; set; }

    }
}
