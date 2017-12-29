using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Framework.Core.Commons
{
    public sealed class ConvertHelper
    {
        public static object ConvertTo(object data, Type targetType)
        {
            if (data == null || Convert.IsDBNull(data))
            {
                return null;
            }
            Type type2 = data.GetType();
            if (targetType == type2)
            {
                return data;
            }
            if (((targetType == typeof(Guid)) || (targetType == typeof(Guid?))) && (type2 == typeof(string)))
            {
                if (string.IsNullOrEmpty(data.ToString()))
                {
                    return null;
                }
                return new Guid(data.ToString());
            }
            if (targetType.IsEnum)
            {
                try
                {
                    return Enum.Parse(targetType, data.ToString(), true);
                }
                catch
                {
                    
                    return Enum.ToObject(targetType, data);
                }
            }
            if (targetType.IsGenericType)
            {
                targetType = targetType.GetGenericArguments()[0];
            }
            return Convert.ChangeType(data, targetType);
        }

        public static T ConvertTo<T>(object data)
        {
            if (data == null || Convert.IsDBNull(data))
            {
                return default(T);
            }
            object obj = ConvertTo(data, typeof(T));
            if (obj == null)
            {
                return default(T);
            }
            return (T)obj;
        }
    }
}
