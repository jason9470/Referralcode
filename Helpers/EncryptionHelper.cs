using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

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

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] nonce = new byte[AesGcm.NonceByteSizes.MaxSize]; // 12 bytes
            RandomNumberGenerator.Fill(nonce);

            byte[] cipherBytes = new byte[plainBytes.Length];
            byte[] tag = new byte[AesGcm.TagByteSizes.MaxSize]; // 16 bytes

            using var aesGcm = new AesGcm(_key, AesGcm.TagByteSizes.MaxSize);
            aesGcm.Encrypt(nonce, plainBytes, cipherBytes, tag);

            // Format: [Nonce (12)] + [Tag (16)] + [Cipher]
            byte[] result = new byte[nonce.Length + tag.Length + cipherBytes.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
            Buffer.BlockCopy(tag, 0, result, nonce.Length, tag.Length);
            Buffer.BlockCopy(cipherBytes, 0, result, nonce.Length + tag.Length, cipherBytes.Length);

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            try
            {
                var fullCipher = Convert.FromBase64String(cipherText);
                
                int nonceSize = AesGcm.NonceByteSizes.MaxSize; // 12 bytes
                int tagSize = AesGcm.TagByteSizes.MaxSize; // 16 bytes

                if (fullCipher.Length < nonceSize + tagSize) return string.Empty;

                byte[] nonce = new byte[nonceSize];
                byte[] tag = new byte[tagSize];
                byte[] cipherBytes = new byte[fullCipher.Length - nonceSize - tagSize];

                Buffer.BlockCopy(fullCipher, 0, nonce, 0, nonceSize);
                Buffer.BlockCopy(fullCipher, nonceSize, tag, 0, tagSize);
                Buffer.BlockCopy(fullCipher, nonceSize + tagSize, cipherBytes, 0, cipherBytes.Length);

                byte[] plainBytes = new byte[cipherBytes.Length];

                using var aesGcm = new AesGcm(_key, AesGcm.TagByteSizes.MaxSize);
                aesGcm.Decrypt(nonce, cipherBytes, tag, plainBytes);

                return Encoding.UTF8.GetString(plainBytes);
            }
            catch
            {
                // 如果解密失敗（例如舊的 CBC 格式或毀損），回傳空字串
                return string.Empty;
            }
        }
    }
}
