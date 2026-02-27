using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
// using Internal;
// using Internal;

namespace dotnetapp.Services
{
    public class AESHelper
    {
        private static readonly byte[] KeyBytes = Encoding.UTF8.GetBytes(Keys.Key); // 16 bytes
        private static readonly byte[] IvBytes = Encoding.UTF8.GetBytes(Keys.IV);  // 16 bytes

        public static string Decrypt(string encryptedText)
        {
            Console.WriteLine("Inside decrypt fn:"+encryptedText);
            byte[] cipherBytes = Convert.FromBase64String(encryptedText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = KeyBytes;
                aes.IV = IvBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using var ms = new MemoryStream(cipherBytes);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var reader = new StreamReader(cs);
                return reader.ReadToEnd();
            }
        }
    }
}
