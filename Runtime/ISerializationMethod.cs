using System.IO;

namespace Gameframe.SaveLoad
{
    /// <summary>
    /// Interface for a method of serializing and deserializing an object tagged with the Serializable attribute
    /// </summary>
    public interface ISerializationMethod
    {
        void Save(object savedObject, FileStream fileStream);
        object Load(System.Type savedObjectType, FileStream fileStream);
        object Copy(object copyObject);
    }
}


