using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static event Action<ObjectPooler> OnDestroyPooler;
    private int returnCount;
    public GameObject prefab;
    public int poolSize;
    private List<GameObject> pool = new List<GameObject>();

    public void CreatePooler()
    {
        for(int i=0; i<poolSize; i++)
        {
            pool.Add(CreateInstance());
        }
    }

    private GameObject CreateInstance()
    {
        GameObject newInstance = Instantiate(prefab);
        newInstance.transform.SetParent(transform);
        newInstance.GetComponent<Enemy>().objectPooler = this;
        newInstance.SetActive(false);
        return newInstance;
    }

    public GameObject GetInstanceFromPool(int index)
    {
        return pool[index];
    }

    public void ReturnToPool(GameObject instance)
    {
        instance.SetActive(false);
        returnCount++;
        if(returnCount == poolSize)
        {
            OnDestroyPooler?.Invoke(this);
        }
    }

    public static IEnumerator ReturnToPoolWithDelay(GameObject instance, float delay)
    {
        yield return new WaitForSeconds(delay);
        instance.SetActive(false);
    }
}
