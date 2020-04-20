using System.IO;
using UnityEngine;

namespace Gameframe.SaveLoad
{
    public static class SaveLoadUtility
    {
        //Default folder name will be used if none is provided.
        private static string defaultFolderName = "SaveLoad";
        
        private static string baseFolderPath = "/GameData/";
        public static string BaseFolderPath
        {
            get => baseFolderPath;
            set => baseFolderPath = value;
        }

        private static string GetSavePath(string folderName = null)
        {
            if (string.IsNullOrEmpty(folderName))
            {
                folderName = defaultFolderName;
            }
            
            var savePath = Application.persistentDataPath + baseFolderPath;
            
#if UNITY_EDITOR
            savePath = Application.dataPath + baseFolderPath;
#endif

            savePath = savePath + folderName + "/";
            return savePath;
        }

        private static string GetSaveFileName(string fileName)
        {
            return fileName;
        }
        
        public static void Save(object saveObject, ISaveLoadMethod saveLoadMethod, string filename, string folderName = null)
        {
            var savePath = GetSavePath(folderName);
            var saveFilename = GetSaveFileName(filename);
            
            //Create directory if it does not exist
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            var saveFile = File.Create(savePath + saveFilename);
            saveLoadMethod.Save(saveObject,saveFile);
            saveFile.Close();
        }
        
        public static object Load(System.Type objectType, ISaveLoadMethod saveLoadMethod, string filename, string folderName = null)
        {
            var savePath = GetSavePath(folderName);
            var saveFilename = savePath + GetSaveFileName(filename);

            object returnObject = null;

            if (!Directory.Exists(savePath) || !File.Exists(saveFilename))
            {
                return null;
            }

            var saveFile = File.Open(saveFilename, FileMode.Open, FileAccess.Read, FileShare.Read);
            returnObject = saveLoadMethod.Load(objectType, saveFile);
            saveFile.Close();
            
            return returnObject;
        }

        public static void Delete(string filename, string folderName = null)
        {
            var savePath = GetSavePath(folderName);
            var saveFilename = savePath + GetSaveFileName(filename);

            if (File.Exists(saveFilename))
            {
                File.Delete(saveFilename);
            }
        }
    }
}


