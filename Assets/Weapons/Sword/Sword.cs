using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MeleeWeaponClass
{
    private float rWeapon = 0f;

    [Range(1f, 15f)] public float mAttackSpeed = 2f;
    [Range(10f, 180f)] public float mAttackAngle = 45f;

    public override void Attack()
    {
        canAttack = false;
        animator.SetTrigger("swing");
    }

    private void ApplyDamage()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(hitTransform.position, hitradius, 0f, enemiesMask);

        for (int i = 0; i < enemiesHit.Length; i++)
        {
            enemiesHit[i].GetComponent<Enemy>().TakeDamage(50);
        }
    }

    private void ResetAttack()
    {
        canAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (hitTransform == null)
            return;

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(hitTransform.position, hitradius);
    }
}
