using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Crestron.SimplSharp.Cryptography;
using Crestron.SimplSharp.CrestronIO;
using Crestron.SimplSharp;

namespace EcloudUtils
{
    public class Des
    {
        
        public static string DesKey = "ecloud88";
        public static string IV = "12345678";
        /// <summary>
        /// 默认加密方法
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string DESEncrypt(string text)
        {
            return DESEncrypt(text, DesKey);
        }
        /// <summary>
        /// DES加密方法
        /// </summary>
        /// <param name="text">明文</param>
        /// <param name="sKey">密钥</param>
        /// <returns>加密后的密文</returns>
        public string DESEncrypt(string text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray;
            inputByteArray = Encoding.Default.GetBytes(text);
            des.Key = Encoding.UTF8.GetBytes(sKey.Substring(0, 8));
            des.IV = Encoding.UTF8.GetBytes(sKey.Substring(0, 8));
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ms.Dispose();
            cs.Dispose();
            return ret.ToString();
        }
        /// <summary>
        /// DES解密方法，默认方法
        /// </summary>
        /// <param name="text">待加密明文</param>
        /// <returns>加密后的密文</returns>
        public static string DESDecrypt(string text)
        {
            return DESDecrypt(text, DesKey);
        }
        /// <summary>
        /// DES解密方法
        /// </summary>
        /// <param name="text">密文</param>
        /// <param name="sKey">密钥</param>
        /// <returns>解密后的明文</returns>
        public static string DESDecrypt(string text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            int len;
            len = text.Length / 2;
            byte[] inputByteArray = new byte[len];
            int x, i;
            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(text.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            try
            {
                des.Key = Encoding.UTF8.GetBytes(sKey.Substring(0, 8));
                des.IV = Encoding.UTF8.GetBytes(sKey.Substring(0, 8));
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                string estring = Encoding.Unicode.GetString(ms.ToArray(), 0, ms.ToArray().Length - 1);
                ms.Dispose();
                cs.Dispose();
                return estring;
            }
            catch
            {
                return "";
            }
        }

        public static byte[] DESDecryptToByte(string text)
        {
            return DESDecryptToByte(text, DesKey);
        }

        public static byte[] DESDecryptToByte(string text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            int len;
            len = text.Length / 2;
            byte[] inputByteArray = new byte[len];
            int x, i;
            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(text.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            des.Key = Encoding.UTF8.GetBytes(sKey.Substring(0, 8));
            des.IV = Encoding.UTF8.GetBytes(sKey.Substring(0, 8));
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            byte[] b = ms.ToArray();
            ms.Dispose();
            cs.Dispose();
            return b;
        }

        /// <summary>  
        /// DES加密  
        /// </summary>  
        /// <param name="pToEncrypt"></param>  
        /// <param name="sKey"></param>  
        /// <returns></returns>  
        public  static string MD5Encrypt(string pToEncrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(IV);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            return Convert.ToBase64String(ms.ToArray());
        }
        /// <summary>  
        /// DES解密  
        /// </summary>  
        /// <param name="pToDecrypt"></param>  
        /// <param name="sKey"></param>  
        /// <returns></returns>  
        public static string MD5Decrypt(string pToDecrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            byte[] inputByteArray = Convert.FromBase64String(pToDecrypt);
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(IV);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            return Encoding.Default.GetString(ms.ToArray(), 0, ms.ToArray().Length - 1);
        }
    
    }
}