using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Gameframe.SaveLoad
{
    public class SerializationMethodBinary : ISerializationMethod
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="savedObject"></param>
        /// <param name="fileStream"></param>
        public void Save(object savedObject, FileStream fileStream)
        {
            //Creating the formatter every time because this may get used in a task and I'm not sure if it's thread safe
            var formatter = new BinaryFormatter();
            formatter.Serialize(fileStream, savedObject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="savedObjectType"></param>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public object Load(Type savedObjectType, FileStream fileStream)
        {
            object loadedObj = null;
            //Creating the formatter every time because this may get used in a task and I'm not sure if it's thread safe
            var formatter = new BinaryFormatter();
            loadedObj = formatter.Deserialize(fileStream);
            return loadedObj;
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


