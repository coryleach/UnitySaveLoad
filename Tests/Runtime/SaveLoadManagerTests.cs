using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gameframe.SaveLoad.Tests
{
    public class SaveLoadManagerTests
    {

        [Serializable]
        public class SaveLoadTestObject
        {
            public List<string> listOfStrings = new List<string>();
            public int count;
        }

        [Serializable]
        public class SaveLoadTestUnityObject : ScriptableObject
        {
            public string textValue = "";
            public Vector3 pt;
            public Quaternion rot;
        }

        [Serializable]
        public class SaveLoadDictionaryTestObject
        {
            public Dictionary<string, int> dict = new Dictionary<string, int>();
            public string name = "";
        }

        private static readonly string BaseDirectory = "GameData";
        private static readonly string SubfolderDirectory = "SaveData";
        private static readonly string TestEncryptionKey = "SaveLoadTestEncryptionKey";
        private static readonly string TestEncryptionSalt = "SaveLoadTestEncryptionSalt";

        private static SaveLoadManager CreateManager(SerializationMethodType method = SerializationMethodType.Default)
        {
            var manager = SaveLoadManager.Create(BaseDirectory,SubfolderDirectory,method,TestEncryptionKey, TestEncryptionSalt);
            if (method == SerializationMethodType.Custom)
            {
                manager.SetCustomSerializationMethod(new SerializationMethodUnityJson());
            }
            return manager;
        }

        private static void CleanupFiles()
        {
            //Cleaning up some files that were made by tests
            const string filename = "Testfile";
            var filepath = $"{SaveLoadUtility.GetSavePath(SubfolderDirectory, BaseDirectory)}/{filename}";
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
        }

        [SetUp]
        public void Setup()
        {
            CleanupFiles();
        }

        [TearDown]
        public void Teardown()
        {
            CleanupFiles();
        }

        [Test]
        public void CanCreateManager([Values] SerializationMethodType method)
        {
            var manager = CreateManager(method);
            Assert.IsTrue(manager != null);
            Object.Destroy(manager);
        }

        [Test]
        public void CanSave([Values] SerializationMethodType method)
        {
            var manager = CreateManager(method);

            var testObject = new SaveLoadTestObject()
            {
                listOfStrings = new List<string> {"one", "two"},
                count = 10,
            };

            const string filename = "Testfile";

            manager.Save(testObject,filename);

            var filepath = $"{SaveLoadUtility.GetSavePath(SubfolderDirectory, BaseDirectory)}/{filename}";
            Assert.IsTrue(File.Exists(filepath));

            manager.DeleteSave(filename);
            Assert.IsFalse(File.Exists(filepath));

            Object.Destroy(manager);
        }

        [Test]
        public void GetPath([Values] SerializationMethodType method)
        {
            var manager = CreateManager(method);
            Debug.Log(manager.GetPath("MyFile.sav"));
        }

        [Test]
        public void GetFiles([Values] SerializationMethodType method)
        {
            var manager = CreateManager(method);

            var testObject = new SaveLoadTestObject()
            {
                listOfStrings = new List<string> {"one", "two"},
                count = 10,
            };

            const string filename = "Testfile";

            manager.Save(testObject,filename);

            var filepath = $"{SaveLoadUtility.GetSavePath(SubfolderDirectory, BaseDirectory)}/{filename}";
            Assert.IsTrue(File.Exists(filepath));

            var files = manager.GetFiles();
            Assert.IsTrue(files.Length == 1,$"Expected 1 file but found {files.Length}. {files.DumpContentsToString()}");
            Assert.IsTrue(files[0] == filename);

            manager.DeleteSave(filename);
            Assert.IsFalse(File.Exists(filepath));

            Object.Destroy(manager);
        }
        
        [Test]
        public void GetFilesList([Values] SerializationMethodType method)
        {
            var manager = CreateManager(method);

            var testObject = new SaveLoadTestObject()
            {
                listOfStrings = new List<string> {"one", "two"},
                count = 10,
            };

            const string filename = "Testfile";

            manager.Save(testObject,filename);

            var filepath = $"{SaveLoadUtility.GetSavePath(SubfolderDirectory, BaseDirectory)}/{filename}";
            Assert.IsTrue(File.Exists(filepath));

            var files = new List<string>();
            manager.GetFiles(files);
            Assert.IsTrue(files.Count == 1,$"Expected 1 file but found {files.Count}");
            Assert.IsTrue(files[0] == filename);

            manager.DeleteSave(filename);
            Assert.IsFalse(File.Exists(filepath));

            Object.Destroy(manager);
        }

        [Test]
        public void GetFilesExtensionFilteringWithFilter([Values] SerializationMethodType method)
        {
            var manager = CreateManager(method);

            var testObject = new SaveLoadTestObject()
            {
                listOfStrings = new List<string> {"one", "two"},
                count = 10,
            };

            const string filename1 = "AutoSave";
            const string filename2 = "Save0001.sav";

            manager.Save(testObject,filename1);
            manager.Save(testObject,filename2);

            var filepath1 = $"{SaveLoadUtility.GetSavePath(SubfolderDirectory, BaseDirectory)}/{filename1}";
            Assert.IsTrue(File.Exists(filepath1));

            var filepath2 = $"{SaveLoadUtility.GetSavePath(SubfolderDirectory, BaseDirectory)}/{filename2}";
            Assert.IsTrue(File.Exists(filepath2));
            
            var files = new List<string>();
            manager.GetFiles(files, extension: "sav");
            Assert.IsTrue(files.Count == 1,$"Expected 1 file but found {files.Count}. {files}");
            Assert.IsTrue(files[0] == filename2);

            manager.DeleteSave(filename1);
            Assert.IsFalse(File.Exists(filepath1));

            manager.DeleteSave(filename2);
            Assert.IsFalse(File.Exists(filepath2));
            
            Object.Destroy(manager);
        }
        
        [Test]
        public void GetFilesExtensionFilteringNoFilter([Values] SerializationMethodType method)
        {
            var manager = CreateManager(method);

            var testObject = new SaveLoadTestObject()
            {
                listOfStrings = new List<string> {"one", "two"},
                count = 10,
            };

            const string filename1 = "AutoSave";
            const string filename2 = "Save0001.sav";

            manager.Save(testObject,filename1);
            manager.Save(testObject,filename2);

            var filepath1 = $"{SaveLoadUtility.GetSavePath(SubfolderDirectory, BaseDirectory)}/{filename1}";
            Assert.IsTrue(File.Exists(filepath1));

            var filepath2 = $"{SaveLoadUtility.GetSavePath(SubfolderDirectory, BaseDirectory)}/{filename2}";
            Assert.IsTrue(File.Exists(filepath2));
            
            var files = new List<string>();
            manager.GetFiles(files);
            Assert.IsTrue(files.Count == 2,$"Expected 2 files but found {files.Count}");
            Assert.IsTrue(files.Contains(filename1));
            Assert.IsTrue(files.Contains(filename2));

            manager.DeleteSave(filename1);
            Assert.IsFalse(File.Exists(filepath1));

            manager.DeleteSave(filename2);
            Assert.IsFalse(File.Exists(filepath2));
            
            Object.Destroy(manager);
        }
        
        [Test]
        public void Exists([Values] SerializationMethodType method)
        {
            var manager = CreateManager(method);

            var testObject = new SaveLoadTestObject()
            {
                listOfStrings = new List<string> {"one", "two"},
                count = 10,
            };

            const string filename1 = "AutoSave";
            const string filename2 = "Save0001.sav";

            manager.Save(testObject,filename1);
            manager.Save(testObject,filename2);

            var path1 = manager.GetPath(filename1);
            var path2 = manager.GetPath(filename2);
            
            Assert.IsTrue(File.Exists(path1));
            Assert.IsTrue(File.Exists(path2));
            
            Assert.IsTrue(manager.Exists(filename1));
            Assert.IsTrue(manager.Exists(filename2));
            
            manager.DeleteSave(filename1);
            Assert.IsFalse(manager.Exists(filename1));

            manager.DeleteSave(filename2);
            Assert.IsFalse(manager.Exists(filename2));
            
            Object.Destroy(manager);
        }

        [Test]
        public void CanSaveAndLoad([Values] SerializationMethodType method)
        {
            var manager = CreateManager(method);

            var testObject = new SaveLoadTestObject()
            {
                listOfStrings = new List<string> {"one", "two"},
                count = 10,
            };

            const string filename = "Testfile";

            manager.Save(testObject,filename);

            var filepath = $"{SaveLoadUtility.GetSavePath(SubfolderDirectory, BaseDirectory)}/{filename}";
            Assert.IsTrue(File.Exists(filepath));

            var loadedObject = manager.Load<SaveLoadTestObject>(filename);

            Assert.IsTrue(loadedObject.listOfStrings.Count == testObject.listOfStrings.Count);

            for (int i = 0; i < testObject.listOfStrings.Count; i++)
            {
                Assert.IsTrue(testObject.listOfStrings[i] == loadedObject.listOfStrings[i]);
            }

            Assert.IsTrue(testObject.count == loadedObject.count);

            manager.DeleteSave(filename);
            Assert.IsFalse(File.Exists(filepath));

            Object.Destroy(manager);
        }

        [Test]
        public void CanCopy([Values] SerializationMethodType method)
        {
            var manager = CreateManager(method);

            var testObject = new SaveLoadTestObject()
            {
                listOfStrings = new List<string> {"one", "two"},
                count = 10,
            };

            var loadedObject = manager.Copy(testObject);

            Assert.NotNull(loadedObject);
            Assert.IsFalse(ReferenceEquals(testObject,loadedObject));
            Assert.NotNull(loadedObject.listOfStrings);
            Assert.IsTrue(loadedObject.listOfStrings.Count == testObject.listOfStrings.Count);

            for (int i = 0; i < testObject.listOfStrings.Count; i++)
            {
                Assert.IsTrue(testObject.listOfStrings[i] == loadedObject.listOfStrings[i]);
            }

            Assert.IsTrue(testObject.count == loadedObject.count);

            Object.Destroy(manager);
        }

        [Test]
        public void LoadReturnsNullWhenFileDoesnotExist([Values] SerializationMethodType method)
        {
            var manager = CreateManager(method);
            const string filename = "Testfile";

            var loadedObject = manager.Load<SaveLoadTestObject>(filename);
            Assert.IsTrue(loadedObject == null);

            Object.Destroy(manager);
        }

        [Test]
        public void CanSaveUnityObject([Values] SerializationMethodType method)
        {
            const string filename = "Testfile";

            var manager = CreateManager(method);
            var testObj = ScriptableObject.CreateInstance<SaveLoadTestUnityObject>();

            testObj.textValue = "MyValue";

            manager.SaveUnityObject(testObj,filename);

            var filepath = $"{SaveLoadUtility.GetSavePath(SubfolderDirectory, BaseDirectory)}/{filename}";
            Assert.IsTrue(File.Exists(filepath));

            manager.DeleteSave(filename);
            Assert.IsFalse(File.Exists(filepath));

            Object.Destroy(testObj);
            Object.Destroy(manager);
        }

        [Test]
        public void CanSaveLoadUnityObject([Values] SerializationMethodType method)
        {
            const string filename = "Testfile";

            var manager = CreateManager(method);
            var testObj = ScriptableObject.CreateInstance<SaveLoadTestUnityObject>();
            var loadedTestObj = ScriptableObject.CreateInstance<SaveLoadTestUnityObject>();

            testObj.textValue = "MyValue";

            manager.SaveUnityObject(testObj,filename);

            var filepath = $"{SaveLoadUtility.GetSavePath(SubfolderDirectory, BaseDirectory)}/{filename}";
            Assert.IsTrue(File.Exists(filepath));

            manager.LoadUnityObjectOverwrite(loadedTestObj,filename);
            Assert.IsTrue(loadedTestObj.textValue == testObj.textValue);

            manager.DeleteSave(filename);
            Assert.IsFalse(File.Exists(filepath));

            Object.Destroy(testObj);
            Object.Destroy(loadedTestObj);
            Object.Destroy(manager);
        }

        [Test]
        public void CanCopyUnityObject([Values] SerializationMethodType method)
        {
            var manager = CreateManager(method);
            var testObj = ScriptableObject.CreateInstance<SaveLoadTestUnityObject>();
            var loadedTestObj = ScriptableObject.CreateInstance<SaveLoadTestUnityObject>();

            testObj.textValue = "MyValue";

            manager.CopyUnityObjectOverwrite(testObj,loadedTestObj);

            Assert.IsTrue(loadedTestObj.textValue == testObj.textValue);

            Object.Destroy(testObj);
            Object.Destroy(loadedTestObj);
            Object.Destroy(manager);
        }

        [Test]
        public void CopyThrowsArgumentExceptionOnUnityObject([Values] SerializationMethodType method)
        {
            var manager = CreateManager(method);
            var testObj = ScriptableObject.CreateInstance<SaveLoadTestUnityObject>();

            Assert.Throws<ArgumentException>(() =>
            {
                manager.Copy(testObj);
            });

            Object.Destroy(testObj);
            Object.Destroy(manager);
        }

        [Test]
        public void CanSaveAndLoadDictionary([Values(
            SerializationMethodType.Binary,
            SerializationMethodType.BinaryEncrypted
#if JSON_DOT_NET
            ,SerializationMethodType.JsonDotNet,
            SerializationMethodType.JsonDotNetEncrypted
#endif
            )] SerializationMethodType method)
        {
            var manager = CreateManager(method);

            var testObject = new SaveLoadDictionaryTestObject()
            {
                dict = new Dictionary<string,int>
                {
                    {"one", 1},
                    {"two", 2}
                },
                name = "Test",
            };

            const string filename = "Testfile";

            manager.Save(testObject,filename);

            var filepath = $"{SaveLoadUtility.GetSavePath(SubfolderDirectory, BaseDirectory)}/{filename}";
            Assert.IsTrue(File.Exists(filepath));

            var loadedObject = manager.Load<SaveLoadDictionaryTestObject>(filename);

            Assert.IsTrue(loadedObject.name == testObject.name);
            Assert.IsTrue(loadedObject.dict.Count == testObject.dict.Count);
            Assert.IsTrue(loadedObject.dict.ContainsKey("one"));
            Assert.IsTrue(loadedObject.dict.ContainsKey("two"));
            Assert.IsTrue(loadedObject.dict["one"] == 1);
            Assert.IsTrue(loadedObject.dict["two"] == 2);

            manager.DeleteSave(filename);
            Assert.IsFalse(File.Exists(filepath));

            Object.Destroy(manager);
        }

        
    }

    static class StringListExtensions
    {
        public static string DumpContentsToString(this IEnumerable<string> strings)
        {
            var builder = new StringBuilder();
            builder.Append("[");
            foreach (var str in strings)
            {
                builder.AppendFormat("{0} ", str);
            }
            builder.Append("]");
            return builder.ToString();
        }
    }
    
}
