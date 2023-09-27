using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameframe.SaveLoad
{
    /// <summary>
    /// SaveLoadManager
    /// Manager for saving and loading objects to/from disk.
    /// </summary>
    [CreateAssetMenu(menuName = "Gameframe/SaveLoad/SaveLoadManager")]
    public class SaveLoadManager : ScriptableObject
    {
        [Header("Settings"),SerializeField] private string defaultFolder = "SaveData";
        public string DefaultFolder => defaultFolder;

        [SerializeField] private string baseFolder = "GameData";
        public string BaseFolder => baseFolder;

        [SerializeField] private SerializationMethodType saveMethod = SerializationMethodType.Default;

        [Header("Encryption"),SerializeField] protected string key = string.Empty;
        public string Key => key;

        [SerializeField] protected string salt = string.Empty;
        public string Salt => salt;

        private Dictionary<SerializationMethodType, ISerializationMethod> _methods;

        private void OnEnable()
        {
            //OnEnabled will be called when entering play mode in editor but also when selecting the object in editor
            //Constructor may only be called once which may lead to some weird behaviour if we use it for initializing this dictionary
            //Using OnEnabled ensures we do this initialization and the dictionary is fresh when we hit the play button in editor
            _methods = new Dictionary<SerializationMethodType, ISerializationMethod>();
        }

        /// <summary>
        /// Helper Method for constructing a new instance of SaveLoadManager which specific protected settings
        /// </summary>
        /// <param name="baseFolder">Base directory folder</param>
        /// <param name="defaultFolder">Default folder to save files to</param>
        /// <param name="saveMethod">Method to use to save and load files</param>
        /// <param name="key">Encryption key is required if using an encrypted method.</param>
        /// <param name="salt">Encryption salt is required if using an ecrypted method.</param>
        /// <returns>Newly created instance of SaveLoadManager</returns>
        public static SaveLoadManager Create(string baseFolder, string defaultFolder, SerializationMethodType saveMethod, string key = null, string salt = null)
        {
            var instance = CreateInstance<SaveLoadManager>();

            instance.baseFolder = baseFolder;
            instance.defaultFolder = defaultFolder;
            instance.key = key;
            instance.salt = salt;
            instance.saveMethod = saveMethod;

            return instance;
        }

        /// <summary>
        /// Save an object to disk
        /// </summary>
        /// <param name="obj">object to be saved</param>
        /// <param name="filename">Name of file that will be written to</param>
        /// <param name="folder">Name of the folder that should contain the file</param>
        public void Save(object obj, string filename, string folder = null)
        {
            if (string.IsNullOrEmpty(folder))
            {
                folder = defaultFolder;
            }
            var saveLoadMethod = GetSaveLoadMethod(saveMethod);
            SaveLoadUtility.Save(obj,saveLoadMethod,filename,folder, baseFolder);
        }

        /// <summary>
        /// Gets the list of save files that have been created
        /// </summary>
        /// <param name="folder">sub folder</param>
        /// <param name="extension"></param>
        /// <param name="streamingAssets">Will use Application.streamingAssetsPath as base path if true otherwise Application.persistentDataPath</param>
        /// <returns>list of file names (excludes the path)</returns>
        public string[] GetFiles(string folder = null, string extension = null, bool streamingAssets = false)
        {
            if (string.IsNullOrEmpty(folder))
            {
                folder = defaultFolder;
            }
            return SaveLoadUtility.GetSavedFiles(folder,baseFolder, extension, streamingAssets);
        }

        /// <summary>
        /// Gets the list of save files that have been created
        /// </summary>
        /// <param name="list">list to be populated with file names</param>
        /// <param name="folder">sub folder</param>
        /// <param name="extension"></param>
        /// <param name="streamingAssets">Will use Application.streamingAssetsPath as base path if true otherwise Application.persistentDataPath</param>
        /// <returns>list of file names (excludes the path)</returns>
        public void GetFiles(List<string> list, string folder = null, string extension = null, bool streamingAssets = false)
        {
            if (string.IsNullOrEmpty(folder))
            {
                folder = defaultFolder;
            }

            SaveLoadUtility.GetSavedFiles(list, folder,baseFolder, extension, streamingAssets);
        }

        /// <summary>
        /// Creat a copy of an object by serializing and deserializing it.
        /// Not compatible with unity objects.
        /// </summary>
        /// <param name="obj">object to be copied</param>
        /// <returns>duplicated instance</returns>
        public object Copy(object obj)
        {
            if (obj is UnityEngine.Object)
            {
                throw new ArgumentException("UnityEngine.Object and child types not supported by copy method.");
            }
            var saveLoadMethod = GetSaveLoadMethod(saveMethod);
            return saveLoadMethod.Copy(obj);
        }

        /// <summary>
        /// Creat a copy of an object by serializing and deserializing it.
        /// Not compatible with Unity objects.
        /// </summary>
        /// <param name="obj">object to be copied</param>
        /// <typeparam name="T">Type of object to be copied.</typeparam>
        /// <returns>duplicated instance</returns>
        public T Copy<T>(T obj)
        {
            if (obj is UnityEngine.Object)
            {
                throw new ArgumentException("UnityEngine.Object and child types not supported by copy method.");
            }
            var saveLoadMethod = GetSaveLoadMethod(saveMethod);
            return (T)saveLoadMethod.Copy(obj);
        }

        /// <summary>
        /// Load an object from disk
        /// </summary>
        /// <param name="filename">Name of file to load from</param>
        /// <param name="folder">Name of folder containing the file</param>
        /// <param name="streamingAssets">Load file from streaming assets</param>
        /// <typeparam name="T">Type of object to be loaded from file</typeparam>
        /// <returns>Instance of object loaded from file</returns>
        public T Load<T>(string filename, string folder = null, bool streamingAssets = false)
        {
            return (T)Load(typeof(T), filename, folder, streamingAssets);
        }

        /// <summary>
        /// Load an object from disk
        /// </summary>
        /// <param name="type">Type of object to be loaded</param>
        /// <param name="filename">Name of file to load object from</param>
        /// <param name="folder">Name of folder containing the file to be loaded</param>
        /// <param name="streamingAssets">Load file from streaming assets</param>
        /// <returns>Instance of object to be loaded. Null if file did not exist.</returns>
        public object Load(Type type, string filename, string folder = null, bool streamingAssets = false)
        {
            if (string.IsNullOrEmpty(folder))
            {
                folder = defaultFolder;
            }
            var saveLoadMethod = GetSaveLoadMethod(saveMethod);
            return SaveLoadUtility.Load(type, saveLoadMethod,filename,folder, baseFolder, streamingAssets);
        }

        /// <summary>
        /// Delete saved file from disk
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="folder"></param>
        public void DeleteSave(string filename, string folder = null)
        {
            if (string.IsNullOrEmpty(folder))
            {
                folder = defaultFolder;
            }
            SaveLoadUtility.DeleteSavedFile(filename,folder, baseFolder);
        }

        /// <summary>
        /// Save object to file and specify the method of save/load
        /// </summary>
        /// <param name="methodType">Method to be used to save the file to disk.</param>
        /// <param name="obj">Object to be written to disk.</param>
        /// <param name="filename">Name of file to write to.</param>
        /// <param name="folder">Name of folder to save to. If null the default folder will be used.</param>
        public void SaveWithMethod(SerializationMethodType methodType, object obj, string filename, string folder = null)
        {
            var saveLoadMethod = GetSaveLoadMethod(methodType);
            SaveLoadUtility.Save(obj,saveLoadMethod,filename,folder);
        }

        /// <summary>
        /// Load object from file and specify the method of save/load
        /// </summary>
        /// <param name="methodType">Method to be used to save the file to disk.</param>
        /// <param name="filename">Name of file to be read from.</param>
        /// <param name="folder">Name of the folder containing the file.</param>
        /// <typeparam name="T">Type of object to be loaded from the file.</typeparam>
        /// <returns>Object instance loaded from file. Null if file does not exist or load failed.</returns>
        public object LoadWithMethod<T>(SerializationMethodType methodType, string filename, string folder = null)
        {
            return (T)LoadWithMethod(methodType, typeof(T), filename, folder);
        }

        /// <summary>
        /// Load object from file and specify the method of save/load
        /// </summary>
        /// <param name="methodType">Method to be used to save the file to disk.</param>
        /// <param name="filename">Name of file to be read from.</param>
        /// <param name="folder">Name of the folder containing the file.</param>
        /// <typeparam name="T">Type of object to be loaded from the file.</typeparam>
        /// <returns>Object instance loaded from file. Null if file does not exist or load failed.</returns>
        public object LoadWithMethod(SerializationMethodType methodType, Type type, string filename, string folder = null)
        {
            var saveLoadMethod = GetSaveLoadMethod(methodType);
            return SaveLoadUtility.Load(type, saveLoadMethod,filename,folder);
        }

        /// <summary>
        /// Saves a unity object to a file.
        /// First converts the object to JSON using the Unity Json utility.
        /// Then wraps the json string in an object so that object can be serialized with the normal methods.
        /// </summary>
        /// <param name="unityObj">Object to be saved.</param>
        /// <param name="filename">Name of file to save to</param>
        /// <param name="folder">Name of folder to save the file in. Uses defualt folder when null.</param>
        public void SaveUnityObject(UnityEngine.Object unityObj, string filename, string folder = null)
        {
            var savedObj = new JsonSerializedUnityObject
            {
                jsonData = JsonUtility.ToJson(unityObj)
            };

            Save(savedObj,filename,folder);
        }

        /// <summary>
        /// Loads from file and overwrites the given object with the saved values.
        /// File must have been saved with SaveUnityObject method.
        /// Uses Unity's JsonUtility and has the same limitations.
        /// </summary>
        /// <param name="objectToOverwrite">Object onto which the values from file will be written</param>
        /// <param name="filename">Name of file to read from.</param>
        /// <param name="folder">Name of folder containing file. If null the default folder will be used.</param>
        /// <returns>True when something is loaded. False if file or data was not found.</returns>
        public bool LoadUnityObjectOverwrite(UnityEngine.Object objectToOverwrite, string filename, string folder = null)
        {
            var savedObj = Load<JsonSerializedUnityObject>(filename, folder);

            if (savedObj == null || string.IsNullOrEmpty(savedObj.jsonData))
            {
                return false;
            }

            JsonUtility.FromJsonOverwrite(savedObj.jsonData,objectToOverwrite);
            return true;
        }

        /// <summary>
        /// Copies the serializable fields from one UnityEngine.Object to another
        /// </summary>
        /// <param name="toCopy">object which should be copied</param>
        /// <param name="toOverwrite">object onto which copied fields should be written</param>
        public void CopyUnityObjectOverwrite(UnityEngine.Object toCopy, UnityEngine.Object toOverwrite)
        {
            var jsonData = JsonUtility.ToJson(toCopy);
            JsonUtility.FromJsonOverwrite(jsonData,toOverwrite);
        }

        /// <summary>
        /// JsonSerializedUnityObject
        /// Wrapper for json data created when using Unity's JsonUtility to serialize an object derived from UnityEngine.Object
        /// This allows us to bypass some of the limitations of BinaryFormatter and serializing unity types such as Vector3, Quaternion etc.
        /// Many Unity structs are oddly not marked as serializable.
        /// </summary>
        [Serializable]
        private class JsonSerializedUnityObject
        {
            public string jsonData;
        }

        public bool IsEncrypted => (saveMethod == SerializationMethodType.BinaryEncrypted || saveMethod == SerializationMethodType.UnityJsonEncrypted);

        /// <summary>
        /// Sets a custom serialization method
        /// </summary>
        /// <param name="customSerializationMethod"></param>
        public void SetCustomSerializationMethod(ISerializationMethod customSerializationMethod)
        {
            if (_methods == null)
            {
                _methods = new Dictionary<SerializationMethodType, ISerializationMethod>();
            }
            _methods[SerializationMethodType.Custom] = customSerializationMethod;
        }

        private ISerializationMethod GetSaveLoadMethod(SerializationMethodType methodType)
        {
            if (_methods == null)
            {
                _methods = new Dictionary<SerializationMethodType, ISerializationMethod>();
            }

            if (_methods.TryGetValue(methodType, out var method))
            {
                return method;
            }

            //Create method if it did not yet exist
            switch (methodType)
            {
                case SerializationMethodType.Default:
                    method = GetSaveLoadMethod(SerializationMethodType.UnityJson);
                    break;
                case SerializationMethodType.Binary:
                    method = new SerializationMethodBinary();
                    break;
                case SerializationMethodType.UnityJson:
                    method = new SerializationMethodUnityJson();
                    break;

                case SerializationMethodType.BinaryEncrypted:
                    method = new SerializationMethodBinaryEncrypted(key,salt);
                    break;
                case SerializationMethodType.UnityJsonEncrypted:
                    method = new SerializationMethodUnityJsonEncrypted(key,salt);
                    break;

#if JSON_DOT_NET
                case SerializationMethodType.JsonDotNet:
                    method = new SerializationMethodJsonDotNet();
                    break;
                case SerializationMethodType.JsonDotNetEncrypted:
                    method = new SerializationMethodJsonDotNetEncrypted(key,salt);
                    break;
#endif

                case SerializationMethodType.Custom:
                    throw new MissingComponentException("SerializationMethodType is Custom but no custom ISerializationMethod was found.");
                default:
                    throw new ArgumentOutOfRangeException(nameof(methodType), methodType, "SaveLoadMethodType not supported");
            }

            _methods[methodType] = method;

            return method;
        }

    }
}
