using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO;

public class JsonControl
{
    string path;
    string mobilePath;

    public void SaveJson(Save save)
    {
        path = Application.dataPath;
        mobilePath = Application.persistentDataPath;
        string json = JsonConvert.SerializeObject(save);

#if UNITY_EDITOR
        File.WriteAllText(path + "/-Save.json", json);
        Debug.Log("Unity Editor");
#elif UNITY_ANDROID
        File.WriteAllText(mobilePath + "/-Save.json", json);
#endif
    }

    public Save LoadJson()
    {
        Save lvl = new Save();
        path = Application.dataPath;
        mobilePath = Application.persistentDataPath;
#if UNITY_EDITOR
        lvl = JsonConvert.DeserializeObject<Save>(File.ReadAllText(path + "/-Save.json"));
#elif UNITY_ANDROID
        lvl = JsonConvert.DeserializeObject<Save>(File.ReadAllText(mobilePath + "/-Save.json"));
#endif

        return lvl;
    }
    
    public Level LoadFromRecurces(string name)
    {
        Level lvl = new Level();
        path = "Jsons/Levels/" + name + ".json";
        string newpath = path.Replace(".json", "");
        TextAsset ta = Resources.Load<TextAsset>(newpath);
        string json = ta.text;
        lvl = JsonConvert.DeserializeObject<Level>(json);
        return lvl;

    }

    public Save LoadSavesFromRecurces()
    {
        Save lvl = new Save();
        path = "Jsons/-Save.json";
        string newpath = path.Replace(".json", "");
        TextAsset ta = Resources.Load<TextAsset>(newpath);
        string json = ta.text;
        lvl = JsonConvert.DeserializeObject<Save>(json);
        return lvl;
    }

}

[JsonObject]
public class Level 
{
    [JsonProperty]
    public short _id;
    [JsonProperty]
    public byte[,] field;
    [JsonProperty]
    public int[,] goals;
    [JsonProperty]
    public int countOfMoves;
    [JsonProperty]
    public int[,] awards;
}

[JsonObject]

public class Save
{
    public int lives;
    public int coins;
    public int stars;
    public int[,] levels;
}
