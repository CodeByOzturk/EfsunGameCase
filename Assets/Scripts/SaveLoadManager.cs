using System;
using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    private string saveFilePath;

    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "gameData.json");
    }

    public void SaveGame(GameData data)
    {
        string json = JsonUtility.ToJson(data, true);

        try
        {
            File.WriteAllText(saveFilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving game: {e.Message}");
        }
    }

    public GameData LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            GameData data = JsonUtility.FromJson<GameData>(json);

            if (data == null)
            {
                Debug.LogError("Failed to load data from JSON, returning default data.");
                return new GameData();
            }
            return data;
        }
        else
        {
            Debug.LogWarning("No save file found, creating new data.");
            return new GameData(); 
        }
    }
}
