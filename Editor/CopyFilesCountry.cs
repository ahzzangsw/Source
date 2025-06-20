#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class CopyFilesCountry : EditorWindow
{
    private bool isRunning = false;

    [MenuItem("Extract Tools/FileCopyCountry")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CopyFilesCountry), false, "FileCopyCountry Extract");
    }

    void OnGUI()
    {
        GUILayout.Space(10);

        if (!isRunning && GUILayout.Button("Run Process"))
        {
            isRunning = true;
            CopyFiles();
            isRunning = false;
        }

        GUILayout.Space(20);

        if (GUILayout.Button("Close Window"))
        {
            this.Close();
        }
    }

    private void CopyFiles()
    {
        string[] copyNameList = new string[] { "arabic", "bulgarian", "schinese", "tchinese", "czech", "danish", "dutch", "english", "finnish"
            , "french", "german", "greek", "hungarian", "Indonesia", "italian", "japanese", "koreana", "norwegian", "polish", "portuguese"
            , "brazilian", "romanian", "russian", "spanish", "latam", "swedish", "thai", "turkish", "ukrainian", "vietnamese" };

        string originFile = "Header";
        string fileType = ".jpg";

        try
        {
            string sourceFilePath = Path.Combine("E:\\Work\\Steam\\Ver3.0", originFile + fileType);
            for (int i = 0; i < copyNameList.Length; ++i)
            {
                string targetFilePath = "E:\\Work\\Steam\\Ver3.0\\Copy\\" + originFile + "_" + copyNameList[i] + fileType;
                File.Copy(sourceFilePath, targetFilePath, true);
            }

            EditorUtility.DisplayDialog("Complete", "", "OK");
        }
        catch
        {
            EditorUtility.DisplayDialog("Error", "", "OK");
        }
    }
}
#endif