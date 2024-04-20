using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witcher : CharacterController
{
    [SerializeField] protected Transform projectileSpawnPosition;
    [SerializeField] private Projectile powerPrefab;

    protected override void Attack()
    {
        base.Attack();
        if(currentEnemyTarget && currentEnemyTarget.gameObject.activeSelf)
        {
            Projectile projectile = Instantiate(powerPrefab, projectileSpawnPosition.position, Quaternion.identity);
            projectile.SetParameters(currentEnemyTarget, damage);
            animator.SetBool("attack", true);
        }
        else
        {
            animator.SetBool("attack", false);
        }
    }
}
