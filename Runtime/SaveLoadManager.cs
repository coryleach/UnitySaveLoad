using UnityEngine;

namespace Gameframe.SaveLoad
{
    public enum SaveLoadMethodType
    {
        Default = 0,
        Binary = 1,
        Json = 2
    }
    
    [CreateAssetMenu(menuName = "")]
    public class SaveLoadManager : ScriptableObject
    {
        [Header("Directories"),SerializeField] private string defaultFolder = "SaveData";
        [SerializeField] private string baseFolder = "/GameData/";

        [Header("Encryption"),SerializeField] protected bool encrypted = false;
        [SerializeField] protected string key = string.Empty;
        [SerializeField] protected string salt = string.Empty;

        [SerializeField] private SaveLoadMethodType saveMethod = SaveLoadMethodType.Default;

        public ISaveLoadMethod GetSaveLoadMethod()
        {
            return null;
        }
        
        public void Save(object obj, string filename, string folder = null)
        {
            
        }

        public object Load<T>(string filename, string folder = null)
        {
            return (T)Load(typeof(T), filename, folder);
        }

        public object Load(System.Type type, string filename, string folder = null)
        {
            return null;
        }
    }    
}


