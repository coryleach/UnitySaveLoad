using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Gameframe.SaveLoad
{
    public class SerializationMethodBinaryEncrypted : ISerializationMethod
    {
        private string _key;
        private string _salt;

        public SerializationMethodBinaryEncrypted(string key, string salt)
        {
            SetEncryption(key,salt);
        }
        
        public void Save(object savedObject, FileStream fileStream)
        {
            using (var serializationStream = new MemoryStream())
            {
                //Creating the formatter every time because this may get used in a task and I'm not sure if it's thread safe
                var formatter = new BinaryFormatter();
                formatter.Serialize(serializationStream, savedObject);
                serializationStream.Flush();
                serializationStream.Position = 0;
                EncryptionUtility.Encrypt(serializationStream,fileStream,_key,_salt);
            }
        }

        public object Load(Type savedObjectType, FileStream fileStream)
        {
            object loadedObj = null;

            using (var memoryStream = new MemoryStream())
            {
                EncryptionUtility.Decrypt(fileStream,memoryStream,_key,_salt);
                memoryStream.Position = 0;
                //Creating the formatter every time because this may get used in a task and I'm not sure if it's thread safe
                var formatter = new BinaryFormatter();
                loadedObj = formatter.Deserialize(memoryStream);
            }
            
            return loadedObj;
        }

        public void SetEncryption(string key, string salt)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (string.IsNullOrEmpty(salt))
            {
                throw new ArgumentNullException(nameof(salt));
            }
            
            _key = key;
            _salt = salt;
        }
        
        public object Copy(object copyObject)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, copyObject);
                stream.Position = 0;
                return formatter.Deserialize(stream);
            }
        }
    }
}


