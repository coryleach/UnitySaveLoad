using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Gameframe.SaveLoad
{
    public static class SaveLoadEncrypter
    {
        public static void Encrypt(Stream inputStream, Stream outputStream, string key, string salt)
        {
            var cryptoMethod = new RijndaelManaged();
            var cryptoKey = new Rfc2898DeriveBytes(key, Encoding.ASCII.GetBytes(salt));
            cryptoMethod.Key = cryptoKey.GetBytes(cryptoMethod.KeySize / 8);
            cryptoMethod.IV = cryptoKey.GetBytes(cryptoMethod.BlockSize / 8);
            var cryptostream = new CryptoStream(inputStream, cryptoMethod.CreateEncryptor(), CryptoStreamMode.Read);
            cryptostream.CopyTo(outputStream);
        }
        
        public static void Decrypt(Stream inputStream, Stream outputStream, string sKey, string salt)
        {
            var cryptoMethod = new RijndaelManaged();
            var cryptoKey = new Rfc2898DeriveBytes(sKey, Encoding.ASCII.GetBytes(salt));
            cryptoMethod.Key = cryptoKey.GetBytes(cryptoMethod.KeySize / 8);
            cryptoMethod.IV = cryptoKey.GetBytes(cryptoMethod.BlockSize / 8);
            var cryptostream = new CryptoStream(inputStream, cryptoMethod.CreateDecryptor(), CryptoStreamMode.Read);
            cryptostream.CopyTo(outputStream);
        }
    }
}


