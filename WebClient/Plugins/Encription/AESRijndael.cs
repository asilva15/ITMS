using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace WebClient.Plugins.Encription
{
    public class AESRijndael
    {
        byte[] _key;
        byte[] _iv;

        public AESRijndael()
        {
            _key = Encoding.ASCII.GetBytes("EDEstaC1@ve34e756d!fLcX!CoN9$7WF");
            _iv = Encoding.ASCII.GetBytes("I@T#M4S$78Rth50p"); 
        }

        public string Encrypt(string inputText)
        {

        byte[] inputBytes = Encoding.ASCII.GetBytes(inputText);
        byte[] encripted;

        RijndaelManaged cripto = new RijndaelManaged();

        using (MemoryStream ms = new MemoryStream(inputBytes.Length)){

            using (CryptoStream objCryptoStream = new CryptoStream(ms,cripto.CreateEncryptor(_key, _iv),CryptoStreamMode.Write))
            {
                objCryptoStream.Write(inputBytes, 0, inputBytes.Length);
                objCryptoStream.FlushFinalBlock();
                objCryptoStream.Close();
            }

            encripted = ms.ToArray();
        }

        return Convert.ToBase64String(encripted);

        }

        public string Decrypt(string inputText)
        {

        byte[] inputBytes = Convert.FromBase64String(inputText);
        byte[] resultBytes = new byte[inputBytes.Length];
        string textoLimpio = String.Empty;

        RijndaelManaged cripto = new RijndaelManaged();

        using (MemoryStream ms = new MemoryStream(inputBytes)){

            using (CryptoStream objCryptoStream = new CryptoStream(ms, cripto.CreateDecryptor(_key, _iv),CryptoStreamMode.Read)){

                using (StreamReader sr = new StreamReader(objCryptoStream, true)){
                textoLimpio = sr.ReadToEnd();
                }

            }

        }

        return textoLimpio;

        }
    }
}