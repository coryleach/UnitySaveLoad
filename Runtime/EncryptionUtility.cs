using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Gameframe.SaveLoad
{
    public static class EncryptionUtility
    {
        /// <summary>
        /// Encrypt an input stream
        /// </summary>
        /// <param name="inputStream">Stream to be encrypted</param>
        /// <param name="outputStream">Stream ecrypted data will be output to</param>
        /// <param name="key">Encryption Key</param>
        /// <param name="salt">Encryption Salt</param>
        public static void Encrypt(Stream inputStream, Stream outputStream, string key, string salt)
        {
            var cryptoMethod = new RijndaelManaged();
            var cryptoKey = new Rfc2898DeriveBytes(key, Encoding.ASCII.GetBytes(salt));
            cryptoMethod.Key = cryptoKey.GetBytes(cryptoMethod.KeySize / 8);
            cryptoMethod.IV = cryptoKey.GetBytes(cryptoMethod.BlockSize / 8);
            using (var cryptostream = new CryptoStream(inputStream, cryptoMethod.CreateEncryptor(), CryptoStreamMode.Read))
            {
                cryptostream.CopyTo(outputStream);
            }
        }
        
        /// <summary>
        /// Decrypt an input stream to an output stream.
        /// </summary>
        /// <param name="inputStream">Stream with encrypted bytes to be read from</param>
        /// <param name="outputStream">Stream that decrypted bytes will be written to</param>
        /// <param name="key">Encryption Key</param>
        /// <param name="key">Encryption Salt</param>
        public static void Decrypt(Stream inputStream, Stream outputStream, string key, string salt)
        {
            var cryptoMethod = new RijndaelManaged();
            var cryptoKey = new Rfc2898DeriveBytes(key, Encoding.ASCII.GetBytes(salt));
            cryptoMethod.Key = cryptoKey.GetBytes(cryptoMethod.KeySize / 8);
            cryptoMethod.IV = cryptoKey.GetBytes(cryptoMethod.BlockSize / 8);
            using (var cryptostream = new CryptoStream(inputStream, cryptoMethod.CreateDecryptor(), CryptoStreamMode.Read))
            {
                cryptostream.CopyTo(outputStream);
            }
        }
    }
}


