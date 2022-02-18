using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
{
    private bool isChasing = false;
    private bool isStunned = false;
    private bool playerClose = false;

    // Update is called once per frame
    void Update()
    {
        if (!isDead && !isChasing && !isStunned)
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
        if (!isDead && isChasing && !isStunned)
        {
            direction = player.transform.position - transform.position;
            Flip();

            animator.SetFloat("Speed", direction.normalized.magnitude);

            rb.velocity = direction.normalized * stats.speed * Time.fixedDeltaTime;
        }
        else if (!isDead && !isChasing)
        {
            animator.SetFloat("Speed", 0f);
        }
    }

    public override void TakeDamage(int damage)
    {
        if (isDead) return;
        
        // Stunt
        StartCoroutine(AfterTakingDamage());

        // Push Back
        Vector2 nVec = (transform.position - player.transform.position).normalized;
        rb.AddForce(nVec * forcePushBack);

        base.TakeDamage(damage);

        if (!isChasing)
            isChasing = true;
    }

    public override void TakeDamage(int damage, GameObject bullet)
    {
        if (isDead) return;
        
        // Stunt
        StartCoroutine(AfterTakingDamage());

        // Push Back
        rb.AddForce(bullet.transform.right * forcePushBack);

        base.TakeDamage(damage, bullet);

        if (!isChasing)
            isChasing = true;
    }

    IEnumerator AfterTakingDamage()
    {
        isChasing = false;
        isStunned = true;

        yield return new WaitForSeconds(0.3f);

        isStunned = false;

        float distance = Vector2.Distance(player.transform.position, transform.position);

        if (!isChasing && distance <= chasingRangeDay)
        {
            isChasing = true;
        }
    }

    protected override IEnumerator Die()
    {
        //GetComponent<BoxCollider2D>().enabled = false;
        animator.SetTrigger("Death");

        return base.Die();
    }
}
