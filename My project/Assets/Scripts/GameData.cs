using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class GameData
{
    public List<Vector2> PinPositions = new List<Vector2>();

    public static GameData LoadData()
    {
        if (!File.Exists(Application.persistentDataPath + "/gamedata.json"))
            return new GameData();

        string json = File.ReadAllText(Application.persistentDataPath + "/gamedata.json");
        return JsonUtility.FromJson<GameData>(json);
    }

    public static void SaveData(GameData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/gamedata.json", json);
    }
}
