using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : CharacterController
{
    protected override void Attack()
    {
        base.Attack();
        if(listTargetEnemies.Count > 0)
        {
            for(int i = 0; i<listTargetEnemies.Count; i++)
            {
                if(listTargetEnemies[i]) listTargetEnemies[i].enemyHealth.DealDamage(damage);
            }
            animator.SetBool("attack", true);
        }
        else
        {
            animator.SetBool("attack", false);
        }
    }
}
