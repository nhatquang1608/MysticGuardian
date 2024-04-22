using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    private int poolerDestroyCount;

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
    [SerializeField] private TextMeshProUGUI healthDecreaseText;
    [SerializeField] private TextMeshProUGUI coinRewardText;
    [SerializeField] private CanvasGroup gameOver;
    [SerializeField] private GameObject completedPanel;
    [SerializeField] private GameObject failedPanel;
    [SerializeField] private Spawner spawner;
    public GameObject[] listPlaces;
    [SerializeField] private List<GameObject> listMapLevels;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button[] listHomeButtons;
    [SerializeField] private Button[] listRestartButtons;
    [SerializeField] private List<Button> listCharacterPurchaseButtons = new List<Button>();
    public List<CharacterController> listCharacters;

    private void Awake()
    {
        listMapLevels[SaveLoadData.Instance.level].SetActive(true);
        listPlaces = GameObject.FindGameObjectsWithTag("Place");
        spawner = listMapLevels[SaveLoadData.Instance.level].GetComponentInChildren<Spawner>();
        coins = SaveLoadData.Instance.listLevels.listLevelDetails[SaveLoadData.Instance.level].coins;
        health = SaveLoadData.Instance.listLevels.listLevelDetails[SaveLoadData.Instance.level].health;

        foreach(GameObject place in listPlaces)
        {
            Places places = place.GetComponentInChildren<Places>();
            places.isOccupied = false;
        }

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
                    listCharacterPurchaseButtons.Add(witcherButton);
                    break;
                case "Bomber":
                    Button bomberButton = Instantiate(bomberButtonPrefab, buttonContainer);
                    listCharacterPurchaseButtons.Add(bomberButton);
                    break;
                case "Hammer":
                    Button hammerButton = Instantiate(hammerButtonPrefab, buttonContainer);
                    listCharacterPurchaseButtons.Add(hammerButton);
                    break;
                case "Archer":
                    Button archerButton = Instantiate(archerButtonPrefab, buttonContainer);
                    listCharacterPurchaseButtons.Add(archerButton);
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

        SetText();

        StartCoroutine(ShowWaveProcess());
    }

    private void OnEnable()
    {
        Enemy.OnDecreaseHealth += DecreaseHealth;
        EnemyHealth.OnEnemyKilled += EnemyKilled;
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
        SoundManager.Instance.PlaySound(SoundManager.Instance.decreaseHealthSound);
        healthDecreaseText.text = "-1";
        if(health <= 0)
        {
            health = 0;
            isGameOver = true;
            GameOver(false);
        }
        SetText();
        StartCoroutine(ShowChangeParameters(healthDecreaseText.gameObject));
    }

    private void EnemyKilled(int deathCoinReward)
    {
        coins += deathCoinReward;
        SoundManager.Instance.PlaySound(SoundManager.Instance.enemyDead);
        coinRewardText.text = "+" + deathCoinReward.ToString();
        SetText();
        StartCoroutine(ShowChangeParameters(coinRewardText.gameObject));
    }

    private IEnumerator ShowChangeParameters(GameObject info)
    {
        info.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        info.SetActive(false);
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

        poolerDestroyCount++;
        if(poolerDestroyCount == spawner.listPoolers.Count)
        {
            isGameOver = true;
            GameOver(true);
        }
    }

    public void SetText()
    {
        coinsText.text = coins.ToString();
        healthText.text = health.ToString();

        foreach(Button button in listCharacterPurchaseButtons)
        {
            CharacterPurchaseButton characterPurchaseButton = button.gameObject.GetComponent<CharacterPurchaseButton>();
            if(coins >= characterPurchaseButton.characterPrefab.GetComponent<CharacterController>().price)
            {
                characterPurchaseButton.SetAvailable(true);
            }
            else
            {
                characterPurchaseButton.SetAvailable(false);
            }
        }
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

        if(completed)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.completedSound);
        }
        else
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.failedSound);
        }
    }

    public void ShowEffect(GameObject effect, GameObject instance)
    {
        StartCoroutine(Effect(effect, instance));
    }

    private IEnumerator Effect(GameObject effect, GameObject instance)
    {
        GameObject newEffect = Instantiate (effect, instance.transform.position, Quaternion.identity);

        yield return new WaitForSeconds(0.4f);

        Destroy(newEffect);
    }

    private void OnDisable()
    {
        Enemy.OnDecreaseHealth -= DecreaseHealth;
        EnemyHealth.OnEnemyKilled -= EnemyKilled;
        ObjectPooler.OnDestroyPooler -= DestroyPooler;
    }
}
