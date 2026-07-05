namespace DailyTaskSheet.App.Interfaces
{
    /// <summary>
    /// Service interface for encrypting and decrypting sensitive data.
    /// Uses AES-256 for local credential storage.
    /// </summary>
    public interface IEncryptionService
    {
        /// <summary>Encrypts a plaintext string using AES-256.</summary>
        /// <param name="plainText">The text to encrypt.</param>
        /// <returns>The encrypted text as a Base64 string.</returns>
        string Encrypt(string plainText);

        /// <summary>Decrypts an AES-256 encrypted string.</summary>
        /// <param name="cipherText">The Base64-encoded encrypted text.</param>
        /// <returns>The decrypted plaintext string.</returns>
        string Decrypt(string cipherText);
    }
}
