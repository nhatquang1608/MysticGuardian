using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public static Action<Enemy> OnEnemyKilled;
    public static Action<Enemy> OnEnemyHit;

    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private Transform barPosition;
    [SerializeField] private float maxHealth;
    [SerializeField] private int deathCoinReward;

    public float currentHealth;

    private Image healthBar;
    private Enemy enemy;
    private GameController gameController;

    private void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        CreateHealthBar();
        currentHealth = maxHealth;

        enemy = GetComponent<Enemy>();
    }

    private void Update()
    {
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, currentHealth / maxHealth, Time.deltaTime * 10f);
    }

    private void CreateHealthBar()
    {
        GameObject newBar = Instantiate(healthBarPrefab, barPosition.position, Quaternion.identity);
        newBar.transform.SetParent(transform);
        EnemyHealthContainer container = newBar.GetComponent<EnemyHealthContainer>();
        healthBar = container.FillAmountImage;
    }

    public void DealDamage(float damageReceived)
    {
        currentHealth -= damageReceived;
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            OnEnemyHit?.Invoke(enemy);
        }
    }

    public void Die()
    {
        enemy.objectPooler.ReturnToPool(gameObject);
        gameController.coins += deathCoinReward;
        gameController.SetText();
    }

    public void ResetHealth()
    {}
}
