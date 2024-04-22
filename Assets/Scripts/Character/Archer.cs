using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : CharacterController
{
    [SerializeField] protected Transform projectileSpawnPosition;
    [SerializeField] private Projectile arrowPrefab;

    protected override void Attack()
    {
        base.Attack();
        if(currentEnemyTarget && currentEnemyTarget.gameObject.activeSelf)
        {
            Projectile projectile = Instantiate(arrowPrefab, projectileSpawnPosition.position, Quaternion.identity);
            projectile.SetParameters(currentEnemyTarget, damage);
            SoundManager.Instance.PlaySound(SoundManager.Instance.archerSound);
            animator.SetBool("attack", true);
        }
        else
        {
            animator.SetBool("attack", false);
        }
    }
}
