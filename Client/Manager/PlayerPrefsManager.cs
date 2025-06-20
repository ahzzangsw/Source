using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{
    private string m_SaveFilename = "save.json";
    void Awake()
    {
        //DontDestroyOnLoad(gameObject);

        Application.targetFrameRate = 90;

        var config = new FBPPConfig()
        {
            SaveFileName = m_SaveFilename,
            AutoSaveData = false,
            ScrambleSaveData = true,
            EncryptionSecret = "my_secret",
            SaveFilePath = Application.persistentDataPath
        };

        FBPP.Start(config);
    }

    public static void DeleteAllKeys()
    {
        FBPP.DeleteAll();
        FBPP.Save();
    }
}
