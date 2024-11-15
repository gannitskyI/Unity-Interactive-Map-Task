using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class GameData
{
    public List<PinInfo> Pins = new List<PinInfo>();

    private static GameData _cachedData;
    private static readonly string FilePath = Application.persistentDataPath + "/gamedata.json";

    public static GameData LoadData()
    {
        if (_cachedData != null)
            return _cachedData;

        if (!File.Exists(FilePath))
        {
            _cachedData = new GameData();
        }
        else
        {
            string json = File.ReadAllText(FilePath);
            _cachedData = JsonUtility.FromJson<GameData>(json) ?? new GameData();
        }

        return _cachedData;
    }

    public static void SaveData(GameData data)
    {
        _cachedData = data;
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(FilePath, json);
    }

    public static void SaveChanges()
    {
        if (_cachedData == null) return;

        string json = JsonUtility.ToJson(_cachedData);
        File.WriteAllText(FilePath, json);
    }
}
