using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Lantern_Man : Enemy
{
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed;

    private void Update()
    {
        if (timerAttack > 0f)
            timerAttack -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (!isDead && timerAttack <= 0f && Vector2.Distance(player.transform.position, transform.position) < attackRange)
        {
            timerAttack = stats.attackRate;
            animator.SetTrigger("Attack");
        }
    }

    private void Attack()
    {
        int choice = Random.Range(0, 3);

        // X
        if (choice == 0)
        {
            for (int y = -1; y <= 1; y += 2)
            {
                for (int x = -1; x <= 1; x += 2)
                {
                    GameObject fireball = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                    fireball.GetComponent<Fireball>().LaunchFireball(new Vector2(x, y) * projectileSpeed, stats.damage, 3f);
                }
            }
        }
        // +
        else if (choice == 1)
        {
            // x direction
            for (int i = -1; i <= 1; i += 2)
            {
                GameObject fireball = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                fireball.GetComponent<Fireball>().LaunchFireball(transform.right * i * projectileSpeed, stats.damage, 3f);
            }
            
            // y direction
            for (int i = -1; i <= 1; i += 2)
            {
                GameObject fireball = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                fireball.GetComponent<Fireball>().LaunchFireball(transform.up * i * projectileSpeed, stats.damage, 3f);
            }
        }
        // Y
        else
        {
            for (int i = -1; i <= 1; i += 2)
            {
                GameObject fireball = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                fireball.GetComponent<Fireball>().LaunchFireball(new Vector2(i, 1) * projectileSpeed, stats.damage, 3f);
            }
            
            GameObject fireballBottom = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            fireballBottom.GetComponent<Fireball>().LaunchFireball(new Vector2(0, -1) * projectileSpeed, stats.damage, 3f);
        }
    }

    protected override IEnumerator Die()
    {
        GetComponent<Collider2D>().enabled = false;
        animator.SetTrigger("Death");

        return base.Die();
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
