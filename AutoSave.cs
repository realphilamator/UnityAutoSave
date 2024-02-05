using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;
using System;

[InitializeOnLoad]
public class AutoSave : MonoBehaviour
{
    private static string outputFolder = "Assets/Autosaves";
    private static double lastAutoSaveTime = 0;
    public static double autoSaveInterval = 1000;

    static AutoSave()
    {
        EditorApplication.update += OnEditorUpdate;

        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }
    }

    private static void OnEditorUpdate()
    {
        if (EditorSceneManager.GetActiveScene().isDirty)
        {
            double currentTime = EditorApplication.timeSinceStartup;

            if (currentTime - lastAutoSaveTime > autoSaveInterval)
            {
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

                string sceneName = Path.GetFileNameWithoutExtension(EditorSceneManager.GetActiveScene().path);
                string sceneFolder = Path.Combine(autosavesRootFolder, sceneName);

                if (!Directory.Exists(sceneFolder))
                {
                    Directory.CreateDirectory(sceneFolder);
                }

                string autosaveFileName = $"{sceneName}.autosave.{GetNextAutoSaveNumber()}.unity";
                string autosaveFilePath = Path.Combine(sceneFolder, autosaveFileName);

                File.Copy(EditorSceneManager.GetActiveScene().path, autosaveFilePath, true);
                Debug.Log($"Autosaved: {autosaveFilePath} at {System.DateTime.Now}");
                lastAutoSaveTime = currentTime;
                DeleteOldAutosaves(sceneFolder);
            }
        }
    }

    private static void DeleteOldAutosaves(string sceneFolder)
    {
        string[] autosaveFiles = Directory.GetFiles(sceneFolder, "*.autosave.*.unity");

        if (autosaveFiles.Length > 10)
        {
            Array.Sort(autosaveFiles, (a, b) => File.GetCreationTime(a).CompareTo(File.GetCreationTime(b)));

            int numToDelete = autosaveFiles.Length - 10;

            for (int i = 0; i < numToDelete; i++)
            {
                string unityFile = autosaveFiles[i];
                string metaFile = unityFile + ".meta";

                if (File.Exists(unityFile))
                {
                    File.Delete(unityFile);
                    Debug.Log($"Deleted autosave file: {unityFile}");
                }

                if (File.Exists(metaFile))
                {
                    File.Delete(metaFile);
                    Debug.Log($"Deleted meta file: {metaFile}");
                }
            }
        }
    }

    private static int GetNextAutoSaveNumber()
    {
        string[] existingSaves = Directory.GetFiles(autosavesRootFolder, "*.autosave.*.unity");
        return existingSaves.Length + 1;
    }
}

}
