using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Gameframe.SaveLoad
{
    public static class SaveLoadUtility
    {
        //Default folder name will be used if none is provided.
        private const string DefaultSubdirectory = "SaveLoad";
        private const string DefaultBaseDirectory = "GameData";

        /// <summary>
        /// Get full path to directory of save files
        /// </summary>
        /// <param name="subdirectoryName">Subdirectory to be appened to the base directory path</param>
        /// <param name="baseDirectoryName">base directory name to be used</param>
        /// <param name="streamingAssets">If true path will be constructed from Application.streamingAssetsPath otherwise Application.persistentDataPath will be used.</param>
        /// <returns>Full path to specified save directory</returns>
        public static string GetSavePath(string subdirectoryName = null, string baseDirectoryName = null, bool streamingAssets = false)
        {
            return !streamingAssets ? GetRuntimeSavePath(subdirectoryName, baseDirectoryName) : GetStreamingAssetsSavePath(subdirectoryName, baseDirectoryName);
        }

        /// <summary>
        /// Get runtime save path which is relative to Application.persistentDataPath
        /// </summary>
        /// <param name="subdirectoryName">subfolder where save files will be placed</param>
        /// <param name="baseDirectoryPath">base directory path</param>
        /// <returns>Full path relative to Application.persistentDataPath</returns>
        public static string GetRuntimeSavePath(string subdirectoryName = null, string baseDirectoryPath = null)
        {
            if (string.IsNullOrEmpty(subdirectoryName))
            {
                subdirectoryName = DefaultSubdirectory;
            }

            if (string.IsNullOrEmpty(baseDirectoryPath))
            {
                baseDirectoryPath = DefaultBaseDirectory;
            }

            var savePath = $"{Application.persistentDataPath}/{baseDirectoryPath}/";
            savePath = $"{savePath}{subdirectoryName}/";
            return savePath;
        }

        /// <summary>
        /// Get a path relative to the StreamingAssets folder
        /// </summary>
        /// <param name="subdirectoryName">Subdirectory name</param>
        /// <param name="baseDirectoryPath">base directory path</param>
        /// <returns></returns>
        public static string GetStreamingAssetsSavePath(string subdirectoryName = null, string baseDirectoryPath = null)
        {
            if (string.IsNullOrEmpty(subdirectoryName))
            {
                subdirectoryName = DefaultSubdirectory;
            }

            if (string.IsNullOrEmpty(baseDirectoryPath))
            {
                baseDirectoryPath = DefaultBaseDirectory;
            }

            var savePath = $"{Application.streamingAssetsPath}/{baseDirectoryPath}/";
            savePath = $"{savePath}{subdirectoryName}/";
            return savePath;
        }

        /// <summary>
        /// This does nothing for now, kept as a placeholder for future changes,
        /// potentially for use in sanitizing file names
        /// </summary>
        /// <param name="fileName">name of file</param>
        /// <returns>writable filename</returns>
        private static string GetSaveFileName(string fileName)
        {
            return fileName;
        }

        /// <summary>
        /// Save serializable object to file
        /// </summary>
        /// <param name="saveObject">Serializable object to be saved</param>
        /// <param name="serializationMethod">Serialization method to be used</param>
        /// <param name="filename">name of file to be saved to</param>
        /// <param name="subdirectoryName">subdirectory name in which files should be placed</param>
        /// <param name="baseDirectoryPath">base directory path</param>
        public static void Save(object saveObject, ISerializationMethod serializationMethod, string filename, string subdirectoryName = null, string baseDirectoryPath = null)
        {
            var savePath = GetSavePath(subdirectoryName,baseDirectoryPath);
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
        /// <param name="objectType">Type of object to be deserialized</param>
        /// <param name="serializationMethod">ISerializationMethod object that will perform deserialization.</param>
        /// <param name="filename">Name of file to be loaded.</param>
        /// <param name="subdirectoryName">subdirectory in which save files are placed</param>
        /// <param name="baseDirectoryPath">path to the base directory where save files will be saved</param>
        /// <param name="streamingAssets">True if path is relative to the StreamingAssets folder.</param>
        /// <returns>Object instance deserialized from file.</returns>
        public static object Load(System.Type objectType, ISerializationMethod serializationMethod, string filename, string subdirectoryName = null, string baseDirectoryPath = null, bool streamingAssets = false)
        {
            var savePath = GetSavePath(subdirectoryName, baseDirectoryPath, streamingAssets);
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
        /// <param name="subdirectoryName">subdirectory containing the save files. Appended onto base directory path.</param>
        /// <param name="baseDirectoryPath">base path</param>
        /// <param name="extension">include only files with this extension</param>
        /// <param name="streamingAssets">Will use Application.streamingAssetsPath as base path if true otherwise Application.persistentDataPath</param>
        /// <returns>list of file names</returns>
        public static IEnumerable<string> EnumerateSavedFiles(string subdirectoryName = null, string baseDirectoryPath = null, string extension = null, bool streamingAssets = false)
        {
            var savePath = GetSavePath(subdirectoryName,baseDirectoryPath,streamingAssets);

            //If directory does not exist we're done
            if (!Directory.Exists(savePath))
            {
                yield break;
            }

            var searchPattern = string.IsNullOrEmpty(extension) ? "*" : $"*.{extension}";
            foreach ( var file in Directory.EnumerateFiles(savePath,searchPattern,SearchOption.AllDirectories) )
            {
                // Skip hidden files
                var fileName = Path.GetFileName(file);
                if (fileName.StartsWith("."))
                {
                    continue;
                }
                yield return fileName;
            }
        }

        /// <summary>
        /// Creates an array list of save files in the given folder and path
        /// </summary>
        /// <param name="subdirectoryName">subdirectory containing save files. Appended onto base directory path.</param>
        /// <param name="baseDirectoryPath">base directory path</param>
        /// <param name="extension">include only files with this extension</param>
        /// <param name="streamingAssets">Will use Application.streamingAssetsPath as base path if true otherwise Application.persistentDataPath</param>
        /// <returns>Array of file names</returns>
        public static string[] GetSavedFiles(string subdirectoryName = null, string baseDirectoryPath = null, string extension = null, bool streamingAssets = false)
        {
            return EnumerateSavedFiles(subdirectoryName, baseDirectoryPath, extension, streamingAssets).ToArray();
        }

        /// <summary>
        /// Populates a given array with a list of save files in the given folder and path
        /// </summary>
        /// <param name="list">list to be populated with file names</param>
        /// <param name="subdirectoryName">subdirectory where saved files are located</param>
        /// <param name="baseDirectory">base directory path</param>
        /// <param name="extension">include only files with this extension</param>
        /// <param name="streamingAssets">Will use Application.streamingAssetsPath as base path if true otherwise Application.persistentDataPath</param>
        /// <returns>Array of file names</returns>
        public static void GetSavedFiles(List<string> list, string subdirectoryName = null, string baseDirectory = null, string extension = null, bool streamingAssets = false)
        {
            list.Clear();
            list.AddRange(EnumerateSavedFiles(subdirectoryName, baseDirectory, extension, streamingAssets));
        }

        /// <summary>
        /// Check if a saved file exists
        /// </summary>
        /// <param name="filename">name of file</param>
        /// <param name="subdirectory">subfolder of file</param>
        /// <param name="baseDirectory">base path to save file location</param>
        /// <param name="streamingAssets">True if path is relative to streaming assets.</param>
        /// <returns>True if the file exists</returns>
        public static bool Exists(string filename, string subdirectory = null, string baseDirectory = null, bool streamingAssets = false)
        {
            var savePath = GetSavePath(subdirectory, baseDirectory, streamingAssets);
            var saveFilename = savePath + GetSaveFileName(filename);
            return Directory.Exists(savePath) && File.Exists(saveFilename);
        }

        /// <summary>
        /// Delete a saved file
        /// </summary>
        /// <param name="filename">filename to be deleted</param>
        /// <param name="subdirectory">subdirectory name</param>
        /// <param name="baseDirectory">base directory path</param>
        public static void DeleteSavedFile(string filename, string subdirectory = null, string baseDirectory = null)
        {
            var saveFilename = GetSavePath(subdirectory,baseDirectory) + GetSaveFileName(filename);
            if (File.Exists(saveFilename))
            {
                File.Delete(saveFilename);
            }
        }
        
        // public static void DeleteDirectory(string path)
        // {
        //     var files = Directory.GetFiles(path);
        //     var dirs = Directory.GetDirectories(path);
        //
        //     foreach (var file in files)
        //     {
        //         File.Delete(file);
        //     }
        //
        //     foreach (var dir in dirs)
        //     {
        //         DeleteDirectory(dir);
        //     }
        //
        //     Directory.Delete(path,false);
        // }
    }
}
