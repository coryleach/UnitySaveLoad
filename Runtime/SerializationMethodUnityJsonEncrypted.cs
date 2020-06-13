using System;
using System.IO;
using UnityEngine;

namespace Gameframe.SaveLoad
{
    public class SerializationMethodUnityJsonEncrypted : ISerializationMethodEncrypted
    {
        private string _key;
        private string _salt;
        
        public SerializationMethodUnityJsonEncrypted(string key, string salt)
        {
            SetEncryption(key,salt);
        }
        
        public void Save(object savedObject, FileStream fileStream)
        {
            //TODO: Using Unity's json serializer... does not support dictionaries. Do better.
            var json = JsonUtility.ToJson(savedObject);
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
                    loadedObj = JsonUtility.FromJson(json, savedObjectType);
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
    }
}


