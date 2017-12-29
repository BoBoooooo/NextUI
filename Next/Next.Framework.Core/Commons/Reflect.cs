using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Next.Framework.Commons
{
    /// <summary>
    /// 通过反射的方式生成业务类的对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Reflect<T> where T:class
    {

        private static Hashtable ObjCache = new Hashtable();
        private static object syncRoot = new Object();
        /// <summary>
        /// 根据参数创建对象实例
        /// </summary>
        /// <param name="sName">对象的名字</param>
        /// <param name="sFilePath">对象的路径</param>
        /// <returns></returns>
        public static T Create(string sName, string sFilePath)
        {
            return Create(sName, sFilePath, true);
        }

        public static T Create(string sName, string sFilePath, bool bCache)
        {
            string CacheKey = sName;
            T objType = null;
            if (bCache)
            {
                objType = (T)ObjCache[CacheKey];
                if (!ObjCache.ContainsKey(CacheKey))
                {
                    lock (syncRoot)
                    {
                        objType = CreateInstance(CacheKey, sFilePath);
                        ObjCache.Add(CacheKey, objType);
                    }
                }
            }
            else
            {
                objType = CreateInstance(CacheKey, sFilePath);
            }
            return objType;
        }
        private static T CreateInstance(string sName, string sFilePath)
        {
            Assembly assemblyObj = Assembly.Load(sFilePath);
            if (assemblyObj == null)
            {
                throw new ArgumentNullException("sFilePath", string.Format("无法加载sFilePath={0}的程序集", sFilePath));
            }
            T obj = (T)assemblyObj.CreateInstance(sName);
            return obj;
        }
    }
}
