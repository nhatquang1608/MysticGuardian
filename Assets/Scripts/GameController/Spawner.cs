using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Fixed Delay")]
    public float currentTime;
    public float totalTime;
    [SerializeField] private float delayBtwSpawns;
    [SerializeField] private float delayPoolSpawns;
    [SerializeField] private int enemySpawned;
    public int poolIndex;

    [Header("List Poolers")]
    [SerializeField] private GameController gameController;
    public Waypoints[] listWaypoints;
    public List<ObjectPooler> listPoolers = new List<ObjectPooler>();

    private void Start()
    {
        CalculateTotalTime();
        StartCoroutine(SpawnPoolWithDelay());
    }

    public void CalculateTotalTime()
    {
        for(int i=0; i<listPoolers.Count-1; i++)
        {
            for(int j=0; j<listPoolers[i].poolSize-1; j++)
            {
                totalTime += delayBtwSpawns;
            }
            totalTime += delayPoolSpawns;
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
