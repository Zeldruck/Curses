using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : Enemy
{
    private bool isChasing = false;

    [SerializeField] private float chasingRange = 2.5f;
    [SerializeField] private float attackRange = 0.7f;
    
    [Header("Attack")]
    private bool isAttacking = false;
    private bool canLaunchFireball = false;
    [SerializeField] private Transform fireballSpawnPoint;
    [SerializeField] private GameObject fireBallPrefab;
    [SerializeField] private float fireBallSpeed = 5f;

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
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

            if (isChasing && !isAttacking && distance <= attackRange && timerAttack <= 0f)
            {
                StartAttack();
            }
            else if (timerAttack > 0f)
            {
                timerAttack -= Time.deltaTime;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isDead && isChasing && !isAttacking)
        {
            direction = player.transform.position - transform.position;
            Flip();

            rb.MovePosition(rb.position + direction.normalized * stats.speed * Time.fixedDeltaTime);
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
    }

    private void CreateFireball()
    {
        StartCoroutine(IELaunchFireball());
    }

    private void LauchFireball()
    {
        canLaunchFireball = true;
    }

    private IEnumerator IELaunchFireball()
    {
        GameObject fireball = Instantiate(fireBallPrefab, fireballSpawnPoint.position, Quaternion.identity);

        while (!canLaunchFireball)
        {
            Vector3 dirAngle = (player.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(dirAngle.y, dirAngle.x) * Mathf.Rad2Deg;
            fireball.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            yield return null;

            if (isDead)
            {
                Destroy(fireball);
                canLaunchFireball = true;
            }
        }

        fireball.GetComponent<Rigidbody2D>().velocity = (player.transform.position - fireball.transform.position).normalized * fireBallSpeed;
        Destroy(fireball, 3f);
    }

    private void AttackEnd()
    {
        isAttacking = false;
        canLaunchFireball = false;
        timerAttack = stats.attackRate;
    }

    protected override IEnumerator Die()
    {
        //GetComponent<CapsuleCollider2D>().enabled = false;
        animator.SetTrigger("Death");

        return base.Die();
    }
    
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chasingRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
