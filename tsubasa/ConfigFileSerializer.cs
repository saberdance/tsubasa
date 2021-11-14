using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace tsubasa
{
    public class ConfigFileSerializer<T> where T:new()
    {
        public T Deserialize(string filePath,Encoding encoding)
        { 
            if (!File.Exists(filePath))
            {
                return default(T);
            }
            T obj = new();
            using (StreamReader sr = new(filePath,encoding))
            {
                string line = null;
                while ((line = sr.ReadLine())!=null)
                {
                    KeyValuePair<string, string> pair = SyncKeyPair(line);
                    if (pair.Key is null||pair.Value is null)
                    {
                        continue;
                    }
                    TryUpdateObjectProperty(pair,ref obj);
                }
            }
            return obj;
        }

        private void TryUpdateObjectProperty(KeyValuePair<string, string> pair,ref T obj)
        {
            try
            {
                var tType = obj.GetType();
                PropertyInfo item = tType.GetRuntimeProperties().Where(o => o.Name == pair.Key).FirstOrDefault();
                if (item == default(PropertyInfo))
                {
                    return;
                }
                switch (item.PropertyType.Name.ToLower())
                {
                    case "string":
                        item.SetValue(obj, pair.Value);
                        break;
                    case "int":
                        item.SetValue(obj, int.Parse(pair.Value));
                        break;
                    case "double":
                        item.SetValue(obj, double.Parse(pair.Value));
                        break;
                    case "float":
                        item.SetValue(obj, float.Parse(pair.Value));
                        break;
                    case "demical":
                        item.SetValue(obj, decimal.Parse(pair.Value));
                        break;
                    case "long":
                        item.SetValue(obj, long.Parse(pair.Value));
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                return;
            }
            
            
        }

        private KeyValuePair<string, string> SyncKeyPair(string line)
        {
            try
            {
                string key = line.Substring(line.IndexOf("[")+1, line.LastIndexOf("]")-1);
                string value = line.Substring(line.IndexOf("=")+1);
                return new KeyValuePair<string, string>(key, value);
            }
            catch (Exception)
            {
                return new KeyValuePair<string, string>(null,null);
            }
            
        }
    }
}
