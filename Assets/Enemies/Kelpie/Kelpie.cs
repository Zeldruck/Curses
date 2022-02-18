using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kelpie : Enemy
{
    private bool isChasing = false;
    private bool isStunned = false;
    
    void Update()
    {
        if (!isDead && !isStunned)
        {
            float distance = Vector2.Distance(player.transform.position, transform.position);

            if (!isChasing && distance <= chasingRangeDay)
            {
                isChasing = true;
            }
            else if (isChasing && distance > chasingRangeDay)
            {
                isChasing = false;
            }

            /*if (isChasing && !isAttacking && distance <= attackRange && timerAttack <= 0f)
            {
                StartAttack();
            }
            else if (timerAttack > 0f)
            {
                timerAttack -= Time.deltaTime;
            }*/
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
        
        // Push Back
        Vector2 nVec = (transform.position - player.transform.position).normalized;
        rb.AddForce(nVec * forcePushBack);

        base.TakeDamage(damage);
    }

    public override void TakeDamage(int damage, GameObject bullet)
    {
        if (isDead) return;
        
        // Stun
        StartCoroutine(AfterTakingDamage());

        // Push Back
        rb.AddForce(bullet.transform.right * forcePushBack);
        

        base.TakeDamage(damage, bullet);
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
        //GetComponent<CapsuleCollider2D>().enabled = false;
        animator.SetTrigger("Death");

        //return base.Die();
        yield return null;
    }
}
