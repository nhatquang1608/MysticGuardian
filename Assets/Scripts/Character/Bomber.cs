using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomber : CharacterController
{
    [SerializeField] protected Transform projectileSpawnPosition;
    [SerializeField] private Projectile bomPrefab;

    protected override void Attack()
    {
        base.Attack();
        if(currentEnemyTarget && currentEnemyTarget.gameObject.activeSelf)
        {
            Projectile projectile = Instantiate(bomPrefab, projectileSpawnPosition.position, Quaternion.identity);
            projectile.SetParameters(currentEnemyTarget, damage);
            SoundManager.Instance.PlaySound(SoundManager.Instance.bomberSound);
            animator.SetBool("attack", true);
        }
        else
        {
            animator.SetBool("attack", false);
        }
    }
}
