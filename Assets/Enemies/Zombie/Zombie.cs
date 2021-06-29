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
            direction = player.transform.position - transform.position;
            Flip();

            animator.SetFloat("Speed", direction.normalized.magnitude);

            rb.MovePosition(rb.position + direction.normalized * stats.speed * Time.fixedDeltaTime);
        }
    }

    public override void TakeDamage(int damage)
    {
        // Push Back
        Vector2 nVec = (transform.position - player.transform.position).normalized;
        rb.AddForce(nVec * forcePushBack * (isChasing ? stats.speed * 2f : 1f));

        base.TakeDamage(damage);

        if (!isChasing)
            isChasing = true;
    }

    public override void TakeDamage(int damage, GameObject bullet)
    {
        // Push Back
        rb.AddForce(bullet.transform.right * forcePushBack * (isChasing ? stats.speed * 2f : 1f));

        base.TakeDamage(damage, bullet);

        if (!isChasing)
            isChasing = true;
    }

    protected override IEnumerator Die()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        animator.SetTrigger("Death");

        return base.Die();
    }
}
