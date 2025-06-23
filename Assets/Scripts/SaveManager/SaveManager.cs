


using System.IO;
using System.Xml;
using Newtonsoft.Json;
using UnityEngine;

public sealed class SaveManager
{

    
    public static readonly SaveManager saveManager = new();

    private SaveManager(){}

    private string FilePath(ISaveData saveData)
    {
        return Path.Combine(Application.persistentDataPath, saveData.Name() + ".json");
    }

    public void Save(ISaveData saveData)
    {
        string json = JsonConvert.SerializeObject(saveData, Newtonsoft.Json.Formatting.Indented);
        string path = FilePath(saveData);
        File.WriteAllText(path, json);
        Debug.Log("Saved: " + path);
    }

    public void Load<T>(T saveData) where T : ISaveData
    {
        string path = FilePath(saveData);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"No save found at {path} creating default one");
            Save(saveData);
        }

        string json = File.ReadAllText(path);
        JsonConvert.PopulateObject(json, saveData);
    }


}