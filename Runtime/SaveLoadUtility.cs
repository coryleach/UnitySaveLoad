using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Gameframe.SaveLoad
{
    public static class SaveLoadUtility
    {
        //Default folder name will be used if none is provided.
        private const string DefaultFolderName = "SaveLoad";
        private const string DefaultBaseFolderPath = "GameData";

        public static string GetSavePath(string folderName = null, string baseFolderPath = null, bool streamingAssets = false)
        {
            return !streamingAssets ? GetRuntimeSavePath(folderName, baseFolderPath) : GetStreamingAssetsSavePath(folderName, baseFolderPath);
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
            savePath = $"{savePath}{folderName}/";
            return savePath;
        }

        public static string GetStreamingAssetsSavePath(string folderName = null, string baseFolderPath = null)
        {
            if (string.IsNullOrEmpty(folderName))
            {
                folderName = DefaultFolderName;
            }

            if (string.IsNullOrEmpty(baseFolderPath))
            {
                baseFolderPath = DefaultBaseFolderPath;
            }

            var savePath = $"{Application.streamingAssetsPath}/{baseFolderPath}/";
            savePath = $"{savePath}{folderName}/";
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
        /// Load object from file
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="serializationMethod"></param>
        /// <param name="filename"></param>
        /// <param name="folderName"></param>
        /// <param name="baseFolderPath"></param>
        /// <param name="streamingAssets"></param>
        /// <returns></returns>
        public static object Load(System.Type objectType, ISerializationMethod serializationMethod, string filename, string folderName = null, string baseFolderPath = null, bool streamingAssets = false)
        {
            var savePath = GetSavePath(folderName, baseFolderPath, streamingAssets);
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
        /// Enumerate files in the save directory
        /// </summary>
        /// <param name="folderName">folder containing the save files</param>
        /// <param name="baseFolderPath">base path to the folder</param>
        /// <param name="extension">include only files with this extension</param>
        /// <param name="streamingAssets">Will use Application.streamingAssetsPath as base path if true otherwise Application.persistentDataPath</param>
        /// <returns>list of file names</returns>
        public static IEnumerable<string> EnumerateSavedFiles(string folderName = null, string baseFolderPath = null, string extension = null, bool streamingAssets = false)
        {
            var savePath = GetSavePath(folderName,baseFolderPath,streamingAssets);

            //If directory does not exist we're done
            if (!Directory.Exists(savePath))
            {
                yield break;
            }

            var searchPattern = string.IsNullOrEmpty(extension) ? "*" : $"*.{extension}";
            foreach ( var file in Directory.EnumerateFiles(savePath,searchPattern,SearchOption.AllDirectories) )
            {
                yield return Path.GetFileName(file);
            }
        }

        /// <summary>
        /// Creates an array list of save files in the given folder and path
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="baseFolderPath"></param>
        /// <param name="extension">include only files with this extension</param>
        /// <param name="streamingAssets">Will use Application.streamingAssetsPath as base path if true otherwise Application.persistentDataPath</param>
        /// <returns>Array of file names</returns>
        public static string[] GetSavedFiles(string folderName = null, string baseFolderPath = null, string extension = null, bool streamingAssets = false)
        {
            return EnumerateSavedFiles(folderName, baseFolderPath, extension, streamingAssets).ToArray();
        }

        /// <summary>
        /// Populates a given array with a list of save files in the given folder and path
        /// </summary>
        /// <param name="list">list to be populated with file names</param>
        /// <param name="folderName"></param>
        /// <param name="baseFolderPath"></param>
        /// <param name="extension">include only files with this extension</param>
        /// <param name="streamingAssets">Will use Application.streamingAssetsPath as base path if true otherwise Application.persistentDataPath</param>
        /// <returns>Array of file names</returns>
        public static void GetSavedFiles(List<string> list, string folderName = null, string baseFolderPath = null, string extension = null, bool streamingAssets = false)
        {
            list.Clear();
            list.AddRange(EnumerateSavedFiles(folderName, baseFolderPath, extension, streamingAssets));
        }

        /// <summary>
        /// Check if a saved file exists
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
        /// Delete a savedd file
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
