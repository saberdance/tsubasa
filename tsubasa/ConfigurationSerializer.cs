using System;
using System.Xml;
using System.Xml.Serialization;

namespace tsubasa
{
    /// <summary>
    /// 保存配置的返回错误码
    /// </summary>
    public class SaveConfigurationException : Exception
    {
        public SaveConfigurationException() { }
        public SaveConfigurationException(string message) : base(message) { }
        public SaveConfigurationException(string message, Exception inner) : base(message, inner) { }
    }
    /// <summary>
    /// 读取配置的返回错误码
    /// </summary>
    public class LoadConfigurationException : Exception
    {
        public LoadConfigurationException() { }
        public LoadConfigurationException(string message) : base(message) { }
        public LoadConfigurationException(string message, Exception inner) : base(message, inner) { }
    }
    public static class ConfigurationSerializer<T>
    {
        public static void Save(string path, T obj)
        {
            XmlSerializer writer = new(typeof(T));
            using (FileStream fs = new(path, FileMode.Create))
            {
                writer.Serialize(fs, obj);
            }
        }

        public static object Load(string path)
        {
            if (!File.Exists(path))
            {
                throw new LoadConfigurationException("文件不存在");
            }
            XmlSerializer reader = new(typeof(T));
            using (FileStream fs = new(path, FileMode.Open))
            {
                XmlReader xmlReader = new XmlTextReader(fs);
                if (!reader.CanDeserialize(xmlReader))
                {
                    throw new LoadConfigurationException("配置文件格式错误");
                }
                return reader.Deserialize(fs);
            }
        }
    }
}
