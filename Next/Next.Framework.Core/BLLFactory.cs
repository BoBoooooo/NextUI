using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Next.Framework.Commons;
namespace Next.Framework.Core
{
    /// <summary>
    /// 对业务类进行构造的工厂类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BLLFactory<T> where T:class
    {
        private static Hashtable objCache = new Hashtable();
        private static object syncRoot = new Object();

        public static T Instance
        {
            get
            {
                string CacheKey = typeof(T).FullName;
                T bll = (T)objCache[CacheKey];//如果新建过，则从缓存中获取对象，减少对象的生成数量
                if (bll == null)
                {
                    lock (syncRoot)
                    {
                        bll = (T)objCache[CacheKey];
                        if (bll == null)
                        {
                            bll = Reflect<T>.Create(typeof(T).FullName, typeof(T).Assembly.GetName().Name); //通过对象的名称，采用反射的方式生成对象
                            objCache.Add(typeof(T).FullName, bll); //将对象保存在哈希表中，下次再使用的时候可以直接获取，不需要在生成了
                        }
                    }
                }
                return bll;
            }
        }
    }
}
