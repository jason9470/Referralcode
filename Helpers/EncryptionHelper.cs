using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Referralcode.Helpers
{
    public static class EncryptionHelper
    {
        // 這些金鑰在正式環境中應該要放在環境變數或是更安全的地方
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("A1B2C3D4E5F67890A1B2C3D4E5F67890"); // 32 bytes for AES-256
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("A1B2C3D4E5F67890"); // 16 bytes for AES block size

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;
            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }
            return Convert.ToBase64String(ms.ToArray());
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;
            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
}
