using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Gameframe.SaveLoad
{
    public class SaveLoadMethodBinary : ISaveLoadMethod
    {
        public void Save(object savedObject, FileStream fileStream)
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(fileStream, savedObject);
            fileStream.Close();   
        }

        public object Load(Type savedObjectType, FileStream fileStream)
        {
            object loadedObj = null;
            var formatter = new BinaryFormatter();
            loadedObj = formatter.Deserialize(fileStream);
            fileStream.Close();
            return loadedObj;
        }
    } 
}


