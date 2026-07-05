using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using DailyTaskSheet.App.Interfaces;

namespace DailyTaskSheet.App.Services
{
    /// <summary>
    /// Provides AES-256-CBC encryption and decryption for securing locally stored data.
    /// Used to encrypt tokens, passwords, and other sensitive information in SharedPreferences.
    /// </summary>
    public class EncryptionService : IEncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        /// <summary>
        /// Initializes a new instance of <see cref="EncryptionService"/>.
        /// Derives a deterministic AES key from the provided key material.
        /// </summary>
        /// <param name="keyMaterial">The key material (device-specific or app-generated).</param>
        public EncryptionService(string keyMaterial)
        {
            if (string.IsNullOrEmpty(keyMaterial))
                throw new ArgumentNullException(nameof(keyMaterial));

            // Derive a 256-bit key and 128-bit IV from the key material using PBKDF2
            using (var deriveBytes = new Rfc2898DeriveBytes(
                keyMaterial,
                Encoding.UTF8.GetBytes("DailyTaskSheetSalt2025"),
                100000,
                HashAlgorithmName.SHA256))
            {
                _key = deriveBytes.GetBytes(32); // 256 bits
                _iv = deriveBytes.GetBytes(16);  // 128 bits
            }
        }

        /// <inheritdoc />
        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;

            try
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = _key;
                    aes.IV = _iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var encryptor = aes.CreateEncryptor())
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        using (var writer = new StreamWriter(cs, Encoding.UTF8))
                        {
                            writer.Write(plainText);
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Encryption failed.", ex);
            }
        }

        /// <inheritdoc />
        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return string.Empty;

            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                using (var aes = Aes.Create())
                {
                    aes.Key = _key;
                    aes.IV = _iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var decryptor = aes.CreateDecryptor())
                    using (var ms = new MemoryStream(cipherBytes))
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    using (var reader = new StreamReader(cs, Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Decryption failed.", ex);
            }
        }
    }
}
