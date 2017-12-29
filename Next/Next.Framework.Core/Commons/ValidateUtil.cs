using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Next.Framework.Core.Commons
{
    public class ValidateUtil
    {
        public static readonly string NumericRegex = @"^[-]?\d+[.]?\d*$";
        public static bool IsNumeric(string inputData)
        {
            Regex RegNumeric = new Regex(NumericRegex);
            Match m = RegNumeric.Match(inputData);
            return m.Success;
        }
        /// <summary>
        /// 是否整数字正则表达式
        /// </summary>
        public static readonly string NumberRegex = @"^[0-9]+$";
        public static bool IsNumber(string inputData)
        {
            Regex RegNumber = new Regex(NumberRegex);
            Match m = RegNumber.Match(inputData);
            return m.Success;
        }
    }
}
