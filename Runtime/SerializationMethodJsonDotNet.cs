#if JSON_DOT_NET

using System;
using System.IO;
using Newtonsoft.Json;

namespace Gameframe.SaveLoad
{
    public class SerializationMethodJsonDotNet : ISerializationMethod
    {
        public void Save(object savedObject, FileStream fileStream)
        {
            var json = JsonConvert.SerializeObject(savedObject);
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
                loadedObj = JsonConvert.DeserializeObject(json,savedObjectType);
            }
            
            return loadedObj;
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