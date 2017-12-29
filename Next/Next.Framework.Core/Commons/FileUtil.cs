using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Framework.Core.Commons
{
    public class FileUtil
    {
        public static string GetFileName(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            return fi.Name;
        }

        public static bool IsExistFile(string filePath)
        {
            return File.Exists(filePath);
        }

        public static void CreateFile(string filePath, byte[] buffer)
        {
            try
            {
                if (!IsExistFile(filePath))
                {
                    using (FileStream fs = File.Create(filePath))
                    {
                        fs.Write(buffer, 0, buffer.Length);
                    }
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }

        public static string GetFileName(string fullPath, bool removeExt)
        {
            FileInfo fi = new FileInfo(fullPath);
            string name = fi.Name;
            if (removeExt)
            {
                if (name.IndexOf('.') != -1)
                {
                    name = name.Remove(name.IndexOf('.'));
                }
            }
            return name;
        }

        public static string GetExtension(string filePath){
            FileInfo fi=new FileInfo(filePath);
            return fi.Extension;
        }
    }
}
