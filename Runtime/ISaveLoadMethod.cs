using System.IO;

namespace Gameframe.SaveLoad
{
    public interface ISaveLoadMethod
    {
        void Save(object savedObject, FileStream fileStream);
        object Load(System.Type savedObjectType, FileStream fileStream);
    }
}


