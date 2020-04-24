namespace Gameframe.SaveLoad
{
    /// <summary>
    /// Interface for an encrypted method of serializing and deserializing an object tagged with the Serializable attribute
    /// </summary>
    public interface ISerializationMethodEncrypted : ISerializationMethod
    {
        void SetEncryption(string key, string salt);
    }
}