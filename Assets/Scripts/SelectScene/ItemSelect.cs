using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ItemSelect : MonoBehaviour
{
    [SerializeField] private int levelId;
    [SerializeField] private GameObject levelText;
    [SerializeField] private GameObject lockedImage;

    private void Awake()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(Play);
    }

    private void Play()
    {
        SaveLoadData.Instance.level = levelId;
        SceneManager.LoadScene("GameScene");
    }

    public void SetLock(bool locked, int level)
    {
        levelText.SetActive(!locked);
        lockedImage.SetActive(locked);

        levelId = level;
    }
}
