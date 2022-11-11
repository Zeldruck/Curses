using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Green_Knight : Enemy
{
    private bool canAttack, canBlock;
    
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isBlocking = false;
    private bool isStunned = false;

    [SerializeField] private float chasingRange = 2.5f;
    [SerializeField] private float attackRange = 0.7f;
    [Space]
    public float radiusAttack = 1f;
    public float attackOffset = 0f;

    [Space]
    [SerializeField] private float blockTimerAttackCooldown;
    [SerializeField] private float timeBlockingDamages = 1f;
    [SerializeField] private float projectileOffsetSpawn;
    [SerializeField] private GameObject blockProjectilePrefab;
    [SerializeField] private LayerMask playerBulletsLayer;

    private void Awake()
    {
        canAttack = true;
        canBlock = true;
    }

    void Update()
    {
        if (!isDead && !isStunned)
        {
            float distance = Vector2.Distance(player.transform.position, transform.position);

            if (!isChasing && distance <= chasingRange)
            {
                isChasing = true;
            }
            else if (isChasing && distance > chasingRange)
            {
                isChasing = false;
            }

            if (!isBlocking && canBlock)
            {
                if (Physics.CheckSphere(
                    (Vector2) transform.position + Vector2.right * -Mathf.Sign(transform.localScale.x), 
                    1f,
                    playerBulletsLayer))
                {
                    StartBlock();
                }
            }

            if (isChasing && !isAttacking && canAttack && !isBlocking && distance <= attackRange)
            {
                StartAttack();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isDead && isChasing && !isStunned && !isAttacking && !isBlocking)
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

    #region Take Damages

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
        if (isDead || bullet == null) return;
        
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

    #endregion

    #region Attack

    private void StartAttack()
    {
        canAttack = false;
        isAttacking = true;
        
        animator.SetTrigger("Attack");
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
        
        CooldownManager.instance.AddCooldown(1.5f, () => canAttack = true);
    }
    
    private void StartBlock()
    {
        canBlock = false;
        isBlocking = true;
        
        animator.SetTrigger("Block");
        CooldownManager.instance.AddCooldown(timeBlockingDamages, BlockAttackTrigger);
    }

    private void BlockAttackTrigger()
    {
        animator.SetTrigger("BlockAttack");
    }

    private void BlockAttack()
    {
        GameObject projectile = Instantiate(blockProjectilePrefab,
            transform.position + new Vector3(projectileOffsetSpawn, 0f, 0f) * Mathf.Sign(transform.localScale.x),
            Quaternion.identity);
        
        projectile.GetComponent<Fireball>().LaunchFireball(Vector2.right * Mathf.Sign(transform.localScale.x), 0, 3f);
    }

    private void EndBlock()
    {
        isBlocking = false;
        
        CooldownManager.instance.AddCooldown(blockTimerAttackCooldown, () => canBlock = true);
    }

    #endregion
    
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chasingRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + Vector2.right * Mathf.Sign(transform.localScale.x) * attackOffset, radiusAttack);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)transform.position + Vector2.right * Mathf.Sign(transform.localScale.x) * projectileOffsetSpawn, 0.5f);
    }
}