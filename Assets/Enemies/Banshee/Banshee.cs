using System;
using System.Collections;
using UnityEngine;

public class Banshee : Enemy
{
    private bool isAggro = false;
    
    [SerializeField] private AudioClip screamSound;

    protected override void Start()
    {
        base.Start();
        
        animator.SetBool("IsWalking", true);
    }

    private void Update()
    {
        if (timerAttack > 0f)
            timerAttack -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            if (isAggro)
            {
                // Move toward the player, go throught walls and attack
                direction = player.transform.position - transform.position;
                Flip();

                rb.velocity = direction.normalized * (stats.speed * 1.5f) * Time.fixedDeltaTime;
            }
            else
            {
                // Move with pathfinding, don't go throught walls and don't attack
            }
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (!isAggro)
        {
            isAggro = true;
            animator.SetBool("IsWalking", false);
            // Scream sound
            GetComponent<Collider2D>().isTrigger = true;
        }
    }

    public override void TakeDamage(int damage, GameObject bullet)
    {
        base.TakeDamage(damage, bullet);
        
        if (!isAggro)
        {
            isAggro = true;
            animator.SetBool("IsWalking", false);
            // Scream sound
            GetComponent<Collider2D>().isTrigger = true;
        }
    }

    protected override IEnumerator Die()
    {
        GetComponent<Collider2D>().enabled = false;
        animator.SetTrigger("Death");

        return base.Die();
    }

    private void Despawn()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }
}
