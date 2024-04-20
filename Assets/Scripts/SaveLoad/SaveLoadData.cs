using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoadData : MonoBehaviour
{
    public static SaveLoadData Instance;
    public int level;
    private string filePath;
    public ListLevels listLevels;

    public void Awake()
    {
        filePath = Application.persistentDataPath + "/data.json";

        if (!File.Exists(filePath))
        {
            SaveData();
        }
        else
        {
            LoadData();
        }

        if (Instance != null) 
        {
            DestroyImmediate(gameObject);
        } 
        else 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SaveData()
    {
        string jsonData = JsonUtility.ToJson(listLevels);
        File.WriteAllText(filePath, jsonData);
    }

    public void LoadData()
    {
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            listLevels = ScriptableObject.CreateInstance<ListLevels>();
            JsonUtility.FromJsonOverwrite(jsonData, listLevels);
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
    }
}
