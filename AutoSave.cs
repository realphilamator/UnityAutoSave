using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;
using System;

[InitializeOnLoad]
public class AutoSave : MonoBehaviour
{
    public static string outputFolder = "Assets/Autosaves";
    public static double lastAutoSaveTime = 0;
    public static double autoSaveInterval = 1000;
    public static int maxAutoFiles = 5;

    [MenuItem("Tools/AutoSave/Configure AutoSave")]
    private static void ConfigureAutoSave()
    {
        AutoSaveConfigWindow.Init();
    }

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
                string sceneFolder = Path.Combine(outputFolder, sceneName);

                if (!Directory.Exists(sceneFolder))
                {
                    Directory.CreateDirectory(sceneFolder);
                }

                string autosaveFileName = $"{sceneName}.autosave.{GetNextAutoSaveNumber(sceneFolder)}.unity";
                string autosaveFilePath = Path.Combine(sceneFolder, autosaveFileName);

                File.Copy(EditorSceneManager.GetActiveScene().path, autosaveFilePath, true);
                lastAutoSaveTime = currentTime;
                DeleteOldAutosaves(sceneFolder);
            }
        }
    }

    private static void DeleteOldAutosaves(string sceneFolder)
    {
        string[] autosaveFiles = Directory.GetFiles(sceneFolder, "*.autosave.*.unity");

        if (autosaveFiles.Length > maxAutoFiles)
        {
            Array.Sort(autosaveFiles, (a, b) => File.GetCreationTime(a).CompareTo(File.GetCreationTime(b)));

            int numToDelete = autosaveFiles.Length - maxAutoFiles;

            for (int i = 0; i < numToDelete; i++)
            {
                string unityFile = autosaveFiles[i];
                string metaFile = unityFile + ".meta";

                if (File.Exists(unityFile))
                {
                    File.Delete(unityFile);
                }

                if (File.Exists(metaFile))
                {
                    File.Delete(metaFile);
                }
            }
        }
    }

    private static int GetNextAutoSaveNumber(string sceneFolder)
    {
        string[] existingSaves = Directory.GetFiles(sceneFolder, "*.autosave.*.unity");
        return existingSaves.Length + 1;
    }
}

public class AutoSaveConfigWindow : EditorWindow
{
    public static void Init()
    {
        AutoSaveConfigWindow window = (AutoSaveConfigWindow)EditorWindow.GetWindow(typeof(AutoSaveConfigWindow));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("AutoSave Configuration", EditorStyles.boldLabel);
        AutoSave.outputFolder = EditorGUILayout.TextField("Output Folder", AutoSave.outputFolder);
        AutoSave.autoSaveInterval = EditorGUILayout.DoubleField("AutoSave Interval", AutoSave.autoSaveInterval);
        AutoSave.maxAutoFiles = EditorGUILayout.IntField("Max Auto Files", AutoSave.maxAutoFiles);

        if (GUILayout.Button("Apply Changes"))
        {
            if (!Directory.Exists(AutoSave.outputFolder))
            {
                Directory.CreateDirectory(AutoSave.outputFolder);
            }

            Close();
        }
    }
}