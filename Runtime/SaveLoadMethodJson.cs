using System;
using System.IO;
using UnityEngine;

namespace Gameframe.SaveLoad
{
    public class SaveLoadMethodJson : ISaveLoadMethod
    {
        public void Save(object savedObject, FileStream fileStream)
        {
            //TODO: Using Unity's json serializer... does not support dictionaries. Do better.
            var json = JsonUtility.ToJson(savedObject);
            var streamWriter = new StreamWriter(fileStream);
            streamWriter.Write(json);
            streamWriter.Close();
            fileStream.Close();
        }

        public object Load(Type savedObjectType, FileStream fileStream)
        {
            object loadedObj = null;
            var streamReader = new StreamReader(fileStream);
            var json = streamReader.ReadToEnd();
            streamReader.Close();
            fileStream.Close();
            loadedObj = JsonUtility.FromJson(json, savedObjectType);
            return loadedObj;
        }
    }
}


