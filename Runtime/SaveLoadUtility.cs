using System.IO;
using UnityEngine;

namespace Gameframe.SaveLoad
{
    public static class SaveLoadUtility
    {
        //Default folder name will be used if none is provided.
        private const string DefaultFolderName = "SaveLoad";
        private const string DefaultBaseFolderPath = "GameData";
        
        public static string GetSavePath(string folderName = null, string baseFolderPath = null)
        {
            return GetRuntimeSavePath(folderName, baseFolderPath);
        }
        
        public static string GetRuntimeSavePath(string folderName = null, string baseFolderPath = null)
        {
            if (string.IsNullOrEmpty(folderName))
            {
                folderName = DefaultFolderName;
            }

            if (string.IsNullOrEmpty(baseFolderPath))
            {
                baseFolderPath = DefaultBaseFolderPath;
            }

            var savePath = $"{Application.persistentDataPath}/{baseFolderPath}/";
            savePath = savePath + folderName + "/";
            return savePath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string GetSaveFileName(string fileName)
        {
            return fileName;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="saveObject"></param>
        /// <param name="serializationMethod"></param>
        /// <param name="filename"></param>
        /// <param name="folderName"></param>
        /// <param name="baseFolderPath"></param>
        public static void Save(object saveObject, ISerializationMethod serializationMethod, string filename, string folderName = null, string baseFolderPath = null)
        {
            var savePath = GetSavePath(folderName,baseFolderPath);
            var saveFilename = GetSaveFileName(filename);
            
            //Create directory if it does not exist
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            using (var saveFile = File.Create(savePath + saveFilename))
            {
                serializationMethod.Save(saveObject,saveFile);
                saveFile.Close();
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="serializationMethod"></param>
        /// <param name="filename"></param>
        /// <param name="folderName"></param>
        /// <param name="baseFolderPath"></param>
        /// <returns></returns>
        public static object Load(System.Type objectType, ISerializationMethod serializationMethod, string filename, string folderName = null, string baseFolderPath = null)
        {
            var savePath = GetSavePath(folderName, baseFolderPath);
            var saveFilename = savePath + GetSaveFileName(filename);

            object returnObject = null;

            if (!Directory.Exists(savePath) || !File.Exists(saveFilename))
            {
                return null;
            }

            using (var saveFile = File.Open(saveFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                returnObject = serializationMethod.Load(objectType, saveFile);
                saveFile.Close();
            }
            
            return returnObject;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="folderName"></param>
        /// <param name="baseFolderPath"></param>
        /// <returns></returns>
        public static bool Exists(string filename, string folderName = null, string baseFolderPath = null)
        {
            var savePath = GetSavePath(folderName, baseFolderPath);
            var saveFilename = savePath + GetSaveFileName(filename);
            return Directory.Exists(savePath) && File.Exists(saveFilename);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="folderName"></param>
        /// <param name="baseFolderPath"></param>
        public static void DeleteSavedFile(string filename, string folderName = null, string baseFolderPath = null)
        {
            var saveFilename = GetSavePath(folderName,baseFolderPath) + GetSaveFileName(filename);
            if (File.Exists(saveFilename))
            {
                File.Delete(saveFilename);
            }
        }

        public static void DeleteDirectory(string path)
        {
            var files = Directory.GetFiles(path);
            var dirs = Directory.GetDirectories(path);

            foreach (var file in files)
            {
                File.Delete(file);
            }

            foreach (var dir in dirs)
            {
                DeleteDirectory(dir);
            }
            
            Directory.Delete(path,false);
        }
    }
}


