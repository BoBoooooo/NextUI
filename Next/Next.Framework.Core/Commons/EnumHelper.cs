using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Framework.Core.Commons
{
    public class EnumHelper
    {
        public static T GetInstance<T>(string member)
        {
            return ConvertHelper.ConvertTo<T>(Enum.Parse(typeof(T), member, true));
        }
        public static string[] GetMemberNames<T>()
        {
            return Enum.GetNames(typeof(T));
        }
    }
}
