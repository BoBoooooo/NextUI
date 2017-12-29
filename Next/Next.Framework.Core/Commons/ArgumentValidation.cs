using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Framework.Core.Commons
{
    public sealed class ArgumentValidation
    {
        private const string ExceptionEmptyString = "参数'{0}'的值不能为空字符串。";
        private const string ExceptionInvalidNullNameArgument = "参数'{0}'的名称不能为空引用或空字符串。";
        private const string ExceptionByteArrayValueMustBeGreaterThanZeroBytes = "数值必须大于0字节。";
        private const string ExceptionExpectedType = "无效的类型，期待的类型必须为'{0}'。";
        private const string ExceptionEnumerationNotDefined = "{0}不是{1}的一个有效值";

        private ArgumentValidation()
        {

        }

        public static void CheckForEmptyString(string variable, string variableName)
        {
            CheckForNullReference(variable, variableName);
            CheckForNullReference(variableName, "variableName");
            if (variable.Length == 0)
            {
                string message = string.Format(ExceptionEmptyString, variableName);
                throw new ArgumentException(message);
            }
            
        }

        public static void CheckForNullReference(object variable, string variableName)
        {
            if (variableName == null)
            {
                throw new ArgumentNullException("variableName");
            }
            if (null == variable)
            {
                throw new ArgumentNullException(variableName);
            }
        }

        public static void CheckForInvalidNullNameReference(string name, string messageName)
        {
            if((null==name)||(name.Length==0)){
                string message = string.Format(ExceptionInvalidNullNameArgument, messageName);
                throw new InvalidOperationException(message);
            }
        }

        public static void CheckForZeroBytes(byte[] bytes, string variableName)
        {
            CheckForNullReference(bytes, "bytes");
            CheckForNullReference(variableName, "variableName");
            if (bytes.Length == 0)
            {
                string message = string.Format(ExceptionByteArrayValueMustBeGreaterThanZeroBytes, variableName);
                throw new ArgumentException(message);
            }
        }

        public static void CheckExpectedType(object variable, Type type)
        {
            CheckForNullReference(variable, "variable");
            CheckForNullReference(type, "type");
            if (!type.IsAssignableFrom(variable.GetType()))
            {
                string message = string.Format(ExceptionExpectedType, type.FullName);
                throw new ArgumentException(message);
            }
        }
        public static void CheckEnumeration(Type enumType, object variable, string variableName)
        {
            CheckForNullReference(variable, "variable");
            CheckForNullReference(enumType, "enumType");
            CheckForNullReference(variableName, "variableName");

            if (!Enum.IsDefined(enumType, variable))
            {
                string message = string.Format(ExceptionEnumerationNotDefined, variable.ToString(), enumType.FullName, variableName);
                throw new ArgumentException(message);
            }
        }
    }
}
