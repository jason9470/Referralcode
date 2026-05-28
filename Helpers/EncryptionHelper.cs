using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Referralcode.Helpers
{
    public class EncryptionHelper
    {
        private readonly byte[] _key;

        public EncryptionHelper(IConfiguration configuration)
        {
            var keyString = configuration["EncryptionSettings:Key"] ?? "";
            _key = Encoding.UTF8.GetBytes(keyString);
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.GenerateIV();
            var iv = aes.IV;

            using var encryptor = aes.CreateEncryptor(aes.Key, iv);
            using var ms = new MemoryStream();
            ms.Write(iv, 0, iv.Length); // 將 IV 放在加密資料的最前方

            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }
            return Convert.ToBase64String(ms.ToArray());
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            try
            {
                var fullCipher = Convert.FromBase64String(cipherText);
                using var aes = Aes.Create();
                aes.Key = _key;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                var ivSize = aes.BlockSize / 8;
                if (fullCipher.Length < ivSize) return string.Empty;

                var iv = new byte[ivSize];
                var cipher = new byte[fullCipher.Length - ivSize];

                Array.Copy(fullCipher, 0, iv, 0, ivSize);
                Array.Copy(fullCipher, ivSize, cipher, 0, cipher.Length);

                using var decryptor = aes.CreateDecryptor(aes.Key, iv);
                using var ms = new MemoryStream(cipher);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var sr = new StreamReader(cs);
                return sr.ReadToEnd();
            }
            catch
            {
                // 如果解密失敗（例如舊格式），回傳空字串或處理錯誤
                return string.Empty;
            }
        }
    }
}
