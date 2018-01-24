using System;
using System.Collections.Generic;
using System.Text;

using System.Text.RegularExpressions;

namespace Next.WorkFlow.Utility
{
    public class HtmlCodeFormat
    {
        public static string Format(string inputString)
        {
            StringBuilder outputString = new StringBuilder();
            string[] arrayString = ToFormatArray(inputString);
            arrayString = FormatImp(arrayString);

            foreach (string ss in arrayString)
            {
                outputString.Append(ss + "\r\n");
            }

            return outputString.ToString();

        }

        private static string[] ToFormatArray(string inputString)
        {
            StringBuilder workString = new StringBuilder(inputString);
            StringBuilder outputString = new StringBuilder();
            //去掉换行
            workString.Replace("\r\n", "");
            //添加必须的换行
            workString.Replace("<", "\n<");
            workString.Replace(">", ">\n");

            char[] cc = new char[1] { '\n' };
            string[] arrayString = workString.ToString().Split(cc);
            //去掉空行
            foreach (string s in arrayString)
            {
                // 去掉前前后空白字符
                string text = s.Trim();
                if (text != "")
                {
                    outputString.Append(text + "\n");
                }
            }

            // 不包含空行的字符串数组
            arrayString = outputString.ToString().Split(cc);

            return arrayString;
        }
        private static string[] FormatImp(string[] arrayString)
        {
            List<int> arrayIndex = new List<int>() ;
            for (int i = 0; i < arrayString.Length;i++ )
            {
                arrayIndex.Add(0);
            }
            int indent = 4;
            int spaceCount = 0;
            for (int i = 0; i < arrayString.Length; i++)
            {
                var current = arrayString[i];
                if (Regex.IsMatch(arrayString[i], "/>$")) //匹配HTML开始标签 </html> </body> </font>
                {
                    arrayString[i] = CreateBlank(indent * spaceCount) + arrayString[i];
                }
                else if (Regex.IsMatch(arrayString[i], "^</")) //匹配HTML开始标签 </html> </body> </font>
                {
                    spaceCount--;
                    arrayString[i] = CreateBlank(indent * spaceCount) + arrayString[i];

                }
                else if (Regex.IsMatch(arrayString[i], "<.+>")) //匹配HTML开始标签 <html> <head> <title> <font>
                {
                    arrayString[i] = CreateBlank(indent * spaceCount) + arrayString[i];
                    spaceCount++;

                }
                else
                {
                    arrayString[i] = CreateBlank(indent * spaceCount) + arrayString[i];
                }
            }
            return arrayString;

        }
        /*private static string[] FormatImp(string[] arrayString)
        {
            int indent = 4;
            int count = 0;
            int level = 1;
            // 找第一个HTML开始标签，<html>
            for (int i = 0; i < arrayString.Length; i++)
            {
                var current = arrayString[i];
                if (Regex.IsMatch(arrayString[i], "<.\\/>")) //匹配HTML开始标签 </html> </body> </font>
                {
                    arrayString[i] = CreateBlank(indent * (count + level)) + arrayString[i];                  
                }
                else if (Regex.IsMatch(arrayString[i], "<\\/.>")) //匹配HTML开始标签 </html> </body> </font>
                {
                    level--;
                    count--;
                    arrayString[i] = CreateBlank(indent * (count+level) ) + arrayString[i];

                }
                else if (Regex.IsMatch(arrayString[i], "<[^\\/].>")) //匹配HTML开始标签 <html> <head> <title> <font>
                {
                    count++;
                    level++;
                    arrayString[i] = CreateBlank(indent * (count + level)) + arrayString[i];

                }
                else
                {

                    arrayString[i] = CreateBlank(indent * (count + level)) + arrayString[i];
                }
            }

            return arrayString;
        }*/

        private static string CreateBlank(int length)
        {
            if (length <= 0)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(" ");
            }
            return sb.ToString();
        }

        private enum LastTagType
        {
            BeginTag,
            Text,
            EndTag,
            None
        }
    }
}
