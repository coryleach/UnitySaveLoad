namespace Gameframe.SaveLoad
{
    public enum SerializationMethodType
    {
        Default = 0,

        Binary = 1,
        BinaryEncrypted = 101,

        UnityJson = 2,
        UnityJsonEncrypted = 102,
        
#if JSON_DOT_NET
        JsonDotNet = 3,
        JsonDotNetEncrypted = 103,
#endif
        
        Custom = 1000
    }
}