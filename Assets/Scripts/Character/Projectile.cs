using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static Action<Enemy, float> OnEnemyHit;

    protected enum Type
    {
        Arrow,
        Bom,
        Magic
    }

    [SerializeField] private Type type;

    [SerializeField] protected float moveSpeed = 10f;
    [SerializeField] private float minDistanceToDealDamage = 0.1f;

    public float characterDamage;
    [SerializeField] private float deltaRotate;
    [SerializeField] private Enemy enemyTarget;

    protected virtual void Update()
    {
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
        transform.position = Vector2.MoveTowards(transform.position, enemyTarget.transform.position, moveSpeed * Time.deltaTime);

        if(type != Type.Bom)
        {
            float distanceToTarget = Vector2.Distance(enemyTarget.transform.position, transform.position);
            if(distanceToTarget < minDistanceToDealDamage)
            {
                OnEnemyHit?.Invoke(enemyTarget, characterDamage);
                enemyTarget.enemyHealth.DealDamage(characterDamage);

                if(type == Type.Magic)
                {
                    enemyTarget.Poisoned();
                }

                Destroy(gameObject);
            }
        }
    }

    private void RotateProjectile()
    {
        Vector3 enemyPos = enemyTarget.transform.position - transform.position;
        float angle = Vector3.SignedAngle(transform.up, enemyPos, transform.forward);
        transform.Rotate(0, 0, angle + deltaRotate);
    }

    public void SetParameters(Enemy enemy, float damage)
    {
        enemyTarget = enemy;
        characterDamage = damage;
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Enemy") && type == Type.Bom && other.gameObject.activeSelf)
        {
            float distanceToTarget = (enemyTarget.transform.position - transform.position).magnitude;
            if(distanceToTarget < minDistanceToDealDamage)
            {
                Enemy enemy = other.GetComponent<Enemy>();
                OnEnemyHit?.Invoke(enemy, characterDamage);
                enemy.enemyHealth.DealDamage(characterDamage);
                Destroy(gameObject);
            }
        }
    }
}
