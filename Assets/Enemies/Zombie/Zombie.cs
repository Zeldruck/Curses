using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
{
    private bool isChasing = false;
    private bool playerClose = false;

    // Update is called once per frame
    void Update()
    {
        if (!isDead && !isChasing)
        {
            float distance = Vector2.Distance(player.transform.position, transform.position);

            if (GameManager.gameManager.isDay)
            {
                if ((!playerClose && distance <= detectionRangeDay) || (playerClose && distance > detectionRangeDay))
                {
                    playerClose = !playerClose;
                    animator.SetTrigger("Angry");
                }

                if (!isChasing && distance <= chasingRangeDay)
                {
                    isChasing = true;
                }
            }
            else
            {
                AnimatorStateInfo ast = animator.GetCurrentAnimatorStateInfo(0);
                if (!ast.IsName("Zombie_Angry_Idle"))
                {
                    playerClose = true;
                    animator.SetTrigger("Angry");
                }

                if (playerClose && !isChasing && distance <= chasingRangeNight)
                {
                    isChasing = true;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isDead && isChasing)
        {
            Vector2 direction = player.transform.position - transform.position;

            animator.SetFloat("Speed", direction.normalized.magnitude);

            rb.MovePosition(rb.position + direction.normalized * stats.speed * Time.fixedDeltaTime);
        }
    }

    public override void TakeDamage(int damage)
    {
        // Push Back
        Vector2 nVec = (transform.position - player.transform.position).normalized;
        rb.AddForce(nVec * forcePushBack * (isChasing ? stats.speed : 1f));

        base.TakeDamage(damage);
    }

    public override void TakeDamage(int damage, GameObject bullet)
    {
        // Push Back
        rb.AddForce(bullet.transform.right * forcePushBack * (isChasing ? stats.speed : 1f));

        base.TakeDamage(damage, bullet);
    }
}
