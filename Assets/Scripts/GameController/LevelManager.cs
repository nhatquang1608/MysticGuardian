using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int lives = 10;

    public int TotalLives;
    public int CurrentWave;

    private void Start()
    {
        TotalLives = lives;
        CurrentWave = 1;
    }

    private void OnEnable()
    {
        Enemy.OnEndReached += ReduceLives;
    }

    private void ReduceLives(Enemy enemy)
    {
        TotalLives--;
        if(TotalLives <= 0)
        {
            TotalLives = 0;
            GameOver();
        }
    }

    private void GameOver()
    {}

    private void WaveCompleted()
    {}

    private void OnDisable()
    {
        Enemy.OnEndReached -= ReduceLives;
    }
}
