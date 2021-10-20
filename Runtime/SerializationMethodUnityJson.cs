using System;
using System.IO;
using UnityEngine;

namespace Gameframe.SaveLoad
{
    public class SerializationMethodUnityJson : ISerializationMethod
    {
        public void Save(object savedObject, FileStream fileStream)
        {
            //TODO: Using Unity's json serializer... does not support dictionaries. Do better.
            var json = JsonUtility.ToJson(savedObject);
            using (var streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.Write(json);
                streamWriter.Close();
            }
        }

        public object Load(Type savedObjectType, FileStream fileStream)
        {
            object loadedObj = null;
            
            using (var streamReader = new StreamReader(fileStream))
            {
                var json = streamReader.ReadToEnd();
                streamReader.Close();
                loadedObj = JsonUtility.FromJson(json, savedObjectType);
            }
            
            return loadedObj;
        }
        
        public object Copy(object copyObject)
        {
            using (var stream = new MemoryStream())
            {
                var writeJson = JsonUtility.ToJson(copyObject);
                var streamWriter = new StreamWriter(stream);
                streamWriter.Write(writeJson);
                streamWriter.Flush();

                stream.Position = 0;
                
                using (var streamReader = new StreamReader(stream))
                {
                    var readJson = streamReader.ReadToEnd();
                    streamReader.Close();
                    return JsonUtility.FromJson(readJson, copyObject.GetType());
                }
            }
        }

    }
}


