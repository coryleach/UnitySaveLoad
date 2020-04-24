using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameframe.SaveLoad
{
    public enum SerializationMethodType
    {
        Default = 0,
        Binary = 1,
        Json = 2,
        BinaryEncrypted = 101,
        JsonEncrypted = 102
    }
}