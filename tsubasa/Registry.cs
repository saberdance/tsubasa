#if _WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;

namespace tsubasa
{
    /// <summary>
    /// 注册表类
    /// </summary>
    public static class Registry
    {
        public static RegistryKey QueryRegistrySubkey(RegistryKey mainKey,string subkeyPath)
        {
           return mainKey.OpenSubKey(subkeyPath, true);
        }
        public static object QueryRegisrtyValue(RegistryKey key, string Value)
        {
#pragma warning disable CA1416 // 验证平台兼容性
            return key.GetValue(Value);
#pragma warning restore CA1416 // 验证平台兼容性
        }
        public static string QueryRegisrtyString(RegistryKey key, string Value)
        {
            //LogInD("QueryRegisrtyString");
            return QueryRegisrtyValue(key, Value).ToString();
        }
        public static int QueryRegisrtyInt(RegistryKey key, string Value)
        {
            return Convert.ToInt32(QueryRegisrtyValue(key, Value));
        }
        public static bool QueryRegisrtyBool(RegistryKey key, string Value)
        {
            return Convert.ToBoolean(QueryRegisrtyValue(key, Value));
        }
        public static RegistryKey CreateMainKey(RegistryKey parentKey, string keyName)
        {
            try
            {
#pragma warning disable CA1416 // 验证平台兼容性
                return parentKey.CreateSubKey(keyName);
#pragma warning restore CA1416 // 验证平台兼容性
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static bool SetKeyValue(RegistryKey parentKey, string valueName, string value)
        {
            try
            {
#pragma warning disable CA1416 // 验证平台兼容性
                parentKey.SetValue(valueName, value);
#pragma warning restore CA1416 // 验证平台兼容性
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
#else
namespace tsubasa
{
    /// <summary>
    /// 非Windows系统无法访问注册表
    /// </summary>
    public static class Registry
    {
    }
}
#endif