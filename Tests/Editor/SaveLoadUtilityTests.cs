using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gameframe.SaveLoad.Tests
{
    public class SaveLoadUtilityTests
    {
        private string TestKey = "TestKey";
        private string TestSalt = "TestSalt";

        private ISerializationMethod GetSerializationMethod(SerializationMethodType method)
        {
            switch (method)
            {
                case SerializationMethodType.Default:
                    return new SerializationMethodUnityJson();
                case SerializationMethodType.Binary:
                    return new SerializationMethodBinary();
                case SerializationMethodType.BinaryEncrypted:
                    return new SerializationMethodBinaryEncrypted(TestKey,TestSalt);
                case SerializationMethodType.UnityJson:
                    return new SerializationMethodUnityJson();
                case SerializationMethodType.UnityJsonEncrypted:
                    return new SerializationMethodUnityJsonEncrypted(TestKey,TestSalt);
#if JSON_DOT_NET
                case SerializationMethodType.JsonDotNet:
                    return new SerializationMethodJsonDotNet();
                case SerializationMethodType.JsonDotNetEncrypted:
                    return new SerializationMethodJsonDotNetEncrypted(TestKey,TestSalt);
#endif
                case SerializationMethodType.Custom:
                    return new SerializationMethodBinary();
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
        }

        [Serializable]
        public class SaveLoadTestObject
        {
            public string testData;
        }

        [Test]
        public void SaveLoadAndDelete([Values]SerializationMethodType method)
        {
            var testSave = new SaveLoadTestObject() {testData = "SaveFileExists"};
            var serializationMethod = GetSerializationMethod(method);
            var filename = "TestSave.sav";

            SaveLoadUtility.Save(testSave,serializationMethod,filename);

            Assert.IsTrue(SaveLoadUtility.Exists(filename));

            var loadedSave = (SaveLoadTestObject)SaveLoadUtility.Load(typeof(SaveLoadTestObject), serializationMethod, filename);
            Assert.NotNull(loadedSave);
            Assert.IsTrue(loadedSave.testData == testSave.testData);

            SaveLoadUtility.DeleteSavedFile(filename);

            Assert.IsFalse(SaveLoadUtility.Exists(filename));
        }

        [Test]
        public void CanGetFiles_Empty()
        {
            var files = SaveLoadUtility.GetSavedFiles();
            Assert.IsTrue(files.Length == 0);
        }

        [Test]
        public void CanGetFiles()
        {
            var testSave = new SaveLoadTestObject() {testData = "SaveFileExists"};
            var serializationMethod = GetSerializationMethod(SerializationMethodType.Binary);
            var filename = "TestSave.sav";
            var folder = "TestFolder";

            SaveLoadUtility.Save(testSave,serializationMethod,filename,folder);

            var files = SaveLoadUtility.GetSavedFiles(folder);
            Assert.IsTrue(files.Length == 1,$"Total Save Files: {files.Length} Expected 1");

            //Files should contain a list of names that exactly match the file name used
            //omits the path of the file
            Assert.IsTrue(files[0] == filename);

            SaveLoadUtility.DeleteSavedFile(filename,folder);

            files = SaveLoadUtility.GetSavedFiles();
            Assert.IsTrue(files.Length == 0);
        }

        [Test]
        public void CanGetFilesWithExtension()
        {
            var testSave = new SaveLoadTestObject() {testData = "SaveFileExists"};
            var serializationMethod = GetSerializationMethod(SerializationMethodType.Binary);
            var filename = "TestSave.sav";
            var folder = "TestFolder";

            SaveLoadUtility.Save(testSave,serializationMethod,filename,folder);

            var files = SaveLoadUtility.GetSavedFiles(folder,null, "sav");
            Assert.IsTrue(files.Length == 1);

            //Files should contain a list of names that exactly match the file name used
            //omits the path of the file
            Assert.IsTrue(files[0] == filename);

            SaveLoadUtility.DeleteSavedFile(filename,folder);

            files = SaveLoadUtility.GetSavedFiles();
            Assert.IsTrue(files.Length == 0);
        }

        [TearDown]
        public void TearDown()
        {
            var path = SaveLoadUtility.GetSavePath();
            string[] files = {"TestSave.sav"};

            foreach (var file in files)
            {
                var filename = path + file;
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }
            }
        }

    }

}
