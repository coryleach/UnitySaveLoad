#if JSON_DOT_NET

using System;
using System.IO;
using Newtonsoft.Json;

namespace Gameframe.SaveLoad
{
    public class SerializationMethodJsonDotNetEncrypted : ISerializationMethodEncrypted
    {
        private string _key = null;
        private string _salt = null;
        
        public SerializationMethodJsonDotNetEncrypted(string key, string salt)
        {
            SetEncryption(key,salt);
        }
        
        public void Save(object savedObject, FileStream fileStream)
        {
            //TODO: Using Unity's json serializer... does not support dictionaries. Do better.
            var json = JsonConvert.SerializeObject(savedObject);
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    memoryStream.Position = 0;
                    EncryptionUtility.Encrypt(memoryStream,fileStream,_key,_salt);
                }
            }
        }

        public object Load(Type savedObjectType, FileStream fileStream)
        {
            object loadedObj = null;

            using (var memoryStream = new MemoryStream())
            {
                EncryptionUtility.Decrypt(fileStream, memoryStream, _key, _salt);
                memoryStream.Position = 0;
                using (var streamReader = new StreamReader(memoryStream))
                {
                    var json = streamReader.ReadToEnd();
                    loadedObj = JsonConvert.DeserializeObject(json, savedObjectType);
                    streamReader.Close();
                }
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
                var writeJson = JsonConvert.SerializeObject(copyObject);
            
                var streamWriter = new StreamWriter(stream);
                streamWriter.Write(writeJson);
                streamWriter.Flush();

                stream.Position = 0;

                using (var streamReader = new StreamReader(stream))
                {
                    var readJson = streamReader.ReadToEnd();
                    return JsonConvert.DeserializeObject(readJson, copyObject.GetType());
                }
            }
        }

    }
}

#endif