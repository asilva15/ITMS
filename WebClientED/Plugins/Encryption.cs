using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Security.Cryptography.SymmetricAlgorithm;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace WebClient.Plugins
{
    public class Encryption
    {
        static byte[] key = { };
        static byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef };
        static string sEncryptionKey = "I@3#S!$0";

        public string Decrypt(string stringToDecrypt)
        {
           byte[] inputByteArray = new byte[stringToDecrypt.Length + 1];
           try
           {
               key = System.Text.Encoding.UTF8.GetBytes(Left(sEncryptionKey, 8));
               DESCryptoServiceProvider des = new DESCryptoServiceProvider();
               inputByteArray = Convert.FromBase64String(stringToDecrypt);
               MemoryStream ms = new MemoryStream();
              CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
              cs.Write(inputByteArray, 0, inputByteArray.Length);
              cs.FlushFinalBlock();
              System.Text.Encoding encoding = System.Text.Encoding.UTF8;
              return encoding.GetString(ms.ToArray());
          }
          catch (Exception e)
          {
              return e.Message;
          }
      }

        public string Encrypt(string stringToEncrypt)
        {
            try
            {
                key = System.Text.Encoding.UTF8.GetBytes(Left(sEncryptionKey, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static string Left(string param, int length)
        {

            string result = param.Substring(0, length);
            return result;
        }
    }
}