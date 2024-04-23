using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TopSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject guide;

    private void Start()
    {
        guide.SetActive(false);
    }

    public void Play()
    {
        SceneManager.LoadScene("SelectScene");
    }
}
