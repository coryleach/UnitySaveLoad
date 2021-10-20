using System.IO;
using UnityEditor;
using UnityEngine;

namespace Gameframe.SaveLoad.Editor
{
    /// <summary>
    /// Editor for SaveLoadManager
    /// </summary>
    [CustomEditor(typeof(SaveLoadManager))]
    public class SaveLoadManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical("Box");
            base.OnInspectorGUI();
            EditorGUILayout.EndVertical();

            var manager = (SaveLoadManager) target;

            if (manager.IsEncrypted && (string.IsNullOrEmpty(manager.Key)))
            {
                EditorGUILayout.HelpBox("Encryption key cannot be null or empty.",MessageType.Error);
            }
            
            if (manager.IsEncrypted && (string.IsNullOrEmpty(manager.Salt)))
            {
                EditorGUILayout.HelpBox("Salt cannot be null or empty.",MessageType.Error);
            }

            var savePath = SaveLoadUtility.GetRuntimeSavePath(manager.DefaultFolder, manager.BaseFolder);
            
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Default Save Path");
            EditorGUILayout.LabelField(savePath);
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Open Data Folder"))
            {
                if (!Directory.Exists(savePath))
                {
                    //Create directory if it does not exist
                    Directory.CreateDirectory(savePath);
                }
                EditorUtility.RevealInFinder(savePath);
            }
        }
    }
}