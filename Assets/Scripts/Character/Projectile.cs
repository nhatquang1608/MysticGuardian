using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    protected enum Type
    {
        Arrow,
        Bom,
        Magic
    }

    [SerializeField] private Type type;

    [SerializeField] protected float moveSpeed = 10f;
    [SerializeField] private float minDistanceToDealDamage = 0.3f;

    public float characterDamage;
    [SerializeField] private float deltaRotate;
    [SerializeField] private bool isBegin;
    [SerializeField] private Vector3 startPos;
    [SerializeField] private Vector3 pointTarget;
    [SerializeField] private Enemy enemyTarget;
    [SerializeField] private GameObject effect;
    [SerializeField] private GameController gameController;

    protected virtual void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();

        if(type == Type.Arrow)
        {
            isBegin = true;
            pointTarget = transform.position + new Vector3(0, 1f, 0);
            transform.Rotate(0, 0, 90);
        }
        else if(type == Type.Bom)
        {
            // isBegin = true;
            startPos = transform.position;
            pointTarget = enemyTarget.transform.position;
            StartCoroutine(FlyItem(startPos, pointTarget));
        }
    }

    protected virtual void Update()
    {
        if(type == Type.Bom) return;
        if(enemyTarget && enemyTarget.gameObject.activeSelf)
        {
            MoveProjectile();
            RotateProjectile();
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    protected virtual void MoveProjectile()
    {
        if(isBegin)
        {
            transform.position = Vector2.MoveTowards(transform.position, pointTarget, moveSpeed * Time.deltaTime);
            float distance = Vector2.Distance(pointTarget, transform.position);
            if(distance < minDistanceToDealDamage)
            {
                isBegin = false;
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget.transform.position, moveSpeed * Time.deltaTime);

            float distanceToTarget = Vector2.Distance(enemyTarget.transform.position, transform.position);
            if(distanceToTarget < minDistanceToDealDamage)
            {
                enemyTarget.enemyHealth.DealDamage(characterDamage);

                if(type == Type.Magic)
                {
                    enemyTarget.Poisoned();
                }

                gameController.ShowEffect(effect, gameObject);
                Destroy(gameObject);
            }
        }
    }

    public IEnumerator FlyItem(Vector3 initialPosition, Vector3 targetPosition)
    {
        float flyDuration = 1f;
        float flyHeight = 3f;
        float elapsedTime = 0f;

        while (elapsedTime < flyDuration)
        {
            float t = elapsedTime / flyDuration;
            transform.position = Vector3.Lerp(initialPosition, targetPosition, t) + Vector3.up * Mathf.Sin(t * Mathf.PI) * flyHeight; // Parabolic path
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        yield return new WaitForSeconds(0.05f);
        if(gameObject)
        {
            gameController.ShowEffect(effect, gameObject);
            Destroy(gameObject);
        }
    }

    private void RotateProjectile()
    {
        if(isBegin) return;
        Vector3 enemyPos = enemyTarget.transform.position - transform.position;
        float angle = Vector3.SignedAngle(transform.up, enemyPos, transform.forward);
        if(!isBegin) transform.Rotate(0, 0, angle + deltaRotate);
    }

    public void SetParameters(Enemy enemy, float damage)
    {
        enemyTarget = enemy;
        characterDamage = damage;
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Enemy") && type == Type.Bom)
        {
            float distanceToTarget = Vector2.Distance(pointTarget, transform.position);
            if(distanceToTarget < minDistanceToDealDamage)
            {
                if(other.gameObject.activeSelf)
                {
                    Enemy enemy = other.GetComponent<Enemy>();
                    enemy.enemyHealth.DealDamage(characterDamage);
                }
                gameController.ShowEffect(effect, gameObject);
                Destroy(gameObject);
            }
        }
    }
}
