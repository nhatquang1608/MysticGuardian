using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    protected enum CharacterRole
    {
        Archer,
        Bomber,
        Hammer,
        Witcher
    }

    [SerializeField] protected CharacterRole role;

    [SerializeField] protected int currentLevel;
    [SerializeField] protected float delayPerAttack;
    [SerializeField] protected float nextAttackTime;
    [SerializeField] protected int damage;
    [SerializeField] protected int damageUpgrade;
    [SerializeField] public int price;
    [SerializeField] protected int priceUpgrade;
    [SerializeField] protected TextMeshProUGUI upgradeInfoText;

    [SerializeField] protected Button upgradeButton;
    
    [SerializeField] protected GameObject level1;
    [SerializeField] protected GameObject level2;
    [SerializeField] protected GameObject level3;

    [SerializeField] protected GameObject radius;
    [SerializeField] protected GameObject upgradePanel;

    [SerializeField] protected Animator animator;
    [SerializeField] protected GameController gameController;
    [SerializeField] protected Enemy currentEnemyTarget;
    [SerializeField] protected List<Enemy> listTargetEnemies;

    protected virtual void Awake()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();

        upgradeButton.onClick.AddListener(() => Upgrade());

        Upgrade();
    }

    protected virtual void Update()
    {
        if(gameController.isGameOver) 
        {
            animator.SetBool("attack", false);
            return;
        }
        GetCurrentEnemyTarget();
        if(Time.time > nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + delayPerAttack;
        }
    }

    protected virtual void Attack()
    {}

    protected virtual void GetCurrentEnemyTarget()
    {
        if(listTargetEnemies.Count <= 0)
        {
            currentEnemyTarget = null;
            return;
        }
        else
        {
            if(currentEnemyTarget == null)
            {
                listTargetEnemies.Remove(currentEnemyTarget);
            }
        }
        currentEnemyTarget = listTargetEnemies[0];
    }

    public virtual void ShowUpgrade()
    {
        gameController.HideAllUpgradePanel();
        radius.SetActive(true);
        upgradePanel.SetActive(true);
    }

    public virtual void HideUpgrade()
    {
        radius.SetActive(false);
        upgradePanel.SetActive(false);
    }

    protected virtual void Upgrade()
    {
        if(gameController.coins < priceUpgrade) return;
        currentLevel++;

        if(currentLevel == 1)
        {
            level1.SetActive(true);
            level2.SetActive(false);
            level3.SetActive(false);

            animator = level1.GetComponent<Animator>();

            gameController.coins -= price;
            upgradeInfoText.text = "Upgrade to level 2";

            gameController.listCharacters.Add(this);
        }
        else if(currentLevel == 2)
        {
            level1.SetActive(false);
            level2.SetActive(true);
            level3.SetActive(false);

            animator = level2.GetComponent<Animator>();

            damage += damageUpgrade;
            gameController.coins -= priceUpgrade;
            upgradeInfoText.text = "Upgrade to level 3";

            SoundManager.Instance.PlaySound(SoundManager.Instance.upgradeCharacterSound);
        }
        else if(currentLevel == 3)
        {
            level1.SetActive(false);
            level2.SetActive(false);
            level3.SetActive(true);

            animator = level3.GetComponent<Animator>();

            damage += damageUpgrade;
            gameController.coins -= priceUpgrade;
            upgradeInfoText.text = "Max level";

            SoundManager.Instance.PlaySound(SoundManager.Instance.upgradeCharacterSound);
        }

        HideUpgrade();
        gameController.SetText();
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            if(other.GetComponent<Enemy>().enemyType == Enemy.EnemyType.Bat)
            {
                if(role == CharacterRole.Archer || role == CharacterRole.Witcher)
                {
                    Enemy bat = other.GetComponent<Enemy>();
                    listTargetEnemies.Add(bat);
                }
            }
            else
            {
                Enemy enemy = other.GetComponent<Enemy>();
                listTargetEnemies.Add(enemy);
            }
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if(listTargetEnemies.Contains(enemy))
            {
                listTargetEnemies.Remove(enemy);
            }
        }
    }
}
