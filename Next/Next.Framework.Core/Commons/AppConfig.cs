using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Next.Framework.Commons
{
    public sealed class AppConfig
    {
        private string filePath;

        public AppConfig()
        {
            string webconfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Web.Config");
            string appconfig = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile.Replace(".vshost", "");

            if(File.Exists(webconfig))
            {
                filePath = webconfig;
            }
            else if(File.Exists(appconfig))
            {
                throw new ArgumentNullException("没有找到Web.Config文件或者应用程序配置文件，请指定配置文件");
            }
        }
        public AppConfig(string configFilePath)
        {
            filePath = configFilePath;
        }
        public void AppConfigSet(string keyName, string keyValue)
        {
            XmlDocument document = new XmlDocument();
            document.Load(filePath);


            XmlNodeList nodes = document.GetElementsByTagName("add");

            for (int i = 0; i < nodes.Count; i++)
            {
                XmlAttribute attribute = nodes[i].Attributes["key"];
                if (attribute != null && (attribute.Value == keyName))
                {
                    attribute = nodes[i].Attributes["value"];
                    if (attribute != null)
                    {
                        attribute.Value = keyValue;
                        break;
                    }
                }
            }
            document.Save(filePath);
        }
        public string AppConfigGet(string keyName)
        {
            string strReturn = string.Empty;
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(filePath);


                XmlNodeList nodes = document.GetElementsByTagName("add");

                for (int i = 0; i < nodes.Count; i++)
                {
                    XmlAttribute attribute = nodes[i].Attributes["key"];
                    if (attribute != null && (attribute.Value == keyName))
                    {
                        attribute = nodes[i].Attributes["value"];
                        if (attribute != null)
                        {
                            strReturn = attribute.Value;
                            break;
                        }
                    }
                }

            }
            catch(Exception e) 
            {
                throw new Exception("没有找到对应的键值"+e.ToString());
            }
            return strReturn;
        }
    }
}
