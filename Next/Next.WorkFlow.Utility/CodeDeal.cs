using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Next.WorkFlow.Utility
{
    public class CodeDeal
    {
        public static string DealCode(string InputString)
        {
            string OutputString = "";
            InputString = Regex.Replace(InputString, ";", ";\n");//���ֺŻ���
            InputString = Regex.Replace(InputString, "}", "\n } \n");//��"}"����
            InputString = Regex.Replace(InputString, "{", "\n { \n");//��"{"����          
            char[] cc = new char[1] { '\n', };
            string[] ArryInput = new string[5000];
            ArryInput = InputString.Split(cc);
            //ȥ������
            foreach (string ss in ArryInput)
            {
                string ss1 = Regex.Replace(ss, "\\s*", "");
                if (ss1 != "")
                {
                    OutputString += ss.Trim() + "\n";
                }
            }
            ArryInput = OutputString.Split(cc);
            OutputString = "";
            ArryInput = FormatKUOHAO(ArryInput);//����{}ƥ��            
            foreach (string ss in ArryInput)
            {
                OutputString += ss + "\r\n";
            }

            OutputString = Regex.Replace(OutputString, @"(?<Function>(?<=\r\n)\s*(int|long|void|char|bool|string|(unsigned\s*int)|(short\s*int)|unsigned|short|(long\s*int)|)\s*\*?\s*[A-Za-z_]*\w*\s*\(.*?\)\s*(?=\r\n|{))", "\r\n${Function}");
            OutputString = Regex.Replace(OutputString, "\r\n\r\n(?<A>(\\s)*?(if|for|while|do)\\()", "\r\n${A}");
            return OutputString;
        }
        /// <summary>
        /// ƥ���������õ����ݽṹ
        /// </summary>
        struct Place
        {
            public string data;
            public int place;
        }

        public static string[] FormatKUOHAO(string[] Intput)
        {
            Place[] MyPlace = new Place[5000];
            int Top = -1;
            int i = 0;  //��������Input  
            int j = 0;
            int k = 0;
            try
            {
                //�ҵ�һ����{��
                for (j = 0; j < Intput.Length; j++)
                {
                    if (Intput[j] == "{")
                    {
                        Top++;
                        MyPlace[Top].data = "{";
                        MyPlace[Top].place = j;
                        break;
                    }
                }
                i = j + 1;
                while (Top >= 0 || i < Intput.Length)
                {
                    if (Intput[i] == "{")
                    {
                        Top++;
                        MyPlace[Top].data = "{";
                        MyPlace[Top].place = i;
                        i++;
                    }
                    else if (Intput[i] == "}")
                    {
                        int P1 = MyPlace[Top].place;
                        int P2 = i;
                        for (k = P1 + 1; k < P2; k++)
                        {
                            Intput[k] = "    " + Intput[k];
                        }
                        //Output[i] += "\n";
                        Top--;
                        i++;
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
                return Intput; //{"\r\n"+"������Ĵ����д��������ɹ���������\r\n���ܴ�����ʾ:\r\n'{'��'}'��ƥ��\r\n�����д���5000��" };             
            }
            finally
            {
                //  return Intput;
            }
            return Intput;
        }
    }
}
