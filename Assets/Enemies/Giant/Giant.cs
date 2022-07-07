using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Giant : Enemy
{
    private bool isChasing = false;
    private bool isStunned = false;
    private bool isCharging = false;
    private bool canAttack = false;
    private bool isAttacking = false;

    private Vector3 chargingTarget = Vector3.zero;
    private Coroutine actualChargingCoroutine = null;
    
    [SerializeField] private float chasingRange = 2.5f;
    [SerializeField] private float attackRange = 0.7f;

    [Space]
    public float radiusAttack = 1f;
    public float attackOffset = 0f;

    [Space]
    [SerializeField] private float timeBeforeCharging = .5f;
    [SerializeField] private float timeCharging = 2f;
    [SerializeField] private float timeAfterCharging = .5f;
    [Space]
    [SerializeField] private float chargeStuntDuration = .3f;

    // Update is called once per frame
    void Update()
    {
        if (!isDead && !isStunned)
        {
            float distance = Vector2.Distance(player.transform.position, transform.position);

            if (!isCharging && rb.velocity.magnitude > 0f)
                rb.velocity = Vector2.zero;

            if (!isChasing && distance <= chasingRange)
            {
                isChasing = true;
            }
            else if (isChasing && distance > chasingRange)
            {
                isChasing = false;
            }

            if (isChasing && !isCharging)
            {
                direction = player.transform.position - transform.position;
                Flip();
            }

            if (!canAttack && distance <= attackRange)
            {
                canAttack = true;
            }
            else if (canAttack && distance > attackRange)
            {
                canAttack = false;
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
        if (!isDead && isChasing && !isCharging && !canAttack && !isAttacking)
        {
            actualChargingCoroutine = StartCoroutine(Charge());
        }
        else if (!isDead && !isCharging && canAttack && !isAttacking)
        {
            StartAttack();
        }

        /*if (!isDead && isChasing && !isStunned)
        {
            direction = player.transform.position - transform.position;
            Flip();

            animator.SetFloat("Speed", direction.normalized.magnitude);

            rb.velocity = direction.normalized * stats.speed * Time.fixedDeltaTime;
        }*/
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        { 
            if (isCharging && !isAttacking)
            {
                // Stunt the player
                player.Stunt(chargeStuntDuration);

                // Cancel the dash then attack
                CutCharging();
                StartAttack();
            }
        }
    }

    IEnumerator Charge()
    {
        // Before charging
        isCharging = true;
        chargingTarget = player.transform.position;

        yield return new WaitForSeconds(timeBeforeCharging - 0.1f);

        animator.SetTrigger("Charge");

        yield return new WaitForSeconds(0.1f);

        // Charging
        rb.AddForce((chargingTarget - transform.position).normalized * stats.speed);

        yield return new WaitForSeconds(timeCharging);

        // After Charging
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Idle");

        yield return new WaitForSeconds(timeAfterCharging);

        isCharging = false;
        chargingTarget = Vector3.zero;

        if (isChasing)
        {
            direction = player.transform.position - transform.position;
            Flip();
        }
    }

    private void CutCharging()
    {
        StopCoroutine(actualChargingCoroutine);
        actualChargingCoroutine = null;

        rb.velocity = Vector2.zero;
        animator.SetTrigger("Idle");
        isCharging = false;
        chargingTarget = Vector3.zero;

        if (isChasing)
        {
            direction = player.transform.position - transform.position;
            Flip();
        }
    }

    private void StartAttack()
    {
        animator.SetTrigger("Attack");

        isAttacking = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
    }

    private void Attack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + new Vector3(attackOffset, 0f, 0f) * Mathf.Sign(transform.localScale.x), radiusAttack);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                Debug.Log("Player Damaged");
                break;
            }
        }
    }

    private void EndAttack()
    {
        isAttacking = false;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public override void TakeDamage(int damage)
    {
        // Push Back
        Vector2 nVec = (transform.position - player.transform.position).normalized;
        rb.AddForce(nVec * forcePushBack);

        base.TakeDamage(damage);
    }

    public override void TakeDamage(int damage, GameObject bullet)
    {
        // Stunt
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

        if (!isChasing && distance <= chasingRange)
        {
            isChasing = true;
        }
    }

    protected override IEnumerator Die()
    {
        //GetComponent<CapsuleCollider2D>().enabled = false;
        animator.SetTrigger("Death");

        return base.Die();
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chasingRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position + new Vector3(attackOffset, 0f, 0f) * Mathf.Sign(transform.localScale.x), radiusAttack);
    }
}
