using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "List Levels")]
public class ListLevels : ScriptableObject
{
    [System.Serializable]
    public class LevelDetails
    {
        public int levelId;
        public int coins;
        public int health;
        public bool isLock;
        public bool isCompleted;
        public List<string> characters;
        public List<WaveInfo> waves;
    }

    [System.Serializable]
    public class WaveInfo
    {
        public string enemyType;
        public int enemyCount;
    }

    public List<LevelDetails> listLevelDetails = new List<LevelDetails>();
}
