using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Fixed Delay")]
    public float currentTime;
    public float totalTime;
    private float delayBtwSpawns = 1f;
    private float delayPoolSpawns = 3f;
    [SerializeField] private int enemySpawned = 0;
    public int poolIndex;

    [Header("List Poolers")]
    [SerializeField] private GameController gameController;
    public Waypoints[] listWaypoints;
    public List<ObjectPooler> listPoolers = new List<ObjectPooler>();

    private void Start()
    {
        currentTime = 0;
        totalTime = 0;
        poolIndex = 0;
        CalculateTotalTime();
        StartCoroutine(SpawnPoolWithDelay());
    }

    public void CalculateTotalTime()
    {
        for(int i=0; i<listPoolers.Count-1; i++)
        {
            totalTime += (listPoolers[i].poolSize - 3) * delayBtwSpawns;

            if (i < listPoolers.Count - 3)
            {
                totalTime += delayPoolSpawns;
            }
        }
    }

    public IEnumerator SpawnPoolWithDelay()
    {
        while(poolIndex < listPoolers.Count && !gameController.isGameOver)
        {
            if(enemySpawned < listPoolers[poolIndex].poolSize)
            {
                SpawnEnemy(enemySpawned);
                enemySpawned++;
                yield return new WaitForSeconds(delayBtwSpawns);
            }
            else
            {
                enemySpawned = 0;
                poolIndex++;
                yield return new WaitForSeconds(delayPoolSpawns);
            }
        }
    }

    private void SpawnEnemy(int index)
    {
        GameObject newInstance = listPoolers[poolIndex].GetInstanceFromPool(index);
        newInstance.transform.position = listWaypoints[0].Points[0];
        newInstance.SetActive(true);
    }
}
