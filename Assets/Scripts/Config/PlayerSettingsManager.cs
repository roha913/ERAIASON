using System.IO;
using UnityEngine;
using static GlobalConfig;

[System.Serializable]
public class PlayerSettings
{
    public ProcessingMethod voxelWorldMethod;
}

public static class PlayerSettingsManager
{
    static string path => Application.persistentDataPath + "/PlayerSettings.json";

    public static void Save(PlayerSettings s)
    {
        File.WriteAllText(path, JsonUtility.ToJson(s));
    }

    public static PlayerSettings Load()
    {
        if (File.Exists(path))
            return JsonUtility.FromJson<PlayerSettings>(File.ReadAllText(path));
        return new PlayerSettings(); // default
    }
}
