using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Parameters")]
    public bool isGameOver;
    public int coins;
    public int health;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI healthText;
    public Image waveProgress;

    [Header("Store")]
    [SerializeField] private Button witcherButtonPrefab;
    [SerializeField] private Button bomberButtonPrefab;
    [SerializeField] private Button hammerButtonPrefab;
    [SerializeField] private Button archerButtonPrefab;
    [SerializeField] private Transform buttonContainer;

    [Header("Character Prefab")]
    public GameObject characterDemo;
    [SerializeField] private GameObject witcherPrefab;
    [SerializeField] private GameObject bomberPrefab;
    [SerializeField] private GameObject hammerPrefab;
    [SerializeField] private GameObject archerPrefab;
    
    [Header("Enemy Prefab")]
    [SerializeField] private GameObject snailPrefab;
    [SerializeField] private GameObject chameleonPrefab;
    [SerializeField] private GameObject pigPrefab;
    [SerializeField] private GameObject rinoPrefab;
    [SerializeField] private GameObject batPrefab;

    [Header("Controller")]
    [SerializeField] private CanvasGroup gameOver;
    [SerializeField] private GameObject completedPanel;
    [SerializeField] private GameObject failedPanel;
    [SerializeField] private Spawner spawner;
    public GameObject[] listPlaces;
    [SerializeField] private List<GameObject> listMapLevels;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button[] listHomeButtons;
    [SerializeField] private Button[] listRestartButtons;
    public List<CharacterController> listCharacters;

    private void Awake()
    {
        listMapLevels[SaveLoadData.Instance.level].SetActive(true);
        listPlaces = GameObject.FindGameObjectsWithTag("Place");
        spawner = listMapLevels[SaveLoadData.Instance.level].GetComponentInChildren<Spawner>();
        coins = SaveLoadData.Instance.listLevels.listLevelDetails[SaveLoadData.Instance.level].coins;
        health = SaveLoadData.Instance.listLevels.listLevelDetails[SaveLoadData.Instance.level].health;
        SetText();

        foreach(ListLevels.WaveInfo waveInfo in SaveLoadData.Instance.listLevels.listLevelDetails[SaveLoadData.Instance.level].waves)
        {
            GameObject poolContainer = new GameObject($"Pool - {waveInfo.enemyType}");
            ObjectPooler objectPooler = poolContainer.AddComponent<ObjectPooler>();
            objectPooler.poolSize = waveInfo.enemyCount;

            switch(waveInfo.enemyType)
            {
                case "Snail":
                    objectPooler.prefab = snailPrefab;
                    break;
                case "Chameleon":
                    objectPooler.prefab = chameleonPrefab;
                    break;
                case "Pig":
                    objectPooler.prefab = pigPrefab;
                    break;
                case "Rino":
                    objectPooler.prefab = rinoPrefab;
                    break;
                case "Bat":
                    objectPooler.prefab = batPrefab;
                    break;
            }

            objectPooler.CreatePooler();
            spawner.listPoolers.Add(objectPooler);
        }

        foreach(string character in SaveLoadData.Instance.listLevels.listLevelDetails[SaveLoadData.Instance.level].characters)
        {
            switch(character)
            {
                case "Witcher":
                    Button witcherButton = Instantiate(witcherButtonPrefab, buttonContainer);
                    break;
                case "Bomber":
                    Button bomberButton = Instantiate(bomberButtonPrefab, buttonContainer);
                    break;
                case "Hammer":
                    Button hammerButton = Instantiate(hammerButtonPrefab, buttonContainer);
                    break;
                case "Archer":
                    Button archerButton = Instantiate(archerButtonPrefab, buttonContainer);
                    break;
            }
        }

        foreach(Button button in listHomeButtons)
        {
            button.onClick.AddListener(() => SceneManager.LoadScene("SelectScene"));
        }

        foreach(Button button in listRestartButtons)
        {
            button.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
        }

        nextButton.onClick.AddListener(OnNext);
    }

    private void Start()
    {
        isGameOver = false;
        gameOver.alpha = 0f;
        gameOver.interactable = false;

        completedPanel.SetActive(false);
        failedPanel.SetActive(false);

        if(SaveLoadData.Instance.level == SaveLoadData.Instance.listLevels.listLevelDetails.Count-1)
        {
            nextButton.interactable = false;
        }

        StartCoroutine(ShowWaveProcess());
    }

    private void OnEnable()
    {
        Enemy.OnDecreaseHealth += DecreaseHealth;
        ObjectPooler.OnDestroyPooler += DestroyPooler;
    }

    private void OnNext()
    {
        if(SaveLoadData.Instance.level < SaveLoadData.Instance.listLevels.listLevelDetails.Count-1)
        {
            SaveLoadData.Instance.level++;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private IEnumerator ShowWaveProcess()
    {
        while(!isGameOver)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            spawner.currentTime += Time.deltaTime;

            if(spawner.currentTime / spawner.totalTime > 1)
            {
                waveProgress.fillAmount = 1;
                break;
            }
            else
            {
                waveProgress.fillAmount = spawner.currentTime / spawner.totalTime;
            }
        }
    }

    private void DecreaseHealth()
    {
        health--;
        if(health <= 0)
        {
            health = 0;
            isGameOver = true;
            GameOver(false);
        }
        SetText();
    }

    private void DestroyPooler(ObjectPooler objectPooler)
    {
        // spawner.listPoolers.Remove(objectPooler);
        // Destroy(objectPooler.gameObject);

        // if(spawner.listPoolers.Count <= 0)
        // {
        //     isGameOver = true;
        //     GameOver(true);
        // }

        if(spawner.poolIndex == spawner.listPoolers.Count)
        {
            isGameOver = true;
            GameOver(true);
        }
    }

    public void SetText()
    {
        coinsText.text = coins.ToString();
        healthText.text = health.ToString();
    }

    public void HideAllUpgradePanel()
    {
        foreach(CharacterController character in listCharacters)
        {
            character.HideUpgrade();
        }
    }

    public void GameOver(bool completed)
    {
        gameOver.interactable = true;

        if(completed)
        {
            completedPanel.SetActive(true);
            SaveLoadData.Instance.listLevels.listLevelDetails[SaveLoadData.Instance.level].isCompleted = true;
            if(SaveLoadData.Instance.level < SaveLoadData.Instance.listLevels.listLevelDetails.Count-1)
            {
                SaveLoadData.Instance.listLevels.listLevelDetails[SaveLoadData.Instance.level+1].isLock = false;
            }
        }
        else 
        {
            failedPanel.SetActive(true);
            SaveLoadData.Instance.listLevels.listLevelDetails[SaveLoadData.Instance.level].isCompleted = false;
        }

        SaveLoadData.Instance.SaveData();

        StartCoroutine(Fade(gameOver, 1f, 1f, completed));
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay = 0f, bool completed = false)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        float duration = 0.5f;
        float from = canvasGroup.alpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;
    }

    private void OnDisable()
    {
        Enemy.OnDecreaseHealth -= DecreaseHealth;
        ObjectPooler.OnDestroyPooler -= DestroyPooler;
    }
}
