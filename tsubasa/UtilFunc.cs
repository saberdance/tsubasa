using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO.Compression;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace tsubasa
{
    public static class UtilFunc
    {
        public static  T findInArray<T>(T[] array,T tMatch) where T:IComparable
        {
            return Array.Find(array, (p) => p.Equals(tMatch));
        }
        //用于获得字符串数组中包含szMatch的元素
        //eg:"huangyi" str[]={"huangyi is a bitch","zhangyi is a badboy"} 
        //得到result[]={"huangyi is a bitch"}
        public static string[] FindMatchInArray(string[] array, string szMatch)
        {
           return array.Where(f1 => f1.Contains(szMatch)).ToArray();
        }
        //用于获得字符串数组中包含于szToFind字符串的元素
        //eg:"huangyi is a bitch" str[]={"huangyi","bitch","fff"} 
        //得到result[]={"huangyi","bitch"}
        public static string[] FindPattenInArray(string[] array, string szToFind)
        {
            return array.Where(f1 => szToFind.Contains(f1)).ToArray();
        }

        public static int RandomNumber(int number,int rangeRate)
        {
            return new Random().Next(0, number / rangeRate);
        }

        public static string MD5String(string src)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] srcByte = Encoding.UTF8.GetBytes(src);
            byte[] retVal = md5.ComputeHash(srcByte);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static string MD5File(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }
            byte[] fileBinary = File.ReadAllBytes(path);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fileBinary);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static byte[] MD5Byte(string src)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] srcByte = Encoding.UTF8.GetBytes(src);
            return md5.ComputeHash(srcByte);    
        }

        public static byte[] Sha256Byte(string src)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] srcByte = Encoding.UTF8.GetBytes(src);
            return sha256.ComputeHash(srcByte);
        }

        public static string FillStringWithToken(string orgStr, string tokenStr, int targetLength, bool isAppend = false)
        {
            string ret = orgStr;
            while (ret.Length < targetLength)
            {
                ret = isAppend ? ret + tokenStr : tokenStr + ret;
            }
            return ret;
        }
        public static string[] SplitStrByFirstSpace(string str)
        {
            string[] ret = new string[2];
            int idx = str.IndexOf(' ');
            if (idx==-1)
            {
                return null;
            }
            ret[0] = str.Substring(0, idx);
            ret[1] = str.Substring(idx+1);
            return ret;
        }
        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                using (FileStream file = new FileStream(fileName, System.IO.FileMode.Open))
                {
                    MD5 md5 = new MD5CryptoServiceProvider();
                    byte[] retVal = md5.ComputeHash(file);
                    file.Close();
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < retVal.Length; i++)
                    {
                        sb.Append(retVal[i].ToString("x2"));
                    }
                    return sb.ToString();
                }           
            }
            catch (Exception ex)
            {
                Logger.Error("MD5 Failed:"+ex.Message);
                return "";
            }
        }

        public static string GetPureJson(string data)
        {
            string result = string.Empty;
            int startindex, endindex;
            try
            {
                startindex = data.IndexOf(@"{");
                if (startindex == -1)
                    return result;
                string tmpstr = data.Substring(startindex);
                Logger.ConsoleLog(tmpstr);
                endindex = tmpstr.IndexOf(@"}");
                if (endindex == -1)
                    return result;
                result = tmpstr.Remove(endindex)+@"}";
            }
            catch (Exception)
            {
                return string.Empty;
            }
            return result;
        }

        /// <summary> 
        /// 字节数组转16进制字符串 
        /// </summary> 
        /// <param name="bytes"></param> 
        /// <returns></returns> 
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
    }
}

